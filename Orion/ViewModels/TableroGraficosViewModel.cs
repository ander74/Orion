#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using LiveCharts;
using Orion.DataModels;
using Orion.Models;
using Orion.Servicios;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.ViewModels {

    public class TableroGraficosViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private IMensajes mensajes;
        private InformesServicio informes;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public TableroGraficosViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes) {
            this.mensajes = servicioMensajes;
            this.informes = servicioInformes;
            // Creamos el formato de las etiquetas de los totales.
            FormatoTotales = valor => {
                decimal porcentaje = TotalGraficos > 0 ? (decimal)Math.Round(valor * 100d / TotalGraficos, 2) : 0;
                return $"{valor}\n {porcentaje:0.00} %".Replace(".", ",");
            };

            // Cargamos los datos de las estadísticas.
            GruposArrasate = BdGruposGraficos.getGrupos(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Arrasate))).ToList();
            GruposBilbao = BdGruposGraficos.getGrupos(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Bilbao))).ToList();
            GruposDonosti = BdGruposGraficos.getGrupos(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Donosti))).ToList();
            GruposVitoria = BdGruposGraficos.getGrupos(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Vitoria))).ToList();
            GrupoSeleccionadoArrasate = GruposArrasate.FirstOrDefault();
            GrupoSeleccionadoBilbao = GruposBilbao.FirstOrDefault();
            GrupoSeleccionadoDonosti = GruposDonosti.FirstOrDefault();
            GrupoSeleccionadoVitoria = GruposVitoria.FirstOrDefault();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private void CargarDatosArrasate() {
            if (GrupoSeleccionadoArrasate == null) {
                EstArrasate = new EstadisticaGrupoGraficos();
                TurnosArrasate = null;
                HoraMediaArrasate = null;
            } else {
                EstArrasate = BdEstadisticas.GetEstadisticasGrupoGraficos(GrupoSeleccionadoArrasate.Id, Centros.Arrasate) ?? new EstadisticaGrupoGraficos();
                TituloArrasate = $"ARRASATE\n({EstArrasate.Validez:dd-MM-yy})";
                // TURNOS
                TurnosArrasate = new ChartValues<decimal> { EstArrasate.PorcentajeTurnos1, EstArrasate.PorcentajeTurnos2,
                                                            EstArrasate.PorcentajeTurnos3, EstArrasate.PorcentajeTurnos4 };
                // HORA MEDIA
                HoraMediaArrasate = new ChartValues<long> { EstArrasate.MediaTrabajadas.Ticks, EstArrasate.MediaTrabajadasTurno1.Ticks,
                                                            EstArrasate.MediaTrabajadasTurno2.Ticks, EstArrasate.MediaTrabajadasTurno3.Ticks, EstArrasate.MediaTrabajadasTurno4.Ticks};
            }
        }


        private void CargarDatosBilbao() {
            if (GrupoSeleccionadoBilbao == null) {
                EstBilbao = new EstadisticaGrupoGraficos();
                TurnosBilbao = null;
                HoraMediaBilbao = null;
            } else {
                EstBilbao = BdEstadisticas.GetEstadisticasGrupoGraficos(GrupoSeleccionadoBilbao.Id, Centros.Bilbao) ?? new EstadisticaGrupoGraficos();
                TituloBilbao = $"BILBAO\n({EstBilbao.Validez:dd-MM-yy})";
                // TURNOS
                TurnosBilbao = new ChartValues<decimal> { EstBilbao.PorcentajeTurnos1, EstBilbao.PorcentajeTurnos2,
                                                          EstBilbao.PorcentajeTurnos3, EstBilbao.PorcentajeTurnos4 };
                // HORA MEDIA
                HoraMediaBilbao = new ChartValues<long> { EstBilbao.MediaTrabajadas.Ticks, EstBilbao.MediaTrabajadasTurno1.Ticks,
                                                          EstBilbao.MediaTrabajadasTurno2.Ticks, EstBilbao.MediaTrabajadasTurno3.Ticks, EstBilbao.MediaTrabajadasTurno4.Ticks};
            }
        }


        private void CargarDatosDonosti() {
            if (GrupoSeleccionadoDonosti == null) {
                EstDonosti = new EstadisticaGrupoGraficos();
                TurnosDonosti = null;
                HoraMediaDonosti = null;
            } else {
                EstDonosti = BdEstadisticas.GetEstadisticasGrupoGraficos(GrupoSeleccionadoDonosti.Id, Centros.Donosti) ?? new EstadisticaGrupoGraficos();
                TituloDonosti = $"DONOSTI\n({EstDonosti.Validez:dd-MM-yy})";
                // TURNOS
                TurnosDonosti = new ChartValues<decimal> { EstDonosti.PorcentajeTurnos1, EstDonosti.PorcentajeTurnos2,
                                                           EstDonosti.PorcentajeTurnos3, EstDonosti.PorcentajeTurnos4 };
                // HORA MEDIA
                HoraMediaDonosti = new ChartValues<long> { EstDonosti.MediaTrabajadas.Ticks, EstDonosti.MediaTrabajadasTurno1.Ticks,
                                                           EstDonosti.MediaTrabajadasTurno2.Ticks, EstDonosti.MediaTrabajadasTurno3.Ticks, EstDonosti.MediaTrabajadasTurno4.Ticks};
            }
        }


        private void CargarDatosVitoria() {
            if (GrupoSeleccionadoVitoria == null) {
                EstVitoria = new EstadisticaGrupoGraficos();
                TurnosVitoria = null;
                HoraMediaVitoria = null;
            } else {
                EstVitoria = BdEstadisticas.GetEstadisticasGrupoGraficos(GrupoSeleccionadoVitoria.Id, Centros.Vitoria) ?? new EstadisticaGrupoGraficos();
                TituloVitoria = $"GASTEIZ\n({EstVitoria.Validez:dd-MM-yy})";
                // TURNOS
                TurnosVitoria = new ChartValues<decimal> { EstVitoria.PorcentajeTurnos1, EstVitoria.PorcentajeTurnos2,
                                                           EstVitoria.PorcentajeTurnos3, EstVitoria.PorcentajeTurnos4 };
                // HORA MEDIA
                HoraMediaVitoria = new ChartValues<long> { EstVitoria.MediaTrabajadas.Ticks, EstVitoria.MediaTrabajadasTurno1.Ticks,
                                                           EstVitoria.MediaTrabajadasTurno2.Ticks, EstVitoria.MediaTrabajadasTurno3.Ticks, EstVitoria.MediaTrabajadasTurno4.Ticks};
            }
        }


        private void CargarDatosGlobal() {
            int totalGraficos = EstArrasate.CantidadGraficos + EstBilbao.CantidadGraficos + EstDonosti.CantidadGraficos + EstVitoria.CantidadGraficos;
            int totalTurnos1 = EstArrasate.CantidadTurnos1 + EstBilbao.CantidadTurnos1 + EstDonosti.CantidadTurnos1 + EstVitoria.CantidadTurnos1;
            int totalTurnos2 = EstArrasate.CantidadTurnos2 + EstBilbao.CantidadTurnos2 + EstDonosti.CantidadTurnos2 + EstVitoria.CantidadTurnos2;
            int totalTurnos3 = EstArrasate.CantidadTurnos3 + EstBilbao.CantidadTurnos3 + EstDonosti.CantidadTurnos3 + EstVitoria.CantidadTurnos3;
            int totalTurnos4 = EstArrasate.CantidadTurnos4 + EstBilbao.CantidadTurnos4 + EstDonosti.CantidadTurnos4 + EstVitoria.CantidadTurnos4;
            TimeSpan totalTrabajadas = EstArrasate.Trabajadas + EstBilbao.Trabajadas + EstDonosti.Trabajadas + EstVitoria.Trabajadas;
            TimeSpan totalTrabajadasTurno1 = EstArrasate.TrabajadasTurno1 + EstBilbao.TrabajadasTurno1 + EstDonosti.TrabajadasTurno1 + EstVitoria.TrabajadasTurno1;
            TimeSpan totalTrabajadasTurno2 = EstArrasate.TrabajadasTurno2 + EstBilbao.TrabajadasTurno2 + EstDonosti.TrabajadasTurno2 + EstVitoria.TrabajadasTurno2;
            TimeSpan totalTrabajadasTurno3 = EstArrasate.TrabajadasTurno3 + EstBilbao.TrabajadasTurno3 + EstDonosti.TrabajadasTurno3 + EstVitoria.TrabajadasTurno3;
            TimeSpan totalTrabajadasTurno4 = EstArrasate.TrabajadasTurno4 + EstBilbao.TrabajadasTurno4 + EstDonosti.TrabajadasTurno4 + EstVitoria.TrabajadasTurno4;
            if (totalGraficos > 0) {
                TurnosGlobal = new ChartValues<decimal>(new decimal[] { Math.Round(totalTurnos1 * 100m / totalGraficos, 2),
                                                                        Math.Round(totalTurnos2 * 100m / totalGraficos, 2),
                                                                        Math.Round(totalTurnos3 * 100m / totalGraficos, 2),
                                                                        Math.Round(totalTurnos4 * 100m / totalGraficos, 2) });
                HoraMediaGlobal = new ChartValues<long>(new long[] { totalTrabajadas.Ticks / totalGraficos,
                                                                     totalTurnos1 == 0 ? 0 : totalTrabajadasTurno1.Ticks / totalTurnos1,
                                                                     totalTurnos2 == 0 ? 0 : totalTrabajadasTurno2.Ticks / totalTurnos2,
                                                                     totalTurnos3 == 0 ? 0 : totalTrabajadasTurno3.Ticks / totalTurnos3,
                                                                     totalTurnos4 == 0 ? 0 : totalTrabajadasTurno4.Ticks / totalTurnos4 });
            } else {
                TurnosGlobal = null;
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void CargarDatos() {
            CargarDatosArrasate();
            CargarDatosBilbao();
            CargarDatosDonosti();
            CargarDatosVitoria();
            CargarDatosGlobal();
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private bool _vertablero;
        public bool VerTablero {
            get { return _vertablero; }
            set {
                if (_vertablero != value) {
                    _vertablero = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _tituloarrasate = "ARRASATE";
        public string TituloArrasate {
            get { return _tituloarrasate; }
            set {
                if (_tituloarrasate != value) {
                    _tituloarrasate = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _titulobilbao = "BILBAO";
        public string TituloBilbao {
            get { return _titulobilbao; }
            set {
                if (_titulobilbao != value) {
                    _titulobilbao = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _titulodonosti = "DONOSTI";
        public string TituloDonosti {
            get { return _titulodonosti; }
            set {
                if (_titulodonosti != value) {
                    _titulodonosti = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _titulovitoria = "GASTEIZ";
        public string TituloVitoria {
            get { return _titulovitoria; }
            set {
                if (_titulovitoria != value) {
                    _titulovitoria = value;
                    PropiedadCambiada();
                }
            }
        }


        private EstadisticaGrupoGraficos _estarrasate = new EstadisticaGrupoGraficos();
        public EstadisticaGrupoGraficos EstArrasate {
            get { return _estarrasate; }
            set {
                if (_estarrasate != value) {
                    _estarrasate = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TotalGraficos));
                }
            }
        }


        private EstadisticaGrupoGraficos _estbilbao = new EstadisticaGrupoGraficos();
        public EstadisticaGrupoGraficos EstBilbao {
            get { return _estbilbao; }
            set {
                if (_estbilbao != value) {
                    _estbilbao = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TotalGraficos));
                }
            }
        }


        private EstadisticaGrupoGraficos _estdonosti = new EstadisticaGrupoGraficos();
        public EstadisticaGrupoGraficos EstDonosti {
            get { return _estdonosti; }
            set {
                if (_estdonosti != value) {
                    _estdonosti = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TotalGraficos));
                }
            }
        }


        private EstadisticaGrupoGraficos _estvitoria = new EstadisticaGrupoGraficos();
        public EstadisticaGrupoGraficos EstVitoria {
            get { return _estvitoria; }
            set {
                if (_estvitoria != value) {
                    _estvitoria = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TotalGraficos));
                }
            }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES GRUPOS
        // ====================================================================================================

        private List<GrupoGraficos> _gruposarrasate;
        public List<GrupoGraficos> GruposArrasate {
            get { return MostrarSoloAñoActual ? _gruposarrasate.Where(G => G.Validez.Year == DateTime.Today.Year).ToList() : _gruposarrasate; }
            set {
                if (_gruposarrasate != value) {
                    _gruposarrasate = value;
                    PropiedadCambiada();
                }
            }
        }


        private List<GrupoGraficos> _gruposbilbao;
        public List<GrupoGraficos> GruposBilbao {
            get { return MostrarSoloAñoActual ? _gruposbilbao.Where(G => G.Validez.Year == DateTime.Today.Year).ToList() : _gruposbilbao; }
            set {
                if (_gruposbilbao != value) {
                    _gruposbilbao = value;
                    PropiedadCambiada();
                }
            }
        }


        private List<GrupoGraficos> _gruposdonosti;
        public List<GrupoGraficos> GruposDonosti {
            get { return MostrarSoloAñoActual ? _gruposdonosti.Where(G => G.Validez.Year == DateTime.Today.Year).ToList() : _gruposdonosti; }
            set {
                if (_gruposdonosti != value) {
                    _gruposdonosti = value;
                    PropiedadCambiada();
                }
            }
        }


        private List<GrupoGraficos> _gruposvitoria;
        public List<GrupoGraficos> GruposVitoria {
            get { return MostrarSoloAñoActual ? _gruposvitoria.Where(G => G.Validez.Year == DateTime.Today.Year).ToList() : _gruposvitoria; }
            set {
                if (_gruposvitoria != value) {
                    _gruposvitoria = value;
                    PropiedadCambiada();
                }
            }
        }


        private GrupoGraficos _gruposeleccionadoarrasate;
        public GrupoGraficos GrupoSeleccionadoArrasate {
            get { return _gruposeleccionadoarrasate; }
            set {
                if (_gruposeleccionadoarrasate != value) {
                    _gruposeleccionadoarrasate = value;
                    CargarDatosArrasate();
                    CargarDatosGlobal();
                    PropiedadCambiada();
                }
            }
        }


        private GrupoGraficos _gruposeleccionadobilbao;
        public GrupoGraficos GrupoSeleccionadoBilbao {
            get { return _gruposeleccionadobilbao; }
            set {
                if (_gruposeleccionadobilbao != value) {
                    _gruposeleccionadobilbao = value;
                    CargarDatosBilbao();
                    CargarDatosGlobal();
                    PropiedadCambiada();
                }
            }
        }


        private GrupoGraficos _gruposeleccionadodonosti;
        public GrupoGraficos GrupoSeleccionadoDonosti {
            get { return _gruposeleccionadodonosti; }
            set {
                if (_gruposeleccionadodonosti != value) {
                    _gruposeleccionadodonosti = value;
                    CargarDatosDonosti();
                    CargarDatosGlobal();
                    PropiedadCambiada();
                }
            }
        }


        private GrupoGraficos _gruposeleccionadovitoria;
        public GrupoGraficos GrupoSeleccionadoVitoria {
            get { return _gruposeleccionadovitoria; }
            set {
                if (_gruposeleccionadovitoria != value) {
                    _gruposeleccionadovitoria = value;
                    CargarDatosVitoria();
                    CargarDatosGlobal();
                    PropiedadCambiada();
                }
            }
        }


        private bool _mostrarsoloañoactual = App.Global.Configuracion.SoloAñoActualEnGruposGraficos;
        public bool MostrarSoloAñoActual {
            get { return _mostrarsoloañoactual; }
            set {
                if (_mostrarsoloañoactual != value) {
                    _mostrarsoloañoactual = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(GruposArrasate));
                    PropiedadCambiada(nameof(GruposBilbao));
                    PropiedadCambiada(nameof(GruposDonosti));
                    PropiedadCambiada(nameof(GruposVitoria));
                }
            }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES TURNOS
        // ====================================================================================================


        private ChartValues<decimal> _turnosarrasate;
        public ChartValues<decimal> TurnosArrasate {
            get { return _turnosarrasate ?? new ChartValues<decimal>(new decimal[] { 0, 0, 0, 0 }); }
            set {
                if (_turnosarrasate != value) {
                    _turnosarrasate = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<decimal> _turnosbilbao;
        public ChartValues<decimal> TurnosBilbao {
            get { return _turnosbilbao ?? new ChartValues<decimal>(new decimal[] { 0, 0, 0, 0 }); }
            set {
                if (_turnosbilbao != value) {
                    _turnosbilbao = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<decimal> _turnosdonosti;
        public ChartValues<decimal> TurnosDonosti {
            get { return _turnosdonosti ?? new ChartValues<decimal>(new decimal[] { 0, 0, 0, 0 }); }
            set {
                if (_turnosdonosti != value) {
                    _turnosdonosti = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<decimal> _turnosvitoria;
        public ChartValues<decimal> TurnosVitoria {
            get { return _turnosvitoria ?? new ChartValues<decimal>(new decimal[] { 0, 0, 0, 0 }); }
            set {
                if (_turnosvitoria != value) {
                    _turnosvitoria = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<decimal> _turnosglobal;
        public ChartValues<decimal> TurnosGlobal {
            get { return _turnosglobal ?? new ChartValues<decimal>(new decimal[] { 0, 0, 0, 0 }); }
            set {
                if (_turnosglobal != value) {
                    _turnosglobal = value;
                    PropiedadCambiada();
                }
            }
        }


        private string[] _etiquetasturnos = new[] { "Turno 1", "Turno 2", "Turno 3", "Turno 4" };
        public string[] EtiquetasTurnos{
            get { return _etiquetasturnos; }
            set {
                if (_etiquetasturnos != value) {
                    _etiquetasturnos = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatoturnos = v => v.ToString("0.00").Replace(".",",") + " %";
        public Func<double, string> FormatoTurnos {
            get { return _formatoturnos; }
            set {
                if (_formatoturnos != value) {
                    _formatoturnos = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES HORA MEDIA
        // ====================================================================================================


        public long HoraMedia {
            get { return App.Global.Convenio.JornadaMedia.Ticks; }
        }


        private ChartValues<long> _horamediaarrasate;
        public ChartValues<long> HoraMediaArrasate {
            get { return _horamediaarrasate ?? new ChartValues<long>(new long[] { 0, 0, 0, 0, 0 }); }
            set {
                if (_horamediaarrasate != value) {
                    _horamediaarrasate = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<long> _horamediabilbao;
        public ChartValues<long> HoraMediaBilbao {
            get { return _horamediabilbao ?? new ChartValues<long>(new long[] { 0, 0, 0, 0, 0 }); }
            set {
                if (_horamediabilbao != value) {
                    _horamediabilbao = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<long> _horamediadonosti;
        public ChartValues<long> HoraMediaDonosti {
            get { return _horamediadonosti ?? new ChartValues<long>(new long[] { 0, 0, 0, 0, 0 }); }
            set {
                if (_horamediadonosti != value) {
                    _horamediadonosti = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<long> _horamediavitoria;
        public ChartValues<long> HoraMediaVitoria {
            get { return _horamediavitoria ?? new ChartValues<long>(new long[] { 0, 0, 0, 0, 0 }); }
            set {
                if (_horamediavitoria != value) {
                    _horamediavitoria = value;
                    PropiedadCambiada();
                }
            }
        }


        private ChartValues<long> _horamediaglobal;
        public ChartValues<long> HoraMediaGlobal {
            get { return _horamediaglobal ?? new ChartValues<long>(new long[] { 0, 0, 0, 0, 0}); }
            set {
                if (_horamediaglobal != value) {
                    _horamediaglobal = value;
                    PropiedadCambiada();
                }
            }
        }


        private string[] _etiquetashoramedia = new[] { "Todos", "Turno 1", "Turno 2", "Turno 3", "Turno 4" };
        public string[] EtiquetasHoraMedia {
            get { return _etiquetashoramedia; }
            set {
                if (_etiquetashoramedia != value) {
                    _etiquetashoramedia = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatohoramedia = v => new TimeSpan((long)v).ToTexto(true);
        public Func<double, string> FormatoHoraMedia {
            get { return _formatohoramedia; }
            set {
                if (_formatohoramedia != value) {
                    _formatohoramedia = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES TOTALES
        // ====================================================================================================

        public int TotalGraficos {
            get { return EstArrasate.CantidadGraficos + EstBilbao.CantidadGraficos + EstDonosti.CantidadGraficos + EstVitoria.CantidadGraficos; }
        }


        private Func<double, string> _formatototales;
        public Func<double, string> FormatoTotales {
            get { return _formatototales; }
            set {
                if (_formatototales != value) {
                    _formatototales = value;
                    PropiedadCambiada();
                }
            }
        }



        #endregion
        // ====================================================================================================



    }
}
