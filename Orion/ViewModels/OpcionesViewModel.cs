#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
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
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public OpcionesViewModel(IMensajes servicioMensajes) {
			mensajes = servicioMensajes;
			CargarDatos();
			AñoPluses = DateTime.Now.Year;

		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void CargarDatos() {
			if (App.Global.CadenaConexion == null) {
				ListaPluses = new NotifyCollection<Pluses>();
				return;
			}
			ListaPluses = new NotifyCollection<Pluses>(App.Global.Repository.GetPluses());
		}


		public void GuardarDatos() {
			App.Global.Repository.GuardarPluses(ListaPluses.Where(item => item.Nuevo || item.Modificado));
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



		#endregion





	}
}
