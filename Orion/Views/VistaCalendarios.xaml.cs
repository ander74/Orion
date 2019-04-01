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

	public partial class VistaCalendarios : UserControl {
		public VistaCalendarios() {

			InitializeComponent();
		}

		private void BtMostrarTodos_Click(object sender, RoutedEventArgs e) {
			BtHorasMes.IsChecked = false;
			BtDiasMes.IsChecked = false;
			BtFindesMes.IsChecked = false;
			BtDietasMes.IsChecked = false;
			BtResumenMes.IsChecked = false;
		}
	}
}
