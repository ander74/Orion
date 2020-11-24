#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Globalization;
using System.Windows.Data;

namespace Orion.Convertidores {

    [ValueConversion(typeof(decimal), typeof(string))]
    public class ConvertidorItinerarioNoMenor100 : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is decimal valor) {
                string textoLinea = valor.ToString().Replace(",", ".");
                int numeroItinerario;
                if (textoLinea.Contains(".")) {
                    var codigos = textoLinea.Split('.');
                    numeroItinerario = int.Parse(codigos[0]);
                } else {
                    numeroItinerario = int.Parse(textoLinea);
                }
                return numeroItinerario < 100 ? string.Empty : numeroItinerario.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string valor && decimal.TryParse(valor.Replace(".", ","), out decimal numero)) return numero;
            return 0m;
        }

    }
}
