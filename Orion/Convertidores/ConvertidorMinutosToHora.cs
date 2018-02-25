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


	[ValueConversion(typeof(int), typeof(string))]
	public class ConvertidorMinutosToHora: IValueConverter {


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un TimeSpan, devolvemos una cadena vacía.
			if (value == null || !(value is int)) return "";
			// Evaluamos el parámetro pasado.
			VerValores ver = VerValores.Normal;
			if (parameter != null && parameter is VerValores) ver = (VerValores)parameter;
			// Asignamos el valor de la hora.
			int minutos = (int)value;
			TimeSpan hora = new TimeSpan(0, minutos, 0);
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
				if (h < 0) {
					esnegativo = true; h = h * -1;
				}
				int dias = (int)Math.Truncate(h / 24m);
				string m = valores.Length > 1 ? valores[1] : "00";
				texto = dias.ToString() + "." + (h - (dias * 24)).ToString("00") + ":" + m;
			}
			// Parseamos la hora en función del parámetro.
			if (TimeSpan.TryParse(texto, out TimeSpan hora)) {
				int resultado = System.Convert.ToInt32(Math.Round(hora.TotalMinutes, 0));
				return esnegativo ? resultado * -1 : resultado;
			} else {
				return 0;
			}



			
		}



	}
}
