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

	[ValueConversion(typeof(DateTime), typeof(string)) ]
	class ConvertidorFecha : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			
			if (value != null && value is DateTime) {
				DateTime fecha = (DateTime)value;
				if (fecha.Year == 2001) return "";
				return fecha.ToString("dd-MM-yyyy");
			} else {
				return "";
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			if (value == null || !(value is string)) return new DateTime(2001, 1, 2);
			DateTime fecha = new DateTime(2001,1,2);
			DateTime.TryParse(value as string, out fecha);
			return fecha;
		}
	}
}
