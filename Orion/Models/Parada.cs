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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

	public class Parada: NotifyBase {


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================

		public Parada() {
		}


		public Parada(OleDbDataReader lector) {
			FromReader(lector);
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Orden": Orden = 0; break;
				case "Descripcion": Descripcion = ""; break;
				case "Tiempo": Tiempo = TimeSpan.Zero; break;
			}
		}


		public void FromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");//(lector["Id"] is DBNull) ? 0 : (Int32)lector["Id"];
			_iditinerario = lector.ToInt32("IdItinerario");//(lector["IdItinerario"] is DBNull) ? 0 : (Int32)lector["IdItinerario"];
			_orden = lector.ToInt16("Orden");//(lector["Orden"] is DBNull) ? 0 : (Int16)lector["Orden"];
			_descripcion = lector.ToString("Descripcion");//(lector["Descripcion"] is DBNull) ? "" : (string)lector["Descripcion"];
			_tiempo = lector.ToTimeSpan("Tiempo");
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================

		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, Parada parada) {
			parada.Id = lector.ToInt32("Id");
			parada.IdItinerario = lector.ToInt32("IdIntinerario");
			parada.Orden = lector.ToInt16("Orden");
			parada.Descripcion = lector.ToString("Descripcion");
			parada.Tiempo = lector.ToTimeSpan("Tiempo");
		}


		public static void ParseToCommand(OleDbCommand Comando, Parada parada) {
			Comando.Parameters.AddWithValue("iditinerario", parada.IdItinerario);
			Comando.Parameters.AddWithValue("orden", parada.Orden);
			Comando.Parameters.AddWithValue("descripcion", parada.Descripcion);
			Comando.Parameters.AddWithValue("Tiempo", parada.Tiempo.Ticks);
			Comando.Parameters.AddWithValue("id", parada.Id);
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS SOBRECARGADOS
		// ====================================================================================================

		public override bool Equals(object obj) {
			if (!(obj is Parada)) return false;
			Parada p = obj as Parada;
			if (p.Orden != Orden) return false;
			if (p.Descripcion != Descripcion) return false;
			if (p.Tiempo != Tiempo) return false;
			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}


		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private long _id;
		public long Id {
			get { return _id; }
			set {
				if (_id != value) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private long _iditinerario;
		public long IdItinerario {
			get { return _iditinerario; }
			set {
				if (_iditinerario != value) {
					_iditinerario = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _orden;
		public int Orden {
			get { return _orden; }
			set {
				if (_orden != value) {
					_orden = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _descripcion = "";
		public string Descripcion {
			get { return _descripcion; }
			set {
				if (_descripcion != value) {
					_descripcion = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _tiempo;
		public TimeSpan Tiempo {
			get { return _tiempo; }
			set {
				if (_tiempo != value) {
					_tiempo = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion



	}
}
