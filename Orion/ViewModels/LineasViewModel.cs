#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using Orion.Config;
using Orion.Models;
using Orion.Servicios;

namespace Orion.ViewModels {

    public partial class LineasViewModel : NotifyBase {

        public enum Tablas { Ninguna = 0, Lineas = 1, Itinerarios = 2, Paradas = 4 }

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private List<Linea> _listalineasborradas = new List<Linea>();
        private List<Itinerario> _listaitinerariosborrados = new List<Itinerario>();
        private List<Parada> _listaparadasborradas = new List<Parada>();
        private IMensajes mensajes;

        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public LineasViewModel(IMensajes servicioMensajes) {
            mensajes = servicioMensajes;
            ListaLineas = new NotifyCollection<Linea>();
            CargarLineas();
        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void CargarLineas() {
            if (App.Global.CadenaConexionLineas == null) {
                ListaLineas.Clear();
                return;
            }
            try {
                ListaLineas = new NotifyCollection<Linea>(App.Global.LineasRepo.GetLineas());
            } catch (Exception ex) {
                mensajes.VerError("LineasViewModel.CargarLineas", ex);
            }
            PropiedadCambiada(nameof(Detalle));
        }


        public void GuardarLineas() {
            try {
                if (ListaLineas != null && ListaLineas.Count > 0) {
                    var lista = ListaLineas.Where(item => item.Nuevo || item.Modificado);
                    if (lista.Count() > 0) {
                        App.Global.LineasRepo.GuardarLineas(lista);
                        foreach (var l in lista) {
                            if (l.ItinerariosBorrados.Count > 0) App.Global.LineasRepo.BorrarItems(l.ItinerariosBorrados);
                            foreach (var i in l.ListaItinerarios) {
                                if (i.ParadasBorradas.Count > 0) App.Global.LineasRepo.BorrarItems(i.ParadasBorradas);
                            }
                        }
                    }
                }
                HayCambios = false;
                if (_listalineasborradas.Count > 0) {
                    App.Global.LineasRepo.BorrarLineas(_listalineasborradas);
                    _listalineasborradas.Clear();
                }
            } catch (Exception ex) {
                mensajes.VerError("LineasViewModel.GuardarLineas", ex);
                HayCambios = true;
            }
        }


        public void GuardarTodo() {
            GuardarLineas();
            HayCambios = false;
        }


        public void Reiniciar() {
            LineaSeleccionada = null;
            ItinerarioSeleccionado = null;
            ParadaSeleccionada = null;
            CargarLineas();
            HayCambios = false;
        }

        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaLineas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (Linea linea in e.NewItems) {
                    linea.Nuevo = true;
                    HayCambios = true;
                }
            }

            if (e.OldItems != null) {
            }

            PropiedadCambiada(nameof(ListaLineas));
        }


        private void ListaLineas_ItemPropertyChanged(object sender, ItemChangedEventArgs<Linea> e) {
            HayCambios = true;
        }


        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private NotifyCollection<Linea> _listalineas = new NotifyCollection<Linea>();
        public NotifyCollection<Linea> ListaLineas {
            get { return _listalineas; }
            set {
                if (_listalineas != value) {
                    _listalineas = value;
                    _listalineas.CollectionChanged += ListaLineas_CollectionChanged;
                    _listalineas.ItemPropertyChanged += ListaLineas_ItemPropertyChanged;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(LineaSeleccionada));
                }
            }
        }

        private Linea _lineaseleccionada;
        public Linea LineaSeleccionada {
            get { return _lineaseleccionada; }
            set {
                if (_lineaseleccionada != value) {
                    _lineaseleccionada = value;
                    ItinerarioSeleccionado = null;
                    _listaitinerariosborrados.Clear();
                    TablaSeleccionada = Tablas.Lineas;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayLinea));
                    PropiedadCambiada(nameof(Detalle));
                }
            }
        }


        private Itinerario _itinerarioseleccionado;
        public Itinerario ItinerarioSeleccionado {
            get { return _itinerarioseleccionado; }
            set {
                if (_itinerarioseleccionado != value) {
                    _itinerarioseleccionado = value;
                    ParadaSeleccionada = null;
                    _listaparadasborradas.Clear();
                    TablaSeleccionada = Tablas.Itinerarios;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayItinerario));
                    PropiedadCambiada(nameof(Detalle));
                }
            }
        }


        private Parada _paradaseleccionada;
        public Parada ParadaSeleccionada {
            get { return _paradaseleccionada; }
            set {
                if (_paradaseleccionada != value) {
                    _paradaseleccionada = value;
                    TablaSeleccionada = Tablas.Paradas;
                    PropiedadCambiada();
                }
            }
        }


        public bool HayLinea {
            get {
                if (LineaSeleccionada == null) return false;
                if (LineaSeleccionada.Nuevo) return false;
                return true;
            }
        }


        public bool HayItinerario {
            get {
                if (ItinerarioSeleccionado == null) return false;
                if (ItinerarioSeleccionado.Nuevo) return false;
                return true;
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


        private Tablas _tablaseleccionada = Tablas.Ninguna;
        public Tablas TablaSeleccionada {
            get { return _tablaseleccionada; }
            set {
                if (_tablaseleccionada != value) {
                    _tablaseleccionada = value;
                    PropiedadCambiada();
                }
            }
        }


        private DataGrid _tablaparacopy;
        public DataGrid TablaParaCopy {
            get { return _tablaparacopy; }
            set {
                if (_tablaparacopy != value) {
                    _tablaparacopy = value;
                    PropiedadCambiada();
                }
            }
        }


        public String Detalle {
            get {
                string texto = "Lineas: " + ListaLineas.Count.ToString();
                if (LineaSeleccionada != null) texto += "  |  Itinerarios: " + LineaSeleccionada.ListaItinerarios.Count.ToString();
                if (ItinerarioSeleccionado != null) texto += "  |  Paradas: " + ItinerarioSeleccionado.ListaParadas.Count.ToString();
                return texto;
            }
        }

        #endregion

    }
}
