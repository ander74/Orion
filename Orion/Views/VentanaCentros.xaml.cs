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
	/// Lógica de interacción para VentanaCentros.xaml
	/// </summary>
	public partial class VentanaCentros : Window {

		public Centros Centro = Centros.Desconocido;

		public VentanaCentros() {
			InitializeComponent();
		}

		private void Boton_Click(object sender, RoutedEventArgs e) {
			Button boton = (Button)sender;
			switch (boton.Name){
				case "Bilbao":
					Centro = Centros.Bilbao;
					break;
				case "Donosti":
					Centro = Centros.Donosti;
					break;
				case "Arrasate":
					Centro = Centros.Arrasate;
					break;
				case "Vitoria":
					Centro = Centros.Vitoria;
					break;
				case "Cerrar":
					break;
				default:
					Centro = Centros.Desconocido;
					break;
			}
			this.Close();
		}

		private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
			DragMove();
		}

	}
}
