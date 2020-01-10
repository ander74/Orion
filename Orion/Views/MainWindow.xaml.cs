#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Orion.Config;
using Orion.DataModels;
using Orion.ViewModels;

namespace Orion.Views {

    public partial class MainWindow : Window {

        //****************************************************************************************************
        // CONSTRUCTOR DE LA VENTANA PRINCIPAL
        //****************************************************************************************************
        public MainWindow() {
            InitializeComponent();

            // Solicitamos aceptar la licencia si no se ha aceptado nunca.
            if (!App.Global.Configuracion.LicenciaAceptada) {
                VentanaAcercaDeVM vm = new VentanaAcercaDeVM() { MostrarAceptarLicencia = true };
                VentanaAcercaDe ventanalicencia = new VentanaAcercaDe() { DataContext = vm };
                ventanalicencia.ShowDialog();
                if (vm.AceptarLicencia == false) {
                    MessageBox.Show("Debe Aceptar la licencia", "Atención");
                    this.Close();
                } else {
                    App.Global.Configuracion.LicenciaAceptada = true;
                }
            }

            // Establecemos la versión del ensamblado.
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            var fechaCompilacion = File.GetLastWriteTime(Path.Combine(Directory.GetCurrentDirectory(), "Orion.exe")).Date;
            DateTime fechaReferencia = new DateTime(2019, 12, 23); // Fecha del cambio de versión a la 1.1
            int buildVersion = Convert.ToInt32(Math.Round((fechaCompilacion - fechaReferencia).TotalDays, 0));
            //App.Global.TextoEstado = $"Versión {ver.Major}.{ver.Minor}.{ver.Build}";
            App.Global.TextoEstado = $"Versión {ver.Major}.{ver.Minor}.{buildVersion}";

            // Registramos el evento cerrar en el viewmodel.
            this.Closing += App.Global.CerrandoVentanaEventHandler;

            // Si hay contraseña de acceso...
            if (Utils.CodificaTexto("") != App.Global.Configuracion.ContraseñaDatos) {
                //Solicitamos la contraseña de acceso a las bases de datos.
                VentanaContraseña ventana = new VentanaContraseña();
                ventana.DataContext = App.Global;
                if (ventana.ShowDialog() == false) this.Close();
            }

            // Si hay que sincronizar con DropBox, se copian los archivos.
            //TODO: Actualizar con el método de autenticación.
            var clave = ApiKeys.DropboxAppKey;
            if (App.Global.Configuracion.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

            // Si hay que actualizar el programa, se actualiza.
            if (App.Global.Configuracion.ActualizarPrograma && !string.IsNullOrWhiteSpace(App.Global.Configuracion.CarpetaOrigenActualizar)) {
                string archivoOrigen = Path.Combine(App.Global.Configuracion.CarpetaOrigenActualizar, "OrionUpdate.exe");
                string archivoDestino = Path.Combine(Directory.GetCurrentDirectory(), "OrionUpdate.exe");
                DateTime origen = File.GetLastWriteTime(archivoOrigen);
                DateTime destino = File.GetLastWriteTime(archivoDestino);
                if (origen > destino && File.Exists(archivoOrigen)) {
                    File.Copy(archivoOrigen, archivoDestino, true);
                }
                archivoOrigen = Path.Combine(App.Global.Configuracion.CarpetaOrigenActualizar, "Orion.exe");
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
            string archivo = Utils.CombinarCarpetas(App.Global.Configuracion.CarpetaDatos, "Lineas.db3");
            // Si el archivo no existe, pedimos cambiar la carpeta de datos
            if (!File.Exists(archivo)) {
                if (MessageBox.Show("La carpeta de datos no está bien definida.\n\n¿Desea elegir otra carpeta?",
                                                   "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    System.Windows.Forms.FolderBrowserDialog dialogo = new System.Windows.Forms.FolderBrowserDialog();
                    dialogo.Description = "Ubicación Bases de Datos";
                    dialogo.RootFolder = Environment.SpecialFolder.MyComputer;
                    dialogo.SelectedPath = App.Global.Configuracion.CarpetaDatos;
                    dialogo.ShowNewFolderButton = true;
                    if (dialogo.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                        App.Global.Configuracion.CarpetaDatos = dialogo.SelectedPath;
                        App.Global.Configuracion.Guardar(App.Global.ArchivoOpcionesConfiguracion);
                    }
                }
            }

            // Establecemos el DataContext de la ventana.
            this.DataContext = App.Global;

            // Activamos el botón de la calculadora.
            //App.Global.Configuracion.BotonCalculadoraActivo = true;

            //Si hay que hacer una copia de seguridad, evaluar si hay que hacerla y si es así, hacerla.
            if (App.Global.Configuracion.CopiaAutomatica > 0 && App.Global.Configuracion.UltimaCopia != null) {
                TimeSpan tiempo = DateTime.Now.Subtract(App.Global.Configuracion.UltimaCopia);
                switch (App.Global.Configuracion.CopiaAutomatica) {
                    case 1: // Semanal
                        if (tiempo.TotalDays > 7) {
                            Respaldo.CopiaDatos();
                            App.Global.Configuracion.UltimaCopia = DateTime.Now;
                        }
                        break;
                    case 2: // Quincenal
                        if (tiempo.TotalDays > 14) {
                            Respaldo.CopiaDatos();
                            App.Global.Configuracion.UltimaCopia = DateTime.Now;
                        }
                        break;
                    case 3: // Mensual
                        if (tiempo.TotalDays > 30) {
                            Respaldo.CopiaDatos();
                            App.Global.Configuracion.UltimaCopia = DateTime.Now;
                        }
                        break;
                }
            }

        }


        //****************************************************************************************************
        // AL SELECCIONAR UNA PESTAÑA
        //****************************************************************************************************
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.Source is TabControl) {
                App.Global.VisibilidadAyuda = Visibility.Collapsed;
            }
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





    }
}
