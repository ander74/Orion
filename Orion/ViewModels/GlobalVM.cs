#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {

    using System;
    using System.ComponentModel;
    using System.Data.OleDb;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using Config;
    using DataModels;
    using Models;
    using Servicios;

    public partial class GlobalVM : NotifyBase, IDisposable {


        // ====================================================================================================
        #region  CAMPOS PRIVADOS
        // ====================================================================================================
        private Centros _centroactual;
        private string _textoestado = "Version Desconocida";
        private string _textodetalle;
        private MensajesServicio mensajes;
        private InformesServicio Informes;

        // ViewModels
        private GraficosViewModel _graficosvm;
        private ConductoresViewModel _conductoresvm;
        private CalendariosViewModel _calendariosvm;
        private LineasViewModel _lineasvm;
        private OpcionesViewModel _opcionesvm;
        private EstadisticasViewModel _estadisticasvm;
        private ResumenAnualViewModel _resumenanualvm;
        private FestivosViewModel _festivosvm;
        private ProgramadorViewModel _programador;
        private EstadisticasTurnosViewModel _estadisticasTurnos;



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR 
        // ====================================================================================================
        public GlobalVM() {

            // Cargamos la configuracion
            Configuracion.Cargar(ArchivoOpcionesConfiguracion);
            Convenio.Cargar(ArchivoOpcionesConvenio);

            // Si las carpetas de configuracion están en blanco, rellenarlas.
            if (Configuracion.CarpetaDatos == "") Configuracion.CarpetaDatos = Path.Combine(Directory.GetCurrentDirectory(), "Datos");
            if (Configuracion.CarpetaDropbox == "") Configuracion.CarpetaDropbox = Path.Combine(Directory.GetCurrentDirectory(), "Dropbox");
            if (Configuracion.CarpetaInformes == "") Configuracion.CarpetaInformes = Path.Combine(Directory.GetCurrentDirectory(), "Informes");
            if (Configuracion.CarpetaAyuda == "") Configuracion.CarpetaAyuda = Path.Combine(Directory.GetCurrentDirectory(), "Ayuda");
            if (Configuracion.CarpetaCopiasSeguridad == "") Configuracion.CarpetaCopiasSeguridad = Path.Combine(Directory.GetCurrentDirectory(), "CopiasSeguridad");
            if (Configuracion.CarpetaOrigenActualizar == "") Configuracion.CarpetaOrigenActualizar = App.RutaInicial;
            // Creamos los servicios
            mensajes = new MensajesServicio();
            Informes = new InformesServicio();

            // Asignamos el centro actual.
            CentroActual = (Centros)Configuracion.CentroInicial;// == Centros.Desconocido ? Centros.Bilbao : (Centros)Configuracion.CentroInicial;
                                                                // Activamos el botón de la calculadora.
            Configuracion.BotonCalculadoraActivo = true;

        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        //      public string GetCadenaConexion(Centros centro) {
        //	// Si no se ha establecido el centro, devolvemos null.
        //	if (centro == Centros.Desconocido) return null;
        //	// Definimos el archivo de base de datos
        //	string archivo = Utils.CombinarCarpetas(Configuracion.CarpetaDatos, centro.ToString() + ".accdb");
        //	// Si no existe el archivo, devolvemos null
        //	if (!File.Exists(archivo)) return null;
        //	// Establecemos la cadena de conexión
        //	string cadenaConexion = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;";
        //	cadenaConexion += "Data Source=" + archivo + ";";
        //	// Devolvemos la cadena de conexión.
        //	return cadenaConexion;
        //}


        public string GetCadenaConexion(Centros centro) {
            // Si no se ha establecido el centro, devolvemos null.
            if (centro == Centros.Desconocido) return null;
            // Definimos el archivo de base de datos
            string archivo = Utils.CombinarCarpetas(Configuracion.CarpetaDatos, centro.ToString() + ".accdb");
            // Si no existe el archivo, devolvemos null
            if (!File.Exists(archivo)) return null;
            // Establecemos la cadena de conexión
            OleDbConnectionStringBuilder cadenaConexionB = new OleDbConnectionStringBuilder {
                DataSource = archivo,
                Provider = "Microsoft.ACE.OLEDB.12.0",
                PersistSecurityInfo = false,
                OleDbServices = -1,
            };
            string cadenaConexion = cadenaConexionB.ToString();

            // Devolvemos la cadena de conexión.
            return cadenaConexion;
        }


        public void IniciarProgreso(string texto = "Trabajando...") {
            TextoProgreso = texto;
            ValorBarraProgreso = 0;
            App.Global.VisibilidadBarraProgreso = Visibility.Visible;
        }


        public void FinalizarProgreso() {
            App.Global.VisibilidadBarraProgreso = Visibility.Collapsed;
        }


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) Informes.Dispose();
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        public void CerrandoVentanaEventHandler(object sender, CancelEventArgs e) {

            if (PuedeGuardarCambios()) {
                bool? resultado = mensajes.VerMensaje("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?",
                                                             "NO SE HAN GUARDADO LOS CAMBIOS", true, true);

                switch (resultado) {
                    case null:
                        e.Cancel = true;
                        return;
                    case true:
                        GuardarCambios();
                        break;
                }
            }

            // Guardamos las configuraciones.
            Configuracion.Guardar(ArchivoOpcionesConfiguracion);
            Convenio.Guardar(ArchivoOpcionesConvenio);
            if (_centroactual != Centros.Desconocido) PorCentro.Guardar(ArchivoOpcionesPorCentro);
            //App.Global.Configuracion.Save();
            //App.Global.Convenio2.Save();
            //PorCentro.Default.Save();

            // Si hay que sincronizar con DropBox, se copian los archivos.
            if (Configuracion.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

            // Intentamos cerrar la calculadora, si existe.
            if (App.Calculadora != null) App.Calculadora.Close();

            // Apagamos los servicios
            if (Informes != null) Informes.Dispose();

            // Si hay que actualizar el programa, lanzamos el actualizador.
            if (App.ActualizarAlSalir) {
                Process.Start(@"OrionUpdate.exe", $"\"{Configuracion.CarpetaOrigenActualizar}\"");
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private string _contraseñadatos = "";
        public string ContraseñaDatos {
            get { return _contraseñadatos; }
            set {
                if (_contraseñadatos != value) {
                    _contraseñadatos = value;
                    PropiedadCambiada();
                }
            }
        }

        public Centros CentroActual {
            get { return _centroactual; }
            set {
                if (_centroactual != value) {
                    // Si el centro actual no es desconocido, guardamos los datos del centro actual.
                    if (_centroactual != Centros.Desconocido) PorCentro.Guardar(ArchivoOpcionesPorCentro);
                    _centroactual = value;
                    // Si el centro actual es el centro inicial, mostramos el botón azul, sino el rojo.
                    if ((int)_centroactual == Configuracion.CentroInicial) {
                        BotonGuardarRojo = Visibility.Collapsed;
                        BotonGuardarAzul = Visibility.Visible;
                    } else {
                        BotonGuardarAzul = Visibility.Collapsed;
                        BotonGuardarRojo = Visibility.Visible;
                    }
                    // Si el centro actual no es desconocido, cargamos las opciones por centro.
                    if (_centroactual != Centros.Desconocido) PorCentro.Cargar(ArchivoOpcionesPorCentro);
                    PropiedadCambiada();
                }
            }
        }


        public string CadenaConexion {
            get {
                return GetCadenaConexion(_centroactual);
            }
        }


        public string CadenaConexionLineas {
            get {
                return GetCadenaConexion(Centros.Lineas);
            }
        }



        private Visibility _botonguardarrojo = Visibility.Collapsed;
        public Visibility BotonGuardarRojo {
            get { return _botonguardarrojo; }
            set {
                if (_botonguardarrojo != value) {
                    _botonguardarrojo = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _botonguardarazul = Visibility.Visible;
        public Visibility BotonGuardarAzul {
            get { return _botonguardarazul; }
            set {
                if (_botonguardarazul != value) {
                    _botonguardarazul = value;
                    PropiedadCambiada();
                }
            }
        }


        public string TextoEstado {
            get { return _textoestado; }
            set {
                if (_textoestado != value) {
                    _textoestado = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _visibilidadbarraprogreso = Visibility.Collapsed;
        public Visibility VisibilidadBarraProgreso {
            get { return _visibilidadbarraprogreso; }
            set {
                if (_visibilidadbarraprogreso != value) {
                    _visibilidadbarraprogreso = value;
                    PropiedadCambiada();
                }
            }
        }


        public string TextoDetalle {
            get { return _textodetalle; }
            set {
                if (_textodetalle != value) {
                    _textodetalle = value;
                    PropiedadCambiada();
                }
            }
        }


        public bool HayCambios {
            get {
                if (GraficosVM.HayCambios) return true;
                if (ConductoresVM.HayCambios) return true;
                if (CalendariosVM.HayCambios) return true;
                if (LineasVM.HayCambios) return true;
                if (OpcionesVM.HayCambios) return true;
                if (FestivosVM.HayCambios) return true;
                return false;
            }
        }


        private double _tamañoayuda = 400;
        public double TamañoAyuda {
            get { return _tamañoayuda; }
            set {
                if (_tamañoayuda != value) {
                    _tamañoayuda = value;
                    PropiedadCambiada();
                }
            }
        }


        private VerticalAlignment _valignayuda = VerticalAlignment.Bottom;
        public VerticalAlignment ValignAyuda {
            get { return _valignayuda; }
            set {
                if (_valignayuda != value) {
                    _valignayuda = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _paginaayuda = "about:blank";
        public string PaginaAyuda {
            get { return _paginaayuda; }
            set {
                if (_paginaayuda != value) {
                    _paginaayuda = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _visibilidadayuda = Visibility.Collapsed;
        public Visibility VisibilidadAyuda {
            get { return _visibilidadayuda; }
            set {
                if (_visibilidadayuda != value) {
                    _visibilidadayuda = value;
                    PropiedadCambiada();
                }
            }
        }


        private Visibility _visibilidadprogramador = Visibility.Collapsed;
        public Visibility VisibilidadProgramador {
            get { return _visibilidadprogramador; }
            set { SetValue(ref _visibilidadprogramador, value); }
        }



        private bool estadisticasTurnosActiva;
        public bool EstadisticasTurnosActiva {
            get => estadisticasTurnosActiva;
            set => SetValue(ref estadisticasTurnosActiva, value);
        }



        //====================================================================================================
        // VENTANA PROGRESO
        //====================================================================================================

        private Visibility _visibilidadprogreso = Visibility.Collapsed;
        public Visibility VisibilidadProgreso {
            get { return _visibilidadprogreso; }
            set {
                if (_visibilidadprogreso != value) {
                    _visibilidadprogreso = value;
                    PropiedadCambiada();
                }
            }
        }


        private double _valorbarraprogreso;
        public double ValorBarraProgreso {
            get { return _valorbarraprogreso; }
            set {
                if (_valorbarraprogreso != value) {
                    _valorbarraprogreso = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _textoprogreso;
        public string TextoProgreso {
            get { return _textoprogreso; }
            set {
                if (_textoprogreso != value) {
                    _textoprogreso = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region VIEWMODELS
        // ====================================================================================================

        public OpcionesViewModel OpcionesVM {
            get {
                if (_opcionesvm == null) _opcionesvm = new OpcionesViewModel(mensajes);
                return _opcionesvm;
            }
        }


        public GraficosViewModel GraficosVM {
            get {
                if (_graficosvm == null) _graficosvm = new GraficosViewModel(mensajes, Informes);
                return _graficosvm;
            }
        }


        public ConductoresViewModel ConductoresVM {
            get {
                if (_conductoresvm == null) _conductoresvm = new ConductoresViewModel(mensajes);
                return _conductoresvm;
            }
        }


        public CalendariosViewModel CalendariosVM {
            get {
                if (_calendariosvm == null) _calendariosvm = new CalendariosViewModel(mensajes, Informes);
                return _calendariosvm;
            }
        }


        public LineasViewModel LineasVM {
            get {
                if (_lineasvm == null) _lineasvm = new LineasViewModel(mensajes);
                return _lineasvm;
            }
        }


        public EstadisticasViewModel EstadisticasVM {
            get {
                if (_estadisticasvm == null) _estadisticasvm = new EstadisticasViewModel(mensajes, Informes);
                return _estadisticasvm;
            }
        }

        public ResumenAnualViewModel ResumenAnualVM {
            get {
                if (_resumenanualvm == null) _resumenanualvm = new ResumenAnualViewModel(mensajes, Informes);
                return _resumenanualvm;
            }
        }

        public FestivosViewModel FestivosVM {
            get {
                if (_festivosvm == null) _festivosvm = new FestivosViewModel(mensajes);
                return _festivosvm;
            }
        }


        public ProgramadorViewModel ProgramadorVM {
            get {
                if (_programador == null) _programador = new ProgramadorViewModel(mensajes);
                return _programador;
            }
        }


        public EstadisticasTurnosViewModel EstadisticasTurnosVM {
            get {
                if (_estadisticasTurnos == null) _estadisticasTurnos = new EstadisticasTurnosViewModel(mensajes);
                return _estadisticasTurnos;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region OPCIONES
        // ====================================================================================================


        public string ArchivoOpcionesConfiguracion {
            get {
                return "Config.json";
            }
        }


        public string ArchivoOpcionesConvenio {
            get {
                return "Convenio.json";
            }
        }


        public string ArchivoOpcionesPorCentro {
            get {
                return $"PorCentro{CentroActual}.json";
            }
        }


        private OpcionesConfiguracion _configuracion = new OpcionesConfiguracion();
        public OpcionesConfiguracion Configuracion {
            get { return _configuracion; }
            set {
                if (_configuracion != value) {
                    _configuracion = value;
                    PropiedadCambiada();
                }
            }
        }


        private OpcionesConvenio _convenio = new OpcionesConvenio();
        public OpcionesConvenio Convenio {
            get { return _convenio; }
            set {
                if (_convenio != value) {
                    _convenio = value;
                    PropiedadCambiada();
                }
            }
        }


        private OpcionesPorCentro _porcentro = new OpcionesPorCentro();
        public OpcionesPorCentro PorCentro {
            get { return _porcentro; }
            set {
                if (_porcentro != value) {
                    _porcentro = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


    } //Fin de la clase.
}
