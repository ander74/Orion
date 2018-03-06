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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Data;

namespace Orion.PrintModel {

	public static class ReclamacionPrintModel {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private static string rutaPlantillaReclamacion = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Reclamacion.xlsx");

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public static void CrearReclamacion(string[,] datos, DateTime fecha, Conductor trabajador, string ruta, DateTime fechaReclamacion) {

			// Creamos una aplicacion Excel y la mantenemos oculta.
			Application ExcelApp = new Application();
			// Creamos el libro
			Workbook Libro = null;

			try {
				ExcelApp.Visible = false;
				// Creamos el libro desde la plantilla.
				Libro = ExcelApp.Workbooks.Add(rutaPlantillaReclamacion);
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Establecemos el centro, la fecha y el conductor.
				Hoja.Range["A6"].Value = $"Centro: {App.Global.CentroActual.ToString().ToUpper()}";
				Hoja.Range["D6"].Value = $"{fecha:MMMM - yyyy}".ToUpper();
				Hoja.Range["A8"].Value = $"Trabajador: {trabajador.Apellidos} ({trabajador.Id:000})";
				Hoja.Range["A10"].Value = $"'Fecha: {fechaReclamacion:dd/MM/yyyy}";
				Hoja.Range["D10"].Value = $"'Reclamación Nº: {fechaReclamacion:yyyyMMdd}{trabajador.Id:000}";
				// Pegamos los datos
				Range rango = Hoja.Range["A13:D29"];
				rango.Value = datos;
				// Exportamos el libro como PDF.
				Libro.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ruta);
				// Si hay que abrir el PDF, se abre.
				if (Settings.Default.AbrirPDFs) Process.Start(ruta);
			} finally {
				if (Libro != null) Libro.Close(false);
				if (ExcelApp != null) ExcelApp.Quit();
			}



		}

		#endregion
		// ====================================================================================================



	}
}
