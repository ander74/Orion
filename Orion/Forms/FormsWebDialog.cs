#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Windows.Forms;

namespace Orion.Forms {
    public partial class FormsWebDialog : Form {

        public string PaginaDevuelta = "";

        private string paginaADevolver = "";


        public FormsWebDialog() {
            InitializeComponent();
        }


        public void OnNavigated(object sender, WebBrowserNavigatedEventArgs e) {
            if (this.webBrowser.IsDisposed) {
                // If the browser is disposed, just cancel out gracefully
                return;
            }
            if (!string.IsNullOrEmpty(paginaADevolver) && e.Url.AbsoluteUri.ToString().StartsWith(paginaADevolver)) {
                PaginaDevuelta = e.Url.AbsoluteUri.ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public void OnNavigating(object sender, WebBrowserNavigatingEventArgs e) {
            // Si el navegador se ha cerrado, salir.
            if (this.webBrowser.IsDisposed) return;

            if (!string.IsNullOrEmpty(paginaADevolver) && e.Url.AbsoluteUri.ToString().StartsWith(paginaADevolver)) {
                e.Cancel = true;
                this.Close();
            }
        }

        public void Navegar(string url, string paginaADevolver) {
            this.paginaADevolver = paginaADevolver;
            webBrowser.Navigate(url);
        }



    }
}
