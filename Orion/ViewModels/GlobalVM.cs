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
        public InformesServicio Informes; // Se hace público para acceder a él vía instancia.
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
        private InformesViewModel _informes;

        // Repositorios
        private OrionRepository bilbaoRepository;
        private OrionRepository donostiRepository;
        private OrionRepository arrasateRepository;
        private OrionRepository gasteizRepository;



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR 
        // ====================================================================================================
        public GlobalVM() {

            // Creamos los servicios
            mensajes = new MensajesServicio();
            Informes = new InformesServicio();
            fileService = new FileService();
            ServicioOneDrive = new OneDriveService();

            // Definimos la carpeta 'Orion' en la carpeta 'Documentos' del usuario. Si no existe, la creamos.
            CarpetaOrion = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Orion");
            if (!Directory.Exists(CarpetaOrion)) Directory.CreateDirectory(CarpetaOrion);

            // Definimos la carpeta 'Configuracion' en la carpeta 'Orion'. Si no existe, la creamos.
            CarpetaConfiguracion = Path.Combine(CarpetaOrion, "Configuracion");
            if (!Directory.Exists(CarpetaConfiguracion)) Directory.CreateDirectory(CarpetaConfiguracion);

            // Cargamos la configuracion
            Configuracion.Cargar(ArchivoOpcionesConfiguracion);
            Convenio.Cargar(ArchivoOpcionesConvenio);

            // Si las carpetas de configuracion están en blanco, crearlas y rellenarlas.
            if (Configuracion.CarpetaDatos == "") Configuracion.CarpetaDatos = CreateAndGetCarpetaEnOrion("Datos");
            //if (Configuracion.CarpetaDropbox == "") Configuracion.CarpetaDropbox = CreateAndGetCarpetaEnOrion("Dropbox");
            if (Configuracion.CarpetaInformes == "") Configuracion.CarpetaInformes = CreateAndGetCarpetaEnOrion("Informes");
            //if (Configuracion.CarpetaAvanza == "") Configuracion.CarpetaAvanza = CreateAndGetCarpetaEnOrion("Avanza");
            if (Configuracion.CarpetaCopiasSeguridad == "") Configuracion.CarpetaCopiasSeguridad = CreateAndGetCarpetaEnOrion("CopiasSeguridad");
            if (Configuracion.CarpetaOrigenActualizar == "") Configuracion.CarpetaOrigenActualizar = App.RutaInicial;

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


        public string GetArchivoDatos(Centros centro) {
            return Path.Combine(Configuracion.CarpetaDatos, $"{centro}.db3");
            //return $"{centro}.db3";
        }


        public string GetArchivoPorCentroPath(Centros centro) {
            return Path.Combine(CarpetaConfiguracion, $"PorCentro{centro}.json");
        }


        public string CreateAndGetCarpetaEnOrion(string carpeta) {
            var resultado = Path.Combine(CarpetaOrion, carpeta);
            if (!Directory.Exists(resultado)) Directory.CreateDirectory(resultado);
            return resultado;
        }


        /// <summary>
        /// Devuelve el repositorio del centro en cuestion.<br/>
        /// No válido para el repositorio de las líneas.
        /// </summary>
        /// <param name="centro"></param>
        /// <returns></returns>
        public OrionRepository GetRepository(Centros centro) {
            switch (centro) {
                case Centros.Bilbao:
                    if (bilbaoRepository == null) bilbaoRepository = new OrionRepository(GetCadenaConexionSQL(Centros.Bilbao));
                    return bilbaoRepository;
                case Centros.Donosti:
                    if (donostiRepository == null) donostiRepository = new OrionRepository(GetCadenaConexionSQL(Centros.Donosti));
                    return donostiRepository;
                case Centros.Arrasate:
                    if (arrasateRepository == null) arrasateRepository = new OrionRepository(GetCadenaConexionSQL(Centros.Arrasate));
                    return arrasateRepository;
                case Centros.Vitoria:
                    if (gasteizRepository == null) gasteizRepository = new OrionRepository(GetCadenaConexionSQL(Centros.Vitoria));
                    return gasteizRepository;
            }
            return null;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================


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

            // Si hay que sincronizar con DropBox, se copian los archivos.
            if (Configuracion.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

            // Guardamos el estado de OneDrive
            File.WriteAllText(ArchivoOpcionesOneDrive, ServicioOneDrive.GetConfiguracionCodificada(), System.Text.Encoding.UTF8);

            // Intentamos cerrar la calculadora, si existe.
            if (App.Calculadora != null) App.Calculadora.Close();

            // Apagamos los servicios
            if (Informes != null) Informes.Dispose();

            // Si hay que actualizar el programa, lanzamos el actualizador.
            if (App.ActualizarAlSalir) {
                //Process.Start(@"OrionUpdate.exe", $"\"{Configuracion.CarpetaOrigenActualizar}\"");
                //TODO: Con el nuevo sistema de actualización, comentar la línea anterior y descomentar las siguientes.
                var exeFile = Path.Combine(Configuracion.CarpetaOrigenActualizar, "setup.exe");
                if (File.Exists(exeFile)) Process.Start(exeFile);
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
                        //// Inicializamos el centro en la base de datos SQL
                        //repository = new OrionRepository(CadenaConexionSQL);
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



        private string paginaWeb = "about:blank";
        public string PaginaWeb {
            get => paginaWeb;
            set => SetValue(ref paginaWeb, value);
        }

        //====================================================================================================
        // REPOSITORIO DE DATOS
        //====================================================================================================

        /// <summary>
        /// Repositorio del centro activo
        /// </summary>
        public OrionRepository Repository {
            get {
                return GetRepository(CentroActual);
            }
        }


        private LineasRepository lineasRepository;
        /// <summary>
        /// Repositorio de líneas.
        /// </summary>
        public LineasRepository LineasRepo {
            get {
                if (lineasRepository == null) lineasRepository = new LineasRepository(CadenaConexionLineasSQL);
                return lineasRepository;
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


        //====================================================================================================
        // OTRAS PROPIEDADES
        //====================================================================================================

        private bool _mostrarBotonCerrar;
        public bool MostrarBotonCerrar {
            get => _mostrarBotonCerrar;
            set => SetValue(ref _mostrarBotonCerrar, value);
        }


        public OneDriveService ServicioOneDrive { get; set; }

        public IMensajes Mensajes { get => mensajes; }



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


        public InformesViewModel InformesVM {
            get {
                _informes = _informes ?? new InformesViewModel(mensajes);
                return _informes;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region OPCIONES
        // ====================================================================================================

        public string CarpetaOrion { get; set; }


        public string CarpetaConfiguracion { get; set; }


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


        public string ArchivoTextoConvenio {
            get {
                return Path.Combine(CarpetaConfiguracion, "TextoConvenio.json");
            }
        }


        public string ArchivoOpcionesOneDrive {
            get {
                return Path.Combine(CarpetaConfiguracion, "OneDrive.json");
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
