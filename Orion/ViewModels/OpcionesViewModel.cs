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
using System.Windows;
using Orion.Config;
using Orion.Models;
using Orion.Servicios;

namespace Orion.ViewModels {

    public partial class OpcionesViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private IMensajes mensajes;

        private List<ArticuloConvenio> articulosEliminados = new List<ArticuloConvenio>();

        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public OpcionesViewModel(IMensajes servicioMensajes) {
            mensajes = servicioMensajes;
            CargarDatos();
            AñoPluses = DateTime.Now.Year;
            App.Global.Configuracion.PropertyChanged += Configuracion_PropertyChanged;
            App.Global.Convenio.PropertyChanged += Configuracion_PropertyChanged;
            App.Global.PorCentro.PropertyChanged += Configuracion_PropertyChanged;
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public void CargarDatos() {
            if (App.Global.CadenaConexion == null) {
                ListaPluses = new NotifyCollection<Pluses>();
            } else {
                ListaPluses = new NotifyCollection<Pluses>(App.Global.Repository.GetPluses());
            }
            if (App.Global.CadenaConexionLineas == null) {
                ArticulosConvenio = new NotifyCollection<ArticuloConvenio>();
            } else {
                ArticulosConvenio = new NotifyCollection<ArticuloConvenio>(App.Global.LineasRepo.GetArticulosConvenio());
            }
        }


        public void GuardarDatos() {
            App.Global.Repository.GuardarPluses(ListaPluses.Where(item => item.Nuevo || item.Modificado));
            App.Global.LineasRepo.GuardarArticulosConvenio(ArticulosConvenio.Where(item => item.Nuevo || item.Modificado));
            if (articulosEliminados.Any()) {
                App.Global.LineasRepo.BorrarArticulosConvenio(articulosEliminados);
                articulosEliminados.Clear();
            }
            HayCambios = false;
        }


        public void GuardarTodo() {
            GuardarDatos();
            HayCambios = false;
        }


        public void Reiniciar() {
            CargarDatos();
            HayCambios = false;
        }


        // USO DE LOS PLUSES
        // ----------------------------------------------------------------------------------------------------

        public Pluses GetPluses(int año) {
            Pluses resultado = ListaPluses.FirstOrDefault(p => p.Año == año);
            if (resultado == null) resultado = new Pluses();
            return resultado;
        }



        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void ListaPluses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (Pluses plus in e.NewItems) {
                    plus.Nuevo = true;
                }
            }
            HayCambios = true;
        }



        private void ListaPluses_ItemPropertyChanged(object sender, ItemChangedEventArgs<Pluses> e) {
            HayCambios = true;
        }


        private void Configuracion_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(App.Global.Configuracion.CarpetaDropbox)) PropiedadCambiada(nameof(SePuedeSincronizarNube));
            HayCambios = true;
        }

        private void ArticulosConvenio_ItemPropertyChanged(object sender, ItemChangedEventArgs<ArticuloConvenio> e) {
            HayCambios = true;
        }


        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public bool HayCambios { get; set; }


        private Visibility _visibilidadpanelgenerales = Visibility.Visible;
        public Visibility VisibilidadPanelGenerales {
            get { return _visibilidadpanelgenerales; }
            set {
                if (_visibilidadpanelgenerales != value) {
                    _visibilidadpanelgenerales = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _visibilidadpanelconvenio = Visibility.Collapsed;
        public Visibility VisibilidadPanelConvenio {
            get { return _visibilidadpanelconvenio; }
            set {
                if (_visibilidadpanelconvenio != value) {
                    _visibilidadpanelconvenio = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _visibilidadpanelporcentro = Visibility.Collapsed;
        public Visibility VisibilidadPanelPorCentro {
            get { return _visibilidadpanelporcentro; }
            set {
                if (_visibilidadpanelporcentro != value) {
                    _visibilidadpanelporcentro = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _porcentajeincremento;
        public decimal PorcentajeIncremento {
            get { return _porcentajeincremento; }
            set {
                if (_porcentajeincremento != value) {
                    _porcentajeincremento = value;
                    PropiedadCambiada();
                }
            }
        }



        // PLUSES
        // ----------------------------------------------------------------------------------------------------

        private NotifyCollection<Pluses> _listapluses = new NotifyCollection<Pluses>();
        public NotifyCollection<Pluses> ListaPluses {
            get { return _listapluses; }
            set {
                if (SetValue(ref _listapluses, value)) {
                    _listapluses.CollectionChanged += ListaPluses_CollectionChanged;
                    _listapluses.ItemPropertyChanged += ListaPluses_ItemPropertyChanged;
                }
            }
        }

        private int _añopluses;
        public int AñoPluses {
            get { return _añopluses; }
            set {
                if (_añopluses != value) {
                    if (!ListaPluses.Any(p => p.Año == value)) {
                        ListaPluses.Add(new Pluses(value, ListaPluses.FirstOrDefault(p => p.Año == value - 1)));
                        HayCambios = true;
                    }
                    _añopluses = value;
                    PropiedadCambiada();
                    PlusesActuales = ListaPluses.FirstOrDefault(p => p.Año == _añopluses);
                }
            }
        }



        private Pluses _plusesactuales;
        public Pluses PlusesActuales {
            get { return _plusesactuales; }
            set { SetValue(ref _plusesactuales, value); }
        }



        public bool SePuedeSincronizarNube {
            get => !string.IsNullOrEmpty(App.Global.Configuracion.CarpetaDropbox);
        }


        // ARTÍCULOS CONVENIO
        // ----------------------------------------------------------------------------------------------------


        private NotifyCollection<ArticuloConvenio> articulosConvenio;
        public NotifyCollection<ArticuloConvenio> ArticulosConvenio {
            get => articulosConvenio;
            set {
                if (SetValue(ref articulosConvenio, value)) {
                    articulosConvenio.ItemPropertyChanged += ArticulosConvenio_ItemPropertyChanged;
                }
            }
        }

        private ArticuloConvenio articuloSelecionado;
        public ArticuloConvenio ArticuloSeleccionado {
            get => articuloSelecionado;
            set {
                if (SetValue(ref articuloSelecionado, value)) {
                    PropiedadCambiada(nameof(FuncionRelacionadaSeleccionada));
                }
            }
        }


        public FuncionRelacionada FuncionRelacionadaSeleccionada {
            get => FuncionesRelacionadas.FirstOrDefault(f => f.Codigo == (ArticuloSeleccionado?.CodigoFuncionRelacionada ?? 0));
            set {
                if (ArticuloSeleccionado != null) {
                    ArticuloSeleccionado.CodigoFuncionRelacionada = value.Codigo;
                    PropiedadCambiada();
                }
            }
        }



        private List<FuncionRelacionada> funcionesRelacionadas;
        public List<FuncionRelacionada> FuncionesRelacionadas {
            get {
                if (funcionesRelacionadas == null) {
                    funcionesRelacionadas = new List<FuncionRelacionada> {
                        new FuncionRelacionada(0, "ninguna", "Ninguna"),
                        new FuncionRelacionada(1, "jornadaMedia", "Jornada Media"),
                        new FuncionRelacionada(2, "nocturnas", "Horas Nocturnas"),
                        new FuncionRelacionada(3, "dietaDesayuno", "Dieta Desayuno"),
                        new FuncionRelacionada(4, "dietaComida", "Dieta Comida"),
                        new FuncionRelacionada(5, "dietaCena", "Dieta Cena"),
                        new FuncionRelacionada(6, "horasCobradas", "Horas Cobradas"),
                        new FuncionRelacionada(7, "jornada", "Jornada"),
                        new FuncionRelacionada(8, "descansos", "Descansos"),
                        new FuncionRelacionada(9, "vacaciones", "Vacaciones"),
                        new FuncionRelacionada(10, "findes", "Fines de semana"),
                        new FuncionRelacionada(11, "limpieza", "Plus de limpieza"),
                        new FuncionRelacionada(12, "plusViaje", "Plus de viaje"),
                        new FuncionRelacionada(13, "importeDietas", "Importe Dietas"),
                        new FuncionRelacionada(14, "plusSabados", "Plus Sábados"),
                        new FuncionRelacionada(15, "plusFestivos", "Plus Festivos"),
                        new FuncionRelacionada(16, "plusNocturnidad", "Plus Nocturnidad"),
                        new FuncionRelacionada(17, "plusMenorDescanso", "Plus de menor descanso"),
                        new FuncionRelacionada(18, "plusPaqueteria", "Plus Paqueteria"),
                        new FuncionRelacionada(19, "plusNavidad", "Plus de Navidad"),
                    };
                }
                return funcionesRelacionadas;
            }
        }

        public class FuncionRelacionada {

            public FuncionRelacionada() { }

            public FuncionRelacionada(int codigo, string tag, string texto) {
                this.Codigo = codigo;
                this.Tag = tag;
                this.Texto = texto;
            }

            public int Codigo { get; set; }

            public string Tag { get; set; }

            public string Texto { get; set; }

        }


        #endregion





    }
}
