#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

	public class NotifyBase : INotifyPropertyChanged {

		/// <summary>
		/// Evento que se lanzará para notificar el cambio en una propiedad.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Evento que se lanzará cuando los cambios en un objeto hagan necesario su guardado.
		/// </summary>
		public event PropertyChangedEventHandler ObjetoCambiado;


		/// <summary>
		/// Establece el objeto como modificado e invoca el evento 'PropertyChanged'.
		/// </summary>
		public void PropiedadCambiada([CallerMemberName] string prop = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}


		/// <summary>
		/// Indica si el objeto ha cambiado.
		/// </summary>
		private bool _modificado;
		public bool Modificado {
			get { return _modificado; }
			set {
				if (_modificado != value) {
					_modificado = value;
					if (_modificado) ObjetoCambiado?.Invoke(this, new PropertyChangedEventArgs(nameof(Modificado)));
					PropiedadCambiada();
				}
			}
		}


		/// <summary>
		/// Indica si el objeto es nuevo.
		/// </summary>
		public bool Nuevo { get; set; }

        /// <summary>
        /// Indica si el objeto va a ser borrado.
        /// </summary>
        public bool Borrado { get; set; }


		/// <summary>
		/// Si el valor de la propiedad es diferente al que se quiere asignar, se cambia y se lanza el
		/// evento PropertyChanged correspondiente a la propiedad. Para ello, hay que pasar el campo
		/// privado por referencia.
		/// <typeparam name="T">Tipo de la propiedad a la que se asigna el valor.</typeparam>
		/// <param name="backingField">Campo privado enlazado a la propiedad.</param>
		/// <param name="value">Valor asignado a la propiedad.</param>
		/// <param name="propertyName">Nombre de la propiedad modificada. Si no se escribe, se infiere su nombre.</param>
		/// <returns>True si se modifica el valor de la propiedad o false si el valor no es modificado por ser igual que el que se pasa.</returns>
		/// </summary>
		protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
			backingField = value;
			Modificado = true;
			PropiedadCambiada(propertyName);
			return true;
		}



	}

}
