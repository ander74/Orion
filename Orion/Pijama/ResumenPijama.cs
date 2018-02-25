#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Pijama {

	public class ResumenPijama {


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private TimeSpan _horasacumuladas;
		/// <summary>
		/// Horas acumuladas.
		/// </summary>
		public TimeSpan HorasAcumuladas{
			get { return _horasacumuladas; }
			set {
				if (_horasacumuladas != value) {
					_horasacumuladas = value;
				}
			}
		}


		private TimeSpan _horasreguladas;
		/// <summary>
		/// Todas las horas reguladas, sin tener en cuenta su código (se hace un computo de todas).
		/// </summary>
		public TimeSpan HorasReguladas {
			get { return _horasreguladas; }
			set {
				if (_horasreguladas != value) {
					_horasreguladas = value;
				}
			}
		}


		private int _diaslibredisposicionf6;
		/// <summary>
		/// Días de libre disposición.
		/// </summary>
		public int DiasLibreDisposicionF6 {
			get { return _diaslibredisposicionf6; }
			set {
				if (_diaslibredisposicionf6 != value) {
					_diaslibredisposicionf6 = value;
				}
			}
		}


		private int _dcsregulados;
		/// <summary>
		/// Descansos compensatorios regulados (se hace un computo de todos).
		/// </summary>
		public int DCsRegulados {
			get { return _dcsregulados; }
			set {
				if (_dcsregulados != value) {
					_dcsregulados = value;
				}
			}
		}


		private int _dcsdisfrutados;
		/// <summary>
		/// Descansos compensatorios disfrutados.
		/// </summary>
		public int DCsDisfrutados {
			get { return _dcsdisfrutados; }
			set {
				if (_dcsdisfrutados != value) {
					_dcsdisfrutados = value;
				}
			}
		}


		private int _dndsdisfrutados;
		/// <summary>
		/// Descansos no disfrutados que se han disfrutado.
		/// </summary>
		public int DNDsDisfrutados {
			get { return _dndsdisfrutados; }
			set {
				if (_dndsdisfrutados != value) {
					_dndsdisfrutados = value;
				}
			}
		}


		private int _diascomiteendescanso;
		/// <summary>
		/// Días de comite que se han tenido un día de descanso.
		/// </summary>
		public int DiasComiteEnDescanso {
			get { return _diascomiteendescanso; }
			set {
				if (_diascomiteendescanso != value) {
					_diascomiteendescanso = value;
				}
			}
		}


		private int _diastrabajoendescanso;
		/// <summary>
		/// Días trabajados en un día de descanso.
		/// </summary>
		public int DiasTrabajoEnDescanso {
			get { return _diastrabajoendescanso; }
			set {
				if (_diastrabajoendescanso != value) {
					_diastrabajoendescanso = value;
				}
			}
		}





		#endregion
		// ====================================================================================================


	}
}
