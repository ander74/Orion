﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.DataModels;
using Orion.Models;
using Orion.Properties;
using Orion.Views;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using Microsoft.Win32;
using Orion.PrintModel;
using Orion.Convertidores;
using System.Threading.Tasks;
using Orion.Servicios;

namespace Orion.ViewModels {

	public partial class CalendariosViewModel {


		#region CAMPOS PRIVADOS
		//====================================================================================================
		private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
		private static ConvertidorSuperHora cnvSuperHora = new ConvertidorSuperHora();
		private static ConvertidorNumeroGrafico cnvNumeroGrafico = new ConvertidorNumeroGrafico();

		private ICommand _cmdretrocedemes;
		private ICommand _cmdavanzames;
		private ICommand _cmdborrarcalendario;
		private ICommand _cmddeshacerborrar;
		private ICommand _cmdborrarceldas;
		private ICommand _cmdquitarfiltro;
		private ICommand _cmdpegarcalendarios;
		private ICommand _cmdabrirpijama;
		private ICommand _cmdcerrarpijama;
		#endregion
		//====================================================================================================


		#region RETROCEDE MES
		//Comando
		public ICommand cmdRetrocedeMes {
			get {
				if (_cmdretrocedemes == null) _cmdretrocedemes = new RelayCommand(p => RetrocedeMes());
				return _cmdretrocedemes;
			}
		}

		// Ejecución del comando
		private void RetrocedeMes() {
			FechaActual = FechaActual.AddMonths(-1);
		}
		#endregion


		#region AVANZA MES
		//Comando
		public ICommand cmdAvanzaMes {
			get {
				if (_cmdavanzames == null) _cmdavanzames = new RelayCommand(p => AvanzaMes());
				return _cmdavanzames;
			}
		}

		// Ejecución del comando
		private void AvanzaMes() {
			FechaActual = FechaActual.AddMonths(1);
		}
		#endregion


