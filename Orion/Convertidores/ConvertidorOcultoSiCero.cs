#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Convertidores {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class ConvertidorOcultoSiCero : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is TimeSpan ts && ts.Ticks == 0) return Visibility.Collapsed;

            if (value is int i && i == 0) return Visibility.Collapsed;

            if (value is decimal d && d == 0) return Visibility.Collapsed;

            if (int.TryParse(value.ToString(), out int v)) {
                if (v == 0) return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }


    }
}
