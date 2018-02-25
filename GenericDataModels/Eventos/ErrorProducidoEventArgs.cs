#region COPYRIGHT
// ===========================================================
//     Copyright 2018 - GenericDataModels 1.0 - A. Herrero    
// -----------------------------------------------------------
//        Vea el archivo Licencia.txt para más detalles 
// ===========================================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataModels {


	/// <summary>
	/// Representa el método que controlará el evento 'ErrorProducido' que se desencadena cuando se 
	/// produce un error en el acceso a la base de datos.
	/// </summary>
	public delegate void ErrorHandler(object sender, ErrorProducidoEventArgs e);


	/// <summary>
	/// Proporciona datos para el evento 'ErrorProducido'.
	/// </summary>
	public class ErrorProducidoEventArgs : EventArgs {


		/// <summary>
		/// Inicializa una nueva instancia de la clase ErrorProducidoEventArgs.
		/// </summary>
		public ErrorProducidoEventArgs(string localizacion, Exception excepcion) {
			_localizacion = localizacion;
			_excepcion = excepcion;
		}


		private string _localizacion;
		/// <summary>
		/// Obtiene el método que ha generado el error.
		/// </summary>
		public string Localizacion {
			get { return _localizacion; }
		}


		private Exception _excepcion;
		/// <summary>
		/// Obtiene la excepción capturada en el método que ha producido el error.
		/// </summary>
		public Exception Excepcion{
			get { return _excepcion; }
		}



	}
}
