#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Convertidores;
using Orion.Models;
using Orion.Properties;
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

	public partial class CalculadoraViewModel {

		#region BOTON PULSADO
		//Comando
		private ICommand _cmdpulsarboton;
		public ICommand cmdPulsarBoton {
			get {
				if (_cmdpulsarboton == null) _cmdpulsarboton = new RelayCommand(p => PulsarBoton(p));
				return _cmdpulsarboton;
			}
		}

		// Ejecución del comando
		private void PulsarBoton(object parametro) {
			// Evaluamos y convertimos el parámetro pasado.
			if (parametro == null) return;
			int.TryParse((string)parametro, out int codigo);

			// Si es un número...
			if (codigo < 10) {
				// Si es operación...
				if (EsOperacion) {
					// Añadimos el número pulsado, eliminando la cifra anterior.
					EsOperacion = false;
					TextoEntrada = codigo.ToString();
					// Si no lo es...
				} else if (EsResultado) {
					EsResultado = false;
					TextoOperacion = "";
					TextoEntrada = codigo.ToString();
				} else {
					// Añadimos el número a la cifra anterior.
					TextoEntrada += codigo.ToString();
				}
			}

			// Si es la tecla ':' ...
			if (codigo == 15) {
				if (TextoOperacion == "*" || TextoOperacion == "/") return;
				if (EsOperacion) {
					EsOperacion = false;
					TextoEntrada = "";
				}
				if (EsResultado) {
					EsResultado = false;
					TextoEntrada = "";
				}
				if (!TextoEntrada.Contains(":")) {
					if (TextoEntrada.Length == 0) {
						TextoEntrada = "0:";
					} else {
						TextoEntrada += ":";
					}
				}
			}

			// Si es la tecla '+-' ...
			if (codigo == 18) {
				if (TextoEntrada.Length == 0) return;
				TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
				if (ts == null) {
					if (TextoEntrada.Substring(0,1) == "-") {
						TextoEntrada = TextoEntrada.Substring(1);
					} else {
						TextoEntrada += "-";
					}
				} else {
					if (EsOperacion || EsResultado) {
						TextoEntrada = (string)cnvSuperHora.Convert(new TimeSpan(ts.Value.Ticks * -1), null, null, null);
						HoraResultado = new TimeSpan(HoraResultado.Ticks * -1);
					} else {
						TextoEntrada = (string)cnvSuperHora.Convert(new TimeSpan(ts.Value.Ticks * -1), null, null, null);
					}
				}
			}

			// Si es la tecla 'Borrar' ...
			if (codigo == 16 && TextoEntrada.Length > 0) {
				TextoEntrada = TextoEntrada.Substring(0, TextoEntrada.Length - 1);
			}

			// Si es la tecla 'C' ...
			if (codigo == 17) {
				// Si hay texto escrito, lo borramos todo.
				if (TextoEntrada.Length > 0) {
					TextoEntrada = "";
				// Si no hay, reseteamos la operacion
				} else {
					EsOperacion = false;
					EsResultado = false;
					TextoOperacion = "";
					HoraResultado = TimeSpan.Zero;
					HoraAlmacen = TimeSpan.Zero;
				}
			}

			// Si es la tecla '+' ...
			if (codigo == 11) {
				if (EsOperacion) {
					if (TextoOperacion == "+") Operar(HoraAlmacen); else HoraAlmacen = TimeSpan.Zero;
				} else if (EsResultado) {

				} else {
					// Si el texto no tiene el símbolo : lo añadimos.
					if (!TextoEntrada.Contains(":")) TextoEntrada += ":00";
					TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
					if (TextoEntrada.Contains(":") && ts != null) {
						HoraAlmacen = ts.Value;
						Operar(ts.Value);
					}

				}
				TextoEntrada = (string)cnvSuperHora.Convert(HoraResultado, null, null, null);
				TextoOperacion = "+";
				EsOperacion = true;
				EsResultado = false;
				
			}


			// Si es la tecla '-' ...
			if (codigo == 12) {
				if (EsOperacion) {
					if (TextoOperacion == "-") Operar(HoraAlmacen); else HoraAlmacen = TimeSpan.Zero;
				} else if (EsResultado) {

				} else {
					// Si el texto no tiene el símbolo : lo añadimos.
					if (!TextoEntrada.Contains(":")) TextoEntrada += ":00";
					TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
					if (TextoEntrada.Contains(":") && ts != null) {
						HoraAlmacen = ts.Value;
						Operar(ts.Value);
					}
				}
				TextoEntrada = (string)cnvSuperHora.Convert(HoraResultado, null, null, null);
				TextoOperacion = "-";
				EsOperacion = true;
				EsResultado = false;

			}


			// Si es la tecla '*' ...
			if (codigo == 13) {
				if (EsOperacion) {
					if (TextoOperacion == "*") Operar(HoraAlmacen); else HoraAlmacen = TimeSpan.Zero;
				} else if (EsResultado) {

				} else {
					// Si el texto no tiene el símbolo : lo añadimos.
					if (!TextoEntrada.Contains(":")) TextoEntrada += ":00";
					TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
					if (TextoEntrada.Contains(":") && ts != null) {
						HoraAlmacen = ts.Value;
						Operar(ts.Value);
					}
				}
				TextoEntrada = (string)cnvSuperHora.Convert(HoraResultado, null, null, null);
				TextoOperacion = "*";
				EsOperacion = true;
				EsResultado = false;

			}


			// Si es la tecla '/' ...
			if (codigo == 14) {
				if (EsOperacion) {
					if (TextoOperacion == "/") Operar(HoraAlmacen); else HoraAlmacen = TimeSpan.Zero;
				} else if (EsResultado) {

				} else {
					// Si el texto no tiene el símbolo : lo añadimos.
					if (!TextoEntrada.Contains(":")) TextoEntrada += ":00";
					TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
					if (TextoEntrada.Contains(":") && ts != null) {
						HoraAlmacen = ts.Value;
						Operar(ts.Value);
					}
				}
				TextoEntrada = (string)cnvSuperHora.Convert(HoraResultado, null, null, null);
				TextoOperacion = "/";
				EsOperacion = true;
				EsResultado = false;

			}


			// Si es la tecla =
			if (codigo == 10) {
				if (EsOperacion || EsResultado) {

				} else {
					// Si el texto no tiene el símbolo : lo añadimos.
					if (!TextoEntrada.Contains(":")) TextoEntrada += ":00";
					TimeSpan? ts = (TimeSpan?)cnvSuperHora.ConvertBack(TextoEntrada, null, null, null);
					if (TextoEntrada.Contains(":") && ts != null) {
						HoraAlmacen = ts.Value;
						Operar(ts.Value);
					}
				}
				TextoEntrada = (string)cnvSuperHora.Convert(HoraResultado, null, null, null);
				TextoOperacion = "=";
				HoraAlmacen = TimeSpan.Zero;
				EsOperacion = false;
				EsResultado = true;

			}
		}
		#endregion


		#region CERRAR VENTANA

		// Comando
		private ICommand _cerrarventana;
		public ICommand cmdCerrarVentana {
			get {
				if (_cerrarventana == null) _cerrarventana = new RelayCommand(p => CerrarVentana());
				return _cerrarventana;
			}
		}


		// Ejecución del comando
		private void CerrarVentana() {
			App.Global.Configuracion.BotonCalculadoraActivo = true;
			App.Global.Configuracion.LeftCalculadora = Izquierda;
			App.Global.Configuracion.TopCalculadora = Arriba;
			SolicitarCierreVentana();

		}
		#endregion


	}
}
