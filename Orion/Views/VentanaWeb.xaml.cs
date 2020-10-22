#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Windows;

namespace Orion.Views {
    /// <summary>
    /// Lógica de interacción para VentanaWeb.xaml
    /// </summary>
    public partial class VentanaWeb : Window {

        public Uri PaginaInicial;
        public Uri PaginaADevolver;

        public VentanaWeb(Uri paginaInicial, Uri paginaADevolver) {
            InitializeComponent();
            this.PaginaInicial = paginaInicial;
            this.PaginaADevolver = paginaADevolver;
        }

        private void Navegador_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e) {
            //if (DataContext == null) return;
            //((GlobalVM)DataContext).PaginaWeb = e.Uri.AbsoluteUri.ToString();
            //if (e.Uri.AbsoluteUri.ToString().StartsWith("https://login.microsoftonline.com/common/oauth2/nativeclient?code=")) {
            //    DialogResult = true;
            //    Close();
            //}
            PaginaInicial = e.Uri;
            if (SonMismasPaginas(PaginaADevolver, e.Uri)) {
                DialogResult = true;
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            webBrowser.Navigate(PaginaInicial);
        }


        private bool SonMismasPaginas(Uri url1, Uri url2) {
            if (url1 == null || url2 == null) return false;
            return url1.Authority.Equals(url2.Authority, StringComparison.OrdinalIgnoreCase) && url1.AbsolutePath.Equals(url2.AbsolutePath);
        }


    }
}
