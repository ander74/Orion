#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Windows.Data;
using Orion.DataModels;
using Orion.Properties;
using System.Diagnostics;

namespace Orion.PrintModel {

	public static class GraficosPrintModel {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private static ConvertidorNumeroGrafico cnvNumGrafico = new ConvertidorNumeroGrafico();
		private static ConvertidorHora cnvHora = new ConvertidorHora();
		private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
		private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
		private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
		private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();

		private static string rutaPlantillaGraficos = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Graficos.xlsx");
		private static string rutaPlantillaGraficosIndividuales = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/GraficoIndividual.xlsx");
		private static string rutaPlantillaEstadisticasGraficos = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/EstadisticasGraficos.xlsx");
		private static string rutaPlantillaEstadisticasGraficosPorCentros = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/EstadisticasGraficosPorCentros.xlsx");
		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================

		private static string[,] RangoGraficos(ListCollectionView lista) {

			// Creamos el array a devolver.
			string[,] datos = new string[lista.Count, 19];
			// Rellenamos el array.
			int fila = 0;
			foreach(Object obj in lista) {
				Grafico g = obj as Grafico;
				if (g == null) continue;
				datos[fila, 0] = (string)cnvNumGrafico.Convert(g.Numero, null, null, null);
				datos[fila, 1] = g.Turno.ToString("0");
				//datos[fila, 2] = g.DescuadreInicio == 0 ? "" : g.DescuadreInicio.ToString("00");
				datos[fila, 3] = (string)cnvHora.Convert(g.Inicio, null, VerValores.NoCeros, null);
				datos[fila, 4] = (string)cnvHora.Convert(g.Final, null, VerValores.NoCeros, null);
				//datos[fila, 5] = g.DescuadreFinal == 0 ? "" : g.DescuadreFinal.ToString("00");
				datos[fila, 6] = (string)cnvHora.Convert(g.InicioPartido, null, VerValores.NoCeros, null);
				datos[fila, 7] = (string)cnvHora.Convert(g.FinalPartido, null, VerValores.NoCeros, null);
				datos[fila, 8] = (string)cnvHora.Convert(g.Valoracion, null, VerValores.NoCeros, null);
				datos[fila, 9] = (string)cnvHora.Convert(g.Trabajadas, null, VerValores.NoCeros, null);
				datos[fila, 10] = (string)cnvHora.Convert(g.DiferenciaValoracion, null, VerValores.NoCeros, null);
				datos[fila, 11] = (string)cnvHora.Convert(g.Acumuladas, null, VerValores.NoCeros, null);
				datos[fila, 12] = (string)cnvHora.Convert(g.Nocturnas, null, VerValores.NoCeros, null);
				datos[fila, 13] = (string)cnvDecimal.Convert(g.Desayuno, null, VerValores.NoCeros, null);
				datos[fila, 14] = (string)cnvDecimal.Convert(g.Comida, null, VerValores.NoCeros, null);
				datos[fila, 15] = (string)cnvDecimal.Convert(g.Cena, null, VerValores.NoCeros, null);
				datos[fila, 16] = (string)cnvDecimal.Convert(g.PlusCena, null, VerValores.NoCeros, null);
				if (g.PlusLimpieza) datos[fila, 17] = "X";
				if (g.PlusPaqueteria) datos[fila, 18] = "X";
				fila++;
			}

			// Devolvemos el array
			return datos;
		}


