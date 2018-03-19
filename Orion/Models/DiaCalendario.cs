#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

	public class DiaCalendario: DiaCalendarioBase {


		// ====================================================================================================
		#region  CONSTRUCTORES
		// ====================================================================================================
		public DiaCalendario() : base() {
			PropertyChanged += LlamarPropiedadCambiada;
		}

		public DiaCalendario(OleDbDataReader lector): base(lector) {
			PropertyChanged += LlamarPropiedadCambiada;
		}

		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================

		private void LlamarPropiedadCambiada(object sender, PropertyChangedEventArgs e) {
			switch (e.PropertyName) {
				case nameof(ExcesoJornada):
					PropiedadCambiada(nameof(HayExtras));
					break;
				case nameof(FacturadoPaqueteria):
					PropiedadCambiada(nameof(HayExtras));
					break;
				case nameof(Limpieza):
					PropiedadCambiada(nameof(HayExtras));
					break;
				case nameof(Notas):
					PropiedadCambiada(nameof(HayNotas));
					PropiedadCambiada(nameof(HayExtras));
					break;
			}
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		public string HayNotas {
			get {
				if (String.IsNullOrEmpty(Notas)) return "";
				return "...";
			}
		}


		public bool HayExtras {
			get {
				if (ExcesoJornada.Ticks != 0) return true;
				if (FacturadoPaqueteria != 0) return true;
				if (Limpieza != false) return true;
				if (!String.IsNullOrWhiteSpace(Notas)) return true;
				return false;
			}
		}


		private bool _resaltar;
		public bool Resaltar {
			get { return _resaltar; }
			set {
				if (_resaltar != value) {
					_resaltar = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion


	}
}
