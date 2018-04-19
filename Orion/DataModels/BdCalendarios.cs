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

namespace Orion.DataModels {


	public static class BdCalendarios {

		/*================================================================================
		 * GET CALENDARIOS
		 *================================================================================*/
		public static ObservableCollection<Calendario> GetCalendarios(int año, int mes, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista y el comando que extrae los gráficos.
			ObservableCollection<Calendario> lista = new ObservableCollection<Calendario>();

			using (conexion) {

				// Creamos el comando SQL.
				//string comandoSQL = "SELECT Calendarios.*, Conductores.Indefinido " +
				//					"FROM Calendarios LEFT JOIN Conductores ON Calendarios.IdConductor = Conductores.Id " +
				//					"WHERE Year(Calendarios.Fecha) = ? AND Month(Calendarios.Fecha) = ? " +
				//					"ORDER BY Calendarios.IdConductor;";
				string comandoSQL = "GetCalendarios";

				// Elementos para la consulta de calendarios y días de calendario.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.CommandType = System.Data.CommandType.StoredProcedure;
				comando.Parameters.AddWithValue("año", año);
				comando.Parameters.AddWithValue("mes", mes);
				OleDbDataReader lector = null;

				try {
					// Extraemos los calendarios.
					conexion.Open();
					lector = comando.ExecuteReader();

					// Por cada calendario extraido...
					while (lector.Read()) {
						// Extraemos el calendario y sus días
						Calendario calendario = new Calendario(lector);
						calendario.ListaDias = BdDiasCalendario.GetDiasCalendario(calendario.Id);
						// Extraemos los datos del conductor.
						calendario.ConductorIndefinido = lector.ToBool("Indefinido");
						// Añadimos el calendario a la lista.
						lista.Add(calendario);
						calendario.Nuevo = false;
						calendario.Modificado = false;

					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetCalendarios", ex);
				}
			}
			// Devolvemos la lista.
			return lista;
		}


		/*================================================================================
 		 * GUARDAR CALENDARIOS
		 *================================================================================*/
		public static void GuardarCalendarios(ObservableCollection<Calendario> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			using (conexion) {

				// Creamos las instrucciones SQL.
				//string SQLInsertar = "INSERT INTO Calendarios (IdConductor, Fecha, HorasDescuadre, ExcesoJornadaCobrada, Notas) " +
				//					 "VALUES (?, ?, ?, ?, ?);";
				string SQLInsertar = "InsertarCalendario";

				//string SQLActualizar = "UPDATE Calendarios SET IdConductor=?, Fecha=?, HorasDescuadre=?, ExcesoJornadaCobrada=?, Notas=? WHERE Id=?;";
				string SQLActualizar = "ActualizarCalendario";

				string SQLGetId = "SELECT @@IDENTITY;";

				try {
					// Abrimos la conexion
					conexion.Open();

					// Recorremos todos los calendarios.
					foreach (Calendario calendario in lista) {
						if (calendario.Nuevo) {
							//if (!BdConductores.ExisteConductor(calendario.IdConductor)) {
							//	BdConductores.InsertarConductorDesconocido(calendario.IdConductor);
							//}
							if (!App.Global.ConductoresVM.ExisteConductor(calendario.IdConductor)) {
								App.Global.ConductoresVM.CrearConductorDesconocido(calendario.IdConductor);
							}
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;
							Calendario.ParseToCommand(comando, calendario);
							comando.ExecuteNonQuery();
							comando.CommandText = SQLGetId;
							comando.CommandType = System.Data.CommandType.Text;
							int idcalendario = (int)comando.ExecuteScalar();
							foreach (DiaCalendario dia in calendario.ListaDias) {
								dia.IdCalendario = idcalendario;
							}
							calendario.Id = idcalendario;
							calendario.Nuevo = false;
							calendario.Modificado = false;
						} else if (calendario.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;
							Calendario.ParseToCommand(comando, calendario);
							comando.ExecuteNonQuery();
							calendario.Modificado = false;
						}
						//Guardamos los días del calendario.
						BdDiasCalendario.GuardarDiasCalendario(calendario.ListaDias);
					}
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GuardarCalendarios", ex);
				}
			}
		}


		/*================================================================================
		 * BORRAR CALENDARIOS
		 *================================================================================*/
		public static void BorrarCalendarios(List<Calendario> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			using (conexion) {
				// Creamos el comando SQL
				//string SQLBorrar = "DELETE FROM Calendarios WHERE Id=?;";
				string SQLBorrar = "BorrarCalendario";

				try {
					conexion.Open();

					foreach (Calendario calendario in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.CommandType = System.Data.CommandType.StoredProcedure;
						comando.Parameters.AddWithValue("@Id", calendario.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.BorrarCalendarios", ex);
				}
			}
		}


		/*================================================================================
		 * GET DESCANSOS REGULADOS HASTA MES
		 *================================================================================*/
		public static int GetDescansosReguladosHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = ? AND Fecha < ?";

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

				try {

					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDescansosReguladosHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);

		}


		/*================================================================================
		 * GET DESCANSOS REGULADOS AÑO
		 *================================================================================*/
		public static int GetDescansosReguladosAño(int año, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = ? AND Year(Fecha) = ? AND Codigo = 2;";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDescansosReguladosAño", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);

		}


		/*================================================================================
		 * GET DESCANSOS DISFRUTADOS HASTA MES
		 *================================================================================*/
		public static int GetDescansosDisfrutadosHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
									"WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
									"      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
				comando.Parameters.AddWithValue("idconductor", idconductor);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDescansosDisfrutadosHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
		}


