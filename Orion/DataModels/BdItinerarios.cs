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
	/// Contiene los métodos necesarios para extraer los itinerarios de la base de datos.
	/// </summary>
	public static class BdItinerarios {


		/*================================================================================
		 * GET ITINERARIOS
		 *================================================================================*/
		public static ObservableCollection<Itinerario> GetItinerarios(long IdLinea, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			// Creamos la lista.
			ObservableCollection<Itinerario> lista = new ObservableCollection<Itinerario>();

			using (conexion) {

				string comandoSQL = "SELECT * FROM Itinerarios WHERE IdLinea=? ORDER BY Nombre";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.Parameters.AddWithValue("idlinea", IdLinea);
				OleDbDataReader lector = null;

				try {
					conexion.Open();
					// Ejecutamos el comando y extraemos los gráficos del lector a la lista.
					lector = Comando.ExecuteReader();
					while (lector.Read()) {
						Itinerario itinerario = new Itinerario(lector);
						itinerario.ListaParadas = BdParadas.GetParadas(itinerario.Id);
						lista.Add(itinerario);
						itinerario.Nuevo = false;
						itinerario.Modificado = false;
					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdItinerarios.GetItinerarios", ex);
				}
			}
			return lista;
		}


		/*================================================================================
		 * GUARDAR ITINERARIOS
		 *================================================================================*/
		public static void GuardarItinerarios(ObservableCollection<Itinerario> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (conexion) {

				string SQLInsertar = "INSERT INTO Itinerarios (IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago) " +
									 "VALUES (?, ?, ?, ?, ?);";

				string SQLActualizar = "UPDATE Itinerarios SET IdLinea=?, Nombre=?, Descripcion=?, TiempoReal=?, TiempoPago=? WHERE Id=?;";
				string SQLGetID = "SELECT @@IDENTITY;";

				try {
					conexion.Open();

					foreach (Itinerario itinerario in lista) {
						if (itinerario.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							Itinerario.ParseToCommand(comando, itinerario);
							comando.ExecuteNonQuery();
							comando.CommandText = SQLGetID;
							int iditinerario = (int)comando.ExecuteScalar();
							foreach (Parada parada in itinerario.ListaParadas) {
								parada.IdItinerario = iditinerario;
							}
							itinerario.Id = iditinerario;
							itinerario.Nuevo = false;
							itinerario.Modificado = false;
						} else if (itinerario.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							Itinerario.ParseToCommand(comando, itinerario);
							comando.ExecuteNonQuery();
							itinerario.Modificado = false;
						}
						BdParadas.GuardarParadas(itinerario.ListaParadas);
						BdParadas.BorrarParadas(itinerario.ParadasBorradas);
					}
				} catch (Exception ex) {
					Utils.VerError("BdItinerarios.GuardarItinerarios", ex);
				}
			}
		}


		/*================================================================================
		 * BORRAR ITINERARIOS
		 *================================================================================*/
		public static void BorrarItinerarios(List<Itinerario> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			using (conexion) {

				string SQLBorrar = "DELETE FROM Itinerarios WHERE Id=?";

				try {
					conexion.Open();

					foreach (Itinerario itinerario in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", itinerario.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdItinerarios.BorrarItinerarios", ex);
				}
			}
		}


		/*================================================================================
		 * GET ITINERARIO BY NOMBRE
		 *================================================================================*/
		 public static Itinerario GetItinerarioByNombre(decimal nombre, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexionLineas);

			Itinerario itinerario = null;

			using (conexion) {

				// Creamos el comando que extrae el itinerario
				string comandoSQL = "SELECT * FROM Itinerarios WHERE Nombre = ?";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.Parameters.AddWithValue("nombre", nombre.ToString());
				OleDbDataReader lector = null;

				try {
					conexion.Open();

					lector = Comando.ExecuteReader();

					if (lector.Read()) {
						itinerario = new Itinerario(lector);
						itinerario.ListaParadas = BdParadas.GetParadas(itinerario.Id);
					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdItinerarios.GetItinerarioByNombre", ex);
				}
			}

			return itinerario;

		}



	}
}
