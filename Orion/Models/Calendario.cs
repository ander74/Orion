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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using System.Linq;
    using Orion.Interfaces;

    public class Calendario : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public Calendario() {
            // Creamos la lista de días.
            //ListaDias = new ObservableCollection<DiaCalendario>();
        }

        public Calendario(DateTime fecha) {
            Fecha = fecha;
            ListaDias = new ObservableCollection<DiaCalendario>(
                Enumerable.Range(1, DateTime.DaysInMonth(fecha.Year, fecha.Month)).Select(d => new DiaCalendario() {
                    Dia = d,
                    DiaFecha = new DateTime(fecha.Year, fecha.Month, d),
                    Nuevo = true,
                }).ToList());
        }

        public Calendario(OleDbDataReader lector) {
            FromReader(lector);
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================
        private void FromReader(OleDbDataReader lector) {
            _id = lector.ToInt32("Id");
            _idconductor = lector.ToInt32("IdConductor");
            _fecha = lector.ToDateTime("Fecha");
            _notas = lector.ToString("Notas");
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public void BorrarValorPorHeader(string header) {
            switch (header) {
                case "Conductor":
                    IdConductor = 0; break;
                case "01":
                case "02":
                case "03":
                case "04":
                case "05":
                case "06":
                case "07":
                case "08":
                case "09":
                case "10":
                case "11":
                case "12":
                case "13":
                case "14":
                case "15":
                case "16":
                case "17":
                case "18":
                case "19":
                case "20":
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "30":
                case "31":
                    ListaDias[int.Parse(header) - 1].Grafico = 0;
                    ListaDias[int.Parse(header) - 1].TurnoAlt = null;
                    ListaDias[int.Parse(header) - 1].InicioAlt = null;
                    ListaDias[int.Parse(header) - 1].FinalAlt = null;
                    ListaDias[int.Parse(header) - 1].InicioPartidoAlt = null;
                    ListaDias[int.Parse(header) - 1].FinalPartidoAlt = null;
                    ListaDias[int.Parse(header) - 1].TrabajadasAlt = null;
                    ListaDias[int.Parse(header) - 1].AcumuladasAlt = null;
                    ListaDias[int.Parse(header) - 1].NocturnasAlt = null;
                    ListaDias[int.Parse(header) - 1].DesayunoAlt = null;
                    ListaDias[int.Parse(header) - 1].ComidaAlt = null;
                    ListaDias[int.Parse(header) - 1].CenaAlt = null;
                    ListaDias[int.Parse(header) - 1].PlusCenaAlt = null;
                    ListaDias[int.Parse(header) - 1].PlusLimpiezaAlt = null;
                    ListaDias[int.Parse(header) - 1].PlusPaqueteriaAlt = null;
                    break;
            }
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, Calendario calendario) {
            calendario.Id = lector.ToInt32("Id");
            calendario.IdConductor = lector.ToInt32("IdConductor");
            calendario.Fecha = lector.ToDateTime("Fecha");
            calendario.Notas = lector.ToString("Notas");
        }


        public static void ParseToCommand(OleDbCommand Comando, Calendario calendario) {
            Comando.Parameters.AddWithValue("@Idconductor", calendario.IdConductor);
            Comando.Parameters.AddWithValue("@Fecha", calendario.Fecha.ToString("yyyy-MM-dd"));
            Comando.Parameters.AddWithValue("@Notas", calendario.Notas);
            Comando.Parameters.AddWithValue("@Id", calendario.Id);
        }


        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaDias_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (DiaCalendario dia in e.NewItems) {
                    dia.IdCalendario = this.Id;
                    dia.Nuevo = true;
                    dia.ObjetoCambiado += ObjetoCambiadoEventHandler;
                }
            }

            if (e.OldItems != null) {
                foreach (DiaCalendario dia in e.OldItems) {
                    dia.ObjetoCambiado -= ObjetoCambiadoEventHandler;
                }
            }

            PropiedadCambiada(nameof(ListaDias));
        }

        private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
            PropiedadCambiada(nameof(GrafDif));
            Modificado = true;
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        public int GrafDif {
            get => ListaDias.Where(d => d.Grafico > 0).GroupBy(d => d.Grafico).Count();
        }


        private int _id;
        public int Id {
            get { return _id; }
            set {
                _id = value;
                Modificado = true;
                PropiedadCambiada();
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


        private DateTime _fecha;
        public DateTime Fecha {
            get { return _fecha; }
            set {
                if (_fecha != value) {
                    _fecha = value;
                    //foreach (DiaCalendario dia in ListaDias) {
                    //	dia.DiaFecha = new DateTime(_fecha.Year, _fecha.Month, dia.DiaFecha.Day);
                    //}
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }



        private string _notas = "";
        public string Notas {
            get { return _notas; }
            set {
                if (_notas != value) {
                    _notas = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private bool _conductorindefinido;
        public bool ConductorIndefinido {
            get { return _conductorindefinido; }
            set {
                if (_conductorindefinido != value) {
                    _conductorindefinido = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _informe = "";
        public string Informe {
            get { return _informe == "" ? null : _informe; }
            set {
                if (_informe != value) {
                    _informe = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayInforme));
                }
            }
        }


        public bool HayInforme {
            get {
                return (Informe != null && Informe.Trim().Length > 0);
            }
        }


        public bool HayDiasNuevos { get; set; }


        private ObservableCollection<DiaCalendario> _listadias;
        public ObservableCollection<DiaCalendario> ListaDias {
            get {
                //TODO: Descomentar la siguiente línea
                //if (_listadias == null) ListaDias = new ObservableCollection<DiaCalendario>();
                return _listadias;
            }
            set {
                if (_listadias != value) {
                    _listadias = value;
                    //// Creamos una nueva lista.
                    //_listadias = new ObservableCollection<DiaCalendario>();
                    //// Activamos el evento CollectionChanged
                    //_listadias.CollectionChanged += ListaDias_CollectionChanged;
                    //// Definimos los días en el mes.
                    //int diasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                    //// Si los días de la lista pasada son menos que los días del mes, se activa HayDiasNuevos.
                    //if (diasMes > value.Count) HayDiasNuevos = true;
                    //// Llenamos la lista con días vacíos (el número que le corresponde al mes).
                    //for (int m = 1; m <= diasMes; m++) {
                    //	DiaCalendario d = new DiaCalendario {
                    //		IdCalendario = this.Id,
                    //		Dia = m,
                    //		DiaFecha = new DateTime(Fecha.Year, Fecha.Month, m),
                    //		Nuevo = true,
                    //	};
                    //	d.Modificado = false;
                    //	_listadias.Add(d);
                    //}
                    //// Sustituimos los días que existen por los días vacíos en la lista.
                    //foreach (DiaCalendario dia in value) {
                    //	_listadias[dia.Dia - 1] = dia;
                    //	dia.Nuevo = false;
                    //}
                    _listadias.CollectionChanged += ListaDias_CollectionChanged;
                    _listadias.ToList().ForEach(dc => dc.ObjetoCambiado += ObjetoCambiadoEventHandler);
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        #endregion


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public void FromReader(SQLiteDataReader lector) {
            _id = lector.ToInt32("_id");
            _idconductor = lector.ToInt32("IdConductor");
            _fecha = lector.ToDateTime("Fecha");
            _notas = lector.ToString("Notas");
            Nuevo = false;
            Modificado = false;
        }


        public IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("@idConductor", IdConductor));
                lista.Add(new SQLiteParameter("@fecha", Fecha.ToString("yyyy-MM-dd")));
                lista.Add(new SQLiteParameter("@notas", Notas));
                lista.Add(new SQLiteParameter("@id", Id));
                return lista;
            }
        }


        public IEnumerable<ISQLiteItem> Lista { get => ListaDias; }


        public bool HasList { get => true; }


        public void InicializarLista() {
            ListaDias = new ObservableCollection<DiaCalendario>();
        }


        public void AddItemToList(ISQLiteItem item) {
            ListaDias.Add(item as DiaCalendario);
        }


        public int ForeignId { get; set; }


        public string ForeignIdName { get => "IdConductor"; }


        public string OrderBy { get => $"IdConductor ASC"; }


        public string TableName { get => "Calendarios"; }


        public string ComandoInsertar {
            get => "INSERT OR REPLACE INTO Calendarios (" +
                "IdConductor, " +
                "Fecha, " +
                "Notas, " +
                "_id) " +
                "VALUES (" +
                "@idConductor, " +
                "@fecha, " +
                "@notas, " +
                "@id);";
        }


        //public string ComandoActualizar {
        //    get => "UPDATE Calendarios SET " +
        //        "IdConductor = @idConductor, " +
        //        "Fecha = @fecha, " +
        //        "Notas = @notas " +
        //        "WHERE _id = @id;";
        //}


        #endregion
        // ====================================================================================================


    }
}
