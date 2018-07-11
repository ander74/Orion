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
using System.Windows.Input;

namespace Orion.ViewModels {

	public partial class ResumenAnualViewModel {



		#region COMANDO AÑO ANTERIOR

		// Comando
		private ICommand añoAnterior;
		public ICommand cmdAñoAnterior {
			get {
				if (añoAnterior == null) añoAnterior = new RelayCommand(p => AñoAnterior());
				return añoAnterior;
			}
		}


		// Ejecución del comando
		private void AñoAnterior() {
			AñoActual--;
		}
		#endregion


		#region COMANDO AÑO SIGUIENTE

		// Comando
		private ICommand añoSiguiente;
		public ICommand cmdAñoSiguiente {
			get {
				if (añoSiguiente == null) añoSiguiente = new RelayCommand(p => AñoSiguiente());
				return añoSiguiente;
			}
		}


		// Ejecución del comando
		private void AñoSiguiente() {
			AñoActual++;
		}
		#endregion





	}
}
