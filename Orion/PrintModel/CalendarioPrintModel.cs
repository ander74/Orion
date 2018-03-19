#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Microsoft.Office.Interop.Excel;
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;
using Orion.Properties;
using Orion.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Orion.PrintModel {

	public static class CalendarioPrintModel {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private static ConvertidorNumeroGrafico cnvNumGrafico = new ConvertidorNumeroGrafico();
		private static ConvertidorHora cnvHora = new ConvertidorHora();
		private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
		private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
		private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
		private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();
		//private static ConvertidorColores cnvColores = new ConvertidorColores();

		private static string rutaPlantillaCalendarios = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Calendarios.xlsx");
		private static string rutaPlantillaFallosCalendarios = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/FallosCalendarios.xlsx");

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
				Hoja.Cells[fila, i + 1].Font.Color = Color.FromArgb(c.R, c.G, c.B).ToArgb();

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
					if (fila % 2 == 0) Hoja.Range[Hoja.Cells[fila, 1], Hoja.Cells[fila, 32]].Interior.Color = Color.FromArgb(255, 252, 228, 214);
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
						if (fila % 2 == 0) Hoja.Range[Hoja.Cells[fila, 1], Hoja.Cells[fila, 3]].Interior.Color = Color.FromArgb(255, 252, 228, 214);
						LlenarExcelCalendarioConFallos(Hoja, fila, cal, cal.Informe);
						fila++;
					}
				}

				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = Hoja.Range["A1", Hoja.Cells[fila - 1, 3]].Address;
			});

		}

		#endregion


	}
}
