#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Convertidores;
using Orion.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orion.ViewModels {

	public partial class CalculadoraViewModel:NotifyBase {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		ConvertidorSuperHora cnvSuperHora = new ConvertidorSuperHora();


		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void Operar(TimeSpan ts) {
			switch (TextoOperacion) {
				case "+":
					HoraResultado += ts;
					break;
				case "-":
					HoraResultado -= ts;
					break;
				case "*":
					HoraResultado = new TimeSpan(HoraResultado.Ticks * Convert.ToInt64(ts.TotalHours));
					break;
				case "/":
					if (ts.Hours == 0) {
						HoraResultado = new TimeSpan(0);
					} else {
						HoraResultado = new TimeSpan(HoraResultado.Ticks / Convert.ToInt64(ts.TotalHours));
					}
					break;
				case "": case "=":
					HoraResultado = ts;
					break;
			}
		}
	


		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================


		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private TimeSpan _horaalmacen = new TimeSpan(0);
		public TimeSpan HoraAlmacen {
			get { return _horaalmacen; }
			set {
				if (_horaalmacen != value) {
					_horaalmacen = value;
					PropiedadCambiada();
				}
			}
		}



		private TimeSpan _horaresultado = new TimeSpan(0);
		public TimeSpan HoraResultado {
			get { return _horaresultado; }
			set {
				if (_horaresultado != value) {
					_horaresultado = value;
					PropiedadCambiada();
				}
			}
		}


		public bool EsResultado = false;
		public bool EsOperacion = false;


		private string _textoentrada = "";
		public string TextoEntrada {
			get { return _textoentrada; }
			set {
				if (_textoentrada != value) {
					_textoentrada = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(TextoHora));
					PropiedadCambiada(nameof(TextoDecimal));
					PropiedadCambiada(nameof(TextoError));
				}
			}
		}


		private string _textooperacion = "";
		public string TextoOperacion {
			get { return _textooperacion; }
			set {
				if (_textooperacion != value) {
					_textooperacion = value;
					PropiedadCambiada();
				}
			}
		}


		public string TextoError {
			get {
				if (TextoEntrada.Trim().Length == 0) return "";
				TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
				if (ts == null) {
					return "Err";
				}
				return "";
			}
		}


		public string TextoHora {
			get {
				// Si el texto contiene el símbolo : y es una hora válida
				TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
				if (TextoEntrada.Contains(":") && ts != null) {
					return (string)cnvSuperHora.Convert(ts, null, null, null);
				}
				return TextoEntrada;
			}
		}


		public string TextoDecimal {
			get {
				TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
				if (TextoEntrada.Contains(":") && ts != null) {
					return ts.ToDecimal().ToString("0.00");
				}
				return "";
			}
		}



		#endregion


	}
}
