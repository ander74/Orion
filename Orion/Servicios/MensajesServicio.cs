#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Linq;
using System.Windows;
using Orion.Convertidores;
using Orion.ViewModels;
using Orion.Views;

namespace Orion.Servicios {

    public interface IMensajes {

        bool? VerMensaje(string mensaje, string titulo, bool SiNo = false, bool Cancelar = false);

        void VerError(string localizacion, Exception ex);

        void VerArticuloConvenio(int codigo);

    }

    public class MensajesServicio : IMensajes {


        private VentanaError ventanaError;
        private VentanaMensajes ventanaMensajes;
        private VentanaArticuloConvenio ventanaArticuloConvenio;


        /// <summary>
        /// Enlaza un MessageBox con un ViewModel utilizando Dependency Injection.
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar.</param>
        /// <param name="titulo">Título de la ventana.</param>
        /// <param name="SiNo">Opcional. Si es true, muestra los botones Si/No. Si es false (predeterminado) muestra el botón Aceptar.</param>
        /// <returns>Devuelve True si se ha pulsado el botón Si, o Aceptar; False si se ha pulsado en No.</returns>
        public bool? VerMensaje(string mensaje, string titulo, bool SiNo = false, bool Cancelar = false) {

            VentanaMensajesVM vm = new VentanaMensajesVM() { Titulo = titulo, Mensaje = mensaje };
            if (SiNo) {
                vm.TextoBotonSi = "Si";
                vm.VerBotonNo = true;
            }
            if (Cancelar) {
                vm.VerBotonCancelar = true;
            }
            ventanaMensajes = new VentanaMensajes() { DataContext = vm, ShowInTaskbar = false };

            ventanaMensajes.ShowDialog();

            return ventanaMensajes.Resultado;
        }


        /// <summary>
        /// Muestra un mensaje de error utilizando una ventana personalizada.
        /// </summary>
        /// <param name="localizacion">Clase.Método donde se produce la excepción.</param>
        /// <param name="ex">Excepción producida.</param>
        public void VerError(string localizacion, Exception ex) {

            ventanaError = new VentanaError();
            ventanaError.Owner = Application.Current.MainWindow;
            ventanaError.ShowInTaskbar = false;
            ventanaError.TbLocalizacion.Text = localizacion + " -- " + ex.Source;
            ventanaError.TbDescripcion.Text = ex.Message + "\n\n--- TRAZADO DEL ERROR ---\n\n" + ex.StackTrace;
            ventanaError.ShowDialog();
        }


        public void VerArticuloConvenio(int codigo) {
            var articulo = App.Global.OpcionesVM.ArticulosConvenio.FirstOrDefault(a => a.CodigoFuncionRelacionada == codigo);
            if (articulo == null) return;
            ventanaArticuloConvenio = new VentanaArticuloConvenio();
            ventanaArticuloConvenio.Owner = Application.Current.MainWindow;
            ventanaArticuloConvenio.ShowInTaskbar = false;
            var converter = new ConvertidorNumeroArticuloConvenio();
            ventanaArticuloConvenio.txtNumero.Text = (string)converter.Convert(articulo.Numero, typeof(string), null, null);
            ventanaArticuloConvenio.txtTitulo.Text = articulo.Titulo;
            ventanaArticuloConvenio.tbTexto.Text = articulo.Texto;
            ventanaArticuloConvenio.ShowDialog();
        }
    }
}
