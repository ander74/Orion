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
	/// Lógica de interacción para VentanaError.xaml
	/// </summary>
	public partial class VentanaError : Window {

		public VentanaError() {
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}
	}
}
