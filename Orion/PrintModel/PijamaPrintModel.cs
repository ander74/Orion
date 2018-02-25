#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using Orion.Models;
using System.Collections.ObjectModel;
using Orion.Config;
using Orion.Views;
using Orion.Convertidores;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Data;
using Orion.Properties;

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

		private static void LlenarExcelPijama(int fila, Pijama.HojaPijama pijama, ref string[,] datos, ref Color[,] colores) {

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
				colores[dia.Dia - 1, 0] = Color.FromArgb(255, c1.R, c1.G, c1.B);
				colores[dia.Dia - 1, 1] = Color.FromArgb(255, c2.R, c2.G, c2.B);

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
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public static void CrearPijamaEnPdf(Pijama.HojaPijama pijama, string ruta) {
			// Creamos una aplicacion Excel y la mantenemos oculta.
			Application ExcelApp = new Application();
			// Creamos el libro
			Workbook Libro = null;

			try {
				ExcelApp.Visible = false;
				// Creamos el libro desde la plantilla.
				Libro = ExcelApp.Workbooks.Add(rutaPlantillaPijama);
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Definimos la matriz de colores para ponerlos.
				string[,] datos = new string[33, 28];
				Color[,] colores = new Color[31, 2];
				// Llenamos la hoja con los datos
				LlenarExcelPijama(1, pijama, ref datos, ref colores);
				// Coloreamos la plantilla
				for (int m = 1; m <= 31; m++) {
					if (colores[m - 1, 0] != null) Hoja.Cells[2 + m, 1].Font.Color = colores[m - 1, 0];
					if (colores[m - 1, 1] != null) Hoja.Cells[2 + m, 2].Font.Color = colores[m - 1, 1];
				}
				// Pegamos los datos
				Range rango = Hoja.Range["A1:AB33"];
				rango.Value = datos;
				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = rango.Address;
				// Exportamos el libro como PDF.
				Libro.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ruta);
				// Si hay que abrir el PDF, se abre.
				if (Settings.Default.AbrirPDFs) Process.Start(ruta);
			} finally {
				if (Libro != null) Libro.Close(false);
				if (ExcelApp != null) ExcelApp.Quit();
			}
		}


		public static void CrearTodosPijamasEnPdf(ListCollectionView ListaCalendarios, string rutaArchivo) {

			Application ExcelApp = new Application();
			Workbook Libro = null;

			try {
				// Mostramos la barra de progreso y le asignamos el valor de 1
				App.Global.VisibilidadBarraProgreso = System.Windows.Visibility.Visible;
				App.Global.TextoProgreso = "Creando PDF...";
				App.Global.ValorBarraProgreso = 0;
				ExcelApp.Visible = false;
				Libro = ExcelApp.Workbooks.Add(rutaPlantillaPijama);
				Worksheet Hoja = Libro.Worksheets[1];
				int fila = 1;
				string[,] datos = new string[ListaCalendarios.Count * 33, 28];
				Color[,] colores = new Color[31, 2];
				double num = 1;
				foreach (Object obj in ListaCalendarios) {
					double valor = num / ListaCalendarios.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					num++;
					Calendario calendario = obj as Calendario;
					if (calendario == null) continue;
					Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(calendario, new MensajeProvider());
					if (fila > 1) {
						Hoja.Range["A1:AB34"].Copy(Hoja.Cells[fila, 1]);
						Hoja.Range[fila.ToString() + ":" + fila.ToString()].RowHeight = 35;
						Hoja.Range[(fila + 1).ToString() + ":" + (fila + 33).ToString()].RowHeight = 25;
					}
					LlenarExcelPijama(fila, hojapijama, ref datos, ref colores);
					for (int m=1; m <= 31; m++) {
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
				// Exportamos el libro como PDF.
				Libro.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, rutaArchivo);
				// Si hay que abrir el PDF, se abre.
				if (Settings.Default.AbrirPDFs) Process.Start(rutaArchivo);
			} finally {
				if (Libro != null) Libro.Close(false);
				if (ExcelApp != null) ExcelApp.Quit();
				App.Global.VisibilidadBarraProgreso = System.Windows.Visibility.Collapsed;
			}

		}

		#endregion




	}
}