		private static void SetRangoGraficosIndividuales(ListCollectionView lista, DateTime fecha, ref string[,] datos) {

			// Definimos la fila inicial
			int fila = 0;
			// Rellenamos el array.
			foreach (object obj in lista) {

				Grafico g = obj as Grafico;
				if (g == null) continue;

				// Título y centro.
				datos[fila, 0] = string.Format("{0:dd} - {0:MMMM} - {0:yyyy}", fecha).ToUpper();
				datos[fila,4] = App.Global.CentroActual.ToString().ToUpper();

				// Gráfico y valoracion
				datos[fila + 2, 0] = "Gráfico: " + (string)cnvNumGrafico.Convert(g.Numero, null, null, null) + "   Turno: " + g.Turno.ToString("0");
				datos[fila + 2, 4] = "Valoración: " + (string)cnvHora.Convert(g.Valoracion, null, VerValores.NoCeros, null);

				// Valoraciones
				datos[fila + 3, 0] = "Inicio";
				datos[fila + 3, 1] = "Línea";
				datos[fila + 3, 2] = "Descripción";
				datos[fila + 3, 3] = "Final";
				datos[fila + 3, 4] = "Tiempo";
				int filavaloracion = 0;
				foreach (ValoracionGrafico v in g.ListaValoraciones) {
					datos[fila + 4 + filavaloracion, 0] = (string)cnvHora.Convert(v.Inicio, null, VerValores.NoCeros, null);
					datos[fila + 4 + filavaloracion, 1] = v.Linea == 0 ? "" : v.Linea.ToString();
					datos[fila + 4 + filavaloracion, 2] = v.Descripcion;
					datos[fila + 4 + filavaloracion, 3] = (string)cnvHora.Convert(v.Final, null, VerValores.NoCeros, null);
					datos[fila + 4 + filavaloracion, 4] = (string)cnvHora.Convert(v.Tiempo, null, VerValores.NoCeros, null);
					filavaloracion++;
				}

				// Dietas y Horas
				string texto = "";
				if (g.Desayuno > 0) texto += String.Format("Desayuno: {0:0.00}  ", g.Desayuno);
				if (g.Comida > 0) texto += String.Format("Comida: {0:0.00}  ", g.Comida);
				if (g.Cena > 0) texto += String.Format("Cena: {0:0.00}  ", g.Cena);
				if (g.PlusCena > 0) texto += String.Format("Plus Cena: {0:0.00}  ", g.PlusCena);
				if (g.PlusLimpieza) texto += "Limp.  ";
				if (g.PlusPaqueteria) texto += "Paqu.  ";
				datos[fila + 4 + filavaloracion, 0] = texto;

				datos[fila + 4 + filavaloracion, 4] = String.Format("Trab: {0} Acum: {1} Noct: {2}",
																	(string)cnvHora.Convert(g.Trabajadas, null, null, null),
																	(string)cnvHora.Convert(g.Acumuladas, null, null, null),
																	(string)cnvHora.Convert(g.Nocturnas, null, null, null));
				// Reajustamos la fila
				fila += 5 + g.ListaValoraciones.Count;

			}
		}


		private static void SetEstadisticasEnHoja(Worksheet hoja, List<EstadisticasGraficos> lista, DateTime fecha, int fila) {
			// Escribimos la fecha del grupo
			hoja.Cells[fila - 1, 1].Value = string.Format("'{0:dd} - {0:MMM} - {0:yyyy}", fecha).ToUpper().Replace(".", "");

			// Definimos el array que se copiará
			object[,] datos = new object[4, 13];

			// ESCRIBIMOS LOS PARCIALES POR TURNO
			foreach (EstadisticasGraficos estadistica in lista) {
				datos[estadistica.Turno - 1, 0] = estadistica.NumeroGraficos;
				datos[estadistica.Turno - 1, 1] = $"=SI(C{fila + estadistica.Turno - 1}=0;;C{fila + estadistica.Turno - 1}/$C${fila + 4})";
				datos[estadistica.Turno - 1, 2] = estadistica.Valoracion.ToTexto(true);
				datos[estadistica.Turno - 1, 3] = estadistica.Trabajadas.ToTexto(true);
				datos[estadistica.Turno - 1, 4] = estadistica.Acumuladas.ToTexto(true);
				datos[estadistica.Turno - 1, 5] = estadistica.Nocturnas.ToTexto(true);
				datos[estadistica.Turno - 1, 6] = estadistica.Desayuno;
				datos[estadistica.Turno - 1, 7] = estadistica.Comida;
				datos[estadistica.Turno - 1, 8] = estadistica.Cena;
				datos[estadistica.Turno - 1, 9] = estadistica.PlusCena;
				datos[estadistica.Turno - 1, 10] = estadistica.TotalDietas;
				datos[estadistica.Turno - 1, 11] = estadistica.Limpieza;
				datos[estadistica.Turno - 1, 12] = estadistica.Paqueteria;
			}

			// PONEMOS LOS DATOS EN LA HOJA
			hoja.Range[$"C{fila}:O{fila + 3}"].FormulaLocal = datos;
		}


