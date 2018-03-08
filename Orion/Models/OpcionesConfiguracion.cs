#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Orion.Models {

	public class OpcionesConfiguracion: NotifyBase {


		// ====================================================================================================
		#region GENERALES
		// ====================================================================================================

		private bool _abrirpdfs = true;
		public bool AbrirPDFs {
			get { return _abrirpdfs; }
			set {
				if (_abrirpdfs != value) {
					_abrirpdfs = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _actualizarprograma;
		public bool ActualizarPrograma {
			get { return _actualizarprograma; }
			set {
				if (_actualizarprograma != value) {
					_actualizarprograma = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _botoncalculadoraactivo = false;
		public bool BotonCalculadoraActivo {
			get { return _botoncalculadoraactivo; }
			set {
				if (_botoncalculadoraactivo != value) {
					_botoncalculadoraactivo = value;
					PropiedadCambiada();
				}
			}
		}


		private int _centroinicial = 0;
		public int CentroInicial {
			get { return _centroinicial; }
			set {
				if (_centroinicial != value) {
					_centroinicial = value;
					PropiedadCambiada();
				}
			}
		}


		private string _contraseñadatos = "n0xvPn6v0UQ=";
		public string ContraseñaDatos {
			get { return _contraseñadatos; }
			set {
				if (_contraseñadatos != value) {
					_contraseñadatos = value;
					PropiedadCambiada();
				}
			}
		}


		private int _copiaautomatica = 0;
		public int CopiaAutomatica {
			get { return _copiaautomatica; }
			set {
				if (_copiaautomatica != value) {
					_copiaautomatica = value;
					PropiedadCambiada();
				}
			}
		}


		private double _leftcalculadora = 0;
		public double LeftCalculadora {
			get { return _leftcalculadora; }
			set {
				if (_leftcalculadora != value) {
					_leftcalculadora = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _sincronizarendropbox = false;
		public bool SincronizarEnDropbox {
			get { return _sincronizarendropbox; }
			set {
				if (_sincronizarendropbox != value) {
					_sincronizarendropbox = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _soloañoactualengruposgraficos = false;
		public bool SoloAñoActualEnGruposGraficos {
			get { return _soloañoactualengruposgraficos; }
			set {
				if (_soloañoactualengruposgraficos != value) {
					_soloañoactualengruposgraficos = value;
					PropiedadCambiada();
				}
			}
		}


		private double _topcalculadora = 0;
		public double TopCalculadora {
			get { return _topcalculadora; }
			set {
				if (_topcalculadora != value) {
					_topcalculadora = value;
					PropiedadCambiada();
				}
			}
		}


		private DateTime _ultimacopia = new DateTime(2000, 1, 1);
		public DateTime UltimaCopia {
			get { return _ultimacopia; }
			set {
				if (_ultimacopia != value) {
					_ultimacopia = value;
					PropiedadCambiada();
				}
			}
		}


		private string _version = "";
		public string Version {
			get { return _version; }
			set {
				if (_version != value) {
					_version = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region CARPETAS
		// ====================================================================================================

		private string _carpetaayuda = "";
		public string CarpetaAyuda {
			get { return _carpetaayuda; }
			set {
				if (_carpetaayuda != value) {
					_carpetaayuda = value;
					PropiedadCambiada();
				}
			}
		}


		private string _carpetacopiasseguridad = "";
		public string CarpetaCopiasSeguridad {
			get { return _carpetacopiasseguridad; }
			set {
				if (_carpetacopiasseguridad != value) {
					_carpetacopiasseguridad = value;
					PropiedadCambiada();
				}
			}
		}


		private string _carpetadatos = "";
		public string CarpetaDatos {
			get { return _carpetadatos; }
			set {
				if (_carpetadatos != value) {
					_carpetadatos = value;
					PropiedadCambiada();
				}
			}
		}


		private string _carpetadropbox = "";
		public string CarpetaDropbox {
			get { return _carpetadropbox; }
			set {
				if (_carpetadropbox != value) {
					_carpetadropbox = value;
					PropiedadCambiada();
				}
			}
		}


		private string _carpetainformes = "Informes";
		public string CarpetaInformes {
			get { return _carpetainformes; }
			set {
				if (_carpetainformes != value) {
					_carpetainformes = value;
					PropiedadCambiada();
				}
			}
		}


		private string _carpetaorigenactualizar = "";
		public string CarpetaOrigenActualizar {
			get { return _carpetaorigenactualizar; }
			set {
				if (_carpetaorigenactualizar != value) {
					_carpetaorigenactualizar = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region COLUMNAS GRÁFICOS
		// ====================================================================================================

		private bool _mostrargrafdescuadres = true;
		public bool MostrarGrafDescuadres {
			get { return _mostrargrafdescuadres; }
			set {
				if (_mostrargrafdescuadres != value) {
					_mostrargrafdescuadres = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargrafdietas = true;
		public bool MostrarGrafDietas {
			get { return _mostrargrafdietas; }
			set {
				if (_mostrargrafdietas != value) {
					_mostrargrafdietas = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargrafhoras = true;
		public bool MostrarGrafHoras {
			get { return _mostrargrafhoras; }
			set {
				if (_mostrargrafhoras != value) {
					_mostrargrafhoras = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargrafnocalcular = true;
		public bool MostrarGrafNoCalcular {
			get { return _mostrargrafnocalcular; }
			set {
				if (_mostrargrafnocalcular != value) {
					_mostrargrafnocalcular = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargrafpluslimpieza = true;
		public bool MostrarGrafPlusLimpieza {
			get { return _mostrargrafpluslimpieza; }
			set {
				if (_mostrargrafpluslimpieza != value) {
					_mostrargrafpluslimpieza = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargrafpluspaqueteria = true;
		public bool MostrarGrafPlusPaqueteria {
			get { return _mostrargrafpluspaqueteria; }
			set {
				if (_mostrargrafpluspaqueteria != value) {
					_mostrargrafpluspaqueteria = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostrargraftiempopartido = true;
		public bool MostrarGrafTiempoPartido {
			get { return _mostrargraftiempopartido; }
			set {
				if (_mostrargraftiempopartido != value) {
					_mostrargraftiempopartido = value;
					PropiedadCambiada();
				}
			}
		}




		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region COLORES
		// ====================================================================================================

		private Color _colordc = Colors.DarkViolet;
		public Color ColorDC {
			get { return _colordc; }
			set {
				if (_colordc != value) {
					_colordc = value;
					PropiedadCambiada();
				}
			}
		}


		private Color _colords = Colors.Brown;
		public Color ColorDS {
			get { return _colords; }
			set {
				if (_colords != value) {
					_colords = value;
					PropiedadCambiada();
				}
			}
		}


		private Color _colore = Color.FromArgb(255, 0, 192, 192);
		public Color ColorE {
			get { return _colore; }
			set {
				if (_colore != value) {
					_colore = value;
					PropiedadCambiada();
				}
			}
		}


		private Color _colorf6 = Colors.Chocolate;
		public Color ColorF6 {
			get { return _colorf6; }
			set {
				if (_colorf6 != value) {
					_colorf6 = value;
					PropiedadCambiada();
				}
			}
		}


		private Color _colorjd = Colors.Magenta;
		public Color ColorJD {
			get { return _colorjd; }
			set {
				if (_colorjd != value) {
					_colorjd = value;
					PropiedadCambiada();
				}
			}
		}


		private Color _colorov = Colors.Green;
		public Color ColorOV {
			get { return _colorov; }
			set {
				if (_colorov != value) {
					_colorov = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS
		// ====================================================================================================

		public void Guardar(string ruta) {

			string datos = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(ruta, datos);
		}

		public void Cargar(string ruta) {

			if (File.Exists(ruta)) {
				string datos = File.ReadAllText(ruta);
				JsonConvert.PopulateObject(datos, this);
				PropiedadCambiada("");
			}
		}

		#endregion
		// ====================================================================================================




	}
}
