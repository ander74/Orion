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
using System.Data.OleDb;
using System.Data.SQLite;
using Newtonsoft.Json;
using Orion.Interfaces;

namespace Orion.Models {

    public class GrupoGraficos : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public GrupoGraficos() { }


        public GrupoGraficos(OleDbDataReader lector) {
            FromReader(lector);
        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================
        private void FromReader(OleDbDataReader lector) {
            _id = lector.ToInt32("Id");
            _validez = lector.ToDateTime("Validez");
            _notas = lector.ToString("Notas");
        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS SOBRECARGADOS
        // ====================================================================================================
        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, GrupoGraficos grupo) {
            grupo.Id = lector.ToInt32("Id");
            grupo.Validez = lector.ToDateTime("Validez");
            grupo.Notas = lector.ToString("Notas");
        }


        public static void ParseToCommand(OleDbCommand Comando, GrupoGraficos grupo) {
            Comando.Parameters.AddWithValue("validez", grupo.Validez.ToString("yyyy-MM-dd"));
            Comando.Parameters.AddWithValue("notas", grupo.Notas);
            Comando.Parameters.AddWithValue("id", grupo.Id);
        }
        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private int _id;
        public int Id {
            get { return _id; }
            set {
                if (value != _id) {
                    _id = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }

        private DateTime _validez;
        public DateTime Validez {
            get { return _validez; }
            set {
                if (value != _validez) {
                    _validez = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private string _notas = "";
        public string Notas {
            get { return _notas; }
            set {
                if (value != _notas) {
                    _notas = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        #endregion


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public virtual void FromReader(DbDataReader lector) {
            _validez = lector.ToDateTime("Validez");
            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("Validez", Validez.ToString("yyyy-MM-dd")));
                return lista;
            }
        }


        [JsonIgnore]
        public virtual IEnumerable<ISQLiteItem> Lista { get; }


        [JsonIgnore]
        public virtual bool HasList { get => false; }


        public void InicializarLista() { }


        public void AddItemToList(ISQLiteItem item) { }


        [JsonIgnore]
        public virtual int ForeignId { get; set; }


        [JsonIgnore]
        public virtual string ForeignIdName { get => "IdGrupo"; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"Validez DESC"; }


        [JsonIgnore]
        public virtual string TableName { get => "GruposGraficos"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar {
            get => "INSERT OR REPLACE INTO GruposGraficos (" +
                "Validez, " +
                "Notas, " +
                "_id) " +
                "VALUES (" +
                "@validez, " +
                "@notas, " +
                "@id);";
        }



        #endregion
        // ====================================================================================================


    } //Final de clase.
}
