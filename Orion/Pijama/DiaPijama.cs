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
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Pijama {

	public class DiaPijama: DiaCalendarioBase {


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public DiaPijama() : base() { }


		public DiaPijama(OleDbDataReader lector): base(lector) { }


		public DiaPijama(DiaCalendarioBase dia) : base(dia) { }

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private GraficoBase _graficotrabajado;
		public GraficoBase GraficoTrabajado {
			get { return _graficotrabajado; }
			set {
				if (_graficotrabajado != value) {
					_graficotrabajado = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _pluslimpieza;
		public decimal PlusLimpieza {
			get { return _pluslimpieza; }
			set {
				if (_pluslimpieza != value) {
					_pluslimpieza = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _pluspaqueteria;
		public decimal PlusPaqueteria {
			get { return _pluspaqueteria; }
			set {
				if (_pluspaqueteria != value) {
					_pluspaqueteria = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _plusmenordescanso;
		public decimal PlusMenorDescanso {
			get { return _plusmenordescanso; }
			set {
				if (_plusmenordescanso != value) {
					_plusmenordescanso = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _plusnocturnidad;
		public decimal PlusNocturnidad {
			get { return _plusnocturnidad; }
			set {
				if (_plusnocturnidad != value) {
					_plusnocturnidad = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _plusnavidad;
		public decimal PlusNavidad {
			get { return _plusnavidad; }
			set {
				if (_plusnavidad != value) {
					_plusnavidad = value;
					PropiedadCambiada();
				}
			}
		}


		public decimal OtrosPluses{
			get {
				return PlusNocturnidad + PlusNavidad;
			}
		}


		public decimal TotalDietas {
			get {
				return (GraficoTrabajado.Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) + GraficoTrabajado.Comida + 
						GraficoTrabajado.Cena + GraficoTrabajado.PlusCena;
			}
		}


		public string TextoInicioFinal {
			get {
				string texto = null;
				if (GraficoTrabajado != null && GraficoTrabajado.Numero > 0) {
					if (GraficoTrabajado.Inicio.HasValue && GraficoTrabajado.Final.HasValue)
						texto = String.Format($"{GraficoTrabajado.Inicio.Value.ToTexto()} - {GraficoTrabajado.Final.Value.ToTexto()}");
					if (GraficoTrabajado.InicioPartido.HasValue && GraficoTrabajado.FinalPartido.HasValue)
						texto += string.Format($"    ({GraficoTrabajado.InicioPartido.Value.ToTexto()} - {GraficoTrabajado.FinalPartido.Value.ToTexto()})");
				}
				return texto;
			}
		}


		private bool _esfestivo;
		public bool EsFestivo {
			get { return _esfestivo; }
			set {
				if (_esfestivo != value) {
					_esfestivo = value;
					PropiedadCambiada();
				}
			}
		}



		#endregion
		// ====================================================================================================



	}
}
