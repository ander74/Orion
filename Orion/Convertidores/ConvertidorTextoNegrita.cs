#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Orion.Convertidores {

    public class ConvertidorTextoNegrita : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var resultado = new FontWeight();
            if (value is bool esCursiva && esCursiva) resultado = FontWeights.Bold;
            return resultado;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
