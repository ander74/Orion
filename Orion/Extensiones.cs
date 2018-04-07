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

namespace Orion {

	public static class Extensiones {

		//====================================================================================================
		// CONSTANTES
		//====================================================================================================
		/// <summary>
		/// Número de ticks de la hora máxima que puede tener un elemento TimeSpan en el entorno Orion.
		/// </summary>
		public const long HoraMáxima = 1_079_400_000_000;


		//====================================================================================================
		// MÉTODOS DE EXTENSIÓN PARA TIMESPAN y TIMESPAN?
		//====================================================================================================
		/// <summary>
		/// Devuelve la hora en formato texto (hh:mm).
		/// </summary>
		/// <param name="hora">Hora que se desea devolver.</param>
		/// <param name="esPlus">Opcional. Si es true, se admiten horas desde las 00:00 hasta las 29:59.</param>
		/// <returns>Hora en formato hh:mm</returns>
		public static string ToTexto(this TimeSpan hora, bool esPlus = false) {
			if (hora.Ticks < 0) return "";
			int horas;
			if (esPlus) {
				horas = (hora.Days * 24) + hora.Hours;
			} else if (hora.Ticks <= HoraMáxima) {
				horas = hora.Hours;
			} else {
				return "";
			}
			return horas.ToString("00") + ":" + hora.Minutes.ToString("00");
		}

		/// <summary>
		/// Devuelve el valor decimal de las horas de un timespan redondeado a cuatro decimales.
		/// </summary>
		/// <param name="hora">Hora a devolver en decimal</param>
		/// <returns>Decimal con las horas que representan el timespan. Se redondea a cuatro decimales.</returns>
		public static decimal ToDecimal(this TimeSpan hora) {
			return Decimal.Round((decimal)(hora.TotalHours), 4);
		}

		/// <summary>
		/// Devuelve la hora en formato texto (hh:mm).
		/// </summary>
		/// <param name="hora">Hora que se desea devolver.</param>
		/// <param name="esPlus">Opcional. Si es true, se admiten horas desde las 00:00 hasta las 29:59.</param>
		/// <returns>Hora en formato hh:mm</returns>
		public static string ToTexto(this TimeSpan? hora, bool esPlus = false) {
			if (!hora.HasValue) return "";
			if (hora.Value.Ticks < 0) return "";
			int horas;
			if (esPlus) {
				horas = (hora.Value.Days * 24) + hora.Value.Hours;
			} else if (hora.Value.Ticks <= HoraMáxima) {
				horas = hora.Value.Hours;
			} else {
				return "";
			}
			return horas.ToString("00") + ":" + hora.Value.Minutes.ToString("00");
		}

		/// <summary>
		/// Devuelve el valor decimal de las horas de un timespan redondeado a cuatro decimales.
		/// </summary>
		/// <param name="hora">Hora a devolver en decimal</param>
		/// <returns>Decimal con las horas que representan el timespan. Se redondea a cuatro decimales.</returns>
		public static decimal ToDecimal(this TimeSpan? hora) {
			if (!hora.HasValue) return 0m;
			return Decimal.Round((decimal)(hora.Value.TotalHours), 4);
		}


		// ====================================================================================================
		#region MÉTODOS DE EXTENSION PARA OLEDBDATAREADER
		// ====================================================================================================

		public static TimeSpan ToTimeSpan(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return TimeSpan.Zero;
			return TimeSpan.FromTicks((long)(double)lector[campo]);
		}

		public static TimeSpan? ToTimeSpanNulable(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return null;
			return TimeSpan.FromTicks((long)(double)lector[campo]);
		}

		public static string ToString(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return "";
			return (string)lector[campo];
		}

		public static bool ToBoolean(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return false;
			return (bool)lector[campo];
		}

		public static DateTime ToDateTime(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return new DateTime(0);
			return (DateTime)lector[campo];
		}

		public static DateTime? ToDateTimeNulable(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return null;
			return (DateTime)lector[campo];
		}

		public static byte ToByte(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (byte)lector[campo];
		}

		public static Single ToSingle(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Single)lector[campo];
		}

		public static Int16 ToInt16(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Int16)lector[campo];
		}

		public static Int32 ToInt32(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (Int32)lector[campo];
		}

		public static double ToDouble(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (double)lector[campo];
		}

		public static decimal ToDecimal(this OleDbDataReader lector, string campo) {
			if (lector == null || lector[campo] is DBNull) return 0;
			return (decimal)lector[campo];
		}





		#endregion
		// ====================================================================================================


	}
}
