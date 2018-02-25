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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Orion.Convertidores {

	class ConvertidorColorDia :IMultiValueConverter {

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

			// Declaramos el color que se devolverá.
			SolidColorBrush color = new SolidColorBrush(Colors.Black);

			// Creamos los objetos que se van a evaluar. Si no están bien, devolvemos el color inicial.
			if (values.Length < 3) return color;
			if (values[0].GetType().Name == "NamedObject") return color;
			Tuple<int, int> combo = values[0] == null ? new Tuple<int, int>(0, 0) : (Tuple<int, int>)values[0];
			int grafico = combo.Item1;
			int codigo = combo.Item2;
			DateTime f = values[1] == null ? new DateTime() : (DateTime)values[1];
			int dia = values[2] == null ? 0 : (int)values[2];
			if (f == null) return color;

			//Creamos la fecha a evaluar.
			if (dia > DateTime.DaysInMonth(f.Year, f.Month)) return color;
			DateTime fecha = new DateTime(f.Year, f.Month, dia);

			// Definimos las variables necesarias
			ConvertidorColores cnv = new ConvertidorColores();
			bool esfinde = false;

			// Evaluamos el color a elegir
			switch (fecha.DayOfWeek) {
				case DayOfWeek.Saturday:
					color = new SolidColorBrush(Colors.Blue); esfinde = true; break;
				case DayOfWeek.Sunday:
					color = new SolidColorBrush(Colors.Red); esfinde = true; break;
				default:
					color = new SolidColorBrush(Colors.Black); esfinde = false; break;
			}
			if (App.Global.CalendariosVM.EsFestivo(fecha)) {
				color = new SolidColorBrush(Colors.Red);
				esfinde = true;
			}
			if (!esfinde) {
				switch (grafico) {
					case -1:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorOV, null, null, null)); break;
					case -2: case -3:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorJD, null, null, null)); break;
					case -4:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorE, null, null, null)); break;
					case -5:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorDS, null, null, null)); break;
					case -6:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorDC, null, null, null)); break;
					case -7:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorF6, null, null, null)); break;
					case -8:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorF6, null, null, null)); break;
					case -9:
						color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorJD, null, null, null)); break;
				}
			}
			if (codigo == 3) color = new SolidColorBrush((System.Windows.Media.Color)cnv.Convert(Properties.Settings.Default.ColorJD, null, null, null));

			// Devolvemos el color.
			return color;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}


	class ConvertidorColorDiaSinGraficos :IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			// Declaramos el color que se devolverá.
			SolidColorBrush color = new SolidColorBrush(Colors.Black);

			// Establecemos ĺa fecha del día.
			if (value == null || !(value is DateTime)) return color;
			DateTime f = (DateTime)value;

			// Definimos las variables necesarias
			ConvertidorColores cnv = new ConvertidorColores();

			// Evaluamos el color a elegir
			switch (f.DayOfWeek) {
				case DayOfWeek.Saturday:
					color = new SolidColorBrush(Colors.Blue); break;
				case DayOfWeek.Sunday:
					color = new SolidColorBrush(Colors.Red); break;
				default:
					color = new SolidColorBrush(Colors.Black); break;
			}
			if (App.Global.CalendariosVM.EsFestivo(f)) {
				color = new SolidColorBrush(Colors.Red);
			}

			// Devolvemos el color.
			return color;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}


}
