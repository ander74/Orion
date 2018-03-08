#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;

namespace Orion.Pijama {

	public static class BdPijamas {


		// ====================================================================================================
		#region CONSULTAS SQL
		// ====================================================================================================

		private static string comandoGetGrafico = "SELECT * " +
												  "FROM (SELECT * " +
												  "      FROM Graficos" +
												  "      WHERE IdGrupo = (SELECT Id " +
												  "                       FROM GruposGraficos " +
												  "                       WHERE Validez = (SELECT Max(Validez) " +
												  "                                        FROM GruposGraficos " +
												  "                                        WHERE Validez <= ?)))" +
												  "WHERE Numero = ?";


		private static string comandoDiasCalendario = "SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha " +
													  "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios.Id " +
													  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?) " +
													  "      AND Grafico > 0 " +
													  "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;";


		private static string comandoAcumuladas = "SELECT Acumuladas " +
												  "FROM (SELECT * FROM Graficos WHERE IdGrupo = (SELECT Id " +
												  "										         FROM GruposGraficos " +
												  "												 WHERE Validez = (SELECT Max(Validez) " +
												  "																  FROM GruposGraficos " +
												  "														          WHERE Validez <= ?)))" +
												  "WHERE Numero = ?";


		private static string comandoReguladas = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Fecha < ?";


		private static string comandoDiasF6 = "SELECT Count(Grafico) FROM DiasCalendario " +
											  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
											  "      AND Grafico = -7; ";


		private static string comandoDCsRegulados = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = ? AND Fecha < ?";


		private static string comandoDCsDisfrutados = "SELECT Count(Grafico) FROM DiasCalendario " +
													  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
													  "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";


		private static string comandoDNDsDisfrutados = "SELECT Count(DiasCalendario.Grafico) " +
													   "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
													   "WHERE Calendarios.IdConductor = ? AND DiasCalendario.Grafico = -8 AND Fecha < ?";


		private static string comandoDiasComiteEnJD = "SELECT Count(DiasCalendario.Grafico) " +
													  "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
													  "WHERE Calendarios.IdConductor = ? AND " +
													  "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5) AND " +
													  "	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
													  "      Fecha < ?";


		private static string comandoDiasTrabajoEnJD = "SELECT Count(DiasCalendario.Grafico) " +
													   "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
													   "WHERE Calendarios.IdConductor = ? AND " +
													   "      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND Fecha < ?";








		#endregion
		// ====================================================================================================


		//================================================================================
 		// GET DÍAS PIJAMA
 		//================================================================================
		public static List<DiaPijama> GetDiasPijama(IEnumerable<DiaCalendarioBase> listadias, OleDbConnection conexion = null) {

			// Si no se pasa una conexión, se establece la conexion del centro actual.
			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
			// Creamos la lista que se devolverá.
			List<DiaPijama> lista = new List<DiaPijama>();
			using (conexion) {
				// Habrá que quitar el control de excepciones aquí y ponerselo en la llamada al método, ya que aquí no hay
				// gestión de ventanas (o no debería haberlo).
				try {
					conexion.Open();
					foreach (DiaCalendarioBase dia in listadias) {
						// Creamos el día pijama a añadir a la lista.
						DiaPijama diaPijama = new DiaPijama(dia);
						// Establecemos el gráfico a buscar, por si está seleccionado el comodín.
						int GraficoBusqueda = dia.Grafico;
						if (dia.GraficoVinculado != 0 && dia.Grafico == App.Global.PorCentro.Comodin) GraficoBusqueda = dia.GraficoVinculado;
						// Creamos el comando SQL y añadimos los parámetros
						OleDbCommand comando = new OleDbCommand(comandoGetGrafico, conexion);
						comando.Parameters.AddWithValue("@Validez", dia.DiaFecha.ToString("yyyy-MM-dd"));
						comando.Parameters.AddWithValue("@Numero", GraficoBusqueda);
						// Ejecutamos el comando y extraemos el gráfico.
						OleDbDataReader lector = comando.ExecuteReader();
						if (lector.Read()) {
							diaPijama.GraficoTrabajado = new GraficoBase(lector);
						} else {
							diaPijama.GraficoTrabajado = new GraficoBase();
						}
						// Añadimos el día pijama a la lista.
						lista.Add(diaPijama);
					}
				} catch (Exception ex) {
					Utils.VerError("BdPijamas.GetDiasPijama", ex);
				}
			}
			return lista;
		}


		//================================================================================
		// GET RESUMEN HASTA MES
		//================================================================================
		public static ResumenPijama GetResumenHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Inicializamos las horas acumuladas.
			ResumenPijama resultado = new ResumenPijama();

