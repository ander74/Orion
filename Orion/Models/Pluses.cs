﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using Newtonsoft.Json;
    using Orion.Interfaces;

    public class Pluses : NotifyBase, ISQLiteItem {

        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public Pluses() { }


        public Pluses(OleDbDataReader lector) {
            ParseFromReader(lector, this);
        }

        /// <summary>
        /// Crea un nuevo objeto Pluses con los importes del objeto pasado, pero con el año indicado.
        /// </summary>
        /// <param name="año"></param>
        /// <param name="pluses"></param>
        public Pluses(int año, Pluses pluses) {
            this.Año = año;
            if (pluses != null) {
                this.ImporteDietas = pluses.ImporteDietas;
                this.ImporteSabados = pluses.ImporteSabados;
                this.ImporteFestivos = pluses.ImporteFestivos;
                this.PlusNocturnidad = pluses.PlusNocturnidad;
                this.DietaMenorDescanso = pluses.DietaMenorDescanso;
                this.PlusLimpieza = pluses.PlusLimpieza;
                this.PlusPaqueteria = pluses.PlusPaqueteria;
                this.PlusNavidad = pluses.PlusNavidad;
                this.QuebrantoMoneda = pluses.QuebrantoMoneda;
            }
            Nuevo = true;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, Pluses pluses) {
            pluses.Id = lector.ToInt32("Id");
            pluses.Año = lector.ToInt16("Año");
            pluses.ImporteDietas = lector.ToDecimal("ImporteDietas");
            pluses.ImporteSabados = lector.ToDecimal("ImporteSabados");
            pluses.ImporteFestivos = lector.ToDecimal("ImporteFestivos");
            pluses.PlusNocturnidad = lector.ToDecimal("PlusNocturnidad");
            pluses.DietaMenorDescanso = lector.ToDecimal("DietaMenorDescanso");
            pluses.PlusLimpieza = lector.ToDecimal("PlusLimpieza");
            pluses.PlusPaqueteria = lector.ToDecimal("PlusPaqueteria");
            pluses.PlusNavidad = lector.ToDecimal("PlusNavidad");
        }


        public static void ParseToCommand(OleDbCommand Comando, Pluses pluses) {
            Comando.Parameters.AddWithValue("Año", pluses.Año);
            Comando.Parameters.AddWithValue("ImporteDietas", pluses.ImporteDietas.ToString("0.0000"));
            Comando.Parameters.AddWithValue("ImporteSabados", pluses.ImporteSabados.ToString("0.0000"));
            Comando.Parameters.AddWithValue("ImporteFestivos", pluses.ImporteFestivos.ToString("0.0000"));
            Comando.Parameters.AddWithValue("PlusNocturnidad", pluses.PlusNocturnidad.ToString("0.0000"));
            Comando.Parameters.AddWithValue("DietaMenorDescanso", pluses.DietaMenorDescanso.ToString("0.0000"));
            Comando.Parameters.AddWithValue("PlusLimpieza", pluses.PlusLimpieza.ToString("0.0000"));
            Comando.Parameters.AddWithValue("PlusPaqueteria", pluses.PlusPaqueteria.ToString("0.0000"));
            Comando.Parameters.AddWithValue("PlusNavidad", pluses.PlusNavidad.ToString("0.0000"));
            Comando.Parameters.AddWithValue("Id", pluses.Id);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private int _id;
        public int Id {
            get { return _id; }
            set { SetValue(ref _id, value); }
        }



        private int _año;
        public int Año {
            get { return _año; }
            set { SetValue(ref _año, value); }
        }


        private decimal _importedietas;
        public decimal ImporteDietas {
            get { return _importedietas; }
            set { SetValue(ref _importedietas, value); }
        }



        private decimal _importesabados;
        public decimal ImporteSabados {
            get { return _importesabados; }
            set { SetValue(ref _importesabados, value); }
        }


        private decimal _importefestivos;
        public decimal ImporteFestivos {
            get { return _importefestivos; }
            set { SetValue(ref _importefestivos, value); }
        }


        private decimal _plusnocturnidad;
        public decimal PlusNocturnidad {
            get { return _plusnocturnidad; }
            set { SetValue(ref _plusnocturnidad, value); }
        }


        private decimal _dietamenordescanso;
        public decimal DietaMenorDescanso {
            get { return _dietamenordescanso; }
            set { SetValue(ref _dietamenordescanso, value); }
        }


        private decimal _pluslimpieza;
        public decimal PlusLimpieza {
            get { return _pluslimpieza; }
            set { SetValue(ref _pluslimpieza, value); }
        }


        private decimal _pluspaqueteria;
        public decimal PlusPaqueteria {
            get { return _pluspaqueteria; }
            set { SetValue(ref _pluspaqueteria, value); }
        }


        private decimal _plusnavidad;
        public decimal PlusNavidad {
            get { return _plusnavidad; }
            set { SetValue(ref _plusnavidad, value); }
        }



        private decimal quebrantoMoneda;
        public decimal QuebrantoMoneda {
            get => quebrantoMoneda;
            set => SetValue(ref quebrantoMoneda, value);
        }







        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public virtual void FromReader(DbDataReader lector) {
            _id = lector.ToInt32("_id");
            _año = lector.ToInt16("Año");
            _importedietas = lector.ToDecimal("ImporteDietas");
            _importesabados = lector.ToDecimal("ImporteSabados");
            _importefestivos = lector.ToDecimal("ImporteFestivos");
            _plusnocturnidad = lector.ToDecimal("PlusNocturnidad");
            _dietamenordescanso = lector.ToDecimal("DietaMenorDescanso");
            _pluslimpieza = lector.ToDecimal("PlusLimpieza");
            _pluspaqueteria = lector.ToDecimal("PlusPaqueteria");
            _plusnavidad = lector.ToDecimal("PlusNavidad");
            quebrantoMoneda = lector.ToDecimal("QuebrantoMoneda");
            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("Año", Año));
                lista.Add(new SQLiteParameter("ImporteDietas", ImporteDietas.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("ImporteSabados", ImporteSabados.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("ImporteFestivos", ImporteFestivos.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusNocturnidad", PlusNocturnidad.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("DietaMenorDescanso", DietaMenorDescanso.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusLimpieza", PlusLimpieza.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusPaqueteria", PlusPaqueteria.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusNavidad", PlusNavidad.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("QuebrantoMoneda", QuebrantoMoneda.ToString("0.0000").Replace(",", ".")));
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
        public virtual string OrderBy { get => $"Año ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "Pluses"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar {
            get => "INSERT OR REPLACE INTO Pluses (" +
                "Año, " +
                "ImporteDietas, " +
                "ImporteSabados, " +
                "ImporteFestivos, " +
                "PlusNocturnidad, " +
                "DietaMenorDescanso, " +
                "PlusLimpieza, " +
                "PlusPaqueteria, " +
                "PlusNavidad, " +
                "_id) " +
                "VALUES (" +
                "@año, " +
                "@importeDietas, " +
                "@importeSabados, " +
                "@importeFestivos, " +
                "@plusNocturnidad, " +
                "@dietaMenorDescanso, " +
                "@plusLimpieza, " +
                "@plusPaqueteria, " +
                "@plusNavidad, " +
                "@id);";
        }


        #endregion
        // ====================================================================================================


    }
}
