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
	/// Lógica de interacción para VentanaAñadirGrafico.xaml
	/// </summary>
	public partial class VentanaAñadirGrafico :Window {

		public VentanaAñadirGrafico() {
			InitializeComponent();
		}

		private void Ventana_Loaded(object sender, RoutedEventArgs e) {
			// Ponemos el foco en el número o en la valoración.
			if (ChDeducirTurno.IsChecked == true && TbNumero.Text != "0") TbValoracion.Focus(); else TbNumero.Focus();
		}

		private void Tb_GotFocus(object sender, RoutedEventArgs e) {
			// Seleccionamos todo el texto.
			((TextBox)sender).SelectAll();
		}

		private void TbNumero_LostKeyboardFocus(object sender, RoutedEventArgs e) {
			// Si está activado la deducción de turno, ponemos el foco en Valoración.
			if (ChDeducirTurno.IsChecked == true) TbValoracion.Focus();
		}

		private void Tb_KeyDown(object sender, KeyEventArgs e) {
			// Si pulsamos Enter, actuamos, según sea el textbox que ha producido el evento.
			if (e.Key == Key.Enter) {
				TextBox tb = (TextBox)sender;
				if (tb == TbNumero) {
					if (ChDeducirTurno.IsChecked == true) TbValoracion.Focus(); else TbTurno.Focus();
					e.Handled = true;
				} else if (tb == TbTurno) {
					TbValoracion.Focus();
					e.Handled = true;
				} else if (tb == TbValoracion) {
					TbInicio.Focus();
					e.Handled = true;
				} else if (tb == TbInicio) {
					TbFinal.Focus();
					e.Handled = true;
				} else if (tb == TbFinal) {
					if (TbInicioPartido.IsVisible) {
						TbInicioPartido.Focus();
						e.Handled = true;
					}
				} else if (tb == TbInicioPartido) {
					TbFinalPartido.Focus();
					e.Handled = true;
				}
			}



		}
	}
}
