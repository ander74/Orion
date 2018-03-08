#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

	public class OpcionesPorCentro: NotifyBase {


		// ====================================================================================================
		#region GRÁFICOS
		// ====================================================================================================

		private int _lundel = 0;
		public int LunDel {
			get { return _lundel; }
			set {
				if (_lundel != value) {
					_lundel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _lunal = 0;
		public int LunAl {
			get { return _lunal; }
			set {
				if (_lunal != value) {
					_lunal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _viedel = 0;
		public int VieDel {
			get { return _viedel; }
			set {
				if (_viedel != value) {
					_viedel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _vieal = 0;
		public int VieAl {
			get { return _vieal; }
			set {
				if (_vieal != value) {
					_vieal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _sabdel = 0;
		public int SabDel {
			get { return _sabdel; }
			set {
				if (_sabdel != value) {
					_sabdel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _sabal = 0;
		public int SabAl {
			get { return _sabal; }
			set {
				if (_sabal != value) {
					_sabal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _domdel = 0;
		public int DomDel {
			get { return _domdel; }
			set {
				if (_domdel != value) {
					_domdel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _domal = 0;
		public int DomAl {
			get { return _domal; }
			set {
				if (_domal != value) {
					_domal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _comodin = 0;
		public int Comodin {
			get { return _comodin; }
			set {
				if (_comodin != value) {
					_comodin = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region LIMPIEZAS
		// ====================================================================================================

		private bool _pagarlimpiezas = false;
		public bool PagarLimpiezas {
			get { return _pagarlimpiezas; }
			set {
				if (_pagarlimpiezas != value) {
					_pagarlimpiezas = value;
					PropiedadCambiada();
				}
			}
		}


		private int _numerolimpiezas = 21;
		public int NumeroLimpiezas {
			get { return _numerolimpiezas; }
			set {
				if (_numerolimpiezas != value) {
					_numerolimpiezas = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PLUS VIAJE
		// ====================================================================================================

		private bool _pagarplusviaje = false;
		public bool PagarPlusViaje {
			get { return _pagarplusviaje; }
			set {
				if (_pagarplusviaje != value) {
					_pagarplusviaje = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _plusviaje = 0;
		public decimal PlusViaje {
			get { return _plusviaje; }
			set {
				if (_plusviaje != value) {
					_plusviaje = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS
		// ====================================================================================================

		public void Guardar(string ruta) {

			string datos = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(ruta, datos);
		}

		public void Cargar(string ruta) {

			if (File.Exists(ruta)) {
				string datos = File.ReadAllText(ruta);
				JsonConvert.PopulateObject(datos, this);
				PropiedadCambiada("");
			}
		}

		#endregion
		// ====================================================================================================





	}
}
