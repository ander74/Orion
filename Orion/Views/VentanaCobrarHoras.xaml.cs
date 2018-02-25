#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
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
using System.Windows.Shapes;

namespace Orion.Views
{
    /// <summary>
    /// Lógica de interacción para VentanaCobrarHoras.xaml
    /// </summary>
    public partial class VentanaCobrarHoras : Window
    {
        public VentanaCobrarHoras(){
            InitializeComponent();
        }

		private void Ventana_Loaded(object sender, RoutedEventArgs e) {
			// Ponemos el foco en Horas a Cobrar
			TbHorasACobrar.Focus();
		}

		private void Tb_GotFocus(object sender, RoutedEventArgs e) {
			// Seleccionamos todo el texto.
			((TextBox)sender).SelectAll();
		}

		private void Calendar_GotMouseCapture(object sender, MouseEventArgs e) {
			if (e.OriginalSource is CalendarDayButton || e.OriginalSource is CalendarItem) Mouse.Capture(null);
		}
	}
}
