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
    public class ConvertidorNumeroArticuloConvenio : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var resultado = string.Empty;
            if (value is decimal numero) {
                if (numero % 1 == 0) {
                    resultado = Math.Round(numero, 0).ToString();
                } else {
                    resultado = numero.ToString("0.0");
                }
            }
            resultado = resultado.Replace(",", ".")
                .Replace("-1.", "A")
                .Replace("-2.", "B")
                .Replace("-3.", "C")
                .Replace("-4.", "D")
                .Replace("-5.", "E")
                .Replace("-6.", "F")
                .Replace("-7.", "G")
                .Replace("-8.", "H")
                .Replace("-9.", "I");
            return resultado;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string texto) {
                texto = texto.ToUpper()
                    .Replace("A", "-1.")
                    .Replace("B", "-2.")
                    .Replace("C", "-3.")
                    .Replace("D", "-4.")
                    .Replace("E", "-5.")
                    .Replace("F", "-6.")
                    .Replace("G", "-7.")
                    .Replace("H", "-8.")
                    .Replace("I", "-9.");

                if (decimal.TryParse(texto.Replace(".", ","), out decimal resultado)) {
                    return resultado;
                }
            }
            return 0m;
        }
    }
}
