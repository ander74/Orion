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
    using System.Collections.Specialized;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using System.Linq;
    using Newtonsoft.Json;
    using Orion.Config;
    using Orion.Interfaces;

    public class Calendario : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public Calendario() { }

        public Calendario(DateTime fecha) {
            Fecha = fecha;
            ListaDias = new NotifyCollection<DiaCalendario>(
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
            matriculaConductor = lector.ToInt32("IdConductor");
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
                    MatriculaConductor = 0; break;
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

        public void Recalcular() {
            diasDC = ListaDias?.Count(d => d.Grafico == -6) ?? 0;
            diasDND = ListaDias?.Count(d => d.Grafico == -8) ?? 0;
            diasF6 = ListaDias?.Count(d => d.Grafico == -7) ?? 0;
            TrabajadasConvenio = new TimeSpan(ListaDias.Sum(d => d.TrabajadasConvenio.Ticks));
            Acumuladas = new TimeSpan(ListaDias.Sum(d => d.Acumuladas.Ticks));
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, Calendario calendario) {
            calendario.Id = lector.ToInt32("Id");
            calendario.MatriculaConductor = lector.ToInt32("IdConductor");
            calendario.Fecha = lector.ToDateTime("Fecha");
            calendario.Notas = lector.ToString("Notas");
        }


        public static void ParseToCommand(OleDbCommand Comando, Calendario calendario) {
            Comando.Parameters.AddWithValue("@Idconductor", calendario.MatriculaConductor);
            Comando.Parameters.AddWithValue("@Fecha", calendario.Fecha.ToString("yyyy-MM-dd"));
            Comando.Parameters.AddWithValue("@Notas", calendario.Notas);
            Comando.Parameters.AddWithValue("@Id", calendario.Id);
        }


        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        /// <summary>
        /// Se dispara cuando en un día del calendario se cambia la propiedad Gráfico.
        /// </summary>
        public event EventHandler<EventArgs> DiaCalendarioChanged;

        private void ListaDias_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (DiaCalendario dia in e.NewItems) {
                    dia.IdCalendario = this.Id;
                    dia.Nuevo = true;
                }
            }

            if (e.OldItems != null) {
                // No hace falta de momento.
            }

            PropiedadCambiada(nameof(ListaDias));
        }

        private void Listadias_ItemPropertyChanged(object sender, ItemChangedEventArgs<DiaCalendario> e) {

            if (e.PropertyName == nameof(DiaCalendario.Grafico)) {
                DiaCalendarioChanged?.Invoke(e.ChangedItem, new EventArgs());
                Recalcular();
            }

            Modificado = true;
            PropiedadCambiada(nameof(GrafDif));
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


        private int matriculaConductor;
        public int MatriculaConductor {
            get { return matriculaConductor; }
            set {
                if (matriculaConductor != value) {
                    matriculaConductor = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private string categoriaConductor = "C";
        public string CategoriaConductor {
            get => categoriaConductor;
            set => SetValue(ref categoriaConductor, value);
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


        private int diasDC;
        public int DiasDC {
            get => diasDC;
            set => SetValue(ref diasDC, value);
        }


        private int diasDND;
        public int DiasDND {
            get => diasDND;
            set => SetValue(ref diasDND, value);
        }


        private int diasF6;
        public int DiasF6 {
            get => diasF6;
            set => SetValue(ref diasF6, value);
        }



        private TimeSpan trabajadasConvenio;
        public TimeSpan TrabajadasConvenio {
            get => trabajadasConvenio;
            set => SetValue(ref trabajadasConvenio, value);
        }


        private TimeSpan acumuladas;
        public TimeSpan Acumuladas {
            get => acumuladas;
            set => SetValue(ref acumuladas, value);
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


        private NotifyCollection<DiaCalendario> _listadias;
        public NotifyCollection<DiaCalendario> ListaDias {
            get {
                return _listadias;
            }
            set {
                if (_listadias != value) {
                    _listadias = value;
                    _listadias.CollectionChanged += ListaDias_CollectionChanged;
                    _listadias.ItemPropertyChanged += Listadias_ItemPropertyChanged;
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
            matriculaConductor = lector.ToInt32("MatriculaConductor");
            _fecha = lector.ToDateTime("Fecha");
            diasDC = lector.ToInt32("DiasDC");
            diasDND = lector.ToInt32("DiasDND");
            diasF6 = lector.ToInt32("DiasF6");
            trabajadasConvenio = lector.ToTimeSpan("TrabajadasConvenio");
            acumuladas = lector.ToTimeSpan("Acumuladas");
            _notas = lector.ToString("Notas");
            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("MatriculaConductor", MatriculaConductor));
                lista.Add(new SQLiteParameter("Fecha", Fecha.ToString("yyyy-MM-dd")));
                lista.Add(new SQLiteParameter("DiasDC", DiasDC));
                lista.Add(new SQLiteParameter("DiasDND", DiasDND));
                lista.Add(new SQLiteParameter("DiasF6", DiasF6));
                lista.Add(new SQLiteParameter("TrabajadasConvenio", TrabajadasConvenio.Ticks));
                lista.Add(new SQLiteParameter("Acumuladas", Acumuladas.Ticks));
                lista.Add(new SQLiteParameter("Notas", Notas));
                //lista.Add(new SQLiteParameter("Id", Id));
                return lista;
            }
        }


        [JsonIgnore]
        public virtual IEnumerable<ISQLiteItem> Lista { get => ListaDias; }


        [JsonIgnore]
        public virtual bool HasList { get => true; }


        public virtual void InicializarLista() {
            ListaDias = new NotifyCollection<DiaCalendario>();
        }


        public virtual void AddItemToList(ISQLiteItem item) {
            ListaDias.Add(item as DiaCalendario);
        }


        [JsonIgnore]
        public virtual int ForeignId { get; set; }


        [JsonIgnore]
        public virtual string ForeignIdName { get => "IdCalendario"; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"MatriculaConductor ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "Calendarios"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar {
            get => "INSERT OR REPLACE INTO Calendarios (" +
                "MatriculaConductor, " +
                "Fecha, " +
                "Notas, " +
                "_id) " +
                "VALUES (" +
                "@matriculaConductor, " +
                "@fecha, " +
                "@notas, " +
                "@id);";
        }


        #endregion
        // ====================================================================================================


    }
}
