#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Collections.Generic;
using System.Linq;
using Win = System.Windows.Media;

namespace Orion.PdfExcel {

    public class PdfTableData {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        /// <summary>
        /// Crea una instancia de la clase <see cref="PdfTableData"/> con un título vacío.
        /// </summary>
        public PdfTableData() : this(string.Empty) { }


        /// <summary>
        /// Crea una instancia de la clase <see cref="PdfTableData"/> con el título definido.
        /// </summary>
        public PdfTableData(string title) {
            this.Title = title;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        /// <summary>
        /// Establece el número de columnas de la tabla, haciendo que todas sean iguales.
        /// </summary>
        public void SetUniformWidths(int numberOfColumns) {
            float columnWidth = 100f / numberOfColumns;
            ColumnWidths = Enumerable.Repeat(columnWidth, numberOfColumns);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        /// <summary>
        /// Título de la tabla, que irá a la izquierda y se repetirá en todas las hojas.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Ruta del archivo que contiene el logotipo que se insertará a la derecha y se
        /// repetirá en todas las hojas.
        /// </summary>
        public string LogoPath { get; set; }

        /// <summary>
        /// Colección de <see cref="float"/> con las anchuras de las columnas que conforman la tabla.
        /// Estas anchuras deberán ser en porcentajes, por lo que la suma de todas debería dar 100.
        /// </summary>
        public IEnumerable<float> ColumnWidths { get; set; }

        /// <summary>
        /// Colección de objetos <see cref="PdfCellInfo"/> con la información de los encabezados.
        /// </summary>
        public IEnumerable<PdfCellInfo> Headers { get; set; }

        /// <summary>
        /// Colección de filas formadas por una colección de objetos <see cref="PdfCellInfo"/> 
        /// con la información de las celdas que se irán añadiendo en la tabla.
        /// </summary>
        public IEnumerable<IEnumerable<PdfCellInfo>> Data { get; set; }

        /// <summary>
        /// Colección de objetos <see cref="PdfCellInfo"/> con la información de los totales.
        /// </summary>
        public IEnumerable<PdfCellInfo> Totals { get; set; }

        /// <summary>
        /// Tamaño de letra del título.
        /// </summary>
        public int TitleFontSize { get; set; } = 14;

        /// <summary>
        /// Color del fondo de los encabezados.
        /// </summary>
        public Win.Color HeadersBackground { get; set; } = Win.Colors.LightSkyBlue;

        /// <summary>
        /// Color del texto de los encabezados.
        /// </summary>
        public Win.Color HeadersForeground { get; set; } = Win.Colors.Black;

        /// <summary>
        /// Tamaño de letra de los encabezados.
        /// </summary>
        public int HeadersFontSize { get; set; } = 8;

        /// <summary>
        /// Color del texto de las celdas.
        /// </summary>
        public Win.Color CellsForeground { get; set; } = Win.Colors.DimGray;

        /// <summary>
        /// Tamaño de letra de las celdas.
        /// </summary>
        public int CellsFontSize { get; set; } = 7;

        /// <summary>
        /// Color del fondo de las filas impares de la tabla.
        /// </summary>
        public Win.Color RowsBackground { get; set; } = Win.Colors.White;

        /// <summary>
        /// Indica cada cuantas filas se cambia de fondo. Por defecto es 1, que significa que
        /// una fila va de un color y la otra de otro.
        /// </summary>
        public int AlternateRowsCount { get; set; } = 1;

        /// <summary>
        /// Color del fondo de las filas pares de la tabla.
        /// </summary>
        public Win.Color AlternateRowsBackground { get; set; } = Win.Colors.LightCyan;

        /// <summary>
        /// Tipo de borde que rodeará la tabla. No afecta a la separación entre celdas.
        /// </summary>
        public PdfBorder TableBorder { get; set; } = PdfBorder.SOLID_MEDIO;

        /// <summary>
        /// Tema de la tabla. Incluye los colores de encabezados y de las filas.
        /// </summary>
        public PdfTableTheme TableTheme {
            set {
                switch (value) {
                    case PdfTableTheme.AMARILLO:
                        HeadersBackground = Win.Color.FromRgb(230, 195, 0);
                        AlternateRowsBackground = Win.Color.FromRgb(255, 244, 179);
                        break;
                    case PdfTableTheme.AZUL_CLARO:
                        HeadersBackground = Win.Colors.LightSkyBlue;
                        AlternateRowsBackground = Win.Colors.LightCyan;
                        break;
                    case PdfTableTheme.AZUL_OSCURO:
                        HeadersBackground = Win.Colors.DodgerBlue;
                        AlternateRowsBackground = Win.Color.FromRgb(206, 235, 253);
                        break;
                    case PdfTableTheme.DEFAULT:
                    case PdfTableTheme.GRIS:
                        HeadersBackground = Win.Color.FromRgb(153, 153, 153);
                        AlternateRowsBackground = Win.Colors.Gainsboro;
                        break;
                    case PdfTableTheme.LIMA:
                        HeadersBackground = Win.Colors.YellowGreen;
                        AlternateRowsBackground = Win.Color.FromRgb(235, 245, 214);
                        break;
                    case PdfTableTheme.NARANJA:
                        HeadersBackground = Win.Color.FromRgb(242, 147, 64);
                        AlternateRowsBackground = Win.Color.FromRgb(255, 221, 179);
                        break;
                    case PdfTableTheme.MARRON_CLARO:
                        HeadersBackground = Win.Color.FromRgb(223, 112, 32);
                        AlternateRowsBackground = Win.Color.FromRgb(246, 212, 188);
                        break;
                    case PdfTableTheme.PURPURA:
                        HeadersBackground = Win.Colors.Orchid;
                        AlternateRowsBackground = Win.Color.FromRgb(244, 215, 244);
                        break;
                    case PdfTableTheme.VERDE:
                        HeadersBackground = Win.Colors.MediumSeaGreen;
                        AlternateRowsBackground = Win.Color.FromRgb(217, 242, 228);
                        break;
                }
            }
        }



        #endregion
        // ====================================================================================================


    }
}
