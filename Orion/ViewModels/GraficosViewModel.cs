#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using Orion.DataModels;
	using Orion.Models;
	using Orion.Servicios;

	public partial class GraficosViewModel: NotifyBase {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private ObservableCollection<Grafico> _listaanteriores = new ObservableCollection<Grafico>();
		private ObservableCollection<GrupoGraficos> _listagrupos = new ObservableCollection<GrupoGraficos>();
		private List<Grafico> _listaborrados = new List<Grafico>();
		private List<ValoracionGrafico> _valoracionesborradas = new List<ValoracionGrafico>();
		private GrupoGraficos _gruposeleccionado;
		private Grafico _graficoseleccionado;
		private ValoracionGrafico _valoracionseleccionada;
		private bool _panelgruposfijo;
		private bool _panelvaloracionesfijo;
		private Visibility _panelgruposvisibilidad = Visibility.Collapsed;
		private Visibility _panelvaloracionesvisibilidad = Visibility.Collapsed;
		private Visibility _visibilidadpanelvalidezgrupo = Visibility.Collapsed;
		private bool _haycambios = false;

		// Servicios
		private IMensajes Mensajes;
		private InformesServicio Informes;
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public GraficosViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes) {
			// Asignamos los servicios
			Mensajes = servicioMensajes;
			Informes = servicioInformes;
			// Añadimos los eventos a las listas.
			_listagraficos.CollectionChanged += ListaGraficos_CollectionChanged;
			_listagrupos.CollectionChanged += ListaGrupos_CollectionChanged;
			// Cargamos los grupos de gráficos.
			CargarGrupos();
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
			ListaGraficos = BdGraficos.getGraficos(GrupoSeleccionado.Id);
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


		public void GuardarGrupos() {
			try {
				HayCambios = false;
				if (ListaGrupos != null && ListaGrupos.Count > 0) {
					BdGruposGraficos.GuardarGrupos(ListaGrupos.Where(gg => gg.Nuevo || gg.Modificado));
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosViewModel.GuardarGrupos", ex);
				HayCambios = true;
			} finally {
			}
		}


		public void GuardarTodo() {
			GuardarGrupos();
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
					if (GrupoSeleccionado != null) grafico.IdGrupo = GrupoSeleccionado.Id;
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
					HayCambios = true;
				}
			}
			PropiedadCambiada(nameof(ListaGrupos));
		}


		private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
			HayCambios = true;
			PropiedadCambiada(nameof(Detalle));
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


		public Visibility VisibilidadPanelFiltros {
			get {
				if (TextoFiltros == "Ninguno") return Visibility.Collapsed;
				return Visibility.Visible;
			}
		}


		private bool _btfiltrarabierto;
		public bool BtFiltrarAbierto {
			get { return _btfiltrarabierto; }
			set {
				if (_btfiltrarabierto != value) {
					_btfiltrarabierto = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _btcompararabierto;
		public bool BtCompararAbierto {
			get { return _btcompararabierto; }
			set {
				if (_btcompararabierto != value) {
					_btcompararabierto = value;
					PropiedadCambiada();
				}
			}
		}


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


		private string _textofiltros = "Ninguno";
		public string TextoFiltros {
			get { return _textofiltros; }
			set {
				if (_textofiltros != value) {
					_textofiltros = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(VisibilidadPanelFiltros));
				}
			}
		}


		public GrupoGraficos GrupoSeleccionado {
			get { return _gruposeleccionado; }
			set {
				if (_gruposeleccionado != value) {
					GuardarGraficos();
					_gruposeleccionado = value;
					CargarGraficos();
					GraficoSeleccionado = null;
					if (!PanelGruposFijo) PanelGruposVisibilidad = Visibility.Collapsed;
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


		public bool PanelGruposFijo {
			get { return _panelgruposfijo; }
			set {
				if (_panelgruposfijo != value) {
					_panelgruposfijo = value;
					PropiedadCambiada();
				}
			}
		}


		public bool PanelValoracionesFijo {
			get { return _panelvaloracionesfijo; }
			set {
				if (_panelvaloracionesfijo != value) {
					_panelvaloracionesfijo = value;
					PropiedadCambiada();
				}
			}
		}


		public Visibility PanelGruposVisibilidad {
			get { return _panelgruposvisibilidad; }
			set {
				if (_panelgruposvisibilidad != value) {
					_panelgruposvisibilidad = value;
					PropiedadCambiada();
				}
			}
		}


		public Visibility PanelValoracionesVisibilidad {
			get { return _panelvaloracionesvisibilidad; }
			set {
				if (_panelvaloracionesvisibilidad != value) {
					_panelvaloracionesvisibilidad = value;
					PropiedadCambiada();
				}
			}
		}


		public Visibility VisibilidadPanelValidezGrupo {
			get { return _visibilidadpanelvalidezgrupo; }
			set {
				if (_visibilidadpanelvalidezgrupo != value) {
					_visibilidadpanelvalidezgrupo = value;
					PropiedadCambiada();
				}
			}
		}


		private DateTime _fechaestadisticas = new DateTime(DateTime.Today.Year, 1,1);
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
				if (VistaGraficos != null) {
					foreach (object obj in VistaGraficos) {
						Grafico g = obj as Grafico;
						if (g == null) continue;
						trab += g.Trabajadas.TotalMinutes;
						acum += g.Acumuladas.TotalMinutes;
						noc += g.Nocturnas.TotalMinutes;
					}
					num = VistaGraficos.Count - 1;
				}
				string texto = "Graficos: " + num.ToString();
				texto += "  |  Trabajadas=" + (trab / 60).ToString("0.00");
				texto += "  |  Acumuladas=" + (acum / 60).ToString("0.00");
				texto += "  |  Nocturnas=" + (noc / 60).ToString("0.00");
				return texto;
			}
		}


		private int _columnaactual = -1;
		public int ColumnaActual {
			get { return _columnaactual; }
			set {
				if (_columnaactual != value) {
					_columnaactual = value;
					PropiedadCambiada();
				}
			}
		}


		private int _filaactual = -1;
		public int FilaActual {
			get { return _filaactual; }
			set {
				if (_filaactual != value) {
					_filaactual = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion


	} //Fin de la clase
}
