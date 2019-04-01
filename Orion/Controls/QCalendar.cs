#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Input;

	public class QCalendar : Calendar {

		protected override void OnGotMouseCapture(MouseEventArgs e) {
			if (e.OriginalSource is CalendarDayButton || e.OriginalSource is CalendarItem) Mouse.Capture(null);
		}

	}
}
