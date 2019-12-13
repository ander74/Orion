#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Data.OleDb;
using Microsoft.Data.Sqlite;

namespace Orion {

    public static class Extensiones {

        // ====================================================================================================
        #region CONSTANTES
        // ====================================================================================================

        /// <summary>
        /// Número de ticks de la hora máxima que puede tener un elemento TimeSpan en el entorno Orion.
        /// </summary>
        public const long HoraMáxima = 1_079_400_000_000;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS DE EXTENSIÓN PARA TIMESPAN y TIMESPAN?
        // ====================================================================================================

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
        /// Devuelve una hora en formato texto (hh:mm) admitiendo valores negativos.
        /// </summary>
        /// <param name="hora">Hora que se desea devolver.</param>
        /// <returns>Hora en formato hh:mm</returns>
        public static string ToTextoNegativo(this TimeSpan hora) {
            int horas = (int)Math.Truncate(hora.TotalHours);
            int minutos = hora.Minutes;
            if (minutos < 0) minutos *= -1;
            return horas.ToString("00") + ":" + minutos.ToString("00");
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

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS DE EXTENSION PARA OLEDBDATAREADER
        // ====================================================================================================

        /// <summary>
        /// Extrae un campo de tipo TimeSpan del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un TimeSpan con el valor almacenado en el campo.</returns>
        public static TimeSpan ToTimeSpan(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return TimeSpan.Zero;
            return TimeSpan.FromTicks((long)(double)lector[campo]);
        }


        /// <summary>
        /// Extrae un campo de tipo TimeSpan? del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un TimeSpan? con el valor almacenado en el campo.</returns>
        public static TimeSpan? ToTimeSpanNulable(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return TimeSpan.FromTicks((long)(double)lector[campo]);
        }


        /// <summary>
        /// Extrae un campo de tipo String del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un String con el valor almacenado en el campo.</returns>
        public static string ToString(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return "";
            return (string)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Bool del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Bool con el valor almacenado en el campo.</returns>
        public static bool ToBool(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return false;
            return (bool)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Bool? del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Bool? con el valor almacenado en el campo.</returns>
        public static bool? ToBoolNulable(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return Convert.ToBoolean(lector[campo]);
        }


        /// <summary>
        /// Extrae un campo de tipo DateTime del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un DateTime con el valor almacenado en el campo.</returns>
        public static DateTime ToDateTime(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return new DateTime(0);
            return (DateTime)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo DateTime? del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un DateTime? con el valor almacenado en el campo.</returns>
        public static DateTime? ToDateTimeNulable(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return (DateTime)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Byte del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Byte con el valor almacenado en el campo.</returns>
        public static byte ToByte(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (byte)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Single del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Single con el valor almacenado en el campo.</returns>
        public static Single ToSingle(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (Single)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Int16 del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Int16 con el valor almacenado en el campo.</returns>
        public static Int16 ToInt16(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (Int16)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Int16? del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Int16? con el valor almacenado en el campo.</returns>
        public static Int16? ToInt16Nulable(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return (Int16?)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Int32(int) del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Int32(int) con el valor almacenado en el campo.</returns>
        public static Int32 ToInt32(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (Int32)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Double del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Double con el valor almacenado en el campo.</returns>
        public static double ToDouble(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (double)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Decimal del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Decimal con el valor almacenado en el campo.</returns>
        public static decimal ToDecimal(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (decimal)lector[campo];
        }


        /// <summary>
        /// Extrae un campo de tipo Decimal? del lector.
        /// </summary>
        /// <param name="lector">Lector del que se extraerá el campo.</param>
        /// <param name="campo">Campo que se va a extraer</param>
        /// <returns>Un Decimal? con el valor almacenado en el campo.</returns>
        public static decimal? ToDecimalNulable(this OleDbDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return (decimal)lector[campo];
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS DE EXTENSION PARA SQLITE DATA READER
        // ====================================================================================================

        public static TimeSpan ToTimeSpan(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return TimeSpan.Zero;
            return TimeSpan.FromTicks(Convert.ToInt64(lector[campo]));
        }


        public static TimeSpan? ToTimeSpanNulable(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return TimeSpan.FromTicks(Convert.ToInt64(lector[campo]));
        }


        public static string ToString(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return "";
            return (string)lector[campo];
        }


        public static bool ToBool(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return false;
            return Convert.ToBoolean(lector[campo]);
        }


        public static bool? ToBoolNulable(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return Convert.ToBoolean(lector[campo]);
        }


        public static DateTime ToDateTime(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return new DateTime(0);
            if (!DateTime.TryParse((string)lector[campo], out DateTime fecha)) return new DateTime(0);
            return fecha;
        }


        public static DateTime? ToDateTimeNulable(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            if (!DateTime.TryParse((string)lector[campo], out DateTime fecha)) return null;
            return fecha;
        }


        public static byte ToByte(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (byte)lector[campo];
        }


        public static Single ToSingle(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return (Single)lector[campo];
        }


        public static Int16 ToInt16(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return Convert.ToInt16(lector[campo]);
        }


        public static Int16? ToInt16Nulable(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return Convert.ToInt16(lector[campo]);
        }


        public static Int32 ToInt32(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return Convert.ToInt32(lector[campo]);
        }


        public static Int64 ToInt64(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return Convert.ToInt64(lector[campo]);
        }


        public static double ToDouble(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return Convert.ToDouble(lector[campo]);
        }


        public static decimal ToDecimal(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return 0;
            return Convert.ToDecimal(lector[campo]);
        }


        public static decimal? ToDecimalNulable(this SqliteDataReader lector, string campo) {
            if (lector == null || lector[campo] is DBNull) return null;
            return Convert.ToDecimal(lector[campo]);
        }


        #endregion
        // ====================================================================================================




    }
}