			using (conexion) {

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
				// Establecemos un objeto a usar para los resultados.
				object objeto = null;

				try {
					conexion.Open();

					//----------------------------------------------------------------------------------------------------
					// HORAS ACUMULADAS
					//----------------------------------------------------------------------------------------------------
					OleDbCommand comando = new OleDbCommand(comandoDiasCalendario, conexion);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					comando.Parameters.AddWithValue("idconductor", idconductor);
					OleDbDataReader lector = comando.ExecuteReader();

					// Por cada día, sumamos las horas acumuladas.
					while (lector.Read()) {
						int d = (lector["Dia"] is DBNull) ? 0 : (Int16)lector["Dia"];
						int g = (lector["Grafico"] is DBNull) ? 0 : (Int16)lector["Grafico"];
						int v = (lector["GraficoVinculado"] is DBNull) ? 0 : (Int16)lector["GraficoVinculado"];
						if (v != 0 && g == App.Global.PorCentro.Comodin) g = v;
						DateTime f = (lector["Fecha"] is DBNull) ? new DateTime(0) : (DateTime)lector["Fecha"];
						if (d > DateTime.DaysInMonth(f.Year, f.Month)) continue;
						DateTime fechadia = new DateTime(f.Year, f.Month, d);
						OleDbCommand comando2 = new OleDbCommand(comandoAcumuladas, conexion);
						comando2.Parameters.AddWithValue("validez", fechadia.ToString("yyyy-MM-dd"));
						comando2.Parameters.AddWithValue("numero", g);
						objeto = comando2.ExecuteScalar();
						long t = 0;
						if (objeto != null) {
							t = objeto == DBNull.Value ? 0 : Convert.ToInt64(objeto); ;
						}
						resultado.HorasAcumuladas += new TimeSpan(t);
					}
					lector.Close();
					//----------------------------------------------------------------------------------------------------
					// HORAS REGULADAS
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoReguladas, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.HorasReguladas = new TimeSpan(Convert.ToInt64(objeto));
					//----------------------------------------------------------------------------------------------------
					// DIAS F6
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDiasF6, conexion);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					comando.Parameters.AddWithValue("idconductor", idconductor);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DiasLibreDisposicionF6 = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// DCS REGULADOS 
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDCsRegulados, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DCsRegulados = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// DCS DISFRUTADOS
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDCsDisfrutados, conexion);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					comando.Parameters.AddWithValue("idconductor", idconductor);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DCsDisfrutados = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// DNDS DISFRUTADOS
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDNDsDisfrutados, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DNDsDisfrutados = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// DÍAS COMITÉ EN DESCANSO
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDiasComiteEnJD, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DiasComiteEnDescanso = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// DÍAS TRABAJO EN DESCANSO
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDiasTrabajoEnJD, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DiasTrabajoEnDescanso = Convert.ToInt32(objeto);
					//----------------------------------------------------------------------------------------------------
					// FINAL
					//----------------------------------------------------------------------------------------------------
				} catch (Exception ex) {
					Utils.VerError("BdPijamas.GetResumenHastaMes", ex);
				}
			}

			// Devolvemos el resultado
			return resultado;
		}



		//================================================================================
		// GET HORAS COBRADAS MES
		//================================================================================
		public static TimeSpan GetHorasCobradasMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Year(Fecha) = ? AND Month(Fecha) = ? AND Codigo = 1;";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasCobradasMes", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan(Convert.ToInt64(resultado));

		}


		//================================================================================
		// GET HORAS COBRADAS AÑO
		//================================================================================
		public static TimeSpan GetHorasCobradasAño(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Fecha > ? AND Fecha < ? AND Codigo = 1;";

				// Definimos las fechas de inicio y final
				DateTime fechainicio = mes == 12 ? new DateTime(año, 11, 30) : new DateTime(año - 1, 11, 30);
				DateTime fechafinal = mes == 12 ? new DateTime(año + 1, 12, 1) : new DateTime(año, 12, 1);

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fechainicio", fechainicio);
				comando.Parameters.AddWithValue("fechafinal", fechafinal);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasCobradasAño", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan(Convert.ToInt64(resultado));

		}


		//================================================================================
		// GET HORAS CAMBIADAS POR DCs MES
		//================================================================================
		public static TimeSpan GetHorasCambiadasPorDCsMes(int año, int mes, int idconductor) {

			object resultado = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Year(Fecha) = ? AND Month(Fecha) = ? AND Codigo = 2;";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasReguladasMes", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan((long)(double)resultado);

		}


		//================================================================================
		// GET HORAS REGULADAS MES
		//================================================================================
		public static TimeSpan GetHorasReguladasMes(int año, int mes, int idconductor) {

			object resultado = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Year(Fecha) = ? AND Month(Fecha) = ? AND Codigo = 0;";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasReguladasMes", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan((long)(double)resultado);

		}






	}
}
