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
	[ValueConversion(typeof(TimeSpan), typeof(string))]
	class ConvertidorHora : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un TimeSpan, devolvemos una cadena vacía.
			if (value == null || !(value is TimeSpan?)) return "";
			// Evaluamos el parámetro pasado.
			VerValores ver = VerValores.Normal;
			if (parameter != null && parameter is VerValores) ver = (VerValores)parameter;
			// Asignamos el valor de la hora.
			TimeSpan hora = (TimeSpan)value;

			if (hora.Ticks == 0) {
				// Si la hora es cero y no se quieren mostrar los ceros, devolvemos una cadena vacía.
				if (ver.HasFlag(VerValores.NoCeros)) return "";
			} else if (hora.Ticks > 0) {
				// Si la hora es mayor que cero y queremos hora plus, devolvemos la hora con el plus activado.
				if (ver.HasFlag(VerValores.HoraPlus)) {
					string xxx = hora.ToTexto(true);
					return xxx;
				}
			} else {
				// Si la hora es negativa y queremos horas negativas, devolvemos la hora en negativo.
				if (ver.HasFlag(VerValores.Negativo)) {
					hora = new TimeSpan(hora.Ticks * -1);
					return "-" + hora.ToTexto();
				}
			}
			// Devolvemos la hora en el formato tradicional.
			return hora.ToTexto();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un string, devolvemos nulo
			if (value == null || !(value is string)) return null;
			// Evaluamos el parámetro pasado.
			VerValores ver = VerValores.Normal;
			if (parameter != null && parameter is VerValores) ver = (VerValores)parameter;
			// Asignamos el valor del texto y la hora que se va a devolver.
			string texto = value as string;
			// Parseamos la hora en función del parámetro.
			if (Utils.ParseHora(texto, out TimeSpan hora, ver.HasFlag(VerValores.HoraPlus))) {
				return hora;
			} else {
				return null;
			}
		}
	}



}