		private static void SetEstadisticasEnHojaPorCentros(Worksheet hoja, List<EstadisticasGraficos> lista, int fila) {
			
			// Definimos el array que se copiará
			object[,] datos = new object[4, 13];

			// ESCRIBIMOS LOS PARCIALES POR TURNO
			foreach (EstadisticasGraficos estadistica in lista) {
				datos[estadistica.Turno - 1, 0] = estadistica.NumeroGraficos;
				datos[estadistica.Turno - 1, 1] = $"=SI(C{fila + estadistica.Turno - 1}=0;;C{fila + estadistica.Turno - 1}/$C${fila + 4})";
				datos[estadistica.Turno - 1, 2] = estadistica.Valoracion.ToTexto(true);
				datos[estadistica.Turno - 1, 3] = estadistica.Trabajadas.ToTexto(true);
				datos[estadistica.Turno - 1, 4] = estadistica.Acumuladas.ToTexto(true);
				datos[estadistica.Turno - 1, 5] = estadistica.Nocturnas.ToTexto(true);
				datos[estadistica.Turno - 1, 6] = estadistica.Desayuno;
				datos[estadistica.Turno - 1, 7] = estadistica.Comida;
				datos[estadistica.Turno - 1, 8] = estadistica.Cena;
				datos[estadistica.Turno - 1, 9] = estadistica.PlusCena;
				datos[estadistica.Turno - 1, 10] = estadistica.TotalDietas;
				datos[estadistica.Turno - 1, 11] = estadistica.Limpieza;
				datos[estadistica.Turno - 1, 12] = estadistica.Paqueteria;
			}

			// PONEMOS LOS DATOS EN LA HOJA
			hoja.Range[$"C{fila}:O{fila + 3}"].FormulaLocal = datos;
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public static async Task CrearGraficosEnPdf(Workbook libro, ListCollectionView lista, DateTime fecha) {

			await Task.Run(() => {
				// Definimos la hoja a usar.
				Worksheet hoja = libro.Worksheets[1];
				// Definimos el título y el centro TODO: Pasar al parser...
				hoja.Range["A1"].Value = string.Format("{0:dd} - {0:MMMM} - {0:yyyy}", fecha).ToUpper();
				hoja.Range["S1"].Value = App.Global.CentroActual.ToString().ToUpper();
				// Llenamos la plantilla con los gráficos.
				string[,] datos = RangoGraficos(lista);
				// Creamos el rango que ocupan los gráficos.
				Range rango = hoja.Range["A3", hoja.Cells[lista.Count + 2, 19]];
				// Rellenamos los bordes de los calendarios.
				rango.Borders.LineStyle = XlLineStyle.xlContinuous;
				rango.Borders.Color = XlRgbColor.rgbBlack;
				rango.Borders.Weight = XlBorderWeight.xlThin;
				rango.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);
				// Copiamos los datos a la hoja.
				rango.Value = datos;
				// Coloreamos las filas pares.
				for (int m = 1; m <= lista.Count; m++) {
					// Pasamos el valor de progreso.
					double valor = Convert.ToDouble(m) / lista.Count * 100;
					App.Global.ValorBarraProgreso = valor;

					object x = hoja.Cells[m + 2, 11].Value;
					string n = x?.GetType().Name;
					if (hoja.Cells[m + 2, 11].Value != null) {
						hoja.Range[hoja.Cells[m + 2, 9], hoja.Cells[m + 2, 11]].Font.Color = Color.DeepPink;
					}
					if (m % 2 == 0) hoja.Range[hoja.Cells[m + 2, 1], hoja.Cells[m + 2, 19]].Interior.Color = Color.FromArgb(255, 226, 239, 218);
				}
				// Establecemos el área de impresión.
				hoja.PageSetup.PrintArea = hoja.Range["A1", hoja.Cells[lista.Count + 2, 19]].Address;
			});
		}


