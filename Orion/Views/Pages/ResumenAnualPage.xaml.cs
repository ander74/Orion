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

        List<CheckBox> ListaCbHoras;
        List<CheckBox> ListaCbDias;
        List<CheckBox> ListaCbFindes;
        List<CheckBox> ListaCbPluses;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public ResumenAnualPage() {
            InitializeComponent();
            // Lista Horas
            ListaCbHoras = new List<CheckBox> {
                cbHorasTrabajadas,
                cbHorasAcumuladas,
                cbHorasNocturnas,
                cbHorasCobradas,
                cbOtrasHoras,
                cbExcesosJornada,
            };
            // Lista Días
            ListaCbDias = new List<CheckBox> {
                cbDiasActivos,
                cbDiasInactivos,
                cbDiasTrabajados,
                cbDiasJD,
                cbDiasFN,
                cbDiasDS,
                cbDiasTrabajadosJD,
                cbDiasOV,
                cbDiasDC,
                cbDiasDND,
                cbDiasE,
                cbDiasPER,
                cbDiasF6,
                cbDiasComite,
                cbDiasComiteJD,
                cbDiasComiteDC,
            };
            // Lista Findes
            ListaCbFindes = new List<CheckBox> {
                cbSabadosTrabajados,
                cbSabadosDescansados,
                cbDomingosTrabajados,
                cbDomingosDescansados,
                cbFestivosTrabajados,
                cbFestivosDescansados,
                cbFindesCompletos,
            };
            // Lista Pluses
            ListaCbPluses = new List<CheckBox> {
                cbPlusesSabado,
                cbPlusesFestivo,
                cbPlusesLimpieza,
                cbPlusesNocturnidad,
                cbVariablesVacaciones,
            };
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
            // Findes
            cbFindes.IsChecked = false;
            changeFindes();
            // Pluses
            cbPluses.IsChecked = false;
            changePluses();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PRESETS Y VACIAR
        // ====================================================================================================

        private void ComboPresets_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = ((ComboBox)sender).SelectedItem as ComboBoxItem;
            if (item == null) return;
            switch (item.Tag) {
                case "VariablesVacaciones":
                    DesSelectAll();
                    cbSabadosTrabajados.IsChecked = true;
                    cbPlusesSabado.IsChecked = true;
                    cbDomingosTrabajados.IsChecked = true;
                    cbFestivosTrabajados.IsChecked = true;
                    cbPlusesFestivo.IsChecked = true;
                    cbPlusesLimpieza.IsChecked = true;
                    cbPlusesNocturnidad.IsChecked = true;
                    cbVariablesVacaciones.IsChecked = true;
                    checkFindesState();
                    checkPlusesState();
                    break;
            }
        }

        private void BtVaciar_Click(object sender, System.Windows.RoutedEventArgs e) {
            DesSelectAll();
            ComboPresets.SelectedIndex = -1;
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
            if (cbHoras.IsChecked != null) ComboPresets.SelectedIndex = -1;
        }

        private void UnaHora_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkHorasState();
            ComboPresets.SelectedIndex = -1;
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
            if (cbDias.IsChecked != null) ComboPresets.SelectedIndex = -1;
        }

        private void cbUnDia_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkDiasState();
            ComboPresets.SelectedIndex = -1;
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region FINES DE SEMANA
        // ====================================================================================================

        private void checkFindesState() {
            if (!ListaCbFindes.Any(c => c.IsChecked == true)) {
                cbFindes.IsChecked = false;
            } else if (!ListaCbFindes.Any(c => c.IsChecked == false)) {
                cbFindes.IsChecked = true;
            } else {
                cbFindes.IsChecked = null;
            }
        }

        private void changeFindes() {
            if (cbFindes.IsChecked == true) {
                foreach (var cb in ListaCbFindes) cb.IsChecked = true;
            } else if (cbFindes.IsChecked == false) {
                foreach (var cb in ListaCbFindes) cb.IsChecked = false;
            }
        }


        private void cbFindes_Click(object sender, System.Windows.RoutedEventArgs e) {
            changeFindes();
            if (cbFindes.IsChecked != null) ComboPresets.SelectedIndex = -1;
        }

        private void UnFinde_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkFindesState();
            ComboPresets.SelectedIndex = -1;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PLUSES
        // ====================================================================================================

        private void checkPlusesState() {
            if (!ListaCbPluses.Any(c => c.IsChecked == true)) {
                cbPluses.IsChecked = false;
            } else if (!ListaCbPluses.Any(c => c.IsChecked == false)) {
                cbPluses.IsChecked = true;
            } else {
                cbPluses.IsChecked = null;
            }
        }

        private void changePluses() {
            if (cbPluses.IsChecked == true) {
                foreach (var cb in ListaCbPluses) cb.IsChecked = true;
            } else if (cbPluses.IsChecked == false) {
                foreach (var cb in ListaCbPluses) cb.IsChecked = false;
            }
        }

        private void cbPluses_Click(object sender, System.Windows.RoutedEventArgs e) {
            changePluses();
            if (cbPluses.IsChecked != null) ComboPresets.SelectedIndex = -1;
        }

        private void cbUnPlus_Click(object sender, System.Windows.RoutedEventArgs e) {
            checkPlusesState();
            ComboPresets.SelectedIndex = -1;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region DESTINO DEL RESUMEN
        // ====================================================================================================

        private void ComboDestino_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (sender is ComboBox combo) {
                if (combo.SelectedIndex == 0) {
                    if (cbIncluirTaquilla != null) cbIncluirTaquilla.IsEnabled = false;
                    if (ComboTrabajadores != null) ComboTrabajadores.IsEnabled = true;
                    if (ComboFiltrado != null) ComboFiltrado.IsEnabled = false;
                } else {
                    if (cbIncluirTaquilla != null) cbIncluirTaquilla.IsEnabled = true;
                    if (ComboTrabajadores != null) ComboTrabajadores.IsEnabled = false;
                    if (ComboFiltrado != null) ComboFiltrado.IsEnabled = true;
                }
            }
        }


        #endregion
        // ====================================================================================================



    }
}
