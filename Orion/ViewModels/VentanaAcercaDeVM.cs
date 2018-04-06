#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using iTextSharp.xmp.impl;
using Orion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orion.ViewModels {

	public class VentanaAcercaDeVM: NotifyBase {

		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================

		public VentanaAcercaDeVM() {
			try {
				var ensamblado = Assembly.GetExecutingAssembly();

				using (StreamReader lector = new StreamReader(ensamblado.GetManifestResourceStream("Orion.Licencia.txt"))) {
					TextoLicencia = lector.ReadToEnd();
				}

				using (StreamReader lector = new StreamReader(ensamblado.GetManifestResourceStream("Orion.LicenciaIText.txt"))) {
					TextoIText = lector.ReadToEnd();
				}
			} catch (Exception ex) {
				TextoLicencia = "Error al extraer la licencia.\n\n" + ex.Message;
			}

		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private string _textolicencia;
		public string TextoLicencia {
			get { return _textolicencia; }
			set {
				if (_textolicencia != value) {
					_textolicencia = value;
					PropiedadCambiada();
				}
			}
		}


		private string _textoitext;
		public string TextoIText {
			get { return _textoitext; }
			set {
				if (_textoitext != value) {
					_textoitext = value;
					PropiedadCambiada();
				}
			}
		}



		private bool _aceptarlicencia;
		public bool AceptarLicencia {
			get { return _aceptarlicencia; }
			set {
				if (_aceptarlicencia != value) {
					_aceptarlicencia = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _mostraraceptarlicencia;
		public bool MostrarAceptarLicencia {
			get { return _mostraraceptarlicencia; }
			set {
				if (_mostraraceptarlicencia != value) {
					_mostraraceptarlicencia = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(TituloVentana));
				}
			}
		}


		public string TituloVentana {
			get {
				if (MostrarAceptarLicencia) {
					return "Acuerdo de Liecncia para el Usuario";
				} else {
					Version ver = Assembly.GetExecutingAssembly().GetName().Version;
					return $"Orion {ver.Major}.{ver.Minor}";
				}
			}
		}



		#endregion
		// ====================================================================================================


	}
}
