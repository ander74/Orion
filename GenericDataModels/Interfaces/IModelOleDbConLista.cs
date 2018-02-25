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


	public interface IModelOleDbConLista : IModelOleDb { 


		/// <summary>
		/// Establece la lista interna de objetos del objeto principal.
		/// </summary>
		IEnumerable<IModelOleDb> ListaInterna { get;  set; }


	}
}
