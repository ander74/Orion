#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.DataModels;
using Orion.Properties;
using Orion.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Orion.ViewModels {


	public partial class GlobalVM {


		#region CAMPOS PRIVADOS
		//====================================================================================================
		#endregion
		//====================================================================================================


		#region GUARDAR CAMBIOS
		//Comando
		private ICommand _cmdguardarcambios;
		public ICommand cmdGuardarCambios {
			get {
				if (_cmdguardarcambios == null) _cmdguardarcambios = new RelayCommand(p => GuardarCambios(), p => PuedeGuardarCambios());
				return _cmdguardarcambios;
			}
		}

		// Puede Ejecutarse
		private bool PuedeGuardarCambios() {
			if (HayCambios) return true;
			return false;
		}

		// Ejecución del comando 
		// (Esta función es pública por el evento Closing de MainView. Ver más información en dicho evento.)
		public void GuardarCambios() {

			if ((int)CentroActual != App.Global.Configuracion.CentroInicial) {
				bool? resultado = mensajes.VerMensaje("¡¡ ATENCIÓN !!\n\nEstá apunto de guardar los datos de un centro que no es el suyo.\n\n" +
															 "¿Desea continuar?\n\nNOTA: Si continúa se podrían dar conflictor en Dropbox.",
															 "CONFLICTOS POTENCIALES EN DROPBOX",
															 true);

				if (resultado != true) return;

			}
			// Guardamos los datos de los ViewModels
			GraficosVM.GuardarTodo();
			ConductoresVM.GuardarTodo();
			CalendariosVM.GuardarTodo();
			LineasVM.GuardarTodo();
			OpcionesVM.GuardarTodo();
			// Guardamos las configuraciones.
			Configuracion.Guardar(ArchivoOpcionesConfiguracion);
			Convenio.Guardar(ArchivoOpcionesConvenio);
			if (_centroactual != Centros.Desconocido) PorCentro.Guardar(ArchivoOpcionesPorCentro);
			//App.Global.Configuracion.Save();
			//App.Global.Convenio2.Save();
			//PorCentro.Default.Save();
		}
		#endregion


		#region CAMBIAR CENTRO
		//Comando
		private ICommand _cmdcambiarcentro;
		public ICommand cmdCambiarCentro {
			get {
				if (_cmdcambiarcentro == null) _cmdcambiarcentro = new RelayCommand(p => CambiarCentro());
				return _cmdcambiarcentro;
			}
		}

		// Ejecución del comando
		private void CambiarCentro() {
			// Leemos el parámetro que indica si se está iniciando el programa.
			//bool iniciando = false;
			//if (parametro != null) iniciando = (bool)parametro;
			// Si hay cambios para guardar, pedimos guardarlos.
			if (PuedeGuardarCambios()) {
				bool? resultado = mensajes.VerMensaje("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?",
															 "NO SE HAN GUARDADO LOS CAMBIOS",
															 true, true);

				switch (resultado) {
					case null:
						return;
					case true:
						GuardarCambios();
						break;
				}
			}

			// Si se está iniciando y el centro inicial es conocido, se carga ese centro, si no, se pide un centro.
			//if (iniciando && App.Global.Configuracion.CentroInicial != (int)Centros.Desconocido) {
			//	CentroActual = (Centros)App.Global.Configuracion.CentroInicial;
			//} else {
				VentanaCentros ventana = new VentanaCentros();
				ventana.Centro = CentroActual;
				ventana.ShowDialog();
				CentroActual = ventana.Centro;
			//}
			//Reiniciamos todos los ViewModels para que reflejen el cambio de centro.
			GraficosVM.Reiniciar();
			ConductoresVM.Reiniciar();
			CalendariosVM.Reiniciar();
			LineasVM.Reiniciar();
			OpcionesVM.Reiniciar();
			ResumenAnualVM.Reiniciar();
			PropiedadCambiada("");
		}
		#endregion


		#region MOSTRAR AYUDA
		//Comando
		private ICommand _cmdmostrarayuda;
		public ICommand cmdMostrarAyuda {
			get {
				if (_cmdmostrarayuda == null) _cmdmostrarayuda = new RelayCommand(p => MostrarAyuda(p), p => PuedeMostrarAyuda(p));
				return _cmdmostrarayuda;
			}
		}

		// Puede ejecutar el comando
		public bool PuedeMostrarAyuda(object parametro) {
			if (VisibilidadAyuda == Visibility.Visible) return false;
			return true;
		}

		// Ejecución del comando
		private void MostrarAyuda(object parametro) {
			// Si no hay parámetro, salimos.
			if (parametro == null || !(parametro is string)) return;
			// Definimos el parámetro y lo evaluamos
			string ayuda = (string)parametro;
			// Mostramos el panel
			PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Index.html");
			if (ayuda == "Gráficos") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Graficos.html");
			if (ayuda == "Conductores") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Conductores.html");
			if (ayuda == "Calendarios") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Calendarios.html");
			if (ayuda == "Pijama") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Pijama.html");
			if (ayuda == "Líneas") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Lineas.html");
			if (ayuda == "Opciones") PaginaAyuda = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaAyuda, "Opciones.html");
			VisibilidadAyuda = Visibility.Visible;
		}
		#endregion


		#region CERRAR AYUDA
		//Comando
		private ICommand _cmdcerrarayuda;
		public ICommand cmdCerrarAyuda {
			get {
				if (_cmdcerrarayuda == null) _cmdcerrarayuda = new RelayCommand(p => CerrarAyuda(p));
				return _cmdcerrarayuda;
			}
		}

		// Ejecución del comando
		private void CerrarAyuda(object parametro) {
			if (parametro != null) {
				Button boton = (Button)parametro;
				ValignAyuda = VerticalAlignment.Bottom;
				TamañoAyuda = 400;
				boton.Content = "5";
			}
			// Cerramos el panel.
			VisibilidadAyuda = Visibility.Collapsed;
		}
		#endregion


		#region MOVER AYUDA
		//Comando
		private ICommand _cmdmoverayuda;
		public ICommand cmdMoverAyuda {
			get {
				if (_cmdmoverayuda == null) _cmdmoverayuda = new RelayCommand(p => MoverAyuda(p));
				return _cmdmoverayuda;
			}
		}

		// Ejecución del comando
		private void MoverAyuda(object parametro) {
			if (parametro == null) return;
			Button boton = (Button)parametro;
			if (TamañoAyuda == 400) {
				ValignAyuda = VerticalAlignment.Stretch;
				TamañoAyuda = double.NaN;
				boton.Content = "6";
			} else {
				ValignAyuda = VerticalAlignment.Bottom;
				TamañoAyuda = 400;
				boton.Content = "5";
			}
		}
		#endregion


		#region ATRAS AYUDA
		//Comando
		private ICommand _cmdatrasayuda;
		public ICommand cmdAtrasAyuda {
			get {
				if (_cmdatrasayuda == null) _cmdatrasayuda = new RelayCommand(p => AtrasAyuda(p));
				return _cmdatrasayuda;
			}
		}

		// Ejecución del comando
		private void AtrasAyuda(object parametro) {
			// Si no hay parámetro, salimos.
			if (parametro == null || !(parametro is WebBrowser)) return;
			// Definimos el parámetro y lo evaluamos
			WebBrowser navegador = (WebBrowser)parametro;
			// Vamos hacia atras en el navegador.
			if (navegador.CanGoBack) navegador.GoBack();
		}
		#endregion


		#region ADELANTE AYUDA
		//Comando
		private ICommand _cmdadelanteayuda;
		public ICommand cmdAdelanteAyuda {
			get {
				if (_cmdadelanteayuda == null) _cmdadelanteayuda = new RelayCommand(p => AdelanteAyuda(p));
				return _cmdadelanteayuda;
			}
		}

		// Ejecución del comando
		private void AdelanteAyuda(object parametro) {
			// Si no hay parámetro, salimos.
			if (parametro == null || !(parametro is WebBrowser)) return;
			// Definimos el parámetro y lo evaluamos
			WebBrowser navegador = (WebBrowser)parametro;
			// Vamos hacia atras en el navegador.
			if (navegador.CanGoForward) navegador.GoForward();
		}
		#endregion


		#region ACTUALIZAR AYUDA
		//Comando
		private ICommand _cmdactualizarayuda;
		public ICommand cmdActualizarAyuda {
			get {
				if (_cmdactualizarayuda == null) _cmdactualizarayuda = new RelayCommand(p => ActualizarAyuda(p));
				return _cmdactualizarayuda;
			}
		}

		// Ejecución del comando
		private void ActualizarAyuda(object parametro) {
			// Si no hay parámetro, salimos.
			if (parametro == null || !(parametro is WebBrowser)) return;
			// Definimos el parámetro y lo evaluamos
			WebBrowser navegador = (WebBrowser)parametro;
			// Vamos hacia atras en el navegador.
			navegador.Refresh();
		}

		#endregion


		#region MOSTRAR CALCULADORA
		//Comando
		private ICommand _cmdmostrarcalculadora;
		public ICommand cmdMostrarCalculadora {
			get {
				if (_cmdmostrarcalculadora == null) _cmdmostrarcalculadora = new RelayCommand(p => MostrarCalculadora());
				return _cmdmostrarcalculadora;
			}
		}

		// Ejecución del comando
		private void MostrarCalculadora() {

			// Creamos la ventana calculadora
			App.Calculadora = new VentanaCalculadora();
			// Creamos el ViewModel de la calculadora y asignamos la ubicacion.
			CalculadoraViewModel CalculadoraVM = new CalculadoraViewModel { Izquierda = Configuracion.LeftCalculadora, Arriba = Configuracion.TopCalculadora };
			// Asignamos el ViewModel
			App.Calculadora.DataContext = CalculadoraVM;
			// Asignamos el delegado de cierre de ventana.
			CalculadoraVM.SolicitarCierreVentana = App.Calculadora.Close;
			// Desactivamos el botón.
			Configuracion.BotonCalculadoraActivo = false;
			// Mostramos la calculadora, no modal.
			App.Calculadora.Show();
			
		}
		#endregion


		#region PANTALLA COMPLETA
		//Comando
		private ICommand _cmdpantallacompleta;
		public ICommand cmdPantallaCompleta {
			get {
				if (_cmdpantallacompleta == null) _cmdpantallacompleta = new RelayCommand(p => PantallaCompleta(p));
				return _cmdpantallacompleta;
			}
		}

		// Ejecución del comando
		private void PantallaCompleta(object parametro) {
			Window ventana = parametro as Window;
			if (ventana == null) return;
			if (ventana.WindowStyle == WindowStyle.ThreeDBorderWindow) {
				ventana.WindowState = WindowState.Normal;
				ventana.WindowStyle = WindowStyle.None;
				ventana.WindowState = WindowState.Maximized;
			} else {
				ventana.WindowStyle = WindowStyle.ThreeDBorderWindow;
			}
			
		}
		#endregion





	}
}
