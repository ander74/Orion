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

    /// <summary>
    /// Convierte el número pasado a texto y viceversa.\n
    ///	Se aplican los valores NoCeros, HoraPlus y Negativo de la enumeración VerValores.
    /// </summary>
    [ValueConversion(typeof(decimal), typeof(string))]
    class ConvertidorItinerario : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            // Si el valor es nulo o no es un decimal devolvemos una cadena vacía.
            if (value == null || !(value is decimal)) return "";
            // Evaluamos el parámetro pasado.
            VerValores ver = VerValores.Normal;
            if (parameter != null & parameter is VerValores) ver = (VerValores)parameter;
            // Desglosamos el código.
            string textoLinea = value.ToString().Replace(",", ".");
            int numeroItinerario = 0;
            int modificador = 0;
            if (textoLinea.Contains(".")) {
                var codigos = textoLinea.Split('.');
                numeroItinerario = int.Parse(codigos[0]);
                modificador = int.Parse(codigos[1]);
            } else {
                numeroItinerario = int.Parse(textoLinea);
            }
            // Si el número de itinerario es menor que cien, no se muestra.
            return numeroItinerario < 100 ? "" : numeroItinerario.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            // Si el valor es nulo o no es un string devolvemos cero.
            if (value == null || !(value is string)) return 0m;
            // Parseamos el texto a decimal.
            decimal.TryParse((value as string).Replace(".", ","), out decimal numero);
            // Devolvemos el número instanciado en el TryParse.
            return numero;
        }


    } // Final de clase.
}
