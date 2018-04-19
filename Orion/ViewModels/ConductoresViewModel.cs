#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orion.Models;
using Orion.DataModels;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Orion.Views;
using System.Windows.Data;
using System.ComponentModel;
using Orion.Servicios;
using System.Windows.Controls;

namespace Orion.ViewModels {


	public partial class ConductoresViewModel: NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		ObservableCollection<Conductor> _listaconductores = new ObservableCollection<Conductor>();
		List<Conductor> _listaborrados = new List<Conductor>();
		List<RegulacionConductor> _regulacionesborradas = new List<RegulacionConductor>();
		Conductor _conductorseleccionado;
		RegulacionConductor _regulacionseleccionada;
		private bool _panelregulacionesfijo;
		private Visibility _panelregulacionesvisibilidad = Visibility.Collapsed;
		private bool _haycambios = false;
		private IMensajes mensajes;
		//private GenericOleDb DatosOleDb;
		#endregion


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================
		public ConductoresViewModel(IMensajes servicioMensajes) {
			mensajes = servicioMensajes;
			//DatosOleDb = new GenericOleDb(App.Global.CadenaConexion);
			//DatosOleDb.ErrorProducido += DatosOleDb_ErrorProducido;
			_listaconductores.CollectionChanged += ListaConductores_CollectionChanged;
			CargarConductores();
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void CargarConductores() {
			if (App.Global.CadenaConexion == null) {
				_listaconductores.Clear();
				return;
			}
			ListaConductores = BdConductores.GetConductores();
			//ListaConductores = DatosOleDb.GetModelos<Conductor>(ProcSQL.GetConductores);
			foreach (Conductor c in ListaConductores) {
				c.PropertyChanged += ConductorPropertyChangedHandler;
			}
			PropiedadCambiada(nameof(Detalle));
		}


		public void GuardarConductores() {
			try {
				HayCambios = false;
				if (_listaborrados.Count > 0) {
					BdConductores.BorrarConductores(_listaborrados);
					_listaborrados.Clear();
				}
				if (_regulacionesborradas.Count > 0) {
					BdRegulacionConductor.BorrarRegulaciones(_regulacionesborradas);
					_regulacionesborradas.Clear();
				}
				if (ListaConductores != null && ListaConductores.Count > 0) {
					BdConductores.GuardarConductores(ListaConductores);
				}
				// Si hay conductores con el id cero, avisamos.
				if (ListaConductores.Count(item => item.Id == 0) > 0) {
					mensajes.VerMensaje("Hay conductores no válidos que no han sido guardados.", "ATENCIÓN");
				}
			} catch (Exception ex) {
				mensajes.VerError("ConductoresViewModel.GuardarConductores", ex);
				HayCambios = true;
			}
		}


		public void GuardarTodo() {
			GuardarConductores();
			HayCambios = false;
		}


		public void Reiniciar() {
			ConductorSeleccionado = null;
			CargarConductores();
			HayCambios = false;
		}


		public Conductor GetConductor(int idconductor) {

			Conductor resultado = ListaConductores.First(c => c.Id == idconductor);
			return resultado;

		}


		public bool ExisteConductor(int idConductor) {
			return ListaConductores.Any(c => c.Id == idConductor);
		}


		public void CrearConductorDesconocido(int idConductor) {
			if (ExisteConductor(idConductor)) return;

		}

		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================
		private void ListaConductores_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			
			if (e.NewItems != null) {
				foreach (Conductor conductor in e.NewItems) {
					conductor.PropertyChanged += (s, ev) => HayCambios = true;
					conductor.Nuevo = true;
					HayCambios = true;
				}
			}

			if (e.OldItems != null) {
				foreach (Conductor conductor in e.OldItems) {
					conductor.PropertyChanged -= ConductorPropertyChangedHandler;
				}
				PropiedadCambiada(nameof(ListaConductores));
			}
		}

		private void ConductorPropertyChangedHandler(object sender, PropertyChangedEventArgs e) {
			HayCambios = true;
		}


		//private void DatosOleDb_ErrorProducido(object sender, ErrorProducidoEventArgs e) {
		//	mensajes.VerError(e.Localizacion, e.Excepcion);
		//}

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================
		public ObservableCollection<Conductor> ListaConductores {
			get { return _listaconductores; }
			set {
				if (_listaconductores != value) {
					_listaconductores = value;
					_listaconductores.CollectionChanged += ListaConductores_CollectionChanged;
					VistaConductores = new ListCollectionView(_listaconductores);//ADDED
					PropiedadCambiada();
				}
			}
		}



		//ADDED: Probar esto.
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


		public bool PanelRegulacionesFijo {
			get { return _panelregulacionesfijo; }
			set {
				if (_panelregulacionesfijo != value) {
					_panelregulacionesfijo = value;
					PropiedadCambiada();
				}
			}
		}


		public Visibility PanelRegulacionesVisibilidad {
			get { return _panelregulacionesvisibilidad; }
			set {
				if (_panelregulacionesvisibilidad != value) {
					_panelregulacionesvisibilidad = value;
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
					PropiedadCambiada(nameof(Detalle));
				}
			}
		}


		public String Detalle {
			get {
				int eventuales = ListaConductores.Count(c => c.Indefinido == false);
				string texto = "Conductores: " + ListaConductores.Count.ToString();
				texto += "  |  Eventuales: " + eventuales.ToString();
				return texto;

			}
		}


		// MODO DE SELECCIÓN DEL GRID

		private bool _btaccionesabierto;
		public bool BtAccionesAbierto {
			get { return _btaccionesabierto; }
			set {
				if (_btaccionesabierto != value) {
					_btaccionesabierto = value;
					PropiedadCambiada();
				}
			}
		}


		private Visibility _visibilidadbotonseleccionfila = Visibility.Collapsed;
		public Visibility VisibilidadBotonSeleccionFila {
			get { return _visibilidadbotonseleccionfila; }
			set {
				if (_visibilidadbotonseleccionfila != value) {
					_visibilidadbotonseleccionfila = value;
					PropiedadCambiada();
				}
			}
		}


		private Visibility _visibilidadbotonseleccioncelda = Visibility.Visible;
		public Visibility VisibilidadBotonSeleccionCelda {
			get { return _visibilidadbotonseleccioncelda; }
			set {
				if (_visibilidadbotonseleccioncelda != value) {
					_visibilidadbotonseleccioncelda = value;
					PropiedadCambiada();
				}
			}
		}


		private DataGridSelectionUnit _modoseleccion = DataGridSelectionUnit.FullRow;
		public DataGridSelectionUnit ModoSeleccion {
			get { return _modoseleccion; }
			set {
				if (_modoseleccion != value) {
					_modoseleccion = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion











	} //Fin de clase.
}
