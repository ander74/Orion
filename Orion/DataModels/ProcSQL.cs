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

namespace Orion.DataModels {

	/// <summary>
	/// Clase que contiene accessos a todos los procedimientos almacenados de la base de datos.
	/// </summary>
	public static class ProcSQL {

		/// <summary>
		/// Procedimiento almacenado que devuelve todos los conductores.
		/// </summary>
		public static string GetConductores => "GetConductores";


	}
}
