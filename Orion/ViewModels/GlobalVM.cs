#region COPYRIGHT
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
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Orion.ViewModels {

	public partial class GlobalVM: NotifyBase{

		// ====================================================================================================
		#region  CAMPOS PRIVADOS
		// ====================================================================================================
		private Centros _centroactual;
		private string _textoestado = "Version Desconocida";
		private string _textodetalle;
		private MensajeProvider mensajeProvider;

		// ViewModels
		private GraficosViewModel _graficosvm;
		private ConductoresViewModel _conductoresvm;
		private CalendariosViewModel _calendariosvm;
		private LineasViewModel _lineasvm;
		private OpcionesViewModel _opcionesvm;


		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region CONSTRUCTOR 
		// ====================================================================================================
		public GlobalVM() {

			// Actualizamos la condiguración si ha cambiado la versión del programa.
			if (Settings.Default.ActualizarSettings) {
				Settings.Default.Upgrade();
				Convenio.Default.Upgrade();
				PorCentro.Default.Upgrade();
				Settings.Default.ActualizarSettings = false;
				Settings.Default.Save();
				Convenio.Default.Save();
				PorCentro.Default.Save();
			}

			// Si las carpetas de configuracion están en blanco, rellenarlas.
			if (Settings.Default.CarpetaDatos == "") Settings.Default.CarpetaDatos = Path.Combine(Directory.GetCurrentDirectory(), "Datos");
			if (Settings.Default.CarpetaDropbox == "") Settings.Default.CarpetaDropbox = Path.Combine(Directory.GetCurrentDirectory(), "Dropbox");
			if (Settings.Default.CarpetaInformes == "") Settings.Default.CarpetaInformes = Path.Combine(Directory.GetCurrentDirectory(), "Informes");
			if (Settings.Default.CarpetaAyuda == "") Settings.Default.CarpetaAyuda = Path.Combine(Directory.GetCurrentDirectory(), "Ayuda");
			if (Settings.Default.CarpetaCopiasSeguridad == "") Settings.Default.CarpetaCopiasSeguridad = Path.Combine(Directory.GetCurrentDirectory(), "CopiasSeguridad");

			mensajeProvider = new MensajeProvider();
			CentroActual = (Centros)Settings.Default.CentroInicial;
			// Activamos el botón de la calculadora.
			Settings.Default.BotonCalculadoraActivo = true;

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
			string archivo = Utils.CombinarCarpetas(Settings.Default.CarpetaDatos, centro.ToString() + ".accdb");
			// Si no existe el archivo, devolvemos null
			if (!File.Exists(archivo)) return null;
			// Establecemos la cadena de conexión
			string cadenaConexion = "Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;";
			cadenaConexion += "Data Source=" + archivo + ";";
			// Devolvemos la cadena de conexión.
			return cadenaConexion;
		}

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================

		public void CerrandoVentanaEventHandler(object sender, CancelEventArgs e) {

			if (PuedeGuardarCambios()) {
				bool? resultado = mensajeProvider.VerMensaje("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?",
															 "NO SE HAN GUARDADO LOS CAMBIOS",true, true);

				switch (resultado) {
					case null:
						e.Cancel = true;
						return;
					case true:
						App.Global.GuardarCambios();
						break;
				}
			}

			// Guardamos las configuraciones.
			Settings.Default.Save();
			Convenio.Default.Save();
			PorCentro.Default.Save();

			// Si hay que sincronizar con DropBox, se copian los archivos.
			if (Settings.Default.SincronizarEnDropbox) Respaldo.SincronizarDropbox();

			// Intentamos cerrar la calculadora, si existe.
			if (App.Calculadora != null) App.Calculadora.Close();

			// Si hay que actualizar el programa, lanzamos el actualizador.
			if (App.ActualizarAlSalir) {
				Process.Start(@"OrionUpdate.exe", $"\"{Settings.Default.CarpetaOrigenActualizar}\"");
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
					_centroactual = value;
					// Si hay que sincronizar con Dropbox, copiamos los archivos a la carpeta actual.
					//if (Settings.Default.SincronizarEnDropbox) Respaldo.SincronizarDropbox();
					if ((int)_centroactual == Settings.Default.CentroInicial) {
						BotonGuardarRojo = Visibility.Collapsed;
						BotonGuardarAzul = Visibility.Visible;
					} else {
						BotonGuardarAzul = Visibility.Collapsed;
						BotonGuardarRojo = Visibility.Visible;
					}
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
		// VIEW MODELS
		//====================================================================================================

		public OpcionesViewModel OpcionesVM {
			get {
				if (_opcionesvm == null) _opcionesvm = new OpcionesViewModel(mensajeProvider);
				return _opcionesvm;
			}
		}


		public GraficosViewModel GraficosVM {
			get {
				if (_graficosvm == null) _graficosvm = new GraficosViewModel(mensajeProvider);
				return _graficosvm;
			}
		}


		public ConductoresViewModel ConductoresVM {
			get {
				if (_conductoresvm == null) _conductoresvm = new ConductoresViewModel(mensajeProvider);
				return _conductoresvm;
			}
		}


		public CalendariosViewModel CalendariosVM {
			get {
				if (_calendariosvm == null) _calendariosvm = new CalendariosViewModel(mensajeProvider);
				return _calendariosvm;
			}
		}


		public LineasViewModel LineasVM {
			get {
				if (_lineasvm == null) _lineasvm = new LineasViewModel(mensajeProvider);
				return _lineasvm;
			}
		}


		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region PROPIEDADES POR CENTROS
		// ====================================================================================================

		public int LunDel {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"LunDel{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"LunDel{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"LunDel{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int LunAl {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"LunAl{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"LunAl{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"LunAl{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int VieDel {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"VieDel{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"VieDel{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"VieDel{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int VieAl {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"VieAl{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"VieAl{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"VieAl{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int SabDel {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"SabDel{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"SabDel{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"SabDel{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int SabAl {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"SabAl{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"SabAl{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"SabAl{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int DomDel {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"DomDel{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"DomDel{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"DomDel{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int DomAl {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"DomAl{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"DomAl{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"DomAl{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int Comodin {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"Comodin{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"Comodin{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"Comodin{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public bool PagarLimpiezas {
			get {
				if (CentroActual == Centros.Desconocido) return false;
				return (bool)PorCentro.Default[$"PagarLimpiezas{CentroActual.ToString()}"];
			}
			set {
				if ((bool)PorCentro.Default[$"PagarLimpiezas{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"PagarLimpiezas{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public int NumeroLimpiezas {
			get {
				if (CentroActual == Centros.Desconocido) return 0;
				return (int)PorCentro.Default[$"NumeroLimpiezas{CentroActual.ToString()}"];
			}
			set {
				if ((int)PorCentro.Default[$"NumeroLimpiezas{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"NumeroLimpiezas{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public bool PagarPlusViaje {
			get {
				if (CentroActual == Centros.Desconocido) return false;
				return (bool)PorCentro.Default[$"PagarPlusViaje{CentroActual.ToString()}"];
			}
			set {
				if ((bool)PorCentro.Default[$"PagarPlusViaje{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"PagarPlusViaje{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}


		public decimal PlusViaje {
			get {
				if (CentroActual == Centros.Desconocido) return 0m;
				return (decimal)PorCentro.Default[$"PlusViaje{CentroActual.ToString()}"];
			}
			set {
				if ((decimal)PorCentro.Default[$"PlusViaje{CentroActual.ToString()}"] != value) {
					PorCentro.Default[$"PlusViaje{CentroActual.ToString()}"] = value;
					OpcionesVM.HayCambios = true;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================




	} //Fin de la clase.
}
