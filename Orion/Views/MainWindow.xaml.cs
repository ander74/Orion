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
using Orion.ViewModels;
using System;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Orion.Views {

	public partial class MainWindow : Window {


		//****************************************************************************************************
		// CONSTRUCTOR DE LA VENTANA PRINCIPAL
		//****************************************************************************************************
		public MainWindow() {
			InitializeComponent();

			// Establecemos la versión del ensamblado.
			Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			App.Global.TextoEstado = $"Versión {ver.Major}.{ver.Minor}.{ver.Build}";

			// Registramos el evento cerrar en el viewmodel.
			this.Closing += App.Global.CerrandoVentanaEventHandler;

			// Si hay contraseña de acceso...
			if (Utils.CodificaTexto("") != Settings.Default.ContraseñaDatos) {
				//Solicitamos la contraseña de acceso a las bases de datos.
				VentanaContraseña ventana = new VentanaContraseña();
				ventana.DataContext = App.Global;
				if (ventana.ShowDialog() == false) this.Close();
			}

			// Si hay que sincronizar con DropBox, se copian los archivos.
			if (Settings.Default.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

			// Si hay que actualizar el programa, se actualiza.
			if (Settings.Default.ActualizarPrograma && !string.IsNullOrWhiteSpace(Settings.Default.CarpetaOrigenActualizar)) {
				string archivoOrigen = Path.Combine(Settings.Default.CarpetaOrigenActualizar, "OrionUpdate.exe");
				string archivoDestino = Path.Combine(Directory.GetCurrentDirectory(), "OrionUpdate.exe");
				DateTime origen = File.GetLastWriteTime(archivoOrigen);
				DateTime destino = File.GetLastWriteTime(archivoDestino);
				if (origen > destino && File.Exists(archivoOrigen)) {
					File.Copy(archivoOrigen, archivoDestino, true);
				}
				archivoOrigen = Path.Combine(Settings.Default.CarpetaOrigenActualizar, "Orion.exe");
				archivoDestino = Path.Combine(Directory.GetCurrentDirectory(), "Orion.exe");
				origen = File.GetLastWriteTime(archivoOrigen);
				destino = File.GetLastWriteTime(archivoDestino);
				if (origen > destino) {
					// Necesita actualizar
					string mensaje = "Hay una nueva versión de Orión.\n\n¿Desea actualizar ahora?";
					if (MessageBox.Show(mensaje, "Actualización", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
						App.ActualizarAlSalir = true;
						this.Close();
					}
				}
			}

			// Definimos el archivo de base de datos
			string archivo = Utils.CombinarCarpetas(Settings.Default.CarpetaDatos, "Lineas.accdb");
			// Si el archivo no existe, pedimos cambiar la carpeta de datos
			if (!File.Exists(archivo)) {
				if (MessageBox.Show("La carpeta de datos no está bien definida.\n\n¿Desea elegir otra carpeta?",
												   "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
					System.Windows.Forms.FolderBrowserDialog dialogo = new System.Windows.Forms.FolderBrowserDialog();
					dialogo.Description = "Ubicación Bases de Datos";
					dialogo.RootFolder = Environment.SpecialFolder.MyComputer;
					dialogo.SelectedPath = Settings.Default.CarpetaDatos;
					dialogo.ShowNewFolderButton = true;
					if (dialogo.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
						Settings.Default.CarpetaDatos = dialogo.SelectedPath;
						Settings.Default.Save();
					}
				} 
			}

			// Establecemos el DataContext de la ventana.
			DataContext = App.Global;

			// Activamos el botón de la calculadora.
			//Settings.Default.BotonCalculadoraActivo = true;

			//Si hay que hacer una copia de seguridad, evaluar si hay que hacerla y si es así, hacerla.
			if (Settings.Default.CopiaAutomatica > 0 && Settings.Default.UltimaCopia != null) {
				TimeSpan tiempo = DateTime.Now.Subtract(Settings.Default.UltimaCopia);
				switch (Settings.Default.CopiaAutomatica) {
					case 1: // Semanal
						if (tiempo.TotalDays > 7) {
							Respaldo.CopiaDatos();
							Settings.Default.UltimaCopia = DateTime.Now;
						}
						break;
					case 2: // Quincenal
						if (tiempo.TotalDays > 14) {
							Respaldo.CopiaDatos();
							Settings.Default.UltimaCopia = DateTime.Now;
						}
						break;
					case 3: // Mensual
						if (tiempo.TotalDays > 30) {
							Respaldo.CopiaDatos();
							Settings.Default.UltimaCopia = DateTime.Now;
						}
						break;
				}
			}

		}


		//****************************************************************************************************
		// AL SELECCIONAR UNA PESTAÑA
		//****************************************************************************************************
		private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			PanelAyuda.Visibility = Visibility.Collapsed;
			
		}

		//****************************************************************************************************
		// AL CARGAR LA VENTANA
		//****************************************************************************************************
		private void Ventana_Loaded(object sender, RoutedEventArgs e) {
			
		}


		//****************************************************************************************************
		// AL PULSAR LA ETIQUETA CENTRO DE TRABAJO
		//****************************************************************************************************
		private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			((GlobalVM)DataContext).cmdCambiarCentro.Execute(null);
		}


		//****************************************************************************************************
		// AL CERRARSE LA APLICACIÓN
		// =========================
		//
		// Esta función viola el patrón MVVM, pero dado lo complejo que es pasar los argumentos del evento
		// a un comando MVVM, he preferido mantener esta función.
		//
		//****************************************************************************************************
		//private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

		//if (App.Global.PuedeGuardarCambios()) {
		//	MessageBoxResult resultado = MessageBox.Show("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?",
		//												 "NO SE HAN GUARDADO LOS CAMBIOS",
		//												 MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

		//	switch (resultado) {
		//		case MessageBoxResult.Cancel:
		//			e.Cancel = true;
		//			break;
		//		case MessageBoxResult.Yes:
		//			App.Global.GuardarCambios();
		//			break;
		//	}
		//}

		//// Guardamos las configuraciones.
		//Settings.Default.Save();
		//Convenio.Default.Save();
		//PorCentro.Default.Save();

		//// Si hay que sincronizar con DropBox, se copian los archivos.
		//if (Settings.Default.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

		//// Intentamos cerrar la calculadora, si existe.
		//if (App.Calculadora != null) App.Calculadora.Close();

		//}




	}
}
