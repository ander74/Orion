#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Orion.Interfaces {
    public interface ISQLiteItem {


        /// <summary>
        /// Id del elemento que se guardará en el campo _id;
        /// </summary>
        int Id { get; set; }


        /// <summary>
        /// Establece todas las propiedades de la base de datos en el objeto.
        /// </summary>
        void FromReader(SqliteDataReader lector);


        /// <summary>
        /// Devuelve los parámetros que se pasarán al comando correspondiente.
        /// </summary>
        IEnumerable<SqliteParameter> Parametros { get; }


        /// <summary>
        /// Lista de los elementos hijo de este elemento. Null si no tiene elementos hijo.
        /// </summary>
        IEnumerable<ISQLiteItem> Lista { get; }


        /// <summary>
        /// Devuelve true si el elemento contiene una lista de elementos hijo.
        /// </summary>
        bool HasList { get; }


        /// <summary>
        /// Inicializa la lista de elementos hijo borrando todos los que pudiera tener.
        /// </summary>
        void InicializarLista();


        /// <summary>
        /// Añade el item a la lista de elementos hijo.
        /// </summary>
        /// <param name="item"></param>
        void AddItemToList(ISQLiteItem item);


        /// <summary>
        /// Id del elemento al que pertenece este elemento, en caso de ser hijo de algun elemento. Cero si no lo es.
        /// </summary>
        int ForeignId { get; set; }


        /// <summary>
        /// Nombre del campo que contiene el id foráneo para construir la clausula WHERE. Por defecto debe ser string.Empty.
        /// </summary>
        string ForeignIdName { get; }


        /// <summary>
        /// Cláusula ORDER BY (sin el ORDER BY) para ordenar los elementos recuperados.
        /// </summary>
        string OrderBy { get; }


        /// <summary>
        /// Nombre de la tabla en la base de datos.
        /// </summary>
        string TableName { get; }


        /// <summary>
        /// Comando SQL para insertar un elemento.
        /// </summary>
        string ComandoInsertar { get; }


        /// <summary>
        /// Comando SQL para actualizar un elemento.
        /// </summary>
        //string ComandoActualizar { get; }


        /// <summary>
        /// Establece si el elemento es nuevo y se debe insertar. TODO: Cambiar por una enumeración.
        /// </summary>
        bool Nuevo { get; set; }


        /// <summary>
        /// Establece si el elemento está modificado y se debe actualizar. Cambiar por una enumeración.
        /// </summary>
        bool Modificado { get; set; }


    }
}
