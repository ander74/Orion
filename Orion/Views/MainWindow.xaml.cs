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
            DateTime fechaReferencia = new DateTime(2020, 11, 10); // Fecha del cambio de versión a la 1.2
            int buildVersion = Convert.ToInt32(Math.Round((fechaCompilacion - fechaReferencia).TotalDays, 0));
            //App.Global.TextoEstado = $"Versión {ver.Major}.{ver.Minor}.{ver.Build}";
            App.Global.TextoEstado = $"Versión {ver.Major}.{ver.Minor}.{ver.Build}";

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
            if (App.Global.Configuracion.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

            // Si hay que comprobar actualizaciones
            if (App.Global.Configuracion.ActualizarPrograma && !string.IsNullOrWhiteSpace(App.Global.Configuracion.CarpetaOrigenActualizar)) {
                var exeFile = Path.Combine(App.Global.Configuracion.CarpetaOrigenActualizar, "setup.exe");
                if (File.Exists(exeFile)) {
                    DateTime lastWrite = File.GetLastWriteTime(exeFile);
                    if (lastWrite > App.Global.Configuracion.LastWriteSetupFile) {
                        //string mensaje = "Hay una nueva versión de Orión.\n\n¿Desea actualizar ahora?";
                        //if (MessageBox.Show(mensaje, "Actualización", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes) {
                        //    App.ActualizarAlSalir = true;
                        //    App.Global.Configuracion.LastWriteSetupFile = lastWrite;
                        //    this.Close();
                        //}
                        string mensaje = "Hay una nueva versión de Orión.\n\nLa actualización automática esta temporalmente desactivada.\n\nCierra Orión y actualiza manualmente.";
                        MessageBox.Show(mensaje, "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                        App.Global.Configuracion.LastWriteSetupFile = lastWrite;
                    }
                }
            }


            // Establecemos el DataContext de la ventana.
            this.DataContext = App.Global;

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

            // SINCRONIZACION CON ONEDRIVE

            //if (App.Global.Configuracion.SincronizarEnDropbox) {
            //    try {
            //        App.Global.IniciarProgreso("Sincronizando con OneDrive...");
            //        await Task.Run(() => {
            //            if (File.Exists(App.Global.ArchivoOpcionesOneDrive)) {
            //                string datos = File.ReadAllText(App.Global.ArchivoOpcionesOneDrive, System.Text.Encoding.UTF8);
            //                App.Global.ServicioOneDrive.SetConfiguraciónCodificada(datos);
            //            }
            //            App.Global.ServicioOneDrive.AutorizarAsync().Wait();
            //            //TODO: Sincronizar los archivos.
            //        });
            //    } catch (Exception ex) {
            //        App.Global.Mensajes.VerError("MainWindow.Ventana_Loaded", ex);
            //    } finally {
            //        App.Global.FinalizarProgreso();
            //    }
            //}

            // FIN ONEDRIVE



        }


        //****************************************************************************************************
        // AL PULSAR LA ETIQUETA CENTRO DE TRABAJO
        //****************************************************************************************************
        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            ((GlobalVM)DataContext).cmdCambiarCentro.Execute(null);
        }


        //****************************************************************************************************
        // AL PULSAR EL BOTON CERRAR EN MODO PANTALLA COMPLETA
        //****************************************************************************************************
        private void BtClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
