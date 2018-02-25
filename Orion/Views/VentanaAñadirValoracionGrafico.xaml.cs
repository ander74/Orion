#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VentanaAñadirValoracionGrafico.xaml
	/// </summary>
	public partial class VentanaAñadirValoracionGrafico :Window {

		public VentanaAñadirValoracionGrafico() {
			InitializeComponent();
		}

		private void Ventana_Loaded(object sender, RoutedEventArgs e) {
			// Ponemos el foco en el inicio.
			if (TbInicio.Text != "") TbLinea.Focus(); else TbInicio.Focus();
		}

		private void Tb_GotFocus(object sender, RoutedEventArgs e) {
			// Seleccionamos todo el texto.
			((TextBox)sender).SelectAll();
		}

		private void Tb_KeyDown(object sender, KeyEventArgs e) {
			// Si pulsamos Enter, actuamos, según sea el textbox que ha producido el evento.
			if (e.Key == Key.Enter) {
				TextBox tb = (TextBox)sender;
				if (tb == TbInicio) {
					TbLinea.Focus();
					e.Handled = true;
				}
			}
		}



	}
}
