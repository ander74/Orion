#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Collections.Generic;
using System.Data.SQLite;

namespace Orion.Models {

    public abstract class SQLiteItem : NotifyBase {


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        /// <summary>
        /// Id del elemento que se guardará en el campo _id;
        /// </summary>
        public abstract int Id { get; set; }


        /// <summary>
        /// Devuelve los parámetros que se pasarán al comando correspondiente.
        /// </summary>
        public abstract IEnumerable<SQLiteParameter> Parametros { get; }


        /// <summary>
        /// Lista de los elementos hijo de este elemento. Null si no tiene elementos hijo.
        /// </summary>
        public abstract IEnumerable<SQLiteItem> Lista { get; set; }


        /// <summary>
        /// Id del elemento al que pertenece este elemento, en caso de ser hijo de algun elemento. Cero si no lo es.
        /// </summary>
        public abstract int ForeignId { get; set; }


        /// <summary>
        /// Nombre del campo que contiene el id foráneo para construir la clausula WHERE. Por defecto debe ser string.Empty.
        /// </summary>
        public abstract string ForeignName { get; }


        /// <summary>
        /// Nombre de la tabla en la base de datos.
        /// </summary>
        public static string TableName { get; } = "";


        /// <summary>
        /// Comando SQL para insertar un elemento.
        /// </summary>
        public abstract string ComandoInsertar { get; }


        /// <summary>
        /// Comando SQL para actualizar un elemento.
        /// </summary>
        public abstract string ComandoActualizar { get; }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Establece todas las propiedades de la base de datos en el objeto.
        /// </summary>
        public abstract void FromReader(SQLiteDataReader lector);


        #endregion
        // ====================================================================================================




    }
}
