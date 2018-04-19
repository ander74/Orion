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
using System.Data.OleDb;
using System.Collections.ObjectModel;
using Orion.Models;
using System.Windows;
using Orion.Config;

namespace Orion.DataModels {

	/// <summary>
	/// Contiene los métodos necesarios para extraer las líneas de la base de datos.
	/// </summary>
	public static class BdLineas {


		/*================================================================================
		* GET LÍNEAS
		*================================================================================*/
		public static ObservableCollection<Linea> GetLineas(OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			// Creamos la lista y el comando que extrae las líneas.
			ObservableCollection<Linea> lista = new ObservableCollection<Linea>();

			using (conexion) {

				string comandoSQL = "SELECT * FROM Lineas ORDER BY Nombre;";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				OleDbDataReader lector = null;

				try {
					conexion.Open();

					lector = Comando.ExecuteReader();

					while (lector.Read()) {
						Linea linea = new Linea(lector);
						linea.ListaItinerarios = BdItinerarios.GetItinerarios(linea.Id);
						lista.Add(linea);
						linea.Nuevo = false;
						linea.Modificado = false;
					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdLineas.GetLineas", ex);
				}
			}
			return lista;
		}


		/*================================================================================
		* GUARDAR LÍNEAS
		*================================================================================*/
		public static void GuardarLineas(ObservableCollection<Linea> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (conexion) {

				string SQLInsertar = "INSERT INTO Lineas (Nombre, Descripcion) VALUES (?, ?)";

				string SQLActualizar = "UPDATE Lineas SET Nombre=?, Descripcion=? WHERE Id=?";

				string SQLGetID = "SELECT @@IDENTITY;";

				try {
					conexion.Open();

					foreach (Linea linea in lista) {
						if (linea.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							Linea.ParseToCommand(comando, linea);
							comando.ExecuteNonQuery();
							comando.CommandText = SQLGetID;
							int idlinea = (int)comando.ExecuteScalar();
							foreach (Itinerario itinerario in linea.ListaItinerarios) {
								itinerario.IdLinea = idlinea;
							}
							linea.Nuevo = false;
							linea.Modificado = false;
						} else if (linea.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							Linea.ParseToCommand(comando, linea);
							comando.ExecuteNonQuery();
							linea.Modificado = false;
						}
					}
					foreach(Linea linea in lista) {
						BdItinerarios.GuardarItinerarios(linea.ListaItinerarios);
						BdItinerarios.BorrarItinerarios(linea.ItinerariosBorrados);
					}
				} catch (Exception ex) {
					Utils.VerError("BdLineas.GuardarLineas", ex);
				}
			}
		}


		/*================================================================================
		* BORRAR LÍNEAS
		*================================================================================*/
		public static void BorrarLineas(List<Linea> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			using (conexion) {

				string SQLBorrar = "DELETE FROM Lineas WHERE Id=?";

				try {
					conexion.Open();

					foreach (Linea linea in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", linea.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdLineas.BorrarLineas", ex);
				}
			}
		}


	}
}
