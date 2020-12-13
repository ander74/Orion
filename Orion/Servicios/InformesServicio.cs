#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.IO;
using iText.IO.Image;
using iText.Layout.Element;
using iText.Layout.Properties;
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
        Pijama,
        EstadisticasCalendarios
    }


    public class InformesServicio : IDisposable {


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS COMUNES
        // ====================================================================================================

        /// <summary>
        /// Muestra un cuadro de diálogo pidiendo la ruta para un archivo y devuelve la ruta.
        /// </summary>
        public string GetRutaArchivo(TiposInforme tipo, string nombreArchivo, bool crearInformeDirectamente, string rutaConductor = "") {
            string resultado = "";
            string rutaparcial = "";
            string ruta = "";
            var carpetaInformes = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Informes\\{App.Global.CentroActual}");
            switch (tipo) {
                case TiposInforme.Graficos:
                    ruta = Path.Combine(carpetaInformes, "Graficos\\Tablas");
                    break;
                case TiposInforme.GraficoIndividual:
                    ruta = Path.Combine(carpetaInformes, "Graficos\\Individuales");
                    break;
                case TiposInforme.Calendarios:
                    ruta = Path.Combine(carpetaInformes, "Calendarios\\Tablas");
                    break;
                case TiposInforme.FallosCalendarios:
                    ruta = Path.Combine(carpetaInformes, "Calendarios\\Fallos");
                    break;
                case TiposInforme.EstadisticasGraficos:
                case TiposInforme.EstadisticasGraficosPorCentros:
                    ruta = Path.Combine(carpetaInformes, "Graficos\\Estadisticas");
                    break;
                case TiposInforme.Pijama:
                    rutaparcial = "Calendarios\\Pijamas";
                    if (rutaConductor.Trim() != "") rutaparcial = "Conductores\\" + rutaConductor.Trim() + "\\Pijamas";
                    ruta = Path.Combine(carpetaInformes, rutaparcial);
                    break;
                case TiposInforme.Reclamacion:
                    rutaparcial = "Reclamaciones";
                    if (rutaConductor.Trim() != "") rutaparcial = "Conductores\\" + rutaConductor.Trim() + "\\Reclamaciones";
                    ruta = Path.Combine(carpetaInformes, rutaparcial);
                    break;
                case TiposInforme.EstadisticasCalendarios:
                    ruta = Path.Combine(carpetaInformes, "Calendarios\\Estadisticas");
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



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS EXCEL
        // ====================================================================================================

        /// <summary>
        /// Cierra la instancia de Excel usada para crear los informes SIN guardar los cambios que haya y SIN avisar de ello.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
            }
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS PDF (ITEXT 7)
        // ====================================================================================================

        /// <summary>
        /// Devuelve un documento PDF nuevo en formato A4 en la ruta que se pasa como argumento.
        /// </summary>
        /// <param name="ruta">Ruta completa (nombre de archivo incluido) en la que se guardará el documento PDF.</param>
        /// <param name="apaisado">Si es true, el documento estará en apaisado. Por defecto es false.</param>
        /// <returns>El documento PDF listo para trabajar en él.</returns>
        public iText.Layout.Document GetNuevoPdf(string ruta, bool apaisado = false) {

            // Se crea el Writer con la ruta pasada.
            iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(ruta);
            // Se crea el PDF que se guardará usando el writer.
            iText.Kernel.Pdf.PdfDocument docPDF = new iText.Kernel.Pdf.PdfDocument(writer);
            // Añadimos los datos de Metadata
            docPDF.GetDocumentInfo().SetAuthor("Orion - AnderSoft - A.Herrero");
            docPDF.GetDocumentInfo().SetCreator("Orion 1.1");
            // Creamos el tamaño de página y le asignamos la rotaciñon si debe ser apaisado.
            iText.Kernel.Geom.PageSize tamañoPagina;
            if (apaisado) {
                tamañoPagina = iText.Kernel.Geom.PageSize.A4.Rotate();
            } else {
                tamañoPagina = iText.Kernel.Geom.PageSize.A4;
            }
            // Se crea el documento con el que se trabajará.
            iText.Layout.Document documento = new iText.Layout.Document(docPDF, tamañoPagina);
            return documento;
        }


        /// <summary>
        /// Devuelve un documento PDF nuevo en formato A5 en la ruta que se pasa como argumento.
        /// </summary>
        /// <param name="ruta">Ruta completa (nombre de archivo incluido) en la que se guardará el documento PDF.</param>
        /// <param name="apaisado">Si es true, el documento estará en apaisado. Por defecto es false.</param>
        /// <returns>El documento PDF listo para trabajar en él.</returns>
        public iText.Layout.Document GetNuevoPdfA5(string ruta, bool apaisado = false) {

            // Se crea el Writer con la ruta pasada.
            iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(ruta);
            // Se crea el PDF que se guardará usando el writer.
            iText.Kernel.Pdf.PdfDocument docPDF = new iText.Kernel.Pdf.PdfDocument(writer);
            // Añadimos los datos de Metadata
            docPDF.GetDocumentInfo().SetAuthor("Orion - AnderSoft - A.Herrero");
            docPDF.GetDocumentInfo().SetCreator("Orion 1.0");
            // Creamos el tamaño de página y le asignamos la rotaciñon si debe ser apaisado.
            iText.Kernel.Geom.PageSize tamañoPagina;
            if (apaisado) {
                tamañoPagina = iText.Kernel.Geom.PageSize.A5.Rotate();
            } else {
                tamañoPagina = iText.Kernel.Geom.PageSize.A5;
            }
            // Se crea el documento con el que se trabajará.
            iText.Layout.Document documento = new iText.Layout.Document(docPDF, tamañoPagina);
            return documento;
        }


        /// <summary>
        /// Devuelve un PDF que se guardará en la ruta indicada, a partir de la plantilla necesaria para el tipo de informe indicado.
        /// </summary>
        /// <param name="ruta">Ruta completa (nombre de archivo incluido) en la que se guardará el PDF.</param>
        /// <param name="tipo">Tipo de informe que se va a generar.</param>
        /// <returns>El PDF listo para editar sus elementos.</returns>
        public iText.Kernel.Pdf.PdfDocument GetPdfDesdePlantilla(string ruta, TiposInforme tipo = TiposInforme.Ninguno) {

            // Definimos el pdf que se va a devolver.
            iText.Kernel.Pdf.PdfDocument docPdf = null;
            // Si hay un tipo de informe creamos el pdf.
            if (tipo != TiposInforme.Ninguno) {
                // Definimos la plantilla
                string plantilla = Utils.CombinarCarpetas(App.RutaInicial, $"/Plantillas/{tipo}.pdf");
                // Se crea el Reader con la plantilla necesaria.
                iText.Kernel.Pdf.PdfReader reader = new iText.Kernel.Pdf.PdfReader(plantilla);
                // Se crea el Writer con la ruta pasada.
                iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(ruta);
                // Creamos el documento, usando el reader y el writer anteriores.
                docPdf = new iText.Kernel.Pdf.PdfDocument(reader, writer);
                // Añadimos los datos de Metadata
                docPdf.GetDocumentInfo().SetAuthor("Orion - AnderSoft - A.Herrero");
                docPdf.GetDocumentInfo().SetCreator("Orion 1.0");

            }
            // Devolvemos el documento.
            return docPdf;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS RECLAMACIONES
        // ====================================================================================================

        //public void GenerarReclamación(Centros centro, Conductor conductor, DateTime fecha, string ruta) {

        //	// Creamos el lector del documento.
        //	string rutaPlantilla = Utils.CombinarCarpetas(App.RutaInicial, $"/Plantillas/Reclamacion.pdf");
        //	PdfReader reader = new PdfReader(rutaPlantilla);
        //	// Creamos el 'modificador' del documento.
        //	FileStream fs = new FileStream(ruta, FileMode.Create);
        //	PdfStamper stamper = new PdfStamper(reader, fs);

        //	// Extraemos los campos del documento.
        //	AcroFields campos = stamper.AcroFields;

        //	// Asignamos los campos
        //	campos.SetField("Centro", centro.ToString().ToUpper());
        //	campos.SetField("Trabajador", $"{conductor.Apellidos}, {conductor.Nombre} ({conductor.Id:000})");
        //	campos.SetField("FechaCabecera", $"{fecha:MMMM - yyyy}".ToUpper());
        //	campos.SetField("NumeroReclamacion", $"Nº Reclamación: {fecha:yyyyMM}{conductor.Id:000}/01");
        //	campos.SetField("FechaFirma", $"{DateTime.Today:dd - MM - yyyy}");

        //	// Cerramos los elementos abiertos
        //	stamper.Close();
        //	fs.Close();
        //	reader.Close();


        //}



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS PDF
        // ====================================================================================================

        /// <summary>
        /// Devuelve una imagen con el logo del sindicato.
        /// </summary>
        public static Image GetLogoSindicato() {
            Image imagen = null;
            string ruta = App.Global.Configuracion.RutaLogoSindicato;
            if (File.Exists(ruta)) {
                string x = Path.GetExtension(ruta).ToLower();
                switch (Path.GetExtension(ruta).ToLower()) {
                    case ".jpg":
                        imagen = new Image(ImageDataFactory.CreateJpeg(new Uri(ruta)));
                        break;
                    case ".png":
                        imagen = new Image(ImageDataFactory.CreatePng(new Uri(ruta)));
                        break;
                }
            }
            return imagen;
        }

        /// <summary>
        /// Devuelve una imagen con la marca de agua.
        /// </summary>
        public static Image GetMarcaDeAgua() {
            Image imagen = null;
            string ruta = App.Global.Configuracion.RutaMarcaAgua;
            if (File.Exists(ruta)) {
                string x = Path.GetExtension(ruta).ToLower();
                switch (Path.GetExtension(ruta).ToLower()) {
                    case ".jpg":
                        imagen = new Image(ImageDataFactory.CreateJpeg(new Uri(ruta)));
                        break;
                    case ".png":
                        imagen = new Image(ImageDataFactory.CreatePng(new Uri(ruta)));
                        break;
                }
            }
            return imagen;
        }



        /// <summary>
        /// Devuelve una Tabla (iText7) con un texto a la izquierda y el logo del sindicato a la derecha
        /// </summary>
        public static Table GetTablaEncabezadoSindicato(string texto, iText.Layout.Style estilo = null) {

            // Si el estilo no se ha definido, definimos un estilo estandar.
            if (estilo == null) {
                estilo = new iText.Layout.Style();
                estilo.SetFontSize(14);
                estilo.SetMargin(0);
                estilo.SetPadding(0);
                estilo.SetWidth(UnitValue.CreatePercentValue(100));
                estilo.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                estilo.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            }
            // Creamos la tabla y le aplicamos el estilo.
            Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }));
            tabla.AddStyle(estilo);
            // Creamos el párrafo de la izquierda y lo añadimos a la tabla
            Paragraph parrafo = new Paragraph(texto).SetFixedLeading(16);
            tabla.AddCell(new Cell().Add(parrafo)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            // Añadimos el logo del sindicato a la derecha.
            Image imagen = GetLogoSindicato();
            if (imagen != null) {
                imagen.SetHeight(35);
                imagen.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                tabla.AddCell(new Cell().Add(imagen)
                                        .SetTextAlignment(TextAlignment.RIGHT)
                                        .SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            }
            // Devolvemos la tabla.
            return tabla;
        }

        #endregion
        // ====================================================================================================





    }
}
