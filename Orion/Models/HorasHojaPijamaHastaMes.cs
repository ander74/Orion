#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {
	public class HorasHojaPijamaHastaMes: NotifyBase {


		// ====================================================================================================
		#region  CONSTRUCTORES
		// ====================================================================================================

		public HorasHojaPijamaHastaMes() { }

		public HorasHojaPijamaHastaMes(int año, int mes, int idTrabajador) {

			// HORAS
			Acumuladas = BdCalendarios.GetAcumuladasHastaMes(año, mes, idTrabajador);
			Reguladas = BdCalendarios.GetHorasReguladasHastaMes(año, mes, idTrabajador);
			//Descuadre = BdDiasCalendario.GetHorasDescuadreHastaMes(año, mes, idTrabajador);
			//ExcesoJornada = BdDiasCalendario.GetExcesoJornadaHastaMes(año, mes, idTrabajador);
			//ExcesoJornadaPendiente = BdDiasCalendario.GetExcesoJornadaPendienteHastaMes(año, mes, idTrabajador);
			//DescuadrePendiente = BdDiasCalendario.GetDescuadrePendienteHastaMes(año, mes, idTrabajador);

			// Cargamos el exceso de jornada cobrada hasta el mes anterior al indicado, ya que se añade el que se indica en la hoja pijama,
			// que corresponde al mes actual.
			//DateTime fecha = new DateTime(año, mes, 1).AddMonths(-1);
			//ExcesoJornadaPendiente = BdCalendarios.GetExcesoJornadaCobradaHastaMes(fecha.Year, fecha.Month, idTrabajador);


			// DÍAS
			DiasF6 = BdCalendarios.GetDiasF6HastaMes(año, mes, idTrabajador);
			DCs = BdCalendarios.GetDescansosDisfrutadosHastaMes(año, mes, idTrabajador);
			DNDs = BdCalendarios.GetDNDHastaMes(idTrabajador, año, mes);
			DcRegulados = BdCalendarios.GetDescansosReguladosHastaMes(año, mes, idTrabajador);
			ComiteEnDescanso = BdCalendarios.GetComiteEnDescansoHastaMes(idTrabajador, año, mes);
			TrabajoEnDescanso = BdCalendarios.GetTrabajoEnDescansoHastaMes(idTrabajador, año, mes);

		}


		#endregion
		

		// ====================================================================================================
		#region  PROPIEDADES HORAS
		// ====================================================================================================

		private TimeSpan _acumuladas;
		public TimeSpan Acumuladas {
			get { return _acumuladas; }
			set {
				if (_acumuladas != value) {
					_acumuladas = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _reguladas;
		public TimeSpan Reguladas {
			get { return _reguladas; }
			set {
				if (_reguladas != value) {
					_reguladas = value;
					PropiedadCambiada();
				}
			}
		}


		//private int _descuadre;
		//public int Descuadre {
		//	get { return _descuadre; }
		//	set {
		//		if (_descuadre != value) {
		//			_descuadre = value;
		//			PropiedadCambiada();
		//		}
		//	}
		//}


		//private TimeSpan _excesojornada;
		//public TimeSpan ExcesoJornada {
		//	get { return _excesojornada; }
		//	set {
		//		if (_excesojornada != value) {
		//			_excesojornada = value;
		//			PropiedadCambiada();
		//		}
		//	}
		//}


		//private TimeSpan _excesojornadapendiente;
		//public TimeSpan ExcesoJornadaPendiente {
		//	get { return _excesojornadapendiente; }
		//	set {
		//		if (_excesojornadapendiente != value) {
		//			_excesojornadapendiente = value;
		//			PropiedadCambiada();
		//		}
		//	}
		//}


		//private int _descuadrependiente;
		//public int DescuadrePendiente {
		//	get { return _descuadrependiente; }
		//	set {
		//		if (_descuadrependiente != value) {
		//			_descuadrependiente = value;
		//			PropiedadCambiada();
		//		}
		//	}
		//}


		#endregion


		// ====================================================================================================
		#region  PROPIEDADES DIAS
		// ====================================================================================================

		private int _diasf6;
		public int DiasF6 {
			get { return _diasf6; }
			set {
				if (_diasf6 != value) {
					_diasf6 = value;
					PropiedadCambiada();
				}
			}
		}


		private int _dcs;
		public int DCs {
			get { return _dcs; }
			set {
				if (_dcs != value) {
					_dcs = value;
					PropiedadCambiada();
				}
			}
		}


		private int _dnds;
		public int DNDs {
			get { return _dnds; }
			set {
				if (_dnds != value) {
					_dnds = value;
					PropiedadCambiada();
				}
			}
		}


		private int _dcregulados;
		public int DcRegulados {
			get { return _dcregulados; }
			set {
				if (_dcregulados != value) {
					_dcregulados = value;
					PropiedadCambiada();
				}
			}
		}


		private int _comiteendescanso;
		public int ComiteEnDescanso {
			get { return _comiteendescanso; }
			set {
				if (_comiteendescanso != value) {
					_comiteendescanso = value;
					PropiedadCambiada();
				}
			}
		}


		private int _trabajoendescanso;
		public int TrabajoEnDescanso {
			get { return _trabajoendescanso; }
			set {
				if (_trabajoendescanso != value) {
					_trabajoendescanso = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion



	}
}
