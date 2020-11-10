#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.Models.PdfModels {

    /// <summary>
    /// Define una columna para tablas PDF que devolverá los estilos de las celdas de encabezado y normales para su proceso posterior.
    /// </summary>
    public class PdfColumn {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public PdfColumn(string header) {
            this.Header = header;
        }


        #endregion
        // ====================================================================================================




        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        /// <summary>
        /// Obtiene o establece el texto del encabezado de la columna.
        /// </summary>
        public string Header { get; set; }



        #endregion
        // ====================================================================================================


    }
}
