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
using System.Windows.Controls;
using Orion.Views;
using Orion.Properties;

namespace Orion.ViewModels {

	public partial class OpcionesViewModel: NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajeProvider _mensajeProvider;
		private List<Festivo> _listaborrados = new List<Festivo>();
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public OpcionesViewModel(IMensajeProvider mensajeProvider) {
			_mensajeProvider = mensajeProvider;
			_listafestivos.CollectionChanged += ListaFestivos_CollectionChanged;
			AñoFestivos = DateTime.Now.Year;
			CargarFestivos();
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void CargarFestivos() {
			if (App.Global.CadenaConexion == null) {
				_listafestivos.Clear();
				return;
			}
			ListaFestivos = BdFestivos.GetFestivosPorAño(AñoFestivos);
		}


		public void GuardarFestivos() {
			HayCambios = false;
			if (ListaFestivos != null && ListaFestivos.Count > 0) {
				BdFestivos.GuardarFestivos(ListaFestivos);
			}
			if (_listaborrados.Count > 0) {
				BdFestivos.BorrarFestivos(_listaborrados);
				_listaborrados.Clear();
			}
		}


		public void GuardarTodo() {
			GuardarFestivos();
			Settings.Default.Save();
			Convenio.Default.Save();
			PorCentro.Default.Save();
			HayCambios = false;
		}


		public void Reiniciar() {
			AñoFestivos = DateTime.Now.Year;
			CargarFestivos();
			HayCambios = false;
		}


		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================
		private void ListaFestivos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

			if (e.NewItems != null) {
				foreach (Festivo festivo in e.NewItems) {
					festivo.Año = AñoFestivos;
					festivo.Nuevo = true;
					festivo.ObjetoCambiado += ObjetoCambiadoEventHandler;
					HayCambios = true;
				}
			}

			if (e.OldItems != null) {
				foreach (Festivo festivo in e.OldItems) {
					festivo.ObjetoCambiado -= ObjetoCambiadoEventHandler;
				}
			}

			PropiedadCambiada(nameof(ListaFestivos));
		}

		private void ObjetoCambiadoEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			HayCambios = true;
		}

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private ObservableCollection<Festivo> _listafestivos = new ObservableCollection<Festivo>();
		public ObservableCollection<Festivo> ListaFestivos {
			get { return _listafestivos; }
			set {
				if (_listafestivos != value) {
					_listafestivos = value;
					_listafestivos.CollectionChanged += ListaFestivos_CollectionChanged;
					foreach(Festivo f in _listafestivos) {
						f.ObjetoCambiado += ObjetoCambiadoEventHandler;
					}
					PropiedadCambiada();
				}
			}
		}


		private int _añofestivos;
		public int AñoFestivos {
			get { return _añofestivos; }
			set {
				if (_añofestivos != value) {
					_añofestivos = value;
					PropiedadCambiada();
				}
			}
		}

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


		#endregion





	}
}
