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
using iTextSharp.text.pdf;
using iTextSharp;
using iTextSharp.text;
using Orion.Models;

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
		#region MÉTODOS PÚBLICOS EXCEL
		// ====================================================================================================

		/// <summary>
		/// Muestra un cuadro de diálogo pidiendo la ruta para un archivo y devuelve la ruta.
		/// </summary>
		public string GetRutaArchivo(TiposInforme tipo, string nombreArchivo, bool crearInformeDirectamente, string rutaConductor = "") {
			string resultado = "";
			string rutaparcial = "";
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
					rutaparcial = "Hojas Pijama";
					if (rutaConductor != "") rutaparcial = "Conductores\\" + rutaConductor + "\\Hojas Pijama";
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, rutaparcial);
					break;
				case TiposInforme.Reclamacion:
					rutaparcial = "Reclamaciones";
					if (rutaConductor != "") rutaparcial = "Conductores\\" + rutaConductor + "\\Reclamaciones";
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, rutaparcial);
					break;
			}
			if (ruta != "") {
				if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
				if (crearInformeDirectamente) {
					resultado = Path.Combine(ruta, nombreArchivo);
				} else {
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
		public void ExportarLibroToPdf(Workbook libro, string ruta, bool abrirPDF) {
			try {
				libro.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, ruta);
				if (abrirPDF) Process.Start(ruta);
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
		#region MÉTODOS PÚBLICOS PDF
		// ====================================================================================================

		/// <summary>
		/// Este es un método de práctica.
		/// </summary>
		public void CrearPDF() {

			// Así se crea o se abre un documento.
			FileStream fs = new FileStream("MiPdf.pdf", FileMode.Create); // Creamos el FileStream del documento a crear o abrir.
			Document doc = new Document(PageSize.A4, 25, 25, 25, 25); // Creamos un documento.
			PdfWriter pdf = PdfWriter.GetInstance(doc, fs); // Creamos una nueva instancia del escritor de PDF pasando el documento y el FileStream.

			pdf.SetEncryption(PdfWriter.ENCRYPTION_AES_128, "ander74", "ander74", PdfWriter.AllowPrinting);
			// Para trabajar con el documento, se abre y se trabaja con el mismo y no el writer.
			doc.Open();
			// Se pueden añadir metadatos.
			
			doc.AddAuthor("A. Herrero");
			doc.AddTitle("Mi primer documento PDF");
			doc.AddSubject("Este es un documento de prueba.");
			// Se crea un párrafo.
			Paragraph parrafo = new Paragraph("Hola Mundo\n\nEste es el primer PDF que creo con clave.");
			// Añadimos el párrafo al documento.
			doc.Add(parrafo);
			// Cerramos el documento, el escritor y el FileStream (en este orden).
			doc.Close();
			pdf.Close();
			fs.Close(); // Tambien podemos poner el FileStream en un using y así que se cierre automáticamente.


			


		}


		public void RellenarPDF() {

			// Creamos el lector del documento.
			PdfReader reader = new PdfReader("Plantillas\\Reclamacion.pdf");
			// Creamos el 'modificador' del documento.
			FileStream fs = new FileStream("MiPdf.pdf", FileMode.Create); // Creamos el FileStream del documento a crear.
			PdfStamper stamper = new PdfStamper(reader, fs);
			
			// Extraemos los campos del documento.
			AcroFields campos = stamper.AcroFields;

			// Asignamos los campos
			campos.SetField("Centro", "Bilbao");
			campos.SetField("Trabajador", "J.A. Cisneros (450)");
			campos.SetField("FechaCabecera", "MARZO - 2018");
			campos.SetField("NumeroReclamacion", "Nº Reclamación: 20180328450");
			campos.SetField("FechaFirma", "28 - 03 - 2018");
			campos.SetField("Notas", "Estas son notas de ejemplo.\n\nAquí va otra línea.");


			// Hacemos que ya no se puedan rellenar los campos.
			//stamper.FormFlattening = true;

			// Cerramos los elementos abiertos
			stamper.Close();
			fs.Close();
			reader.Close();




		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS RECLAMACIONES
		// ====================================================================================================

		public void GenerarReclamación(Centros centro, Conductor conductor, DateTime fecha, string ruta) {

			// Creamos el lector del documento.
			string rutaPlantilla = Utils.CombinarCarpetas(App.RutaInicial, $"/Plantillas/Reclamacion.pdf");
			PdfReader reader = new PdfReader(rutaPlantilla);
			// Creamos el 'modificador' del documento.
			FileStream fs = new FileStream(ruta, FileMode.Create);
			PdfStamper stamper = new PdfStamper(reader, fs);

			// Extraemos los campos del documento.
			AcroFields campos = stamper.AcroFields;

			// Asignamos los campos
			campos.SetField("Centro", centro.ToString().ToUpper());
			campos.SetField("Trabajador", $"{conductor.Apellidos}, {conductor.Nombre} ({conductor.Id:000})");
			campos.SetField("FechaCabecera", $"{fecha:MMMM - yyyy}".ToUpper());
			campos.SetField("NumeroReclamacion", $"Nº Reclamación: {fecha:yyyyMM}{conductor.Id:000}/01");
			campos.SetField("FechaFirma", $"{DateTime.Today:dd - MM - yyyy}");

			// Cerramos los elementos abiertos
			stamper.Close();
			fs.Close();
			reader.Close();


		}



		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES EXCEL
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
