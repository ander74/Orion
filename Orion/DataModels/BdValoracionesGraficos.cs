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


	public class BdValoracionesGraficos {


		/*================================================================================
		* GET VALORACIONES
		*================================================================================*/
		/// <summary>
		/// Devuelve una colección con las valoraciones pertenecientes a un gráfico
		/// </summary>
		/// <param name="IdGrafico">Id del gráfico al que pertenecen las valoraciones.</param>
		/// <returns>Colección de valoraciones.</returns>
		public static ObservableCollection<ValoracionGrafico> getValoraciones(long IdGrafico, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la conexión no existe, devolvemos nulo.
			//if (App.Global.Conexion == null) return null;

			// Creamos la lista y el comando que extrae los gráficos.
			ObservableCollection<ValoracionGrafico> lista = new ObservableCollection<ValoracionGrafico>();

			using (conexion) {
				string comandoSQL = "SELECT * FROM Valoraciones WHERE IdGrafico=? ORDER BY Inicio, Id";
				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				OleDbDataReader lector = null;
				Comando.Parameters.AddWithValue("idgrafico", IdGrafico);

				// Ejecutamos el comando y extraemos los gráficos del lector a la lista.
				try {

					conexion.Open();
					lector = Comando.ExecuteReader();

					while (lector.Read()) {
						ValoracionGrafico valoracion = new ValoracionGrafico(lector);
						lista.Add(valoracion);
						valoracion.Nuevo = false;
						valoracion.Modificado = false;
					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdValoracionesGraficos.GetValoraciones", ex);
				}
			}
			return lista;
		}


		/*================================================================================
		* GUARDAR VALORACIONES
		*================================================================================*/
		/// <summary>
		/// Guarda la lista de valoraciones que se le pasa en la base de datos, actualizando las modificadas e insertando las nuevas.
		/// </summary>
		/// <param name="lista">Lista con las valoraciones a guardar.</param>
		public static void GuardarValoraciones(ObservableCollection<ValoracionGrafico> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la conexión no existe, devolvemos nulo.
			//if (App.Global.Conexion == null) return;

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (conexion) {

				string SQLInsertar = "INSERT INTO Valoraciones (IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo) " +
									 "VALUES (?, ?, ?, ?, ?, ?);";

				string SQLActualizar = "UPDATE Valoraciones SET IdGrafico=?, Inicio=?, Linea=?, Descripcion=?, " +
									   "Final=?, Tiempo=? WHERE Id=?;";

				try {
					foreach (ValoracionGrafico valoracion in lista) {
						if (valoracion.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							ValoracionGrafico.ParseToCommand(comando, valoracion);
							if (conexion.State == System.Data.ConnectionState.Closed) conexion.Open();
							comando.ExecuteNonQuery();
							valoracion.Nuevo = false;
							valoracion.Modificado = false;
						} else if (valoracion.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar,conexion);
							ValoracionGrafico.ParseToCommand(comando, valoracion);
							conexion.Open();
							comando.ExecuteNonQuery();
							valoracion.Modificado = false;
						}
					}
				} catch (OleDbException ex) {
					Utils.VerError("BdValoracionesGraficos.GuardarValoraciones", ex);
				}
			}
		}


		/*================================================================================
		* INSERTAR VALORACION
		*================================================================================*/
		/// <summary>
		/// Guarda la lista de valoraciones que se le pasa en la base de datos, actualizando las modificadas e insertando las nuevas.
		/// </summary>
		/// <param name="lista">Lista con las valoraciones a guardar.</param>
		public static void InsertarValoracion(ValoracionGrafico valoracion, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la conexión no existe o la valoracion es nula, devolvemos nulo.
			if (App.Global.CadenaConexion == null || valoracion == null) return;

			string SQLInsertar = "INSERT INTO Valoraciones (IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo) " +
								 "VALUES (?, ?, ?, ?, ?, ?);";

			using (conexion) {

				OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
				ValoracionGrafico.ParseToCommand(comando, valoracion);

				try {
					//App.Global.AbrirConexion();
					conexion.Open();
					comando.ExecuteNonQuery();
				} catch (OleDbException ex) {
					Utils.VerError("BdValoracionesGraficos.InsertarValoracion", ex);
				}
			}
		}

		
		/*================================================================================
		 * BORRAR VALORACIONES
		 *================================================================================*/
		/// <summary>
		/// Elimina de la base de datos las valoraciones pasados en la lista.
		/// </summary>
		/// <param name="lista">Lista con las valoraciones a borrar.</param>
		public static void BorrarValoraciones(List<ValoracionGrafico> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			string SQLBorrar = "DELETE FROM Valoraciones WHERE Id=?";

			using (conexion) {

				try {
					//App.Global.AbrirConexion();
					conexion.Open(); // TODO: Probar a ver si podemos borrar todas usando Where Id IN (lista)
					foreach (ValoracionGrafico valoracion in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", valoracion.Id);
						comando.ExecuteNonQuery();
					}
				} catch (OleDbException ex) {
					Utils.VerError("BdValoracionesGraficos.BorrarValoraciones", ex);
				}
			}
		}



	} //Final de clase.
}
