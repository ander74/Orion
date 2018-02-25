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
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.Convertidores {

	class ConvertidorPrueba :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			// Declaramos el color que se devolverá.
			SolidColorBrush color = new SolidColorBrush(Colors.Transparent);

			// Si el valor es nulo, devolvemos el color
			DiaCalendario dia = value as DiaCalendario;
			if (dia == null) return color;
			
			if (dia.Grafico == 3502) color = new SolidColorBrush(Colors.Chocolate);
			if (dia.Grafico == 3501) color = new SolidColorBrush(Colors.Yellow);
			if (dia.Grafico == 3510) color = new SolidColorBrush(Colors.Red);

			// Devolvemos el color
			return color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			throw new NotImplementedException();
		}
	}
}
