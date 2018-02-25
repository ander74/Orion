#region COPYRIGHT
// ===========================================================
//     Copyright 2018 - GenericDataModels 1.0 - A. Herrero    
// -----------------------------------------------------------
//        Vea el archivo Licencia.txt para más detalles 
// ===========================================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataModels {

	/// <summary>
	/// Esta interface debe ser implementada por los objetos que van a ser extraidos de una base de datos a través de la biblioteca GenericDataModels.
	/// </summary>
	public interface IModelOleDb {


		/// <summary>
		/// Devuelve o establece el identificador único del objeto.
		/// </summary>
		int Id { get; set; }


		/// <summary>
		/// Devuelve el Id que relaciona el objeto principal con los objetos de la lista interna.
		/// </summary>
		int IdRelacionado { get; set; }



		/// <summary>
		/// Extrae los datos de un lector de base de datos y los parsea en las propiedades del objeto
		/// </summary>
		void SetDataFromReader(OleDbDataReader reader);


		/// <summary>
		/// Inserta las propiedades del objeto como parámetros en un comando SQL.
		/// </summary>
		void GetDataToCommand(ref OleDbCommand comando);


		/// <summary>
		/// Devuelve o establece si el objeto es nuevo en la colección.
		/// </summary>
		bool Nuevo { get; set; }


		/// <summary>
		/// Devuelve o establece si el objeto ha sido modificado.
		/// </summary>
		bool Modificado { get; set; }

	}
}
