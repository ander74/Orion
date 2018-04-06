#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
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
			// Definimos los parámetros de la tabla en función de los días del mes.
			float[] ancho = null;
			(int izq, int der) titulos = (0, 0);
			int diasMes = DateTime.DaysInMonth(fecha.Year, fecha.Month);
			switch (diasMes) {
				case 28:
					ancho = Anchos28;
					titulos = (15, 14);
					break;
				case 29:
					ancho = Anchos29;
					titulos = (15, 15);
					break;
				case 30:
					ancho = Anchos30;
					titulos = (16, 15);
					break;
				case 31:
					ancho = Anchos31;
					titulos = (16, 16);
					break;
			}
			// Creamos la tabla
			Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
			// Asignamos el estilo a la tabla.
			tabla.AddStyle(estiloTabla);
			// Insertamos los títulos.
			tabla.AddHeaderCell(new Cell(1, titulos.izq).Add(new Paragraph($"{fecha:MMMM - yyyy}".ToUpper())).AddStyle(estiloTitulos)
														.SetTextAlignment(TextAlignment.LEFT));
			tabla.AddHeaderCell(new Cell(1, titulos.der).Add(new Paragraph(App.Global.CentroActual.ToString().ToUpper())).AddStyle(estiloTitulos)
														.SetTextAlignment(TextAlignment.RIGHT));
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

			// Creamos la tabla de encabezado 1
			Table tablaEncabezado = new Table(UnitValue.CreatePercentArray(new float[] { 50, 50 }));
			tablaEncabezado.SetFont(arial);
			tablaEncabezado.SetFontSize(20);
			tablaEncabezado.SetMargin(0);
			tablaEncabezado.SetPadding(0);
			tablaEncabezado.SetWidth(UnitValue.CreatePercentValue(100));
			tablaEncabezado.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
			tablaEncabezado.AddCell(new Cell().Add(new Paragraph($"{fecha:MMMM - yyyy}".ToUpper()))
														.SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT));
			tablaEncabezado.AddCell(new Cell().Add(new Paragraph(App.Global.CentroActual.ToString().ToUpper()))
														.SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

			// Creamos la tabla que tendrá la Hoja Pijama.
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

		public static async Task CrearCalendariosEnPdf_7(Document doc, ListCollectionView listaCalendarios, DateTime fecha) {

			await Task.Run(() => {
				// Añadimos los fallos al documento.
				doc.Add(GetTablaCalendarios(listaCalendarios, fecha));
			});
		}


		public static async Task FallosEnCalendariosEnPdf_7(Document doc, ListCollectionView listaCalendarios, DateTime fecha) {

			await Task.Run(() => {
				// Añadimos los fallos al documento.
				doc.Add(GetTablaFallosCalendario(listaCalendarios, fecha));
			});
		}

		#endregion
		// ====================================================================================================


	}
	}
