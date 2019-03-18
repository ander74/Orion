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
using System.Linq;

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


		/// <summary>
		/// Este comando recoge todos los gráficos que pertenecen al calendario indicado en @IdCalendario, cogiendo cada gráfico
		/// del grupo correspondiente. Falta resolver cómo se captura el gráfico comodín. En el futuro (Orion 2) el gráfico comodín
		/// no será necesario ya que se permitirá editar cada día de calendario individualmente.
		/// Finalmente no se usa, ya que es más lento que el método anterior (primera versión).
		/// </summary>
		private static string comandoGetGrafico2 = "SELECT DC.DiaFecha, G.* " +
												   "FROM DiasCalendario DC LEFT JOIN Graficos G " +
												   "					   ON DC.Grafico = G.Numero " +
												   "WHERE DC.IdCalendario = @IdCaledario AND " +
												   "	  G.IdGrupo = (SELECT Id " +
												   "		           FROM GruposGraficos GG " +
												   "		           WHERE GG.Validez = (SELECT Max(GG.Validez) " +
												   "		                               FROM GruposGraficos GG " +
												   "		                               WHERE GG.Validez <= DC.DiaFecha)) " +
												   "ORDER BY DC.DiaFecha, G.Numero";


		private static string comandoDiasCalendario = "SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha, " +
													  "DiasCalendario.ExcesoJornada " +
													  "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios.Id " +
													  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?) " +
													  "      AND Grafico > 0 " +
													  "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;";


		private static string comandoAcumuladas = "SELECT * " +
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


		private static string comandoDiasF6DC = "SELECT Count(Grafico) FROM DiasCalendario " +
											  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
											  "      AND Grafico = -14; ";


		private static string comandoDCsRegulados = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = ? AND Fecha < ?";


		private static string comandoDCsDisfrutados = "SELECT Count(Grafico) FROM DiasCalendario " +
													  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
													  "      AND Grafico = -6;";// AND (Codigo = 0 OR Codigo Is Null);";


		private static string comandoDNDsDisfrutados = "SELECT Count(DiasCalendario.Grafico) " +
													   "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
													   "WHERE Calendarios.IdConductor = ? AND DiasCalendario.Grafico = -8 AND Fecha < ?";


		private static string comandoDiasComiteEnJD = "SELECT Count(DiasCalendario.Grafico) " +
													  "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
													  "WHERE Calendarios.IdConductor = ? AND " +
													  "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5 OR " +
													  "       DiasCalendario.Grafico = -6 OR DiasCalendario.Grafico = -1) AND " +
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
		public static List<DiaPijama> GetDiasPijama(IEnumerable<DiaCalendarioBase> listadias) {

			// Creamos la lista que se devolverá.
			List<DiaPijama> lista = new List<DiaPijama>();
			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
				// Habrá que quitar el control de excepciones aquí y ponerselo en la llamada al método, ya que aquí no hay
				// gestión de ventanas (o no debería haberlo).
				try {
					conexion.Open();
					foreach (DiaCalendarioBase dia in listadias) {
						// Creamos el día pijama a añadir a la lista.
						DiaPijama diaPijama = new DiaPijama(dia);
						// Si el día no pertenece al mes, continuamos el bucle al siguiente día.
						if (dia.Dia > DateTime.DaysInMonth(dia.DiaFecha.Year, dia.DiaFecha.Month)) continue;
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
						// Modificamos los parámetros del gráfico trabajado en función de si existen en el DiaCalendarioBase.
						if (dia.TurnoAlt.HasValue) diaPijama.GraficoTrabajado.Turno = dia.TurnoAlt.Value;
						if (dia.InicioAlt.HasValue) diaPijama.GraficoTrabajado.Inicio = new TimeSpan(dia.InicioAlt.Value.Ticks);
						if (dia.FinalAlt.HasValue) diaPijama.GraficoTrabajado.Final = new TimeSpan(dia.FinalAlt.Value.Ticks);
						if (dia.InicioPartidoAlt.HasValue) diaPijama.GraficoTrabajado.InicioPartido = new TimeSpan(dia.InicioPartidoAlt.Value.Ticks);
						if (dia.FinalPartidoAlt.HasValue) diaPijama.GraficoTrabajado.FinalPartido = new TimeSpan(dia.FinalPartidoAlt.Value.Ticks);
						if (dia.TrabajadasAlt.HasValue) diaPijama.GraficoTrabajado.Trabajadas = new TimeSpan(dia.TrabajadasAlt.Value.Ticks);
						if (dia.AcumuladasAlt.HasValue) diaPijama.GraficoTrabajado.Acumuladas = new TimeSpan(dia.AcumuladasAlt.Value.Ticks);
						if (dia.NocturnasAlt.HasValue) diaPijama.GraficoTrabajado.Nocturnas = new TimeSpan(dia.NocturnasAlt.Value.Ticks);
						if (dia.DesayunoAlt.HasValue) diaPijama.GraficoTrabajado.Desayuno = dia.DesayunoAlt.Value;
						if (dia.ComidaAlt.HasValue) diaPijama.GraficoTrabajado.Comida = dia.ComidaAlt.Value;
						if (dia.CenaAlt.HasValue) diaPijama.GraficoTrabajado.Cena = dia.CenaAlt.Value;
						if (dia.PlusCenaAlt.HasValue) diaPijama.GraficoTrabajado.PlusCena = dia.PlusCenaAlt.Value;
						if (dia.PlusLimpiezaAlt.HasValue) diaPijama.GraficoTrabajado.PlusLimpieza = dia.PlusLimpiezaAlt.Value;
						if (dia.PlusPaqueteriaAlt.HasValue) diaPijama.GraficoTrabajado.PlusPaqueteria = dia.PlusPaqueteriaAlt.Value;
						// Añadimos el día pijama a la lista.
						lista.Add(diaPijama);
						// Cerramos el lector.
						lector.Close();
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
		public static ResumenPijama GetResumenHastaMes(int año, int mes, int idconductor) {

			// Inicializamos las horas acumuladas.
			ResumenPijama resultado = new ResumenPijama();

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

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
						TimeSpan ej = lector.ToTimeSpan("ExcesoJornada");
						if (v != 0 && g == App.Global.PorCentro.Comodin) g = v;
						DateTime f = (lector["Fecha"] is DBNull) ? new DateTime(0) : (DateTime)lector["Fecha"];
						if (d > DateTime.DaysInMonth(f.Year, f.Month)) continue;
						DateTime fechadia = new DateTime(f.Year, f.Month, d);
						OleDbCommand comando2 = new OleDbCommand(comandoAcumuladas, conexion);
						comando2.Parameters.AddWithValue("validez", fechadia.ToString("yyyy-MM-dd"));
						comando2.Parameters.AddWithValue("numero", g);
						OleDbDataReader lector2 = comando2.ExecuteReader();

						GraficoBase grafico = null;
						if (lector2.Read()) {
							grafico = new GraficoBase(lector2);
							if (ej != TimeSpan.Zero) {
								if (grafico != null) grafico.Final += ej;
							}
							resultado.HorasAcumuladas += grafico.Acumuladas;
						}
						lector2.Close();
					}
					lector.Close();
					//----------------------------------------------------------------------------------------------------
					// HORAS REGULADAS
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoReguladas, conexion);
					comando.Parameters.AddWithValue("idconductor", idconductor);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value)
					{
						objeto = 0d;
					}
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
					// DIAS F6DC
					//----------------------------------------------------------------------------------------------------
					comando = new OleDbCommand(comandoDiasF6DC, conexion);
					comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
					comando.Parameters.AddWithValue("idconductor", idconductor);
					objeto = comando.ExecuteScalar();
					if (objeto == DBNull.Value) objeto = 0d;
					resultado.DiasF6DC = Convert.ToInt32(objeto);
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
		public static TimeSpan GetHorasCobradasMes(int año, int mes, int idconductor) {

			object resultado = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

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
		public static TimeSpan GetHorasCobradasAño(int año, int mes, int idconductor) {

			object resultado = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

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


		//================================================================================
		// GET GRAFICO
		//================================================================================
		public static GraficoBase GetGrafico(int numero, DateTime validez) {

			GraficoBase grafico = null;
			OleDbDataReader lector = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

				try {
					conexion.Open();
					OleDbCommand comando = new OleDbCommand(comandoGetGrafico, conexion);
					comando.Parameters.AddWithValue("@Validez", validez.ToString("yyyy-MM-dd"));
					comando.Parameters.AddWithValue("@Numero", numero);
					// Ejecutamos el comando y extraemos el gráfico.
					lector = comando.ExecuteReader();
					if (lector.Read()) {
						grafico = new GraficoBase(lector);
					}
				} catch (Exception ex) {
					Utils.VerError("BdPijamas.GetGrafico", ex);
				} finally {
					lector?.Close();
				}
			}
			return grafico;
		}




	}
}
