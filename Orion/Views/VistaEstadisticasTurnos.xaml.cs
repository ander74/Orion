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
            if (sender is CheckBox check) {
                switch (check.Tag) {
                    case "Dias":
                        Dias1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Trabajadas":
                        Trabajadas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Acumuladas":
                        Acumuladas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Nocturnas":
                        Nocturnas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "ImporteDietas":
                        ImporteDietas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Festivos":
                        DiasFestivos1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "DiasTrabajo":
                        DiasTrabajo.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "DiasDescanso":
                        DiasDescanso.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "FindesTrabajo":
                        FindesTrabajados.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "FindesDescanso":
                        FindesDescansados.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "PlusPaqueteria":
                        PlusPaqueteria.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "PlusMenorDescanso":
                        PlusMenorDescanso.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Todos":
                        ChDias.IsChecked = check.IsChecked;
                        ChTrabajadas.IsChecked = check.IsChecked;
                        ChAcumuladas.IsChecked = check.IsChecked;
                        ChNocturnas.IsChecked = check.IsChecked;
                        ChImporteDietas.IsChecked = check.IsChecked;
                        ChFestivos.IsChecked = check.IsChecked;
                        ChDiasTrabajo.IsChecked = check.IsChecked;
                        ChDiasDescanso.IsChecked = check.IsChecked;
                        ChFindesTrabajo.IsChecked = check.IsChecked;
                        ChFindesDescanso.IsChecked = check.IsChecked;
                        ChPlusPaqueteria.IsChecked = check.IsChecked;
                        ChPlusMenorDescanso.IsChecked = check.IsChecked;

                        Dias1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Dias4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Trabajadas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Acumuladas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        Nocturnas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        ImporteDietas4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos1.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos2.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos3.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasFestivos4.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasTrabajo.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        DiasDescanso.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        FindesTrabajados.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        FindesDescansados.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        PlusPaqueteria.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        PlusMenorDescanso.Visibility = check.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                        break;
                }
            }
        }

    }
}
