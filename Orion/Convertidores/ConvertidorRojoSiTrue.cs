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
using System.Windows.Media;

namespace Orion.Convertidores {

	[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
	class ConvertidorRojoSiTrue :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			if (value != null && value is bool) {
				bool condicion = (bool)value;
				if (condicion) return new SolidColorBrush(Colors.Red);
			}
			return new SolidColorBrush(Colors.Black);
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}

	}


}
