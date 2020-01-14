#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {
    using System.Collections.Generic;
    using System.Data.OleDb;
    using Microsoft.Data.Sqlite;
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







        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public void FromReader(SqliteDataReader lector) {
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
            Nuevo = false;
            Modificado = false;
        }


        public IEnumerable<SqliteParameter> Parametros {
            get {
                var lista = new List<SqliteParameter>();
                lista.Add(new SqliteParameter("Año", Año));
                lista.Add(new SqliteParameter("ImporteDietas", ImporteDietas.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("ImporteSabados", ImporteSabados.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("ImporteFestivos", ImporteFestivos.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("PlusNocturnidad", PlusNocturnidad.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("DietaMenorDescanso", DietaMenorDescanso.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("PlusLimpieza", PlusLimpieza.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("PlusPaqueteria", PlusPaqueteria.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SqliteParameter("PlusNavidad", PlusNavidad.ToString("0.0000").Replace(",", ".")));

                //lista.Add(new SqliteParameter("ImporteDietas", ImporteDietas));
                //lista.Add(new SqliteParameter("ImporteSabados", ImporteSabados));
                //lista.Add(new SqliteParameter("ImporteFestivos", ImporteFestivos));
                //lista.Add(new SqliteParameter("PlusNocturnidad", PlusNocturnidad));
                //lista.Add(new SqliteParameter("DietaMenorDescanso", DietaMenorDescanso));
                //lista.Add(new SqliteParameter("PlusLimpieza", PlusLimpieza));
                //lista.Add(new SqliteParameter("PlusPaqueteria", PlusPaqueteria));
                //lista.Add(new SqliteParameter("PlusNavidad", PlusNavidad));

                //lista.Add(new SqliteParameter("Id", Id));                
                return lista;
            }
        }


        public IEnumerable<ISQLiteItem> Lista { get; }


        public bool HasList { get => false; }


        public void InicializarLista() { }


        public void AddItemToList(ISQLiteItem item) { }


        public int ForeignId { get; set; }


        public string ForeignIdName { get; }


        public string OrderBy { get => $"Año ASC"; }


        public string TableName { get => "Pluses"; }


        public string ComandoInsertar {
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


        //public string ComandoActualizar {
        //    get => "UPDATE Pluses SET " +
        //        "Año = @año, " +
        //        "ImporteDietas = @importeDietas, " +
        //        "ImporteSabados = @importeSabados, " +
        //        "ImporteFestivos = @importeFestivos, " +
        //        "PlusNocturnidad = @plusNocturnidad, " +
        //        "DietaMenorDescanso = @dietaMenorDescanso, " +
        //        "PlusLimpieza = @plusLimpieza, " +
        //        "PlusPaqueteria = @plusPaqueteria, " +
        //        "PlusNavidad = @plusNavidad " +
        //        "WHERE _id = @id;";
        //}


        #endregion
        // ====================================================================================================


    }
}
