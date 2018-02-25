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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		// La celda actual ha cambiado.
		private void TablaGraficos_CurrentCellChanged(object sender, EventArgs e) {
			if (TablaGraficos.CurrentCell == null || TablaGraficos.CurrentCell.Column == null) return;
			if (TablaGraficos.CurrentCell.Column.Header.ToString() == "Número") {
				TablaGraficos.SelectionMode = DataGridSelectionMode.Extended;
			} else {
				TablaGraficos.SelectionMode = DataGridSelectionMode.Single;
			}

		}

	}
}
