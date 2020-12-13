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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Input;
    using Orion.Models;
    using Orion.MVVM;
    using Orion.Servicios;

    public class EstadisticasTurnosViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private readonly IMensajes mensajes;


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public EstadisticasTurnosViewModel(IMensajes mensajes) {

            ListaEstadisticas = new List<EstadisticaPorTurnos>();
            Fecha = DateTime.MinValue;
            this.mensajes = mensajes;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public async Task<List<EstadisticaPorTurnos>> CargarEstadisticasAsync(IEnumerable<Calendario> listaCalendarios) {

            var lista = new List<EstadisticaPorTurnos>();
            await Task.Run(() => {
                EstadisticaPorTurnos estadisticaTotal = new EstadisticaPorTurnos();
                estadisticaTotal.Conductor = new Conductor { Matricula = 0, Apellidos = "TOTAL" };
                foreach (Calendario calendario in listaCalendarios) {
                    Pijama.HojaPijama pijama = new Pijama.HojaPijama(calendario, mensajes);
                    EstadisticaPorTurnos estadistica = new EstadisticaPorTurnos();

                    // CONDUCTOR
                    estadistica.Conductor = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);

                    // EVENTUAL PARCIAL
                    var evnt = !estadistica.Conductor.Indefinido;
                    var parc = pijama.ListaDias.Any(d => d.Grafico == 0);

                    if (!estadistica.Conductor.Indefinido && pijama.ListaDias.Any(d => d.Grafico == 0)) estadistica.EventualParcial = true;

                    // TURNO 1
                    estadisticaTotal.Dias[1] += estadistica.Dias[1] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1));
                    var listaDiasT1 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1);
                    estadisticaTotal.Trabajadas[1] += estadistica.Trabajadas[1] = new TimeSpan(listaDiasT1.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[1] += estadistica.Acumuladas[1] = new TimeSpan(listaDiasT1.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[1] += estadistica.Nocturnas[1] = new TimeSpan(listaDiasT1.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[1] += estadistica.ImporteDietas[1] = listaDiasT1.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[1] += estadistica.DiasFestivos[1] = listaDiasT1.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 2
                    estadisticaTotal.Dias[2] += estadistica.Dias[2] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2));
                    var listaDiasT2 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2);
                    estadisticaTotal.Trabajadas[2] += estadistica.Trabajadas[2] = new TimeSpan(listaDiasT2.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[2] += estadistica.Acumuladas[2] = new TimeSpan(listaDiasT2.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[2] += estadistica.Nocturnas[2] = new TimeSpan(listaDiasT2.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[2] += estadistica.ImporteDietas[2] = listaDiasT2.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[2] += estadistica.DiasFestivos[2] = listaDiasT2.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 3
                    estadisticaTotal.Dias[3] += estadistica.Dias[3] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3));
                    var listaDiasT3 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3);
                    estadisticaTotal.Trabajadas[3] += estadistica.Trabajadas[3] = new TimeSpan(listaDiasT3.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[3] += estadistica.Acumuladas[3] = new TimeSpan(listaDiasT3.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[3] += estadistica.Nocturnas[3] = new TimeSpan(listaDiasT3.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[3] += estadistica.ImporteDietas[3] = listaDiasT3.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[3] += estadistica.DiasFestivos[3] = listaDiasT3.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 4
                    estadisticaTotal.Dias[4] += estadistica.Dias[4] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4));
                    var listaDiasT4 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4);
                    estadisticaTotal.Trabajadas[4] += estadistica.Trabajadas[4] = new TimeSpan(listaDiasT4.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[4] += estadistica.Acumuladas[4] = new TimeSpan(listaDiasT4.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[4] += estadistica.Nocturnas[4] = new TimeSpan(listaDiasT4.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[4] += estadistica.ImporteDietas[4] = listaDiasT4.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[4] += estadistica.DiasFestivos[4] = listaDiasT4.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // DESCANSOS
                    estadisticaTotal.DiasDescanso += estadistica.DiasDescanso = pijama.Descanso + pijama.DescansoEnFinde + pijama.DescansoSuelto;
                    estadisticaTotal.FindesDescansados += estadistica.FindesDescansados = pijama.FindesCompletos;

                    // PLUSES
                    estadisticaTotal.PlusPaqueteria += estadistica.PlusPaqueteria = pijama.PlusPaqueteria;
                    estadisticaTotal.PlusMenorDescanso += estadistica.PlusMenorDescanso = pijama.PlusMenorDescanso;

                    lista.Add(estadistica);
                }
                //lista.Add(estadisticaTotal);
            });
            return lista;
        }


        public async Task<List<EstadisticaPorTurnos>> CargarEstadisticasAñoAsync(int año) {

            var lista = new List<EstadisticaPorTurnos>();
            var listaCalendarios = new List<Calendario>();
            await Task.Run(() => {
                for (int mes = 1; mes <= 12; mes++) {
                    listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(año, mes));
                }
                EstadisticaPorTurnos estadisticaTotal = new EstadisticaPorTurnos();
                estadisticaTotal.Conductor = new Conductor { Matricula = 0, Apellidos = "TOTAL" };
                foreach (Calendario calendario in listaCalendarios) {
                    Pijama.HojaPijama pijama = new Pijama.HojaPijama(calendario, mensajes);
                    EstadisticaPorTurnos estadistica = lista.FirstOrDefault(e => e.Conductor.Matricula == calendario.MatriculaConductor);
                    if (estadistica == null) {
                        estadistica = new EstadisticaPorTurnos();
                        // CONDUCTOR
                        estadistica.Conductor = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);
                        lista.Add(estadistica);
                    }

                    // EVENTUAL PARCIAL
                    if (!estadistica.Conductor.Indefinido && pijama.ListaDias.Any(d => d.Grafico == 0)) estadistica.EventualParcial = true;

                    // TURNO 1
                    estadisticaTotal.Dias[1] += estadistica.Dias[1] += pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1));
                    var listaDiasT1 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1);
                    estadisticaTotal.Trabajadas[1] += estadistica.Trabajadas[1] += new TimeSpan(listaDiasT1.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[1] += estadistica.Acumuladas[1] += new TimeSpan(listaDiasT1.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[1] += estadistica.Nocturnas[1] += new TimeSpan(listaDiasT1.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[1] += estadistica.ImporteDietas[1] += listaDiasT1.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[1] += estadistica.DiasFestivos[1] += listaDiasT1.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 2
                    estadisticaTotal.Dias[2] += estadistica.Dias[2] += pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2));
                    var listaDiasT2 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2);
                    estadisticaTotal.Trabajadas[2] += estadistica.Trabajadas[2] += new TimeSpan(listaDiasT2.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[2] += estadistica.Acumuladas[2] += new TimeSpan(listaDiasT2.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[2] += estadistica.Nocturnas[2] += new TimeSpan(listaDiasT2.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[2] += estadistica.ImporteDietas[2] += listaDiasT2.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[2] += estadistica.DiasFestivos[2] += listaDiasT2.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 3
                    estadisticaTotal.Dias[3] += estadistica.Dias[3] += pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3));
                    var listaDiasT3 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3);
                    estadisticaTotal.Trabajadas[3] += estadistica.Trabajadas[3] += new TimeSpan(listaDiasT3.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[3] += estadistica.Acumuladas[3] += new TimeSpan(listaDiasT3.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[3] += estadistica.Nocturnas[3] += new TimeSpan(listaDiasT3.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[3] += estadistica.ImporteDietas[3] += listaDiasT3.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[3] += estadistica.DiasFestivos[3] += listaDiasT3.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // TURNO 4
                    estadisticaTotal.Dias[4] += estadistica.Dias[4] += pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4));
                    var listaDiasT4 = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4);
                    estadisticaTotal.Trabajadas[4] += estadistica.Trabajadas[4] += new TimeSpan(listaDiasT4.Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.TrabajadasReales.Ticks));
                    estadisticaTotal.Acumuladas[4] += estadistica.Acumuladas[4] += new TimeSpan(listaDiasT4.Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks));
                    estadisticaTotal.Nocturnas[4] += estadistica.Nocturnas[4] += new TimeSpan(listaDiasT4.Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks));
                    estadisticaTotal.ImporteDietas[4] += estadistica.ImporteDietas[4] += listaDiasT4.Sum(d => d.ImporteTotalDietas);
                    estadisticaTotal.DiasFestivos[4] += estadistica.DiasFestivos[4] += listaDiasT4.Count(d => d.Grafico > 0 && d.EsFestivo);

                    // DESCANSOS
                    estadisticaTotal.DiasDescanso += estadistica.DiasDescanso += pijama.Descanso + pijama.DescansoEnFinde + pijama.DescansoSuelto;
                    estadisticaTotal.FindesDescansados += estadistica.FindesDescansados += pijama.FindesCompletos;

                    // PLUSES
                    estadisticaTotal.PlusPaqueteria += estadistica.PlusPaqueteria += pijama.PlusPaqueteria;
                    estadisticaTotal.PlusMenorDescanso += estadistica.PlusMenorDescanso += pijama.PlusMenorDescanso;
                }
                //lista.Add(estadisticaTotal);
            });
            return lista.OrderBy(e => e.Conductor.Matricula).ToList();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================

        // CARGAR ESTADÍSTICAS DEL MES
        public ICommandAsync cmdCargarEstadisticasDeCalendario { get => new CommandAsync(CargarEstadisticasDeCalendarioAsync); }
        private async Task CargarEstadisticasDeCalendarioAsync() {
            IsBusy = true;
            try {
                ListaEstadisticas = await CargarEstadisticasAsync(App.Global.CalendariosVM.ListaCalendarios);
                if (ListaEstadisticas.Count > 0) {
                    TextoCabecera = App.Global.CalendariosVM.FechaActual.ToString("MMMM - yyyy").ToUpper();
                } else {
                    TextoCabecera = "Sin Datos";
                }
            } finally {
                IsBusy = false;
            }
        }


        // CARGAR ESTADÍSTICAS DEL AÑO
        public ICommandAsync cmdCargarEstadisticasCalendariosAño { get => new CommandAsync(CargarEstadisticasCalendariosAñoAsync); }
        private async Task CargarEstadisticasCalendariosAñoAsync() {
            IsBusy = true;
            try {
                ListaEstadisticas = await CargarEstadisticasAñoAsync(App.Global.CalendariosVM.FechaActual.Year);
                TextoCabecera = $"ESTADÍSTICAS DEL AÑO {App.Global.CalendariosVM.FechaActual.Year}";
                if (!App.Global.CalendariosVM.FiltroAplicado.Equals("Ninguno")) {
                    TextoCabecera += $" ({App.Global.CalendariosVM.FiltroAplicado})";
                }
            } finally {
                IsBusy = false;
            }
        }



        // GENERAR EXCEL DE ESTADÍSTICAS
        public ICommand cmdGenerarExcelEstadisticas { get => new RelayCommand(p => GenerarExcelEstadisticas(), p => PuedeGenerarExcelEstadisticas()); }
        private bool PuedeGenerarExcelEstadisticas() => ListaEstadisticas.Any();
        private void GenerarExcelEstadisticas() {
            IsBusy = true;
            try {
                var carpetaInformes = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Informes\\{App.Global.CentroActual}");
                string nombreArchivo = $"Estadisticas Calendarios {App.Global.CentroActual} - {TextoCabecera.Replace(" ", "")}.xlsx";
                string rutaInformes = Path.Combine(carpetaInformes, "Calendarios\\Estadisticas");
                if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
                string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);
                var lista = new List<EstadisticaPorTurnos>();
                foreach (var obj in VistaEstadisticas) {
                    if (obj is EstadisticaPorTurnos estadistica) lista.Add(estadistica);
                }
                ExcelService.getInstance().GenerarEstadisticasCalendario(rutaDestino, lista, TextoCabecera + FiltroAplicado);
                if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);

            } finally {
                IsBusy = false;
            }
        }


        // APLICAR FILTRO
        public ICommand cmdAplicarFiltro { get => new RelayCommand(p => AplicarFiltro(p), p => PuedeAplicarFiltro()); }
        private bool PuedeAplicarFiltro() => ListaEstadisticas.Any();
        private void AplicarFiltro(object parametro) {
            if (parametro is string filtro) {
                switch (filtro) {
                    case "Indefinidos":
                        VistaEstadisticas.Filter = (c) => (c as EstadisticaPorTurnos).Conductor.Indefinido;
                        FiltroAplicado = " (Conductores Indefinidos)";
                        break;
                    case "Eventuales":
                        VistaEstadisticas.Filter = (c) => !(c as EstadisticaPorTurnos).Conductor.Indefinido;
                        FiltroAplicado = " (Conductores Eventuales)";
                        break;
                    case "EventualesParcial":
                        VistaEstadisticas.Filter = (c) => (c as EstadisticaPorTurnos).EventualParcial;
                        FiltroAplicado = " (Conductores Eventuales Parcial)";
                        break;
                    case "Ninguno":
                        VistaEstadisticas.Filter = null;
                        FiltroAplicado = "";
                        break;
                }
            }

        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private List<EstadisticaPorTurnos> listaEstadisticas;
        public List<EstadisticaPorTurnos> ListaEstadisticas {
            get => listaEstadisticas;
            set {
                if (SetValue(ref listaEstadisticas, value)) {
                    VistaEstadisticas = new ListCollectionView(listaEstadisticas);
                }
            }
        }



        private ListCollectionView vistaEstadisticas;

        public ListCollectionView VistaEstadisticas {
            get => vistaEstadisticas;
            set => SetValue(ref vistaEstadisticas, value);
        }



        private DateTime fecha;
        public DateTime Fecha {
            get => fecha;
            set {
                if (SetValue(ref fecha, value)) {
                    PropiedadCambiada(nameof(TextoFecha));
                }
            }
        }



        public string TextoFecha {
            get => Fecha == DateTime.MinValue ? "Sin Datos" : Fecha.ToString("MMMM - yyyy").ToUpper();
        }


        private string textoCabecera = "Sin Datos";
        public string TextoCabecera {
            get => textoCabecera;
            set => SetValue(ref textoCabecera, value);
        }


        private string filtroAplicado = "";
        public string FiltroAplicado {
            get => filtroAplicado;
            set => SetValue(ref filtroAplicado, value);
        }


        private bool isBusy;
        public bool IsBusy {
            get => isBusy;
            set => SetValue(ref isBusy, value);
        }



        private bool mostrarDiasTrabajados = true;
        public bool MostrarDiasTrabajados {
            get => mostrarDiasTrabajados;
            set => SetValue(ref mostrarDiasTrabajados, value);
        }



        private bool mostrarHorasTrabajadas = true;
        public bool MostrarHorasTrabajadas {
            get => mostrarHorasTrabajadas;
            set => SetValue(ref mostrarHorasTrabajadas, value);
        }



        private bool mostrarHorasAcumuladas = true;
        public bool MostrarHorasAcumuladas {
            get => mostrarHorasAcumuladas;
            set => SetValue(ref mostrarHorasAcumuladas, value);
        }



        private bool mostrarHorasNocturnas = true;
        public bool MostrarHorasNocturnas {
            get => mostrarHorasNocturnas;
            set => SetValue(ref mostrarHorasNocturnas, value);
        }



        private bool mostrarImporteDietas = true;
        public bool MostrarImporteDietas {
            get => mostrarImporteDietas;
            set => SetValue(ref mostrarImporteDietas, value);
        }



        private bool mostrarFestivosTrabajados = true;
        public bool MostrarFestivosTrabajados {
            get => mostrarFestivosTrabajados;
            set => SetValue(ref mostrarFestivosTrabajados, value);
        }



        private bool mostrarDescansos = true;
        public bool MostrarDescansos {
            get => mostrarDescansos;
            set => SetValue(ref mostrarDescansos, value);
        }



        private bool mostrarPluses = true;
        public bool MostrarPluses {
            get => mostrarPluses;
            set => SetValue(ref mostrarPluses, value);
        }



        private bool elegirTurnos = true;
        public bool ElegirTurnos {
            get => elegirTurnos;
            set => SetValue(ref elegirTurnos, value);
        }


        private bool elegirJornadas = true;
        public bool ElegirJornadas {
            get => elegirJornadas;
            set => SetValue(ref elegirJornadas, value);
        }


        private bool elegirHoras = true;
        public bool ElegirHoras {
            get => elegirHoras;
            set => SetValue(ref elegirHoras, value);
        }


        private bool elegirDietas = true;
        public bool ElegirDietas {
            get => elegirDietas;
            set => SetValue(ref elegirDietas, value);
        }


        private bool elegirPluses = true;
        public bool ElegirPluses {
            get => elegirPluses;
            set => SetValue(ref elegirPluses, value);
        }



        #endregion
        // ====================================================================================================


    }
}
