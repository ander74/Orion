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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VistaLineas.xaml
	/// </summary>
	public partial class VistaLineas :UserControl {
		public VistaLineas() {
			InitializeComponent();
		}

		private void Tabla_GotFocus(object sender, RoutedEventArgs e) {
			((GlobalVM)this.DataContext).LineasVM.TablaParaCopy = sender as DataGrid;
		}
	}
}
