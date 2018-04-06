#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Microsoft.Office.Interop.Excel;
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;
using Orion.Properties;
using Orion.Views;
using System;
using System.Collections.Generic;
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

		[Obsolete("Este método usa Excel y ya no es necesario. Usar GenerarReclamación")]
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


		public static async Task GenerarReclamacion(Centros centro, Conductor conductor, DateTime fecha, PdfDocument docPdf) {

			await Task.Run(() => {

				// Extraemos los campos del formulario que contiene el PDF.
				PdfAcroForm formulario = PdfAcroForm.GetAcroForm(docPdf, true);
				IDictionary<String, PdfFormField> campos = formulario.GetFormFields();
				// Creamos la fuente a usar.
				PdfFont fuente = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
				// Definimos el campo a usar en Try.
				PdfFormField campo;
				// Centro
				if (campos.TryGetValue("Centro", out campo)) campo.SetValue(centro.ToString().ToUpper(), fuente, 16);
				// Trabajador
				if (campos.TryGetValue("Trabajador", out campo)) campo.SetValue($"{conductor.Apellidos}, {conductor.Nombre} ({conductor.Id:000})", fuente, 16);
				// FechaCabecera
				if (campos.TryGetValue("FechaCabecera", out campo)) campo.SetValue($"{fecha:MMMM - yyyy}".ToUpper(), fuente, 16);
				// NumeroReclamacion
				if (campos.TryGetValue("NumeroReclamacion", out campo)) campo.SetValue($"Nº Reclamación: {fecha:yyyyMM}{conductor.Id:000}/01", fuente, 12);
				// FechaFirma
				if (campos.TryGetValue("FechaFirma", out campo)) campo.SetValue($"{DateTime.Today:dd - MM - yyyy}", fuente, 16);
			});
		}

		#endregion
		// ====================================================================================================



	}
}
