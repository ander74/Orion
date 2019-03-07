#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Orion.Config;
using Orion.DataModels;
using Orion.Models;
using Orion.PrintModel;
using Orion.Properties;
using Orion.Servicios;
using Orion.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Orion.ViewModels {


	public partial class GraficosViewModel {


		#region CAMPOS PRIVADOS
		//====================================================================================================
		private ICommand _cmdmostrarpanelgrupos;
		private ICommand _cmdmostrarpanelvaloraciones;
		private ICommand _cmdañadirgrafico;
		private ICommand _cmdborrargrafico;
		private ICommand _cmddeshacerborrar;
		private ICommand _cmdcalculartabla;
		private ICommand _cmdborrarceldas;
		private ICommand _cmdborrarceldasvaloracion;
		private ICommand _cmdfijarpanelgrupos;
		private ICommand _cmdfijarpanelvaloraciones;
		private ICommand _cmdborrargrupo;
		private ICommand _cmdquitarfiltro;
		private ICommand _cmdpegargraficos;
		private ICommand _cmdborrarvaloracion;
		private ICommand _cmddeshacerborrarvaloraciones;
		private ICommand _cmdcalcularvaloraciones;
		private ICommand _cmdnuevogrupo;
		private ICommand _cmdcomparargrupo;
		private ICommand _cmdañadirvaloracion;
		private ICommand _cmdeditarfilavaloracion;
		private ICommand _modificarfechagrupo;
		private ICommand _seleccionarfechagrupo;
		#endregion
		//====================================================================================================


		#region MOSTRAR PANEL GRUPOS
		//Comando
		public ICommand cmdMostrarPanelGrupos {
			get {
				if (_cmdmostrarpanelgrupos == null) _cmdmostrarpanelgrupos = new RelayCommand(p => MostrarPanelGrupos(p));
				return _cmdmostrarpanelgrupos;
			}
		}

		// Ejecución del comando
		private void MostrarPanelGrupos(object parametro) {
			Grid panel = (Grid)parametro;
			if (panel == null) return;
			if (PanelGruposVisibilidad == Visibility.Visible) {
				PanelGruposVisibilidad = Visibility.Collapsed;
				PanelGruposFijo = false;
				Grid.SetColumn(panel, 1);
			} else {
				PanelGruposVisibilidad = Visibility.Visible;
			}
		}
		#endregion


		#region MOSTRAR PANEL VALORACIONES
		//Comando
		public ICommand cmdMostrarPanelValoraciones {
			get {
				if (_cmdmostrarpanelvaloraciones == null) _cmdmostrarpanelvaloraciones = new RelayCommand(p => MostrarPanelValoraciones(p));
				return _cmdmostrarpanelvaloraciones;
			}
		}

		// Ejecución del comando
		private void MostrarPanelValoraciones(object parametro) {
			Grid panel = (Grid)parametro;
			if (panel == null) return;
			if (panel.IsVisible) {
				PanelValoracionesVisibilidad = Visibility.Collapsed;
				PanelValoracionesFijo = false;
				Grid.SetColumn(panel, 1);
			} else {
				PanelValoracionesVisibilidad = Visibility.Visible;
			}
		}
		#endregion


		#region AÑADIR GRAFICO
		// Comando
		public ICommand cmdAñadirGrafico {
			get {
				if (_cmdañadirgrafico == null) _cmdañadirgrafico = new RelayCommand(p => AñadirGrafico(), p => PuedeAñadirGrafico());
				return _cmdañadirgrafico;
			}
		}

		// Se puede ejecutar
		private bool PuedeAñadirGrafico() {
			return HayGrupo;
		}

		// Ejecución del comando
		private void AñadirGrafico() {

			// Creamos la ventana
			VentanaAñadirGrafico ventana;

			// Creamos las variables auxiliares
			bool incrementar = true;
			bool deducir = true;
			int numero = 0;
			if (ListaGraficos.Count > 0) numero = ListaGraficos.Max(g => g.Numero);

			// Iniciamos el bucle para mostrar la ventana.
			do {
				// Instanciamos la ventana y la vinculamos al ViewModel correspondiente.
				ventana = new VentanaAñadirGrafico();
				ventana.DataContext = new AñadirGraficoViewModel(Mensajes);
				((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado = incrementar;
				((AñadirGraficoViewModel)ventana.DataContext).DeducirTurnoMarcado = deducir;
				((AñadirGraficoViewModel)ventana.DataContext).Numero = ((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado ? numero + 1 : numero;
				// Si al mostrar la ventana, pulsamos aceptar...
				if (ventana.ShowDialog() == true) {
					// Creamos el gráfico y lo guardamos en la lista.
					Grafico grafico = ((AñadirGraficoViewModel)ventana.DataContext).GraficoActual;
					grafico.IdGrupo = GrupoSeleccionado.Id;
					ListaGraficos.Add(grafico);
					// Establecemos las variables.
					incrementar = ((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado;
					deducir = ((AñadirGraficoViewModel)ventana.DataContext).DeducirTurnoMarcado;
					numero = ((AñadirGraficoViewModel)ventana.DataContext).Numero;
				} else {
					return;
				}
			} while (true);

		}
		#endregion


		#region BORRAR GRAFICO
		// Comando
		public ICommand cmdBorrarGrafico {
			get {
				if (_cmdborrargrafico == null) _cmdborrargrafico = new RelayCommand(p => BorrarGrafico(p), p => PuedeBorrarGrafico(p));
				return _cmdborrargrafico;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarGrafico(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return false;
			if (tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Número" || tabla.CurrentCell.Item.GetType().Name != "Grafico") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarGrafico(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.SelectedCells == null) return;
			List<Grafico> lista = new List<Grafico>();
			foreach (DataGridCellInfo celda in tabla.SelectedCells) {
				if (celda.Column.Header.ToString() == "Número" && celda.Item.GetType().Name == "Grafico") {
					lista.Add((Grafico)celda.Item);
				}
			}
			foreach(Grafico g in lista) {
				_listaborrados.Add(g);
				_listagraficos.Remove(g);
				HayCambios = true;
				PropiedadCambiada(nameof(Detalle));
			}
			lista.Clear();
			
		}
		#endregion


		#region DESHACER BORRAR
		public ICommand cmdDeshacerBorrar {
			get {
				if (_cmddeshacerborrar == null) _cmddeshacerborrar = new RelayCommand(p => DeshacerBorrar(), p => PuedeDeshacerBorrar());
				return _cmddeshacerborrar;
			}
		}

		private bool PuedeDeshacerBorrar() {
			return _listaborrados.Count > 0;
		}

		private void DeshacerBorrar() {
			if (_listaborrados == null) return;
			foreach (Grafico grafico in _listaborrados) {
				if (grafico.Nuevo) {
					_listagraficos.Add(grafico);
				} else {
					_listagraficos.Add(grafico);
					grafico.Nuevo = false;
				}
				HayCambios = true;
				PropiedadCambiada(nameof(Detalle));
			}
			_listaborrados.Clear();
			//Propiedades.DatosModificados = true;
		}
		#endregion


		#region CALCULAR TABLA
		public ICommand cmdCalcularTabla {
			get {
				if (_cmdcalculartabla == null) _cmdcalculartabla = new RelayCommand(p => CalcularTabla(), p => PuedeCalcularTabla());
				return _cmdcalculartabla;
			}
		}

		private bool PuedeCalcularTabla() {
			return _listagraficos.Count > 0;
		}

		private void CalcularTabla() {
			foreach (Grafico grafico in _listagraficos) {
				grafico.Recalcular();
				HayCambios = true;
			}
			BtAccionesAbierto = false;
		}
		#endregion


		#region BORRAR CELDAS
		public ICommand cmdBorrarCeldas {
			get {
				if (_cmdborrarceldas == null) _cmdborrarceldas = new RelayCommand(p => BorrarCeldas(p));
				return _cmdborrarceldas;
			}
		}

		private void BorrarCeldas(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return;
			if (tabla.CurrentCell == null) return;
			DataGridCellInfo celda = tabla.CurrentCell;

			Grafico grafico = tabla.SelectedCells[0].Item as Grafico;
			if (grafico == null) return;
			string encabezado;
			if (celda.Column.Header is TextBlock) {
				encabezado = ((TextBlock)celda.Column.Header).Text;
			} else {
				encabezado = celda.Column.Header.ToString();
			}
			grafico.BorrarValorPorHeader(encabezado);
			HayCambios = true;
			PropiedadCambiada(nameof(Detalle));
		}
		#endregion


		#region BORRAR CELDAS VALORACION
		public ICommand cmdBorrarCeldasValoracion {
			get {
				if (_cmdborrarceldasvaloracion == null) _cmdborrarceldasvaloracion = new RelayCommand(p => BorrarCeldasValoracion(p));
				return _cmdborrarceldasvaloracion;
			}
		}

		private void BorrarCeldasValoracion(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			foreach (DataGridCellInfo celda in tabla.SelectedCells) {
				((ValoracionGrafico)celda.Item).BorrarValorPorHeader(celda.Column.Header.ToString());
				HayCambios = true;
			}
			//Propiedades.DatosModificados = true;
		}
		#endregion


		#region FIJAR PANEL GRUPOS
		//Comando
		public ICommand cmdFijarPanelGrupos {
			get {
				if (_cmdfijarpanelgrupos == null) _cmdfijarpanelgrupos = new RelayCommand(p => FijarPanelGrupos(p));
				return _cmdfijarpanelgrupos;
			}
		}

		// Ejecución del comando
		private void FijarPanelGrupos(object parametro) {
			Grid panel = (Grid)parametro;
			if (panel == null) return;
			if (PanelGruposFijo) {
				Grid.SetColumn(panel, 0);
			} else {
				Grid.SetColumn(panel, 1);
			}
		}
		#endregion


		#region FIJAR PANEL VALORACIONES
		//Comando
		public ICommand cmdFijarPanelValoraciones {
			get {
				if (_cmdfijarpanelvaloraciones == null) _cmdfijarpanelvaloraciones = new RelayCommand(p => FijarPanelValoraciones(p));
				return _cmdfijarpanelvaloraciones;
			}
		}

		// Ejecución del comando
		private void FijarPanelValoraciones(object parametro) {
			Grid panel = (Grid)parametro;
			if (panel == null) return;
			if (PanelValoracionesFijo) {
				Grid.SetColumn(panel, 2);
			} else {
				Grid.SetColumn(panel, 1);
			}
		}
		#endregion


		#region BORRAR GRUPO
		//Comando
		public ICommand cmdBorrarGrupo {
			get {
				if (_cmdborrargrupo == null) _cmdborrargrupo = new RelayCommand(p => BorrarGrupo(), p => PuedeBorrarGrupo());
				return _cmdborrargrupo;
			}
		}

		// Puede ejecutarse
		private bool PuedeBorrarGrupo() {
			return (GrupoSeleccionado != null);
		}

		// Ejecución del comando
		private void BorrarGrupo() {
			bool? resultado = Mensajes.VerMensaje("Va a borrar el grupo " + GrupoSeleccionado.Validez.ToString("dd-MM-yyyy") +
														 " con " + _listagraficos.Count.ToString() + " gráficos." +
														 "\n\n¿Desea continuar?", "BORRAR GRUPO",
														 true);

			if (resultado != true) return;
			try {
				BdGruposGraficos.BorrarGrupoPorId(GrupoSeleccionado.Id);
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.BorrarGrupo", ex);
			}
			CargarGrupos();
			HayCambios = true;
		}
		#endregion


		#region QUITAR FILTRO
		// Comando
		public ICommand cmdQuitarFiltro {
			get {
				if (_cmdquitarfiltro == null) _cmdquitarfiltro = new RelayCommand(p => QuitarFiltro(p), p => PuedeQuitarFiltro(p));
				return _cmdquitarfiltro;
			}
		}

		// Puede ejecutarse
		private bool PuedeQuitarFiltro(object parametro) {
			if (parametro == null) return false;
			DataGrid tabla = (DataGrid)parametro;
			bool resultado = false;
			foreach (DataGridColumn columna in tabla.Columns) {
				if (columna.SortDirection != null) resultado = true;
			}
			if (VistaGraficos != null && VistaGraficos.Filter != null) return true;
			return resultado;
		}

		private void QuitarFiltro(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			ICollectionView view = CollectionViewSource.GetDefaultView(tabla.ItemsSource);
			if (view != null && view.SortDescriptions != null) {
				view.SortDescriptions.Clear();
				foreach (DataGridColumn columna in tabla.Columns) {
					columna.SortDirection = null;
				}
			}
			if (VistaGraficos != null) VistaGraficos.Filter = null;
			TextoFiltros = "Ninguno";
			PropiedadCambiada(nameof(Detalle));
		}
		#endregion


		#region APLICAR FILTRO
		// Comando
		private ICommand _cmdaplicarfiltro;
		public ICommand cmdAplicarFiltro {
			get {
				if (_cmdaplicarfiltro == null) _cmdaplicarfiltro = new RelayCommand(p => AplicarFiltro(p), p => PuedeAplicarFiltro());
				return _cmdaplicarfiltro;
			}
		}

		// Puede ejecutarse
		private bool PuedeAplicarFiltro() {
			return (HayGrupo && ListaGraficos.Count > 0 && !(VistaGraficos.IsAddingNew || VistaGraficos.IsEditingItem));
		}


		private void AplicarFiltro(object parametro) {
			if (App.Global.CentroActual == Centros.Desconocido) return;
			string filtro = parametro as string;
			if (filtro == null) return;
			int del = 0;
			int al = 0;
			switch (filtro) {
				case "LunJue":
					del = App.Global.PorCentro.LunDel;
					al = App.Global.PorCentro.LunAl;
					//VistaGraficos.Filter = (g) => { return ((g as Grafico).Numero >= del && (g as Grafico).Numero <= al); };
					VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "L"); };
					TextoFiltros = "Lunes a Jueves";
					break;
				case "Vie":
					del = App.Global.PorCentro.VieDel;
					al = App.Global.PorCentro.VieAl;
					//VistaGraficos.Filter = (g) => { return ((g as Grafico).Numero >= del && (g as Grafico).Numero <= al); };
					VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "V"); };
					TextoFiltros = "Viernes";
					break;
				case "Sab":
					del = App.Global.PorCentro.SabDel;
					al = App.Global.PorCentro.SabAl;
					//VistaGraficos.Filter = (g) => { return ((g as Grafico).Numero >= del && (g as Grafico).Numero <= al); };
					VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "S"); };
					TextoFiltros = "Sábados";
					break;
				case "Fes":
					del = App.Global.PorCentro.DomDel;
					al = App.Global.PorCentro.DomAl;
					//VistaGraficos.Filter = (g) => { return ((g as Grafico).Numero >= del && (g as Grafico).Numero <= al); };
					VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "F"); };
					TextoFiltros = "Domingos y Festivos";
					break;
				case "Modificados":
					VistaGraficos.Filter = (g) => { return (g as Grafico).Diferente == true; };
					TextoFiltros = "Gráficos Modificados";
					break;
			}
			BtFiltrarAbierto = false;
			PropiedadCambiada(nameof(Detalle));
		}
		#endregion


		#region PEGAR GRAFICOS
		public ICommand cmdPegarGraficos {
			get {
				if (_cmdpegargraficos == null) _cmdpegargraficos = new RelayCommand(p => PegarGraficos(), p => PuedePegarGraficos());
				return _cmdpegargraficos;
			}
		}

		[Obsolete("Este método está optimizado en el siguiente.")]
		private bool PuedePegarGraficos2(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return false;
			bool resultado = true;
			foreach (DataGridColumn columna in tabla.Columns) {
				if (columna.SortDirection != null) resultado = false;
			}
			if (VistaGraficos != null && VistaGraficos.Count < ListaGraficos.Count) resultado = false;
			return resultado && tabla.CurrentCell != null & Clipboard.ContainsText();
		}

		// NUEVO MÉTODO QUE NO NECESITA EVALUAR
		private bool PuedePegarGraficos() {
			if (ColumnaActual == -1) return false;
			return true;
		}

		[Obsolete("Este método está optimizado en el siguiente.")]
		private void PegarGraficos2(object parametro) {
			// Convertimos el parámetro pasado.
			DataGrid grid = parametro as DataGrid;
			if (grid == null) return;
			// Parseamos los datos del portapapeles y definimos las variables.
			List<string[]> portapapeles = Utils.parseClipboard();
			int columnagrid;
			int filagrid;
			bool esnuevo;
			// Si no hay datos, salimos.
			if (portapapeles == null) return;
			// Si no hay celdas seleccionadas, salimos.
			if (grid.CurrentCell == null) return;
			// Establecemos la columna donde se empieza a pegar.
			columnagrid = grid.Columns.IndexOf(grid.CurrentCell.Column);
			filagrid = grid.Items.IndexOf(grid.CurrentCell.Item);
			// Creamos un objeto ConvertidorHora
			Convertidores.ConvertidorHora cnvHora = new Convertidores.ConvertidorHora();
			// Iteramos por las filas del portapapeles.
			foreach (string[] fila in portapapeles) {
				// Creamos un objeto Grafico o reutilizamos el existente.
				Grafico grafico;
				if (filagrid < ListaGraficos.Count) {
					grafico = ListaGraficos[filagrid];
					esnuevo = false;
				} else {
					grafico = new Grafico();
					esnuevo = true;
				}
				int columna = columnagrid;

				foreach (string texto in fila) {
					if (columna >= grid.Columns.Count) continue;
					while (grid.Columns[columna].Visibility == Visibility.Collapsed) {
						columna++;
					}
					decimal d;
					int i;
					TimeSpan? h;
					switch (columna) {
						case 0: // No Calcular.
							grafico.NoCalcular = false;
							if (int.TryParse(texto, out i)) {
								grafico.NoCalcular = (i != 0);
							} else if (texto.ToLower() != "false") grafico.NoCalcular = true;
							break;
						case 1: // Numero.
							grafico.Numero = Int32.TryParse(texto, out i) ? i : 0;
							break;
						case 2: // Turno.
							grafico.Turno = int.TryParse(texto, out i) ? i : 1;
							break;
						case 3: // Inicio.
							grafico.Inicio = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 4: // Final.
							grafico.Final = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 5: // InicioPartido.
							grafico.InicioPartido = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 6: // FinalPartido.
							grafico.FinalPartido = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 7: // Valoracion.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Valoracion = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 8: // Trabajadas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Trabajadas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 9: // Acumuladas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Acumuladas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 10: // Nocturnas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Nocturnas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 11: // Desayuno.
							grafico.Desayuno = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 12: // Comida.
							grafico.Comida = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 13: // Cena.
							grafico.Cena = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 14: // PlusCena.
							grafico.PlusCena = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 15: // PlusLimpieza.
							grafico.PlusLimpieza = false;
							if (int.TryParse(texto, out i)) {
								grafico.PlusLimpieza = (i != 0);
							} else if (texto.ToLower() != "false") grafico.PlusLimpieza = true;
							break;
						case 16: // PlusPaqueteria.
							grafico.PlusPaqueteria = false;
							if (int.TryParse(texto, out i)) {
								grafico.PlusPaqueteria = (i != 0);
							} else if (texto.ToLower() != "false") grafico.PlusPaqueteria = true;
							break;
					}
					columna++;
				}
				if (esnuevo) {
					ListaGraficos.Add(grafico);
				}
				filagrid++;
				HayCambios = true;
				PropiedadCambiada(nameof(Detalle));
			}
			//Propiedades.DatosModificados = true;
		}

		// NUEVO MÉTODO PEGAR CON VIEWMODEL Y SIN DEPENDENCIA DEL GRID
		private void PegarGraficos() {
			// Parseamos los datos del portapapeles y definimos las variables.
			List<string[]> portapapeles = Utils.parseClipboard();
			bool esnuevo;
			// Si no hay datos, salimos.
			if (portapapeles == null) return;
			// Establecemos la fila donde se empieza a pegar.
			int filagrid = FilaActual;
			if (filagrid == -1) filagrid = VistaGraficos.Count - 1;
			// Creamos un objeto ConvertidorHora
			Convertidores.ConvertidorHora cnvHora = new Convertidores.ConvertidorHora();
			// Iteramos por las filas del portapapeles.
			foreach (string[] fila in portapapeles) {
				// Creamos un objeto Grafico o reutilizamos el existente.
				Grafico grafico;
				if (filagrid < VistaGraficos.Count - 1) { 
					grafico = VistaGraficos.GetItemAt(filagrid) as Grafico;
					esnuevo = false;
				} else {
					grafico = new Grafico();
					esnuevo = true;
				}
				// Establecemos la columna inicial en la que se va a pegar.
				int columna = ColumnaActual;
				// Iteramos por cada campo de la fila del portapapeles
				foreach (string texto in fila) {
					if (columna >= 18) continue;
					while (!ColumnaVisible(columna)) {
						columna++;
					}
					// Evaluamos la columna actual y parseamos el valor del portapapeles a su valor.
					decimal d;
					int i;
					TimeSpan? h;
					switch (columna) {
						case 0: // No Calcular.
							grafico.NoCalcular = false;
							if (int.TryParse(texto, out i)) {
								grafico.NoCalcular = (i != 0);
							} else if (texto.ToLower() != "false") grafico.NoCalcular = true;
							break;
						case 1: // Numero.
							grafico.Numero = Int32.TryParse(texto, out i) ? i : 0;
							break;
						case 2: // DiaSemana.
							grafico.DiaSemana = texto;
							break;
						case 3: // Turno.
							grafico.Turno = int.TryParse(texto, out i) ? i : 1;
							break;
						case 4: // Inicio.
							grafico.Inicio = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 5: // Final.
							grafico.Final = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 6: // InicioPartido.
							grafico.InicioPartido = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 7: // FinalPartido.
							grafico.FinalPartido = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							break;
						case 8: // Valoracion.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Valoracion = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 9: // Trabajadas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Trabajadas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 10: // Acumuladas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Acumuladas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 11: // Nocturnas.
							h = (TimeSpan?)cnvHora.ConvertBack(texto, null, null, null);
							grafico.Nocturnas = h != null ? h.Value : TimeSpan.Zero;
							break;
						case 12: // Desayuno.
							grafico.Desayuno = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 13: // Comida.
							grafico.Comida = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 14: // Cena.
							grafico.Cena = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 15: // PlusCena.
							grafico.PlusCena = decimal.TryParse(texto, out d) ? d : 0;
							break;
						case 16: // PlusLimpieza.
							grafico.PlusLimpieza = false;
							if (int.TryParse(texto, out i)) {
								grafico.PlusLimpieza = (i != 0);
							} else if (texto.ToLower() != "false") grafico.PlusLimpieza = true;
							break;
						case 17: // PlusPaqueteria.
							grafico.PlusPaqueteria = false;
							if (int.TryParse(texto, out i)) {
								grafico.PlusPaqueteria = (i != 0);
							} else if (texto.ToLower() != "false") grafico.PlusPaqueteria = true;
							break;
					}
					columna++;
				}
				// Si el elemento es nuevo, se añade a la vista.
				if (esnuevo) {
					VistaGraficos.AddNewItem(grafico);
					VistaGraficos.CommitNew();
				}
				filagrid++;
				HayCambios = true;
				PropiedadCambiada(nameof(Detalle));
			}
		}



		#endregion


		#region BORRAR VALORACION
		// Comando
		public ICommand cmdBorrarValoracion {
			get {
				if (_cmdborrarvaloracion == null) _cmdborrarvaloracion = new RelayCommand(p => BorrarValoracion(p), p => PuedeBorrarValoracion(p));
				return _cmdborrarvaloracion;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarValoracion(object parametro) {
			if (parametro == null) return false;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.SelectedCells.Count == 0) return false;
			if (tabla.SelectedCells.Count == 1 && tabla.SelectedCells[0].Column.Header.ToString() != "Inicio") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarValoracion(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.SelectedCells == null) return;
			List<ValoracionGrafico> lista = new List<ValoracionGrafico>();
			foreach (DataGridCellInfo celda in tabla.SelectedCells) {
				if (celda.Column.Header.ToString() == "Inicio" && celda.Item.GetType().Name == "ValoracionGrafico") {
					lista.Add((ValoracionGrafico)celda.Item);
				}
			}
			foreach (ValoracionGrafico v in lista) {
				_valoracionesborradas.Add(v);
				GraficoSeleccionado.ListaValoraciones.Remove(v);
				HayCambios = true;
			}
			lista.Clear();
			
		}
		#endregion


		#region DESHACER BORRAR VALORACIONES
		public ICommand cmdDeshacerBorrarValoraciones {
			get {
				if (_cmddeshacerborrarvaloraciones == null) {
					_cmddeshacerborrarvaloraciones = new RelayCommand(p => DeshacerBorrarValoraciones(), p => PuedeDeshacerBorrarValoraciones());
				}
				return _cmddeshacerborrarvaloraciones;
			}
		}

		private bool PuedeDeshacerBorrarValoraciones() {
			return _valoracionesborradas.Count > 0;
		}

		private void DeshacerBorrarValoraciones() {
			if (_valoracionesborradas == null) return;
			foreach (ValoracionGrafico valoracion in _valoracionesborradas) {
				if (valoracion.Nuevo) {
					GraficoSeleccionado.ListaValoraciones.Add(valoracion);
				} else {
					GraficoSeleccionado.ListaValoraciones.Add(valoracion);
					valoracion.Nuevo = false;
				}
				HayCambios = true;;
			}
			_valoracionesborradas.Clear();
			//Propiedades.DatosModificados = true;
		}
		#endregion


		#region CALCULAR VALORACIONES
		public ICommand cmdCalcularValoraciones {
			get {
				if (_cmdcalcularvaloraciones == null) _cmdcalcularvaloraciones = new RelayCommand(p => CalcularValoraciones(), p => PuedeCalcularValoraciones());
				return _cmdcalcularvaloraciones;
			}
		}

		private bool PuedeCalcularValoraciones() {
			if (GraficoSeleccionado == null) return false;
			return GraficoSeleccionado.ListaValoraciones.Count > 0;
		}

		private void CalcularValoraciones() {
			foreach (ValoracionGrafico valoracion in GraficoSeleccionado.ListaValoraciones) {
				valoracion.Calcular();
				HayCambios = true;
			}
			//Propiedades.DatosModificados = true;
		}
		#endregion


		#region NUEVO GRUPO
		//Comando
		public ICommand cmdNuevoGrupo {
			get {
				if (_cmdnuevogrupo == null) _cmdnuevogrupo = new RelayCommand(p => NuevoGrupo());
				return _cmdnuevogrupo;
			}
		}

		// Ejecución del comando
		private void NuevoGrupo() {
			VentanaNuevoGrupoVM ventanaVM = new VentanaNuevoGrupoVM(new MensajesServicio()) { FechaActual = DateTime.Now };
			ventanaVM.ListaGrupos = ListaGrupos;
			if (ListaGrupos.Count > 0) ventanaVM.GrupoSeleccionado = ListaGrupos[ListaGrupos.Count - 1];
			VentanaNuevoGrupo ventana = new VentanaNuevoGrupo { DataContext = ventanaVM };
			if (ventana.ShowDialog() == true) CargarGrupos();
			ventana = null;
			ventanaVM = null;
		}
		#endregion


		#region COMPARAR GRUPO
		public ICommand cmdCompararGrupo {
			get {
				if (_cmdcomparargrupo == null) _cmdcomparargrupo = new RelayCommand(p => CompararGrupo(), p => PuedeCompararGrupo());
				return _cmdcomparargrupo;
			}
		}

		private bool PuedeCompararGrupo() {
			return HayGrupo && GrupoComparacionSeleccionado != null;
		}

		private void CompararGrupo() {

			if (GrupoSeleccionado.Validez.Ticks == GrupoComparacionSeleccionado.Validez.Ticks) return;

			List<Grafico> anteriores = null;
			try {
				anteriores = BdGraficos.getGraficos(GrupoComparacionSeleccionado.Id).ToList();
				if (anteriores.Count == 0) return;
			} catch (Exception ex) {
				Mensajes.VerError("GraficosViewModel.CompararGrupo", ex);
			}
			
			foreach (Grafico grafico in ListaGraficos) {
				Grafico graficoanterior = anteriores.Find(g => g.Numero == grafico.Numero);
				if (grafico.Equals(graficoanterior)) {
					grafico.Diferente = false;
				} else {
					grafico.Diferente = true;
				}
			}

			BtCompararAbierto = false;

		}
		#endregion


		#region AÑADIR VALORACION
		// Comando
		public ICommand cmdAñadirValoracion {
			get {
				if (_cmdañadirvaloracion == null) _cmdañadirvaloracion = new RelayCommand(p => AñadirValoracion(), p => PuedeAñadirValoracion());
				return _cmdañadirvaloracion;
			}
		}

		// Se puede ejecutar
		private bool PuedeAñadirValoracion() {
			if (GraficoSeleccionado == null) return false;
			return HayGrupo;
		}

		// Ejecución del comando
		private void AñadirValoracion() {

			// Creamos la ventana
			VentanaAñadirValoracionGrafico ventana;

			// Creamos las variables auxiliares
			TimeSpan? Inicio = GraficoSeleccionado.HoraFinalValoraciones();
			if (!Inicio.HasValue) { 
				Inicio = GraficoSeleccionado.Inicio;
			}
			
			// Iniciamos el bucle para mostrar la ventana.
			do {
				// Instanciamos la ventana y la vinculamos al ViewModel correspondiente.
				ventana = new VentanaAñadirValoracionGrafico();
				AñadirValoracionGraficoViewModel contextoVentana = new AñadirValoracionGraficoViewModel(Mensajes);
				ventana.DataContext = contextoVentana;

				// Asignamos el inicio de la ventana y el número de gráfico
				contextoVentana.Inicio = Inicio;
				contextoVentana.NumeroGrafico = GraficoSeleccionado.Numero;

				// Creamos la valoración a añadir.
				ValoracionGrafico valoracion = new ValoracionGrafico();

				// Si al mostrar la ventana, pulsamos aceptar...
				if (ventana.ShowDialog() == true) {
					//Extraemos la línea.
					decimal Linea = contextoVentana.Linea;
					// Ponemos el inicio a la valoracion.
					valoracion.Inicio = contextoVentana.Inicio;
					// Si la línea es menor que cero...
					if (Linea < 0) {
						// Cambiamos el signo.
						Linea = Linea * -1;
						// Si está entre 1000 y 1999...
						if (Linea >= 1000 && Linea < 2000) {
							// Extraemos el descanso.
							int descanso = (int)(Linea - 1000);
							valoracion.Descripcion = "Descanso " + descanso.ToString() + " minutos.";
							valoracion.Final = contextoVentana.Inicio + new TimeSpan(0, descanso, 0);
						}
					// Si la línea es mayor que cero.
					} else if (Linea > 0) {
						// Creamos un itinerario.
						Itinerario itinerario = null;
						// Buscamos el itinerario.
						try {
							itinerario = BdItinerarios.GetItinerarioByNombre(Linea);
						} catch (Exception ex) {
							Mensajes.VerError("GraficosViewModel.AñadirValoracion", ex);
						}

						// Si el itinerario no es nulo (existe)...
						if (itinerario != null) {
							valoracion.Linea = Linea;
							valoracion.Descripcion = itinerario.Descripcion;
							valoracion.Final = contextoVentana.Inicio + new TimeSpan(0, itinerario.TiempoReal, 0);
						} else {
							valoracion.Linea = Linea;
							valoracion.Descripcion = "Línea desconocida.";
							valoracion.Final = contextoVentana.Inicio;
						}

					// Si la línea es cero.
					} else {
						valoracion.Final = contextoVentana.Inicio;
					}
					// Guardamos la valoracion
					valoracion.IdGrafico = GraficoSeleccionado.Id;
					GraficoSeleccionado.ListaValoraciones.Add(valoracion);
					HayCambios = true;
					// Ponemos Inicio en la hora final
					Inicio = valoracion.Final;
				} else {
					return;
				}
			} while (true);

		}
		#endregion


		#region EDITAR FILA VALORACION
		//Comando
		public ICommand cmdEditarFilaValoracion {
			get {
				if (_cmdeditarfilavaloracion == null) _cmdeditarfilavaloracion = new RelayCommand(p => EditarFilaValoracion(p));
				return _cmdeditarfilavaloracion;
			}
		}

		// Ejecución del comando
		private void EditarFilaValoracion(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.CurrentCell == null) return;

			DataGridCellInfo celda = tabla.CurrentCell;

			if (celda.Column.Header.ToString() == "Línea") {

				//Extraemos la valoracion.
				ValoracionGrafico valoracion = (ValoracionGrafico)celda.Item;

				// Extraemos la línea
				decimal linea = valoracion.Linea;
				
				// Si la línea es menor que cero...
				if (linea < 0) {
					// Cambiamos el signo.
					linea = linea * -1;
					// Si está entre 1000 y 1999...
					if (linea >= 1000 && linea < 2000) {
						// Extraemos el descanso.
						int descanso = (int)(linea - 1000);
						valoracion.Linea = 0m;
						valoracion.Descripcion = "Descanso " + descanso.ToString() + " minutos.";
						valoracion.Final = valoracion.Inicio + new TimeSpan(0, descanso, 0);
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

					// Si el itinerario no es nulo (existe)...
					if (itinerario != null) {
						valoracion.Linea = linea;
						valoracion.Descripcion = itinerario.Descripcion;
						valoracion.Final = valoracion.Inicio + new TimeSpan(0, itinerario.TiempoReal, 0);
					} else {
						valoracion.Linea = linea;
						valoracion.Descripcion = "Línea desconocida.";
						valoracion.Final = valoracion.Inicio;
					}

					// Si la línea es cero.
				} else {
					valoracion.Final = valoracion.Inicio;
				}
			}
			GraficoSeleccionado.NotificarCambioValoraciones();

		}
		#endregion


		#region MODIFICAR FECHA GRUPO
		// Comando
		public ICommand cmdModificarFechaGrupo {
			get {
				if (_modificarfechagrupo == null) _modificarfechagrupo = new RelayCommand(p => ModificarFechaGrupo(), p => PuedeModificarFechaGrupo());
				return _modificarfechagrupo;
			}
		}

		// Se puede ejecutar
		private bool PuedeModificarFechaGrupo() {
			if (GrupoSeleccionado == null) return false;
			return HayGrupo;
		}

		// Ejecución del comando
		private void ModificarFechaGrupo() {

			// Avisamos de que se va a cambiar la fecha, y si contestamos no, salimos.
			if (Mensajes.VerMensaje("Vas a cambiar la fecha de validez del grupo.\n" +
									   "Esto puede alterar los calendarios que dependían de la fecha anterior\n\n" +
									   "¿Desea continuar?", "Cambiar fecha del grupo", true) == false) return;


			VisibilidadPanelValidezGrupo = Visibility.Visible;

			

		}
		#endregion


		#region SELECCIONAR FECHA GRUPO
		//// Comando
		public ICommand cmdSeleccionarFechaGrupo {
			get {
				if (_seleccionarfechagrupo == null) _seleccionarfechagrupo = new RelayCommand(p => SeleccionarFechaGrupo(p), p => PuedeSeleccionarFechaGrupo(p));
				return _seleccionarfechagrupo;
			}
		}

		// Se puede ejecutar
		private bool PuedeSeleccionarFechaGrupo(object parametro) {
			if (GrupoSeleccionado == null) return false;
			return HayGrupo;
		}

		// Ejecución del comando
		private void SeleccionarFechaGrupo(object parametro) {

			VisibilidadPanelValidezGrupo = Visibility.Collapsed;
			if (parametro == null) HayCambios = true;

		}
		#endregion


		#region GRÁFICOS EN PDF
		// Comando
		private ICommand _cmdgraficosenpdf;
		public ICommand cmdGraficosEnPdf {
			get {
				if (_cmdgraficosenpdf == null) _cmdgraficosenpdf = new RelayCommand(p => GraficosEnPDF(), p => PuedeGraficosEnPdf());
				return _cmdgraficosenpdf;
			}
		}


		private bool PuedeGraficosEnPdf() {
			return ListaGraficos.Count > 0;
		}


		private async void GraficosEnPDF2() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el archivo donde guardarlo.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1}.pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.Graficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.Graficos);
					await GraficosPrintModel.CrearGraficosEnPdf(libro, VistaGraficos, GrupoSeleccionado.Validez);
					Informes.ExportarLibroToPdf(libro, ruta, App.Global.Configuracion.AbrirPDFs);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}


		private async void GraficosEnPDF() {
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el archivo donde guardarlo.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1}.pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.Graficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
					doc.GetPdfDocument().GetDocumentInfo().SetTitle("Gráficos");
					doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{GrupoSeleccionado.Validez.ToString("dd-MMMM-yyyy").ToUpper()}");
					doc.SetMargins(25, 25, 25, 25);
					await GraficosPrintModel.CrearGraficosEnPdf_7(doc, VistaGraficos, GrupoSeleccionado.Validez);
					doc.Close();
					if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}


		#endregion


		#region GRÁFICOS INDIVIDUALES EN PDF
		// Comando
		private ICommand _cmdgraficosindividualesenpdf;
		public ICommand cmdGraficosIndividualesEnPdf {
			get {
				if (_cmdgraficosindividualesenpdf == null) _cmdgraficosindividualesenpdf = new RelayCommand(p => GraficosIndividualesEnPDF(), 
																											p => PuedeGraficosIndividualesEnPdf());
				return _cmdgraficosindividualesenpdf;
			}
		}


		private bool PuedeGraficosIndividualesEnPdf() {
			return ListaGraficos.Count > 0;
		}


		private async void GraficosIndividualesEnPDF2() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Definimos el nombre del archivo a guardar.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1} (Individuales).pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.GraficoIndividual, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.GraficoIndividual);
					await GraficosPrintModel.CrearGraficosIndividualesEnPdf(libro, VistaGraficos, GrupoSeleccionado.Validez);
					Informes.ExportarLibroToPdf(libro, ruta, App.Global.Configuracion.AbrirPDFs);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}


		private async void GraficosIndividualesEnPDF() {

			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Definimos el nombre del archivo a guardar.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1} (Individuales).pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.GraficoIndividual, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					iText.Layout.Document doc = Informes.GetNuevoPdfA5(ruta);
					doc.GetPdfDocument().GetDocumentInfo().SetTitle("Gráficos Individuales");
					doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{GrupoSeleccionado.Validez.ToString("dd-MMMM-yyyy").ToUpper()}");
					doc.SetMargins(25, 25, 25, 25);
					await GraficosPrintModel.CrearGraficosIndividualesEnPdf_7(doc, VistaGraficos, GrupoSeleccionado.Validez);
					doc.Close();
					if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}

		#endregion


		#region SOLO AÑO ACTUAL EN GRUPOS
		// Comando
		private ICommand _soloañoactualengrupos;
		public ICommand cmdSoloAñoActualEnGrupos {
			get {
				if (_soloañoactualengrupos == null) _soloañoactualengrupos = new RelayCommand(p => SoloAñoActualEnGrupos());
				return _soloañoactualengrupos;
			}
		}

		private void SoloAñoActualEnGrupos() {

			if (VistaGrupos == null) return;

			if (App.Global.Configuracion.SoloAñoActualEnGruposGraficos) {
				VistaGrupos.Filter = (g) => { return (g as GrupoGraficos).Validez.Year == DateTime.Now.Year; };
			} else {
				VistaGrupos.Filter = null;
			}
		}
		#endregion


		#region ESTADÍSTICAS GRÁFICOS EN PDF
		// Comando
		private ICommand _cmdestadisticasgraficos;
		public ICommand cmdEstadisticasGraficos {
			get {
				if (_cmdestadisticasgraficos == null) _cmdestadisticasgraficos = new RelayCommand(p => EstadisticasGraficos(), p => PuedeEstadisticasGraficos());
				return _cmdestadisticasgraficos;
			}
		}


		private bool PuedeEstadisticasGraficos() {
			return VistaGraficos?.Count > 0;
		}


		private async void EstadisticasGraficos() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Definimos el nombre del archivo a guardar.
				string nombreArchivo = String.Format("Estadisticas Gráficos {0:yyyy}-{0:MM}-{0:dd} - {1}.pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.EstadisticasGraficos);
					await GraficosPrintModel.CrearEstadisticasGraficosEnPdf(libro, GrupoSeleccionado.Validez, GrupoSeleccionado.Id);
					Informes.ExportarLibroToPdf(libro, ruta, App.Global.Configuracion.AbrirPDFs);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.EstadisticasGraficos", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region ESTADÍSTICAS GRUPOS GRÁFICOS EN PDF
		// Comando
		private ICommand _cmdestadisticasgruposgraficos;
		public ICommand cmdEstadisticasGruposGraficos {
			get {
				if (_cmdestadisticasgruposgraficos == null) _cmdestadisticasgruposgraficos = new RelayCommand(p => EstadisticasGruposGraficos(), p => PuedeEstadisticasGruposGraficos());
				return _cmdestadisticasgruposgraficos;
			}
		}


		private bool PuedeEstadisticasGruposGraficos() {
			return ListaGrupos.Count > 0;
		}


		private async void EstadisticasGruposGraficos() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Definimos el nombre del archivo a guardar.
				string nombreArchivo = String.Format("Estadisticas Gráficos - {0}.pdf", App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.EstadisticasGraficos);
					await GraficosPrintModel.CrearEstadisticasGruposGraficosEnPdf(libro, FechaEstadisticas);
					Informes.ExportarHojaToPdf(libro, 1, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.EstadisticasGruposGraficos", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region ESTADÍSTICAS GRÁFICOS POR CENTROS
		// Comando
		private ICommand _cmdestadisticasgraficosporcentros;
		public ICommand cmdEstadisticasGraficosPorCentros {
			get {
				if (_cmdestadisticasgraficosporcentros == null) _cmdestadisticasgraficosporcentros = new RelayCommand(p => EstadisticasGraficosPorCentros());
				return _cmdestadisticasgraficosporcentros;
			}
		}


		private async void EstadisticasGraficosPorCentros() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Definimos el nombre del archivo a guardar.
				string nombreArchivo = String.Format("Estadisticas Gráficos Por Centros.pdf");
				string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficosPorCentros, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.EstadisticasGraficosPorCentros);
					List<EstadisticasGraficos> lista = new List<EstadisticasGraficos>();
					List<EstadisticasGraficos> listaTemporal;
					GrupoGraficos grupo = null;
					// Estadisticas de Bilbao
					grupo = BdGruposGraficos.GetUltimoGrupo(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Bilbao)));
					if (grupo != null) {
						listaTemporal =BdGraficos.GetEstadisticasGrupoGraficos(grupo.Id, new OleDbConnection(App.Global.GetCadenaConexion(Centros.Bilbao)));
						foreach(EstadisticasGraficos e in listaTemporal) {
							e.Centro = Centros.Bilbao;
							lista.Add(e);
						}
					}
					// Estadisticas de Donosti
					grupo = BdGruposGraficos.GetUltimoGrupo(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Donosti)));
					if (grupo != null) {
						listaTemporal = BdGraficos.GetEstadisticasGrupoGraficos(grupo.Id, new OleDbConnection(App.Global.GetCadenaConexion(Centros.Donosti)));
						foreach (EstadisticasGraficos e in listaTemporal) {
							e.Centro = Centros.Donosti;
							lista.Add(e);
						}
					}
					// Estadisticas de Arrasate
					grupo = BdGruposGraficos.GetUltimoGrupo(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Arrasate)));
					if (grupo != null) {
						listaTemporal = BdGraficos.GetEstadisticasGrupoGraficos(grupo.Id, new OleDbConnection(App.Global.GetCadenaConexion(Centros.Arrasate)));
						foreach (EstadisticasGraficos e in listaTemporal) {
							e.Centro = Centros.Arrasate;
							lista.Add(e);
						}
					}
					// Estadisticas de Vitoria
					grupo = BdGruposGraficos.GetUltimoGrupo(new OleDbConnection(App.Global.GetCadenaConexion(Centros.Vitoria)));
					if (grupo != null) {
						listaTemporal = BdGraficos.GetEstadisticasGrupoGraficos(grupo.Id, new OleDbConnection(App.Global.GetCadenaConexion(Centros.Vitoria)));
						foreach (EstadisticasGraficos e in listaTemporal) {
							e.Centro = Centros.Vitoria;
							lista.Add(e);
						}
					}
					//Action<double> modificaBarra = new Action<double>((valor) => App.Global.ValorBarraProgreso = valor);
					await GraficosPrintModel.CrearEstadisticasGraficosPorCentros(libro, lista);
					Informes.ExportarLibroToPdf(libro, ruta, App.Global.Configuracion.AbrirPDFs);
				}
			} catch (Exception ex) {
				Mensajes.VerError("GraficosCommands.EstadisticasGruposGraficos", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region CAMBIAR MODO SELECCION

		// Comando
		private ICommand _cmdcambiarmodoseleccion;
		public ICommand cmdCambiarModoSeleccion {
			get {
				if (_cmdcambiarmodoseleccion == null) _cmdcambiarmodoseleccion = new RelayCommand(p => CambiarModoSeleccion());
				return _cmdcambiarmodoseleccion;
			}
		}


		// Ejecución del comando
		private void CambiarModoSeleccion() {

			if (VisibilidadBotonSeleccionFila == Visibility.Visible) {
				VisibilidadBotonSeleccionFila = Visibility.Collapsed;
				VisibilidadBotonSeleccionCelda = Visibility.Visible;
				ModoSeleccion = DataGridSelectionUnit.FullRow;
			} else {
				VisibilidadBotonSeleccionCelda = Visibility.Collapsed;
				VisibilidadBotonSeleccionFila = Visibility.Visible;
				ModoSeleccion = DataGridSelectionUnit.Cell;
			}

		}
		#endregion


		#region COMANDO DEDUCIR DÍA SEMANA

		// Comando
		private ICommand _cmddeducirdiasemana;
		public ICommand cmdDeducirDiaSemana
		{
			get
			{
				if (_cmddeducirdiasemana == null) _cmddeducirdiasemana = new RelayCommand(p => DeducirDiaSemana(), p => PuedeDeducirDiaSemana());
				return _cmddeducirdiasemana;
			}
		}


		// Se puede ejecutar el comando
		private bool PuedeDeducirDiaSemana()
		{
			return ListaGraficos.Any();
		}

		// Ejecución del comando
		private void DeducirDiaSemana()
		{
			foreach (Grafico grafico in _listagraficos)
			{
				if (string.IsNullOrWhiteSpace(grafico.DiaSemana))
				{
					if (grafico.Numero >= App.Global.PorCentro.LunDel && grafico.Numero <= App.Global.PorCentro.LunAl) grafico.DiaSemana = "L";
					if (grafico.Numero >= App.Global.PorCentro.VieDel && grafico.Numero <= App.Global.PorCentro.VieAl) grafico.DiaSemana = "V";
					if (grafico.Numero >= App.Global.PorCentro.SabDel && grafico.Numero <= App.Global.PorCentro.SabAl) grafico.DiaSemana = "S";
					if (grafico.Numero >= App.Global.PorCentro.DomDel && grafico.Numero <= App.Global.PorCentro.DomAl) grafico.DiaSemana = "F";
				}
				//HayCambios = true;
			}
			BtAccionesAbierto = false;

		}
		#endregion










	} //Fin de la clase.
}
