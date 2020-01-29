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
                    switch (App.Global.CalendariosVM.FiltroAplicado) {
                        case "Conductores Indefinidos":
                            listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(año, mes)
                                .Where(c => App.Global.ConductoresVM.IsIndefinido(c.MatriculaConductor)));
                            break;
                        case "Conductores Eventuales":
                            listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(año, mes)
                                .Where(c => !App.Global.ConductoresVM.IsIndefinido(c.MatriculaConductor)));
                            break;
                        case "Conductores Eventuales Parcial":
                            listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(año, mes)
                                .Where(c => !App.Global.ConductoresVM.IsIndefinido(c.MatriculaConductor) && c.ListaDias.Any(d => d.Grafico == 0)));
                            break;
                        default:
                            listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(año, mes));
                            break;
                    }

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

        public ICommandAsync cmdCargarEstadisticasDeCalendario { get => new CommandAsync(CargarEstadisticasDeCalendarioAsync); }
        private async Task CargarEstadisticasDeCalendarioAsync() {
            IsBusy = true;
            try {
                List<Calendario> lista = new List<Calendario>();
                foreach (Object obj in App.Global.CalendariosVM.VistaCalendarios) {
                    if (obj is Calendario cal) lista.Add(cal);
                }
                ListaEstadisticas = await CargarEstadisticasAsync(lista);
                if (lista.Count > 0) {
                    Fecha = lista[0].Fecha;
                    TextoCabecera = lista[0].Fecha.ToString("MMMM - yyyy").ToUpper();
                    if (!App.Global.CalendariosVM.FiltroAplicado.Equals("Ninguno")) {
                        TextoCabecera += $" ({App.Global.CalendariosVM.FiltroAplicado})";
                    }
                } else {
                    Fecha = DateTime.MinValue;
                    TextoCabecera = "Sin Datos";
                }
            } finally {
                IsBusy = false;
            }
        }


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



        // Comando
        public ICommand cmdGenerarExcelEstadisticas { get => new RelayCommand(p => GenerarExcelEstadisticas(), p => PuedeGenerarExcelEstadisticas()); }
        private bool PuedeGenerarExcelEstadisticas() => ListaEstadisticas.Any();
        private void GenerarExcelEstadisticas() {
            IsBusy = true;
            try {
                string nombreArchivo = $"Estadisticas Calendarios {App.Global.CentroActual} - {TextoCabecera.Replace(" ", "")}.xlsx";
                string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Calendarios\\Estadisticas");
                if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
                string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);
                ExcelService.getInstance().GenerarEstadisticasCalendario(rutaDestino, ListaEstadisticas, textoCabecera);
                if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);

            } finally {
                IsBusy = false;
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



        #endregion
        // ====================================================================================================


    }
}
