#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Windows;
using System.Windows.Input;

namespace Orion.Views {
    /// <summary>
    /// Lógica de interacción para VentanaArticuloConvenio.xaml
    /// </summary>
    public partial class VentanaArticuloConvenio : Window {
        public VentanaArticuloConvenio() {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
