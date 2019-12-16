#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using DataModels;
    using Models;
    using Orion.Interfaces;
    using Servicios;

    public partial class CalendariosViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private List<Calendario> _listaborrados = new List<Calendario>();

        // SERVICIOS
        private IMensajes Mensajes;
        private InformesServicio Informes;
        private IFileService FileService;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public CalendariosViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes, IFileService fileService) {
            Mensajes = servicioMensajes;
            Informes = servicioInformes;
            FileService = fileService;
            _listacalendarios = new ObservableCollection<Calendario>();
            _listacalendarios.CollectionChanged += ListaCalendarios_CollectionChanged;
            FechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // PASO DE ACCESS A SQLITE
            //if (App.Global.Reposritory.GetCount<Calendario>() == 0) {
            //    var listaSQLite = BdCalendarios.GetCalendariosSinLista();
            //    App.Global.Reposritory.GuardarCalendarios(listaSQLite);
            //    listaSQLite = null;
            //}
            //if (App.Global.Reposritory.GetCount<DiaCalendario>() == 0) {
            //    var listaSQLite2 = BdDiasCalendario.GetDiasCalendario();
            //    App.Global.Reposritory.GuardarItems(listaSQLite2);
            //    listaSQLite2 = null;
            //}

        }
        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public void CargarCalendarios() {
            if (App.Global.CadenaConexion == null) {
                _listacalendarios.Clear();
                return;
            }
            ListaCalendarios = BdCalendarios.GetCalendarios(FechaActual.Year, FechaActual.Month);
            if (ListaCalendarios.Any(x => x.HayDiasNuevos)) HayCambios = true;
            foreach (Calendario c in ListaCalendarios) {
                c.ObjetoCambiado += ObjetoCambiadoEventHandler;
            }
            CalendarioSeleccionado = null;
            PropiedadCambiada(nameof(Detalle));
        }

        public bool EsFestivo(DateTime fecha) {
            return App.Global.FestivosVM.ListaFestivos.Count(f => f.Fecha.Year == fecha.Year && f.Fecha.Month == fecha.Month && f.Fecha.Day == fecha.Day) > 0;

        }

        public void GuardarCalendarios() {
            if (AñadirConductoresDesconocidos()) {
                App.Global.ConductoresVM.GuardarConductores();
            }
            try {
                HayCambios = false;
                if (ListaCalendarios != null && ListaCalendarios.Count > 0) {
                    BdCalendarios.GuardarCalendarios(ListaCalendarios.Where(c => c.Modificado || c.Nuevo));
                }
                if (_listaborrados.Count > 0) {
                    BdCalendarios.BorrarCalendarios(_listaborrados);
                    _listaborrados.Clear();
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosViewModel.GuardarCalendarios", ex);
                HayCambios = true;
            }

        }

        public bool AñadirConductoresDesconocidos() {
            bool hayDesconocidos = false;
            foreach (Calendario calendario in ListaCalendarios) {
                if (!App.Global.ConductoresVM.ExisteConductor(calendario.IdConductor)) {
                    App.Global.ConductoresVM.CrearConductorDesconocido(calendario.IdConductor);
                    hayDesconocidos = true;
                }

            }
            return hayDesconocidos;
        }

        public void BorrarCalendariosPorConductor(int idConductor) {
            List<Calendario> lista = ListaCalendarios.Where(c => c.IdConductor == idConductor).ToList();
            foreach (Calendario c in lista) {
                _listaborrados.Add(c);
                ListaCalendarios.Remove(c);
                HayCambios = true;
            }
        }

        public void DeshacerBorrarPorConductor(int idConductor) {
            if (_listaborrados == null) return;
            List<Calendario> lista = new List<Calendario>();
            foreach (Calendario c in _listaborrados.Where(c => c.IdConductor == idConductor)) {
                lista.Add(c);
            }
            foreach (Calendario calendario in lista) {
                if (calendario.Nuevo) {
                    _listacalendarios.Add(calendario);
                } else {
                    _listacalendarios.Add(calendario);
                    calendario.Nuevo = false;
                }
                _listaborrados.Remove(calendario);
                HayCambios = true;
            }
        }

        public void GuardarTodo() {
            GuardarCalendarios();
            HayCambios = false;
        }

        public void Reiniciar() {
            FechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            CargarCalendarios();
            HayCambios = false;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaCalendarios_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (Calendario calendario in e.NewItems) {
                    calendario.Fecha = FechaActual;
                    if (calendario.ListaDias == null) {
                        calendario.ListaDias = new ObservableCollection<DiaCalendario>(
                            Enumerable.Range(1, DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month)).Select(d => new DiaCalendario() {
                                Dia = d,
                                DiaFecha = new DateTime(FechaActual.Year, FechaActual.Month, d),
                                Nuevo = true,
                            }).ToList());
                    }
                    calendario.Nuevo = true;
                    calendario.Modificado = false;
                    calendario.ObjetoCambiado += ObjetoCambiadoEventHandler;
                    HayCambios = true;
                }
            }
            if (e.OldItems != null) {
                foreach (Calendario calendario in e.OldItems) {
                    calendario.ObjetoCambiado -= ObjetoCambiadoEventHandler;
                }
            }
            PropiedadCambiada(nameof(ListaCalendarios));
        }

        private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
            HayCambios = true;
        }

        private void _pijama_DiaCambiado(object sender, Pijama.HojaPijama.DiaCambiadoEventArgs e) {
            DiaCalendarioSeleccionado = CalendarioSeleccionado?.ListaDias?[e.Dia.Dia - 1];
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================
        private ObservableCollection<Calendario> _listacalendarios;
        public ObservableCollection<Calendario> ListaCalendarios {
            get { return _listacalendarios; }
            set {
                if (_listacalendarios != value) {
                    _listacalendarios = value;
                    _listacalendarios.CollectionChanged += ListaCalendarios_CollectionChanged;
                    VistaCalendarios = new ListCollectionView(ListaCalendarios);
                    FiltroAplicado = "Ninguno";
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(ExcesoJornada));
                }
            }
        }


        private ListCollectionView _vistacalendarios;
        public ListCollectionView VistaCalendarios {
            get { return _vistacalendarios; }
            set {
                if (_vistacalendarios != value) {
                    _vistacalendarios = value;
                    PropiedadCambiada();
                }
            }
        }


        private Calendario _calendarioseleccionado;
        public Calendario CalendarioSeleccionado {
            get { return _calendarioseleccionado; }
            set {
                if (_calendarioseleccionado != value) {
                    _calendarioseleccionado = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(Detalle));
                }
            }
        }


        private DiaCalendario _diacalendarioseleccionado;
        public DiaCalendario DiaCalendarioSeleccionado {
            get { return _diacalendarioseleccionado; }
            set {
                if (_diacalendarioseleccionado != value) {
                    _diacalendarioseleccionado = value;
                    GraficoOriginal = null;
                    PropiedadCambiada();
                }
            }
        }



        private int idConductorPijama;
        public int IdConductorPijama {
            get => idConductorPijama;
            set => SetValue(ref idConductorPijama, value);
        }


        private DateTime _fechaactual;
        public DateTime FechaActual {
            get { return _fechaactual; }
            set {
                if (_fechaactual != value) {
                    _fechaactual = value;
                    if (HayCambios) GuardarCalendarios();
                    CargarCalendarios();
                    FechaCalendarios = _fechaactual;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(MesActual));
                    PropiedadCambiada(nameof(AñoActual));
                    PropiedadCambiada(nameof(ExisteDia29));
                    PropiedadCambiada(nameof(ExisteDia30));
                    PropiedadCambiada(nameof(ExisteDia31));
                }
            }
        }


        private DateTime _fechacalendarios;
        public DateTime FechaCalendarios {
            get { return _fechacalendarios; }
            set { SetValue(ref _fechacalendarios, value); }
        }

        public string MesActual {
            get {
                return FechaActual.ToString("MMMM").ToUpper();
            }
        }

        public string AñoActual {
            get {
                return FechaActual.Year.ToString();
            }
        }

        public TimeSpan ExcesoJornada {
            get {
                if (_calendarioseleccionado == null) return TimeSpan.Zero;
                return new TimeSpan(_calendarioseleccionado.ListaDias.Sum(Ld => Ld.ExcesoJornada.Ticks));
            }
        }


        private bool _haycambios;
        public bool HayCambios {
            get { return _haycambios; }
            set {
                if (_haycambios != value) {
                    _haycambios = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(Detalle));
                }
            }
        }


        private string _filtroAplicado = "Ninguno";
        public string FiltroAplicado {
            get { return _filtroAplicado; }
            set {
                if (_filtroAplicado != value) {
                    _filtroAplicado = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayFiltroAplicado));
                }
            }
        }

        public bool HayFiltroAplicado {
            get => !FiltroAplicado.Equals("Ninguno");
        }


        private Pijama.HojaPijama _pijama;
        public Pijama.HojaPijama Pijama {
            get { return _pijama; }
            set {
                if (_pijama != value) {
                    _pijama = value;
                    if (_pijama != null) _pijama.DiaCambiado += _pijama_DiaCambiado;
                    PropiedadCambiada();
                }
            }
        }

        public String Detalle {
            get {
                string texto = $"Calendarios: {ListaCalendarios.Count.ToString()}";
                if (CalendarioSeleccionado != null) {
                    texto += $" => {CalendarioSeleccionado.IdConductor}" +
                             $"  |  Exceso = {ExcesoJornada.ToTexto()}";
                }
                return texto;
            }
        }


        private AccionesLotesCalendariosVM _accioneslotesvm = new AccionesLotesCalendariosVM();
        public AccionesLotesCalendariosVM AccionesLotesVM {
            get { return _accioneslotesvm; }
            set {
                if (_accioneslotesvm != value) {
                    _accioneslotesvm = value;
                    PropiedadCambiada();
                }
            }
        }

        public bool ExisteDia29 {
            get {
                return !(DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month) < 29);
            }
        }

        public bool ExisteDia30 {
            get {
                return !(DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month) < 30);
            }
        }

        public bool ExisteDia31 {
            get {
                return !(DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month) < 31);
            }
        }


        private GraficoBase _graficooriginal;
        public GraficoBase GraficoOriginal {
            get { return _graficooriginal; }
            set { SetValue(ref _graficooriginal, value); }
        }

        #endregion
        // ====================================================================================================

    }
}
