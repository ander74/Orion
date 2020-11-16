#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using Newtonsoft.Json;
using Orion.Interfaces;

namespace Orion.Models {

    public class ArticuloConvenio : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private int id;
        public int Id {
            get => id;
            set => SetValue(ref id, value);
        }



        private decimal numero;
        public decimal Numero {
            get => numero;
            set => SetValue(ref numero, value);
        }



        private string titulo;
        public string Titulo {
            get => titulo;
            set => SetValue(ref titulo, value);
        }



        private string texto;
        public string Texto {
            get => texto;
            set => SetValue(ref texto, value);
        }


        private int codigoFuncionRelacionada;
        public int CodigoFuncionRelacionada {
            get => codigoFuncionRelacionada;
            set => SetValue(ref codigoFuncionRelacionada, value);
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public virtual void FromReader(DbDataReader lector) {
            id = lector.ToInt32("_id");
            numero = lector.ToDecimal("Numero");
            titulo = lector.ToString("Titulo");
            texto = lector.ToString("Texto");
            codigoFuncionRelacionada = lector.ToInt32("CodigoFuncionRelacionada");
            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                //lista.Add(new SQLiteParameter("Activo", Activo ? 1 : 0));
                //lista.Add(new SQLiteParameter("Categoria", Categoria));
                lista.Add(new SQLiteParameter("Numero", Numero));
                lista.Add(new SQLiteParameter("Titulo", Titulo));
                lista.Add(new SQLiteParameter("Texto", Texto));
                lista.Add(new SQLiteParameter("CodigoFuncionRelacionada", CodigoFuncionRelacionada));
                return lista;
            }
        }


        [JsonIgnore]
        public virtual IEnumerable<ISQLiteItem> Lista { get; }


        [JsonIgnore]
        public virtual bool HasList { get => false; }


        public virtual void InicializarLista() { }


        public virtual void AddItemToList(ISQLiteItem item) { }


        [JsonIgnore]
        public virtual int ForeignId { get; set; }


        [JsonIgnore]
        public virtual string ForeignIdName { get; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"Numero ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "ArticulosConvenio"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar { get; }


        #endregion
        // ====================================================================================================


    }
}
