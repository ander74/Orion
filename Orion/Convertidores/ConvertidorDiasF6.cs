#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Orion.Config;

namespace Orion.Convertidores {

	/// <summary>
	/// Convierte la hora pasada a texto y viceversa.\n
	///	Se aplican los valores NoCeros, HoraPlus y Negativo de la enumeración VerValores.
	/// </summary>
	[ValueConversion(typeof(int), typeof(string))]
	class ConvertidorDiasF6 :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un TimeSpan, devolvemos una cadena vacía.
			if (value == null || !(value is int)) return "";
			int dias = (int)value;
			if (dias == 0) return "00";
			TimeSpan hora = new TimeSpan( dias * App.Global.Convenio.JornadaMedia.Ticks);
			int horas = (int)Math.Truncate(hora.TotalHours);
			string resultado = dias.ToString("00") + " = -";
			resultado += horas.ToString("00") + ":" + (hora.Minutes).ToString("00") + " (-" + hora.TotalHours.ToString("0.00") + ")";
			return resultado;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}



}
