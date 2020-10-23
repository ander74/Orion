#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Orion.Convertidores {

    [ValueConversion(typeof(DataGridSelectionUnit), typeof(bool))]
    public class ConvertidorSelectionUnitToEnabled : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            switch (value) {
                case DataGridSelectionUnit valor:
                    return valor == DataGridSelectionUnit.Cell;
            }
            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            switch (value) {
                case bool valor:
                    return valor ? DataGridSelectionUnit.Cell : DataGridSelectionUnit.FullRow;
            }
            return DataGridSelectionUnit.FullRow;
        }
    }
}
