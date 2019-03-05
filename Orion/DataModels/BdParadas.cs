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
	/// Contiene los métodos necesarios para extraer las paradas de la base de datos.
	/// </summary>
	public static class BdParadas {


		/*================================================================================
 		 * GET PARADAS
		 *================================================================================*/
		public static ObservableCollection<Parada> GetParadas(long IdItinerario) {

			// Creamos la lista y el comando que extrae los gráficos.
			ObservableCollection<Parada> lista = new ObservableCollection<Parada>();

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas))
			{
				string comandoSQL = "SELECT * FROM Paradas WHERE IdItinerario=? ORDER BY Orden";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.Parameters.AddWithValue("iditinerario", IdItinerario);
				OleDbDataReader lector = null;

				// Ejecutamos el comando y extraemos los gráficos del lector a la lista.
				try {
					conexion.Open();
					lector = Comando.ExecuteReader();

					while (lector.Read()) {
						Parada parada = new Parada(lector);
						lista.Add(parada);
						parada.Nuevo = false;
						parada.Modificado = false;
					}
				} catch (Exception ex) {
					Utils.VerError("BdParadas.GetParadas", ex);
				} finally {
					lector.Close();
				}
			}
			return lista;
		}


		/*================================================================================
		* GUARDAR PARADAS
		*================================================================================*/
		public static void GuardarParadas(ObservableCollection<Parada> lista) {

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas))
			{
				string SQLInsertar = "INSERT INTO Paradas (IdItinerario, Orden, Descripcion, Tiempo) " +
									 "VALUES (?, ?, ?, ?);";

				string SQLActualizar = "UPDATE Paradas SET IdItinerario=?, Orden=?, Descripcion=?, Tiempo=? WHERE Id=?;";

				try {
					conexion.Open();

					foreach (Parada parada in lista) {
						if (parada.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							Parada.ParseToCommand(comando, parada);
							comando.ExecuteNonQuery();
							parada.Nuevo = false;
							parada.Modificado = false;
						} else if (parada.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							Parada.ParseToCommand(comando, parada);
							comando.ExecuteNonQuery();
							parada.Modificado = false;
						}
					}
				} catch (Exception ex) {
					Utils.VerError("BdParadas.GuardarParadas", ex);
				}
			}
		}


		/*================================================================================
		 * BORRAR PARADAS
		 *================================================================================*/
		public static void BorrarParadas(List<Parada> lista) {

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas))
			{
				string SQLBorrar = "DELETE FROM Paradas WHERE Id=?";

				try {
					conexion.Open();

					foreach (Parada parada in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", parada.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdParadas.BorrarParadas", ex);
				}
			}
		}


	}
}
