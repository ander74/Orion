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
	using System.ComponentModel;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Input;
	using Config;
	using Models;

	public partial class ConductoresViewModel {


		#region CAMPOS PRIVADOS (Declaración de comandos)
		//====================================================================================================
		private ICommand _cmdverregulaciones;
		private ICommand _cmdfijarpanelregulaciones;
		private ICommand _cmdborrarconductor;
		private ICommand _cmddeshacerborrar;
		private ICommand _cmdborrarceldas;
		private ICommand _cmdquitarfiltro;
		private ICommand _cmdpegarconductores;
		private ICommand _cmdborrarregulacion;
		private ICommand _cmddeshacerborrarregulaciones;
		private ICommand _cmdactualizarlista;
		#endregion
		//====================================================================================================


		//#region VER REGULACIONES
		////Comando
		//public ICommand cmdVerRegulaciones {
		//	get {
		//		if (_cmdverregulaciones == null) _cmdverregulaciones = new RelayCommand(p => VerRegulaciones(p));
		//		return _cmdverregulaciones;
		//	}
		//}

		//// Ejecución del comando
		//private void VerRegulaciones(object parametro) {
		//	if (parametro == null) return;
		//	Grid panel = (Grid)parametro;
		//	if (panel.IsVisible) {
		//		PanelRegulacionesVisibilidad = Visibility.Collapsed;
		//		PanelRegulacionesFijo = false;
		//		Grid.SetColumn(panel, 1);
		//	} else {
		//		PanelRegulacionesVisibilidad = Visibility.Visible;
		//	}
		//}
		//#endregion


		//#region FIJAR PANEL REGULACIONES
		////Comando
		//public ICommand cmdFijarPanelRegulaciones {
		//	get {
		//		if (_cmdfijarpanelregulaciones == null) _cmdfijarpanelregulaciones = new RelayCommand(p => FijarPanelRegulaciones(p));
		//		return _cmdfijarpanelregulaciones;
		//	}
		//}

		//// Ejecución del comando
		//private void FijarPanelRegulaciones(object parametro) {
		//	if (parametro == null) return;
		//	Grid panel = (Grid)parametro;
		//	if (PanelRegulacionesFijo) {
		//		Grid.SetColumn(panel, 2);
		//	} else {
		//		Grid.SetColumn(panel, 1);
		//	}
		//}
		//#endregion


		#region BORRAR CONDUCTOR
		// Comando
		public ICommand cmdBorrarConductor {
			get { if (_cmdborrarconductor == null) _cmdborrarconductor = new RelayCommand(p => BorrarConductor(p), p => PuedeBorrarConductor(p));
				return _cmdborrarconductor;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarConductor(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			return tabla.CurrentCell.Column.Header.ToString() == "Número" || tabla.CurrentCell.Item.GetType().Name == "Conductor";
		}

		// Ejecución del comando
		private void BorrarConductor(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null) return;
			List<Conductor> lista = new List<Conductor>();
			DataGridCellInfo celda = tabla.CurrentCell;
			if (!(celda.Item is Conductor conductor)) return;
			if (mensajes.VerMensaje($"Vas a borrar al conductor:\n\n{conductor.Id:000}: {conductor.Apellidos}\n\n" +
									$"Esto hará que se borren todos sus calendarios.\n\n" +
									$"¿Deseas continuar?", "ATENCIÓN", true) == false) return;
			if (celda.Column.Header.ToString() == "Número" && celda.Item.GetType().Name == "Conductor") {
				lista.Add(conductor);
			}
			
			foreach (Conductor c in lista) {
				_listaborrados.Add(c);
				ListaConductores.Remove(c);
				HayCambios = true;
			}
			lista.Clear();
			App.Global.CalendariosVM.BorrarCalendariosPorConductor(conductor.Id);
			//TODO: Esta última igual se elimina y se deja que sea el propio sistema el que se encargue de eliminar los calendarios.
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
			foreach (Conductor conductor in _listaborrados) {
				if (conductor.Nuevo) {
					ListaConductores.Add(conductor);
				} else {
					ListaConductores.Add(conductor);
					conductor.Nuevo = false;
				}
				HayCambios = true;
				App.Global.CalendariosVM.DeshacerBorrarPorConductor(conductor.Id);
			}
			_listaborrados.Clear();
		}
		#endregion


		//#region BORRAR CELDAS
		//public ICommand cmdBorrarCeldas {
		//	get {
		//		if (_cmdborrarceldas == null) _cmdborrarceldas = new RelayCommand(p => BorrarCeldas(p));
		//		return _cmdborrarceldas;
		//	}
		//}

		//private void BorrarCeldas(object parametro) {
		//	DataGrid tabla = parametro as DataGrid;
		//	if (tabla == null || tabla.CurrentCell == null) return;
		//	Conductor conductor = tabla.CurrentCell.Item as Conductor;
		//	if (conductor == null) return;
		//	conductor.BorrarValorPorHeader(tabla.CurrentCell.Column.Header.ToString());
		//	HayCambios = true;
		//}
		//#endregion


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
			foreach (DataGridColumn columna in tabla.Columns) {
				if (columna.SortDirection != null) return true;
			}
			if (VistaConductores != null && ListaConductores.Count > VistaConductores.Count) return true;//ADDED
			return false;
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
			VistaConductores.Filter = null;//ADDED
		}
		#endregion


		//#region PEGAR CONDUCTORES
		//public ICommand cmdPegarConductores {
		//	get {
		//		if (_cmdpegarconductores == null) _cmdpegarconductores = new RelayCommand(p => PegarConductores(p), p => PuedePegarConductores(p));
		//		return _cmdpegarconductores;
		//	}
		//}

		//private bool PuedePegarConductores(object parametro) {
		//	DataGrid tabla = parametro as DataGrid;
		//	if (tabla == null) return false;
		//	bool resultado = true;
		//	foreach (DataGridColumn columna in tabla.Columns) {
		//		if (columna.SortDirection != null) resultado = false;
		//	}
		//	if (VistaConductores != null && VistaConductores.Count < ListaConductores.Count) resultado = false;//ADDED
		//	return resultado && tabla.CurrentCell != null & Clipboard.ContainsText();
		//}

		//private void PegarConductores(object parametro) {
		//	// Convertimos el parámetro pasado.
		//	DataGrid grid = parametro as DataGrid;
		//	if (grid == null || grid.CurrentCell == null) return;
		//	// Parseamos los datos del portapapeles y definimos las variables.
		//	List<string[]> portapapeles = Utils.parseClipboard();
		//	int columnagrid;
		//	int filagrid;
		//	bool esnuevo;
		//	// Si no hay datos, salimos.
		//	if (portapapeles == null) return;
		//	// Establecemos la columna donde se empieza a pegar.
		//	columnagrid = grid.Columns.IndexOf(grid.CurrentCell.Column);
		//	filagrid = grid.Items.IndexOf(grid.CurrentCell.Item);
		//	// Creamos un objeto ConvertidorHora
		//	Convertidores.ConvertidorHora cnvHora = new Convertidores.ConvertidorHora();
		//	// Iteramos por las filas del portapapeles.
		//	foreach (string[] fila in portapapeles) {
		//		// Creamos un objeto Conductor o reutilizamos el existente.
		//		Conductor conductor;
		//		if (filagrid < ListaConductores.Count) {
		//			conductor = ListaConductores[filagrid];
		//			esnuevo = false;
		//		} else {
		//			conductor = new Conductor();
		//			esnuevo = true;
		//		}
		//		int columna = columnagrid;

		//		foreach (string texto in fila) {
		//			while (grid.Columns[columna].Visibility == Visibility.Collapsed) {
		//				columna++;
		//			}
		//			int i;
		//			switch (columna) {
		//				case 0: // Numero.
		//					conductor.Id = int.TryParse(texto, out i) ? i : 0;
		//					break;
		//				case 1: // Nombre.
		//					conductor.Nombre = texto;
		//					break;
		//				case 2: // Apellidos.
		//					conductor.Apellidos = texto;
		//					break;
		//				case 3: // Telefono.
		//					conductor.Telefono = texto;
		//					break;
		//				case 4: // Email.
		//					conductor.Email = texto;
		//					break;
		//				case 5: // Fijo.
		//					conductor.Indefinido = false;
		//					if (int.TryParse(texto, out i)) {
		//						conductor.Indefinido = (i != 0);
		//					} else if (texto.ToLower() != "false") conductor.Indefinido = true;
		//					break;
		//				case 6: // Horas.
		//					conductor.Acumuladas = (TimeSpan)cnvHora.ConvertBack(texto, null, null, null);
		//					break;
		//				case 7: // Descansos.
		//					conductor.Descansos = int.TryParse(texto, out i) ? i : 0;
		//					break;
		//				case 8: // Descansos No Disfrutados.
		//					conductor.DescansosNoDisfrutados = int.TryParse(texto, out i) ? i : 0;
		//					break;
		//			}
		//			columna++;
		//		}
		//		if (esnuevo) {
		//			ListaConductores.Add(conductor);
		//		}
		//		filagrid++;
		//		HayCambios = true;
		//	}
		//}
		//#endregion


		#region BORRAR REGULACION
		// Comando
		public ICommand cmdBorrarRegulacion {
			get {
				if (_cmdborrarregulacion == null) _cmdborrarregulacion = new RelayCommand(p => BorrarRegulacion(p), p => PuedeBorrarRegulacion(p));
				return _cmdborrarregulacion;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarRegulacion(object parametro) {
			if (parametro == null) return false;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.SelectedCells.Count == 0) return false;
			if (tabla.SelectedCells.Count == 1 && tabla.SelectedCells[0].Column.Header.ToString() != "Fecha") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarRegulacion(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.SelectedCells == null) return;
			List<RegulacionConductor> lista = new List<RegulacionConductor>();
			foreach (DataGridCellInfo celda in tabla.SelectedCells) {
				if (celda.Column.Header.ToString() == "Fecha" && celda.Item.GetType().Name == "RegulacionConductor") {
					lista.Add((RegulacionConductor)celda.Item);
				}
			}
			foreach (RegulacionConductor r in lista) {
				_regulacionesborradas.Add(r);
				ConductorSeleccionado.ListaRegulaciones.Remove(r);
				HayCambios = true;
			}
			lista.Clear();
			
		}
		#endregion


		#region DESHACER BORRAR REGULACIONES
		public ICommand cmdDeshacerBorrarRegulaciones {
			get {
				if (_cmddeshacerborrarregulaciones == null) {
					_cmddeshacerborrarregulaciones = new RelayCommand(p => DeshacerBorrarRegulaciones(), p => PuedeDeshacerBorrarRegulaciones());
				}
				return _cmddeshacerborrarregulaciones;
			}
		}

		private bool PuedeDeshacerBorrarRegulaciones() {
			return _regulacionesborradas.Count > 0;
		}

		private void DeshacerBorrarRegulaciones() {
			if (_regulacionesborradas == null) return;
			foreach (RegulacionConductor valoracion in _regulacionesborradas) {
				if (valoracion.Nuevo) {
					ConductorSeleccionado.ListaRegulaciones.Add(valoracion);
				} else {
					ConductorSeleccionado.ListaRegulaciones.Add(valoracion);
					valoracion.Nuevo = false;
				}
				HayCambios = true;
			}
			_regulacionesborradas.Clear();
		}
		#endregion


		#region ACTUALIZAR LISTA
		// Comando
		public ICommand cmdActualizarLista {
			get {
				if (_cmdactualizarlista == null) _cmdactualizarlista = new RelayCommand(p => ActualizarLista(), p => PuedeActualizarLista());
				return _cmdactualizarlista;
			}
		}

		// Se puede ejecutar
		private bool PuedeActualizarLista() {
			if (App.Global.CentroActual == Centros.Desconocido) return false;
			return true;
		}

		// Ejecución del comando
		private void ActualizarLista() {

			GuardarConductores();
			Reiniciar();
		}
		#endregion


		#region AÑADIR REGULACION A TODOS
		// Comando
		private ICommand _cmdañadirregulacionatodos;
		public ICommand cmdAñadirRegulacionATodos {
			get {
				if (_cmdañadirregulacionatodos == null) _cmdañadirregulacionatodos = new RelayCommand(p => AñadirRegulacionATodos(), p => PuedeAñadirRegulacionATodos());
				return _cmdañadirregulacionatodos;
			}
		}

		// Se puede ejecutar
		private bool PuedeAñadirRegulacionATodos() {
			if (RegulacionSeleccionada == null) return false;
			return true;
		}

		// Ejecución del comando
		private void AñadirRegulacionATodos() {

			string mensaje = "Vas a insertar la siguiente regulacion a todos los conductores.\n\n";
			mensaje += $"Fecha: {RegulacionSeleccionada.Fecha:dd-MM-yyyy}\n";
			mensaje += $"Horas: {RegulacionSeleccionada.Horas.ToTexto(true)}\n";
			mensaje += $"DCs: {RegulacionSeleccionada.Descansos:0.0000}\n";
			mensaje += $"DNDs: {RegulacionSeleccionada.Dnds:0.0000}\n";
			mensaje += $"Motivo: {RegulacionSeleccionada.Motivo}\n\n";
			mensaje += "¿Desea continuar?";
			if (mensajes.VerMensaje(mensaje, "Añadir Regulacion a Todos", true) == true) {
				foreach (Conductor cond in ListaConductores) {
					if (cond.Id != RegulacionSeleccionada.IdConductor) {
						RegulacionConductor reg = new RegulacionConductor();
						reg.Codigo = RegulacionSeleccionada.Codigo;
						reg.Descansos = RegulacionSeleccionada.Descansos;
						reg.Dnds = RegulacionSeleccionada.Dnds;
						reg.Fecha = RegulacionSeleccionada.Fecha;
						reg.Horas = RegulacionSeleccionada.Horas;
						reg.Motivo = RegulacionSeleccionada.Motivo;
						reg.IdConductor = cond.Id;
						cond.ListaRegulaciones.Add(reg);
					}
				}
			}
		}
		#endregion


		#region TEXTOMINUSCULAS

		// Comando
		private ICommand _cmdtextominusculas;
		public ICommand cmdTextoMinusculas {
			get {
				if (_cmdtextominusculas == null) _cmdtextominusculas = new RelayCommand(p => TextoMinusculas(), p => PuedeTextoMinusculas());
				return _cmdtextominusculas;
			}
		}


		// Se puede ejecutar el comando
		private bool PuedeTextoMinusculas() {
			return ListaConductores != null && ListaConductores.Count > 0;
		}

		// Ejecución del comando
		private void TextoMinusculas() {
			foreach (Conductor c in ListaConductores) {
				string nombre = c.Nombre.ToLower();
				string apellidos = c.Apellidos.ToLower();
				c.Nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombre);
				c.Apellidos = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(apellidos);
			}
		}
		#endregion


		//#region CAMBIAR MODO SELECCION

		//// Comando
		//private ICommand _cmdcambiarmodoseleccion;
		//public ICommand cmdCambiarModoSeleccion {
		//	get {
		//		if (_cmdcambiarmodoseleccion == null) _cmdcambiarmodoseleccion = new RelayCommand(p => CambiarModoSeleccion());
		//		return _cmdcambiarmodoseleccion;
		//	}
		//}


		//// Ejecución del comando
		//private void CambiarModoSeleccion() {

		//	if (VisibilidadBotonSeleccionFila == Visibility.Visible) {
		//		VisibilidadBotonSeleccionFila = Visibility.Collapsed;
		//		VisibilidadBotonSeleccionCelda = Visibility.Visible;
		//		ModoSeleccion = DataGridSelectionUnit.FullRow;
		//	} else {
		//		VisibilidadBotonSeleccionCelda = Visibility.Collapsed;
		//		VisibilidadBotonSeleccionFila = Visibility.Visible;
		//		ModoSeleccion = DataGridSelectionUnit.Cell;
		//	}

		//}
		//#endregion




	} //Final de clase
}
