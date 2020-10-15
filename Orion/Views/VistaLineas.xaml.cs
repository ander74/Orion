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
    using ViewModels;

    public partial class VistaLineas : UserControl {
        public VistaLineas() {
            InitializeComponent();
        }


        private void Tabla_GotFocus(object sender, RoutedEventArgs e) {
            ((GlobalVM)this.DataContext).LineasVM.TablaParaCopy = sender as DataGrid;
        }


        // AL PULSAR UNO DE LOS BOTONES DEL DESPLEGABLE EXPORTAR
        private void BotonExportar_Click(object sender, RoutedEventArgs e) {
            BtExportarLineas.IsOpen = false;
        }

    }
}
