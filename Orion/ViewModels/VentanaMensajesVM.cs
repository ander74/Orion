#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;

namespace Orion.ViewModels {

	public class VentanaMensajesVM : NotifyBase {

		public enum BotonesMensaje { Cancelar, No, Si }

		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private string _titulo;
		public string Titulo {
			get { return _titulo; }
			set {
				if (_titulo != value) {
					_titulo = value;
					PropiedadCambiada();
				}
			}
		}


		private string _mensaje;
		public string Mensaje {
			get { return _mensaje; }
			set {
				if (_mensaje != value) {
					_mensaje = value;
					PropiedadCambiada();
				}
			}
		}


		private string _textobotoncancelar = "Cancelar";
		public string TextoBotonCancelar {
			get { return _textobotoncancelar; }
			set {
				if (_textobotoncancelar != value) {
					_textobotoncancelar = value;
					PropiedadCambiada();
				}
			}
		}


		private string _textobotonno = "No";
		public string TextoBotonNo {
			get { return _textobotonno; }
			set {
				if (_textobotonno != value) {
					_textobotonno = value;
					PropiedadCambiada();
				}
			}
		}


		private string _textobotonsi = "Ok";
		public string TextoBotonSi {
			get { return _textobotonsi; }
			set {
				if (_textobotonsi != value) {
					_textobotonsi = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _verbotoncancelar = false;
		public bool VerBotonCancelar {
			get { return _verbotoncancelar; }
			set {
				if (_verbotoncancelar != value) {
					_verbotoncancelar = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _verbotonno = false;
		public bool VerBotonNo {
			get { return _verbotonno; }
			set {
				if (_verbotonno != value) {
					_verbotonno = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _verbotonsi = true;
		public bool VerBotonSi {
			get { return _verbotonsi; }
			set {
				if (_verbotonsi != value) {
					_verbotonsi = value;
					PropiedadCambiada();
				}
			}
		}


		private BotonesMensaje _botonpulsado;
		public BotonesMensaje BotonPulsado {
			get { return _botonpulsado; }
			set {
				if (_botonpulsado != value) {
					_botonpulsado = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================



	}
}
