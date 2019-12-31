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

    public partial class GraficosViewModel : NotifyBase {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private ObservableCollection<GrupoGraficos> _listagrupos = new ObservableCollection<GrupoGraficos>();
        private List<Grafico> _listaborrados = new List<Grafico>();
        private List<ValoracionGrafico> _valoracionesborradas = new List<ValoracionGrafico>();
        private GrupoGraficos _gruposeleccionado;
        private Grafico _graficoseleccionado;
        private ValoracionGrafico _valoracionseleccionada;
        private bool _haycambios = false;

        // Servicios
        private IMensajes Mensajes;
        private InformesServicio Informes;
        private IFileService FileService;
        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public GraficosViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes, IFileService fileService) {
            // Asignamos los servicios
            Mensajes = servicioMensajes;
            Informes = servicioInformes;
            FileService = fileService;
            // Añadimos los eventos a las listas.
            _listagraficos.CollectionChanged += ListaGraficos_CollectionChanged;
            _listagrupos.CollectionChanged += ListaGrupos_CollectionChanged;
            // Cargamos los grupos de gráficos.
            CargarGrupos();



            // PASO DE ACCESS A SQLITE
            //if (App.Global.Reposritory.GetCount<GrupoGraficos>() == 0) {
            //    var listaSQLite = BdGruposGraficos.getGrupos();
            //    App.Global.Reposritory.GuardarGrupos(listaSQLite);
            //}
            //if (App.Global.Reposritory.GetCount<Grafico>() == 0) {
            //    var listaSQLite = BdGraficos.getGraficosSinLista();
            //    App.Global.Reposritory.GuardarGraficos(listaSQLite);
            //}
            //if (App.Global.Reposritory.GetCount<ValoracionGrafico>() == 0) {
            //    var listaSQLite = BdValoracionesGraficos.getValoraciones();
            //    App.Global.Reposritory.GuardarItems(listaSQLite);
            //}


        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void CargarGraficos() {
            if (App.Global.CadenaConexion == null || GrupoSeleccionado == null) {
                ListaGraficos.Clear();
                return;
            }
            ListaGraficos = BdGraficos.getGraficos(GrupoSeleccionado.Validez);
        }

        public void CargarGrupos() {
            if (App.Global.CadenaConexion == null) {
                ListaGrupos.Clear();
                return;
            }
            ListaGrupos = BdGruposGraficos.getGrupos();

        }

        public void GuardarGraficos() {
            try {
                HayCambios = false;
                if (ListaGraficos != null && ListaGraficos.Count > 0) {
                    BdGraficos.GuardarGraficos(ListaGraficos.Where(gg => gg.Nuevo || gg.Modificado));
                }
                if (_listaborrados.Count > 0) {
                    BdGraficos.BorrarGraficos(_listaborrados);
                    _listaborrados.Clear();
                }
                if (_valoracionesborradas.Count > 0) {
                    BdValoracionesGraficos.BorrarValoraciones(_valoracionesborradas);
                    _valoracionesborradas.Clear();
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosViewModel.GuardarGraficos", ex);
                HayCambios = true;
            }

        }

        //public void GuardarGrupos() { //TODO: Eliminar.
        //    try {
        //        HayCambios = false;
        //        if (ListaGrupos != null && ListaGrupos.Count > 0) {
        //            BdGruposGraficos.GuardarGrupos(ListaGrupos.Where(gg => gg.Nuevo || gg.Modificado));
        //        }
        //    } catch (Exception ex) {
        //        Mensajes.VerError("GraficosViewModel.GuardarGrupos", ex);
        //        HayCambios = true;
        //    } finally {
        //    }
        //}

        public void GuardarTodo() {
            //GuardarGrupos(); // TODO: Eliminar
            GuardarGraficos();
            HayCambios = false;
        }

        public void Reiniciar() {
            CargarGrupos();
            GrupoSeleccionado = null;
            GraficoSeleccionado = null;
            HayCambios = false;
        }

        public bool ColumnaVisible(int numeroColumna) {
            switch (numeroColumna) {
                case 0: return App.Global.Configuracion.MostrarGrafNoCalcular;
                case 5: return App.Global.Configuracion.MostrarGrafTiempoPartido;
                case 6: return App.Global.Configuracion.MostrarGrafTiempoPartido;
                case 8: return App.Global.Configuracion.MostrarGrafHoras;
                case 9: return App.Global.Configuracion.MostrarGrafHoras;
                case 10: return App.Global.Configuracion.MostrarGrafHoras;
                case 11: return App.Global.Configuracion.MostrarGrafDietas;
                case 12: return App.Global.Configuracion.MostrarGrafDietas;
                case 13: return App.Global.Configuracion.MostrarGrafDietas;
                case 14: return App.Global.Configuracion.MostrarGrafDietas;
                case 15: return App.Global.Configuracion.MostrarGrafPlusLimpieza;
                case 16: return App.Global.Configuracion.MostrarGrafPlusPaqueteria;
            }
            return true;
        }

        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaGraficos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (Grafico grafico in e.NewItems) {
                    if (GrupoSeleccionado != null) {
                        grafico.IdGrupo = GrupoSeleccionado.Id;
                        grafico.Validez = GrupoSeleccionado.Validez;
                    }
                    grafico.Nuevo = true;
                    grafico.ObjetoCambiado += ObjetoCambiadoEventHandler;
                    HayCambios = true;
                }
            }

            if (e.OldItems != null) {
                foreach (Grafico grafico in e.OldItems) {
                    grafico.ObjetoCambiado -= ObjetoCambiadoEventHandler;
                }
            }

            PropiedadCambiada(nameof(ListaGraficos));
            PropiedadCambiada(nameof(Detalle));
        }

        private void ListaGrupos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (GrupoGraficos grupo in e.NewItems) {
                }
                HayCambios = true;
            }
            PropiedadCambiada(nameof(ListaGrupos));
        }

        private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
            HayCambios = true;
            PropiedadCambiada(nameof(Detalle));
        }

        private void Valoracionseleccionada_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Linea") {
                // Extraemos la línea
                decimal linea = ValoracionSeleccionada.Linea;
                // Si la línea es menor que cero...
                if (linea < 0) {
                    // Cambiamos el signo.
                    linea = linea * -1;
                    // Si está entre 1000 y 1999...
                    if (linea >= 1000 && linea < 2000) {
                        // Extraemos el descanso.
                        int descanso = (int)(linea - 1000);
                        var indice = GraficoSeleccionado.ListaValoraciones.IndexOf(ValoracionSeleccionada);
                        if (indice > 0) {
                            ValoracionSeleccionada.Inicio = GraficoSeleccionado.ListaValoraciones[indice - 1].Final;
                        } else if (indice == 0 && !ValoracionSeleccionada.Inicio.HasValue) {
                            ValoracionSeleccionada.Inicio = GraficoSeleccionado.Inicio;
                        }
                        ValoracionSeleccionada.Linea = 0m;
                        ValoracionSeleccionada.Descripcion = "Descanso " + descanso.ToString() + " minutos.";
                        ValoracionSeleccionada.Final = ValoracionSeleccionada.Inicio + new TimeSpan(0, descanso, 0);
                    }
                    // Si la línea es mayor que cero.
                } else if (linea > 0) {
                    // Creamos un itinerario.
                    Itinerario itinerario = null;
                    // Buscamos el itinerario.
                    try {
                        itinerario = BdItinerarios.GetItinerarioByNombre(linea);
                    } catch (Exception ex) {
                        Mensajes.VerError("GraficosViewModel.AñadirValoracion", ex);
                    }

                    var indice = GraficoSeleccionado.ListaValoraciones.IndexOf(ValoracionSeleccionada);
                    if (indice > 0) {
                        ValoracionSeleccionada.Inicio = GraficoSeleccionado.ListaValoraciones[indice - 1].Final;
                    } else if (indice == 0 && !ValoracionSeleccionada.Inicio.HasValue) {
                        ValoracionSeleccionada.Inicio = GraficoSeleccionado.Inicio;
                    }
                    ValoracionSeleccionada.Linea = linea;
                    // Si el itinerario no es nulo (existe)...
                    if (itinerario != null) {
                        ValoracionSeleccionada.Descripcion = itinerario.Descripcion;
                        ValoracionSeleccionada.Final = ValoracionSeleccionada.Inicio + new TimeSpan(0, itinerario.TiempoReal, 0);
                    } else {
                        ValoracionSeleccionada.Descripcion = "Línea desconocida.";
                        ValoracionSeleccionada.Final = ValoracionSeleccionada.Inicio;
                    }

                    // Si la línea es cero.
                } else {
                    ValoracionSeleccionada.Final = ValoracionSeleccionada.Inicio;
                }

            }
        }

        private void Gruposeleccionado_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Validez") {
                HayCambios = true;
            }
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================
        private ObservableCollection<Grafico> _listagraficos = new ObservableCollection<Grafico>();
        public ObservableCollection<Grafico> ListaGraficos {
            get { return _listagraficos; }
            set {
                if (_listagraficos != value) {
                    _listagraficos = value;
                    foreach (Grafico g in _listagraficos) {
                        g.ObjetoCambiado += ObjetoCambiadoEventHandler;
                    }
                    _listagraficos.CollectionChanged += ListaGraficos_CollectionChanged;
                    VistaGraficos = new ListCollectionView(ListaGraficos);
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(GraficoSeleccionado));
                }
            }
        }


        private ListCollectionView _vistagraficos;
        public ListCollectionView VistaGraficos {
            get { return _vistagraficos; }
            set {
                if (_vistagraficos != value) {
                    _vistagraficos = value;
                    PropiedadCambiada();
                }
            }
        }

        public ObservableCollection<GrupoGraficos> ListaGrupos {
            get { return _listagrupos; }
            set {
                if (_listagrupos != value) {
                    _listagrupos = value;
                    _listagrupos.CollectionChanged += ListaGrupos_CollectionChanged;
                    VistaGrupos = new ListCollectionView(ListaGrupos);
                    PropiedadCambiada();
                }
            }
        }


        private ListCollectionView _vistagrupos;
        public ListCollectionView VistaGrupos {
            get { return _vistagrupos; }
            set {
                if (_vistagrupos != value) {
                    _vistagrupos = value;
                    SoloAñoActualEnGrupos();
                    PropiedadCambiada();
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
            get => FiltroAplicado != "Ninguno";
        }


        public GrupoGraficos GrupoSeleccionado {
            get { return _gruposeleccionado; }
            set {
                if (_gruposeleccionado != value) {
                    GuardarGraficos();
                    _gruposeleccionado = value;
                    if (_gruposeleccionado != null) {
                        _gruposeleccionado.PropertyChanged += Gruposeleccionado_PropertyChanged;
                    }
                    CargarGraficos();
                    GraficoSeleccionado = null;
                    PropiedadCambiada();
                    HayCambios = false; // Desconozco por qué se establece HayCambios en True cuando se llama a PropiedadCambiada.
                                        // Como los gráficos quedan guardados, se establece de nuevo en False para evitar guardados innecesarios.
                    PropiedadCambiada(nameof(HayGrupo));
                    PropiedadCambiada(nameof(Detalle));
                }
            }
        }


        private GrupoGraficos _grupocomparacionseleccionado;
        public GrupoGraficos GrupoComparacionSeleccionado {
            get { return _grupocomparacionseleccionado; }
            set {
                if (_grupocomparacionseleccionado != value) {
                    _grupocomparacionseleccionado = value;
                    PropiedadCambiada();
                }
            }
        }

        public Grafico GraficoSeleccionado {
            get { return _graficoseleccionado; }
            set {
                if (_graficoseleccionado != value) {
                    _graficoseleccionado = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayGrafico));
                }
            }
        }

        public ValoracionGrafico ValoracionSeleccionada {
            get { return _valoracionseleccionada; }
            set {
                if (_valoracionseleccionada != value) {
                    _valoracionseleccionada = value;
                    if (_valoracionseleccionada != null) {
                        _valoracionseleccionada.PropertyChanged += Valoracionseleccionada_PropertyChanged;
                    }
                    PropiedadCambiada();
                }
            }
        }

        public bool HayGrafico {
            get {
                if (GraficoSeleccionado == null) return false;
                if (GraficoSeleccionado.Nuevo) return false;
                return true;
            }
        }

        public bool HayGrupo {
            get { return GrupoSeleccionado != null; }
        }


        private DateTime _fechaestadisticas = new DateTime(DateTime.Today.Year, 1, 1);
        public DateTime FechaEstadisticas {
            get { return _fechaestadisticas; }
            set {
                if (_fechaestadisticas != value) {
                    _fechaestadisticas = value;
                    PropiedadCambiada();
                }
            }
        }

        public bool HayCambios {
            get { return _haycambios; }
            set {
                if (_haycambios != value) {
                    _haycambios = value;
                    PropiedadCambiada();
                }
            }
        }

        public String Detalle {
            get {
                int num = 0;
                double trab = 0;
                double acum = 0;
                double noc = 0;
                var turnos = new int[4];
                if (VistaGraficos != null) {
                    foreach (object obj in VistaGraficos) {
                        if (obj is Grafico g) {
                            trab += g.Trabajadas.TotalMinutes;
                            acum += g.Acumuladas.TotalMinutes;
                            noc += g.Nocturnas.TotalMinutes;
                            turnos[g.Turno - 1]++;
                        }
                    }
                    num = VistaGraficos.Count - 1;
                }
                string texto = "Graf: " + num.ToString();
                texto += $"  |  Trab: {(trab / 60).ToString("0.00")}  |  Acum: {(acum / 60).ToString("0.00")}  |  Noct: {(noc / 60).ToString("0.00")}";
                texto += $"  |  T-1: {turnos[0]}  |  T-2: {turnos[1]}  |  T-3: {turnos[2]}  |  T-4: {turnos[3]}";
                return texto;
            }
        }

        #endregion

    } //Fin de la clase
}
