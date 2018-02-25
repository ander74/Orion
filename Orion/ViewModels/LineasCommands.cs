#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using Orion.Config;
using Orion.Views;
using Orion.DataModels;
using System.Collections.ObjectModel;

namespace Orion.ViewModels {


	public partial class LineasViewModel {


		#region BORRAR LÍNEA
		// Comando
		private ICommand _cmdborrarLinea;
		public ICommand cmdBorrarLinea {
			get {
				if (_cmdborrarLinea == null) _cmdborrarLinea = new RelayCommand(p => BorrarLinea(p), p => PuedeBorrarLinea(p));
				return _cmdborrarLinea;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarLinea(object parametro) {
			if (TablaSeleccionada != Tablas.Lineas) return false;
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Línea" || tabla.CurrentCell.Item.GetType().Name != "Linea") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarLinea(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null) return;
			DataGridCellInfo celda = tabla.CurrentCell;
			if (celda.Column.Header.ToString() == "Línea" && celda.Item.GetType().Name == "Linea") {
				_listalineasborradas.Add(celda.Item as Linea);
				_listalineas.Remove(celda.Item as Linea);
				HayCambios = true;
			}
			LineaSeleccionada = null;
			ItinerarioSeleccionado = null;
			ParadaSeleccionada = null;
		}
		#endregion


		#region BORRAR ITINERARIO
		// Comando
		private ICommand _cmdborraritinerario;
		public ICommand cmdBorrarItinerario {
			get {
				if (_cmdborraritinerario == null) _cmdborraritinerario = new RelayCommand(p => BorrarItinerario(p), p => PuedeBorrarItinerario(p));
				return _cmdborraritinerario;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarItinerario(object parametro) {
			if (TablaSeleccionada != Tablas.Itinerarios) return false;
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Itinerario" || tabla.CurrentCell.Item.GetType().Name != "Itinerario") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarItinerario(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null) return;
			DataGridCellInfo celda = tabla.CurrentCell;
			if (celda.Column.Header.ToString() == "Itinerario" && celda.Item.GetType().Name == "Itinerario") {
				LineaSeleccionada.ItinerariosBorrados.Add(celda.Item as Itinerario);
				LineaSeleccionada.ListaItinerarios.Remove(celda.Item as Itinerario);
				LineaSeleccionada.Modificado = true;
				HayCambios = true;
			}
			ItinerarioSeleccionado = null;
			ParadaSeleccionada = null;
		}
		#endregion


		#region BORRAR PARADA
		// Comando
		private ICommand _cmdborrarparada;
		public ICommand cmdBorrarParada {
			get {
				if (_cmdborrarparada == null) _cmdborrarparada = new RelayCommand(p => BorrarParada(p), p => PuedeBorrarParada(p));
				return _cmdborrarparada;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarParada(object parametro) {
			if (TablaSeleccionada != Tablas.Paradas) return false;
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Orden" || tabla.CurrentCell.Item.GetType().Name != "Parada") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarParada(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null) return;
			DataGridCellInfo celda = tabla.CurrentCell;
			if (celda.Column.Header.ToString() == "Orden" && celda.Item.GetType().Name == "Parada") {
				ItinerarioSeleccionado.ParadasBorradas.Add(celda.Item as Parada);
				ItinerarioSeleccionado.ListaParadas.Remove(celda.Item as Parada);
				LineaSeleccionada.Modificado = true;
				HayCambios = true;
			}
			ParadaSeleccionada = null;
			
		}
		#endregion


		#region DESHACER BORRAR
		private ICommand _cmddeshacerborrar;
		public ICommand cmdDeshacerBorrar {
			get {
				if (_cmddeshacerborrar == null) _cmddeshacerborrar = new RelayCommand(p => DeshacerBorrar(), p => PuedeDeshacerBorrar());
				return _cmddeshacerborrar;
			}
		}

		private bool PuedeDeshacerBorrar() {
			if (LineaSeleccionada == null) return false;
			if (_listalineasborradas.Count > 0) return true;
			if (LineaSeleccionada.ItinerariosBorrados.Count > 0) return true;
			if (ItinerarioSeleccionado == null) return false;
			if (ItinerarioSeleccionado.ParadasBorradas.Count > 0) return true;
			return false;
		}

		private void DeshacerBorrar() {

			if (ItinerarioSeleccionado != null) {
				foreach (Parada p in ItinerarioSeleccionado.ParadasBorradas) {
					if (p.Nuevo) {
						ItinerarioSeleccionado.ListaParadas.Add(p);
					} else {
						ItinerarioSeleccionado.ListaParadas.Add(p);
						p.Nuevo = false;
					}
				}
				ItinerarioSeleccionado.ParadasBorradas.Clear();
			}

			if (LineaSeleccionada != null) {
				foreach (Itinerario i in LineaSeleccionada.ItinerariosBorrados) {
					if (i.Nuevo) {
						LineaSeleccionada.ListaItinerarios.Add(i);
					} else {
						LineaSeleccionada.ListaItinerarios.Add(i);
						i.Nuevo = false;
					}
				}
				LineaSeleccionada.ItinerariosBorrados.Clear();
			}

			if (_listalineasborradas != null) {
				foreach (Linea l in _listalineasborradas) {
					if (l.Nuevo) {
						ListaLineas.Add(l);
					} else {
						ListaLineas.Add(l);
						l.Nuevo = false;
					}
				}
				_listalineasborradas.Clear();
			}

			HayCambios = true;
		}
		#endregion


		#region PEGAR
		private ICommand _cmdpegar;
		public ICommand cmdPegar {
			get {
				if (_cmdpegar == null) _cmdpegar = new RelayCommand(p => Pegar(), p => PuedePegar());
				return _cmdpegar;
			}
		}

		private bool PuedePegar() {
			if (TablaParaCopy == null) return false;
			return TablaParaCopy.SelectedCells.Count > 0 & Clipboard.ContainsText();
		}

		private void Pegar() {
			// Si no hay seleccionada una tabla
			if (TablaParaCopy == null || TablaParaCopy.CurrentCell == null) return;
			// Parseamos los datos del portapapeles y definimos las variables.
			List<string[]> portapapeles = Utils.parseClipboard();
			int columnagrid;
			int filagrid;
			bool esnuevo;
			// Si no hay datos, salimos.
			if (portapapeles == null) return;
			// Establecemos la columna donde se empieza a pegar.
			columnagrid = TablaParaCopy.Columns.IndexOf(TablaParaCopy.CurrentCell.Column);
			filagrid = TablaParaCopy.Items.IndexOf(TablaParaCopy.CurrentCell.Item);
			// Creamos un objeto ConvertidorHora y otro ConvertidorItinerario
			Convertidores.ConvertidorHora cnvHora = new Convertidores.ConvertidorHora();
			Convertidores.ConvertidorItinerario cnvItinerario = new Convertidores.ConvertidorItinerario();
			// Iteramos por las filas del portapapeles.
			foreach (string[] fila in portapapeles) {
				if (TablaSeleccionada == Tablas.Lineas) {
					// Creamos un objeto Linea o reutilizamos el existente.
					Linea linea;
					if (filagrid < ListaLineas.Count) {
						linea = ListaLineas[filagrid];
						esnuevo = false;
					} else {
						linea = new Linea();
						esnuevo = true;
					}
					int columna = columnagrid;

					foreach (string texto in fila) {
						if (columna >= TablaParaCopy.Columns.Count) continue;
						while (TablaParaCopy.Columns[columna].Visibility == Visibility.Collapsed) {
							
							columna++;
						}
						switch (columna) {
							case 0: linea.Nombre = texto; break; // NOMBRE
							case 1: linea.Descripcion = texto; break; //DESCRIPCIÓN
						}
						columna++;
					}
					if (esnuevo) {
						ListaLineas.Add(linea);
					}
					filagrid++;
					HayCambios = true;
				}
				if (TablaSeleccionada == Tablas.Itinerarios) {
					// Creamos un objeto Itinerario o reutilizamos el existente.
					Itinerario itinerario;
					if (filagrid < LineaSeleccionada.ListaItinerarios.Count) {
						itinerario = LineaSeleccionada.ListaItinerarios[filagrid];
						esnuevo = false;
					} else {
						itinerario = new Itinerario();
						esnuevo = true;
					}
					int columna = columnagrid;

					foreach (string texto in fila) {
						if (columna >= TablaParaCopy.Columns.Count) continue;
						while (TablaParaCopy.Columns[columna].Visibility == Visibility.Collapsed) {
							columna++;
						}
						int i;
						switch (columna) {
							case 0: itinerario.Nombre = (decimal)cnvItinerario.ConvertBack(texto, null, null, null); break; // NOMBRE
							case 1: itinerario.Descripcion = texto; break; //DESCRIPCIÓN
							case 2: itinerario.TiempoReal = int.TryParse(texto, out i) ? i : 0; break; //TIEMPO REAL
							case 3: itinerario.TiempoPago = int.TryParse(texto, out i) ? i : 0; break; //TIEMPO PAGO
						}
						columna++;
					}
					if (esnuevo) {
						LineaSeleccionada.ListaItinerarios.Add(itinerario);
					}
					filagrid++;
					HayCambios = true;
				}
				if (TablaSeleccionada == Tablas.Paradas) {
					// Creamos un objeto Parada o reutilizamos el existente.
					Parada parada;
					if (filagrid < ItinerarioSeleccionado.ListaParadas.Count) {
						parada = ItinerarioSeleccionado.ListaParadas[filagrid];
						esnuevo = false;
					} else {
						parada = new Parada();
						esnuevo = true;
					}
					int columna = columnagrid;

					foreach (string texto in fila) {
						if (columna >= TablaParaCopy.Columns.Count) continue;
						while (TablaParaCopy.Columns[columna].Visibility == Visibility.Collapsed) {
							columna++;
						}
						int i;
						switch (columna) {
							case 0: parada.Orden = int.TryParse(texto, out i) ? i : 0; break; // ORDEN
							case 1: parada.Descripcion = texto; break; //DESCRIPCIÓN
							case 2: parada.Tiempo = (TimeSpan)cnvHora.ConvertBack(texto, null, null, null); break; //TIEMPO
						}
						columna++;
					}
					if (esnuevo) {
						ItinerarioSeleccionado.ListaParadas.Add(parada);
					}
					filagrid++;
					HayCambios = true;

				}
			}
		}

		#endregion


		#region BORRAR CELDAS
		private ICommand _cmdborrarceldas;
		public ICommand cmdBorrarCeldas {
			get {
				if (_cmdborrarceldas == null) _cmdborrarceldas = new RelayCommand(p => BorrarCeldas());
				return _cmdborrarceldas;
			}
		}

		private void BorrarCeldas() {
			DataGridCellInfo celda = TablaParaCopy.CurrentCell;
			if (TablaSeleccionada == Tablas.Lineas) {
				(celda.Item as Linea)?.BorrarValorPorHeader(celda.Column.Header.ToString());
			}
			if (TablaSeleccionada == Tablas.Itinerarios) {
				(celda.Item as Itinerario)?.BorrarValorPorHeader(celda.Column.Header.ToString());
			}
			if (TablaSeleccionada == Tablas.Paradas) {
				(celda.Item as Parada)?.BorrarValorPorHeader(celda.Column.Header.ToString());
			}
			HayCambios = true;
			
		}
		#endregion










	}
}
