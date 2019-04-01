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


		#endregion
		// ====================================================================================================


		// Al terminar de editar una fila de la tabla valoraciones
		private void TablaValoraciones_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
			GraficosViewModel VM = ((GlobalVM)this.DataContext).GraficosVM;
			VM.cmdEditarFilaValoracion.Execute(TablaValoraciones);
		}

		// Al cambiar una fecha en el calendario
		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
			PanelValidezGrupo.Visibility = Visibility.Collapsed;
			((GlobalVM)this.DataContext).GraficosVM.HayCambios = true;
		}

		// Al hacer doble click en la etiqueta GRUPO DE GRÁFICOS
		private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			((GlobalVM)this.DataContext).VisibilidadProgramador = Visibility.Visible;
		}
	}
}
