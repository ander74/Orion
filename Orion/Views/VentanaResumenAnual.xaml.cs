﻿#region COPYRIGHT
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
	/// Lógica de interacción para VistaResumenAnual.xaml
	/// </summary>
	public partial class VentanaResumenAnual : Window {
		public VentanaResumenAnual() {
			InitializeComponent();
		}


		/// <summary>
		///  A PESAR DE ROMPER EL PATRÓN MVVM, CIERRO LA VENTANA DESDE EL CODE BEHIND.
		/// </summary>
		private void Button_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
