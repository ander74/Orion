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
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using Config;
    using DataModels;
    using Models;
    using Orion.Interfaces;
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
        private IFileService fileService;

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

            // Definimos la carpeta 'Configuracion Orion' en la carpeta 'Documentos' del usuario. Si no existe, la creamos.
            CarpetaConfiguracion = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Configuracion Orion");
            if (!Directory.Exists(CarpetaConfiguracion)) Directory.CreateDirectory(CarpetaConfiguracion);

            // Definimos la carpeta 'Datos Orion' en la carpeta 'Documentos' del usuario. Si no existe, la creamos.
            CarpetaDatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Datos Orion");
            if (!Directory.Exists(CarpetaDatos)) Directory.CreateDirectory(CarpetaDatos);

            // ----------------------------------------------------------------------------------------------------------------------------
            // Si los archivos no existen y sí existen en la raiz, copiarlos.
            //
            // El siguiente bloque se puede eliminar una vez que ya llevemos un tiempo usando las nuevas ubicaciones de los archivos.
            // ----------------------------------------------------------------------------------------------------------------------------
            if (!File.Exists(ArchivoOpcionesConfiguracion) && File.Exists("Config.json"))
                File.Copy("Config.json", ArchivoOpcionesConfiguracion);
            if (!File.Exists(ArchivoOpcionesConvenio) && File.Exists("Convenio.json"))
                File.Copy("Convenio.json", ArchivoOpcionesConvenio);
            // ARRASATE
            if (!File.Exists(GetArchivoPorCentroPath(Centros.Arrasate)) && File.Exists($"PorCentro{Centros.Arrasate}.json"))
                File.Copy($"PorCentro{Centros.Arrasate}.json", GetArchivoPorCentroPath(Centros.Arrasate));
            // DONOSTI
            if (!File.Exists(GetArchivoPorCentroPath(Centros.Donosti)) && File.Exists($"PorCentro{Centros.Donosti}.json"))
                File.Copy($"PorCentro{Centros.Donosti}.json", GetArchivoPorCentroPath(Centros.Donosti));
            // BILBAO
            if (!File.Exists(GetArchivoPorCentroPath(Centros.Bilbao)) && File.Exists($"PorCentro{Centros.Bilbao}.json"))
                File.Copy($"PorCentro{Centros.Bilbao}.json", GetArchivoPorCentroPath(Centros.Bilbao));
            // VITORIA
            if (!File.Exists(GetArchivoPorCentroPath(Centros.Vitoria)) && File.Exists($"PorCentro{Centros.Vitoria}.json"))
                File.Copy($"PorCentro{Centros.Vitoria}.json", GetArchivoPorCentroPath(Centros.Vitoria));
            // ----------------------------------------------------------------------------------------------------------------------------

            // Cargamos la configuracion
            Configuracion.Cargar(ArchivoOpcionesConfiguracion);
            Convenio.Cargar(ArchivoOpcionesConvenio);

            // Si las carpetas de configuracion están en blanco, crearlas y rellenarlas.
            if (Configuracion.CarpetaDatos == "") Configuracion.CarpetaDatos = CreateAndGetCarpetaEnDatos("Datos");
            if (Configuracion.CarpetaDropbox == "") Configuracion.CarpetaDropbox = CreateAndGetCarpetaEnDatos("Dropbox");
            if (Configuracion.CarpetaInformes == "") Configuracion.CarpetaInformes = CreateAndGetCarpetaEnDatos("Informes");
            if (Configuracion.CarpetaAyuda == "") Configuracion.CarpetaAyuda = CreateAndGetCarpetaEnDatos("Ayuda");
            if (Configuracion.CarpetaCopiasSeguridad == "") Configuracion.CarpetaCopiasSeguridad = CreateAndGetCarpetaEnDatos("CopiasSeguridad");
            if (Configuracion.CarpetaOrigenActualizar == "") Configuracion.CarpetaOrigenActualizar = App.RutaInicial;
            // Creamos los servicios
            mensajes = new MensajesServicio();
            Informes = new InformesServicio();
            fileService = new FileService();

            // Asignamos el centro actual.
            CentroActual = (Centros)Configuracion.CentroInicial;

            // Activamos el botón de la calculadora.
            Configuracion.BotonCalculadoraActivo = true;

        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

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


        public string GetCadenaConexionSQL(Centros centro) {
            // Si no se ha establecido el centro, devolvemos null.
            if (centro == Centros.Desconocido) return null;
            // Definimos el archivo de base de datos
            string archivo = Utils.CombinarCarpetas(Configuracion.CarpetaDatos, centro.ToString() + ".db3");
            //if (!File.Exists(archivo)) SQLiteConnection.CreateFile(archivo); //TODO: Comprobar que esto funciona bien.
            // Establecemos la cadena de conexión
            SQLiteConnectionStringBuilder cadenaConexionBuilder = new SQLiteConnectionStringBuilder {
                DataSource = archivo,
            };
            string cadenaConexion = cadenaConexionBuilder.ToString();
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


        public string GetArchivoPorCentroPath(Centros centro) {
            return Path.Combine(CarpetaConfiguracion, $"PorCentro{centro}.json");
        }


        public string CreateAndGetCarpetaEnDatos(string carpeta) {
            var resultado = Path.Combine(CarpetaDatos, carpeta);
            if (!Directory.Exists(resultado)) Directory.CreateDirectory(resultado);
            return resultado;
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
                        EnOtroCentro = false;
                    } else {
                        EnOtroCentro = true;
                    }
                    // Si el centro actual no es desconocido...
                    if (_centroactual != Centros.Desconocido) {
                        // Cargamos las opciones por centro.
                        PorCentro.Cargar(ArchivoOpcionesPorCentro);
                        // Inicializamos el centro en la base de datos SQL
                        repository = new OrionRepository(CadenaConexionSQL);
                    }
                    PropiedadCambiada();
                }
            }
        }


        private bool enOtroCentro;
        public bool EnOtroCentro {
            get => enOtroCentro;
            set => SetValue(ref enOtroCentro, value);
        }


        public string CadenaConexion {
            get {
                //return GetCadenaConexion(_centroactual);
                // Si volvemos a Access, recuperar la línea anterior y eliminar la siguiente.
                return GetCadenaConexionSQL(_centroactual);
            }
        }


        public string CadenaConexionSQL {
            get {
                return GetCadenaConexionSQL(_centroactual);
            }
        }


        public string CadenaConexionLineas {
            get {
                //return GetCadenaConexion(Centros.Lineas);
                // Si volvemos a Access, recuperar la línea anterior y eliminar la siguiente.
                return GetCadenaConexionSQL(Centros.Lineas);
            }
        }


        public string CadenaConexionLineasSQL {
            get {
                return GetCadenaConexionSQL(Centros.Lineas);
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
        // REPOSITORIO DE DATOS
        //====================================================================================================

        private OrionRepository repository;
        public OrionRepository Repository {
            get {
                if (repository == null) repository = new OrionRepository(CadenaConexionSQL);
                return repository;
            }
        }



        private LineasRepository lineasRepo;
        public LineasRepository LineasRepo {
            get {
                if (lineasRepo == null) lineasRepo = new LineasRepository(CadenaConexionLineasSQL);
                return lineasRepo;
            }
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
                _opcionesvm = _opcionesvm ?? new OpcionesViewModel(mensajes);
                return _opcionesvm;
            }
        }


        public GraficosViewModel GraficosVM {
            get {
                _graficosvm = _graficosvm ?? new GraficosViewModel(mensajes, Informes, fileService);
                return _graficosvm;
            }
        }


        public ConductoresViewModel ConductoresVM {
            get {
                _conductoresvm = _conductoresvm ?? new ConductoresViewModel(mensajes);
                return _conductoresvm;
            }
        }


        public CalendariosViewModel CalendariosVM {
            get {
                _calendariosvm = _calendariosvm ?? new CalendariosViewModel(mensajes, Informes, fileService);
                return _calendariosvm;
            }
        }


        public LineasViewModel LineasVM {
            get {
                _lineasvm = _lineasvm ?? new LineasViewModel(mensajes);
                return _lineasvm;
            }
        }


        public EstadisticasViewModel EstadisticasVM {
            get {
                _estadisticasvm = _estadisticasvm ?? new EstadisticasViewModel(mensajes, Informes);
                return _estadisticasvm;
            }
        }

        public ResumenAnualViewModel ResumenAnualVM {
            get {
                _resumenanualvm = _resumenanualvm ?? new ResumenAnualViewModel(mensajes, Informes);
                return _resumenanualvm;
            }
        }

        public FestivosViewModel FestivosVM {
            get {
                _festivosvm = _festivosvm ?? new FestivosViewModel(mensajes);
                return _festivosvm;
            }
        }


        public ProgramadorViewModel ProgramadorVM {
            get {
                _programador = _programador ?? new ProgramadorViewModel(mensajes);
                return _programador;
            }
        }


        public EstadisticasTurnosViewModel EstadisticasTurnosVM {
            get {
                _estadisticasTurnos = _estadisticasTurnos ?? new EstadisticasTurnosViewModel(mensajes);
                return _estadisticasTurnos;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region OPCIONES
        // ====================================================================================================


        public string CarpetaConfiguracion { get; set; }


        public string CarpetaDatos { get; set; }


        public string ArchivoOpcionesConfiguracion {
            get {
                return Path.Combine(CarpetaConfiguracion, "Config.json");
            }
        }


        public string ArchivoOpcionesConvenio {
            get {
                return Path.Combine(CarpetaConfiguracion, "Convenio.json");
            }
        }


        public string ArchivoOpcionesPorCentro {
            get {
                return GetArchivoPorCentroPath(CentroActual);
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
