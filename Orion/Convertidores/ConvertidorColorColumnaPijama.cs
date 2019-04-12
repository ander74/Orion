using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using static Orion.Pijama.DiaPijama;

namespace Orion.Convertidores {

	[ValueConversion(typeof(int), typeof(SolidColorBrush))]
	class ConvertidorColorColumnaPijama : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Reclama i) {
                if (i == Reclama.EnContra) { 
                    return new SolidColorBrush(Colors.DarkRed);
                }
                if (i == Reclama.AFavor) {
                    return new SolidColorBrush(Colors.DarkGreen);
                }
            }
            return new SolidColorBrush(Colors.Black);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
