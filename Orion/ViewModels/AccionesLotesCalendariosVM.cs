#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Orion.ViewModels {

	public class AccionesLotesCalendariosVM: NotifyBase {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private int _deldia = 1;
		public int DelDia {
			get { return _deldia; }
			set {
				if (_deldia != value) {
					_deldia = value;
					PropiedadCambiada();
				}
			}
		}


		private int _aldia = 31;
		public int AlDia {
			get { return _aldia; }
			set {
				if (_aldia != value) {
					_aldia = value;
					PropiedadCambiada();
				}
			}
		}


		private int _grafico = 0;
		public int Grafico {
			get { return _grafico; }
			set {
				if (_grafico != value) {
					_grafico = value;
					PropiedadCambiada();
				}
			}
		}


		private int _codigo = 4;
		public int Codigo {
			get { return _codigo; }
			set {
				if (_codigo != value) {
					_codigo = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _horas;
		public TimeSpan Horas {
			get { return _horas; }
			set {
				if (_horas != value) {
					_horas = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _importe;
		public decimal Importe {
			get { return _importe; }
			set {
				if (_importe != value) {
					_importe = value;
					PropiedadCambiada();
				}
			}
		}


		private int _nuevografico = 0;
		public int NuevoGrafico {
			get { return _nuevografico; }
			set {
				if (_nuevografico != value) {
					_nuevografico = value;
					PropiedadCambiada();
				}
			}
		}


		private int _nuevocodigo = 4;
		public int NuevoCodigo {
			get { return _nuevocodigo; }
			set {
				if (_nuevocodigo != value) {
					_nuevocodigo = value;
					PropiedadCambiada();
				}
			}
		}


		private int _codigoaccion;
		public int CodigoAccion {
			get { return _codigoaccion; }
			set {
				if (_codigoaccion != value) {
					_codigoaccion = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(TextoValorPedido));
					PropiedadCambiada(nameof(VisibilidadHoras));
					PropiedadCambiada(nameof(VisibilidadImporte));
					PropiedadCambiada(nameof(VisibilidadNumero));
				}
			}
		}


		private bool _sumarvalor = true;
		public bool SumarValor {
			get { return _sumarvalor; }
			set {
				if (_sumarvalor != value) {
					_sumarvalor = value;
					PropiedadCambiada();
				}
			}
		}


		private string _notas;
		public string Notas {
			get { return _notas; }
			set {
				if (_notas != value) {
					_notas = value;
					PropiedadCambiada();
				}
			}
		}


		public string TextoValorPedido {
			get {
				switch (CodigoAccion) {
					case 0: return "Horas: ";
					case 1: return "Importe: ";
					case 5: return "Nuevo número: ";
					default: return "";
				}
			}
		}


		public Visibility VisibilidadHoras {
			get {
				switch (CodigoAccion) {
					case 0: return Visibility.Visible;
					default: return Visibility.Collapsed;
				}
			}
		}


		public Visibility VisibilidadImporte {
			get {
				switch (CodigoAccion) {
					case 1: return Visibility.Visible;
					default: return Visibility.Collapsed;
				}
			}
		}


		public Visibility VisibilidadNumero {
			get {
				switch (CodigoAccion) {
					case 5: return Visibility.Visible;
					default: return Visibility.Collapsed;
				}
			}
		}




		#endregion


	}
}
