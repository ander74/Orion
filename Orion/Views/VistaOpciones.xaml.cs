#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Views {
    using System;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    public partial class VistaOpciones : UserControl {
        public VistaOpciones() {
            InitializeComponent();

            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            gridArticulosConvenio.ContextMenu.Items.Add(new Separator());
            gridArticulosConvenio.ContextMenu.Items.Add(new MenuItem {
                Header = "Borrar Artículo",
                Icon = new Image { Source = new BitmapImage(new Uri($"/{assemblyName};component/Views/Imagenes/BorrarFila.png", UriKind.Relative)) },
                Command = App.Global.OpcionesVM.CmdBorrarArticuloConvenio,
            });
            gridArticulosConvenio.ContextMenu.Items.Add(new MenuItem {
                Header = "Deshacer Borrar",
                Icon = new Image { Source = new BitmapImage(new Uri($"/{assemblyName};component/Views/Imagenes/Volver.png", UriKind.Relative)) },
                Command = App.Global.OpcionesVM.CmdDeshacerBorrarArticuloConvenio,
            });

        }

        private void TextBlock_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (sender is TextBlock textBlock && textBlock.Tag is string tag) {
                App.Global.OpcionesVM.CmdMostrarArticuloConvenio.Execute(tag);
            }
        }
    }
}
