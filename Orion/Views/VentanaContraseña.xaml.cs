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
	/// Lógica de interacción para VentanaContraseña.xaml
	/// </summary>
	public partial class VentanaContraseña :Window {
		public VentanaContraseña() {
			InitializeComponent();
			PwContraseña.Password = "";
			PwContraseña.Focus();
		}

		private void BtAceptar_Click(object sender, RoutedEventArgs e) {
			if (Utils.CodificaTexto(PwContraseña.Password) != App.Global.Configuracion.ContraseñaDatos) {
				TxtMensajeError.Text = "Contraseña incorrecta.";
				PwContraseña.Focus();
			} else {
				this.DialogResult = true;
				this.Close();
			}
		}


		private void PwContraseña_KeyDown(object sender, KeyEventArgs e) {
			TxtMensajeError.Text = "";
		}

		private void BtSalir_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
			this.Close();
		}

		//private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
		//	DragMove();
		//}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}
	}
}
