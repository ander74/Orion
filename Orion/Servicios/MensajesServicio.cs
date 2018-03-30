#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.ViewModels;
using Orion.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Orion.Servicios{

	public interface IMensajes {

		bool? VerMensaje(string mensaje, string titulo, bool SiNo = false, bool Cancelar = false);

		void VerError(string localizacion, Exception ex);

	}

	public class MensajesServicio :IMensajes {


		private VentanaError ventanaError;
		private VentanaMensajes ventanaMensajes;


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
			ventanaMensajes = new VentanaMensajes() { DataContext = vm, ShowInTaskbar= false };

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

	}
}
