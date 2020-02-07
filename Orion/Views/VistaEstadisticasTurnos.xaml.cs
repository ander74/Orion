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

    public partial class VistaEstadisticasTurnos : UserControl {
        public VistaEstadisticasTurnos() {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            if (sender is CheckBox check && check.Tag is string etiqueta && etiqueta.Equals("Todos")) {
                ChDias.IsChecked = check.IsChecked;
                ChTrabajadas.IsChecked = check.IsChecked;
                ChAcumuladas.IsChecked = check.IsChecked;
                ChNocturnas.IsChecked = check.IsChecked;
                ChImporteDietas.IsChecked = check.IsChecked;
                ChFestivos.IsChecked = check.IsChecked;
                ChDescansos.IsChecked = check.IsChecked;
                ChPluses.IsChecked = check.IsChecked;
            }
        }


        private void ChElegirConceptos_Click(object sender, RoutedEventArgs e) {
            if (sender is CheckBox check && check.Tag is string etiqueta && etiqueta.Equals("Todos")) {
                Ch2Turnos.IsChecked = check.IsChecked;
                Ch2Jornadas.IsChecked = check.IsChecked;
                Ch2Horas.IsChecked = check.IsChecked;
                Ch2Dietas.IsChecked = check.IsChecked;
                Ch2Pluses.IsChecked = check.IsChecked;
            }
        }


        // Cierra el desplegable Filtrar cuando se hace click en alguno de sus botones.
        private void BtFiltrar_Click(object sender, RoutedEventArgs e) {
            BtFiltrar.IsOpen = false;
        }
    }
}
