#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VistaGraficos.xaml
	/// </summary>
	public partial class VistaGraficos : UserControl {

		public VistaGraficos() {
			InitializeComponent();
		}

		// Al capturar el raton el calendario, soltarlo si se lo queda.
		private void Calendar_GotMouseCapture(object sender, MouseEventArgs e) {
			if (e.OriginalSource is CalendarDayButton || e.OriginalSource is CalendarItem) Mouse.Capture(null);
		}

		// Al poner el foco en el DataGrid Graficos
		private void TablaGraficos_GotFocus(object sender, RoutedEventArgs e) {
			GraficosViewModel VM = ((GlobalVM)this.DataContext).GraficosVM;
			if (!VM.PanelGruposFijo) PanelGrupos.Visibility = Visibility.Collapsed;
			if (!VM.PanelValoracionesFijo) PanelValoraciones.Visibility = Visibility.Collapsed;
			
		}

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

		private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
			var tabla = sender as DataGrid;
			if (tabla != null && e.AddedCells != null && e.AddedCells.Count > 0) {
				var cell = e.AddedCells[0];
				if (!cell.IsValid) return;
				ColumnaActual.Tag = cell.Column.DisplayIndex;
				FilaActual.Tag = tabla.Items.IndexOf(cell.Item);
			}
		}

			// Al hacer doble click en la etiqueta GRUPO DE GRÁFICOS
			private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			//if (e.ClickCount != 4) return;
			((GlobalVM)this.DataContext).VisibilidadProgramador = Visibility.Visible;
		}
	}
}
