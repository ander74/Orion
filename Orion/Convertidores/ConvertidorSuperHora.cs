﻿#region COPYRIGHT
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
	class ConvertidorSuperHora :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un TimeSpan, devolvemos una cadena vacía.
			if (value == null || !(value is TimeSpan?)) return "";
			// Evaluamos el parámetro pasado.
			VerValores ver = VerValores.Normal;
			if (parameter != null && parameter is VerValores) ver = (VerValores)parameter;
			// Asignamos el valor de la hora.
			TimeSpan hora = (TimeSpan)value;
			int horas = (int)Math.Truncate(hora.TotalHours);

			if (hora.Ticks == 0) {
				// Si la hora es cero y no se quieren mostrar los ceros, devolvemos una cadena vacía.
				if (ver.HasFlag(VerValores.NoCeros)) return "";
			}
			if (hora.Ticks < 0) {
				string resultado = "";
				if (horas == 0) resultado = "-";
				return resultado + horas.ToString("00") + ":" + (hora.Minutes * -1).ToString("00");
			}
			// Devolvemos la hora en el formato tradicional.
			return horas.ToString("00") + ":" + hora.Minutes.ToString("00");

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un string, devolvemos nulo
			if (value == null || !(value is string)) return null;
			// Asignamos el valor del texto y la hora que se va a devolver.
			string texto = (value as string).Replace(".", ":");
			string[] valores = texto.Split(':');
			bool esnegativo = false;
			if (int.TryParse(valores[0], out int h)) {
				if (valores[0].StartsWith("-")) {
					esnegativo = true;
					h = h * -1;
				}
				int dias = (int)Math.Truncate(h / 24m);
				string m = valores.Length > 1 ? valores[1] : "00";
				texto = dias.ToString() + "." + (h - (dias * 24)).ToString("00") + ":" + m;
			}
			// Parseamos la hora en función del parámetro.
			if (TimeSpan.TryParse(texto, out TimeSpan hora)) {
				return esnegativo ? new TimeSpan(hora.Ticks * -1) : hora;
			} else {
				return null;
			}
		}
	}



}
