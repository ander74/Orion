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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VistaOpciones.xaml
	/// </summary>
	public partial class VistaOpciones :UserControl {
		public VistaOpciones() {
			InitializeComponent();
		}


		private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			((TextBox)sender).SelectAll();
		}

		private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			TextBox tb = (sender as TextBox);

			if (!tb.IsKeyboardFocusWithin) {
				e.Handled = true;
				tb.Focus();
			}
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}


	}
}
