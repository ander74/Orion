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
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Data;
    using Models;
    using Orion.Config;
    using Orion.Interfaces;
    using Servicios;

    public partial class CalendariosViewModel : NotifyBase {



        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private List<Calendario> _listaborrados = new List<Calendario>();

        private string claveGraficosAsociados;
        private List<GraficoBase> listaGraficosAsociados = new List<GraficoBase>();
        private IEnumerable<GrupoGraficos> gruposGraficos;

        // SERVICIOS
        private IMensajes Mensajes;
        private InformesServicio Informes;
        private IFileService FileService;

        // DÍACALENDARIO ANTERIOR Y POSTERIOR AL MES ACTUAL
        private List<DiaCalendarioConductor> diasAnterior;
        private List<DiaCalendarioConductor> diasPosterior;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public CalendariosViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes, IFileService fileService) {
            Mensajes = servicioMensajes;
            Informes = servicioInformes;
            FileService = fileService;
            _listacalendarios = new NotifyCollection<Calendario>();
            listaResumen = new List<ResumenCalendarios>();
            _listacalendarios.CollectionChanged += ListaCalendarios_CollectionChanged;
            VistaCalendarios = new ListCollectionView(ListaCalendarios);
            FechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
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
            ListaCalendarios = new NotifyCollection<Calendario>(App.Global.Repository.GetCalendarios(FechaActual.Year, FechaActual.Month));

            // Cargamos los gráficos asociados, si no están cargados ya.
            var grupos = App.Global.GraficosVM.ListaGrupos;
            var maxValidez = grupos.Where(g => g.Validez <= FechaActual).Max(gg => gg.Validez);
            gruposGraficos = grupos.Where(g => g.Validez >= maxValidez && g.Validez < FechaActual.AddMonths(1));
            var clave = string.Empty;
            foreach (var grupo in gruposGraficos) clave += $"{grupo.Validez:ddMMyyyy} ";
            if (!clave.Equals(claveGraficosAsociados)) {
                claveGraficosAsociados = clave;
                listaGraficosAsociados = App.Global.Repository.GetGraficosVariasFechas(gruposGraficos).ToList();
            }
            // Cargamos el resumen anual hasta el mes anterior a este.
            if (FechaActual.Month > 1) {
                ListaResumen = App.Global.Repository.GetResumenCalendariosAnualHastaMes(FechaActual.AddMonths(-1)).ToList();
            } else {
                ListaResumen = new List<ResumenCalendarios>();
            }
            // Cargamos los días anterior y posterior de todos los calendarios.
            diasAnterior = App.Global.Repository.GetDiasCalendarioConductor(FechaActual.AddDays(-1)).ToList();
            diasPosterior = App.Global.Repository.GetDiasCalendarioConductor(FechaActual.AddMonths(1)).ToList();

            HayCambios = false; // Hay que ponerlo en false, ya que el foreach anterior modifica propiedades.
            CalendarioSeleccionado = null;
            ResumenSeleccionado = null;
            PropiedadCambiada(nameof(Detalle));
            PropiedadCambiada(nameof(GruposGraficos));
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
                    App.Global.Repository.GuardarCalendarios(ListaCalendarios.Where(c => c.Modificado || c.Nuevo));
                }
                if (_listaborrados.Count > 0) {
                    App.Global.Repository.BorrarCalendarios(_listaborrados);
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
                if (!App.Global.ConductoresVM.ExisteConductor(calendario.MatriculaConductor)) {
                    App.Global.ConductoresVM.CrearConductorDesconocido(calendario.MatriculaConductor);
                    hayDesconocidos = true;
                }
            }
            return hayDesconocidos;
        }

        public void BorrarCalendariosPorConductor(int matricula) {
            List<Calendario> lista = ListaCalendarios.Where(c => c.MatriculaConductor == matricula).ToList();
            foreach (Calendario c in lista) {
                _listaborrados.Add(c);
                ListaCalendarios.Remove(c);
                HayCambios = true;
            }
        }

        public void DeshacerBorrarPorConductor(int matricula) {
            if (_listaborrados == null) return;
            List<Calendario> lista = new List<Calendario>();
            foreach (Calendario c in _listaborrados.Where(c => c.MatriculaConductor == matricula)) {
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
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private void RegenerarDiaCalendario(DiaCalendarioBase dia) {
            if (dia.Grafico > 0) {
                var maxValidez = listaGraficosAsociados.Where(g => g.Validez <= dia.DiaFecha).Max(gg => gg.Validez);
                var grafico = listaGraficosAsociados.FirstOrDefault(g => g.Numero == dia.Grafico && g.Validez == maxValidez);
                if (grafico != null) {
                    dia.CategoriaGrafico = grafico.Categoria;
                    dia.Turno = dia.TurnoAlt.HasValue ? dia.TurnoAlt.Value : grafico.Turno;
                    dia.Inicio = dia.InicioAlt.HasValue ? dia.Inicio.Value : grafico.Inicio;
                    dia.Final = dia.FinalAlt.HasValue ? dia.FinalAlt.Value : grafico.Final;
                    dia.InicioPartido = dia.InicioPartidoAlt.HasValue ? dia.InicioPartidoAlt.Value : grafico.InicioPartido;
                    dia.FinalPartido = dia.FinalPartidoAlt.HasValue ? dia.FinalPartidoAlt.Value : grafico.FinalPartido;
                    dia.TiempoPartido = grafico.TiempoPartido;
                    dia.TiempoTotal = grafico.TiempoTotal;
                    dia.Valoracion = grafico.Valoracion;
                    dia.Trabajadas = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : grafico.Trabajadas;
                    dia.TrabajadasConvenio = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : grafico.TrabajadasConvenio;
                    dia.TiempoVacio = grafico.TiempoVacio;
                    dia.Acumuladas = dia.AcumuladasAlt.HasValue ? dia.AcumuladasAlt.Value : grafico.Acumuladas;
                    dia.Nocturnas = dia.NocturnasAlt.HasValue ? dia.NocturnasAlt.Value : grafico.Nocturnas;
                    dia.Desayuno = dia.DesayunoAlt.HasValue ? dia.DesayunoAlt.Value : grafico.Desayuno;
                    dia.Comida = dia.ComidaAlt.HasValue ? dia.ComidaAlt.Value : grafico.Comida;
                    dia.Cena = dia.CenaAlt.HasValue ? dia.CenaAlt.Value : grafico.Cena;
                    dia.PlusCena = dia.PlusCenaAlt.HasValue ? dia.PlusCenaAlt.Value : grafico.PlusCena;
                    //dia.PlusLimpieza = dia.PlusLimpiezaAlt.HasValue ? dia.PlusLimpiezaAlt.Value : grafico.PlusLimpieza;
                    dia.PlusPaqueteria = dia.PlusPaqueteriaAlt.HasValue ? dia.PlusPaqueteriaAlt.Value : grafico.PlusPaqueteria;
                } else {
                    //TODO: Evaluar poner inicio y final...
                    dia.Trabajadas = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : App.Global.Convenio.JornadaMedia;
                    dia.TrabajadasConvenio = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : App.Global.Convenio.JornadaMedia;
                    dia.Desayuno = dia.DesayunoAlt.HasValue ? dia.DesayunoAlt.Value : 0m;
                    dia.Comida = dia.ComidaAlt.HasValue ? dia.ComidaAlt.Value : 0m;
                    dia.Cena = dia.CenaAlt.HasValue ? dia.CenaAlt.Value : 0m;
                    dia.PlusCena = dia.PlusCenaAlt.HasValue ? dia.PlusCenaAlt.Value : 0m;
                    //dia.PlusLimpieza = dia.PlusLimpiezaAlt.HasValue ? dia.PlusLimpiezaAlt.Value :false;
                    dia.PlusPaqueteria = dia.PlusPaqueteriaAlt.HasValue ? dia.PlusPaqueteriaAlt.Value : false;
                }
            }

        }


        private void RecalcularCalendario(Calendario calendario) {
            calendario.Recalcular();
            decimal resultado =
                    calendario.ListaDias?.Select((dia, index) => new {
                        sab = dia,
                        dom = index + 1 < calendario.ListaDias.Count ? calendario.ListaDias[index + 1] : new DiaCalendario()
                    }).Count(finde => finde.sab.DiaFecha.DayOfWeek == DayOfWeek.Saturday && finde.sab.EsDiaDeDescanso &&
                                      finde.dom.DiaFecha.DayOfWeek == DayOfWeek.Sunday && finde.dom.EsDiaDeDescanso) ?? 0;
            // Si el primer día es domingo de descanso y el día anterior es de descanso, sumamos medio fin de semana
            if (diasAnterior != null) {
                var diaAnterior = diasAnterior.FirstOrDefault(d => d.MatriculaConductor == calendario.MatriculaConductor);
                if (diaAnterior != null && diaAnterior.DiaFecha.DayOfWeek == DayOfWeek.Saturday &&
                    diaAnterior.EsDiaDeDescanso &&
                    calendario.ListaDias.First().EsDiaDeDescanso) resultado += 0.5m;
            }
            // Si el día posterior es domingo y descanso, y el ultimo día del mes es descanso, sumamos medio finde completo.
            if (diasPosterior != null) {
                var diaPosterior = diasPosterior.FirstOrDefault(d => d.MatriculaConductor == calendario.MatriculaConductor);
                if (diaPosterior != null && diaPosterior.DiaFecha.DayOfWeek == DayOfWeek.Sunday &&
                    diaPosterior.EsDiaDeDescanso &&
                    calendario.ListaDias.Last().EsDiaDeDescanso) resultado += 0.5m;
                // Establecemos los findes completos.
                calendario.FindesCompletos = resultado;
            }
        }


        private void RegenerarDiaCalendarioAislado(DiaCalendarioBase dia) {
            if (dia.Grafico > 0) {
                var maxValidez = App.Global.GraficosVM.ListaGrupos.Where(g => g.Validez <= dia.DiaFecha).Max(gg => gg.Validez);
                var grafico = App.Global.Repository.GetGrafico(dia.Grafico, maxValidez);
                if (grafico != null) {
                    dia.CategoriaGrafico = grafico.Categoria;
                    dia.Turno = dia.TurnoAlt.HasValue ? dia.TurnoAlt.Value : grafico.Turno;
                    dia.Inicio = dia.InicioAlt.HasValue ? dia.Inicio.Value : grafico.Inicio;
                    dia.Final = dia.FinalAlt.HasValue ? dia.FinalAlt.Value : grafico.Final;
                    dia.InicioPartido = dia.InicioPartidoAlt.HasValue ? dia.InicioPartidoAlt.Value : grafico.InicioPartido;
                    dia.FinalPartido = dia.FinalPartidoAlt.HasValue ? dia.FinalPartidoAlt.Value : grafico.FinalPartido;
                    dia.TiempoPartido = grafico.TiempoPartido;
                    dia.TiempoTotal = grafico.TiempoTotal;
                    dia.Valoracion = grafico.Valoracion;
                    dia.Trabajadas = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : grafico.Trabajadas;
                    dia.TrabajadasConvenio = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : grafico.TrabajadasConvenio;
                    dia.TiempoVacio = grafico.TiempoVacio;
                    dia.Acumuladas = dia.AcumuladasAlt.HasValue ? dia.AcumuladasAlt.Value : grafico.Acumuladas;
                    dia.Nocturnas = dia.NocturnasAlt.HasValue ? dia.NocturnasAlt.Value : grafico.Nocturnas;
                    dia.Desayuno = dia.DesayunoAlt.HasValue ? dia.DesayunoAlt.Value : grafico.Desayuno;
                    dia.Comida = dia.ComidaAlt.HasValue ? dia.ComidaAlt.Value : grafico.Comida;
                    dia.Cena = dia.CenaAlt.HasValue ? dia.CenaAlt.Value : grafico.Cena;
                    dia.PlusCena = dia.PlusCenaAlt.HasValue ? dia.PlusCenaAlt.Value : grafico.PlusCena;
                    //dia.PlusLimpieza = dia.PlusLimpiezaAlt.HasValue ? dia.PlusLimpiezaAlt.Value : grafico.PlusLimpieza;
                    dia.PlusPaqueteria = dia.PlusPaqueteriaAlt.HasValue ? dia.PlusPaqueteriaAlt.Value : grafico.PlusPaqueteria;
                } else {
                    //TODO: Evaluar poner inicio y final...
                    dia.Trabajadas = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : App.Global.Convenio.JornadaMedia;
                    dia.TrabajadasConvenio = dia.TrabajadasAlt.HasValue ? dia.TrabajadasAlt.Value : App.Global.Convenio.JornadaMedia;
                    dia.Desayuno = dia.DesayunoAlt.HasValue ? dia.DesayunoAlt.Value : 0m;
                    dia.Comida = dia.ComidaAlt.HasValue ? dia.ComidaAlt.Value : 0m;
                    dia.Cena = dia.CenaAlt.HasValue ? dia.CenaAlt.Value : 0m;
                    dia.PlusCena = dia.PlusCenaAlt.HasValue ? dia.PlusCenaAlt.Value : 0m;
                    //dia.PlusLimpieza = dia.PlusLimpiezaAlt.HasValue ? dia.PlusLimpiezaAlt.Value :false;
                    dia.PlusPaqueteria = dia.PlusPaqueteriaAlt.HasValue ? dia.PlusPaqueteriaAlt.Value : false;
                }
            }

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
                        calendario.ListaDias = new NotifyCollection<DiaCalendario>(
                            Enumerable.Range(1, DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month)).Select(d => new DiaCalendario() {
                                Dia = d,
                                DiaFecha = new DateTime(FechaActual.Year, FechaActual.Month, d),
                                Nuevo = true,
                            }).ToList());
                    }
                    calendario.DiaCalendarioChanged += Calendario_DiaCalendarioChanged;
                    calendario.Nuevo = true;
                    calendario.Modificado = false;
                    HayCambios = true;
                }
            }
            if (e.OldItems != null) {
                foreach (Calendario c in e.OldItems) {
                    c.DiaCalendarioChanged -= Calendario_DiaCalendarioChanged;
                }
            }
            PropiedadCambiada(nameof(ListaCalendarios));
        }

        private void Calendario_DiaCalendarioChanged(object sender, EventArgs e) {
            if (sender is DiaCalendarioBase dia) RegenerarDiaCalendario(dia);
        }

        private void ListaCalendarios_ItemPropertyChanged(object sender, ItemChangedEventArgs<Calendario> e) {
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

        private NotifyCollection<Calendario> _listacalendarios;
        public NotifyCollection<Calendario> ListaCalendarios {
            get { return _listacalendarios; }
            set {
                if (_listacalendarios != value) {
                    _listacalendarios = value;
                    _listacalendarios.CollectionChanged += ListaCalendarios_CollectionChanged;
                    _listacalendarios.ItemPropertyChanged += ListaCalendarios_ItemPropertyChanged;
                    VistaCalendarios = new ListCollectionView(ListaCalendarios);
                    foreach (var c in _listacalendarios) c.DiaCalendarioChanged += Calendario_DiaCalendarioChanged;
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


        private List<ResumenCalendarios> listaResumen;
        public List<ResumenCalendarios> ListaResumen {
            get => listaResumen;
            set => SetValue(ref listaResumen, value);
        }


        private Calendario _calendarioseleccionado;
        public Calendario CalendarioSeleccionado {
            get { return _calendarioseleccionado; }
            set {
                if (_calendarioseleccionado != value) {
                    _calendarioseleccionado = value;
                    ResumenSeleccionado = ListaResumen?.FirstOrDefault(r => r.MatriculaConductor == CalendarioSeleccionado?.MatriculaConductor);
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HorasTrabajadas));
                    PropiedadCambiada(nameof(JornadaAnual));
                    PropiedadCambiada(nameof(DiasTrabajo));
                    PropiedadCambiada(nameof(DiasDescanso));
                    PropiedadCambiada(nameof(DiasDescansosSueltos));
                    PropiedadCambiada(nameof(FindesCompletos));
                    PropiedadCambiada(nameof(DiasVacaciones));
                    PropiedadCambiada(nameof(DiasEnfermo));
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



        private ResumenCalendarios resumenSeleccionado;
        public ResumenCalendarios ResumenSeleccionado {
            get => resumenSeleccionado;
            set => SetValue(ref resumenSeleccionado, value);
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


        public string GruposGraficos {
            get {
                if (gruposGraficos == null && !gruposGraficos.Any()) return "Ninguno";
                var texto = string.Empty;
                foreach (var grupo in gruposGraficos.OrderBy(g => g.Validez)) {
                    texto += $"{grupo.Validez:dd-MM-yy} | ";
                }
                return texto.Substring(0, texto.Length - 3);
            }
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
                int condFijos = ListaCalendarios.Count(c => c.CategoriaConductor == "C" && c.ConductorIndefinido);
                int condEventuales = ListaCalendarios.Count(c => c.CategoriaConductor == "C" && !c.ConductorIndefinido);
                int taquFijos = ListaCalendarios.Count(c => c.CategoriaConductor == "T" && c.ConductorIndefinido);
                int taquEventuales = ListaCalendarios.Count(c => c.CategoriaConductor == "T" && !c.ConductorIndefinido);
                texto += $"  |  Conducción: {condFijos + condEventuales} => Fijos: {condFijos} - Eventuales: {condEventuales}  /  " +
                    $"Taquilla: {taquFijos + taquEventuales} => Fijos: {taquFijos} - Eventuales: {taquEventuales}";
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


        // ====================================================================================================
        #region PROPIEDADES PARA INDICADORES CON SUS FORMATEADORES
        // ====================================================================================================

        public decimal HorasTrabajadas {
            get => ((ResumenSeleccionado?.TrabajadasConvenio ?? TimeSpan.Zero) + (CalendarioSeleccionado?.TrabajadasConvenio ?? TimeSpan.Zero)).ToDecimal();
        }


        public decimal JornadaAnual {
            get => ((ResumenSeleccionado?.DiasTrabajoConvenio ?? 0) + (CalendarioSeleccionado?.DiasTrabajoConvenio ?? 0)) * Math.Round(App.Global.Convenio.JornadaMedia.ToDecimal(), 4);
        }

        public Func<double, string> FormatoJornadaAnual {
            get => valor => {
                decimal total = App.Global.Convenio.HorasAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:0.00}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        public int DiasTrabajo {
            get => (ResumenSeleccionado?.DiasTrabajoConvenio ?? 0) + (CalendarioSeleccionado?.DiasTrabajoConvenio ?? 0);
        }
        public Func<double, string> FormatoDiasTrabajo {
            get => valor => {
                decimal total = App.Global.Convenio.TrabajoAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        public int DiasDescanso {
            get => (ResumenSeleccionado?.DiasDescansoConvenio ?? 0) + (CalendarioSeleccionado?.DiasDescansoConvenio ?? 0);
        }
        public Func<double, string> FormatoDiasDescanso {
            get => valor => {
                decimal total = App.Global.Convenio.DescansosAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        public int DiasDescansosSueltos {
            get => (ResumenSeleccionado?.DiasDS ?? 0) + (CalendarioSeleccionado?.DiasDS ?? 0);
        }
        public Func<double, string> FormatoDiasDescansosSueltos {
            get => valor => {
                decimal total = App.Global.Convenio.DescansosSueltosAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        //TODO: Añadir esto al calendario y al resumen.
        public decimal FindesCompletos {
            get => (ResumenSeleccionado?.FindesCompletos ?? 0) + (CalendarioSeleccionado?.FindesCompletos ?? 0m);
        }
        public Func<double, string> FormatoFindesCompletos {
            get => valor => {
                decimal total = App.Global.Convenio.FindesCompletosAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:0.0}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        public int DiasVacaciones {
            get => (ResumenSeleccionado?.DiasVacacionesConvenio ?? 0) + (CalendarioSeleccionado?.DiasVacacionesConvenio ?? 0);
        }
        public Func<double, string> FormatoDiasVacaciones {
            get => valor => {
                decimal total = App.Global.Convenio.VacacionesAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 4) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };
        }


        // No lleva Formatador.
        public int DiasEnfermo {
            get => (ResumenSeleccionado?.DiasEnfermoConvenio ?? 0) + (CalendarioSeleccionado?.DiasEnfermoConvenio ?? 0);
        }













        #endregion
        // ====================================================================================================




    }
}
