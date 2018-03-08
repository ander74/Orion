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
using Orion.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Orion.ViewModels
{
    public partial class VentanaCobrarHorasVM: NotifyBase{


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajeProvider _mensajeProvider;
		#endregion


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================

		public VentanaCobrarHorasVM(IMensajeProvider mensajeProvider) {
			_mensajeProvider = mensajeProvider;
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS
		// ====================================================================================================

		#endregion


		// ====================================================================================================
		#region COMANDOS
		// ====================================================================================================


		#region CANCELAR
		//Comando
		private ICommand _cmdcancelar;
				public ICommand cmdCancelar {
					get {
						if (_cmdcancelar == null) _cmdcancelar = new RelayCommand(p => Cancelar(p));
						return _cmdcancelar;
					}
				}

				// Ejecución del comando
				private void Cancelar(object parametro) {
					if (parametro == null) return;
					((Window)parametro).Close();
				}
				#endregion


		#region ACEPTAR
		//Comando
		private ICommand _cmdaceptar;
		public ICommand cmdAceptar {
			get {
				if (_cmdaceptar == null) _cmdaceptar = new RelayCommand(p => Aceptar(p));
				return _cmdaceptar;
			}
		}


		// Ejecución del comando
		private void Aceptar(object parametro) {
			if (parametro == null) return;
			Window ventana = (Window)parametro;

			// Cerramos la ventana enviando True.
			ventana.DialogResult = true;
			ventana.Close();
		}

		#endregion


		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private TimeSpan _horasdisponibles;
		public TimeSpan HorasDisponibles {
			get { return _horasdisponibles; }
			set {
				if (_horasdisponibles != value) {
					_horasdisponibles = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(HorasMaximas));
				}
			}
		}


		private TimeSpan _horascobradas;
		public TimeSpan HorasCobradas {
			get { return _horascobradas; }
			set {
				if (_horascobradas != value) {
					_horascobradas = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(HorasMaximas));
				}
			}
		}


		public TimeSpan HorasMaximas {
			get {
				TimeSpan maxconvenio = new TimeSpan(App.Global.Convenio.MaxHorasCobradas,0,0);
				if (HorasDisponibles <= new TimeSpan(0) || HorasCobradas >= maxconvenio) return new TimeSpan(0);
				if ((maxconvenio - HorasCobradas) > HorasDisponibles) {
					return HorasDisponibles;
				} else {
					return maxconvenio - HorasCobradas;
				}
			}
		}


		private TimeSpan? _horasacobrar;
		public TimeSpan? HorasACobrar {
			get { return _horasacobrar; }
			set {
				if (_horasacobrar != value) {
					_horasacobrar = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion

	}
}
