#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Linq;
using System.Windows;
using Orion.Config;
using Orion.DataModels;
using Orion.Models;
using Orion.Servicios;

namespace Orion.ViewModels {

	public partial class OpcionesViewModel : NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajes mensajes;
		//private List<Festivo> _listaborrados = new List<Festivo>();
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public OpcionesViewModel(IMensajes servicioMensajes) {
			mensajes = servicioMensajes;
			//_listafestivos.CollectionChanged += ListaFestivos_CollectionChanged;
			//AñoFestivos = DateTime.Now.Year;
			CargarDatos();
			AñoPluses = DateTime.Now.Year;

		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void CargarDatos() {
			//if (App.Global.CadenaConexion == null) {
			//	_listafestivos.Clear();
			//	return;
			//}
			//ListaFestivos = BdFestivos.GetFestivosPorAño(AñoFestivos);
			if (App.Global.CadenaConexion == null) return;
			ListaPluses = BdPluses.GetPluses();
			ListaPluses.ItemPropertyChanged += ListaPluses_ItemPropertyChanged;
		}


		public void GuardarDatos() {
			HayCambios = false;
			//if (ListaFestivos != null && ListaFestivos.Count > 0) {
			//	BdFestivos.GuardarFestivos(ListaFestivos);
			//}
			//if (_listaborrados.Count > 0) {
			//	BdFestivos.BorrarFestivos(_listaborrados);
			//	_listaborrados.Clear();
			//}
			if (ListaPluses != null && ListaPluses.Any() && App.Global.CadenaConexion != null) {
				BdPluses.GuardarPluses(ListaPluses.Where(item => item.Nuevo || item.Modificado));
			}
		}


		public void GuardarTodo() {
			GuardarDatos();
			HayCambios = false;
		}


		public void Reiniciar() {
			//AñoFestivos = DateTime.Now.Year;
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
		//private void ListaFestivos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

		//	if (e.NewItems != null) {
		//		foreach (Festivo festivo in e.NewItems) {
		//			festivo.Año = AñoFestivos;
		//			festivo.Nuevo = true;
		//			festivo.ObjetoCambiado += ObjetoCambiadoEventHandler;
		//			HayCambios = true;
		//		}
		//	}

		//	if (e.OldItems != null) {
		//		foreach (Festivo festivo in e.OldItems) {
		//			festivo.ObjetoCambiado -= ObjetoCambiadoEventHandler;
		//		}
		//	}

		//	PropiedadCambiada(nameof(ListaFestivos));
		//}


		private void ListaPluses_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e) {
			HayCambios = true;
		}


		private void ObjetoCambiadoEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
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

		private FullObservableCollection<Pluses> _listapluses = new FullObservableCollection<Pluses>();
		public FullObservableCollection<Pluses> ListaPluses {
			get { return _listapluses; }
			set { SetValue(ref _listapluses, value); }
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



		#endregion





	}
}