		public static async Task CrearGraficosIndividualesEnPdf(Workbook libro, ListCollectionView lista, DateTime fecha) {

			await Task.Run(() => {

				// Definimos la hoja.
				Worksheet Hoja = libro.Worksheets[1];
				// Definimos el número de valoraciones totales a pegar.
				int valoraciones = 0;
				// Formateamos cada gráfico en la hoja.
				int fila = 1;
				double num = 1;

				foreach (object obj in lista) {
					double valor = num / lista.Count * 100;
					App.Global.ValorBarraProgreso = valor;
					num++;
					Grafico g = obj as Grafico;
					if (g == null) continue;
					// Sumamos las valoraciones al global.
					valoraciones += g.ListaValoraciones.Count;
					// Altura de las filas.
					Hoja.Range[fila.ToString() + ":" + fila.ToString()].RowHeight = 35;
					Hoja.Range[(fila + 1).ToString() + ":" + (fila + 3).ToString()].RowHeight = 25;
					Hoja.Range[(fila + 4 + g.ListaValoraciones.Count).ToString() + ":" + (fila + 4 + g.ListaValoraciones.Count).ToString()].RowHeight = 35;
					// Estilos de texto
					Hoja.Range[fila.ToString() + ":" + fila.ToString()].Font.Size = 22;
					Hoja.Range[(fila + 2).ToString() + ":" + (fila + 2).ToString()].Font.Size = 16;
					Hoja.Range[(fila + 3).ToString() + ":" + (fila + 3).ToString()].Font.Size = 14;
					Hoja.Range[fila.ToString() + ":" + (fila + 3).ToString()].Font.Bold = true;
					Hoja.Range[(fila + 4 + g.ListaValoraciones.Count).ToString() + ":" + (fila + 4 + g.ListaValoraciones.Count).ToString()].Font.Size = 16;
					Hoja.Range[(fila + 4 + g.ListaValoraciones.Count).ToString() + ":" + (fila + 4 + g.ListaValoraciones.Count).ToString()].Font.Bold = true;
					// Alineaciones
					Hoja.Cells[fila, 1].HorizontalAlignment = XlHAlign.xlHAlignLeft;
					Hoja.Cells[fila, 5].HorizontalAlignment = XlHAlign.xlHAlignRight;
					Hoja.Cells[fila + 2, 1].HorizontalAlignment = XlHAlign.xlHAlignLeft;
					Hoja.Cells[fila + 2, 5].HorizontalAlignment = XlHAlign.xlHAlignRight;
					Hoja.Cells[fila + 4 + g.ListaValoraciones.Count, 1].HorizontalAlignment = XlHAlign.xlHAlignLeft;
					Hoja.Cells[fila + 4 + g.ListaValoraciones.Count, 5].HorizontalAlignment = XlHAlign.xlHAlignRight;
					// Colores
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3, 5]].Interior.Color = Color.FromArgb(255, 112, 173, 71);
					// Bordes.
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3 + g.ListaValoraciones.Count, 5]].Borders.LineStyle = XlLineStyle.xlContinuous;
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3 + g.ListaValoraciones.Count, 5]].Borders.Color = XlRgbColor.rgbBlack;
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3 + g.ListaValoraciones.Count, 5]].Borders.Weight = XlBorderWeight.xlThin;
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3, 5]].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium,
																							  XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);
					Hoja.Range[Hoja.Cells[fila + 3, 1], Hoja.Cells[fila + 3 + g.ListaValoraciones.Count, 5]].BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium,
																											 XlColorIndex.xlColorIndexNone, XlRgbColor.rgbBlack);

					// Colores de las líneas pares de las valoraciones
					for (int i = 0; i < g.ListaValoraciones.Count; i++) {
						if (i % 2 != 0) Hoja.Range[Hoja.Cells[fila + i + 4, 1], Hoja.Cells[fila + i + 4, 5]].Interior.Color = Color.FromArgb(255, 226, 239, 218);
					}

					// Saltos de página
					Hoja.HPageBreaks.Add(Hoja.Cells[fila + 5 + g.ListaValoraciones.Count, 1]);

					// Líneas totales y parciales
					fila = fila + 5 + g.ListaValoraciones.Count;
				}

				// Definimos el array con los datos...
				string[,] datos = new string[(5 * lista.Count + valoraciones), 5];
				// Extraemos los datos de la lista de gráficos
				SetRangoGraficosIndividuales(lista, fecha, ref datos);
				// Pegamos los datos.
				Hoja.Range["A1", Hoja.Cells[(5 * lista.Count + valoraciones), 5]].Value = datos;
				// Establecemos el área de impresión.
				Hoja.PageSetup.PrintArea = Hoja.Range["A1", Hoja.Cells[(5 * (lista.Count - 1) + valoraciones), 5]].Address;

			});
		}


		public static async Task CrearEstadisticasGraficosEnPdf(Workbook libro, DateTime fecha, long idGrupoGrafico) {

			await Task.Run(() => {
				// Definimos las hojas.
				Worksheet Hoja = libro.Worksheets[1]; // Hoja de datos
													  // Definimos y llenamos la lista de estadisticas.
				List<EstadisticasGraficos> lista = BdGraficos.GetEstadisticasGrupoGraficos(idGrupoGrafico);
				// Definimos el título y el centro de la hoja de datos.
				Hoja.Range["O1"].Value = App.Global.CentroActual.ToString().ToUpper();
				// Escribimos las estadísticas en la hoja.
				SetEstadisticasEnHoja(Hoja, lista, fecha, 4);

				// Establecemos la gráfica y la serie de la gráfica.
				//Chart grafica = Hoja2.ChartObjects("Grafico1").Chart;
				Chart grafica = libro.Charts[1];
				Series serie = grafica.SeriesCollection(1);
				// Establecemos el título de la serie y su formato.
				//serie.Name = "Grupo de gráficos => " + fecha.ToString("dd-MMM-yyyy").Replace(".", "");
				//serie.Name = "Días trabajados por turno.";
				grafica.ChartTitle.Text = "Porcentaje de gráficos por turno.";
				//grafica.ChartTitle.Top = 410;
				//grafica.ChartTitle.Font.Size = 22;
				//grafica.ChartTitle.Font.Bold = true;
				//grafica.ChartTitle.Font.Color = Color.FromArgb(0, 255, 255, 153);
				grafica.ChartTitle.Left = (grafica.ChartArea.Width - grafica.ChartTitle.Width) / 2;
				grafica.ChartArea.RoundedCorners = true;
				//grafica.HasLegend = false;

				// Establecemos el origen de datos de la serie.
				serie.Values = Hoja.Range["D4:D7"];
				//serie.HasDataLabels = true;
				// Formateamos la serie. Esto lo dejaremos fijo en la plantilla.
				//foreach (DataLabel dl in serie.DataLabels()) {
				//	dl.Font.Size = 16;
				//	dl.Font.Color = Color.FromArgb(0, 255, 255, 255);
				//	dl.Separator = ": ";
				//	dl.Position = XlDataLabelPosition.xlLabelPositionOutsideEnd;
				//	dl.ShowCategoryName = true;
				//	dl.ShowValue = true;
				//}
				//serie.HasLeaderLines = false;
				//serie.Explosion = 10; // Con esto, separamos los sectores.


				// Seleccionamos todas las hojas.
				libro.Worksheets.Select();
			});

		}


		public static async Task CrearEstadisticasGruposGraficosEnPdf(Workbook libro, DateTime fecha) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = libro.Worksheets[1];
				// Definimos y llenamos la lista de estadisticas.
				List<EstadisticasGraficos> lista = BdGraficos.GetEstadisticasGraficosDesdeFecha(fecha);
				// Definimos el título y el centro
				Hoja.Range["O1"].Value = App.Global.CentroActual.ToString().ToUpper();
				// Establecemos la fila inicial, los grupos procesados la última fecha procesada y la lista temporal.
				int fila = 0;
				int grupos = 0;
				int totalgrupos = 0;
				double num = 1;
				DateTime ultimafecha = new DateTime(0);
				List<EstadisticasGraficos> listatemporal = new List<EstadisticasGraficos>();


				// Pasamos centro a centro y lo generamos.
				foreach (EstadisticasGraficos estadistica in lista) {
					double valor = num / lista.Count * 100;
					//modificaBarra.Invoke(valor);
					App.Global.ValorBarraProgreso = valor;
					num++;
					// Si la fecha es igual a la última, la añadimos a la lista.
					if (estadistica.Validez == ultimafecha) {
						listatemporal.Add(estadistica);
						// Si no es igual, reiniciamos la lista temporal, 
					} else {
						if (fila == 0) {
							listatemporal.Add(estadistica);
							fila = 4;
							grupos = 1;
							ultimafecha = estadistica.Validez;
							continue;
						}
						if (fila > 4) Hoja.Range["A3:O8"].Copy(Hoja.Cells[fila - 1, 1]);
						if (grupos == 3) {
							Hoja.HPageBreaks.Add(Hoja.Cells[fila + 7, 1]);
						}
						SetEstadisticasEnHoja(Hoja, listatemporal, ultimafecha, fila);
						totalgrupos++;
						listatemporal = new List<EstadisticasGraficos>();
						listatemporal.Add(estadistica);
						fila += 8;
						grupos = grupos == 3 ? 1 : grupos + 1;
						ultimafecha = estadistica.Validez;
					}
					if (totalgrupos > 0) {
						if (fila > 3) Hoja.Range["A3:O8"].Copy(Hoja.Cells[fila - 1, 1]);
						SetEstadisticasEnHoja(Hoja, listatemporal, ultimafecha, fila);
					}
				}


				// Establecemos el rango de impresión.
				Hoja.PageSetup.PrintArea = Hoja.Range["A1", Hoja.Cells[fila + 4, 15]].Address;
				// Seleccionamos sólo la primera hoja del libro.
				libro.Worksheets[1].Select();
			});

		}


		public static async Task CrearEstadisticasGraficosPorCentros(Workbook libro, List<EstadisticasGraficos> lista) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = libro.Worksheets[1];
				// Definimos la lista temporal que se va a usar.
				List<EstadisticasGraficos> listaTemporal;
				// Establecemos las estadisticas de Bilbao.
				App.Global.ValorBarraProgreso = 25;
				listaTemporal = lista.Where(e => e.Centro == Centros.Bilbao).ToList();
				if (listaTemporal.Count > 0) {
					DateTime fechaTemporal = listaTemporal[0].Validez;
					SetEstadisticasEnHojaPorCentros(Hoja, listaTemporal, 4);
					Hoja.Cells[4, 1].Value = string.Format($"'BILBAO\n{fechaTemporal:dd}-{fechaTemporal:MMM}-{fechaTemporal:yyyy}").ToUpper().Replace(".", "");
				}
				// Establecemos las estadisticas de Donosti.
				App.Global.ValorBarraProgreso = 50;
				listaTemporal = lista.Where(e => e.Centro == Centros.Donosti).ToList();
				if (listaTemporal.Count > 0) {
					DateTime fechaTemporal = listaTemporal[0].Validez;
					SetEstadisticasEnHojaPorCentros(Hoja, listaTemporal, 9);
					Hoja.Cells[9, 1].Value = string.Format($"'DONOSTI\n{fechaTemporal:dd}-{fechaTemporal:MMM}-{fechaTemporal:yyyy}").ToUpper().Replace(".", "");
				}
				// Establecemos las estadisticas de Arrasate.
				App.Global.ValorBarraProgreso = 75;
				listaTemporal = lista.Where(e => e.Centro == Centros.Arrasate).ToList();
				if (listaTemporal.Count > 0) {
					DateTime fechaTemporal = listaTemporal[0].Validez;
					SetEstadisticasEnHojaPorCentros(Hoja, listaTemporal, 14);
					Hoja.Cells[14, 1].Value = string.Format($"'ARRASATE\n{fechaTemporal:dd}-{fechaTemporal:MMM}-{fechaTemporal:yyyy}").ToUpper().Replace(".", "");
				}
				// Establecemos las estadisticas de Vitoria.
				App.Global.ValorBarraProgreso = 100;
				listaTemporal = lista.Where(e => e.Centro == Centros.Vitoria).ToList();
				if (listaTemporal.Count > 0) {
					DateTime fechaTemporal = listaTemporal[0].Validez;
					SetEstadisticasEnHojaPorCentros(Hoja, listaTemporal, 19);
					Hoja.Cells[19, 1].Value = string.Format($"'GASTEIZ\n{fechaTemporal:dd}-{fechaTemporal:MMM}-{fechaTemporal:yyyy}").ToUpper().Replace(".", "");
				}

				// Establecemos los parámetros de la gráfica.
				Chart grafica = libro.Charts[1];
				grafica.ChartTitle.Text = "Número de gráficos por turno.";
				grafica.ChartTitle.Left = (grafica.ChartArea.Width - grafica.ChartTitle.Width) / 2;
				grafica.ChartArea.RoundedCorners = true;

				// Seleccionamos todas las hojas del libro.
				libro.Worksheets.Select();
			});


		}

		#endregion



	}

}
