﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Orion.Models {

    /// <summary>
    /// Contiene una <b>expresión SQL</b> y los parámetros que se usan para el comando que la ejecute. <br/>
    ///  <br/>
    /// La expresión puede ser una consulta, una clausula WHERE, etc... <br/>
    ///  <br/>
    /// Los parámetros se adaptan al formato de <b>SQLite</b> de la siguiente manera: <br/>
    ///  <br/>
    ///     Si es un <see cref="DateTime"/> se pasa como texto con el formato <i>'yyyy-MM-dd'</i> <br/>
    ///     Si es un <see cref="decimal"/>, se pasa como texto con el formato <i>'0.0000'</i> <br/>
    ///     Si es un <see cref="TimeSpan"/>, se pasa el número de <i>ticks</i> como un <see cref="long"/>. <br/>
    ///     Si es un <see cref="bool"/>, se pasa 1 para <b>true</b> y 0 para <b>false</b>. <br/>
    ///     Si es <b>null</b>, se pasa el objeto <see cref="DBNull"/> <br/>
    ///      <br/>
    /// Si la expresión es una consulta completa, se puede generar un comando con los parámetros ya añadidos.
    /// </summary>
    public class SQLiteExpression {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================


        /// <summary>
        /// Instancia un objeto <see cref="SQLiteExpression"/>
        /// </summary>
        public SQLiteExpression() {
            Parametros = new List<SqliteParameter>();
        }


        /// <summary>
        /// Instancia un objeto <see cref="SQLiteExpression"/> y establece la expresión.
        /// </summary>
        public SQLiteExpression(string expresion) : this() {
            this.Expresion = expresion;
        }


        /// <summary>
        /// Instancia un objeto <see cref="SQLiteExpression"/> y establece la expresión y la lista de parámetros.
        /// </summary>
        public SQLiteExpression(string expresion, List<SqliteParameter> parametros) {
            this.Expresion = expresion;
            this.Parametros = parametros;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        /// <summary>
        /// Añade un parámetro a la lista de parámetros.
        /// </summary>
        public void AddParameter(string paramName, object paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue ?? DBNull.Value));
        }


        /// <summary>
        /// Añade un DateTime a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// El DateTime se añade con formato 'yyyy-MM-dd'.
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, DateTime paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue.ToString("yyyy-MM-dd")));
            return this;
        }


        /// <summary>
        /// Añade un long con los ticks del TimeSpan a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, TimeSpan paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue.Ticks));
            return this;
        }


        /// <summary>
        /// Añade un decimal a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// El decimal se añade con formato '0.0000'.
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, decimal paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue.ToString("0.0000").Replace(",", ".")));
            return this;
        }


        /// <summary>
        /// Añade un int a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, int paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue));
            return this;
        }


        /// <summary>
        /// Añade un long a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, long paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue));
            return this;
        }


        /// <summary>
        /// Añade un bool a la lista de parámetros y devuelve el propio objeto <see cref="SQLiteExpression"/> <br/>
        /// </summary>
        public SQLiteExpression AddParameter(string paramName, bool paramValue) {
            Parametros.Add(new SqliteParameter(paramName, paramValue ? 1 : 0));
            return this;
        }


        /// <summary>
        /// Devuelve un <see cref="SqliteCommand"/> con la expresión como consulta y los parámetros. <br />
        /// La conexión pasada se asocia al comando.
        /// </summary>
        public SqliteCommand GetCommand(SqliteConnection conexion) {
            var comando = new SqliteCommand(Expresion, conexion);
            comando.Parameters.AddRange(Parametros.ToArray());
            return comando;
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        /// <summary>
        /// Expresión que se usará en un comando SQLite. <br/>
        /// Puede ser una consulta, una clausula WHERE, etc...
        /// </summary>
        public string Expresion { get; set; }


        /// <summary>
        /// Lista de parámetros que se añadirán al comando SQLite.
        /// </summary>
        public List<SqliteParameter> Parametros { get; set; }



        #endregion
        // ====================================================================================================




    }
}
