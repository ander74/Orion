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
using System.Data.OleDb;
using System.Windows;
using Orion.Config;

namespace Orion.Models {

	public class GrupoGraficos: NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public GrupoGraficos() { }


		public GrupoGraficos(OleDbDataReader lector) {
			FromReader(lector);
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = DBUtils.FromReaderEnteroLargo(lector, "Id");//(lector["Id"] is DBNull) ? 0 : (Int32)lector["Id"];
			_validez = DBUtils.FromReaderFechaHora(lector, "Validez");//(lector["Validez"] is DBNull) ? new DateTime(2001,1,2) : (DateTime)lector["Validez"];
			_notas = DBUtils.FromReaderTexto(lector, "Notas");//(lector["Notas"] is DBNull) ? "" : (string)lector["Notas"];
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS SOBRECARGADOS
		// ====================================================================================================
		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, GrupoGraficos grupo) {
			grupo.Id = DBUtils.FromReaderEnteroLargo(lector, "Id");//(lector["Id"] is DBNull) ? 0 : (Int32)lector["Id"];
			grupo.Validez = DBUtils.FromReaderFechaHora(lector, "Validez");//(lector["Validez"] is DBNull) ? new DateTime(0) : (DateTime)lector["Validez"];
			grupo.Notas = DBUtils.FromReaderTexto(lector, "Notas");//(lector["Notas"] is DBNull) ? "" : (string)lector["Notas"];
		}


		public static void ParseToCommand(OleDbCommand Comando, GrupoGraficos grupo) {
			Comando.Parameters.AddWithValue("validez", grupo.Validez.ToString("yyyy-MM-dd"));
			Comando.Parameters.AddWithValue("notas", grupo.Notas);
			Comando.Parameters.AddWithValue("id", grupo.Id);
		}
		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private long _id;
		public long Id {
			get { return _id; }
			set {
				if (value != _id) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}

		private DateTime _validez;
		public DateTime Validez {
			get { return _validez; }
			set {
				if (value != _validez) {
					_validez = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _notas = "";
		public string Notas {
			get { return _notas; }
			set {
				if (value != _notas) {
					_notas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion



	} //Final de clase.
}
