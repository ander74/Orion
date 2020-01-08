#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Orion.Config;
using Orion.Models;

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
