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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.Convertidores {


	[ValueConversion(typeof(Tuple<int, int>), typeof(FontWeight))]
	class ConvertidorNegritaDia :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			Tuple<int, int> combo = value as Tuple<int, int>;
			if (combo == null) return FontWeights.Normal;

			if (combo.Item1 < 0) return FontWeights.Bold;
			if (combo.Item2 == 3) return FontWeights.Bold;

			return FontWeights.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}

}
