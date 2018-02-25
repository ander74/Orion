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
using Orion.Config;
using System.Data.OleDb;
using System.Windows;

namespace Orion.Models {


	public class RegulacionConductor: NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public RegulacionConductor() { }


		public RegulacionConductor(OleDbDataReader lector) {
			FromReader(lector);
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Fecha": Fecha = new DateTime(2001,1,2); break;
				case "Horas": Horas = new TimeSpan(0); break;
				case "Descansos": Descansos = 0; break;
				case "Motivo": Motivo = ""; break;
			}
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = DBUtils.FromReaderEnteroLargo(lector, "Id");//(lector["Id"] is DBNull) ? 0 : (int)lector["Id"];
			_idconductor = DBUtils.FromReaderEnteroLargo(lector, "IdConductor");//(lector["IdConductor"] is DBNull) ? 0 : (int)lector["IdConductor"];
			_codigo = DBUtils.FromReaderEntero(lector, "Codigo");//(lector["Codigo"] is DBNull) ? 0 : (Int16)lector["Codigo"];
			_fecha = (lector["Fecha"] is DBNull) ? new DateTime(2001, 1, 2) : (DateTime)lector["Fecha"];
			_horas = Utils.ReaderToHoraSinNulo(lector, "Horas");
			_descansos = DBUtils.FromReaderEntero(lector, "Descansos");//(lector["Descansos"] is DBNull) ? 0 : (Int16)lector["Descansos"];
			_motivo = DBUtils.FromReaderTexto(lector, "Motivo");//(lector["Motivo"] is DBNull) ? "" : (string)lector["Motivo"];
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS SOBRECARGADOS
		// ====================================================================================================
		public override bool Equals(object obj) {
			if (!(obj is RegulacionConductor)) return false;
			RegulacionConductor r = obj as RegulacionConductor;
			if (r.Codigo != Codigo || r.Fecha.Ticks != Fecha.Ticks) return false;
			if (r.Horas != Horas) return false;
			if (r.Descansos != Descansos) return false;
			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}


		public override string ToString() {
			return Fecha.ToString(@"dd-MM-yyyy") + ": " + Horas.ToTexto() + " horas y " + Descansos + " descansos (" + Motivo + ").";
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================
		public static void ParseFromReader(OleDbDataReader lector, RegulacionConductor regulacion) {
			regulacion.Id = DBUtils.FromReaderEnteroLargo(lector, "Id");//(lector["Id"] is DBNull) ? 0 : (int)lector["Id"];
			regulacion.IdConductor = DBUtils.FromReaderEnteroLargo(lector, "IdConductor");//(lector["IdConductor"] is DBNull) ? 0 : (int)lector["IdConductor"];
			regulacion.Codigo = DBUtils.FromReaderEntero(lector, "Codigo");//(lector["Codigo"] is DBNull) ? 0 : (int)lector["Codigo"];
			regulacion.Fecha = (lector["Fecha"] is DBNull) ? new DateTime(2001, 1, 2) : (DateTime)lector["Fecha"];
			regulacion.Horas = Utils.ReaderToHoraSinNulo(lector, "Horas");
			regulacion.Descansos = DBUtils.FromReaderEntero(lector, "Descansos");//(lector["Descansos"] is DBNull) ? 0 : (int)lector["Descansos"];
			regulacion.Motivo = DBUtils.FromReaderTexto(lector, "Motivo");//(lector["Motivo"] is DBNull) ? "" : (string)lector["Motivo"];
		}


		public static void ParseToCommand(OleDbCommand Comando, RegulacionConductor regulacion) {
			Comando.Parameters.AddWithValue("idconductor", regulacion.IdConductor);
			Comando.Parameters.AddWithValue("codigo", regulacion.Codigo);
			Comando.Parameters.AddWithValue("fecha", regulacion.Fecha.ToString("yyyy-MM-dd"));
			Comando.Parameters.AddWithValue("horas", regulacion.Horas.Ticks);
			Comando.Parameters.AddWithValue("descansos", regulacion.Descansos);
			Comando.Parameters.AddWithValue("motivo", regulacion.Motivo);
			Comando.Parameters.AddWithValue("id", regulacion.Id);
		}

		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private int _id;
		public int Id {
			get { return _id; }
			set {
				if (_id != value) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _idconductor;
		public int IdConductor {
			get { return _idconductor; }
			set {
				if (_idconductor != value) {
					_idconductor = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _codigo;
		public int Codigo {
			get { return _codigo; }
			set {
				if (_codigo != value) {
					_codigo = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private DateTime _fecha;
		public DateTime Fecha {
			get { return _fecha; }
			set {
				if (_fecha != value) {
					_fecha = value;
					Modificado = true;
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
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _descansos;
		public int Descansos {
			get { return _descansos; }
			set {
				if (_descansos != value) {
					_descansos = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _motivo = "";
		public string Motivo {
			get { return _motivo; }
			set {
				if (value == "0") { Codigo = 0; Modificado = true; PropiedadCambiada(); return; }
				if (value == "1") { Codigo = 1; Modificado = true; PropiedadCambiada(); return; }
				if (value == "2") { Codigo = 2; Modificado = true; PropiedadCambiada(); return; }
				if (_motivo != value) {
					_motivo = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion







	}
}
