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

namespace Orion.Convertidores {



	[ValueConversion(typeof(int), typeof(string))]
	public class ConvertidorNumeroGrafico : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

			if (value != null && value is int) {
				int Grafico = (int)value;
				if (Grafico == 0) return "";
				if (Grafico > 0) return Grafico.ToString("0000");
				switch (Grafico) {
					case -1: return "O-V";
					case -2: return "J-D";
					case -3: return "FN";
					case -4: return "E";
					case -5: return "DS";
					case -6: return "DC";
					case -7: return "F6";
					case -8: return "DND";
					case -9: return "PER";
					case -10: return "E(JD)";
					case -11: return "E(FN)";
					case -12: return "OV(JD)";
					case -13: return "OV(FN)";
					case -14: return "F6(DC)";
				}
			}
			return "";

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

			int grafico = 0;
			if (value != null && value is string) {
				string texto = ((string)value).ToLower().Replace("-", "");

				switch (texto) {
					case "ov": grafico = -1; break;
					case "jd": grafico = -2; break;
					case "fn": grafico = -3; break;
					case "e": case "ge": grafico = -4; break;
					case "ds": grafico = -5; break;
					case "dc": case "oh": case "dh": grafico = -6; break;
					case "f6": case "f4": grafico = -7; break;
					case "dnd": case "df": grafico = -8; break;
					case "per": grafico = -9; break;
					case "ejd": grafico = -10; break;
					case "efn": grafico = -11; break;
					case "ovjd": grafico = -12; break;
					case "ovfn": grafico = -13; break;
					case "f6dc": case "dcf6": grafico = -14; break;
					default: Int32.TryParse(texto, out grafico); break;
				}

			}
			return grafico;
		}


	} // Final de clase.
}
