#region COPYRIGHT
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

    public class RegulacionConductor : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public RegulacionConductor() { }


        public RegulacionConductor(OleDbDataReader lector) {
            FromReader(lector);
        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public void BorrarValorPorHeader(string header) {
            switch (header) {
                case "Fecha": Fecha = new DateTime(2001, 1, 2); break;
                case "Horas": Horas = TimeSpan.Zero; break;
                case "DCs": Descansos = 0m; break;
                case "DNDs": Dnds = 0m; break;
                case "Motivo": Motivo = ""; break;
            }
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================
        private void FromReader(OleDbDataReader lector) {
            _id = lector.ToInt32("Id");
            _idconductor = lector.ToInt32("IdConductor");
            _codigo = lector.ToInt16("Codigo");
            _fecha = (lector["Fecha"] is DBNull) ? new DateTime(2001, 1, 2) : (DateTime)lector["Fecha"];
            _horas = lector.ToTimeSpan("Horas");
            _descansos = lector.ToDecimal("Descansos");
            _dnds = lector.ToDecimal("Dnds");
            _motivo = lector.ToString("Motivo");
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS SOBRECARGADOS
        // ====================================================================================================
        public override bool Equals(object obj) {
            if (!(obj is RegulacionConductor)) return false;
            RegulacionConductor r = obj as RegulacionConductor;
            if (r.Codigo != Codigo || r.Fecha.Ticks != Fecha.Ticks) return false;
            if (r.Horas != Horas) return false;
            if (r.Descansos != Descansos) return false;
            if (r.Dnds != Dnds) return false;
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }


        public override string ToString() {
            return Fecha.ToString(@"dd-MM-yyyy") + ": " + Horas.ToTexto() + " horas, " + Descansos + " descansos y " + Dnds + "(" + Motivo + ").";
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================
        public static void ParseFromReader(OleDbDataReader lector, RegulacionConductor regulacion) {
            regulacion.Id = lector.ToInt32("Id");
            regulacion.IdConductor = lector.ToInt32("IdConductor");
            regulacion.Codigo = lector.ToInt16("Codigo");
            regulacion.Fecha = (lector["Fecha"] is DBNull) ? new DateTime(2001, 1, 2) : (DateTime)lector["Fecha"];
            regulacion.Horas = lector.ToTimeSpan("Horas");
            regulacion.Descansos = lector.ToDecimal("Descansos");
            regulacion.Dnds = lector.ToDecimal("Dnds");
            regulacion.Motivo = lector.ToString("Motivo");
        }


        public static void ParseToCommand(OleDbCommand Comando, RegulacionConductor regulacion) {
            Comando.Parameters.AddWithValue("idconductor", regulacion.IdConductor);
            Comando.Parameters.AddWithValue("codigo", regulacion.Codigo);
            Comando.Parameters.AddWithValue("fecha", regulacion.Fecha.ToString("yyyy-MM-dd"));
            Comando.Parameters.AddWithValue("horas", regulacion.Horas.Ticks);
            Comando.Parameters.AddWithValue("descansos", regulacion.Descansos.ToString("0.0000"));
            Comando.Parameters.AddWithValue("dnds", regulacion.Dnds.ToString("0.0000"));
            Comando.Parameters.AddWithValue("motivo", regulacion.Motivo);
            Comando.Parameters.AddWithValue("id", regulacion.Id);
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private int _id;
        public int Id {
            get { return _id; }
            set {
                if (_id != value) {
                    _id = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private int _idconductor;
        public int IdConductor {
            get { return _idconductor; }
            set {
                if (_idconductor != value) {
                    _idconductor = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private int _codigo;
        public int Codigo {
            get { return _codigo; }
            set {
                if (_codigo != value) {
                    _codigo = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private DateTime _fecha;
        public DateTime Fecha {
            get { return _fecha; }
            set {
                if (_fecha != value) {
                    _fecha = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _horas;
        public TimeSpan Horas {
            get { return _horas; }
            set {
                if (_horas != value) {
                    _horas = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _descansos;
        public decimal Descansos {
            get { return _descansos; }
            set {
                if (_descansos != value) {
                    _descansos = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _dnds;
        public decimal Dnds {
            get => _dnds;
            set => SetValue(ref _dnds, value);
        }



        private string _motivo = "";
        public string Motivo {
            get { return _motivo; }
            set {
                if (value == "0") { Codigo = 0; Modificado = true; PropiedadCambiada(); return; }
                if (value == "1") { Codigo = 1; Modificado = true; PropiedadCambiada(); return; }
                if (value == "2") { Codigo = 2; Modificado = true; PropiedadCambiada(); return; }
                if (_motivo != value) {
                    _motivo = value;
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
            _id = lector.ToInt32("_id");
            _idconductor = lector.ToInt32("IdConductor");
            _codigo = lector.ToInt32("Codigo");
            _fecha = lector.ToDateTime("Fecha");
            _horas = lector.ToTimeSpan("Horas");
            _descansos = lector.ToDecimal("Descansos");
            _dnds = lector.ToDecimal("Dnds");
            _motivo = lector.ToString("Motivo");
            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("IdConductor", IdConductor));
                lista.Add(new SQLiteParameter("Codigo", Codigo));
                lista.Add(new SQLiteParameter("Fecha", Fecha.ToString("yyyy-MM-dd")));
                lista.Add(new SQLiteParameter("Horas", Horas.Ticks));
                lista.Add(new SQLiteParameter("Descansos", Descansos.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("Dnds", Dnds.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("Motivo", Motivo));
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
        public virtual int ForeignId {
            get => IdConductor;
            set => IdConductor = value;
        }


        [JsonIgnore]
        public virtual string ForeignIdName { get; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"Fecha ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "Regulaciones"; }


        [JsonIgnore]
        public virtual string ComandoInsertar {
            get => "INSERT OR REPLACE INTO Regulaciones (" +
                "IdConductor, " +
                "Codigo, " +
                "Fecha, " +
                "Horas, " +
                "Descansos, " +
                "Dnds, " +
                "Motivo, " +
                "_id) " +
                "VALUES (" +
                "@idConductor, " +
                "@codigo, " +
                "@fecha, " +
                "@horas, " +
                "@descansos, " +
                "@dnds, " +
                "@motivo, " +
                "@id);";
        }


        #endregion
        // ====================================================================================================




    }
}
