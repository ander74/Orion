#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Orion.Config;

namespace Orion.Servicios {

	/// <summary>
	/// Enumera los tipos de informe que se pueden generar con Orión.
	/// </summary>
	public enum TiposInforme {
		Ninguno,
		Graficos,
		EstadisticasGraficos,
		GraficoIndividual,
		EstadisticasGraficosPorCentros,
		Calendarios,
		FallosCalendarios,
		Reclamacion,
		Pijama
	}


	public class InformesServicio: IDisposable {


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		/// <summary>
		/// Muestra un cuadro de diálogo pidiendo la ruta para un archivo y devuelve la ruta.
		/// </summary>
		public string GetRutaArchivo(TiposInforme tipo, string nombreArchivo) {
			string resultado = "";
			string ruta = "";
			switch (tipo) {
				case TiposInforme.Graficos:
				case TiposInforme.GraficoIndividual:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Graficos");
					break;
				case TiposInforme.Calendarios:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Calendarios");
					break;
				case TiposInforme.FallosCalendarios:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Calendarios");
					break;
				case TiposInforme.EstadisticasGraficos:
				case TiposInforme.EstadisticasGraficosPorCentros:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Estadisticas Graficos");
					break;
				case TiposInforme.Pijama:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Hojas Pijama");
					break;
				case TiposInforme.Reclamacion:
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Reclamaciones");
					break;
			}
			if (ruta != "") {
				if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
				SaveFileDialog dialogo = new SaveFileDialog();
				dialogo.Filter = "Archivos PDF|*.pdf|Todos los archivos|*.*";
				dialogo.FileName = nombreArchivo;
				dialogo.InitialDirectory = ruta;
				dialogo.OverwritePrompt = true;
				dialogo.Title = "Guardar Informe";
				if (dialogo.ShowDialog() == true) {
					resultado = dialogo.FileName;
				}
			}
			return resultado;
		}

		/// <summary>
		/// Devuelve un archivo de Excel en blanco o con la plantilla indicada.
		/// </summary>
		public Workbook GetArchivoExcel(TiposInforme tipo = TiposInforme.Ninguno) {
			// Definimos el libro que se va a devolver.
			Workbook libro = null;
			if (tipo == TiposInforme.Ninguno) {
				// Si no hay una plantilla, creamos un archivo Excel en blanco.
				libro = ExcelApp.Workbooks.Add();
			} else {
				// Si hay una plantilla creamos un archivo Excel desde la plantilla.
				string ruta = Utils.CombinarCarpetas(App.RutaInicial, $"/Plantillas/{tipo}.xlsx");
				libro = ExcelApp.Workbooks.Add(ruta);
			}
			// Devolvemos el libro.
			return libro;
		}


		/// <summary>
		/// Exporta a PDF el libro pasado en la ruta pasada, y lo cierra.
		/// </summary>
		public void ExportarLibroToPdf(Workbook libro, string ruta) {
			try {
				libro.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ruta);
				if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
			} finally {
				libro.Close(false);
			}
		}


		/// <summary>
		/// Exporta a PDF el libro pasado en la ruta pasada, y lo cierra.
		/// </summary>
		public void ExportarHojaToPdf(Workbook libro, int hoja, string ruta) {
			try {
				libro.Worksheets[hoja].ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ruta);
				if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
			} finally {
				libro.Close(false);
			}
		}


		/// <summary>
		/// Cierra la instancia de Excel usada para crear los informes SIN guardar los cambios que haya y SIN avisar de ello.
		/// </summary>
		public void Dispose() {
			if (_excelapp != null) {
				_excelapp.DisplayAlerts = false;
				_excelapp.Quit();
			}
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private Application _excelapp;
		public Application ExcelApp {
			get {
				if (_excelapp == null) _excelapp = new Application { Visible = false };
				return _excelapp;
			}
		}



		#endregion
		// ====================================================================================================




	}
}
