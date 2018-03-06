#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.ViewModels;
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

namespace Orion.Views
{
    /// <summary>
    /// Lógica de interacción para VentanaReclamaciones.xaml
    /// </summary>
    public partial class VentanaReclamaciones : Window
    {
        public VentanaReclamaciones()
        {
            InitializeComponent();
        }


		// AL HACER CLICK SOBRE EL ENCABEZADO
		private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}


		// AL HACER CLICK EN EL BOTÓN CANCELAR
		private void Cancelar_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
			this.Close();
		}


		// AL HACER CLICK EN EL BOTÓN ACEPTAR
		private void Aceptar_Click(object sender, RoutedEventArgs e) {
			ReclamacionesViewModel VM = this.DataContext as ReclamacionesViewModel;
			if (VM != null) {
				if (VM.ConceptosReclamados > 17) {
					MessageBox.Show("No se pueden reclamar más de 17 conceptos a la vez.\n\nPruebe a hacer dos hojas de reclamación.",
									"Exceso de conceptos a reclamar", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				if (VM.ConceptosReclamados < 1) {
					MessageBox.Show("No hay conceptos a reclamar.\n\nAsegúrese de que están las casillas marcadas.",
									"Sin conceptos a reclamar", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
			}
			this.DialogResult = true;
			this.Close();
		}


		// AL COGER EL FOCO UN TEXTBOX
		private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			((TextBox)sender).SelectAll();
		}


		// AL HACER CLICK EN UN TEXTBOX
		private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			TextBox tb = (sender as TextBox);

			if (!tb.IsKeyboardFocusWithin) {
				e.Handled = true;
				tb.Focus();
			}
		}

		// AL PULSAR UNA TECLA EN UN TEXTBOX
		private void TextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}




	}
}
