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
using System.Windows.Data;

namespace Orion.Convertidores
{
	[ValueConversion(typeof(string), typeof(string))]
	public class ConvertidorDiaSemanaGrafico : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is string)
			{
				string valor = (string)value;
				if (string.IsNullOrEmpty(valor)) return string.Empty;
				switch (valor.ToLower())
				{
					case "l": case "lu": case "lun": case "lunes": return "L";
					case "m": case "ma": case "mar": case "martes": return "L";
					case "x": case "mi": case "mie": case "miercoles": case "mié": case "miércoles": return "L";
					case "j": case "ju": case "jue": case "jueves": return "L";
					case "v": case "vi": case "vie": case "viernes": return "V";
					case "s": case "sa": case "sab": case "sabado": case "sá": case "sáb": case "sábado": return "S";
					case "d": case "do": case "dom": case "domingo": return "F";
					case "f": case "fe": case "fes": case "festivo": return "F";
                    case "r": case "re": case "red": case "reduccion": return "R";
				}
			}
			return "";
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value is string)
			{
				string valor = (string)value;
				if (string.IsNullOrEmpty(valor)) return string.Empty;
				switch (valor.ToLower())
				{
					case "l": case "lu": case "lun": case "lunes": return "L";
					case "m": case "ma": case "mar": case "martes": return "L";
					case "x": case "mi": case "mie": case "miercoles": case "mié": case "miércoles": return "L";
					case "j": case "ju": case "jue": case "jueves": return "L";
					case "v": case "vi": case "vie": case "viernes": return "V";
					case "s": case "sa": case "sab": case "sabado": case "sá": case "sáb": case "sábado": return "S";
					case "d": case "do": case "dom": case "domingo": return "F";
					case "f": case "fe": case "fes": case "festivo": return "F";
                    case "r": case "re": case "red": case "reduccion": return "R";
                }
            }
			return "";
		}
	}
}
