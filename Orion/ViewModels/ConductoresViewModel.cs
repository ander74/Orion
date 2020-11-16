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
    using Config;
    using Models;
    using Servicios;

    public partial class ConductoresViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        List<Conductor> _listaborrados = new List<Conductor>();
        List<RegulacionConductor> _regulacionesborradas = new List<RegulacionConductor>();
        private static IMensajes mensajes;

        #endregion


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public ConductoresViewModel(IMensajes servicioMensajes) {
            _listaconductores = new NotifyCollection<Conductor>();
            mensajes = servicioMensajes;
            CargarConductores();
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void CargarConductores() {
            if (App.Global.CadenaConexion == null) {
                ListaConductores.Clear();
                return;
            }
            ListaConductores = new NotifyCollection<Conductor>(App.Global.Repository.GetConductores());
            PropiedadCambiada(nameof(Detalle));
        }

        public void GuardarConductores() {
            try {
                App.Global.CalendariosVM.AñadirConductoresDesconocidos();
                HayCambios = false;
                if (_listaborrados.Count > 0) {
                    App.Global.Repository.BorrarConductores(_listaborrados);
                    _listaborrados.Clear();
                }
                if (_regulacionesborradas.Count > 0) {
                    App.Global.Repository.BorrarRegulaciones(_regulacionesborradas);
                    _regulacionesborradas.Clear();
                }
                if (ListaConductores != null && ListaConductores.Count > 0) {
                    App.Global.Repository.GuardarConductores(ListaConductores.Where(c => c.Nuevo || c.Modificado));
                }
                // Si hay conductores con la matrícula cero, avisamos.
                if (ListaConductores.Count(item => item.Matricula == 0) > 0) {
                    mensajes.VerMensaje("Hay conductores no válidos que podrían no haberse guardado.", "ATENCIÓN");
                }
            } catch (Exception ex) {
                mensajes.VerError("ConductoresViewModel.GuardarConductores", ex);
                HayCambios = true;
            }
        }


        public void GuardarTodo() {
            GuardarConductores();
            Reiniciar();
        }


        public void Reiniciar() {
            ConductorSeleccionado = null;
            CargarConductores();
            HayCambios = false;
        }


        public Conductor GetConductor(int matricula) {
            return ListaConductores.FirstOrDefault(c => c.Matricula == matricula);
        }


        public bool ExisteConductor(int matricula) {
            return ListaConductores.Any(c => c.Matricula == matricula);
        }


        public void CrearConductorDesconocido(int matricula) {
            if (ExisteConductor(matricula)) return;
            Conductor desconocido = new Conductor { Matricula = matricula, Nombre = "Desconocido", Notas = "Conductor insertado automáticamente por el sistema." };
            ListaConductores.Add(desconocido);
        }


        public void InsertarRegulacion(RegulacionConductor regulacion) {
            Conductor conductor = ListaConductores.First(c => c.Id == regulacion.IdConductor);
            conductor.ListaRegulaciones.Add(regulacion);
        }


        public bool IsIndefinidoById(int idConductor) {
            return ListaConductores.FirstOrDefault(c => c.Id == idConductor)?.Indefinido ?? false;
        }


        public bool IsIndefinido(int matricula) {
            return ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Indefinido ?? false;
        }



        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaConductores_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (Conductor conductor in e.NewItems) {
                    conductor.Nuevo = true;
                }
                HayCambios = true;
            }
        }

        private void ListaConductores_ItemPropertyChanged(object sender, ItemChangedEventArgs<Conductor> e) {
            HayCambios = true;
            PropiedadCambiada(nameof(Detalle));
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        NotifyCollection<Conductor> _listaconductores;
        public NotifyCollection<Conductor> ListaConductores {
            get { return _listaconductores; }
            set {
                if (_listaconductores != value) {
                    _listaconductores = value;
                    _listaconductores.CollectionChanged += ListaConductores_CollectionChanged;
                    _listaconductores.ItemPropertyChanged += ListaConductores_ItemPropertyChanged;
                    VistaConductores = new ListCollectionView(_listaconductores);//ADDED
                    PropiedadCambiada();
                }
            }
        }


        private ListCollectionView _vistaconductores;
        public ListCollectionView VistaConductores {
            get { return _vistaconductores; }
            set {
                if (_vistaconductores != value) {
                    _vistaconductores = value;
                    PropiedadCambiada();
                }
            }
        }


        private Conductor _conductorseleccionado;
        public Conductor ConductorSeleccionado {
            get { return _conductorseleccionado; }
            set {
                if (_conductorseleccionado != value) {
                    _conductorseleccionado = value;
                    _regulacionesborradas.Clear();
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayConductor));
                    PropiedadCambiada(nameof(ConductorFijo));
                    PropiedadCambiada(nameof(ConductorEventual));
                }
            }
        }


        private RegulacionConductor _regulacionseleccionada;
        public RegulacionConductor RegulacionSeleccionada {
            get { return _regulacionseleccionada; }
            set {
                if (_regulacionseleccionada != value) {
                    _regulacionseleccionada = value;
                    PropiedadCambiada();
                }
            }
        }


        public bool HayConductor {
            get {
                if (ConductorSeleccionado == null) return false;
                if (ConductorSeleccionado.Nuevo) return false;
                return true;
            }
        }


        public bool ConductorFijo {
            get {
                if (ConductorSeleccionado != null && ConductorSeleccionado.Indefinido) return true;
                return false;
            }
        }


        public bool ConductorEventual {
            get {
                return !ConductorFijo;
            }
        }


        private bool _haycambios = false;
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


        public String Detalle {
            get {
                int condFijos = ListaConductores.Count(c => c.Categoria == "C" && c.Indefinido);
                int condEventuales = ListaConductores.Count(c => c.Categoria == "C" && !c.Indefinido);
                int taquFijos = ListaConductores.Count(c => c.Categoria == "T" && c.Indefinido);
                int taquEventuales = ListaConductores.Count(c => c.Categoria == "T" && !c.Indefinido);
                string texto = $"Conducción: {condFijos + condEventuales} => Fijos: {condFijos} - Eventuales: {condEventuales}  /  " +
                    $"Taquilla: {taquFijos + taquEventuales} => Fijos: {taquFijos} - Eventuales: {taquEventuales}";
                return texto;

            }
        }


        #endregion


    } //Fin de clase.
}
