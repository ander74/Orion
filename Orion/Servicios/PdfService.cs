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
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Orion.Servicios {

    /// <summary>
    /// Proporciona métodos para generar tablas y documentos PDF genéricos.
    /// </summary>
    public class PdfService {


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Devuelve un estilo estandar para una tabla genérica.
        /// </summary>
        public Style GetEstiloTablaEstandar() {
            var estilo = new Style();
            estilo.SetFontSize(14);
            estilo.SetMargin(0);
            estilo.SetPadding(0);
            estilo.SetWidth(UnitValue.CreatePercentValue(100));
            estilo.SetBorder(Border.NO_BORDER);
            estilo.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            return estilo;
        }


        /// <summary>
        /// Extrae el logo del sindicato de las opciones y lo devuelve como un objeto Image.
        /// </summary>
        public Image GetLogoSindicato() {
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
        /// Extrae la marca de agua de las opciones y la devuelve como un objeto Image.
        /// </summary>
        public Image GetMarcaDeAgua() {
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
        /// Devuelve una tabla que será el encabezado de los documentos genéricos.
        /// Esta tabla tiene un título a la izquierda y el logo del sindicato a la derecha.
        /// </summary>
        public Table GetTablaEncabezadoSindicato(string texto) {

            //TODO: Debemos estandarizar esto. Comprobar que se puede aislar como propiedades externas de solo lectura
            //      y sustituir las referencias del método que hay en InformeServicio por este, para comprobar que funciona.

            //TODO: Para crear una tabla genérica, crear una clase que tenga propiedades como una lista de columnas (que pueden
            //      ser otra clase en la que se define el header y los estilos de celda, así como la lista de valores que puede
            //      ser un objeto también o una matriz o un dictionary, etc.

            // Creamos un estilo estandar.
            var estilo = GetEstiloTablaEstandar();

            // Creamos la tabla y le aplicamos el estilo.
            Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }));
            tabla.AddStyle(estilo);
            // Creamos el párrafo de la izquierda y lo añadimos a la tabla
            Paragraph parrafo = new Paragraph(texto).SetFixedLeading(16);
            tabla.AddCell(new Cell().Add(parrafo)
                                    .SetTextAlignment(TextAlignment.LEFT)
                                    .SetBorder(Border.NO_BORDER));
            // Añadimos el logo del sindicato a la derecha.
            Image imagen = GetLogoSindicato();
            if (imagen != null) {
                imagen.SetHeight(35);
                imagen.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                tabla.AddCell(new Cell().Add(imagen)
                                        .SetTextAlignment(TextAlignment.RIGHT)
                                        .SetBorder(Border.NO_BORDER));
            }
            // Devolvemos la tabla.
            return tabla;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        #endregion
        // ====================================================================================================



    }
}
