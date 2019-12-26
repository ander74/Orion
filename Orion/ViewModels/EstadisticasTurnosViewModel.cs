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
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;
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

        public async Task<List<EstadisticaPorTurnos>> CargarEstadisticas(IEnumerable<Calendario> listaCalendarios) {

            var lista = new List<EstadisticaPorTurnos>();
            await Task.Run(() => {
                foreach (Calendario calendario in listaCalendarios) {
                    Pijama.HojaPijama pijama = new Pijama.HojaPijama(calendario, mensajes);
                    EstadisticaPorTurnos estadistica = new EstadisticaPorTurnos();
                    // CONDUCTOR
                    estadistica.Conductor = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);
                    // TURNO 1
                    estadistica.Dias[1] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)).ToString("00");
                    estadistica.Trabajadas[1] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)
                                                                .Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.GraficoTrabajado.TrabajadasReales.Ticks))
                                                                .ToTexto(true);
                    estadistica.Acumuladas[1] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)
                                                                .Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks))
                                                                .ToTexto(true);
                    estadistica.Nocturnas[1] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)
                                                                .Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks))
                                                                .ToTexto(true);
                    estadistica.ImporteDietas[1] = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)
                                                                   .Sum(d => d.ImporteTotalDietas)
                                                                   .ToString("C2");
                    estadistica.DiasFestivos[1] = pijama.ListaDias.Count(d => (d.TurnoAlt.HasValue ? d.TurnoAlt == 1 : d.GraficoTrabajado.Turno == 1)
                                                                              && d.Grafico > 0 && (d.EsFestivo)).ToString("00");
                    // TURNO 2
                    estadistica.Dias[2] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)).ToString("00");
                    estadistica.Trabajadas[2] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)
                                                                .Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.GraficoTrabajado.TrabajadasReales.Ticks))
                                                                .ToTexto(true);
                    estadistica.Acumuladas[2] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)
                                                                .Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks))
                                                                .ToTexto(true);
                    estadistica.Nocturnas[2] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)
                                                                .Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks))
                                                                .ToTexto(true);
                    estadistica.ImporteDietas[2] = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)
                                                                   .Sum(d => d.ImporteTotalDietas)
                                                                   .ToString("C2");
                    estadistica.DiasFestivos[2] = pijama.ListaDias.Count(d => (d.TurnoAlt.HasValue ? d.TurnoAlt == 2 : d.GraficoTrabajado.Turno == 2)
                                                                              && d.Grafico > 0 && (d.EsFestivo)).ToString("00");
                    // TURNO 3
                    estadistica.Dias[3] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)).ToString("00");
                    estadistica.Trabajadas[3] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)
                                                                .Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.GraficoTrabajado.TrabajadasReales.Ticks))
                                                                .ToTexto(true);
                    estadistica.Acumuladas[3] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)
                                                                .Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks))
                                                                .ToTexto(true);
                    estadistica.Nocturnas[3] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)
                                                                .Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks))
                                                                .ToTexto(true);
                    estadistica.ImporteDietas[3] = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)
                                                                   .Sum(d => d.ImporteTotalDietas)
                                                                   .ToString("C2");
                    estadistica.DiasFestivos[3] = pijama.ListaDias.Count(d => (d.TurnoAlt.HasValue ? d.TurnoAlt == 3 : d.GraficoTrabajado.Turno == 3)
                                                                              && d.Grafico > 0 && (d.EsFestivo)).ToString("00");
                    // TURNO 4
                    estadistica.Dias[4] = pijama.ListaDias.Count(d => (d.Grafico > 0) && (d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)).ToString("00");
                    estadistica.Trabajadas[4] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)
                                                                .Sum(d => d.TrabajadasAlt.HasValue ? d.TrabajadasAlt.Value.Ticks : d.GraficoTrabajado.TrabajadasReales.Ticks))
                                                                .ToTexto(true);
                    estadistica.Acumuladas[4] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)
                                                                .Sum(d => d.AcumuladasAlt.HasValue ? d.AcumuladasAlt.Value.Ticks : d.GraficoTrabajado.Acumuladas.Ticks))
                                                                .ToTexto(true);
                    estadistica.Nocturnas[4] = new TimeSpan(pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)
                                                                .Sum(d => d.NocturnasAlt.HasValue ? d.NocturnasAlt.Value.Ticks : d.GraficoTrabajado.Nocturnas.Ticks))
                                                                .ToTexto(true);
                    estadistica.ImporteDietas[4] = pijama.ListaDias.Where(d => d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)
                                                                   .Sum(d => d.ImporteTotalDietas)
                                                                   .ToString("C2");
                    estadistica.DiasFestivos[4] = pijama.ListaDias.Count(d => (d.TurnoAlt.HasValue ? d.TurnoAlt == 4 : d.GraficoTrabajado.Turno == 4)
                                                                              && d.Grafico > 0 && (d.EsFestivo)).ToString("00");
                    // DÍAS TRABAJO
                    estadistica.DiasTrabajo = pijama.Trabajo.ToString("00");
                    // DÍAS DESCANSO
                    estadistica.DiasDescanso = pijama.Descanso.ToString("00");
                    // FINDES TRABAJO No hay forma de saberlo.
                    //estadistica.FindesTrabajados = pijama.FindesCompletos.ToString("00");
                    // FINDES DESCANSO
                    estadistica.FindesDescansados = pijama.FindesCompletos.ToString("0.00");
                    // PLUS PAQUETERIA
                    estadistica.PlusPaqueteria = pijama.PlusPaqueteria.ToString("0.00");
                    // PLUS MENOR DESCANSO
                    estadistica.PlusMenorDescanso = pijama.PlusMenorDescanso.ToString("0.00");

                    lista.Add(estadistica);
                }
            });
            return lista;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================

        public ICommandAsync cmdCargarEstadisticasDeCalendario { get => new CommandAsync(CargarEstadisticasDeCalendario); }
        private async Task CargarEstadisticasDeCalendario() {
            IsBusy = true;
            try {
                List<Calendario> lista = new List<Calendario>();
                foreach (Object obj in App.Global.CalendariosVM.VistaCalendarios) {
                    if (obj is Calendario cal) lista.Add(cal);
                }
                ListaEstadisticas = await CargarEstadisticas(lista);
                if (lista.Count > 0) {
                    Fecha = lista[0].Fecha;
                } else {
                    Fecha = DateTime.MinValue;
                }
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



        private bool isBusy;
        public bool IsBusy {
            get => isBusy;
            set => SetValue(ref isBusy, value);
        }



        #endregion
        // ====================================================================================================


    }
}
