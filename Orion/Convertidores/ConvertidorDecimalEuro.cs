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

namespace Orion.Convertidores {

	/// <summary>
	/// Convierte el número pasada a texto y viceversa.\n
	///	Se aplican los valores NoCeros, HoraPlus y Negativo de la enumeración VerValores.
	/// </summary>
	[ValueConversion(typeof(decimal), typeof(string))]
	class ConvertidorDecimalEuro :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un decimal devolvemos una cadena vacía.
			if (value == null || !(value is decimal)) return "";
			// Evaluamos el parámetro pasado.
			VerValores ver = VerValores.Normal;
			if (parameter != null & parameter is VerValores) ver = (VerValores)parameter;
			// Asignamos el valor al número.
			decimal numero = (decimal)value;
			// Si el número es cero y no se muestran los ceros, devolvemos una cadena vacía.
			if (numero == 0 && ver.HasFlag(VerValores.NoCeros)) return "";
			// Devolvemos el número.
			return numero.ToString("0.00 €");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			// Si el valor es nulo o no es un string devolvemos cero.
			if (value == null || !(value is string)) return 0m;
			// Parseamos el texto a decimal.
			decimal.TryParse((value as string).Replace(".", ","), out decimal numero);
			// Devolvemos el número instanciado en el TryParse.
			return numero;
		}


	} // Final de clase.
}
