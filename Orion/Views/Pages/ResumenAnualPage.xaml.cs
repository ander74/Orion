#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Orion.Views.Pages {
    /// <summary>
    /// Lógica de interacción para ResumenAnualPage.xaml
    /// </summary>
    public partial class ResumenAnualPage : UserControl {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        List<CheckBox> ListaCbHoras = new List<CheckBox>();
        List<CheckBox> ListaCbDias = new List<CheckBox>();

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public ResumenAnualPage() {
            InitializeComponent();
            // Lista Horas
            ListaCbHoras.Add(cbHorasTrabajadas);
            ListaCbHoras.Add(cbHorasAcumuladas);
            ListaCbHoras.Add(cbHorasNocturnas);
            // Lista Días
            ListaCbDias.Add(cbDiasActivos);
            ListaCbDias.Add(cbDiasInactivos);
            ListaCbDias.Add(cbDiasTrabajados);
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        public void DesSelectAll() {
            // Horas
            cbHoras.IsChecked = false;
            changeHoras();
            // Días
            cbDias.IsChecked = false;
            changeDias();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PRESETS
        // ====================================================================================================

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = ((ComboBox)sender).SelectedItem as ComboBoxItem;
            if (item == null) return;
            switch (item.Tag) {
                case "Horas":
                    DesSelectAll();
                    cbHorasTrabajadas.IsChecked = true;
                    cbHorasAcumuladas.IsChecked = true;
                    checkHorasState();
                    break;
                case "Dias":
                    DesSelectAll();
                    cbDiasActivos.IsChecked = true;
                    cbDiasInactivos.IsChecked = true;
                    cbDiasTrabajados.IsChecked = true;
                    checkDiasState();
                    break;
            }
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region HORAS
        // ====================================================================================================

        private void checkHorasState() {
            if (!ListaCbHoras.Any(c => c.IsChecked == true)) {
                cbHoras.IsChecked = false;
            } else if (!ListaCbHoras.Any(c => c.IsChecked == false)) {
                cbHoras.IsChecked = true;
            } else {
                cbHoras.IsChecked = null;
            }
        }

        private void changeHoras() {
            if (cbHoras.IsChecked == true) {
                foreach (var cb in ListaCbHoras) cb.IsChecked = true;
            } else if (cbHoras.IsChecked == false) {
                foreach (var cb in ListaCbHoras) cb.IsChecked = false;
            }
        }


        private void cbHoras_Click(object sender, System.Windows.RoutedEventArgs e) {
            changeHoras();
            if (cbHoras.IsChecked != null) ComboPresets.SelectedIndex = 0;
        }

        private void UnaHora_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkHorasState();
            ComboPresets.SelectedIndex = 0;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region DÍAS
        // ====================================================================================================

        private void checkDiasState() {
            if (!ListaCbDias.Any(c => c.IsChecked == true)) {
                cbDias.IsChecked = false;
            } else if (!ListaCbDias.Any(c => c.IsChecked == false)) {
                cbDias.IsChecked = true;
            } else {
                cbDias.IsChecked = null;
            }
        }

        private void changeDias() {
            if (cbDias.IsChecked == true) {
                foreach (var cb in ListaCbDias) cb.IsChecked = true;
            } else if (cbDias.IsChecked == false) {
                foreach (var cb in ListaCbDias) cb.IsChecked = false;
            }
        }


        private void cbDias_Click(object sender, System.Windows.RoutedEventArgs e) {
            changeDias();
            if (cbDias.IsChecked != null) ComboPresets.SelectedIndex = 0;
        }

        private void cbUnDia_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkDiasState();
            ComboPresets.SelectedIndex = 0;
        }



        #endregion
        // ====================================================================================================


    }
}
