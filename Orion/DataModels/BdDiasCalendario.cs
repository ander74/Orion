#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.DataModels {

    public static class BdDiasCalendario {


        /*================================================================================
		 * GET DIAS CALENDARIO
         * Ok
		 *================================================================================*/
        //public static ObservableCollection<DiaCalendario> GetDiasCalendario(int idcalendario) {

        //    // Creamos la lista y el comando que extrae los gráficos.
        //    ObservableCollection<DiaCalendario> lista = new ObservableCollection<DiaCalendario>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        //string comandoSQL = "SELECT * FROM DiasCalendario WHERE IdCalendario = ? ORDER BY Dia";
        //        string comandoSQL = DbService2.GetDiasCalendario;

        //        // Elementos para la consulta de calendarios y días de calendario.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("id", idcalendario);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();
        //            // Extraemos los calendarios.
        //            lector = comando.ExecuteReader();

        //            // Por cada dia extraido...
        //            while (lector.Read()) {
        //                DiaCalendario dia = new DiaCalendario(lector);
        //                // Añadimos el calendario a la lista.
        //                lista.Add(dia);
        //                dia.Nuevo = false;
        //                dia.Modificado = false;
        //            }

        //        } catch (OleDbException ex) {
        //            Utils.VerError("BdDiasCalendarios.GetDiasCalendario(idcalendario)", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    // Devolvemos la lista.
        //    return lista;
        //}


        //public static List<DiaCalendario> GetDiasCalendario() {

        //    // Creamos la lista y el comando que extrae los gráficos.
        //    List<DiaCalendario> lista = new List<DiaCalendario>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        string comandoSQL = "SELECT * FROM DiasCalendario";
        //        //string comandoSQL = DbService2.GetDiasCalendario;

        //        // Elementos para la consulta de calendarios y días de calendario.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        //comando.Parameters.AddWithValue("id", idcalendario);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();
        //            // Extraemos los calendarios.
        //            lector = comando.ExecuteReader();

        //            // Por cada dia extraido...
        //            while (lector.Read()) {
        //                DiaCalendario dia = new DiaCalendario(lector);
        //                // Añadimos el calendario a la lista.
        //                lista.Add(dia);
        //                dia.Nuevo = false;
        //                dia.Modificado = false;
        //            }

        //        } catch (OleDbException ex) {
        //            Utils.VerError("BdDiasCalendarios.GetDiasCalendario(idcalendario)", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    // Devolvemos la lista.
        //    return lista;
        //}


        /*================================================================================
		 * GET DIA CALENDARIO
         * Ok
		 *================================================================================*/
        //public static DiaCalendarioBase GetDiaCalendario(int idConductor, DateTime fecha) {

        //    return App.Global.Repository.GetDiaCalendario(idConductor, fecha);

        //    DiaCalendarioBase resultado = null;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        string comandoSQL = "SELECT * FROM DiasCalendario WHERE IdCalendario IN (SELECT Id " +
        //                                                                                "FROM Calendarios " +
        //                                                                                "WHERE IdConductor = ?) " +
        //                            "AND DiaFecha = ?;";

        //        // Elementos para la consulta de calendarios y días de calendario.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("@IdConductor", idConductor);
        //        comando.Parameters.AddWithValue("@Validez", fecha.ToString("yyyy-MM-dd"));
        //        OleDbDataReader lector = null;
        //        try {
        //            conexion.Open();
        //            lector = comando.ExecuteReader();
        //            if (lector.Read()) resultado = new DiaCalendarioBase(lector);
        //        } catch (OleDbException ex) {
        //            Utils.VerError("BdDiasCalendarios.GetDiaCalendario", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    return resultado;
        //}


        /*================================================================================
		* GUARDAR DIAS CALENDARIO
        * Ok
		*================================================================================*/
        //public static void GuardarDiasCalendario(IEnumerable<DiaCalendario> lista) {

        //    // Si la lista está vacía, salimos.
        //    if (lista == null || lista.Count() == 0) return;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        string SQLInsertar = "INSERT INTO DiasCalendario (IdCalendario, Dia, DiaFecha, Grafico, Codigo, ExcesoJornada, FacturadoPaqueteria, " +
        //                             "Limpieza, GraficoVinculado, Notas, TurnoAlt, InicioAlt, FinalAlt, InicioPartidoAlt, FinalPartidoAlt, " +
        //                             "TrabajadasAlt, AcumuladasAlt, NocturnasAlt, DesayunoAlt, ComidaAlt, CenaAlt, PlusCenaAlt, " +
        //                             "PlusLimpiezaAlt, PlusPaqueteriaAlt) " +
        //                             "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
        //        //string SQLInsertar = "InsertarDiaCalendario";

        //        string SQLActualizar = "UPDATE DiasCalendario SET IdCalendario = ?, Dia = ?, DiaFecha = ?, Grafico = ?, Codigo = ?, " +
        //                               "ExcesoJornada = ?, FacturadoPaqueteria = ?, Limpieza = ?, GraficoVinculado = ?, Notas = ?, " +
        //                               "TurnoAlt=?, InicioAlt=?, FinalAlt=?, InicioPartidoAlt=?, FinalPartidoAlt=?, TrabajadasAlt=?, AcumuladasAlt=?, " +
        //                               "NocturnasAlt=?, DesayunoAlt=?, ComidaAlt=?, CenaAlt=?, PlusCenaAlt=?, PlusLimpiezaAlt=?, PlusPaqueteriaAlt=? " +
        //                               "WHERE Id=?;";
        //        //string SQLActualizar = "ActualizarDiaCalendario";

        //        string SQLGetId = "SELECT @@IDENTITY;";

        //        try {
        //            conexion.Open();
        //            //foreach (DiaCalendario dia in lista) {
        //            lista.ToList().ForEach(dia => {
        //                // Si el día tiene como gráfico cero y no existe, no se evalúa.
        //                //if (dia.Id == 0 && dia.Grafico == 0) continue;
        //                if (dia.Nuevo) {
        //                    OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
        //                    DiaCalendario.ParseToCommand(comando, dia);
        //                    int xx = comando.ExecuteNonQuery();
        //                    comando.CommandText = SQLGetId;
        //                    comando.CommandType = System.Data.CommandType.Text;
        //                    int iddia = (int)comando.ExecuteScalar();
        //                    dia.Id = iddia;
        //                    dia.Nuevo = false;
        //                    dia.Modificado = false;
        //                } else if (dia.Modificado) {
        //                    OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
        //                    DiaCalendario.ParseToCommand(comando, dia);
        //                    int x = comando.ExecuteNonQuery();
        //                    dia.Modificado = false;
        //                }
        //                //}
        //            });
        //        } catch (OleDbException ex) {
        //            Utils.VerError("BdDiasCalendario.GuardarDiasCalendario", ex);
        //        }
        //    }
        //}


        /*================================================================================
		 * GET HORAS DESCUADRE HASTA MES
         * Ok
		 *================================================================================*/
        //public static int GetHorasDescuadreHastaMes(int año, int mes, int idconductor) {

        //    object resultado = null;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        // Definimos el comando SQL.
        //        string comandoSQL = "SELECT Sum(Descuadre) " +
        //                            "FROM DiasCalendario " +
        //                            "WHERE BloquearDescuadre = False AND IdCalendario IN (SELECT Id " +
        //                                                                "FROM Calendarios " +
        //                                                                "WHERE IdConductor = ? AND Fecha < ?);";

        //        // Establecemos la fecha del día 1 del siguiente mes al indicado.
        //        DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

        //        // Creamos el comando y añadimos los parámetros.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("idconductor", idconductor);
        //        comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

        //        try {
        //            conexion.Open();
        //            // Ejecutamos el comando y guardamos el resultado.
        //            resultado = comando.ExecuteScalar();
        //        } catch (Exception ex) {
        //            Utils.VerError("BdCalendarios.GetHorasDescuadreHastaMes", ex);
        //        }
        //    }
        //    // Devolvemos el resultado.
        //    if (resultado == DBNull.Value) resultado = 0;
        //    return Convert.ToInt32(resultado);

        //}


        /*================================================================================
		 * GET EXCESO JORNADA HASTA MES
         * Ok
		 *================================================================================*/
        //public static TimeSpan GetExcesoJornadaHastaMes(int año, int mes, int idconductor) {

        //    object resultado = null;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        // Definimos el comando SQL.
        //        string comandoSQL = "SELECT Sum(ExcesoJornada) " +
        //                            "FROM DiasCalendario " +
        //                            "WHERE BloquearExcesoJornada <> True AND IdCalendario IN (SELECT Id " +
        //                                                                    "FROM Calendarios " +
        //                                                                    "WHERE IdConductor = ? AND Fecha < ?);";

        //        // Establecemos la fecha del día 1 del siguiente mes al indicado.
        //        DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

        //        // Creamos el comando y añadimos los parámetros.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("idconductor", idconductor);
        //        comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

        //        try {
        //            conexion.Open();
        //            // Ejecutamos el comando y guardamos el resultado.
        //            resultado = comando.ExecuteScalar();
        //        } catch (Exception ex) {
        //            Utils.VerError("BdCalendarios.GetExcesoJornadaHastaMes", ex);
        //        }
        //    }
        //    // Devolvemos el resultado.
        //    if (resultado == DBNull.Value) resultado = 0d;
        //    return new TimeSpan(Convert.ToInt64(resultado));

        //}


        /*================================================================================
		 * GET DIAS CALENDARIO CON BLOQUEOS
         * Ok
		 *================================================================================*/
        //public static ObservableCollection<DiaCalendario> GetDiasCalendarioConBloqueos(long idConductor) {

        //    // Creamos la lista y el comando que extrae los gráficos.
        //    ObservableCollection<DiaCalendario> lista = new ObservableCollection<DiaCalendario>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
        //        //string comandoSQL = "SELECT * " +
        //        //					"FROM DiasCalendario " +
        //        //					"WHERE (ExcesoJornada <> 0 OR Descuadre <> 0) AND IdCalendario IN (SELECT Id " +
        //        //					"																	FROM Calendarios " +
        //        //					"																	WHERE IdConductor = ?)";
        //        string comandoSQL = DbService2.GetDiasCalendarioConBloqueos;
        //        // Elementos para la consulta de calendarios y días de calendario.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("@IdConductor", idConductor);
        //        OleDbDataReader lector = null;
        //        try {
        //            conexion.Open();
        //            // Extraemos los calendarios.
        //            lector = comando.ExecuteReader();
        //            // Por cada dia extraido...
        //            while (lector.Read()) {
        //                DiaCalendario dia = new DiaCalendario(lector);
        //                // Añadimos el calendario a la lista.
        //                lista.Add(dia);
        //                dia.Nuevo = false;
        //                dia.Modificado = false;
        //            }
        //        } catch (OleDbException ex) {
        //            Utils.VerError("BdDiasCalendarios.GetDiasCalendarioConBloqueos", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    // Devolvemos la lista.
        //    return lista;

        //}


        /*================================================================================
		 * GET EXCESO JORNADA PENDIENTE HASTA MES
         * Ok
		 *================================================================================*/
        //public static TimeSpan GetExcesoJornadaPendienteHastaMes(int año, int mes, int idconductor) {

        //    object resultado = null;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        // Definimos el comando SQL.
        //        string comandoSQL = "SELECT Sum(ExcesoJornada) " +
        //                            "FROM DiasCalendario " +
        //                            "WHERE BloquearExcesoJornada = True AND IdCalendario IN (SELECT Id " +
        //                                                                    "FROM Calendarios " +
        //                                                                    "WHERE IdConductor = ? AND Fecha < ?);";

        //        // Establecemos la fecha del día 1 del siguiente mes al indicado.
        //        DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

        //        // Creamos el comando y añadimos los parámetros.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("idconductor", idconductor);
        //        comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

        //        try {
        //            conexion.Open();
        //            // Ejecutamos el comando y guardamos el resultado.
        //            resultado = comando.ExecuteScalar();
        //        } catch (Exception ex) {
        //            Utils.VerError("BdCalendarios.GetExcesoJornadaPendienteHastaMes", ex);
        //        }
        //    }
        //    // Devolvemos el resultado.
        //    if (resultado == DBNull.Value) resultado = 0d;
        //    return new TimeSpan(Convert.ToInt64(resultado));

        //}


        /*================================================================================
		 * GET DESCUADRE PENDIENTE HASTA MES
         * Ok
		 *================================================================================*/
        //public static int GetDescuadrePendienteHastaMes(int año, int mes, int idconductor) {

        //    object resultado = null;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        // Definimos el comando SQL.
        //        string comandoSQL = "SELECT Sum(Descuadre) " +
        //                            "FROM DiasCalendario " +
        //                            "WHERE BloquearDescuadre = True AND IdCalendario IN (SELECT Id " +
        //                                                                "FROM Calendarios " +
        //                                                                "WHERE IdConductor = ? AND Fecha < ?);";

        //        // Establecemos la fecha del día 1 del siguiente mes al indicado.
        //        DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);

        //        // Creamos el comando y añadimos los parámetros.
        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("idconductor", idconductor);
        //        comando.Parameters.AddWithValue("fecha", fecha.ToString("yyyy-MM-dd"));

        //        try {
        //            conexion.Open();
        //            // Ejecutamos el comando y guardamos el resultado.
        //            resultado = comando.ExecuteScalar();
        //        } catch (Exception ex) {
        //            Utils.VerError("BdCalendarios.GetDescuadrePendienteHastaMes", ex);
        //        }
        //    }
        //    // Devolvemos el resultado.
        //    if (resultado == DBNull.Value) resultado = 0;
        //    return Convert.ToInt32(resultado);

        //}



    }
}
