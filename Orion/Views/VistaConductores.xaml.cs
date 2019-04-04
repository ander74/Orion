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
    using System.Windows.Data;
    using Controls;

	public partial class VistaConductores : UserControl {
		public VistaConductores() {
			InitializeComponent();
		}


		// ====================================================================================================
		#region GESTIÓN PANEL REGULACIONES
		// ====================================================================================================

		// TABLA CONDUCTORES - GOT FOCUS
		private void TablaConductores_GotFocus(object sender, RoutedEventArgs e) {
			if (!ChPanelRegulaciones.IsChecked ?? false) PanelRegulaciones.Visibility = Visibility.Collapsed;
		}


		// CHECK REGULACIONES - CLICK
		private void ChPanelRegulaciones_Click(object sender, RoutedEventArgs e) {
			if (ChPanelRegulaciones.IsChecked == true) {
				Grid.SetColumn(PanelRegulaciones, 0);
			} else {
				Grid.SetColumn(PanelRegulaciones, 1);
			}
		}


		// BOTÓN REGULACIONES - CLICK
		private void BtRegulaciones_Click(object sender, RoutedEventArgs e) {
			if (PanelRegulaciones.IsVisible) {
				PanelRegulaciones.Visibility = Visibility.Collapsed;
				ChPanelRegulaciones.IsChecked = false;
				Grid.SetColumn(PanelRegulaciones, 1);
			} else {
				PanelRegulaciones.Visibility = Visibility.Visible;
			}
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region GESTIÓN DESPLEGABLE ACCIONES
		// ====================================================================================================

		private void BtAccionesPulsado_Click(object sender, RoutedEventArgs e) {
			BtDropAcciones.IsOpen = false;
		}

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BOTÓN QUITAR FILTRO
        // ====================================================================================================

        private void BtQuitarFiltro_Click(object sender, RoutedEventArgs e) {
            var view = CollectionViewSource.GetDefaultView(TablaConductores.ItemsSource);
            if (view != null && view.SortDescriptions != null) {
                view.SortDescriptions.Clear();
                foreach (DataGridColumn columna in TablaConductores.Columns) {
                    columna.SortDirection = null;
                }
                view.Filter = null;
            }
        }


        #endregion
        // ====================================================================================================





    }
}
