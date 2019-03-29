#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using Orion.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VistaCalendarios.xaml
	/// </summary>
	public partial class VistaCalendarios :UserControl {
		public VistaCalendarios() {

			InitializeComponent();
		}

		// Al capturar el raton el calendario, soltarlo si se lo queda.
		private void Calendar_GotMouseCapture(object sender, MouseEventArgs e)
		{
			if (e.OriginalSource is CalendarDayButton || e.OriginalSource is CalendarItem) Mouse.Capture(null);
		}


		// AL HACER DOBLE CLIC EN LA TABLA CALENDARIOS
		private void TablaCalendarios_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			DataGrid tabla = sender as DataGrid;
			if (tabla == null || tabla.CurrentCell == null) return;
			if (tabla.CurrentCell.Column.Header.ToString() == "Conductor") {
				e.Handled = true;
				((GlobalVM)this.DataContext).CalendariosVM.cmdAbrirPijama.Execute(null);
			}
		}


		// AL COGER EL FOCO UN TEXTBOX
		private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			((TextBox)sender).SelectAll();
		}


		// AL HACER CLICK EN UN TEXTBOX
		private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			TextBox tb = (sender as TextBox);

			if (!tb.IsKeyboardFocusWithin) {
				e.Handled = true;
				tb.Focus();
			}
		}

		// AL PULSAR UNA TECLA EN UN TEXTBOX
		private void TextBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
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

	}
}
