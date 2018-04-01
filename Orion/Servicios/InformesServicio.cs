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
					if (rutaConductor.Trim() != "") rutaparcial = "Conductores\\" + rutaConductor.Trim() + "\\Hojas Pijama";
					ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, rutaparcial);
					break;
				case TiposInforme.Reclamacion:
					rutaparcial = "Reclamaciones";
					if (rutaConductor.Trim() != "") rutaparcial = "Conductores\\" + rutaConductor.Trim() + "\\Reclamaciones";
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
		#region MÉTODOS PÚBLICOS PDF (ITEXT 7)
		// ====================================================================================================

		

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS PDF DE PRUEBA
		// ====================================================================================================

		/// <summary>
		/// Este es un método de práctica.
		/// </summary>
		public void CrearPDF() {

			// Así se crea o se abre un documento.
			FileStream fs = new FileStream("MiPdf.pdf", FileMode.Create); // Creamos el FileStream del documento a crear o abrir.
			Document doc = new Document(PageSize.A4, 25, 25, 25, 25); // Creamos un documento.
			PdfWriter pdf = PdfWriter.GetInstance(doc, fs); // Creamos una nueva instancia del escritor de PDF pasando el documento y el FileStream.

			//pdf.SetEncryption(PdfWriter.ENCRYPTION_AES_128, "ander74", "ander74", PdfWriter.AllowPrinting);

			Document d2 = LlenarDocumentoPDF();

			PdfDocument d = new PdfDocument();
			
		
			


			pdf.Close();
			//fs.Close(); // El filestream no es necesario cerrarlo, ya que se cierra automáticamente al cerrar el documento.





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


		public Document LlenarDocumentoPDF() {


			Document doc = new Document(PageSize.A4, 25, 25, 25, 25); // Creamos un documento.
																	  // Para trabajar con el documento, se abre y se trabaja con el mismo y no el writer.
			doc.Open();
			// Se pueden añadir metadatos.
			doc.AddAuthor("A. Herrero");
			doc.AddTitle("Mi primer documento PDF");
			doc.AddSubject("Este es un documento de prueba.");

			PdfPTable inicial = new PdfPTable(new float[] { 60, 5, 40 });
			inicial.WidthPercentage = 100;

			// Creamos una tabla para ver si podemos reutilizar las celdas.
			PdfPTable tabla = new PdfPTable(3);
			tabla.WidthPercentage = 60;
			tabla.HorizontalAlignment = Element.ALIGN_LEFT;
			tabla.SetWidths(new int[] { 1, 1, 3 });
			tabla.HeaderRows = 1; // Determinamos las filas que son encabezado, para que se repitan en las páginas nuevas.

			// Establecemos la fuente de los encabezados y las celdas.
			iTextSharp.text.Font fuenteEncabezados = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLUE);
			iTextSharp.text.Font fuenteCeldas = FontFactory.GetFont(FontFactory.HELVETICA, 10, iTextSharp.text.Font.NORMAL);

			// Creamos las tres celdas de la fila
			PdfPCell celdaEncabezado = new PdfPCell();
			celdaEncabezado.Colspan = 1;
			celdaEncabezado.Padding = 5;
			celdaEncabezado.BackgroundColor = BaseColor.ORANGE;
			celdaEncabezado.HorizontalAlignment = Element.ALIGN_CENTER;
			celdaEncabezado.VerticalAlignment = Element.ALIGN_MIDDLE;

			PdfPCell celdaNormal = new PdfPCell();
			celdaNormal.Colspan = 1;
			celdaNormal.Padding = 5;
			celdaNormal.BackgroundColor = BaseColor.WHITE;
			celdaNormal.HorizontalAlignment = Element.ALIGN_CENTER;
			celdaNormal.VerticalAlignment = Element.ALIGN_MIDDLE;

			celdaEncabezado.Phrase = new Phrase("ID", fuenteEncabezados);
			tabla.AddCell(celdaEncabezado);
			celdaEncabezado.Phrase = new Phrase("NOMBRE", fuenteEncabezados);
			tabla.AddCell(celdaEncabezado);
			celdaEncabezado.Phrase = new Phrase("APELLIDOS", fuenteEncabezados);
			tabla.AddCell(celdaEncabezado);


			celdaNormal.Phrase = new Phrase("5060", fuenteCeldas);
			tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Andrés", fuenteCeldas);
			tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Herrero Módenes", fuenteCeldas);
			tabla.AddCell(celdaNormal);

			celdaNormal.Phrase = new Phrase("4935", fuenteCeldas);
			//celdaNormal.Rowspan = 3;
			tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Joseba", fuenteCeldas);
			celdaNormal.Rowspan = 1;
			tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Moyano Reyero.\n\nAsignamos más texto para ver si se hace wrapping automáticamente y podemos trabajar con los fallos de calendario y ahorrar al máximo el uso de excel.", fuenteCeldas);
			celdaNormal.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
			tabla.AddCell(celdaNormal);
			celdaNormal.HorizontalAlignment = Element.ALIGN_CENTER;

			celdaNormal.Phrase = new Phrase("5060", fuenteCeldas);
			tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Andrés", fuenteCeldas);
			celdaNormal.Colspan = 2;
			//tabla.AddCell(celdaNormal);
			celdaNormal.Phrase = new Phrase("Herrero Módenes", fuenteCeldas);
			tabla.AddCell(celdaNormal);

			//for (int m = 1; m < 60; m++) {
			//	celdaNormal.Phrase = new Phrase("5069", fuenteCeldas);
			//	tabla.AddCell(celdaNormal);
			//	celdaNormal.Phrase = new Phrase("Txus", fuenteCeldas);
			//	tabla.AddCell(celdaNormal);
			//	celdaNormal.Phrase = new Phrase("Alberto González", fuenteCeldas);
			//	tabla.AddCell(celdaNormal);
			//}

			PdfPCell c1 = new PdfPCell(tabla);
			c1.BorderWidth = 0;


			// Se crea un párrafo.
			Paragraph parrafo = new Paragraph("Hola Mundo\n\nEste es el primer PDF que creo con clave.\n\n");
			// Añadimos el párrafo al documento.
			PdfPCell c2 = new PdfPCell(parrafo);
			c2.BorderWidth = 0;

			inicial.AddCell(c1);
			inicial.AddCell(new PdfPCell() { BorderWidth = 0 });
			inicial.AddCell(c2);

			doc.Add(inicial);


			// Cerramos el documento, el escritor y el FileStream (en este orden).
			doc.Close();

			return doc;

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
