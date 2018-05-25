#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using Microsoft.Office.Interop.Excel;
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;
using Orion.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Orion.PrintModel
{

    public static class CalendarioPrintModel {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private static ConvertidorNumeroGrafico cnvNumGrafico = new ConvertidorNumeroGrafico();
		private static ConvertidorHora cnvHora = new ConvertidorHora();
		private static ConvertidorSuperHora cnvSuperHora = new ConvertidorSuperHora();
        private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
        private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
        private static ConvertidorDecimalEuro cnvDecimalEuro = new ConvertidorDecimalEuro();
		private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
		private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();
		//private static ConvertidorColores cnvColores = new ConvertidorColores();

		private static string rutaPlantillaCalendarios = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Calendarios.xlsx");
		private static string rutaPlantillaFallosCalendarios = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/FallosCalendarios.xlsx");

		// Anchos de tablas para los calendarios.
		private static float[] Anchos28 = new float[] { 6.6666f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f,
														3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f,
														3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f, 3.3333f };
		private static float[] Anchos29 = new float[] { 6.4516f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f,
														3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f,
														3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f, 3.2258f };
		private static float[] Anchos30 = new float[] { 6.2500f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f,
														3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f,
														3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f, 3.1250f };
		private static float[] Anchos31 = new float[] { 6.0606f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f,
														3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f,
														3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f, 3.0303f,
														3.0303f, 3.0303f, };


		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private static void LlenarExcelCalendario(Worksheet Hoja, int fila, Calendario calendario) {

			// Conductor
			Hoja.Cells[fila, 1].Value = calendario.IdConductor;

			// Días
			int diasEnMes = DateTime.DaysInMonth(calendario.Fecha.Year, calendario.Fecha.Month);
			for (int i = 1; i <= diasEnMes; i++) {
				// Escribimos el gráfico.
				Hoja.Cells[fila, i + 1].Value = cnvNumGrafico.Convert(calendario.ListaDias[i - 1].Grafico, null, null, null);
				// Definimos la matriz de objetos para el color
				object[] valores = new object[] { calendario.ListaDias[i - 1].ComboGrafico, calendario.Fecha, calendario.ListaDias[i - 1].Dia };
				System.Windows.Media.Color c = ((System.Windows.Media.SolidColorBrush)cnvColorDia.Convert(valores, null, null, null)).Color;
				// Definimos la fecha del día
				Hoja.Cells[fila, i + 1].Font.Color = System.Drawing.Color.FromArgb(c.R, c.G, c.B).ToArgb();

			}

		}


		private static void LlenarExcelCalendarioConFallos(Worksheet Hoja, int fila, Calendario calendario, string fallos) {

			// Conductor
			//Hoja.Range[Hoja.Cells[fila, 1], Hoja.Cells[fila + 1, 1]].MergeCells = true;
			Hoja.Cells[fila, 1].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium,
											 XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);
			Hoja.Cells[fila, 1].Value = calendario.IdConductor;

			// Días
			//int diasEnMes = DateTime.DaysInMonth(calendario.Fecha.Year, calendario.Fecha.Month);
			//for (int i = 1; i <= diasEnMes; i++) {
			//	// Escribimos el gráfico.
			//	Hoja.Cells[fila, i + 1].Value = cnvNumGrafico.Convert(calendario.ListaDias[i - 1].Grafico, null, null, null);
			//	// Definimos la matriz de objetos para el color
			//	object[] valores = new object[] { calendario.ListaDias[i - 1].Grafico, calendario.Fecha, calendario.ListaDias[i - 1].Dia };
			//	System.Windows.Media.Color c = ((System.Windows.Media.SolidColorBrush)cnvColorDia.Convert(valores, null, null, null)).Color;
			//	// Definimos la fecha del día
			//	Hoja.Cells[fila, i + 1].Font.Color = Color.FromArgb(c.R, c.G, c.B).ToArgb();
			//}
			
			// Fallos
			Hoja.Range[Hoja.Cells[fila, 2], Hoja.Cells[fila, 3]].MergeCells = true;
			Hoja.Range[Hoja.Cells[fila, 2], Hoja.Cells[fila, 3]].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium,
																			  XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);
			Hoja.Cells[fila, 2].WrapText = true;
			Hoja.Cells[fila, 2].HorizontalAlignment = XlHAlign.xlHAlignLeft;
			Hoja.Cells[fila, 2].VerticalAlignment = XlVAlign.xlVAlignTop;
			Hoja.Cells[fila, 2].IndentLevel = 1;
			Hoja.Cells[fila, 2].Value = "\n" + fallos;
			int num = fallos.Count(r => r == "\n".ToCharArray()[0]);
			Hoja.Rows[fila].RowHeight = 20 * (num + 1);

		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS (ITEXT 7)
		// ====================================================================================================

		private static Table GetTablaCalendarios(ListCollectionView listaCalendarios, DateTime fecha) {
			// Fuente a utilizar en la tabla.
			PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
			// Estilo de la tabla.
			iText.Layout.Style estiloTabla = new iText.Layout.Style();
			estiloTabla.SetTextAlignment(TextAlignment.CENTER)
					   .SetVerticalAlignment(VerticalAlignment.MIDDLE)
					   .SetMargins(0, 0, 0, 0)
					   .SetPaddings(0, 0, 0, 0)
					   .SetWidth(UnitValue.CreatePercentValue(100))
					   .SetFont(arial)
					   .SetFontSize(7);
			// Estilo titulos
			iText.Layout.Style estiloTitulos = new iText.Layout.Style();
			estiloTitulos.SetBold()
						 .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
						 .SetFontSize(14);
			// Estilo de las celdas de encabezado.
			iText.Layout.Style estiloEncabezados = new iText.Layout.Style();
			estiloEncabezados.SetBackgroundColor(new DeviceRgb(255, 153, 102))
							 .SetBold()
							 .SetFontSize(8);
			// Estilo de las celdas de conductor.
			iText.Layout.Style estiloConductor = new iText.Layout.Style();
			estiloConductor.SetBorderLeft(new SolidBorder(1))
						   .SetBorderRight(new SolidBorder(1));
			// Estilo de las celdas de dia.
			iText.Layout.Style estiloDia = new iText.Layout.Style();
			estiloDia.SetBorderTop(new SolidBorder(1))
					 .SetBorderBottom(new SolidBorder(1));

            // Creamos la tabla con el encabezado con el logo UGT.
            string textoEncabezado = $"CALENDARIOS\n{fecha:MMMM - yyyy} ({App.Global.CentroActual.ToString()})".ToUpper();
            Table tablaEncabezado = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);

            // Definimos los parámetros de la tabla en función de los días del mes.
            float[] ancho = null;
			int diasMes = DateTime.DaysInMonth(fecha.Year, fecha.Month);
			switch (diasMes) {
				case 28:
					ancho = Anchos28;
					break;
				case 29:
					ancho = Anchos29;
					break;
				case 30:
					ancho = Anchos30;
					break;
				case 31:
					ancho = Anchos31;
					break;
			}
			// Creamos la tabla
			Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
			// Asignamos el estilo a la tabla.
			tabla.AddStyle(estiloTabla);

            // Insertamos los títulos.
            tabla.AddHeaderCell(new Cell(1, diasMes + 1).Add(tablaEncabezado).AddStyle(estiloTitulos));
			
            // Añadimos las celdas de encabezado.
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("Conductor")).AddStyle(estiloEncabezados).AddStyle(estiloConductor).AddStyle(estiloDia));
			for (int i = 1; i <= diasMes; i++) {
				// Definimos el estilo para la última celda.
				iText.Layout.Style estiloUltima = new iText.Layout.Style();
				if (i == diasMes) estiloUltima.SetBorderRight(new SolidBorder(1));
				tabla.AddHeaderCell(new Cell().Add(new Paragraph($"{i:00}")).AddStyle(estiloEncabezados).AddStyle(estiloDia).AddStyle(estiloUltima));
			}
			// Añadimos las celdas con los calendarios.
			int indice = 1;
			foreach (Object obj in listaCalendarios) {
				// Estilo Fondo Alternativo
				iText.Layout.Style estiloFondo = new iText.Layout.Style().SetBackgroundColor(ColorConstants.WHITE);
				if (indice % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(252, 228, 214));
				indice++;
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				// Escribimos el conductor.
				tabla.AddCell(new Cell().Add(new Paragraph($"{cal.IdConductor:000}")).AddStyle(estiloConductor).AddStyle(estiloFondo));
				// Escribimos los días.
				for (int i = 1; i <= diasMes; i++) {
					// Definimos el estilo para la última celda.
					iText.Layout.Style estiloUltima = new iText.Layout.Style();
					if (i == diasMes) estiloUltima.SetBorderRight(new SolidBorder(1));
					// Definimos los datos para extraer el color.
					object[] valores = new object[] { cal.ListaDias[i - 1].ComboGrafico, cal.Fecha, cal.ListaDias[i - 1].Dia };
					System.Windows.Media.Color c = ((System.Windows.Media.SolidColorBrush)cnvColorDia.Convert(valores, null, null, null)).Color;
					tabla.AddCell(new Cell().Add(new Paragraph($"{cnvNumGrafico.Convert(cal.ListaDias[i - 1].Grafico, null, null, null)}"))
											.SetFontColor(new DeviceRgb(c.R, c.G, c.B)).AddStyle(estiloFondo).AddStyle(estiloUltima));
				}
			}
			// Ponemos los bordes de la tabla.
			tabla.SetBorderBottom(new SolidBorder(1));
			// Devolvemos la tabla.
			return tabla;
		}


		private static Table GetTablaFallosCalendario(ListCollectionView listaCalendarios, DateTime fecha) {

			// Fuente a utilizar en la tabla.
			PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
			// Estilo de la tabla.
			iText.Layout.Style estiloTabla = new iText.Layout.Style();
			estiloTabla.SetTextAlignment(TextAlignment.CENTER)
					   .SetMargins(0, 0, 0, 0)
					   .SetPaddings(0, 0, 0, 0)
					   .SetWidth(UnitValue.CreatePercentValue(100))
					   .SetFont(arial)
					   .SetFontSize(10);
			// Estilo encabezados 1
			iText.Layout.Style estiloEncabezados1 = new iText.Layout.Style();
			estiloEncabezados1.SetBold()
							  .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
							  .SetFontSize(8);
			// Estilo de las celdas de encabezado 2.
			iText.Layout.Style estiloEncabezados2 = new iText.Layout.Style();
			estiloEncabezados2.SetBackgroundColor(new DeviceRgb(255, 153, 102))
							 .SetBold()
							 .SetFontSize(8);
			// Estilo de las celdas de conductor.
			iText.Layout.Style estiloConductor = new iText.Layout.Style();
			estiloConductor.SetVerticalAlignment(VerticalAlignment.MIDDLE)
						   .SetFontSize(14)
						   .SetKeepTogether(true);
			// Estilo de las celdas de Fallos.
			iText.Layout.Style estiloFallos = new iText.Layout.Style();
			estiloFallos.SetTextAlignment(TextAlignment.LEFT)
						.SetPadding(5)
						.SetKeepTogether(true);

            // Creamos la tabla con el encabezado con el logo UGT.
            string textoEncabezado = $"ERRORES EN CALENDARIOS\n{fecha:MMMM - yyyy} ({App.Global.CentroActual.ToString()})".ToUpper();
            Table tablaEncabezado = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);

            // Creamos la tabla que tendrá los fallos del calendario.
            float[] columnas = new float[] { 11.5044f, 88.4956f };
			Table tabla = new Table(UnitValue.CreatePercentArray(columnas));
			// Asignamos el estilo a la tabla.
			tabla.AddStyle(estiloTabla);
			// Añadimos el encabezado inicial.
			tabla.AddHeaderCell(new Cell(1, 2).Add(tablaEncabezado).AddStyle(estiloEncabezados1));
			// Añadimos las celdas de encabezado.
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("CONDUCTOR")).AddStyle(estiloEncabezados2));
			tabla.AddHeaderCell(new Cell().Add(new Paragraph("FALLOS DETECTADOS")).AddStyle(estiloEncabezados2));
			// Añadimos las celdas con los fallos.
			int indice = 1;
			foreach (Object obj in listaCalendarios) {
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				if (!string.IsNullOrWhiteSpace(cal.Informe)) {
					// Estilo Fondo Alternativo
					iText.Layout.Style estiloFondo = new iText.Layout.Style().SetBackgroundColor(ColorConstants.WHITE);
					if (indice % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(252, 228, 214));
					tabla.AddCell(new Cell().Add(new Paragraph($"{cal.IdConductor:000}")).AddStyle(estiloConductor).AddStyle(estiloFondo));
					tabla.AddCell(new Cell().Add(new Paragraph($"{cal.Informe}")).AddStyle(estiloFallos).AddStyle(estiloFondo));
					indice++;
				}
			}

			return tabla;
		}


        private static Table GetTablaEstadisticasCalendarios(List<EstadisticaCalendario> lista, DateTime fecha, bool segundaQuincena = false) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            iText.Layout.Style estiloTabla = new iText.Layout.Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(8);
            // Estilo encabezados 1
            iText.Layout.Style estiloEncabezados1 = new iText.Layout.Style();
            estiloEncabezados1.SetBold()
                              .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                              .SetFontSize(8);
            // Estilo de las celdas de encabezado 2.
            iText.Layout.Style estiloEncabezados2 = new iText.Layout.Style();
            estiloEncabezados2.SetBackgroundColor(new DeviceRgb(255, 153, 102))
                             .SetBold()
                             .SetFontSize(9);
            // Estilo de fondo 2
            iText.Layout.Style estiloFondo = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(252, 228, 214));
            // Estilo Sábados
            iText.Layout.Style estiloSabados = new iText.Layout.Style().SetFontColor(new DeviceRgb(0, 0, 255));
            // Estilo Festivos
            iText.Layout.Style estiloFestivos = new iText.Layout.Style().SetFontColor(new DeviceRgb(255, 0, 0));

            // Creamos la tabla con el encabezado con el logo UGT.
            string textoEncabezado = $"ESTADÍSTICAS DE CALENDARIOS\n{fecha:MMMM - yyyy} ({App.Global.CentroActual.ToString()})".ToUpper();
            Table tablaEncabezado = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);

            // Creamos la tabla que tendrá las estadísticas.
            float[] anchos14 = new float[] { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] anchos15 = new float[] { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] anchos16 = new float[] { 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            // Definimos el número de días de la tabla.
            int diasMes = DateTime.DaysInMonth(fecha.Year, fecha.Month);
            int dias = 0;
            switch(diasMes) {
                case 28: dias = 14; break;
                case 29: dias = segundaQuincena ? 14 : 15; break;
                case 30: dias = 15; break;
                case 31: dias = segundaQuincena ? 16 : 15; break;
            }
            Table tabla = null;
            switch (dias) {
                case 14: tabla = new Table(UnitValue.CreatePercentArray(anchos14)); break;
                case 15: tabla = new Table(UnitValue.CreatePercentArray(anchos15)); break;
                case 16: tabla = new Table(UnitValue.CreatePercentArray(anchos16)); break;
            }
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos el encabezado inicial.
            tabla.AddHeaderCell(new Cell(1, dias + 1).Add(tablaEncabezado).AddStyle(estiloEncabezados1));
            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("CONCEPTOS")).AddStyle(estiloEncabezados2));
            for (int d = 1; d <= dias; d++) {
                int dd = segundaQuincena ? diasMes - dias + d : d;
                DateTime f = new DateTime(fecha.Year, fecha.Month, dd);
                iText.Layout.Style estiloDia = null;
                if (f.DayOfWeek == DayOfWeek.Saturday) {
                    estiloDia = estiloSabados;
                } else if (f.DayOfWeek == DayOfWeek.Sunday || App.Global.CalendariosVM.EsFestivo(f)) {
                    estiloDia = estiloFestivos;
                } else {
                    estiloDia = new iText.Layout.Style().SetFontColor(new DeviceRgb(0, 0, 0));
                }
                tabla.AddHeaderCell(new Cell().Add(new Paragraph($"{dd:00}")).AddStyle(estiloEncabezados2).AddStyle(estiloDia));
            }
            // Turnos 1
            tabla.AddCell(new Cell().Add(new Paragraph($"Turnos 1").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].Turno1 == 0 ? "" : $"{lista[i].Turno1:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Turnos 2
            tabla.AddCell(new Cell().Add(new Paragraph($"Turnos 2").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].Turno2 == 0 ? "" : $"{lista[i].Turno2:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Turnos 3
            tabla.AddCell(new Cell().Add(new Paragraph($"Turnos 3").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].Turno3 == 0 ? "" : $"{lista[i].Turno3:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Turnos 4
            tabla.AddCell(new Cell().Add(new Paragraph($"Turnos 4").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].Turno4 == 0 ? "" : $"{lista[i].Turno4:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Jornadas Trabajadas
            tabla.AddCell(new Cell().Add(new Paragraph($"Jornadas trabajadas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].TotalJornadas == 0 ? "" : $"{lista[i].TotalJornadas:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Jornadas menores de 7:20
            tabla.AddCell(new Cell().Add(new Paragraph($"Jornadas menores de {App.Global.Convenio.JornadaMedia.ToTexto()}").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].JornadasMenoresMedia == 0 ? "" : $"{lista[i].JornadasMenoresMedia:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Jornadas mayores o iguales de 7:20
            tabla.AddCell(new Cell().Add(new Paragraph($"Jornadas mayores de {App.Global.Convenio.JornadaMedia.ToTexto()}").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = lista[i].JornadasMayoresMedia == 0 ? "" : $"{lista[i].JornadasMayoresMedia:00}";
                tabla.AddCell(new Cell().Add(new Paragraph(dato)));
            }
            // Horas trabajadas
            tabla.AddCell(new Cell().Add(new Paragraph($"Horas trabajadas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].Trabajadas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Media de horas trabajadas
            tabla.AddCell(new Cell().Add(new Paragraph($"Media de horas trabajadas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].MediaTrabajadas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Tiempo partido no computable
            tabla.AddCell(new Cell().Add(new Paragraph($"Tiempo partido no computable").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].TiempoPartido, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Horas Negativas
            tabla.AddCell(new Cell().Add(new Paragraph($"Horas negativas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].HorasNegativas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Horas Acumuladas
            tabla.AddCell(new Cell().Add(new Paragraph($"Horas Acumuladas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].Acumuladas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Acumuladas - Negativas
            tabla.AddCell(new Cell().Add(new Paragraph($"Diferencia Acumuladas - Negativas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].Acumuladas - lista[i].HorasNegativas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Horas Nocturnas
            tabla.AddCell(new Cell().Add(new Paragraph($"Horas Nocturnas").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvSuperHora.Convert(lista[i].Nocturnas, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Desayuno
            tabla.AddCell(new Cell().Add(new Paragraph($"Dieta Desayuno").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimal.Convert(lista[i].Desayuno, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Importe Desayuno
            tabla.AddCell(new Cell().Add(new Paragraph($"Importe Desayuno").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].ImporteDesayuno, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Comida
            tabla.AddCell(new Cell().Add(new Paragraph($"Dieta Comida").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimal.Convert(lista[i].Comida, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Importe Comida
            tabla.AddCell(new Cell().Add(new Paragraph($"Importe Comida").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].ImporteComida, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Cena
            tabla.AddCell(new Cell().Add(new Paragraph($"Dieta Cena").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimal.Convert(lista[i].Cena, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Importe Cena
            tabla.AddCell(new Cell().Add(new Paragraph($"Importe Cena").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].ImporteCena, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Cena
            tabla.AddCell(new Cell().Add(new Paragraph($"Dieta Plus Cena").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimal.Convert(lista[i].PlusCena, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Importe Plus Cena
            tabla.AddCell(new Cell().Add(new Paragraph($"Importe Plus Cena").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].ImportePlusCena, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Nocturnidad
            tabla.AddCell(new Cell().Add(new Paragraph($"Plus Nocturnidad").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].PlusNocturnidad, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Menor Descanso
            tabla.AddCell(new Cell().Add(new Paragraph($"Plus Menor Descnaso").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].PlusMenorDescanso, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Limpieza
            tabla.AddCell(new Cell().Add(new Paragraph($"Plus Limpieza").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].PlusLimipeza, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Paqueteria
            tabla.AddCell(new Cell().Add(new Paragraph($"Plus Paqueteria").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].PlusPaqueteria, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }
            // Plus Navidad
            tabla.AddCell(new Cell().Add(new Paragraph($"Plus Navidad").SetBold()));
            for (int d = 1; d <= dias; d++) {
                int i = (segundaQuincena ? diasMes - dias + d : d) - 1;
                string dato = (string)cnvDecimalEuro.Convert(lista[i].PlusNavidad, null, VerValores.NoCeros, null);
                tabla.AddCell(new Cell().Add(new Paragraph($"{dato}")));
            }


            // Ponemos el fondo de las filas alternas.
            for (int fila = 0; fila < tabla.GetNumberOfRows(); fila++) {
                for (int col = 0; col < tabla.GetNumberOfColumns(); col++) {
                    if (col > 0) {
                        int dia = (segundaQuincena ? diasMes - dias + col : col);
                        DateTime f = new DateTime(fecha.Year, fecha.Month, dia);
                        if (f.DayOfWeek == DayOfWeek.Saturday) tabla.GetCell(fila, col).AddStyle(estiloSabados);
                        if (f.DayOfWeek == DayOfWeek.Sunday || App.Global.CalendariosVM.EsFestivo(f)) tabla.GetCell(fila, col).AddStyle(estiloFestivos);
                    }
                    if (fila % 2 != 0) {
                        tabla.GetCell(fila, col).AddStyle(estiloFondo);
                    }
                }
            }


            return tabla;
        }


        private static Table GetTablaEstadisticasMes2(List<GraficoFecha> listaGraficos, List<GraficosPorDia> listaNumeros, List<DescansosPorDia> listaDescansos) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            iText.Layout.Style estiloTabla = new iText.Layout.Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetKeepTogether(true)
                       .SetFont(arial)
                       .SetFontSize(9);
            // Estilo titulos
            iText.Layout.Style estiloTitulos = new iText.Layout.Style();
            estiloTitulos.SetBold()
                         .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                         .SetFontSize(14);
            // Estilo de las celdas de encabezado.
            iText.Layout.Style estiloEncabezados = new iText.Layout.Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                             .SetBorderBottom(new SolidBorder(1))
                             .SetBorderTop(new SolidBorder(1))
                             .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                             .SetBold();
            // Estilo de las celdas de total.
            iText.Layout.Style estiloTotales = new iText.Layout.Style();
            estiloTotales.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                         .SetBorderBottom(new SolidBorder(1))
                         .SetBorderTop(new SolidBorder(1))
                         .SetFontSize(12)
                         .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                         .SetBold();
            // Estilo de las celdas de dia.
            iText.Layout.Style estiloDia = new iText.Layout.Style();
            estiloDia.SetBorder(new SolidBorder(1))
                     .SetFontSize(14)
                     .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                     .SetKeepTogether(true)
                     .SetBold();
            // Estilo de las celdas del final.
            iText.Layout.Style estiloCeldaFinal = new iText.Layout.Style();
            estiloCeldaFinal.SetHorizontalAlignment(HorizontalAlignment.LEFT)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderRight(new SolidBorder(1))
                            .SetKeepTogether(true)
                            .SetTextAlignment(TextAlignment.LEFT);
            // Estilo de las celdas del final.
            iText.Layout.Style estiloCeldaDerecha = new iText.Layout.Style().SetBorderRight(new SolidBorder(1));
            // Creamos la tabla que tendrá la Hoja Pijama.
            float[] columnas = new float[] { 10, 18, 18, 18, 18, 18 };
            Table tabla = new Table(UnitValue.CreatePercentArray(columnas));

            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos el encabezado y el logo
            string textoEncabezado = $"ESTADÍSTICAS\n{listaGraficos[0].Fecha:MMMM - yyyy} ({App.Global.CentroActual.ToString()})".ToUpper();
            Table tablaTitulos = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);
            tabla.AddHeaderCell(new Cell(1, 6).Add(tablaTitulos).AddStyle(estiloTitulos));

            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Día")).AddStyle(estiloEncabezados).SetBorder(new SolidBorder(1)));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Descansos")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Gr.Totales")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Gr.Asignad.")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Gr.No Asig.")).AddStyle(estiloEncabezados).AddStyle(estiloCeldaDerecha));
            // Añadimos las celdas de cada día de la lista.
            int diasMes = DateTime.DaysInMonth(listaGraficos[0].Fecha.Year, listaGraficos[0].Fecha.Month);
            long totalTrabajadas = 0;
            int totalDescansos = 0;
            int totalGraficosTotales = 0;
            int totalGraficosAsignados = 0;
            for (int dia = 1; dia <= diasMes; dia++) {
                List<GraficoFecha> graficos = listaGraficos.Where(g => g.Dia == dia && g.Numero > 0).ToList();
                if (graficos == null) continue;
                GraficosPorDia numeros = listaNumeros.FirstOrDefault(n => n.Dia == dia);
                // Establecemos el color de fondo de las filas
                iText.Layout.Style estiloCeldas = new iText.Layout.Style().SetBackgroundColor(ColorConstants.WHITE);
                if (dia % 2 == 0) estiloCeldas.SetBackgroundColor(new DeviceRgb(204, 236, 255));
                // Establecemos el estilo para el fin de semana
                iText.Layout.Style estiloFindes = new iText.Layout.Style().SetFontColor(ColorConstants.BLACK);
                if (graficos.Count > 0) {
                    if (graficos[0].Fecha.DayOfWeek == DayOfWeek.Saturday) estiloFindes.SetFontColor(ColorConstants.BLUE);
                    if (graficos[0].Fecha.DayOfWeek == DayOfWeek.Sunday) estiloFindes.SetFontColor(ColorConstants.RED);
                    if (App.Global.CalendariosVM.EsFestivo(graficos[0].Fecha)) estiloFindes.SetFontColor(ColorConstants.RED);
                }
                // DÍA.
                tabla.AddCell(new Cell(2, 1).Add(new Paragraph($"{dia:00}")).AddStyle(estiloDia).AddStyle(estiloFindes).AddStyle(estiloCeldas));
                // TRABAJADAS
                long trabajadas = graficos.Sum(g => g.Trabajadas.Ticks);
                totalTrabajadas += trabajadas;
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(new TimeSpan(trabajadas), null, VerValores.HoraPlus, null))).AddStyle(estiloCeldas));
                // DESCANSOS
                DescansosPorDia descansos = listaDescansos.FirstOrDefault(n => n.Dia == dia);
                totalDescansos += descansos.Descansos;
                tabla.AddCell(new Cell().Add(new Paragraph($"{descansos.Descansos:00}")).AddStyle(estiloCeldas));
                // GRÁFICOS TOTALES
                int graficosTotales = numeros.Lista.Count;
                totalGraficosTotales += graficosTotales;
                tabla.AddCell(new Cell().Add(new Paragraph($"{graficosTotales:00}")).AddStyle(estiloCeldas));
                // GRÁFICOS ASIGNADOS
                int graficosAsignados = graficos.Count(g => g.Numero > 0);
                totalGraficosAsignados += graficosAsignados;
                tabla.AddCell(new Cell().Add(new Paragraph($"{graficosAsignados:00}")).AddStyle(estiloCeldas));
                // GRÁFICOS NO ASIGNADOS
                tabla.AddCell(new Cell().Add(new Paragraph($"{graficosTotales - graficosAsignados:00}")).AddStyle(estiloCeldas).AddStyle(estiloCeldaDerecha));
                // NÚMEROS DE GRÁFICOS NO ASIGNADOS
                List<int> noAsignados = numeros.Lista.Except(graficos.Select(g => g.Numero)).ToList();
                string graficosSinAsignar = "";
                foreach (int i in noAsignados) {
                    if (graficosSinAsignar.Length > 0) graficosSinAsignar += ", ";
                    graficosSinAsignar += $"{i:0000}";
                }
                Paragraph textoNoAsignados = new Paragraph();
                textoNoAsignados.Add(new Text("Graficos No Asignados: ").SetBold());
                textoNoAsignados.Add(graficosSinAsignar);
                tabla.AddCell(new Cell(1, 5).Add(textoNoAsignados).AddStyle(estiloCeldaFinal).AddStyle(estiloCeldas));
            }
            // TOTALES.
            tabla.AddCell(new Cell().Add(new Paragraph("TOTAL")).AddStyle(estiloDia).AddStyle(estiloEncabezados));
            tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(new TimeSpan(totalTrabajadas), null, VerValores.HoraPlus, null))).AddStyle(estiloTotales));
            tabla.AddCell(new Cell().Add(new Paragraph($"{totalDescansos:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell().Add(new Paragraph($"{totalGraficosTotales:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell().Add(new Paragraph($"{totalGraficosAsignados:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell().Add(new Paragraph($"{totalGraficosTotales - totalGraficosAsignados:00}")).AddStyle(estiloTotales));

            return tabla;
        }


        private static Table GetTablaEstadisticasMes(List<GraficoFecha> listaGraficos, List<GraficosPorDia> listaNumeros, List<DescansosPorDia> listaDescansos) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            iText.Layout.Style estiloTabla = new iText.Layout.Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetKeepTogether(true)
                       .SetFont(arial)
                       .SetFontSize(7);
            // Estilo titulos
            iText.Layout.Style estiloTitulos = new iText.Layout.Style();
            estiloTitulos.SetBold()
                         .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                         .SetFontSize(14);
            // Estilo de las celdas de encabezado.
            iText.Layout.Style estiloEncabezados = new iText.Layout.Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                             .SetBorderBottom(new SolidBorder(1))
                             .SetBorderTop(new SolidBorder(1))
                             .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                             .SetBold();
            // Estilo de las celdas de encabezado.
            iText.Layout.Style estiloEncabezadoFilas = new iText.Layout.Style();
            estiloEncabezadoFilas.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                                 .SetBorderLeft(new SolidBorder(1))
                                 .SetBorderRight(new SolidBorder(1))
                                 .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                 .SetBold();
            // Estilo de las celdas de encabezado total.
            iText.Layout.Style estiloTotalesEncabezado = new iText.Layout.Style();
            estiloTotalesEncabezado.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                                   .SetBorder(new SolidBorder(1))
                                   .SetFontSize(9)
                                   .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                                   .SetBold();
            // Estilo de las celdas de total.
            iText.Layout.Style estiloTotales = new iText.Layout.Style();
            estiloTotales.SetBorder(new SolidBorder(1))
                         .SetFontSize(9)
                         .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // Estilo de las celdas de dia.
            iText.Layout.Style estiloDia = new iText.Layout.Style();
            estiloDia.SetBorder(new SolidBorder(1))
                     .SetFontSize(14)
                     .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                     .SetKeepTogether(true)
                     .SetBold();
            // Estilo de las celdas del final.
            iText.Layout.Style estiloCeldaFinal = new iText.Layout.Style();
            estiloCeldaFinal.SetHorizontalAlignment(HorizontalAlignment.LEFT)
                            .SetBorderBottom(new SolidBorder(1))
                            .SetBorderRight(new SolidBorder(1))
                            .SetKeepTogether(true)
                            .SetTextAlignment(TextAlignment.LEFT);
            // Estilo de las celdas del final.
            iText.Layout.Style estiloCeldaDerecha = new iText.Layout.Style().SetBorderRight(new SolidBorder(1));
            // Variables a usar.
            int diasMes = DateTime.DaysInMonth(listaGraficos[0].Fecha.Year, listaGraficos[0].Fecha.Month);
            long totalTrabajadas = 0;
            int totalDescansos = 0;
            int totalGraficosTotales = 0;
            int totalGraficosAsignados = 0;


            // Creamos la tabla que tendrá la Hoja Pijama.
            float[] columnas = null;
            switch (diasMes) {
                case 28:
                    columnas = new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                    break;
                case 29:
                    columnas = new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                    break;
                case 30:
                    columnas = new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                    break;
                case 31:
                    columnas = new float[] { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                    break;
            }
            Table tabla = new Table(UnitValue.CreatePercentArray(columnas));

            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos el encabezado y el logo
            string textoEncabezado = $"ESTADÍSTICAS\n{listaGraficos[0].Fecha:MMMM - yyyy} ({App.Global.CentroActual.ToString()})".ToUpper();
            Table tablaTitulos = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);
            tabla.AddHeaderCell(new Cell(1, diasMes+1).Add(tablaTitulos).AddStyle(estiloTitulos));

            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("")).AddStyle(estiloEncabezados).SetBorder(new SolidBorder(1)));
            for (int dia = 1; dia <= diasMes; dia++) {
                // Establecemos el estilo para el fin de semana
                DateTime fechaFindes = new DateTime(listaGraficos[0].Fecha.Year, listaGraficos[0].Fecha.Month, dia);
                iText.Layout.Style estiloFindes = new iText.Layout.Style().SetFontColor(ColorConstants.BLACK);
                if (fechaFindes.DayOfWeek == DayOfWeek.Saturday) estiloFindes.SetFontColor(ColorConstants.BLUE);
                if (fechaFindes.DayOfWeek == DayOfWeek.Sunday) estiloFindes.SetFontColor(ColorConstants.RED);
                if (App.Global.CalendariosVM.EsFestivo(fechaFindes)) estiloFindes.SetFontColor(ColorConstants.RED);
                Cell celda = new Cell().Add(new Paragraph($"{dia:00}")).AddStyle(estiloEncabezados).AddStyle(estiloFindes);
                if (dia == diasMes) celda.SetBorderRight(new SolidBorder(1));
                tabla.AddHeaderCell(celda);
            }

            // Definimos un array de textos que se van a añadir después a la tabla.
            string[,] celdas = new string[6, diasMes + 1];

            // Añadimos las celdas de encabezado para las filas
            celdas[0, 0] = "Gráficos\nNo Asig.";
            celdas[1, 0] = "Trabajadas";
            celdas[2, 0] = "Descansos";
            celdas[3, 0] = "Graf.Tot.";
            celdas[4, 0] = "Asignados";
            celdas[5, 0] = "No Asig.";

            // Creamos todas las celdas.
            for (int dia = 1; dia <= diasMes; dia++) {

                // Variables temporales
                List<GraficoFecha> graficos = listaGraficos.Where(g => g.Dia == dia && g.Numero > 0).ToList();
                if (graficos == null) continue;
                GraficosPorDia numeros = listaNumeros.FirstOrDefault(n => n.Dia == dia);

                // GRÁFICOS NO ASIGNADOS
                List<int> noAsignados = numeros.Lista.Except(graficos.Select(g => g.Numero)).ToList();
                string graficosSinAsignar = "";
                foreach (int i in noAsignados) {
                    if (graficosSinAsignar.Length == 0) graficosSinAsignar += "\n";
                    graficosSinAsignar += $"{i:0000}\n";
                }
                celdas[0, dia] = graficosSinAsignar + "\n";

                // TRABAJADAS
                long trabajadas = graficos.Sum(g => g.Trabajadas.Ticks);
                totalTrabajadas += trabajadas;
                celdas[1, dia] = (string)cnvHora.Convert(new TimeSpan(trabajadas), null, VerValores.HoraPlus, null);

                // DESCANSOS
                DescansosPorDia descansos = listaDescansos.FirstOrDefault(n => n.Dia == dia);
                totalDescansos += descansos.Descansos;
                celdas[2, dia] = $"{descansos.Descansos:00}";

                // GRÁFICOS TOTALES
                int graficosTotales = numeros.Lista.Count;
                totalGraficosTotales += graficosTotales;
                celdas[3, dia] = $"{graficosTotales:00}";

                // GRÁFICOS ASIGNADOS
                int graficosAsignados = graficos.Count(g => g.Numero > 0);
                totalGraficosAsignados += graficosAsignados;
                celdas[4, dia] = $"{graficosAsignados:00}";

                // GRÁFICOS NO ASIGNADOS
                celdas[5, dia] = $"{graficosTotales - graficosAsignados:00}";

            }

            // AÑADIMOS LAS CELDAS A LA TABLA
            for (int fila = 0; fila < 6; fila++) {
                for (int dia = 0; dia <= diasMes; dia++) {
                    Cell celda = new Cell().Add(new Paragraph(celdas[fila, dia]));
                    if (fila % 2 == 0 && dia > 0) celda.SetBackgroundColor(new DeviceRgb(204, 236, 255));
                    if (dia == 0) celda.AddStyle(estiloEncabezadoFilas);
                    if (dia == diasMes) celda.AddStyle(estiloCeldaDerecha);
                    if (fila == 0 || fila == 5) celda.SetBorderBottom(new SolidBorder(1));
                    tabla.AddCell(celda);
                }
            }

            // AÑADIMOS UNA FILA VACÍA
            tabla.AddCell(new Cell(1, diasMes + 1).Add(new Paragraph("\n\n")).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

            // AÑADIMOS LAS CABECERAS DE LOS TOTALES
            tabla.AddCell(new Cell(1, 3).Add(new Paragraph("Total Trabajadas")).AddStyle(estiloTotalesEncabezado));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph("Total Descansos")).AddStyle(estiloTotalesEncabezado));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph("Total Gráficos")).AddStyle(estiloTotalesEncabezado));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph("Total Asignados")).AddStyle(estiloTotalesEncabezado));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph("Total No Asignados")).AddStyle(estiloTotalesEncabezado));
            tabla.AddCell(new Cell(1, diasMes + 1 - 15).Add(new Paragraph()).SetBorder(iText.Layout.Borders.Border.NO_BORDER));

            // AÑADIMOS LOS TOTALES
            tabla.AddCell(new Cell(1, 3).Add(new Paragraph((string)cnvHora.Convert(new TimeSpan(totalTrabajadas), null, VerValores.HoraPlus, null))).AddStyle(estiloTotales));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph($"{totalDescansos:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph($"{totalGraficosTotales:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph($"{totalGraficosAsignados:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell(1, 4).Add(new Paragraph($"{totalGraficosTotales - totalGraficosAsignados:00}")).AddStyle(estiloTotales));
            tabla.AddCell(new Cell(1, diasMes + 1 - 15).Add(new Paragraph()).SetBorder(iText.Layout.Borders.Border.NO_BORDER));


            return tabla;
        }





        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public static async Task CrearCalendariosEnPdf(Workbook Libro, ListCollectionView lista, DateTime fecha) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Título
				Hoja.Range["A1"].Value = string.Format("{0:MMMM} - {0:yyyy}", fecha).ToUpper();
				// Centro
				Hoja.Range["AF1"].Value = App.Global.CentroActual.ToString().ToUpper();
				// Llenamos la plantilla con los calendarios.
				int fila = 3;
				double num = 1;
				foreach (Object obj in lista) {
					double valor = num / lista.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					num++;
					Calendario cal = obj as Calendario;
					if (cal == null) continue;
					if (fila % 2 == 0) Hoja.Range[Hoja.Cells[fila, 1], Hoja.Cells[fila, 32]].Interior.Color = System.Drawing.Color.FromArgb(255, 252, 228, 214);
					LlenarExcelCalendario(Hoja, fila, cal);
					fila++;
				}
				// Rellenamos los bordes de los calendarios.
				Hoja.Range["A2", Hoja.Cells[fila - 1, 32]].Borders.LineStyle = XlLineStyle.xlContinuous;
				Hoja.Range["A2", Hoja.Cells[fila - 1, 32]].Borders.Color = XlRgbColor.rgbBlack;
				Hoja.Range["A2", Hoja.Cells[fila - 1, 32]].Borders.Weight = XlBorderWeight.xlThin;
				Hoja.Range["A2", Hoja.Cells[fila - 1, 32]].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlRgbColor.rgbBlack);
				Hoja.Range["A2:AF2"].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);
				Hoja.Range["A2", Hoja.Cells[fila - 1, 1]].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium,
																	   XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);

				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = Hoja.Range["A1", Hoja.Cells[fila - 1, 32]].Address;
			});

		}


		public static async Task FallosEnCalendariosEnPdf(Workbook Libro, ListCollectionView lista, DateTime fecha) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Título
				Hoja.Range["A1"].Value = string.Format("{0:MMMM} - {0:yyyy}", fecha).ToUpper();
				// Centro
				Hoja.Range["C1"].Value = App.Global.CentroActual.ToString().ToUpper();
				// Llenamos la plantilla con los calendarios.
				int fila = 3;
				double num = 1;
				foreach (Object obj in lista) {
					double valor = num / lista.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					num++;
					Calendario cal = obj as Calendario;
					if (cal == null) continue;
					if (!string.IsNullOrWhiteSpace(cal.Informe)) {
						if (fila % 2 == 0) Hoja.Range[Hoja.Cells[fila, 1], Hoja.Cells[fila, 3]].Interior.Color = System.Windows.Media.Color.FromArgb(255, 252, 228, 214);
						LlenarExcelCalendarioConFallos(Hoja, fila, cal, cal.Informe);
						fila++;
					}
				}

				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = Hoja.Range["A1", Hoja.Cells[fila - 1, 3]].Address;
			});

		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS (ITEXT 7)
		// ====================================================================================================

        /// <summary>
        /// Crea un PDF con la tabla de calendarios
        /// </summary>
		public static async Task CrearCalendariosEnPdf_7(Document doc, ListCollectionView listaCalendarios, DateTime fecha) {

			await Task.Run(() => {
                try { 
				doc.Add(GetTablaCalendarios(listaCalendarios, fecha));
                } catch (Exception ex) {
                }
            });
		}


        /// <summary>
        /// Crea un PDF con una tabla y los fallos de todos los calendarios.
        /// </summary>
		public static async Task FallosEnCalendariosEnPdf_7(Document doc, ListCollectionView listaCalendarios, DateTime fecha) {

			await Task.Run(() => {
                try { 
				doc.Add(GetTablaFallosCalendario(listaCalendarios, fecha));
                } catch (Exception ex) {
                }
            });
		}


        /// <summary>
        /// Crea un PDF con las estadísticas de los calendarios.
        /// </summary>
        public static async Task EstadisticasCalendariosEnPdf(Document doc, List<EstadisticaCalendario> listaEstadisticas, DateTime fecha) {

            await Task.Run(() => {
                try {
                    doc.Add(GetTablaEstadisticasCalendarios(listaEstadisticas, fecha));
                    doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    doc.Add(GetTablaEstadisticasCalendarios(listaEstadisticas, fecha, true));
                }catch (Exception ex) {
                }
            });

        }


        /// <summary>
        /// Crea un PDF con las estadísticas del mes actual en los calendarios.
        /// </summary>
        public static async Task EstadisticasMesEnPdf(Document doc, List<GraficoFecha> listaGraficos, List<GraficosPorDia> listaNumeros, List<DescansosPorDia> listaDescansos) {

            await Task.Run(() => {

                try {
                    doc.Add(GetTablaEstadisticasMes(listaGraficos, listaNumeros, listaDescansos));
                } catch (Exception ex) {

                }

            });

        }



    #endregion
    // ====================================================================================================


}
	}
