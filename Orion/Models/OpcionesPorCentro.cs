#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.IO;
using Newtonsoft.Json;
using Orion.Config;

namespace Orion.Models {

	public class OpcionesPorCentro : NotifyBase {


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


		private int _lunal = 499;
		public int LunAl {
			get { return _lunal; }
			set {
				if (_lunal != value) {
					_lunal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _viedel = 500;
		public int VieDel {
			get { return _viedel; }
			set {
				if (_viedel != value) {
					_viedel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _vieal = 599;
		public int VieAl {
			get { return _vieal; }
			set {
				if (_vieal != value) {
					_vieal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _sabdel = 600;
		public int SabDel {
			get { return _sabdel; }
			set {
				if (_sabdel != value) {
					_sabdel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _sabal = 699;
		public int SabAl {
			get { return _sabal; }
			set {
				if (_sabal != value) {
					_sabal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _domdel = 700;
		public int DomDel {
			get { return _domdel; }
			set {
				if (_domdel != value) {
					_domdel = value;
					PropiedadCambiada();
				}
			}
		}


		private int _domal = 799;
		public int DomAl {
			get { return _domal; }
			set {
				if (_domal != value) {
					_domal = value;
					PropiedadCambiada();
				}
			}
		}


		private int _comodin = 999;
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
		#region NUEVOS GRAFICOS
		// ====================================================================================================

		// ESTO SE CAMBIARÁ POR EL SIGUIENTE BLOQUE COMENTADO CUANDO YA ESTÉ OPERATIVO.
		private string lun;
		public string Lun {
			get => string.IsNullOrEmpty(lun) ? $"{LunDel}-{LunAl}" : lun;
			set => lun = value;
		}

		private string vie;
		public string Vie {
			get => string.IsNullOrEmpty(vie) ? $"{VieDel}-{VieAl}" : vie;
			set => vie = value;
		}

		private string sab;
		public string Sab {
			get => string.IsNullOrEmpty(sab) ? $"{SabDel}-{SabAl}" : sab;
			set => sab = value;
		}

		private string dom;
		public string Dom {
			get => string.IsNullOrEmpty(dom) ? $"{DomDel}-{DomAl}" : dom;
			set => dom = value;
		}

		//public string Lun { get; set; } = "3000-3499";

		//public string Vie { get; set; } = "3500-3599";

		//public string Sab { get; set; } = "3600-3699";

		//public string Dom { get; set; } = "3700-3799";


		[JsonIgnore]
		public IntRangoCollection RangoLun { get => new IntRangoCollection(Lun); }

		[JsonIgnore]
		public IntRangoCollection RangoVie { get => new IntRangoCollection(Vie); }

		[JsonIgnore]
		public IntRangoCollection RangoSab { get => new IntRangoCollection(Sab); }

		[JsonIgnore]
		public IntRangoCollection RangoDom { get => new IntRangoCollection(Dom); }


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
