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


	public class BdRegulacionConductor {

		/*================================================================================
		 * GET REGULACIONES
		 *================================================================================*/
		/// <summary>
		/// Devuelve una colección con las regulaciones pertenecientes a un conductor
		/// </summary>
		/// <param name="IdConductor">Id del conductor al que pertenecen las regulaciones.</param>
		/// <returns>Colección de regulaciones.</returns>
		public static ObservableCollection<RegulacionConductor> GetRegulaciones(int IdConductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista y el comando que extrae los gráficos.
			ObservableCollection<RegulacionConductor> lista = new ObservableCollection<RegulacionConductor>();

			using (conexion) {

				string comandoSQL = "SELECT * FROM Regulaciones WHERE IdConductor=? ORDER BY Fecha,Id";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				OleDbDataReader lector = null;
				Comando.Parameters.AddWithValue("idconductor", IdConductor);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos los gráficos del lector a la lista.
					lector = Comando.ExecuteReader();
					while (lector.Read()) {
						RegulacionConductor regulacion = new RegulacionConductor(lector);
						lista.Add(regulacion);
						regulacion.Nuevo = false;
						regulacion.Modificado = false;
					}
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetRegulaciones", ex);
				} finally {
					lector.Close();
				}
			}
			return lista;
		}


		/*================================================================================
		 * GUARDAR REGULACIONES
		 *================================================================================*/
		/// <summary>
		/// Guarda la lista de regulaciones que se le pasa en la base de datos, actualizando las modificadas e insertando las nuevas.
		/// </summary>
		/// <param name="lista">Lista con las regulaciones a guardar.</param>
		public static void GuardarRegulaciones(ObservableCollection<RegulacionConductor> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (conexion) {

				string SQLInsertar = "INSERT INTO Regulaciones (IdConductor, Codigo, Fecha, Horas, Descansos, Motivo) " +
									 "VALUES (?, ?, ?, ?, ?, ?);";

				string SQLActualizar = "UPDATE Regulaciones SET IdConductor=?, Codigo=?, Fecha=?, Horas=?, Descansos=?, " +
									   "Motivo=? WHERE Id=?;";

				try {
					conexion.Open();

					foreach (RegulacionConductor regulacion in lista) {
						if (regulacion.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							RegulacionConductor.ParseToCommand(comando, regulacion);
							comando.ExecuteNonQuery();
							regulacion.Nuevo = false;
							regulacion.Modificado = false;
						} else if (regulacion.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							RegulacionConductor.ParseToCommand(comando, regulacion);
							comando.ExecuteNonQuery();
							regulacion.Modificado = false;
						}
					}
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GuardarRegulaciones", ex);
				}
			}
		}


		/*================================================================================
 		 * INSERTAR REGULACION
 		 *================================================================================*/
		/// <summary>
		/// Inserta una regulación en la base de datos.
		/// </summary>
		/// <param name="regulacion">Regulación a insertar.</param>
		/// <returns>Id asignado a la nueva regulación.</returns>
		public static int InsertarRegulacion(RegulacionConductor regulacion, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la conexión no existe o la valoracion es nula, devolvemos nulo.
			if (regulacion == null) return -1;

			int resultado = -1;

			using (conexion) {

				string SQLInsertar = "INSERT INTO Regulaciones (IdConductor, Codigo, Fecha, Horas, Descansos, Motivo) " +
								 "VALUES (?, ?, ?, ?, ?, ?);";

				OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
				RegulacionConductor.ParseToCommand(comando, regulacion);

				try {
					conexion.Open();

					comando.ExecuteNonQuery();
					comando.CommandText = "SELECT @@IDENTITY";
					resultado = (int)comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.InsertarRegulacion", ex);
				}
			}
			return resultado;
		}


		/*================================================================================
		 * BORRAR REGULACIONES
		 *================================================================================*/
		/// <summary>
		/// Elimina de la base de datos las regulaciones pasados en la lista.
		/// </summary>
		/// <param name="lista">Lista con las regulaciones a borrar.</param>
		public static void BorrarRegulaciones(List<RegulacionConductor> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			using (conexion) {

				string SQLBorrar = "DELETE FROM Regulaciones WHERE Id=?";

				try {
					conexion.Open();

					foreach (RegulacionConductor regulacion in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", regulacion.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.BorrarRegulaciones", ex);
				}
			}
		}




	} //Fin de clase.
}
