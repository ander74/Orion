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
using System.Windows.Media;

namespace Orion.Convertidores {

    public class ConvertidorColorFecha : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            // Declaramos el color que se devolverá.
            SolidColorBrush color = new SolidColorBrush(Colors.Black);

            if (value is DateTime fecha && parameter is string diaTexto) {
                if (int.TryParse(diaTexto, out int dia) && dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month)) {
                    var fechaFinal = new DateTime(fecha.Year, fecha.Month, dia);
                    if (fechaFinal.DayOfWeek == DayOfWeek.Sunday || App.Global.CalendariosVM.EsFestivo(fechaFinal)) {
                        color = new SolidColorBrush(Colors.Red);
                    }
                    if (fechaFinal.DayOfWeek == DayOfWeek.Saturday) {
                        color = new SolidColorBrush(Colors.Blue);
                    }
                }
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
