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
using System.Windows.Input;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VistaCalendarios.xaml
	/// </summary>
	public partial class VistaCalendarios :UserControl {
		public VistaCalendarios() {

			InitializeComponent();
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


		// AL CAMBIAR LA CELDA ACTUAL DE LA TABLA CALENDARIOS
		private void TablaCalendarios_CurrentCellChanged(object sender, EventArgs e) {
			//if (TablaCalendarios?.CurrentCell == null) return;
			CalendariosViewModel VM = ((GlobalVM)this.DataContext).CalendariosVM;
			int columna = -1;
			if (TablaCalendarios?.CurrentCell.Column != null) columna = TablaCalendarios.CurrentCell.Column.DisplayIndex;
			if (columna >= 0) VM.CalendarioSeleccionado = TablaCalendarios.CurrentCell.Item as Calendario;
			if (columna <= 0) {
				TablaCalendarios.SelectionMode = DataGridSelectionMode.Extended;
				if (columna == 0) VM.DiaCalendarioSeleccionado = null;
			} else {
				TablaCalendarios.SelectionMode = DataGridSelectionMode.Single;
				VM.DiaCalendarioSeleccionado = VM.CalendarioSeleccionado?.ListaDias[columna - 1];
			}
			
		}

	}
}