		#region BORRAR CALENDARIO
		// Comando
		public ICommand cmdBorrarCalendario {
			get {
				if (_cmdborrarcalendario == null) _cmdborrarcalendario = new RelayCommand(p => BorrarCalendario(p), p => PuedeBorrarCalendario(p));
				return _cmdborrarcalendario;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarCalendario(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return false;
			if (tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Conductor" || tabla.CurrentCell.Item.GetType().Name != "Calendario") return false;
			return true;
		}

		// Ejecución del comando
		private void BorrarCalendario(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null || tabla.CurrentCell == null || tabla.SelectedCells == null) return;
			List<Calendario> lista = new List<Calendario>();
			foreach (DataGridCellInfo celda in tabla.SelectedCells) {
				if (celda.Column.Header.ToString() == "Conductor" && celda.Item.GetType().Name == "Calendario") {
					lista.Add((Calendario)celda.Item);
				}
			}
			foreach (Calendario c in lista) {
				_listaborrados.Add(c);
				_listacalendarios.Remove(c);
				HayCambios = true;
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
			foreach (Calendario calendario in _listaborrados) {
				if (calendario.Nuevo) {
					_listacalendarios.Add(calendario);
				} else {
					_listacalendarios.Add(calendario);
					calendario.Nuevo = false;
				}
				HayCambios = true;
			}
			_listaborrados.Clear();
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
			if (tabla == null || tabla.CurrentCell == null) return;
			DataGridCellInfo celda = tabla.CurrentCell;
			Calendario calendario = celda.Item as Calendario;
			if (calendario == null) return;
			calendario.BorrarValorPorHeader(celda.Column.Header.ToString());
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
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return false;
			foreach (DataGridColumn columna in tabla.Columns) {
				if (columna.SortDirection != null) return true;
			}
			if (VistaCalendarios != null && VistaCalendarios.Filter != null) return true;
			return false;
		}

		private void QuitarFiltro(object parametro) {
			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			if (VistaCalendarios != null && VistaCalendarios.SortDescriptions != null) {
				VistaCalendarios.SortDescriptions.Clear();
				foreach (DataGridColumn columna in tabla.Columns) {
					columna.SortDirection = null;
				}
			}
			if (VistaCalendarios != null) VistaCalendarios.Filter = null;
			TextoFiltros = "Ninguno";
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

		private bool PuedeAplicarFiltro() {
			if (ListaCalendarios.Count == 0) return false;
			return true;
		}

		private void AplicarFiltro(object parametro) {
			string filtro = parametro as string;
			if (filtro == null) return;
			switch (filtro) {
				case "Indefinidos":
					VistaCalendarios.Filter = (c) => { return (c as Calendario).ConductorIndefinido; };
					TextoFiltros = "Conductores Indefinidos";
					break;
				case "Eventuales":
					VistaCalendarios.Filter = (c) => { return !(c as Calendario).ConductorIndefinido; };
					TextoFiltros = "Conductores Eventuales";
					break;
				case "EventualesParcial":
					VistaCalendarios.Filter = (c) => {
						Calendario cal = c as Calendario;
						int dias = DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month);
						return (!cal.ConductorIndefinido && cal.ListaDias.Count(cc => cc.Grafico != 0) != dias);
					};
					TextoFiltros = "Conductores Eventuales Parcial";
					break;
			}
			BtFiltrarAbierto = false;
		}
		#endregion


		#region PEGAR CALENDARIOS
		public ICommand cmdPegarCalendarios {
			get {
				if (_cmdpegarcalendarios == null) _cmdpegarcalendarios = new RelayCommand(p => PegarCalendarios(p), p => PuedePegarCalendarios(p));
				return _cmdpegarcalendarios;
			}
		}

		private bool PuedePegarCalendarios(object parametro) {
			DataGrid tabla = parametro as DataGrid;
			if (tabla == null) return false;
			bool resultado = true;
			
			foreach (DataGridColumn columna in tabla.Columns) {
				if (columna.SortDirection != null) resultado = false;
			}
			if (VistaCalendarios != null && VistaCalendarios.Count < ListaCalendarios.Count) resultado = false;
			return resultado && tabla.CurrentCell != null & Clipboard.ContainsText();
		}

		private void PegarCalendarios(object parametro) {
			// Convertimos el parámetro pasado.
			DataGrid grid = parametro as DataGrid;
			if (grid == null || grid.CurrentCell == null) return;
			// Parseamos los datos del portapapeles y definimos las variables.
			List<string[]> portapapeles = Utils.parseClipboard();
			int columnagrid;
			int filagrid;
			bool esnuevo;
			// Si no hay datos, salimos.
			if (portapapeles == null) return;
			// Establecemos la columna donde se empieza a pegar.
			columnagrid = grid.Columns.IndexOf(grid.CurrentCell.Column);
			filagrid = grid.Items.IndexOf(grid.CurrentCell.Item);
			// Iteramos por las filas del portapapeles.
			foreach (string[] fila in portapapeles) {
				// Creamos un objeto Calendario o reutilizamos el existente.
				Calendario calendario;
				if (filagrid < ListaCalendarios.Count) {
					calendario = ListaCalendarios[filagrid];
					esnuevo = false;
				} else {
					calendario = new Calendario();
					calendario.Fecha = FechaActual;
					esnuevo = true;
				}
				int columna = columnagrid;

				foreach (string texto in fila) {
					while (grid.Columns[columna].Visibility == Visibility.Collapsed) {
						columna++;
					}
					switch (columna) {
						case 0: // Conductor.
							calendario.IdConductor = int.TryParse(texto, out int i) ? i : 0;
							break;
						default: // Dia x.
							ConvertidorNumeroGraficoCalendario convertidor = new ConvertidorNumeroGraficoCalendario();
							Tuple<int, int> combografico = (Tuple<int, int>)convertidor.ConvertBack(texto, null, null, null);
							calendario.ListaDias[columna - 1].Grafico = combografico.Item1;
							calendario.ListaDias[columna - 1].Codigo = combografico.Item2;
							break;
					}
					columna++;
				}
				if (esnuevo) {
					ListaCalendarios.Add(calendario);
				}
				filagrid++;
				HayCambios = true;
			}
		}

		#endregion


		#region ABRIR PIJAMA
		// COMANDO
		public ICommand cmdAbrirPijama {
			get {
				if (_cmdabrirpijama == null) _cmdabrirpijama = new RelayCommand(p => AbrirPijama(), p=> PuedeAbrirPijama());
				return _cmdabrirpijama;
			}
		}

		// SE PUEDE EJECUTAR
		private bool PuedeAbrirPijama() {
			if (CalendarioSeleccionado == null) return false;
			return true;
		}

		// EJECUCIÓN DEL COMANDO
		private void AbrirPijama() {

			Pijama = new Pijama.HojaPijama(CalendarioSeleccionado, Mensajes);
			VisibilidadTablaCalendarios = Visibility.Collapsed;

		}
		#endregion
		

		#region CERRAR PIJAMA
		// Comando
		public ICommand cmdCerrarPijama {
			get {
				if (_cmdcerrarpijama == null) _cmdcerrarpijama = new RelayCommand(p => CerrarPijama());
				return _cmdcerrarpijama;
			}
		}

		private void CerrarPijama() {
			Pijama = null;
			VisibilidadTablaCalendarios = Visibility.Visible;
		}
		#endregion


		#region ACTIVAR BOTON PANEL PIJAMA
		// COMANDO
		private ICommand _cmdactivarbotonpanelpijama;
		public ICommand cmdActivarBotonPanelPijama {
			get {
				if (_cmdactivarbotonpanelpijama == null) _cmdactivarbotonpanelpijama = new RelayCommand(p => ActivarBotonPanelPijama());
				return _cmdactivarbotonpanelpijama;
			}
		}

		// EJECUCIÓN DEL COMANDO
		private void ActivarBotonPanelPijama() {

			if (VisibilidadPanelPijama == Visibility.Visible) {
				VisibilidadPanelPijama = Visibility.Collapsed;
				TextoBotonPanelPijama = "<<";
			} else {
				VisibilidadPanelPijama = Visibility.Visible;
				TextoBotonPanelPijama = ">>";
			}

		}
		#endregion


		#region CREAR PDF PIJAMA
		// Comando
		private ICommand _cmdcrearpdfpijama;
		public ICommand cmdCrearPdfPijama {
			get {
				if (_cmdcrearpdfpijama == null) _cmdcrearpdfpijama = new RelayCommand(p => CrearPdfPijama());
				return _cmdcrearpdfpijama;
			}
		}

		private async void CrearPdfPijama() {

			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el nombre de archivo
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1}.pdf", FechaActual, Pijama.TextoTrabajador).Replace(":", "");
				string ruta = Informes.GetRutaArchivo(TiposInforme.Pijama, nombreArchivo);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.Pijama);
					await PijamaPrintModel.CrearPijamaEnPdf(libro, Pijama);
					Informes.ExportarLibroToPdf(libro, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("CalendariosCommands.CrearPdfPijama", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}
		}
		#endregion


		#region PIJAMAS EN PDF
		// Comando
		private ICommand _cmdpijamasenpdf;
		public ICommand cmdPijamasEnPDF {
			get {
				if (_cmdpijamasenpdf == null) _cmdpijamasenpdf = new RelayCommand(p => PijamasEnPdf(), p => PuedePijamasEnPdf());
				return _cmdpijamasenpdf;
			}
		}

		private bool PuedePijamasEnPdf() {
			if (VistaCalendarios == null) return false;
			return VistaCalendarios.Count > 0;
		}


		private async void PijamasEnPdf() {

			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el nombre de archivo
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM}", FechaActual);
				nombreArchivo += TextoFiltros == "Ninguno" ? " - Todos" : $" - {TextoFiltros}";
				string ruta = Informes.GetRutaArchivo(TiposInforme.Pijama, nombreArchivo);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.Pijama);
					await PijamaPrintModel.CrearTodosPijamasEnPdf(libro, VistaCalendarios);
					Informes.ExportarLibroToPdf(libro, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("CalendariosCommands.PijamasEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region  RESUMEN AÑO
		// Comando
		private ICommand _cmdmostrarresumenaño;
		public ICommand cmdMostrarResumenAño {
			get {
				if (_cmdmostrarresumenaño == null) _cmdmostrarresumenaño = new RelayCommand(p => MostrarResumenAño(), p => PuedeMostrarResumenAño());
				return _cmdmostrarresumenaño;
			}
		}

		private bool PuedeMostrarResumenAño() {
			return CalendarioSeleccionado != null;
		}

		private void MostrarResumenAño() {


		}
		#endregion


		#region DETECTAR FALLOS
		// Comando
		private ICommand _cmddetectarfallos;
		public ICommand cmdDetectarFallos {
			get {
				if (_cmddetectarfallos == null) _cmddetectarfallos = new RelayCommand(p => DetectarFallos());
				return _cmddetectarfallos;
			}
		}

		private void DetectarFallos() {

			BtAccionesAbierto = false;

			Task.Run(() => {
				try {
					// Mostramos la barra de progreso y le asignamos el texto
					App.Global.VisibilidadBarraProgreso = System.Windows.Visibility.Visible;
					App.Global.TextoProgreso = "Buscando errores...";
					double num = 1;
					foreach (Calendario cal in ListaCalendarios) {
						double valor = num / ListaCalendarios.Count * 100;
						App.Global.ValorBarraProgreso = valor;
						num++;
						Pijama.HojaPijama hoja = new Pijama.HojaPijama(cal, Mensajes);
						cal.Informe = hoja.GetInformeFallos();
					}
				} finally {
					App.Global.VisibilidadBarraProgreso = System.Windows.Visibility.Collapsed;
				}
			});

		}

		#endregion


		#region CAMBIAR JD POR DS
		// Comando
		private ICommand _cmdcambiarjdpords;
		public ICommand cmdCambiarJDPorDS {
			get {
				if (_cmdcambiarjdpords == null) _cmdcambiarjdpords = new RelayCommand(p => CambiarJDPorDS());
				return _cmdcambiarjdpords;
			}
		}

		private void CambiarJDPorDS() {

			BtAccionesAbierto = false;
			int numero = 0;
			foreach (object obj in VistaCalendarios) {
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				for (int i = 1; i < cal.ListaDias.Count - 1; i++) {
					if (cal.ListaDias[i].Grafico == -2 ) {
						if (cal.ListaDias[i - 1].Grafico > 0 && cal.ListaDias[i + 1].Grafico > 0) {
							cal.ListaDias[i].Grafico = -5;
							numero++;
						}
					}
				}
			}
			Mensajes.VerMensaje($"Se han cambiado {numero} JDs.", "JDs cambiados a DS");
		}
		#endregion


		#region REGULAR HORAS
		// COMANDO
		private ICommand _cmdregularhoras;
		public ICommand cmdRegularHoras {
			get {
				if (_cmdregularhoras == null) _cmdregularhoras = new RelayCommand(p => RegularHoras(p), p => PuedeRegularHoras());
				return _cmdregularhoras;
			}
		}

		// SE PUEDE EJECUTAR
		private bool PuedeRegularHoras() {
			if (CalendarioSeleccionado == null) return false;
			if (Pijama == null) return false;
			if ((Pijama.AcumuladasHastaAñoAnterior) < App.Global.Convenio.JornadaMedia) return false;
			return true;
		}

		// EJECUCIÓN DEL COMANDO
		private void RegularHoras(object parametro) {
			string p = parametro as string;
			if (p == null) p = "Año";

			int diasmes = DateTime.DaysInMonth(Pijama.Fecha.Year, Pijama.Fecha.Month);
			TimeSpan horasdisponibles = Pijama.AcumuladasHastaAñoAnterior;
			TimeSpan resto = new TimeSpan(horasdisponibles.Ticks % App.Global.Convenio.JornadaMedia.Ticks);
			decimal dcs = (horasdisponibles - resto).ToDecimal() / App.Global.Convenio.JornadaMedia.ToDecimal();
			string usadas = (string)cnvSuperHoraMixta.Convert(horasdisponibles - resto, null, null, null);
			string restantes = (string)cnvSuperHoraMixta.Convert(resto, null, null, null);
			string disponibles = (string)cnvSuperHoraMixta.Convert(horasdisponibles, null, null, null);
			string titulo = "Regulación de Horas";

			string mensaje = "ATENCIÓN.\n\n";
			mensaje += String.Format("Dispone de {0} horas para regular.\n\n", disponibles);
			mensaje += String.Format("Se van a regular {0} horas en {1:00} DCs.\n\n", usadas, dcs);
			mensaje += String.Format("Tras la conversión le sobrarán {0} horas.\n\n", restantes);
			mensaje += "¿Desea continuar?";

			if (Mensajes.VerMensaje(mensaje, titulo, true) == true) {

				RegulacionConductor regulacion = new RegulacionConductor();
				regulacion.Codigo = 2;
				regulacion.Descansos = (int)dcs;
				regulacion.Horas = new TimeSpan((horasdisponibles - resto).Ticks * -1);
				regulacion.IdConductor = Pijama.Trabajador.Id;
				regulacion.Fecha = new DateTime(Pijama.Fecha.Year, Pijama.Fecha.Month, 1);

				BdRegulacionConductor.InsertarRegulacion(regulacion);

				CerrarPijama();
			}

		}
		#endregion


		#region COBRAR HORAS
		// Comando
		private ICommand _cmdcobrarhoras;
		public ICommand cmdCobrarHoras {
			get {
				if (_cmdcobrarhoras == null) _cmdcobrarhoras = new RelayCommand(p => CobrarHoras(p), p => PuedeCobrarHoras(p));
				return _cmdcobrarhoras;
			}
		}

		// Se puede ejecutar
		private bool PuedeCobrarHoras(object parametro) {
			if (parametro == null) return false;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return false;
			if (tabla.CurrentCell.Column.Header.ToString() != "Día") return false;
			if (Pijama == null) return false;
			if (Pijama.AcumuladasHastaMes.Ticks <= 0) return false;
			return true;
		}

		// Ejecución del comando
		private void CobrarHoras(object parametro) {

			if (parametro == null) return;
			DataGrid tabla = (DataGrid)parametro;
			if (tabla.CurrentCell == null || tabla.CurrentCell.Column == null) return;
			if (tabla.CurrentCell.Column.Header.ToString() != "Día") return;
			int dia = ((Pijama.DiaPijama)tabla.CurrentCell.Item).Dia;

			// Creamos el view-model para la ventana.
			VentanaCobrarHorasVM contexto = new VentanaCobrarHorasVM(Mensajes);
			// Introducimos las opciones del view-model.
			if (Pijama != null) {
				contexto.HorasDisponibles = Pijama.AcumuladasHastaMes;
				contexto.HorasCobradas = Pijama.HorasCobradasAño;
			}
			// Creamos la ventana y le añadimos el view-model.
			VentanaCobrarHoras ventana = new VentanaCobrarHoras() { DataContext = contexto };

			if (ventana.ShowDialog() == true) {
				if (contexto.HorasACobrar.HasValue) {
					RegulacionConductor regulacion = new RegulacionConductor();
					regulacion.Fecha = new DateTime(FechaActual.Year, FechaActual.Month, dia);
					regulacion.IdConductor = Pijama.Trabajador.Id;
					regulacion.Motivo = "Horas Cobradas";
					regulacion.Codigo = 1;
					regulacion.Horas = new TimeSpan(contexto.HorasACobrar.Value.Ticks * -1);
					BdRegulacionConductor.InsertarRegulacion(regulacion);
				} else {
					Mensajes.VerMensaje("Debes escribir las horas que quieres cobrar.", "ATENCIÓN");
				}
			}
		}
		#endregion


		#region CALENDARIOS EN PDF
		// Comando
		private ICommand _cmdcalendariosenpdf;
		public ICommand cmdCalendariosEnPDF {
			get {
				if (_cmdcalendariosenpdf == null) _cmdcalendariosenpdf = new RelayCommand(p => CalendariosEnPDF(), p=> PuedeCalendariosEnPdf());
				return _cmdcalendariosenpdf;
			}
		}


		private bool PuedeCalendariosEnPdf() {
			if (VistaCalendarios == null) return false;
			return VistaCalendarios.Count > 0;
		}


		private async void CalendariosEnPDF() {

			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el archivo donde guardarlo.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1}", FechaActual, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.Calendarios, nombreArchivo);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.Calendarios);
					await CalendarioPrintModel.CrearCalendariosEnPdf(libro, VistaCalendarios, FechaActual);
					Informes.ExportarLibroToPdf(libro, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("CalendariosCommands.CalendariosEnPDF", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region CALENDARIOS EN PDF CON FALLOS
		// Comando
		private ICommand _cmdcalendariosenpdfconfallos;
		public ICommand cmdCalendariosEnPdfConFallos {
			get {
				if (_cmdcalendariosenpdfconfallos == null) _cmdcalendariosenpdfconfallos = 
						new RelayCommand(p => CalendariosEnPdfConFallos(), p => PuedeCalendariosEnPdfConFallos());
				return _cmdcalendariosenpdfconfallos;
			}
		}


		private bool PuedeCalendariosEnPdfConFallos() {
			if (VistaCalendarios == null) return false;
			return VistaCalendarios.Count > 0;
		}


		private async void CalendariosEnPdfConFallos() {
			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el archivo donde guardarlo.
				string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1} - Fallos", FechaActual, App.Global.CentroActual.ToString());
				if (TextoFiltros != "Ninguno") nombreArchivo += $" - ({TextoFiltros})";
				string ruta = Informes.GetRutaArchivo(TiposInforme.FallosCalendarios, nombreArchivo);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.FallosCalendarios);
					await CalendarioPrintModel.FallosEnCalendariosEnPdf(libro, VistaCalendarios, FechaActual);
					Informes.ExportarLibroToPdf(libro, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("CalendariosCommands.CalendariosEnPDFConFallos", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion


		#region ACCIONES POR LOTES
		// Comando
		private ICommand _cmdaccionesporlotes;
		public ICommand cmdAccionesPorLotes {
			get {
				if (_cmdaccionesporlotes == null) _cmdaccionesporlotes = new RelayCommand(p => AccionesPorLotes(), p => PuedeAccionesPorLotes());
				return _cmdaccionesporlotes;
			}
		}

		private bool PuedeAccionesPorLotes() {
			if (VistaCalendarios != null && VistaCalendarios.Count > 1) return true;
			return false;
		}

		private void AccionesPorLotes() {

			BtAccionesAbierto = false;

			if (VisibilidadAccionesLotes == Visibility.Visible) {
				VisibilidadAccionesLotes = Visibility.Collapsed;
			} else {
				VisibilidadAccionesLotes = Visibility.Visible;
			}

		}
		#endregion


		#region APLICAR ACCION LOTES
		// Comando
		private ICommand _cmdaplicaraccionlotes;
		public ICommand cmdAplicarAccionLotes {
			get {
				if (_cmdaplicaraccionlotes == null) _cmdaplicaraccionlotes = new RelayCommand(p => AplicarAccionLotes());
				return _cmdaplicaraccionlotes;
			}
		}

		private void AplicarAccionLotes() {

			// Si hemos elegido aplicar a todos los días, avisar antes de hacerlo.
			if (AccionesLotesVM.Grafico == -9999) {
				if (Mensajes.VerMensaje("Va a realizar una acción que afecta a todos los días de los calendarios.\n\n¿Desea continuar?", "ATENCIÓN", true) == false) return;
			}

			foreach (object obj in VistaCalendarios) {
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				foreach (DiaCalendario dia in cal.ListaDias) {
					if (dia.Grafico == 0) continue;
					if (dia.Dia >= AccionesLotesVM.DelDia && dia.Dia <= AccionesLotesVM.AlDia) {
						if (AccionesLotesVM.Grafico == -9999 || dia.Grafico == AccionesLotesVM.Grafico) {
							if (AccionesLotesVM.Codigo == 4 || (AccionesLotesVM.Codigo == dia.Codigo)) {
								switch (AccionesLotesVM.CodigoAccion) {
									case 0: // Exceso Jornada
										dia.ExcesoJornada = AccionesLotesVM.SumarValor ? dia.ExcesoJornada + AccionesLotesVM.Horas : AccionesLotesVM.Horas;
										break;
									case 1: // Facturado Paqueteria
										dia.FacturadoPaqueteria = AccionesLotesVM.SumarValor ? dia.FacturadoPaqueteria + AccionesLotesVM.Importe : AccionesLotesVM.Importe;
										break;
									case 2: // Limpieza
										dia.Limpieza = true;
										break;
									case 3: // Media Limpieza
										dia.Limpieza = null;
										break;
									case 4: // Quitar Limpieza
										dia.Limpieza = false;
										break;
									case 5: // Cambiar Número
										if (AccionesLotesVM.NuevoGrafico != 0)
											dia.Grafico = AccionesLotesVM.NuevoGrafico;
										if (AccionesLotesVM.NuevoCodigo != 4)
											dia.Codigo = AccionesLotesVM.NuevoCodigo;
										break;
									case 6: // Borrar Notas
										dia.Notas = "";
										break;
								}
								if (!String.IsNullOrWhiteSpace(AccionesLotesVM.Notas) && AccionesLotesVM.CodigoAccion != 7) {
									if (String.IsNullOrWhiteSpace(dia.Notas)) {
										dia.Notas = AccionesLotesVM.Notas;
									} else {
										dia.Notas = AccionesLotesVM.SumarValor ? dia.Notas + "\n" + AccionesLotesVM.Notas : AccionesLotesVM.Notas;
									}
								}
								HayCambios = true;
							}
						}
					}
				}
			}
			//TODO: Implementar.
			VisibilidadAccionesLotes = Visibility.Collapsed;

		}
		#endregion


		#region MARCAR ACCIONES LOTES

		// Comando
		private ICommand _cmdmarcaraccioneslotes;
		public ICommand cmdMarcarAccionesLotes {
			get {
				if (_cmdmarcaraccioneslotes == null) _cmdmarcaraccioneslotes = new RelayCommand(p => MarcarAccionesLotes());
				return _cmdmarcaraccioneslotes;
			}
		}


		// Ejecución del comando
		private void MarcarAccionesLotes() {

			foreach (object obj in VistaCalendarios) {
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				foreach (DiaCalendario dia in cal.ListaDias) {
					if (dia.Grafico == 0) continue;
					if (dia.Dia >= AccionesLotesVM.DelDia && dia.Dia <= AccionesLotesVM.AlDia) {
						if (AccionesLotesVM.Grafico == 0 || dia.Grafico == AccionesLotesVM.Grafico) {
							if (AccionesLotesVM.Codigo == 4 || (AccionesLotesVM.Codigo == dia.Codigo)) {
								dia.Resaltar = true;
							}
						}
					}
				}
			}
		}
		#endregion


		#region BORRAR ACCIONES POR LOTES
		// Comando
		private ICommand _cmdborraraccionlotes;
		public ICommand cmdBorrarAccionLotes {
			get {
				if (_cmdborraraccionlotes == null) _cmdborraraccionlotes = new RelayCommand(p => BorrarAccionLotes());
				return _cmdborraraccionlotes;
			}
		}

		private void BorrarAccionLotes() {

			AccionesLotesVM = new AccionesLotesCalendariosVM();
			foreach (object obj in VistaCalendarios) {
				Calendario cal = obj as Calendario;
				if (cal == null) continue;
				foreach (DiaCalendario dia in cal.ListaDias) {
					dia.Resaltar = false;
				}
			}

		}
		#endregion


		#region MOSTRAR PANELES PIJAMA
		// Comando
		private ICommand _cmdmostrarpanelespijama;
		public ICommand cmdMostrarPanelesPijama {
			get {
				if (_cmdmostrarpanelespijama == null) _cmdmostrarpanelespijama = new RelayCommand(p => MostrarPanelesPijama(p));
				return _cmdmostrarpanelespijama;
			}
		}

		private void MostrarPanelesPijama(object p) {

			string panel = p as string;
			if (panel == null) return;
			Visibility v = new Visibility();

			switch (panel){
				case "HorasMes":
					if (VisibilidadPanelHorasMes == Visibility.Visible) {
						VisibilidadPanelHorasMes = Visibility.Collapsed;
					} else {
						VisibilidadPanelHorasMes = Visibility.Visible;
					}
					break;
				case "DiasMes":
					if (VisibilidadPanelDiasMes == Visibility.Visible) {
						VisibilidadPanelDiasMes = Visibility.Collapsed;
					} else {
						VisibilidadPanelDiasMes = Visibility.Visible;
					}
					break;
				case "FindesMes":
					if (VisibilidadPanelFindesMes == Visibility.Visible) {
						VisibilidadPanelFindesMes = Visibility.Collapsed;
					} else {
						VisibilidadPanelFindesMes = Visibility.Visible;
					}
					break;
				case "DietasMes":
					if (VisibilidadPanelDietasMes == Visibility.Visible) {
						VisibilidadPanelDietasMes = Visibility.Collapsed;
					} else {
						VisibilidadPanelDietasMes = Visibility.Visible;
					}
					break;
				case "ResumenAño":
					if (VisibilidadPanelResumenAño == Visibility.Visible) {
						VisibilidadPanelResumenAño = Visibility.Collapsed;
					} else {
						VisibilidadPanelResumenAño = Visibility.Visible;
					}
					break;
				case "Todos":
					VisibilidadPanelHorasMes = Visibility.Visible;
					VisibilidadPanelDiasMes = Visibility.Visible;
					VisibilidadPanelFindesMes = Visibility.Visible;
					VisibilidadPanelDietasMes = Visibility.Visible;
					VisibilidadPanelResumenAño = Visibility.Visible;
					break;
			}

			if (v == Visibility.Visible) {
				v = Visibility.Collapsed;
			} else {
				v = Visibility.Visible;
			}

		}
		#endregion


		#region RECLAMAR
		// Comando
		private ICommand _cmdreclamar;
		public ICommand cmdReclamar {
			get {
				if (_cmdreclamar == null) _cmdreclamar = new RelayCommand(p => Reclamar(), p => PuedeReclamar());
				return _cmdreclamar;
			}
		}


		// Se puede ejecutar el comando
		private bool PuedeReclamar() {
			return Pijama != null;
		}

		// Ejecución del comando
		private async void Reclamar() {

			// Inicializamos la Ventana de reclamaciones y su ViewModel y se lo asignamos.
			VentanaReclamaciones Ventana = new VentanaReclamaciones();
			ReclamacionesViewModel ReclamacionVM = new ReclamacionesViewModel();
			Ventana.DataContext = ReclamacionVM;
			// Mostramos la ventana y seguimos si se pulsa el botón Aceptar.
			if (Ventana.ShowDialog() != true) return;
			// Definimos las varables a utilizar.
			int contador = 0;
			var conceptos = new string[17, 4];
			
			#region Horas
			if (ReclamacionVM.TrabajadasChecked) {
				conceptos[contador, 0] = "Horas Trabajadas";
				conceptos[contador, 1] = "'" + cnvSuperHora.Convert(ReclamacionVM.Trabajadas, null, null, null);
				conceptos[contador, 2] = "'" + cnvSuperHora.Convert(Pijama.Trabajadas, null, null, null);
				conceptos[contador, 3] = "'" + cnvSuperHora.Convert(Pijama.Trabajadas - ReclamacionVM.Trabajadas, null, null, null);
				contador++;
			}
			if (ReclamacionVM.AcumuladasChecked) {
				conceptos[contador, 0] = "Horas Acumuladas";
				conceptos[contador, 1] = "'" + cnvSuperHora.Convert(ReclamacionVM.Acumuladas, null, null, null);
				conceptos[contador, 2] = "'" + cnvSuperHora.Convert(Pijama.Acumuladas, null, null, null);
				conceptos[contador, 3] = "'" + cnvSuperHora.Convert(Pijama.Acumuladas - ReclamacionVM.Acumuladas, null, null, null);
				contador++;
			}
			if (ReclamacionVM.NocturnasChecked) {
				conceptos[contador, 0] = "Horas Nocturnas";
				conceptos[contador, 1] = "'" + cnvSuperHora.Convert(ReclamacionVM.Nocturnas, null, null, null);
				conceptos[contador, 2] = "'" + cnvSuperHora.Convert(Pijama.Nocturnas, null, null, null);
				conceptos[contador, 3] = "'" + cnvSuperHora.Convert(Pijama.Nocturnas - ReclamacionVM.Nocturnas, null, null, null);
				contador++;
			}
			if (ReclamacionVM.CobradasChecked) {
				conceptos[contador, 0] = "Horas Cobradas";
				conceptos[contador, 1] = "'" + cnvSuperHora.Convert(ReclamacionVM.Cobradas, null, null, null);
				conceptos[contador, 2] = "'" + cnvSuperHora.Convert(Pijama.HorasCobradas, null, null, null);
				conceptos[contador, 3] = "'" + cnvSuperHora.Convert(Pijama.HorasCobradas - -ReclamacionVM.Cobradas, null, null, null);
				contador++;
			}
			#endregion

			#region Importes
			if (ReclamacionVM.DietasChecked) {
				conceptos[contador, 0] = "Dietas";
				conceptos[contador, 1] = $"'{ReclamacionVM.Dietas:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.TotalDietas:0.00} ({Pijama.ImporteTotalDietas:0.00 €})";
				conceptos[contador, 3] = $"'{Pijama.ImporteTotalDietas - ReclamacionVM.Dietas:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.SabadosChecked) {
				conceptos[contador, 0] = "Plus Sábados";
				conceptos[contador, 1] = $"'{ReclamacionVM.Sabados:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusSabados:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusSabados - ReclamacionVM.Sabados:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.FestivosChecked) {
				conceptos[contador, 0] = "Plus Festivos";
				conceptos[contador, 1] = $"'{ReclamacionVM.Festivos:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusFestivos + Pijama.PlusDomingos:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusFestivos + Pijama.PlusDomingos - ReclamacionVM.Festivos:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.MenorDescansoChecked) {
				conceptos[contador, 0] = "Plus Menor Descanso";
				conceptos[contador, 1] = $"'{ReclamacionVM.MenorDescanso:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusMenorDescanso:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusMenorDescanso - ReclamacionVM.MenorDescanso:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.LimpiezaChecked) {
				conceptos[contador, 0] = "Plus Limpieza";
				conceptos[contador, 1] = $"'{ReclamacionVM.Limpieza:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusLimpieza:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusLimpieza - ReclamacionVM.Limpieza:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.PaqueteriaChecked) {
				conceptos[contador, 0] = "Plus Paquetería";
				conceptos[contador, 1] = $"'{ReclamacionVM.Paqueteria:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusPaqueteria:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusPaqueteria - ReclamacionVM.Paqueteria:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.NocturnidadChecked) {
				conceptos[contador, 0] = "Plus Nocturnidad";
				conceptos[contador, 1] = $"'{ReclamacionVM.Nocturnidad:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusNocturnidad:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusNocturnidad - ReclamacionVM.Nocturnidad:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.ViajeChecked) {
				conceptos[contador, 0] = "Plus Viaje";
				conceptos[contador, 1] = $"'{ReclamacionVM.Viaje:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusViaje:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusViaje - ReclamacionVM.Viaje:0.00 €}";
				contador++;
			}
			if (ReclamacionVM.NavidadChecked) {
				conceptos[contador, 0] = "Plus Navidad";
				conceptos[contador, 1] = $"'{ReclamacionVM.Navidad:0.00 €}";
				conceptos[contador, 2] = $"'{Pijama.PlusNavidad:0.00 €}";
				conceptos[contador, 3] = $"'{Pijama.PlusNavidad - ReclamacionVM.Navidad:0.00 €}";
				contador++;
			}

			#endregion

			#region Días
			if (ReclamacionVM.DescansosChecked) {
				conceptos[contador, 0] = "Descansos ordinarios";
				conceptos[contador, 1] = $"'{ReclamacionVM.Descansos:00}";
				conceptos[contador, 2] = $"'{Pijama.Descanso:00}";
				conceptos[contador, 3] = $"'{Pijama.Descanso - ReclamacionVM.Descansos:00}";
				contador++;
			}
			if (ReclamacionVM.VacacionesChecked) {
				conceptos[contador, 0] = "Vacaciones";
				conceptos[contador, 1] = $"'{ReclamacionVM.Vacaciones:00}";
				conceptos[contador, 2] = $"'{Pijama.Vacaciones:00}";
				conceptos[contador, 3] = $"'{Pijama.Vacaciones - ReclamacionVM.Vacaciones:00}";
				contador++;
			}
			if (ReclamacionVM.DescansosNoDisfrutadosChecked) {
				conceptos[contador, 0] = "Descansos no disfrutados";
				conceptos[contador, 1] = $"'{ReclamacionVM.DescansosNoDisfrutados:00}";
				conceptos[contador, 2] = $"'{Pijama.DescansosNoDisfrutados:00}";
				conceptos[contador, 3] = $"'{Pijama.DescansosNoDisfrutados - ReclamacionVM.DescansosNoDisfrutados:00}";
				contador++;
			}
			if (ReclamacionVM.EnfermoChecked) {
				conceptos[contador, 0] = "Días enfermedad";
				conceptos[contador, 1] = $"'{ReclamacionVM.Enfermo:00}";
				conceptos[contador, 2] = $"'{Pijama.Enfermo:00}";
				conceptos[contador, 3] = $"'{Pijama.Enfermo - ReclamacionVM.Enfermo:00}";
				contador++;
			}
			if (ReclamacionVM.DescansosSueltosChecked) {
				conceptos[contador, 0] = "Descansos sueltos";
				conceptos[contador, 1] = $"'{ReclamacionVM.DescansosSueltos:00}";
				conceptos[contador, 2] = $"'{Pijama.DescansoSuelto:00}";
				conceptos[contador, 3] = $"'{Pijama.DescansoSuelto - ReclamacionVM.DescansosSueltos:00}";
				contador++;
			}
			if (ReclamacionVM.DescansosCompensatoriosChecked) {
				conceptos[contador, 0] = "Descansos compensatorios";
				conceptos[contador, 1] = $"'{ReclamacionVM.DescansosCompensatorios:00}";
				conceptos[contador, 2] = $"'{Pijama.DescansoCompensatorio:00}";
				conceptos[contador, 3] = $"'{Pijama.DescansoCompensatorio - ReclamacionVM.DescansosCompensatorios:00}";
				contador++;
			}
			if (ReclamacionVM.PermisoChecked) {
				conceptos[contador, 0] = "Días de permiso";
				conceptos[contador, 1] = $"'{ReclamacionVM.Permiso:00}";
				conceptos[contador, 2] = $"'{Pijama.Permiso:00}";
				conceptos[contador, 3] = $"'{Pijama.Permiso - ReclamacionVM.Permiso:00}";
				contador++;
			}
			if (ReclamacionVM.LibreDisposicionChecked) {
				conceptos[contador, 0] = "Días de libre disposición";
				conceptos[contador, 1] = $"'{ReclamacionVM.LibreDisposicion:00}";
				conceptos[contador, 2] = $"'{Pijama.LibreDisposicionF6:00}";
				conceptos[contador, 3] = $"'{Pijama.LibreDisposicionF6 - ReclamacionVM.LibreDisposicion:00}";
				contador++;
			}
			if (ReclamacionVM.ComiteChecked) {
				conceptos[contador, 0] = "Días de comité";
				conceptos[contador, 1] = $"'{ReclamacionVM.Comite:00}";
				conceptos[contador, 2] = $"'{Pijama.Comite:00}";
				conceptos[contador, 3] = $"'{Pijama.Comite - ReclamacionVM.Comite:00}";
				contador++;
			}



			#endregion

			#region Gráficos
			foreach(ReclamacionesViewModel.DiaGrafico G in ReclamacionVM.ListaGraficos) {
				conceptos[contador, 0] = $"'Gráfico ({G.Dia:00}/{Pijama.Fecha.Month:00}/{Pijama.Fecha.Year:0000})";
				conceptos[contador, 1] = $"'{cnvNumeroGrafico.Convert(G.Grafico, null, null, null)}";
				conceptos[contador, 2] = $"'{cnvNumeroGrafico.Convert(Pijama.ListaDias[G.Dia-1].Grafico, null, null, null)}";
				contador++;
			}

			#endregion

			// Creamos el libro a usar.
			Workbook libro = null;
			try {
				// Activamos la barra de progreso.
				App.Global.IniciarProgreso("Creando PDF...");
				// Pedimos el archivo donde guardarlo.
				string nombreArchivo = String.Format("Reclamación {0:yyyy}-{0:MM} - {1:000}", Pijama.Fecha, Pijama.Trabajador.Id);
				string ruta = Informes.GetRutaArchivo(TiposInforme.Reclamacion, nombreArchivo);
				if (ruta != "") {
					libro = Informes.GetArchivoExcel(TiposInforme.Reclamacion);
					await ReclamacionPrintModel.CrearReclamacion(libro, conceptos, Pijama.Fecha, Pijama.Trabajador,
																 ReclamacionVM.FechaReclamacion, ReclamacionVM.Notas);
					Informes.ExportarLibroToPdf(libro, ruta);
				}
			} catch (Exception ex) {
				Mensajes.VerError("CalendariosCommands.Reclamar", ex);
			} finally {
				App.Global.FinalizarProgreso();
			}

		}
		#endregion




	}// Fin de la clase.
}
