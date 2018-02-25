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
    public partial class VentanaRegularAñoVM: NotifyBase{


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		private IMensajeProvider _mensajeProvider;
		#endregion


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================

		public VentanaRegularAñoVM(IMensajeProvider mensajeProvider) {
			_mensajeProvider = mensajeProvider;
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS
		// ====================================================================================================
		public void RefrescarValores() {

			if (IdConductor <= 0) return;

			try {
				// Cargamos las horas.
				TimeSpan acumuladas = BdCalendarios.GetAcumuladasHastaMes(Año, 11, IdConductor);
				TimeSpan reguladas = BdCalendarios.GetHorasReguladasHastaMes(Año, 11, IdConductor);
				HorasDisponibles = HorasConductor + acumuladas + reguladas;
			} catch (Exception ex) {
				_mensajeProvider.VerError("VentanaCobrarHorasVM.RefrescarValores", ex);
			}
		}

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
				if (_cmdaceptar == null) _cmdaceptar = new RelayCommand(p => Aceptar(p), p => SePuedeAceptar(p));
				return _cmdaceptar;
			}
		}

		// Se puede ejecutar
		private bool SePuedeAceptar(object parametro) {
			if (HorasDisponibles <= Convenio.Default.JornadaMedia) return false;
			return true;
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


		#region AÑO ANTERIOR
		//Comando
		private ICommand _cmdañoanterior;
		public ICommand cmdAñoAnterior {
			get {
				if (_cmdañoanterior == null) _cmdañoanterior = new RelayCommand(p => AñoAnterior());
				return _cmdañoanterior;
			}
		}

		// Ejecución del comando
		private void AñoAnterior() {
			Año--;
		}
		#endregion


		#region AÑO SIGUIENTE
		//Comando
		private ICommand _cmdañosiguiente;
		public ICommand cmdAñoSiguiente {
			get {
				if (_cmdañosiguiente == null) _cmdañosiguiente = new RelayCommand(p => AñoSiguiente());
				return _cmdañosiguiente;
			}
		}

		// Ejecución del comando
		private void AñoSiguiente() {
			Año++;
		}
		#endregion



		#endregion



		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private int _idconductor;
		public int IdConductor {
			get { return _idconductor; }
			set {
				if (_idconductor != value) {
					_idconductor = value;
					PropiedadCambiada();
				}
			}
		}


		private string _apellidosconductor;
		public string ApellidosConductor {
			get { return _apellidosconductor; }
			set {
				if (_apellidosconductor != value) {
					_apellidosconductor = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _horasconductor;
		public TimeSpan HorasConductor {
			get { return _horasconductor; }
			set {
				if (_horasconductor != value) {
					_horasconductor = value;
					PropiedadCambiada();
				}
			}
		}


		private int _año = DateTime.Now.Year;
		public int Año {
			get { return _año; }
			set {
				if (_año != value) {
					_año = value;
					PropiedadCambiada();
					RefrescarValores();
				}
			}
		}

		private TimeSpan _horasdisponibles;
		public TimeSpan HorasDisponibles {
			get { return _horasdisponibles; }
			set {
				if (_horasdisponibles != value) {
					_horasdisponibles = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(DescansosGenerados));
					PropiedadCambiada(nameof(HorasARegular));
					PropiedadCambiada(nameof(HorasRestantes));
				}
			}
		}


		public int DescansosGenerados {
			get {
				return (int)Math.Truncate(HorasDisponibles.ToDecimal() / Convenio.Default.JornadaMedia.ToDecimal());
			}
		}


		public TimeSpan HorasARegular {
			get {
				return new TimeSpan(DescansosGenerados * Convenio.Default.JornadaMedia.Ticks);
			}
		}


		public TimeSpan HorasRestantes {
			get {
				return HorasDisponibles - HorasARegular;
			}
		}




		public RegulacionConductor Regulacion {
			get {
				RegulacionConductor regulacion = new RegulacionConductor();
				regulacion.Codigo = 2;
				regulacion.Descansos = DescansosGenerados;
				regulacion.Fecha = new DateTime(Año, 11, 30);
				regulacion.Horas = new TimeSpan(HorasARegular.Ticks * -1);
				regulacion.IdConductor = IdConductor;
				regulacion.Motivo = "Regulación anual.";
				return regulacion;
			}
		}


		#endregion

	}
}
