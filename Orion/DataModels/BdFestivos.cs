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
	public static class BdFestivos {


		/*================================================================================
		* GET FESTIVOS POR MES
		*================================================================================*/
		public static ObservableCollection<Festivo> GetFestivosPorMes(int año, int mes, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista y el comando que extrae las líneas.
			ObservableCollection<Festivo> lista = new ObservableCollection<Festivo>();

			using (conexion) {

				//string comandoSQL = "SELECT * FROM Festivos WHERE Año=? AND Month(Fecha)=? ORDER BY Fecha;";
				string comandoSQL = "GetFestivosPorMes";

				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.CommandType = System.Data.CommandType.StoredProcedure;
				comando.Parameters.AddWithValue("Año", año);
				comando.Parameters.AddWithValue("Mes", mes);
				OleDbDataReader lector = null;

				try {
					conexion.Open();

					lector = comando.ExecuteReader();

					while (lector.Read()) {
						Festivo festivo = new Festivo(lector);
						lista.Add(festivo);
						festivo.Nuevo = false;
						festivo.Modificado = false;
					}
				} catch (Exception ex) {
					Utils.VerError("BdFestivos.GetFestivosPorMes", ex);
				} finally {
					lector.Close();
				}
			}
			return lista;
		}


		/*================================================================================
		* GET FESTIVOS POR AÑO
		*================================================================================*/
		public static ObservableCollection<Festivo> GetFestivosPorAño(int año, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista y el comando que extrae las líneas.
			ObservableCollection<Festivo> lista = new ObservableCollection<Festivo>();

			using (conexion) {

				//string comandoSQL = "SELECT * FROM Festivos WHERE Año=? ORDER BY Fecha;";
				string comandoSQL = "GetFestivosPorAño";

				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.CommandType = System.Data.CommandType.StoredProcedure;
				comando.Parameters.AddWithValue("Año", año);
				OleDbDataReader lector = null;

				try {
					conexion.Open();

					lector = comando.ExecuteReader();

					while (lector.Read()) {
						Festivo festivo = new Festivo(lector);
						lista.Add(festivo);
						festivo.Nuevo = false;
						festivo.Modificado = false;
					}
				} catch (Exception ex) {
					Utils.VerError("BdFestivos.GetFestivosPorAño", ex);
				} finally {
					lector.Close();
				}
			}
			return lista;
		}


		/*================================================================================
		* GUARDAR FESTIVOS
		*================================================================================*/
		public static void GuardarFestivos(IEnumerable<Festivo> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count() == 0) return;

			using (conexion) {

				//string SQLInsertar = "INSERT INTO Festivos (Año, Fecha) VALUES (?, ?)";
				string SQLInsertar = "InsertarFestivo";

				//string SQLActualizar = "UPDATE Festivos SET Año=?, Fecha=? WHERE Id=?";
				string SQLActualizar = "ActualizarFestivo";

				try {
					conexion.Open();

					foreach (Festivo festivo in lista) {
						if (festivo.Nuevo) {
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;
							Festivo.ParseToCommand(comando, festivo);
							comando.ExecuteNonQuery();
							festivo.Nuevo = false;
							festivo.Modificado = false;
						} else if (festivo.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;
							Festivo.ParseToCommand(comando, festivo);
							comando.ExecuteNonQuery();
							festivo.Modificado = false;
						}
					}
				} catch (Exception ex) {
					Utils.VerError("BdFestivos.GuardarFestivos", ex);
				}
			}
		}


		/*================================================================================
		* BORRAR FESTIVOS
		*================================================================================*/
		public static void BorrarFestivos(IEnumerable<Festivo> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			using (conexion) {

				//string SQLBorrar = "DELETE FROM Festivos WHERE Id=?";
				string SQLBorrar = "BorrarFestivo";

				try {
					conexion.Open();

					foreach (Festivo festivo in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.CommandType = System.Data.CommandType.StoredProcedure;
						comando.Parameters.AddWithValue("id", festivo.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdFestivos.BorrarFestivo", ex);
				}
			}
		}


	}
}
