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
                    //ListaDias[int.Parse(header) - 1].PlusLimpiezaAlt = null;
                    ListaDias[int.Parse(header) - 1].PlusPaqueteriaAlt = null;
                    break;
            }
        }

        public void Recalcular() {
            DiasOV = ListaDias?.Count(d => d.Grafico == -1) ?? 0;
            DiasJD = ListaDias?.Count(d => d.Grafico == -2) ?? 0;
            DiasFN = ListaDias?.Count(d => d.Grafico == -3) ?? 0;
            DiasE = ListaDias?.Count(d => d.Grafico == -4) ?? 0;
            DiasDS = ListaDias?.Count(d => d.Grafico == -5) ?? 0;
            DiasDC = ListaDias?.Count(d => d.Grafico == -6) ?? 0;
            DiasF6 = ListaDias?.Count(d => d.Grafico == -7) ?? 0;
            DiasDND = ListaDias?.Count(d => d.Grafico == -8) ?? 0;
            DiasPER = ListaDias?.Count(d => d.Grafico == -9) ?? 0;
            DiasEJD = ListaDias?.Count(d => d.Grafico == -10) ?? 0;
            DiasEFN = ListaDias?.Count(d => d.Grafico == -11) ?? 0;
            DiasOVJD = ListaDias?.Count(d => d.Grafico == -12) ?? 0;
            DiasOVFN = ListaDias?.Count(d => d.Grafico == -13) ?? 0;
            DiasF6DC = ListaDias?.Count(d => d.Grafico == -14) ?? 0;
            DiasFOR = ListaDias?.Count(d => d.Grafico == -15) ?? 0;
            DiasOVA = ListaDias?.Count(d => d.Grafico == -16) ?? 0;
            DiasOVAJD = ListaDias?.Count(d => d.Grafico == -17) ?? 0;
            DiasOVAFN = ListaDias?.Count(d => d.Grafico == -18) ?? 0;
            DiasLAC = ListaDias?.Count(d => d.Grafico == -19) ?? 0;
            DiasCO = ListaDias?.Count(d => d.Codigo == 1) ?? 0;
            DiasCE = ListaDias?.Count(d => d.Codigo == 2) ?? 0;
            DiasTrabajo = ListaDias?.Count(d => d.Grafico > 0) ?? 0;
            DiasTrabajoJD = ListaDias?.Count(d => d.Codigo == 3) ?? 0;
            DiasActivo = ListaDias?.Count(d => d.Grafico != 0) ?? 0;
            DiasInactivo = ListaDias?.Count(d => d.Grafico == 0) ?? 0;
            TrabajadasConvenio = new TimeSpan(ListaDias.Sum(d => d.TrabajadasConvenio.Ticks));
            Acumuladas = new TimeSpan(ListaDias.Sum(d => d.Acumuladas.Ticks));
            Nocturnas = new TimeSpan(ListaDias.Sum(d => d.Nocturnas.Ticks));
            // FindesCompletos = Se recalcula en el CalendarioViewModel, por tener los días anterior y posterior.
            // En Orion 2, los días anterior y posterior se incluirán en el propio calendario.
        
            //TODO: Consultar si los fines de semana que tocarían en vacaciones cuentan para los findes completos.
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

            if (e.PropertyName == nameof(DiaCalendario.Grafico) || e.PropertyName == nameof(DiaCalendario.TrabajadasAlt)) {
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


        private int diasOV;
        public int DiasOV {
            get => diasOV;
            set {
                if (SetValue(ref diasOV, value)) {
                    PropiedadCambiada(nameof(DiasVacacionesConvenio));
                }
            }
        }


        private int diasJD;
        public int DiasJD {
            get => diasJD;
            set {
                if (SetValue(ref diasJD, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                }
            }
        }


        private int diasFN;
        public int DiasFN {
            get => diasFN;
            set {
                if (SetValue(ref diasFN, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                    PropiedadCambiada(nameof(DescansosFindeConvenio));
                }
            }
        }


        private int diasE;
        public int DiasE {
            get => diasE;
            set {
                if (SetValue(ref diasE, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                    PropiedadCambiada(nameof(DiasEnfermoConvenio));
                }
            }
        }


        private int diasDS;
        public int DiasDS {
            get => diasDS;
            set {
                if (SetValue(ref diasDS, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                }
            }
        }


        private int diasDC;
        public int DiasDC {
            get => diasDC;
            set {
                if (SetValue(ref diasDC, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasF6;
        public int DiasF6 {
            get => diasF6;
            set {
                if (SetValue(ref diasF6, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasDND;
        public int DiasDND {
            get => diasDND;
            set {
                if (SetValue(ref diasDND, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasPER;
        public int DiasPER {
            get => diasPER;
            set {
                if (SetValue(ref diasPER, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasEJD;
        public int DiasEJD {
            get => diasEJD;
            set {
                if (SetValue(ref diasEJD, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                    PropiedadCambiada(nameof(DiasEnfermoConvenio));
                }
            }
        }


        private int diasEFN;
        public int DiasEFN {
            get => diasEFN;
            set {
                if (SetValue(ref diasEFN, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                    PropiedadCambiada(nameof(DescansosFindeConvenio));
                    PropiedadCambiada(nameof(DiasEnfermoConvenio));
                }
            }
        }


        private int diasOVJD;
        public int DiasOVJD {
            get => diasOVJD;
            set {
                if (SetValue(ref diasOVJD, value)) {
                    PropiedadCambiada(nameof(DiasVacacionesConvenio));
                }
            }
        }


        private int diasOVFN;
        public int DiasOVFN {
            get => diasOVFN;
            set {
                if (SetValue(ref diasOVFN, value)) {
                    PropiedadCambiada(nameof(DiasVacacionesConvenio));
                }
            }
        }


        private int diasF6DC;
        public int DiasF6DC {
            get => diasF6DC;
            set {
                if (SetValue(ref diasF6DC, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasFOR;
        public int DiasFOR {
            get => diasFOR;
            set {
                if (SetValue(ref diasFOR, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasOVA;
        public int DiasOVA {
            get => diasOVA;
            set {
                if (SetValue(ref diasOVA, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasOVAJD;
        public int DiasOVAJD {
            get => diasOVAJD;
            set {
                if (SetValue(ref diasOVAJD, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                }
            }
        }


        private int diasOVAFN;
        public int DiasOVAFN {
            get => diasOVAFN;
            set {
                if (SetValue(ref diasOVAFN, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                    PropiedadCambiada(nameof(DescansosFindeConvenio));
                }
            }
        }


        private int diasLAC;
        public int DiasLAC {
            get => diasLAC;
            set {
                if (SetValue(ref diasLAC, value)) {
                    PropiedadCambiada(nameof(DiasTrabajoConvenio));
                }
            }
        }


        private int diasCO;
        public int DiasCO {
            get => diasCO;
            set => SetValue(ref diasCO, value);
        }


        private int diasCE;
        public int DiasCE {
            get => diasCE;
            set => SetValue(ref diasCE, value);
        }


        private int diasTrabajo;
        public int DiasTrabajo {
            get => diasTrabajo;
            set {
                if (SetValue(ref diasTrabajo, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                }
            }
        }


        private int diasTrabajoJD;
        public int DiasTrabajoJD {
            get => diasTrabajoJD;
            set {
                if (SetValue(ref diasTrabajoJD, value)) {
                    PropiedadCambiada(nameof(DiasDescansoConvenio));
                }
            }
        }


        private int diasActivo;
        public int DiasActivo {
            get => diasActivo;
            set => SetValue(ref diasActivo, value);
        }


        private int diasInactivo;
        public int DiasInactivo {
            get => diasInactivo;
            set => SetValue(ref diasInactivo, value);
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


        private TimeSpan nocturnas;
        public TimeSpan Nocturnas {
            get => nocturnas;
            set => SetValue(ref nocturnas, value);
        }


        private decimal findesCompletos;
        public decimal FindesCompletos {
            get => findesCompletos;
            set => SetValue(ref findesCompletos, value);
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

        public int DiasTrabajoConvenio {
            get => DiasTrabajo + DiasE + DiasDC + DiasF6 + DiasDND + DiasPER + DiasF6DC + DiasFOR + DiasLAC;
        }

        public int DiasDescansoConvenio {
            get => DiasJD + DiasFN + DiasDS + DiasEJD + DiasEFN + DiasOVAJD + DiasOVAFN;
        }

        public int DescansosFindeConvenio {
            get => DiasFN + DiasEFN + DiasOVAFN;
        }

        public int DiasVacacionesConvenio {
            get => DiasOV + DiasOVJD + DiasOVFN;
        }

        public int DiasEnfermoConvenio {
            get => DiasE + DiasEJD + DiasEFN;
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
            _conductorindefinido = lector.ToBool("ConductorIndefinido");
            categoriaConductor = lector.ToString("CategoriaConductor");
            _fecha = lector.ToDateTime("Fecha");
            diasOV = lector.ToInt32("DiasOV");
            diasJD = lector.ToInt32("DiasJD");
            diasFN = lector.ToInt32("DiasFN");
            diasE = lector.ToInt32("DiasE");
            diasDS = lector.ToInt32("DiasDS");
            diasDC = lector.ToInt32("DiasDC");
            diasF6 = lector.ToInt32("DiasF6");
            diasDND = lector.ToInt32("DiasDND");
            diasPER = lector.ToInt32("DiasPER");
            diasEJD = lector.ToInt32("DiasEJD");
            diasEFN = lector.ToInt32("DiasEFN");
            diasOVJD = lector.ToInt32("DiasOVJD");
            diasOVFN = lector.ToInt32("DiasOVFN");
            diasF6DC = lector.ToInt32("DiasF6DC");
            diasFOR = lector.ToInt32("DiasFOR");
            diasOVA = lector.ToInt32("DiasOVA");
            diasOVAJD = lector.ToInt32("DiasOVAJD");
            diasOVAFN = lector.ToInt32("DiasOVAFN");
            diasLAC = lector.ToInt32("DiasLAC");
            diasCO = lector.ToInt32("DiasCO");
            diasCE = lector.ToInt32("DiasCE");
            diasTrabajo = lector.ToInt32("DiasTrabajo");
            diasTrabajoJD = lector.ToInt32("DiasTrabajoJD");
            diasActivo = lector.ToInt32("DiasActivo");
            diasInactivo = lector.ToInt32("DiasInactivo");
            trabajadasConvenio = lector.ToTimeSpan("TrabajadasConvenio");
            acumuladas = lector.ToTimeSpan("Acumuladas");
            nocturnas = lector.ToTimeSpan("Nocturnas");
            findesCompletos = lector.ToDecimal("FindesCompletos");
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
                lista.Add(new SQLiteParameter("DiasOV", DiasOV));
                lista.Add(new SQLiteParameter("DiasJD", DiasJD));
                lista.Add(new SQLiteParameter("DiasFN", DiasFN));
                lista.Add(new SQLiteParameter("DiasE", DiasE));
                lista.Add(new SQLiteParameter("DiasDS", DiasDS));
                lista.Add(new SQLiteParameter("DiasDC", DiasDC));
                lista.Add(new SQLiteParameter("DiasF6", DiasF6));
                lista.Add(new SQLiteParameter("DiasDND", DiasDND));
                lista.Add(new SQLiteParameter("DiasPER", DiasPER));
                lista.Add(new SQLiteParameter("DiasEJD", DiasEJD));
                lista.Add(new SQLiteParameter("DiasEFN", DiasEFN));
                lista.Add(new SQLiteParameter("DiasOVJD", DiasOVJD));
                lista.Add(new SQLiteParameter("DiasOVFN", DiasOVFN));
                lista.Add(new SQLiteParameter("DiasF6DC", DiasF6DC));
                lista.Add(new SQLiteParameter("DiasFOR", DiasFOR));
                lista.Add(new SQLiteParameter("DiasOVA", DiasOVA));
                lista.Add(new SQLiteParameter("DiasOVAJD", DiasOVAJD));
                lista.Add(new SQLiteParameter("DiasOVAFN", DiasOVAFN));
                lista.Add(new SQLiteParameter("DiasLAC", DiasLAC));
                lista.Add(new SQLiteParameter("DiasCO", DiasCO));
                lista.Add(new SQLiteParameter("DiasCE", DiasCE));
                lista.Add(new SQLiteParameter("DiasTrabajo", DiasTrabajo));
                lista.Add(new SQLiteParameter("DiasTrabajoJD", DiasTrabajoJD));
                lista.Add(new SQLiteParameter("DiasActivo", DiasActivo));
                lista.Add(new SQLiteParameter("DiasInactivo", DiasInactivo));
                lista.Add(new SQLiteParameter("TrabajadasConvenio", TrabajadasConvenio.Ticks));
                lista.Add(new SQLiteParameter("Acumuladas", Acumuladas.Ticks));
                lista.Add(new SQLiteParameter("Nocturnas", Nocturnas.Ticks));
                lista.Add(new SQLiteParameter("FindesCompletos", FindesCompletos.ToString("0.0000").Replace(",", ".")));
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
