#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.Properties;
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
	/// Lógica de interacción para VentanaCambioContraseña.xaml
	/// </summary>
	public partial class VentanaCambioContraseña :Window {
		public VentanaCambioContraseña() {
			InitializeComponent();
			PwAnterior.Focus();
		}


		private void BtAceptar_Click(object sender, RoutedEventArgs e) {

			if (Utils.CodificaTexto(PwAnterior.Password) != App.Global.Configuracion.ContraseñaDatos) {
				TxtMensajeError.Foreground = Brushes.IndianRed;
				TxtMensajeError.Text = "Contraseña Anterior no válida.";
				PwAnterior.Focus();
			} else if (PwNueva.Password != PwNueva2.Password) {
				PwNueva.Focus();
			} else {
				this.DialogResult = true;
				this.Close();
			}
		}


		private void BtCancelar_Click(object sender, RoutedEventArgs e) {

			this.DialogResult = false;
			this.Close();
		}


		private void PwNueva_KeyUp(object sender, KeyEventArgs e) {
			if (PwNueva.Password == PwNueva2.Password) {
				TxtMensajeError.Foreground = Brushes.DarkGreen;
				TxtMensajeError.Text = "Contraseñas válidas.";
			} else {
				TxtMensajeError.Foreground = Brushes.IndianRed;
				TxtMensajeError.Text = "Las contraseñas no coinciden.";
			}
		}


		private void PwAnterior_KeyDown(object sender, KeyEventArgs e) {
			TxtMensajeError.Text = "";
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}
	}
}
