﻿#region COPYRIGHT
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

    class ConvertidorVisibilityInverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is Visibility v) {
                if (v == Visibility.Visible) return Visibility.Collapsed;
            }
            if (value is bool) {
                if ((bool)value) return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
