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


	public class RelayCommand : ICommand {

		//CAMPOS PRIVADOS
		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		// CONSTRUCTORES
		public RelayCommand(Action<object> execute)
			: this(execute, null) { }

		public RelayCommand(Action<object> execute, Predicate<object> canExecute) {
			if (execute == null) {
				throw new ArgumentNullException("execute");
			}
			this.execute = execute;
			this.canExecute = canExecute;
		}

		// SE PUEDE EJECUTAR
		public bool CanExecute(object parameter) {
			return canExecute == null ? true : canExecute(parameter);
		}

		// EJECUTAR
		public void Execute(object parameter) {
			execute(parameter);
		}


		// EVENTO CAMBIA SE PUEDE EJECUTAR
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}


	} //Final de clase.
}
