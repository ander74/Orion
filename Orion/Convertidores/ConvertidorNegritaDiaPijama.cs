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
using System.Windows;
using System.Windows.Data;

namespace Orion.Convertidores {

	class ConvertidorNegritaDiaPijama : IMultiValueConverter {

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

			if (!(values[0] is Tuple<int, int> combo)) return FontWeights.Normal;
			if (!(values[1] is bool alternativo)) return FontWeights.Normal;
			FontWeight fuente = FontWeights.Normal;

			if (combo.Item1 < 0) fuente = FontWeights.Bold;
			if (combo.Item2 == 3) fuente = FontWeights.Bold;
			if (alternativo) fuente = FontWeights.Bold;
			return fuente;

		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
