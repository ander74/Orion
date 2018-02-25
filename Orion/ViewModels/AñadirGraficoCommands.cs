#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Orion.ViewModels
{
    public partial class AñadirGraficoViewModel
    {


		#region BOTON ACEPTAR
		// Comando
		private ICommand _cmdbotonaceptar;

		public ICommand cmdBotonAceptar {
			get {
				if (_cmdbotonaceptar == null) _cmdbotonaceptar = new RelayCommand(p => BotonAceptar(p), p => PuedeBotonAceptar());
				return _cmdbotonaceptar;
			}
		}

		// Se puede ejecutar
		private bool PuedeBotonAceptar() {
			return Numero > 0;
		}

		// Ejecución del comando
		private void BotonAceptar(object parametro) {
			if (parametro == null) return;
			Window ventana = (Window)parametro;
			ventana.DialogResult = true;
			ventana.Close();
		}
		#endregion


		



	}
}
