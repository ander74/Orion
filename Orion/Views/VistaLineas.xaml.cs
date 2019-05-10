#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Views {

	using System.Windows;
	using System.Windows.Controls;
	using ViewModels;

	public partial class VistaLineas : UserControl {
		public VistaLineas() {
			InitializeComponent();
		}

		//TODO: Desacoplar esto.
		private void Tabla_GotFocus(object sender, RoutedEventArgs e) {
			((GlobalVM)this.DataContext).LineasVM.TablaParaCopy = sender as DataGrid;
		}
	}
}
