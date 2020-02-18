#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;

namespace Orion.Pijama {

	public class ResumenPijama {


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private TimeSpan _horasacumuladas;
		/// <summary>
		/// Horas acumuladas.
		/// </summary>
		public TimeSpan HorasAcumuladas {
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


		private decimal _diaslibredisposicionf6;
		/// <summary>
		/// Días de libre disposición.
		/// </summary>
		public decimal DiasLibreDisposicionF6 {
			get { return _diaslibredisposicionf6; }
			set {
				if (_diaslibredisposicionf6 != value) {
					_diaslibredisposicionf6 = value;
				}
			}
		}


		private decimal _diasF6DC;
		/// <summary>
		/// Días de libre disposición.
		/// </summary>
		public decimal DiasF6DC {
			get { return _diasF6DC; }
			set {
				if (_diasF6DC != value) {
					_diasF6DC = value;
				}
			}
		}


		private decimal _dcsregulados;
		/// <summary>
		/// Descansos compensatorios regulados (se hace un computo de todos).
		/// </summary>
		public decimal DCsRegulados {
			get { return _dcsregulados; }
			set {
				if (_dcsregulados != value) {
					_dcsregulados = value;
				}
			}
		}


		private decimal _dcsdisfrutados;
		/// <summary>
		/// Descansos compensatorios disfrutados.
		/// </summary>
		public decimal DCsDisfrutados {
			get { return _dcsdisfrutados; }
			set {
				if (_dcsdisfrutados != value) {
					_dcsdisfrutados = value;
				}
			}
		}


		private decimal _dndsregulados;
		/// <summary>
		/// Descansos no disfrutados que se han disfrutado.
		/// </summary>
		public decimal DNDsRegulados {
			get { return _dndsregulados; }
			set {
				if (_dndsregulados != value) {
					_dndsregulados = value;
				}
			}
		}


		private decimal _dndsdisfrutados;
		/// <summary>
		/// Descansos no disfrutados que se han disfrutado.
		/// </summary>
		public decimal DNDsDisfrutados {
			get { return _dndsdisfrutados; }
			set {
				if (_dndsdisfrutados != value) {
					_dndsdisfrutados = value;
				}
			}
		}


		private decimal _diascomiteendescanso;
		/// <summary>
		/// Días de comite que se han tenido un día de descanso.
		/// </summary>
		public decimal DiasComiteEnDescanso {
			get { return _diascomiteendescanso; }
			set {
				if (_diascomiteendescanso != value) {
					_diascomiteendescanso = value;
				}
			}
		}


		private decimal _diastrabajoendescanso;
		/// <summary>
		/// Días trabajados en un día de descanso.
		/// </summary>
		public decimal DiasTrabajoEnDescanso {
			get { return _diastrabajoendescanso; }
			set {
				if (_diastrabajoendescanso != value) {
					_diastrabajoendescanso = value;
				}
			}
		}


		/// <summary>
		/// Días de vacaciones disfrutados.
		/// </summary>
		private int diasVacaciones;
		public int DiasVacaciones {
			get => diasVacaciones;
			set {
				if (diasVacaciones != value) {
					diasVacaciones = value;
				}
			}

		}

		/// <summary>
		/// Días de vacaciones disfrutados.
		/// </summary>
		private int diasInactivo;
		public int DiasInactivo {
			get => diasInactivo;
			set {
				if (diasInactivo != value) {
					diasInactivo = value;
				}
			}

		}



		public int DiasTrabajados { get; set; }


		public int DiasDescansados => DiasJD + DiasFN + DiasDS;

		public int DiasJD { get; set; }

		public int DiasFN { get; set; }

		public int DiasDS { get; set; }

		public int DiasPermiso { get; set; }

		public int DiasF6 { get; set; }


		public int DiasEnfermo { get; set; }


		public int DiasDC { get; set; }


		public TimeSpan HorasAnuales { get; set; }








		#endregion
		// ====================================================================================================


	}
}
