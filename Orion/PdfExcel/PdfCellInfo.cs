#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Win = System.Windows.Media;


namespace Orion.PdfExcel {

    /// <summary>
    /// Proporciona los ajustes de una Celda Pdf de una manera más rápida.
    /// </summary>
    public class PdfCellInfo {

        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        /// <summary>
        /// Crea una instancia de la clase <see cref="PdfCellInfo"/>.
        /// </summary>
        public PdfCellInfo() : this(string.Empty) { }

        /// <summary>
        /// Crea una instancia de la clase <see cref="PdfCellInfo"/> asignando el texto de la celda.
        /// </summary>
        public PdfCellInfo(string text) { this.Value = text; }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        /// <summary>
        /// Obtiene o establece el valor que se insertará en la celda.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Proporciona el formato personalizado de excel a la celda.
        /// </summary>
        public string ExcelNumberFormat { get; set; }

        /// <summary>
        /// Obtiene o establece el número de filas y columnas que ocupa la celda.
        /// </summary>
        public (int rows, int cols) MergedCells { get; set; } = (1, 1);

        /// <summary>
        /// Determina si esta celda está unida a la anterior, tanto por la izquierda como por arriba,
        /// saltándosela a la hora de imprimirla.
        /// </summary>
        public bool IsMerged { get; set; }

        /// <summary>
        /// Obtiene o establece el color del texto de la celda.
        /// </summary>
        public Win.Color? TextFontColor { get; set; } = null;

        /// <summary>
        /// Obtiene o establece el tamaño del texto de la celda.
        /// </summary>
        public double? TextFontSize { get; set; } = null;

        /// <summary>
        /// Obtiene o establece si el texto de la celda está en negrita.
        /// </summary>
        public bool? IsBold { get; set; } = null;

        /// <summary>
        /// Obtiene o establece si el texto de la celda está en cursiva.
        /// </summary>
        public bool? IsItalic { get; set; } = null;

        /// <summary>
        /// Obtiene o establece la alineación del texto de la celda.
        /// </summary>
        public System.Windows.TextAlignment? TextAlign { get; set; } = null;

        /// <summary>
        /// Obtiene o establece la alineación vertical de la celda.
        /// </summary>
        public System.Windows.VerticalAlignment? VAlign { get; set; } = null;

        /// <summary>
        /// Obtiene o establece loa grosores de los bordes de la celda.
        /// Si un valor es -1, no se modifica del estilo por defecto.
        /// </summary>
        public (PdfBorder left, PdfBorder top, PdfBorder right, PdfBorder bottom) Borders { get; set; } = (PdfBorder.DEFAULT, PdfBorder.DEFAULT, PdfBorder.DEFAULT, PdfBorder.DEFAULT);


        #endregion
        // ====================================================================================================

    }
}
