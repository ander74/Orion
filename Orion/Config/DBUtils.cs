#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Config {

	/// <summary>
	/// Contiene métodos estáticos que extraen los valores de un DataReader a su equivalente en C#, teniendo en cuenta los valores DBNull.
	/// </summary>
	public static class DBUtils {


		/// <summary>
		/// Devuelve un string desde un objeto Texto en una base de datos.
		/// </summary>
		public static string FromReaderTexto(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return "";
			return (string)lector[campo];
		}


		/// <summary>
		/// Devuelve un bool desde un objeto SiNo en una base de datos.
		/// </summary>
		public static bool FromReaderSiNo(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return false;
			return (bool)lector[campo];
		}


		/// <summary>
		/// Devuelve un DateTime desde un objeto FechaHora en una base de datos.
		/// </summary>
		public static DateTime FromReaderFechaHora(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return new DateTime(0);
			return (DateTime)lector[campo];
		}


		/// <summary>
		/// Devuelve un DateTime que puede ser nulo desde un objeto FechaHora en una base de datos.
		/// </summary>
		public static DateTime? FromReaderFechaHoraNulo(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return null;
			return (DateTime)lector[campo];
		}


		/// <summary>
		/// Devuelve un byte desde un objeto Numero:Byte en una base de datos.
		/// </summary>
		public static byte FromReaderByte(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (byte)lector[campo];
		}


		/// <summary>
		/// Devuelve un Single desde un objeto Numero:Único en una base de datos.
		/// </summary>
		public static Single FromReaderUnico(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Single)lector[campo];
		}


		/// <summary>
		/// Devuelve un Int16 desde un objeto Numero:Entero en una base de datos.
		/// </summary>
		public static Int16 FromReaderEntero(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Int16)lector[campo];
		}


		/// <summary>
		/// Devuelve un Int32 desde un objeto Numero:Entero Largo en una base de datos.
		/// </summary>
		public static Int32 FromReaderEnteroLargo(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Int32)lector[campo];
		}


		/// <summary>
		/// Devuelve un double desde un objeto Numero:Doble en una base de datos.
		/// </summary>
		public static double FromReaderDoble(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (double)lector[campo];
		}


		/// <summary>
		/// Devuelve un decimal desde un objeto Moneda o un objeto Numero:Decimal en una base de datos.
		/// </summary>
		public static decimal FromReaderDecimal(OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (decimal)lector[campo];
		}


	}
}
