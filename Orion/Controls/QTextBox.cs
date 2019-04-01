#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

	using System.Windows.Controls;
	using System.Windows.Input;

	public class QTextBox : TextBox {

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			SelectAll();
		}


		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			if (!IsKeyboardFocusWithin) {
				e.Handled = true;
				Focus();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.Key == Key.Enter) MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}
	}
}
