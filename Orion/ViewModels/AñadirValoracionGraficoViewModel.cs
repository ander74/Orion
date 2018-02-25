#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using Orion.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Orion.ViewModels {

	public partial class AñadirValoracionGraficoViewModel: NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajeProvider _mensajeProvider;

		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public AñadirValoracionGraficoViewModel(IMensajeProvider mensajeProvider) {
			_mensajeProvider = mensajeProvider;
		}
		#endregion


		// ====================================================================================================
		#region COMANDOS
		// ====================================================================================================

		#region BOTON ACEPTAR
		// Comando
		private ICommand _cmdbotonaceptar;

		public ICommand cmdBotonAceptar {
			get {
				if (_cmdbotonaceptar == null) _cmdbotonaceptar = new RelayCommand(p => BotonAceptar(p), p => PuedeBotonAceptar());
				return _cmdbotonaceptar;
			}
		}

		// Se puede ejecutar
		private bool PuedeBotonAceptar() {
			return Inicio.HasValue;
		}

		// Ejecución del comando
		private void BotonAceptar(object parametro) {
			if (parametro == null) return;
			Window ventana = (Window)parametro;
			ventana.DialogResult = true;
			ventana.Close();
		}
		#endregion



		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private TimeSpan? _inicio;
		public TimeSpan? Inicio {
			get { return _inicio; }
			set {
				if (_inicio != value) {
					_inicio = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _linea;
		public decimal Linea {
			get { return _linea; }
			set {
				if (_linea != value) {
					_linea = value;
					PropiedadCambiada();
				}
			}
		}



		private decimal _numerografico;
		public decimal NumeroGrafico {
			get { return _numerografico; }
			set {
				if (_numerografico != value) {
					_numerografico = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion


	}
}
