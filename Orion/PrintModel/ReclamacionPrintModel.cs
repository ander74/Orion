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
using System.Threading.Tasks;
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

		public static async Task CrearReclamacion(Workbook Libro, string[,] datos, DateTime fecha, Conductor trabajador, DateTime fechaReclamacion, string notas) {

			await Task.Run(() => {
				// Definimos la hoja.
				Worksheet Hoja = Libro.Worksheets[1];
				// Establecemos el centro, la fecha y el conductor.
				Hoja.Range["A6"].Value = $"Centro: {App.Global.CentroActual.ToString().ToUpper()}";
				Hoja.Range["A8"].Value = $"Trabajador/a: {trabajador.Apellidos} ({trabajador.Id:000})";
				Hoja.Range["A10"].Value = $"{fecha:MMMM - yyyy}".ToUpper();
				Hoja.Range["D10"].Value = $"'Reclamación Nº: {fechaReclamacion:yyyyMMdd}{trabajador.Id:000}";
				Hoja.Range["C32"].Value = $"'{fechaReclamacion:dd - MM - yyyy}";
				Hoja.Range["A32"].Value = notas;
				// Pegamos los datos
				Range rango = Hoja.Range["A13:D29"];
				rango.Value = datos;
			});



		}

		#endregion
		// ====================================================================================================



	}
}