		/*================================================================================
		 * GET DC DISFRUTADOS AÑO
		 *================================================================================*/
		public static int GetDCDisfrutadosAño(int idconductor, int año, int mes=0, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
									"WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Year(Fecha) = ? AND IdConductor = ?)" +
									"      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";

				string comandoSQLconMes = "SELECT Count(Grafico) FROM DiasCalendario " +
										  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Year(Fecha) = ? AND Month(Fecha) <= ? AND IdConductor = ?)" +
										  "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = null;
				if (mes == 0) {
					comando = new OleDbCommand(comandoSQL, conexion);
					comando.Parameters.AddWithValue("año", año);
					comando.Parameters.AddWithValue("idconductor", idconductor);
				} else {
					comando = new OleDbCommand(comandoSQLconMes, conexion);
					comando.Parameters.AddWithValue("año", año);
					comando.Parameters.AddWithValue("mes", mes);
					comando.Parameters.AddWithValue("idconductor", idconductor);
				}

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDCDisfrutadosAño", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
		}


		/*================================================================================
		 * GET DIAS F6 HASTA MES
		 *================================================================================*/
		public static int GetDiasF6HastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
								"WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?)" +
								"      AND Grafico = -7; ";

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
				comando.Parameters.AddWithValue("idconductor", idconductor);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDiasF6HastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
		}


		/*================================================================================
		 * GET HORAS REGULADAS HASTA MES
		 *================================================================================*/
		public static TimeSpan GetHorasReguladasHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Fecha < ?";

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasReguladasHastaMes", ex);
				}
			} 
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan(Convert.ToInt64(resultado));

		}


		/*================================================================================
		 * GET HORAS REGULADAS MES
		 *================================================================================*/
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


		/*================================================================================
		 * GET HORAS CAMBIADAS POR DCs MES
		 *================================================================================*/
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


		/*================================================================================
		 * GET HORAS REGULADAS AÑO
		 *================================================================================*/
		public static TimeSpan GetHorasReguladasAño(int año, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = ? AND Year(Fecha) = ? AND Codigo = 2;";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("año", año);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetHorasReguladasAño", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan(Convert.ToInt64(resultado));

		}


		/*================================================================================
		 * GET HORAS COBRADAS MES
		 *================================================================================*/
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


		/*================================================================================
		 * GET HORAS COBRADAS AÑO
		 *================================================================================*/
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


		/*================================================================================
		 * GET EXCESO JORNADA COBRADA HASTA MES
		 *================================================================================*/
		public static TimeSpan GetExcesoJornadaCobradaHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Sum(ExcesoJornadaCobrada) FROM Calendarios WHERE IdConductor = ? AND Fecha < ?;";

				// Creamos la fecha del primer día del mes siguiente al mes y año dados.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
				
				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetExcesoJornadaHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			if (resultado == DBNull.Value) resultado = 0d;
			return new TimeSpan(Convert.ToInt64(resultado));

		}


		/*================================================================================
		 * GET ACUMULADAS HASTA MES
		 *================================================================================*/
		public static TimeSpan GetAcumuladasHastaMes(int año, int mes, int idconductor, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Inicializamos las horas acumuladas.
			TimeSpan acumuladas = new TimeSpan(0);

			using (conexion) {

				// Creamos el comando SQL para los días
				string comandoDias = "SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha " +
								 "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios.Id " +
								 "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Fecha < ? AND IdConductor = ?) " +
								 "      AND Grafico > 0 " +
								 "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;";

				string comandoSQL = "SELECT Acumuladas " +
									"FROM (SELECT * " +
									"      FROM Graficos" +
									"      WHERE IdGrupo = (SELECT Id " +
									"                       FROM GruposGraficos " +
									"                       WHERE Validez = (SELECT Max(Validez) " +
									"                                        FROM GruposGraficos " +
									"                                        WHERE Validez <= ?)))" +
									"WHERE Numero = ?";

				// Establecemos la fecha del día 1 del siguiente mes al indicado.
				DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

				// Leemos los días hasta el mes actual
				OleDbCommand comando = new OleDbCommand(comandoDias, conexion);
				comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));
				comando.Parameters.AddWithValue("idconductor", idconductor);

				try {
					conexion.Open();
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
						OleDbCommand comando2 = new OleDbCommand(comandoSQL, conexion);
						comando2.Parameters.AddWithValue("validez", fechadia.ToString("yyyy-MM-dd"));
						comando2.Parameters.AddWithValue("numero", g);
						object acum = comando2.ExecuteScalar();
						long t = 0;
						if (acum != null) {
							t = acum == DBNull.Value ? 0 : Convert.ToInt64(acum); ;
						}
						acumuladas += new TimeSpan(t);
					}
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetAcumuladasHastaMes", ex);
				}
			}

			// Devolvemos el resultado
			return acumuladas;
		}


		/*================================================================================
		 * GET COMITE EN DESCANSO HASTA MES
		 *================================================================================*/
		public static int GetComiteEnDescansoHastaMes(int idconductor, int año, int mes, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Inicializamos el resultado.
			int resultado = 0;
			// Establecemos la fecha del día 1 del siguiente mes al indicado.
			DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
									"FROM Calendarios INNER JOIN DiasCalendario " +
									"ON Calendarios.Id = DiasCalendario.IdCalendario " +
									"WHERE Calendarios.IdConductor = ? AND " +
									"      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5) AND " +
									"	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
									"      Fecha < ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();
					// Ejecutamos el comando y extraemos el gráfico.
					object res = comando.ExecuteScalar();
					resultado = Convert.ToInt32(res);
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetComiteEnDescansoHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado;

		}


		/*================================================================================
		 * GET TRABAJO EN DESCANSO HASTA MES
		 *================================================================================*/
		public static int GetTrabajoEnDescansoHastaMes(int idconductor, int año, int mes, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Inicializamos el resultado.
			int resultado = 0;
			// Establecemos la fecha del día 1 del siguiente mes al indicado.
			DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
									"FROM Calendarios INNER JOIN DiasCalendario " +
									"ON Calendarios.Id = DiasCalendario.IdCalendario " +
									"WHERE Calendarios.IdConductor = ? AND " +
									"      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND Fecha < ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();
					// Ejecutamos el comando y extraemos el gráfico.
					object res = comando.ExecuteScalar();
					resultado = Convert.ToInt32(res);
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetComiteEnDescansoHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado;

		}


		/*================================================================================
		 * GET DND HASTA MES
		 *================================================================================*/
		public static int GetDNDHastaMes(int idconductor, int año, int mes, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Inicializamos el resultado.
			int resultado = 0;
			// Establecemos la fecha del día 1 del siguiente mes al indicado.
			DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
									"FROM Calendarios INNER JOIN DiasCalendario " +
									"ON Calendarios.Id = DiasCalendario.IdCalendario " +
									"WHERE Calendarios.IdConductor = ? AND " +
									"      DiasCalendario.Grafico = -8 AND " +
									"      Fecha < ?";

				// Creamos el comando que extrae el gráfico correspondiente.
				OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
				comando.Parameters.AddWithValue("idconductor", idconductor);
				comando.Parameters.AddWithValue("fecha", fecha);

				try {
					conexion.Open();
					// Ejecutamos el comando y extraemos el gráfico.
					object res = comando.ExecuteScalar();
					resultado = Convert.ToInt32(res);
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDNDHastaMes", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado;

		}


		/*================================================================================
		 * GET DND DISFRUTADOS AÑO
		 *================================================================================*/
		public static int GetDNDDisfrutadosAño(int idconductor, int año, int mes = 0, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			object resultado = null;

			using (conexion) {

				// Definimos el comando SQL.
				string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
									"WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Year(Fecha) = ? AND IdConductor = ?)" +
									"      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";

				string comandoSQLconMes = "SELECT Count(Grafico) FROM DiasCalendario " +
										  "WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE Year(Fecha) = ? AND Month(Fecha) <= ? AND IdConductor = ?)" +
										  "      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";

				// Creamos el comando y añadimos los parámetros.
				OleDbCommand comando = null;
				if (mes == 0) {
					comando = new OleDbCommand(comandoSQL, conexion);
					comando.Parameters.AddWithValue("año", año);
					comando.Parameters.AddWithValue("idconductor", idconductor);
				} else {
					comando = new OleDbCommand(comandoSQLconMes, conexion);
					comando.Parameters.AddWithValue("año", año);
					comando.Parameters.AddWithValue("mes", mes);
					comando.Parameters.AddWithValue("idconductor", idconductor);
				}

				try {
					conexion.Open();
					// Ejecutamos el comando y guardamos el resultado.
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdCalendarios.GetDCDisfrutadosAño", ex);
				}
			}
			// Devolvemos el resultado.
			return resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
		}









	}
}
