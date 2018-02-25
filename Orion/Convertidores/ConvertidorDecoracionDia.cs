#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.Convertidores {


	[ValueConversion(typeof(int), typeof(FontWeight))]
	class ConvertidorDecoracionDia :IMultiValueConverter {

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

			TextDecorationCollection resultado = new TextDecorationCollection();

			if (values.Length < 2) return resultado;
			if (values[0].GetType().Name == "NamedObject") return resultado;
			int codigo = values[0] == null ? 0 : (int)values[0];
			bool haydatos = values[1] == null ? false : (bool)values[1];
			
			// Evaluamos el código.
			switch (codigo) {
				case 1:
				case 2:
					resultado.Add(TextDecorations.Strikethrough);
					break;
			}

			// Si hay extras, subrayamos.
			if (haydatos) resultado.Add(TextDecorations.Underline);

			return resultado;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			return null;
		}
	}

}
