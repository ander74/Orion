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
	/// Lógica de interacción para VentanaMensajes.xaml
	/// </summary>
	public partial class VentanaMensajes :Window {

		public VentanaMensajes() {
			InitializeComponent();
		}

		public bool? Resultado { get; set; }

		private void Cancelar_Click(object sender, RoutedEventArgs e) {
			Resultado = null;
			this.Close();
		}


		private void No_Click(object sender, RoutedEventArgs e) {
			Resultado = false;
			this.Close();
		}


		private void Si_Click(object sender, RoutedEventArgs e) {
			Resultado = true;
			this.Close();
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}
	}
}
