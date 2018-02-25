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
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetRegulaciones", ex);
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

		//------------------------------------------------------------------------------------------------------------------------

		/*================================================================================
		 * GET HORAS REGULADAS HASTA MES
		 *================================================================================*/
		/// <summary>
		/// Devuelve todas las horas de regulación hasta final del mes dado.
		/// (No se incluyen las cobradas ni las reguladas al final del año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasReguladasHastaMes(int idconductor, int año, int mes) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 0 AND Fecha < ?;";

				// Establecemos la fecha a evaluar
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasReguladasHastaMes", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}


		/*================================================================================
		 * GET HORAS REGULADAS MES
		 *================================================================================*/
		/// <summary>
		/// Devuelve todas las horas de regulación del mes dado.<br/>
		/// (No se incluyen las cobradas ni las reguladas al final del año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasReguladasMes(int idconductor, int año, int mes) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 0" +
								"      Year(Fecha) = ? AND Month(Fecha) = ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasReguladasMes", ex);
				}
			}
			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}


		/*================================================================================
		 * GET HORAS REGULADAS AÑO
		 *================================================================================*/
		/// <summary>
		/// Devuelve todas las horas de regulación del año dado.<br/>
		/// (No se incluyen las cobradas ni las reguladas al final del año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasReguladasAño(int idconductor, int año) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 0 AND Year(Fecha) = ?;";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasReguladasAño", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}

		//------------------------------------------------------------------------------------------------------------------------

		/*================================================================================
		 * GET HORAS REGULADAS FIN AÑO
		 *================================================================================*/
		/// <summary>
		/// Devuelve las horas reguladas, consecuencia de la regulación de final de año.
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasReguladasFinAño(int idconductor, int año) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 2 AND Year(Fecha) = ?;";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasReguladasFinAño", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}

		//------------------------------------------------------------------------------------------------------------------------

		/*================================================================================
		 * GET HORAS COBRADAS MES
		 *================================================================================*/
		/// <summary>
		/// Devuelve las horas cobradas del mes dado.
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasCobradasMes(int idconductor, int año, int mes) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 1" +
								"      Year(Fecha) = ? AND Month(Fecha) = ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasCobradasMes", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}


		/*================================================================================
		 * GET HORAS COBRADAS AÑO
		 *================================================================================*/
		/// <summary>
		/// Devuelve las horas cobradas del año dado.
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasCobradasAño(int idconductor, int año) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 1 AND Year(Fecha) = ?;";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasCobradasAño", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}


		/*================================================================================
		 * GET HORAS COBRADAS HASTA
		 *================================================================================*/
		/// <summary>
		/// Devuelve las horas cobradas hasta el mes dado.
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static TimeSpan GetHorasCobradasHastaMes(int idconductor, int año, int mes) {

			long ticks = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 1 AND Fecha < ?;";

				// Establecemos la fecha a evaluar
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos las horas.
					object resultado = comando.ExecuteScalar();
					ticks = resultado == DBNull.Value ? 0 : (long)(double)resultado;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetHorasCobradasHastaMes", ex);
				}
			}

			// Devolvemos el resultado.
			return new TimeSpan(ticks);

		}

		//------------------------------------------------------------------------------------------------------------------------

		/*================================================================================
		 * GET DESCANSOS REGULADOS HASTA MES
		 *================================================================================*/
		/// <summary>
		/// Devuelve los descansos regulados hasta el mes dado.
		/// (No se incluyen los descansos generados en las regulaciones de final de año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static int GetDescansosReguladosHastaMes(int idconductor, int año, int mes) {

			int resultado = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Descansos) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND COdigo = 0 AND Fecha < ?;";

				// Establecemos la fecha a evaluar
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos los descansos.
					object r = comando.ExecuteScalar();
					resultado = r == DBNull.Value ? 0 : (int)(double)r;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetDescansosReguladosHastaMes", ex);
				}
			}

			// Devolvemos el resultado.
			return resultado;

		}


		/*================================================================================
		 * GET DESCANSOS REGULADOS MES
		 *================================================================================*/
		/// <summary>
		/// Devuelve los descansos regulados del mes dado.
		/// (No se incluyen los descansos generados en las regulaciones de final de año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static int GetDescansosReguladosMes(int idconductor, int año, int mes) {

			int resultado = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Descansos) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 0" +
								"      Year(Fecha) = ? AND Month(Fecha) = ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos los descansos.
					object r = comando.ExecuteScalar();
					resultado = r == DBNull.Value ? 0 : (int)(double)r;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetDescansosReguladosMes", ex);
				}
			}

			// Devolvemos el resultado.
			return resultado;

		}


		/*================================================================================
		 * GET DESCANSOS REGULADOS AÑO
		 *================================================================================*/
		/// <summary>
		/// Devuelve los descansos regulados del año dado.
		/// (No se incluyen los descansos generados en las regulaciones de final de año).
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static int GetDescansosReguladosAño(int idconductor, int año) {

			int resultado = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Descansos) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 0 AND Year(Fecha) = ?;";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos los descansos.
					object r = comando.ExecuteScalar();
					resultado = r == DBNull.Value ? 0 : (int)(double)r;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetDescansosReguladosAño", ex);
				}
			}

			// Devolvemos el resultado.
			return resultado;

		}

		//------------------------------------------------------------------------------------------------------------------------

		/*================================================================================
		 * GET DESCANSOS REGULADOS FIN AÑO
		 *================================================================================*/
		/// <summary>
		/// Devuelve los descansos generados en la regulación de final del año dado.
		/// </summary>
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static int GetDescansosReguladosFinAño(int idconductor, int año) {

			int resultado = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) " +
								"FROM Regulaciones " +
								"WHERE IdConductor = ? AND Codigo = 2" +
								"      Year(Fecha) = ?;";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();

					// Ejecutamos el comando y extraemos los descansos.
					object r = comando.ExecuteScalar();
					resultado = r == DBNull.Value ? 0 : (int)(double)r;
				} catch (Exception ex) {
					Utils.VerError("BdRgulacionConductor.GetDescansosReguladosFinAño", ex);
				}
			}

			// Devolvemos el resultado.
			return resultado;

		}



	} //Fin de clase.
}
