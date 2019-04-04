#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Views {

	using System;
	using System.Windows;
	using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
	using ViewModels;

	public partial class VistaGraficos : UserControl {

		public VistaGraficos() {
			InitializeComponent();
		}


		// ====================================================================================================
		#region GESTIÓN PANELES GRUPOS Y VALORACIONES
		// ====================================================================================================

		// TABLA GRAFICOS - GOT FOCUS
		private void TablaGraficos_GotFocus(object sender, RoutedEventArgs e) {
			if (ChPanelValoraciones.IsChecked != true) PanelValoraciones.Visibility = Visibility.Collapsed;
			if (ChPanelGrupos.IsChecked != true) PanelGrupos.Visibility = Visibility.Collapsed;
		}


		// CHECK GRUPOS - CLICK
		private void ChPanelGrupos_Click(object sender, RoutedEventArgs e) {
			if (ChPanelGrupos.IsChecked == true) {
				Grid.SetColumn(PanelGrupos, 0);
			} else {
				Grid.SetColumn(PanelGrupos, 1);
			}
		}


		// BOTÓN GRUPOS - CLICK
		private void BtGrupos_Click(object sender, RoutedEventArgs e) {
			if (PanelGrupos.IsVisible) {
				PanelGrupos.Visibility = Visibility.Collapsed;
				ChPanelGrupos.IsChecked = false;
				Grid.SetColumn(PanelGrupos, 1);
			} else {
				if (ChPanelValoraciones.IsChecked != true && PanelValoraciones.IsVisible) PanelValoraciones.Visibility = Visibility.Collapsed;
				PanelGrupos.Visibility = Visibility.Visible;
			}
		}

		// CHECK VALORACIONES - CLICK
		private void ChPanelValoraciones_Click(object sender, RoutedEventArgs e) {
			if (ChPanelGrupos.IsChecked == true) {
				Grid.SetColumn(PanelValoraciones, 2);
			} else {
				Grid.SetColumn(PanelValoraciones, 1);
			}
		}


		// BOTÓN VALORACIONES - CLICK
		private void BtValoraciones_Click(object sender, RoutedEventArgs e) {
			if (PanelValoraciones.IsVisible) {
				PanelValoraciones.Visibility = Visibility.Collapsed;
				ChPanelValoraciones.IsChecked = false;
				Grid.SetColumn(PanelValoraciones, 1);
			} else {
				if (ChPanelGrupos.IsChecked != true && PanelGrupos.IsVisible) PanelGrupos.Visibility = Visibility.Collapsed;
				PanelValoraciones.Visibility = Visibility.Visible;
			}
		}

        private void ListaGrupos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ChPanelGrupos.IsChecked == false) PanelGrupos.Visibility = Visibility.Collapsed;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region GESTIÓN BOTONES DESPLEGABLES
        // ====================================================================================================

        // AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE FILTRAR
        private void BotonFiltrado_Click(object sender, RoutedEventArgs e) {
			BtDropFiltros.IsOpen = false;
		}

		// AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE ACCIONES
		private void BotonAcciones_Click(object sender, RoutedEventArgs e) {
			BtDropAcciones.IsOpen = false;
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region GESTIÓN VALIDEZ GRUPOS
		// ====================================================================================================

		private void QCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
			PanelValidezGrupo.Visibility = Visibility.Collapsed;
		}

		private void BotonValidezGrupo_Click(object sender, RoutedEventArgs e) {
			PanelValidezGrupo.Visibility = Visibility.Visible;
		}

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BOTÓN QUITAR FILTRO
        // ====================================================================================================

        private void BtQuitarFiltro_Click(object sender, RoutedEventArgs e) {
            var view = CollectionViewSource.GetDefaultView(TablaGraficos.ItemsSource);
            if (view != null && view.SortDescriptions != null) {
                view.SortDescriptions.Clear();
                foreach (DataGridColumn columna in TablaGraficos.Columns) {
                    columna.SortDirection = null;
                }
                view.Filter = null;
            }
        }

        #endregion
        // ====================================================================================================


        // Al hacer doble click en la etiqueta GRUPO DE GRÁFICOS
        [Obsolete("Esto se tiene que hacer de otra manera")]
		private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			((GlobalVM)this.DataContext).VisibilidadProgramador = Visibility.Visible;
		}

    }
}
