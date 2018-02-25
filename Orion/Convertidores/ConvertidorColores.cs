#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.Convertidores {

	class ConvertidorColores :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Declaramos el color que se devolverá.
			System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(0, 0, 0);

			// Si el valor es nulo, devolvemos el color
			if (value == null || !(value is System.Drawing.Color)) return color;

			// Convertimos el valor en un color
			System.Drawing.Color c = (System.Drawing.Color)value;

			// Establecemos los componentes del color pasado al nuevo.
			color.A = c.A;
			color.R = c.R;
			color.G = c.G;
			color.B = c.B;

			// Devolvemos el color
			return color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			// Declaramos el color que se devolverá.
			System.Drawing.Color color = System.Drawing.Color.Black;

			// Si el valor es nulo, devolvemos el color
			if (value == null || !(value is System.Windows.Media.Color)) return color;

			// Convertimos el valor en un color
			System.Windows.Media.Color c = (System.Windows.Media.Color)value;

			// Establecemos los componentes del color pasado al nuevo.
			color = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);

			// Devolvemos el color
			return color;
		}
	}
}
