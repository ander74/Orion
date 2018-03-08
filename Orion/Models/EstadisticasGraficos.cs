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
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {


	public class EstadisticasGraficos {


		// ====================================================================================================
		#region  CONSTRUCTORES
		// ====================================================================================================

		public EstadisticasGraficos() { }


		public EstadisticasGraficos(OleDbDataReader lector) {
			FromReader(lector);
		}

		#endregion


		// ====================================================================================================
		#region  MÉTODOS PÚBLICOS
		// ====================================================================================================

		public void FromReader(OleDbDataReader lector) {
			_validez = DBUtils.FromReaderFechaHora(lector, "xValidez");
			_turno = DBUtils.FromReaderEntero(lector, "xTurno");
			_numerograficos = DBUtils.FromReaderEnteroLargo(lector, "xNumero");
			_valoracion = Utils.ReaderToHora(lector, "xValoracion").Value;
			_trabajadas = Utils.ReaderToHora(lector, "xTrabajadas").Value;
			_acumuladas = Utils.ReaderToHora(lector, "xAcumuladas").Value;
			_nocturnas = Utils.ReaderToHora(lector, "xNocturnas").Value;
			_desayuno = DBUtils.FromReaderDecimal(lector, "xDesayuno");
			_comida = DBUtils.FromReaderDecimal(lector, "xComida");
			_cena = DBUtils.FromReaderDecimal(lector, "xCena");
			_pluscena = DBUtils.FromReaderDecimal(lector, "xPlusCena");
			_limpieza = (int)DBUtils.FromReaderDoble(lector, "xLimpieza");
			_paqueteria =(int)DBUtils.FromReaderDoble(lector, "xPaqueteria");
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region  PROPIEDADES
		// ====================================================================================================


		private Centros _centro;
		public Centros Centro {
			get { return _centro; }
			set { _centro = value; }
		}


		private DateTime _validez;
		public DateTime Validez {
			get { return _validez; }
			set { _validez = value; }
		}

		
		private int _turno;
		public int Turno {
			get { return _turno; }
			set { _turno = value; }
		}


		private int _numerograficos;
		public int NumeroGraficos {
			get { return _numerograficos; }
			set { _numerograficos = value; }
		}


		private TimeSpan _valoracion;
		public TimeSpan Valoracion {
			get { return _valoracion; }
			set { _valoracion = value; }
		}


		private TimeSpan _trabajadas;
		public TimeSpan Trabajadas {
			get { return _trabajadas; }
			set { _trabajadas = value; }
		}

		private TimeSpan _acumuladas;
		public TimeSpan Acumuladas {
			get { return _acumuladas; }
			set { _acumuladas = value; }
		}


		private TimeSpan _nocturnas;
		public TimeSpan Nocturnas {
			get { return _nocturnas; }
			set { _nocturnas = value; }
		}


		private decimal _desayuno;
		public decimal Desayuno {
			get { return _desayuno; }
			set { _desayuno = value; }
		}


		private decimal _comida;
		public decimal Comida {
			get { return _comida; }
			set { _comida = value; }
		}


		private decimal _cena;
		public decimal Cena {
			get { return _cena; }
			set { _cena = value; }
		}


		private decimal _pluscena;
		public decimal PlusCena {
			get { return _pluscena; }
			set { _pluscena = value; }
		}


		public decimal TotalDietas {
			get {
				return (_desayuno * App.Global.Convenio.PorcentajeDesayuno / 100m) + Comida + Cena + PlusCena;
			}
		}


		private int _limpieza;
		public int Limpieza {
			get { return _limpieza; }
			set { _limpieza = value; }
		}


		private int _paqueteria;
		public int Paqueteria {
			get { return _paqueteria; }
			set { _paqueteria = value; }
		}

		


		#endregion
		// ====================================================================================================





	}
}
