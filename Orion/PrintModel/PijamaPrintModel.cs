#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Office.Interop.Excel;
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;
using Orion.Properties;
using Orion.Servicios;
using Orion.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.PrintModel {

	public static class PijamaPrintModel {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private static ConvertidorNumeroGraficoPijama cnvNumGraficoPijama = new ConvertidorNumeroGraficoPijama();
		private static ConvertidorHora cnvHora = new ConvertidorHora();
		private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
		private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
		private static ConvertidorDecimalEuro cnvDecimalEuro = new ConvertidorDecimalEuro();
		private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
		private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();
		private static ConvertidorColorDiaPijama cnvColorDiaPijama = new ConvertidorColorDiaPijama();
		private static ConvertidorColorDiaSinGraficos cnvColorDiaSinGraficos = new ConvertidorColorDiaSinGraficos();

		private static string rutaPlantillaPijama = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Pijama.xlsx");

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================

		private static void LlenarExcelPijama(int fila, Pijama.HojaPijama pijama, ref string[,] datos, ref System.Drawing.Color[,] colores) {

			// Conductor y Mes
			datos[fila - 1, 0] = pijama.TextoTrabajador;
			datos[fila - 1, 14] = pijama.TextoMesActual;

			// Encabezados Pijama
			datos[fila, 0] = "Día";
			datos[fila, 1] = "Gráfico";
			datos[fila, 2] = "Horas";
			datos[fila, 3] = "Acumuladas";
			datos[fila, 4] = "Nocturnas";
			datos[fila, 5] = "Ex.Jornada";
			datos[fila, 6] = "Desayuno";
			datos[fila, 7] = "Comida";
			datos[fila, 8] = "Cena";
			datos[fila, 9] = "Plus Cena";
			datos[fila, 10] = "Total Dietas";
			datos[fila, 11] = "Menor Des.";
			datos[fila, 12] = "Paqueteria";
			datos[fila, 13] = "Limpieza";
			datos[fila, 14] = "Otros";

			// Lista de días
			foreach (Pijama.DiaPijama dia in pijama.ListaDias) {
				datos[fila + dia.Dia, 0] = dia.Dia.ToString("00");
				datos[fila + dia.Dia, 1] = (string)cnvNumGraficoPijama.Convert(dia.ComboGrafico, null, null, null);
				datos[fila + dia.Dia, 2] = (string)cnvHora.Convert(dia.GraficoTrabajado.Trabajadas, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 3] = (string)cnvHora.Convert(dia.GraficoTrabajado.Acumuladas, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 4] = (string)cnvHora.Convert(dia.GraficoTrabajado.Nocturnas, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 5] = (string)cnvHora.Convert(dia.ExcesoJornada, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 6] = (string)cnvDecimal.Convert(dia.GraficoTrabajado.Desayuno, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 7] = (string)cnvDecimal.Convert(dia.GraficoTrabajado.Comida, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 8] = (string)cnvDecimal.Convert(dia.GraficoTrabajado.Cena, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 9] = (string)cnvDecimal.Convert(dia.GraficoTrabajado.PlusCena, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 10] = (string)cnvDecimal.Convert(dia.TotalDietas, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 11] = (string)cnvDecimalEuro.Convert(dia.PlusMenorDescanso, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 12] = (string)cnvDecimalEuro.Convert(dia.PlusPaqueteria, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 13] = (string)cnvDecimalEuro.Convert(dia.PlusLimpieza, null, VerValores.NoCeros, null);
				datos[fila + dia.Dia, 14] = (string)cnvDecimalEuro.Convert(dia.OtrosPluses, null, VerValores.NoCeros, null);

				//DateTime fecha = new DateTime(pijama.Fecha.Year, pijama.Fecha.Month, dia.dia);
				object[] valores2 = new object[] { dia.ComboGrafico, pijama.Fecha, dia.Dia };
				System.Windows.Media.Color c1 = ((System.Windows.Media.SolidColorBrush)cnvColorDiaSinGraficos.Convert(dia.DiaFecha, null, null, null)).Color;
				System.Windows.Media.Color c2 = ((System.Windows.Media.SolidColorBrush)cnvColorDiaPijama.Convert(valores2, null, null, null)).Color;
				colores[dia.Dia - 1, 0] = System.Drawing.Color.FromArgb(255, c1.R, c1.G, c1.B);
				colores[dia.Dia - 1, 1] = System.Drawing.Color.FromArgb(255, c2.R, c2.G, c2.B);

			}

			// Encabezados resumen
			datos[fila, 16] = "Horas Mes Actual";
			datos[fila + 5, 16] = "Días Mes Actual";
			datos[fila + 13, 16] = "Fines de Semana Mes Actual";
			datos[fila + 19, 16] = "Dietas y Pluses Mes Actual";
			datos[fila + 27, 16] = "Resumen Hasta Mes Actual";

			// Horas Mes Actual
			datos[fila + 1, 16] = "Trabajadas";
			datos[fila + 2, 16] = (string)cnvSuperHoraMixta.Convert(pijama.Trabajadas, null, null, null);
			datos[fila + 1, 20] = "Acumuladas";
			datos[fila + 2, 20] = (string)cnvSuperHoraMixta.Convert(pijama.Acumuladas, null, null, null);
			datos[fila + 1, 24] = "Nocturnas";
			datos[fila + 2, 24] = (string)cnvSuperHoraMixta.Convert(pijama.Nocturnas, null, null, null);
			datos[fila + 3, 16] = "Cobradas";
			datos[fila + 4, 16] = (string)cnvSuperHoraMixta.Convert(pijama.HorasCobradas, null, null, null);
			datos[fila + 3, 20] = "Ex. Jornada";
			datos[fila + 4, 20] = (string)cnvSuperHoraMixta.Convert(pijama.ExcesoJornada, null, null, null);
			datos[fila + 3, 24] = "Otras Horas";
			datos[fila + 4, 24] = (string)cnvSuperHoraMixta.Convert(pijama.OtrasHoras, null, null, null);

			// Días Mes Actual
			datos[fila + 6, 16] = "Trabajo";
			datos[fila + 7, 16] = pijama.Trabajo.ToString("00");
			datos[fila + 6, 18] = "J-D";
			datos[fila + 7, 18] = pijama.Descanso.ToString("00");
			datos[fila + 6, 20] = "O-V";
			datos[fila + 7, 20] = pijama.Vacaciones.ToString("00");
			datos[fila + 6, 22] = "DND";
			datos[fila + 7, 22] = pijama.DescansosNoDisfrutados.ToString("00");
			datos[fila + 6, 24] = "Trabajo (JD)";
			datos[fila + 7, 24] = pijama.TrabajoEnDescanso.ToString("00");
			if (pijama.NoEsFijo) {
				datos[fila + 8, 16] = pijama.DiasComputoTrabajo.ToString("00.##");
				datos[fila + 8, 18] = pijama.DiasComputoDescanso.ToString("00.##");
				datos[fila + 8, 20] = pijama.DiasComputoVacaciones.ToString("00.##");
				datos[fila + 8, 22] = pijama.TextoComputoEventuales;
			}

			datos[fila + 9, 16] = "FN";
			datos[fila + 10, 16] = pijama.DescansoEnFinde.ToString("00");
			datos[fila + 9, 18] = "E";
			datos[fila + 10, 18] = pijama.Enfermo.ToString("00");
			datos[fila + 9, 20] = "DS";
			datos[fila + 10, 20] = pijama.DescansoSuelto.ToString("00");
			datos[fila + 9, 22] = "DC";
			datos[fila + 10, 22] = pijama.DescansoCompensatorio.ToString("00");
			datos[fila + 9, 24] = "PER";
			datos[fila + 10, 24] = pijama.Permiso.ToString("00");
			datos[fila + 9, 26] = "F6";
			datos[fila + 10, 26] = pijama.LibreDisposicionF6.ToString("00");

			if (pijama.VerComite) {
				datos[fila + 11, 16] = "Días Comité: " + pijama.Comite.ToString("00");
				datos[fila + 11, 22] = "En JD: " + pijama.ComiteEnDescanso.ToString("00");
				datos[fila + 11, 25] = "En DC: " + pijama.ComiteEnDC.ToString("00");
			}

			datos[fila + 12, 16] = $"Total Días: {pijama.DiasActivo:00} de {pijama.DiasMes:00}";

			// Fines de Semana Mes Actual
			datos[fila + 15, 16] = "Descanso";
			datos[fila + 16, 16] = "Trabajo";
			datos[fila + 17, 16] = "Pluses";
			datos[fila + 14, 19] = "Sábados";
			datos[fila + 15, 19] = pijama.SabadosDescansados.ToString("00");
			datos[fila + 16, 19] = pijama.SabadosTrabajados.ToString("00");
			datos[fila + 17, 19] = pijama.PlusSabados.ToString("0.00 €");
			datos[fila + 14, 22] = "Domingos";
			datos[fila + 15, 22] = pijama.DomingosDescansados.ToString("00");
			datos[fila + 16, 22] = pijama.DomingosTrabajados.ToString("00");
			datos[fila + 17, 22] = pijama.PlusDomingos.ToString("0.00 €");
			datos[fila + 14, 25] = "Festivos";
			datos[fila + 15, 25] = pijama.FestivosDescansados.ToString("00");
			datos[fila + 16, 25] = pijama.FestivosTrabajados.ToString("00");
			datos[fila + 17, 25] = pijama.PlusFestivos.ToString("0.00 €");

			datos[fila + 18, 16] = $"Fines de Semana Completos: {pijama.FindesCompletos:0.00}";

			// Dietas y Pluses Mes Actual
			datos[fila + 20, 16] = "Desayuno";
			datos[fila + 21, 16] = pijama.DietaDesayuno.ToString("0.00");
			datos[fila + 20, 19] = "Comida";
			datos[fila + 21, 19] = pijama.DietaComida.ToString("0.00");
			datos[fila + 20, 22] = "Cena";
			datos[fila + 21, 22] = pijama.DietaCena.ToString("0.00");
			datos[fila + 20, 25] = "P.Cena";
			datos[fila + 21, 25] = pijama.DietaPlusCena.ToString("0.00");

			datos[fila + 22, 16] = "Menor D.";
			datos[fila + 23, 16] = pijama.PlusMenorDescanso.ToString("0.00 €");
			datos[fila + 22, 19] = "Limpieza";
			datos[fila + 23, 19] = pijama.PlusLimpieza.ToString("0.00 €");
			datos[fila + 22, 22] = "Paquet.";
			datos[fila + 23, 22] = pijama.PlusPaqueteria.ToString("0.00 €");
			datos[fila + 22, 25] = "Otros";
			datos[fila + 23, 25] = pijama.OtrosPluses.ToString("0.00 €");

			datos[fila + 24, 16] = "Total Findes";
			datos[fila + 25, 16] = pijama.ImporteTotalFindes.ToString("0.00 €");
			datos[fila + 24, 20] = "Total Dietas";
			datos[fila + 25, 20] = $"{pijama.TotalDietas:0.00;} ({pijama.ImporteTotalDietas:0.00 €})";
			datos[fila + 24, 24] = "Total Pluses";
			datos[fila + 25, 24] = pijama.ImporteTotalPluses.ToString("0.00 €");

			datos[fila + 26, 16] = $"Importe Total: {pijama.ImporteTotal:0.00 €}";

			// Resumen Hasta Mes Actual
			datos[fila + 28, 16] = "DNDs Pendientes";
			datos[fila + 29, 16] = pijama.DNDsPendientesHastaMes.ToString("00");
			datos[fila + 28, 20] = "DCs Pendientes";
			datos[fila + 29, 20] = pijama.DCsPendientesHastaMes.ToString("00");
			datos[fila + 28, 24] = "H. Acumuladas";
			datos[fila + 29, 24] = (string)cnvSuperHoraMixta.Convert(pijama.AcumuladasHastaMes, null, null, null);

			datos[fila + 30, 16] = "DCs Generados: " + pijama.DCsGeneradosHastaMes.ToString("0.00");







		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS (ITEXT 7)
		// ====================================================================================================

		private static Table GetTablaPijama(IEnumerable dias) {
			
			// Fuente a utilizar en la tabla.
			PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
			// Estilo de la tabla.
			iText.Layout.Style estiloTabla = new iText.Layout.Style();
			estiloTabla.SetTextAlignment(TextAlignment.CENTER)
					   .SetMargins(0, 0, 0, 0)
					   .SetPaddings(0, 0, 0, 0)
					   .SetWidth(UnitValue.CreatePercentValue(100))
					   .SetFont(arial)
					   .SetFontSize(7);
			// Estilo de las celdas de encabezado.
			iText.Layout.Style estiloEncabezados = new iText.Layout.Style();
			estiloEncabezados.SetBackgroundColor(new DeviceRgb(102,153,255))
							 .SetBold();
			// Estilo de las celdas de dia.
			iText.Layout.Style estiloDia = new iText.Layout.Style();
			estiloDia.SetBorderLeft(new SolidBorder(1))
					 .SetBorderRight(new SolidBorder(1))
					 .SetBold();
			// Creamos la tabla que tendrá la Hoja Pijama.
			float[] columnas = new float[] { 4f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f };
			Table tabla = new Table(UnitValue.CreatePercentArray(columnas));
			// Asignamos el estilo a la tabla.
			tabla.AddStyle(estiloTabla);
			// Añadimos las celdas de encabezado.
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Día")).AddStyle(estiloEncabezados).SetBorder(new SolidBorder(1)));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Gráfico")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Horas")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Ex.Jornada")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Plus Cena")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Menor Des.")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Paquetería")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezados));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Otros")).AddStyle(estiloEncabezados));
			// Añadimos las celdas de cada día de la lista.
			foreach (Pijama.DiaPijama dia in dias) {
				if (dia.Dia > DateTime.DaysInMonth(dia.DiaFecha.Year, dia.DiaFecha.Month)) continue;
				// Establecemos el color de fondo de las filas
				iText.Layout.Style estiloCeldas = new iText.Layout.Style().SetBackgroundColor(ColorConstants.WHITE);
				if (dia.Dia % 2 == 0) estiloCeldas.SetBackgroundColor(new DeviceRgb(204, 236, 255));
				// Deducimos los colores de la primera y segunda columnas.
				System.Windows.Media.Color c1 = Colors.Black;
				System.Windows.Media.Color c2 = Colors.Black;
				if (dia.Grafico != 0) {
					object[] valores2 = new object[] { dia.ComboGrafico, dia.DiaFecha, dia.Dia };
					c1 = ((System.Windows.Media.SolidColorBrush)cnvColorDiaSinGraficos.Convert(dia.DiaFecha, null, null, null)).Color;
					c2 = ((System.Windows.Media.SolidColorBrush)cnvColorDiaPijama.Convert(valores2, null, null, null)).Color;
				}
				// Insertamos cada celda.
				tabla.AddCell(new Cell().Add(new Paragraph(dia.Dia.ToString("00")).SetFontColor(new DeviceRgb(c1.R, c1.G, c1.B))).AddStyle(estiloDia).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvNumGraficoPijama.Convert(dia.ComboGrafico, null, null, null)).SetFontColor(new DeviceRgb(c2.R, c2.G, c2.B))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.GraficoTrabajado.Trabajadas, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.GraficoTrabajado.Acumuladas, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.GraficoTrabajado.Nocturnas, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.ExcesoJornada, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Desayuno, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Comida, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Cena, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.PlusCena, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.TotalDietas, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusMenorDescanso, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusPaqueteria, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusLimpieza, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
				tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.OtrosPluses, null, VerValores.NoCeros, null))).AddStyle(estiloCeldas));
			}
			// Añadimos el borde a la tabla y al encabezado
			tabla.SetBorder(new SolidBorder(1));
			tabla.GetHeader().SetBorder(new SolidBorder(1));
			// Devolvemos la tabla.
			return tabla;
		}


		private static Cell GetResumenPijama(Pijama.HojaPijama pijama) {

			// Definimos la celda a devolver.
			Cell celda = new Cell();
			// Fuente a utilizar en la tabla.
			PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
			// Estilo Tabla
			iText.Layout.Style estiloTabla = new iText.Layout.Style().SetTextAlignment(TextAlignment.CENTER)
																	 .SetMargins(0, 0, 2, 0)
																	 .SetPaddings(1.5f, 0, 1.5f, 0)
																	 .SetBorder(new SolidBorder(1))
																	 .SetFont(arial)
																	 .SetWidth(UnitValue.CreatePercentValue(100))
																	 .SetFontSize(7);
			// Estilo Seccion
			iText.Layout.Style estiloSeccion = new iText.Layout.Style().SetTextAlignment(TextAlignment.LEFT)
																	   .SetFontColor(new DeviceRgb(192,0,0))
																	   .SetMargin(0)
																	   .SetBold()
																	   .SetFont(arial)
																	   .SetFontSize(9);
			// Estilo Encabezado
			iText.Layout.Style estiloEncabezado = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(153,204,255))
																		  .SetBold();
			// Estilo Resumen
			iText.Layout.Style estiloResumen = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(204,236,255))
																	   .SetPaddings(1.5f, 0, 1.5f, 0)
																	   .SetFont(arial)
																	   .SetFontSize(8);
			// Estilo Eventuales
			iText.Layout.Style estiloEventuales = new iText.Layout.Style().SetMargins(0, 0, 0, 0)
																		  .SetFontColor(new DeviceRgb(0, 112, 192));
			// Definimos la tabla y párrafo a usar.
			Table tabla;
			Paragraph parrafo;
			// SECCION HORAS MES ACTUAL
			celda.Add(new Paragraph("Horas Mes Actual").AddStyle(estiloSeccion));
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
			tabla.AddStyle(estiloTabla);
			tabla.AddCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezado));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Trabajadas, null, null, null));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Acumuladas, null, null, null));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Nocturnas, null, null, null));
			tabla.AddCell(new Cell().Add(new Paragraph("Cobradas")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Ex. Jornada")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Otras Horas")).AddStyle(estiloEncabezado));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.HorasCobradas, null, null, null));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.ExcesoJornada, null, null, null));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.OtrasHoras, null, null, null));
			celda.Add(tabla);
			//SECCION DÍAS MES ACTUAL
			celda.Add(new Paragraph("Días Mes Actual").AddStyle(estiloSeccion));
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 16.6666f, 16.6666f, 16.6666f, 16.6666f, 16.6666f, 16.6666f }));
			tabla.AddStyle(estiloTabla);
			tabla.AddCell(new Cell().Add(new Paragraph("Trabajo")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("J-D")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("O-V")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("DND")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell(1,2).Add(new Paragraph("Trabajo (JD)")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.Trabajo.ToString("00"));
			tabla.AddCell(pijama.Descanso.ToString("00"));
			tabla.AddCell(pijama.Vacaciones.ToString("00"));
			tabla.AddCell(pijama.DescansosNoDisfrutados.ToString("00"));
			tabla.AddCell(new Cell(1, 2).Add(new Paragraph(pijama.TrabajoEnDescanso.ToString("00"))));
			if (pijama.NoEsFijo) {
				tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoTrabajo.ToString("00.##"))).AddStyle(estiloEventuales));
				tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoDescanso.ToString("00.##"))).AddStyle(estiloEventuales));
				tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoVacaciones.ToString("00.##"))).AddStyle(estiloEventuales));
				tabla.AddCell(new Cell(1, 3).Add(new Paragraph(pijama.TextoComputoEventuales)).AddStyle(estiloEventuales));
			}
			tabla.AddCell(new Cell().Add(new Paragraph("FN")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("E")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("DS")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("DC")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("PER")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("F6")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.DescansoEnFinde.ToString("00"));
			tabla.AddCell(pijama.Enfermo.ToString("00"));
			tabla.AddCell(pijama.DescansoSuelto.ToString("00"));
			tabla.AddCell(pijama.DescansoCompensatorio.ToString("00"));
			tabla.AddCell(pijama.Permiso.ToString("00"));
			tabla.AddCell(pijama.LibreDisposicionF6.ToString("00"));
			if (pijama.VerComite) {
				tabla.AddCell(new Cell(1, 2).Add(new Paragraph("Días Comité: " + pijama.Comite.ToString("00"))));
				tabla.AddCell(new Cell(1, 2).Add(new Paragraph("En JD: " + pijama.ComiteEnDescanso.ToString("00"))));
				tabla.AddCell(new Cell(1, 2).Add(new Paragraph("En DC: " + pijama.ComiteEnDC.ToString("00"))));
			}
			parrafo = new Paragraph();
			parrafo.Add(new Text("Total días: ").SetBold());
			parrafo.Add(new Text($"{pijama.DiasActivo:00} de {pijama.DiasMes:00}"));
			tabla.AddCell(new Cell(1, 6).Add(parrafo).AddStyle(estiloResumen));
			celda.Add(tabla);
			//SECCION FINES DE SEMANA MES ACTUAL
			celda.Add(new Paragraph("Fines de Semana Mes Actual").AddStyle(estiloSeccion));
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }));
			tabla.AddStyle(estiloTabla);
			tabla.AddCell(new Cell().Add(new Paragraph("")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Sábados")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Domingos")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Festivos")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Descanso")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.SabadosDescansados.ToString("00"));
			tabla.AddCell(pijama.DomingosDescansados.ToString("00"));
			tabla.AddCell(pijama.FestivosDescansados.ToString("00"));
			tabla.AddCell(new Cell().Add(new Paragraph("Trabajo")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.SabadosTrabajados.ToString("00"));
			tabla.AddCell(pijama.DomingosTrabajados.ToString("00"));
			tabla.AddCell(pijama.FestivosTrabajados.ToString("00"));
			tabla.AddCell(new Cell().Add(new Paragraph("Pluses")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.PlusSabados.ToString("0.00 €"));
			tabla.AddCell(pijama.PlusDomingos.ToString("0.00 €"));
			tabla.AddCell(pijama.PlusFestivos.ToString("0.00 €"));
			parrafo = new Paragraph();
			parrafo.Add(new Text("Fines de Semana Completos: ").SetBold());
			parrafo.Add(new Text($"{pijama.FindesCompletos:0.00}"));
			tabla.AddCell(new Cell(1, 4).Add(parrafo).AddStyle(estiloResumen));
			celda.Add(tabla);
			//SECCION DIETAS Y PLUSES MES ACTUAL
			celda.Add(new Paragraph("Dietas y Pluses Mes Actual").AddStyle(estiloSeccion));
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }));
			tabla.AddStyle(estiloTabla);
			tabla.AddCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("P. Cena")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.DietaDesayuno.ToString("0.00"));
			tabla.AddCell(pijama.DietaComida.ToString("0.00"));
			tabla.AddCell(pijama.DietaCena.ToString("0.00"));
			tabla.AddCell(pijama.DietaPlusCena.ToString("0.00"));
			tabla.AddCell(new Cell().Add(new Paragraph("Menor D.")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Paquet.")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Otros")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.PlusMenorDescanso.ToString("0.00 €"));
			tabla.AddCell(pijama.PlusLimpieza.ToString("0.00 €"));
			tabla.AddCell(pijama.PlusPaqueteria.ToString("0.00 €"));
			tabla.AddCell(pijama.OtrosPluses.ToString("0.00 €"));
			tabla.SetBorderBottom(new SolidBorder(0));
			celda.Add(tabla);
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
			tabla.AddStyle(estiloTabla).SetMarginTop(-3);
			tabla.AddCell(new Cell().Add(new Paragraph("Total Findes")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("Total Pluses")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.ImporteTotalFindes.ToString("0.00 €"));
			tabla.AddCell($"{pijama.TotalDietas:0.00} ({pijama.ImporteTotalDietas:0.00 €})");
			tabla.AddCell(pijama.ImporteTotalPluses.ToString("0.00 €"));
			parrafo = new Paragraph();
			parrafo.Add(new Text("Importe Total: ").SetBold());
			parrafo.Add(new Text($"{pijama.ImporteTotal:0.00 €}"));
			tabla.AddCell(new Cell(1, 3).Add(parrafo).AddStyle(estiloResumen));
			tabla.SetBorderTop(new SolidBorder(0));
			celda.Add(tabla);
			//SECCION RESUMEN HASTA MES ACTUAL
			celda.Add(new Paragraph("Resumen Hasta Mes Actual").AddStyle(estiloSeccion));
			tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
			tabla.AddStyle(estiloTabla);
			tabla.AddCell(new Cell().Add(new Paragraph("DNDs Pendientes")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("DCs Pendientes")).AddStyle(estiloEncabezado));
			tabla.AddCell(new Cell().Add(new Paragraph("H. Acumuladas")).AddStyle(estiloEncabezado));
			tabla.AddCell(pijama.DNDsPendientesHastaMes.ToString("00"));
			tabla.AddCell(pijama.DCsPendientesHastaMes.ToString("00"));
			tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.AcumuladasHastaMes, null, null, null));
			parrafo = new Paragraph();
			parrafo.Add(new Text("DCs Generados: ").SetBold());
			parrafo.Add(new Text($"{pijama.DCsGeneradosHastaMes:0.00}"));
			tabla.AddCell(new Cell(1, 3).Add(parrafo).AddStyle(estiloResumen));
			celda.Add(tabla);
			// Devolvemos la celda.
			return celda;

		}


		private static Table GetHojaPijama(Pijama.HojaPijama pijama) {

			// Fuente a utilizar en la tabla.
			PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
			// Estilo de la tabla.
			iText.Layout.Style estiloTabla = new iText.Layout.Style();
			estiloTabla.SetWidth(UnitValue.CreatePercentValue(100))
					   .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
					   .SetFont(arial)
					   .SetMargin(0)
					   .SetPadding(0);
			iText.Layout.Style estiloCelda = new iText.Layout.Style();
			estiloCelda.SetBorder(iText.Layout.Borders.Border.NO_BORDER)
					   .SetMargin(0)
					   .SetPadding(0)
					   .SetFont(arial)
					   .SetFontSize(16);
			// Creamos la tabla
			Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 50, 25, 25 }));
			tabla.AddStyle(estiloTabla);

			Cell celda;
			// Añadimos al trabajador
			celda = new Cell();
			celda.Add(new Paragraph(pijama.TextoTrabajador).SetMargin(0));
			celda.AddStyle(estiloCelda);
			celda.SetPaddings(0, 0, 0, 2);
			celda.SetTextAlignment(TextAlignment.LEFT);
            celda.SetVerticalAlignment(VerticalAlignment.BOTTOM);
			tabla.AddCell(celda);
			// Añadimos la fecha
			celda = new Cell();
			celda.Add(new Paragraph(pijama.TextoMesActual).SetMargin(0));
			celda.AddStyle(estiloCelda);
			celda.SetPaddings(0, 10, 0, 0);
			celda.SetTextAlignment(TextAlignment.RIGHT);
            celda.SetVerticalAlignment(VerticalAlignment.BOTTOM);
            tabla.AddCell(celda);
			// Añadimos el logo del sindicato
			celda = new Cell();
			celda.AddStyle(estiloCelda);
            tabla.AddCell(celda);
			// Añadimos la tabla
			celda = new Cell(1, 2);
			celda.AddStyle(estiloCelda);
			celda.SetPaddings(0, 8, 0, 0);
			celda.Add(GetTablaPijama(pijama.ListaDias));
			tabla.AddCell(celda);
			// Añadimos el resumen
			celda = GetResumenPijama(pijama);
			celda.AddStyle(estiloCelda);
			tabla.AddCell(celda);
			// Devolvemos la tabla
			return tabla;
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public static async Task CrearPijamaEnPdf(Workbook Libro, Pijama.HojaPijama pijama) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Definimos la matriz de colores para ponerlos.
				string[,] datos = new string[33, 28];
				System.Drawing.Color[,] colores = new System.Drawing.Color[31, 2];
				// Llenamos la hoja con los datos
				LlenarExcelPijama(1, pijama, ref datos, ref colores);
				// Coloreamos la plantilla
				for (int m = 1; m <= 31; m++) {
					double valor = m / 31d * 100;
					App.Global.ValorBarraProgreso = valor;
					if (colores[m - 1, 0] != null) Hoja.Cells[2 + m, 1].Font.Color = colores[m - 1, 0];
					if (colores[m - 1, 1] != null) Hoja.Cells[2 + m, 2].Font.Color = colores[m - 1, 1];
				}
				// Pegamos los datos
				Range rango = Hoja.Range["A1:AB33"];
				rango.Value = datos;
				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = rango.Address;
			});
		}


		public static async Task CrearTodosPijamasEnPdf(Workbook Libro, ListCollectionView ListaCalendarios) {

			await Task.Run(() => {
				Worksheet Hoja = Libro.Worksheets[1];
				int fila = 1;
				string[,] datos = new string[ListaCalendarios.Count * 33, 28];
				System.Drawing.Color[,] colores = new System.Drawing.Color[31, 2];
				double num = 1;
				foreach (Object obj in ListaCalendarios) {
					double valor = num / ListaCalendarios.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					num++;
					Calendario calendario = obj as Calendario;
					if (calendario == null) continue;
					Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(calendario, new MensajesServicio());
					if (fila > 1) {
						Hoja.Range["A1:AB34"].Copy(Hoja.Cells[fila, 1]);
						Hoja.Range[fila.ToString() + ":" + fila.ToString()].RowHeight = 35;
						Hoja.Range[(fila + 1).ToString() + ":" + (fila + 33).ToString()].RowHeight = 25;
					}
					LlenarExcelPijama(fila, hojapijama, ref datos, ref colores);
					for (int m = 1; m <= 31; m++) {
						if (colores[m - 1, 0] != null) Hoja.Cells[fila + 1 + m, 1].Font.Color = colores[m - 1, 0];
						if (colores[m - 1, 1] != null) Hoja.Cells[fila + 1 + m, 2].Font.Color = colores[m - 1, 1];
					}
					Hoja.HPageBreaks.Add(Hoja.Cells[fila + 33, 1]);
					fila += 33;
				}
				// Definimos el rango a llenar y lo llenamos
				Range rango = Hoja.Range[Hoja.Cells[1, 1], Hoja.Cells[fila - 1, 28]];
				rango.Value = datos;
				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = rango.Address;
			});

		}


		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS PDF (ITEXT 7)
		// ====================================================================================================

		public static async Task CrearPijamaEnPdf_7(Document doc, Pijama.HojaPijama pijama) {

			await Task.Run(() => {

                float anchoHoja = PageSize.A4.Rotate().GetWidth();
                float altoHoja = PageSize.A4.Rotate().GetHeight();
                iText.Layout.Element.Image imagen = InformesServicio.GetLogoSindicato();
                if (imagen != null) {
                    imagen.ScaleToFit(anchoHoja / 5, 30);
                    float ancho = imagen.GetImageScaledWidth();
                    float alto = imagen.GetImageScaledHeight();
                    imagen.SetFixedPosition(anchoHoja - 25 - ancho, altoHoja - 25 - alto);
                    doc.Add(imagen);
                }
                // Añadimos la tabla al documento
                doc.Add(GetHojaPijama(pijama));
			});
		}


		[Obsolete("Este método funciona, pero es mucho menos eficiente que el que está.")]
		public static async Task CrearTodosPijamasEnPdf_72(Document doc, ListCollectionView listaCalendarios) {

			await Task.Run(() => {
				double num = 1;
				foreach (Object obj in listaCalendarios) {
					double valor = num / listaCalendarios.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					Calendario calendario = obj as Calendario;
					if (calendario == null) continue;
					Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(calendario, new MensajesServicio());
					if (num > 1) doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
					num++;
					doc.Add(GetHojaPijama(hojapijama));
					hojapijama = null;
				}
			});
		}


		public static async Task CrearTodosPijamasEnPdf_7(Document doc, ListCollectionView listaCalendarios) {

			await Task.Run(() => {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Recopilando...");
				double num = 1;
				List<Pijama.HojaPijama> Lista = new List<Pijama.HojaPijama>();
				foreach (Object obj in listaCalendarios) {
					double valor = num / listaCalendarios.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					Calendario calendario = obj as Calendario;
					if (calendario == null) continue;
					Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(calendario, new MensajesServicio());
					Lista.Add(hojapijama);
					num++;
				}
                // Definimos el logo del sindicato.
                float anchoHoja = PageSize.A4.Rotate().GetWidth();
                float altoHoja = PageSize.A4.Rotate().GetHeight();
                iText.Layout.Element.Image imagen = InformesServicio.GetLogoSindicato();
                if (imagen != null) {
                    imagen.ScaleToFit(anchoHoja / 5, 30);
                    float ancho = imagen.GetImageScaledWidth();
                    float alto = imagen.GetImageScaledHeight();
                    imagen.SetFixedPosition(anchoHoja - 25 - ancho, altoHoja - 25 - alto);
                }
                // Creamos una hoja por cada pijama.
                num = 1;
				App.Global.IniciarProgreso("Creando PDF...");
				foreach(Pijama.HojaPijama pijama in Lista) {
					double valor = num / Lista.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					if (num > 1) doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
					num++;
					doc.Add(GetHojaPijama(pijama));
                    if (imagen != null) doc.Add(imagen);
				}
			});
		}


        public static async Task CrearReclamacionEnPdf(Document doc, DateTime fecha, Conductor conductor) {

            await Task.Run(() => {

                // Iniciamos la creación del PDF.
                App.Global.IniciarProgreso("Creando PDF...");
                float anchoHoja = PageSize.A4.GetWidth();
                float altoHoja = PageSize.A4.GetHeight();
                PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
                doc.SetFont(arial);
                doc.SetFontSize(12);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc.GetPdfDocument(), true);

                // Añadimos el texto inicial
                Paragraph parrafo = new Paragraph();
                parrafo.SetFontSize(26).SetBold();
                parrafo.SetMarginTop(4);
                parrafo.SetFixedLeading(27);
                parrafo.Add("RECLAMACIÓN\nDE CONCEPTOS");
                doc.Add(parrafo);

                // Añadimos el centro de trabajo y el conductor
                parrafo = new Paragraph();
                parrafo.SetFontSize(16);
                parrafo.SetMarginTop(12);
                parrafo.Add(new Text($"Centro: ").SetBold());
                parrafo.Add($"{App.Global.CentroActual.ToString().ToUpper()}");
                parrafo.Add("\n");
                parrafo.Add(new Text($"Trabajador/a: ").SetBold());
                parrafo.Add($"{conductor.Apellidos}, {conductor.Nombre} ({conductor.Id:000})");
                doc.Add(parrafo);

                // ==================================================
                // TABLA
                // ==================================================
                // Estilos
                iText.Layout.Style estiloCabecera = new iText.Layout.Style();
                estiloCabecera.SetFontSize(11).SetBold();
                estiloCabecera.SetTextAlignment(TextAlignment.CENTER);
                estiloCabecera.SetBackgroundColor(new DeviceRgb(255, 80, 80));
                estiloCabecera.SetBorder(new SolidBorder(1));
                iText.Layout.Style estiloBordes = new iText.Layout.Style();
                estiloBordes.SetBorderLeft(new SolidBorder(1));
                estiloBordes.SetBorderRight(new SolidBorder(1));
                // Creación de tabla
                Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 1, 1 }));
                tabla.SetWidth(UnitValue.CreatePercentValue(100));
                tabla.SetFontSize(10);
                tabla.SetMargins(40, 0, 0, 0);
                tabla.SetPadding(0);
                // Añadimos los encabezados.
                Cell celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("CONCEPTOS A RECLAMAR"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("En PIJAMA"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("REAL"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("DIFERENCIA"));
                tabla.AddHeaderCell(celda);
                // Añadimos las filas.
                for (int f = 1; f <= 17; f++) {
                    for (int c = 1; c <= 4; c++) {
                        App.Global.ValorBarraProgreso = 5.8;
                        celda = new Cell();
                        if (f % 2 == 0) celda.SetBackgroundColor(new DeviceRgb(221, 221, 221));
                        celda.SetHeight(16);
                        celda.AddStyle(estiloBordes);
                        celda.SetNextRenderer(new PdfServicio.TextFieldRenderer(celda, $"Fila{f:00}Columna{c}", 2, 10, PdfFormField.ALIGN_CENTER));
                        tabla.AddCell(celda);
                    }
                }
                // Dibujamos los bordes de la tabla.
                tabla.SetBorder(new SolidBorder(1));
                // Añadimos la tabla al documento.
                doc.Add(tabla);

                // Añadimos el logo del sindicato.
                iText.Layout.Element.Image logoSindicato = InformesServicio.GetLogoSindicato();
                if (logoSindicato != null) {
                    logoSindicato.ScaleToFit(anchoHoja / 3, 75);
                    logoSindicato.SetFixedPosition(anchoHoja - 40 - logoSindicato.GetImageScaledWidth(), altoHoja - 40 - logoSindicato.GetImageScaledHeight());
                    doc.Add(logoSindicato);
                }

                // Añadimos la fecha
                parrafo = new Paragraph($"{fecha:MMMM yyyy}".ToUpper());
                parrafo.SetFontSize(14);
                parrafo.SetFixedPosition(50, 640, (anchoHoja - 80) / 2);
                doc.Add(parrafo);

                // Añadimos el número de reclamación
                iText.Kernel.Geom.Rectangle cuadroNumReclamacion = new iText.Kernel.Geom.Rectangle(40 + ((anchoHoja - 80) / 2), 642, ((anchoHoja - 80) / 2) - 10, 20);
                string numReclamacion = $"Nº Reclamación: {fecha:yyyyMM}{conductor.Id:000}/01";
                PdfTextFormField campoNumReclamacion = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroNumReclamacion, "NumeroReclamacion", numReclamacion);
                campoNumReclamacion.SetJustification(PdfFormField.ALIGN_RIGHT);
                campoNumReclamacion.SetVisibility(PdfFormField.VISIBLE);
                campoNumReclamacion.SetFontSize(14);
                form.AddField(campoNumReclamacion);

                // Añadimos el título de las notas.
                parrafo = new Paragraph($"NOTAS");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(40, 225, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos el campo para las notas.
                iText.Kernel.Geom.Rectangle cuadroNotas = new iText.Kernel.Geom.Rectangle(45, 55, ((anchoHoja - 80) / 3) * 2 - 20, 160);
                PdfTextFormField campoNotas = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroNotas, "Notas", "", arial, 12);
                campoNotas.SetJustification(PdfFormField.ALIGN_LEFT);
                campoNotas.SetVisibility(PdfFormField.VISIBLE);
                campoNotas.SetMultiline(true);
                form.AddField(campoNotas);

                // Añadimos el título de la fecha.
                parrafo = new Paragraph($"FECHA");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(((anchoHoja - 80) / 3 ) * 2 + 40, 225, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos el campo para la fecha.
                iText.Kernel.Geom.Rectangle cuadroFecha = new iText.Kernel.Geom.Rectangle(((anchoHoja - 80) / 3) * 2 + 45, 190, ((anchoHoja - 80) / 3) - 10, 25);
                string fechaFinal = $"{DateTime.Now:dd - MM - yyyy}";
                PdfTextFormField campoFecha = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroFecha, "FechaFinal", fechaFinal, arial, 16);
                campoFecha.SetJustification(PdfFormField.ALIGN_CENTER);
                campoFecha.SetVisibility(PdfFormField.VISIBLE);
                form.AddField(campoFecha);

                // Añadimos el título de la firma.
                parrafo = new Paragraph($"El Trabajador/a");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(((anchoHoja - 80) / 3) * 2 + 40, 150, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos la marca de agua.
                iText.Layout.Element.Image marcaAgua = InformesServicio.GetMarcaDeAgua();
                if (marcaAgua != null) {
                    marcaAgua.ScaleToFit((anchoHoja / 3) * 2, 800);
                    marcaAgua.SetOpacity(0.05f);
                    marcaAgua.SetFixedPosition((anchoHoja / 2) - (marcaAgua.GetImageScaledWidth() / 2), ((altoHoja) / 2) - (marcaAgua.GetImageScaledHeight() / 2));
                    doc.Add(marcaAgua);
                }

                // ==================================================
                // ACCIONES EN EL LIENZO
                // ==================================================

                PdfCanvas lienzo = new PdfCanvas(doc.GetPdfDocument().GetFirstPage());

                // Fecha y Número de reclamación
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(40, 640, anchoHoja - 80, 24, 5);
                lienzo.Stroke();

                // Notas
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(40, 50, ((anchoHoja - 80) / 3) * 2 - 10, 170, 5);
                lienzo.Stroke();

                // Fecha Final
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(((anchoHoja - 80) / 3) * 2 + 40, 185, (anchoHoja - 80) / 3, 35, 5);
                lienzo.Stroke();

                // Firma
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(((anchoHoja - 80) / 3) * 2 + 40, 50, (anchoHoja - 80) / 3, 95, 5);
                lienzo.Stroke();
            });

        }




		#endregion
		// ====================================================================================================




	}
}
