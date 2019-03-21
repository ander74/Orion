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
	class ConvertidorColorColumnaPijama : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null) {
				return new SolidColorBrush(Colors.DarkRed);
			}
			//if (value is bool valor && valor) {
			//	return new SolidColorBrush(Colors.IndianRed);
			//}
			return new SolidColorBrush(Colors.Black);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
