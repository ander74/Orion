﻿#region COPYRIGHT
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

	}

}
