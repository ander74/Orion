#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Orion.Models;

namespace Orion.DataModels {


    public class BdEstadisticas {


        /*================================================================================
        * GET ESTADÍSTICAS ÚLTIMO GRUPO GRÁFICOS
        * Ok
        *================================================================================*/
        public static EstadisticaGrupoGraficos GetEstadisticasUltimoGrupoGraficos(Centros centro) {

            return App.Global.Repository.GetEstadisticasUltimoGrupoGraficos(centro);

            EstadisticaGrupoGraficos resultado = null;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.GetCadenaConexion(centro))) {

                string comandoSQL = "SELECT  GG.Validez, Count(G.Id) as Cantidad, Count(IIf(G.Turno = 1, 1, null)) as Turnos1, " +
                                    "        Count(IIf(G.Turno = 2, 1, null)) as Turnos2, Count(IIf(G.Turno = 3, 1, null)) as Turnos3, " +
                                    "        Count(IIf(G.Turno = 4, 1, null)) as Turnos4, Sum(G.Valoracion) As Valoraciones, Sum(G.Trabajadas) As H_Trabajadas, " +
                                    "        Sum(IIf(G.Turno = 1, G.Trabajadas, 0)) As TrabajadasTurno1, Sum(IIf(G.Turno = 2, G.Trabajadas, 0)) As TrabajadasTurno2, " +
                                    "        Sum(IIf(G.Turno = 3, G.Trabajadas, 0)) As TrabajadasTurno3, Sum(IIf(G.Turno = 4, G.Trabajadas, 0)) As TrabajadasTurno4, " +
                                    "        Sum(G.Acumuladas) As H_Acumuladas, Sum(G.Nocturnas) As H_Nocturnas, Sum(G.Desayuno) As Desayunos, " +
                                    "        Sum(G.Comida) As Comidas, Sum(G.Cena) As Cenas, Sum(G.PlusCena) As PlusesCena " +
                                    "FROM Graficos G LEFT JOIN GruposGraficos GG ON G.IdGrupo = GG.Id " +
                                    "WHERE GG.Validez = (SELECT Max(Validez) FROM GruposGraficos)" +
                                    "GROUP BY GG.Validez " +
                                    "ORDER BY GG.Validez";

                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                conexion.Open();
                OleDbDataReader lector = Comando.ExecuteReader();
                if (lector.Read()) resultado = new EstadisticaGrupoGraficos(lector);
                lector.Close();
            }
            return resultado;
        }


        /*================================================================================
        * GET ESTADÍSTICAS GRUPO GRÁFICOS
        * Ok
        *================================================================================*/
        [Obsolete("Ya no hay grupos de gráficos.")]
        public static EstadisticaGrupoGraficos GetEstadisticasGrupoGraficos(long idGrupo, Centros centro) {

            //return App.Global.Repository.GetEstadisticasGrupoGraficos(idGrupo, centro);
            return null;

            EstadisticaGrupoGraficos resultado = null;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.GetCadenaConexion(centro))) {

                string comandoSQL = "SELECT  GG.Validez, Count(G.Id) as Cantidad, Count(IIf(G.Turno = 1, 1, null)) as Turnos1, " +
                                    "        Count(IIf(G.Turno = 2, 1, null)) as Turnos2, Count(IIf(G.Turno = 3, 1, null)) as Turnos3, " +
                                    "        Count(IIf(G.Turno = 4, 1, null)) as Turnos4, Sum(G.Valoracion) As Valoraciones, Sum(G.Trabajadas) As H_Trabajadas, " +
                                    "        Sum(IIf(G.Turno = 1, G.Trabajadas, 0)) As TrabajadasTurno1, Sum(IIf(G.Turno = 2, G.Trabajadas, 0)) As TrabajadasTurno2, " +
                                    "        Sum(IIf(G.Turno = 3, G.Trabajadas, 0)) As TrabajadasTurno3, Sum(IIf(G.Turno = 4, G.Trabajadas, 0)) As TrabajadasTurno4, " +
                                    "        Sum(G.Acumuladas) As H_Acumuladas, Sum(G.Nocturnas) As H_Nocturnas, Sum(G.Desayuno) As Desayunos, " +
                                    "        Sum(G.Comida) As Comidas, Sum(G.Cena) As Cenas, Sum(G.PlusCena) As PlusesCena " +
                                    "FROM Graficos G LEFT JOIN GruposGraficos GG ON G.IdGrupo = GG.Id " +
                                    "WHERE GG.Id = ? " +
                                    "GROUP BY GG.Validez " +
                                    "ORDER BY GG.Validez";

                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                Comando.Parameters.AddWithValue("IdGrupo", idGrupo);
                conexion.Open();
                OleDbDataReader lector = Comando.ExecuteReader();
                if (lector.Read()) resultado = new EstadisticaGrupoGraficos(lector);
                lector.Close();
            }
            return resultado;
        }


        public static EstadisticaGrupoGraficos GetEstadisticasGrupoGraficos(DateTime fecha, Centros centro) {

            return App.Global.Repository.GetEstadisticasGrupoGraficos(fecha, centro);

        }




        /*================================================================================
        * GET GRÁFICOS FROM DÍA CALENDARIO
        * Ok
        *================================================================================*/
        public static List<GraficoFecha> GetGraficosFromDiaCalendario(DateTime fecha) {

            return App.Global.Repository.GetGraficosFromDiaCalendario(fecha, App.Global.PorCentro.Comodin).ToList();

            List<GraficoFecha> lista = new List<GraficoFecha>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.GetCadenaConexion(App.Global.CentroActual))) {

                string comandoSQL = "SELECT @fecha AS Fecha, * " +
                                    "FROM(SELECT * " +
                                    "     FROM Graficos " +
                                    "     WHERE IdGrupo = (SELECT Id " +
                                    "                      FROM GruposGraficos " +
                                    "                      WHERE Validez = (SELECT Max(Validez) " +
                                    "                                       FROM GruposGraficos " +
                                    "                                       WHERE Validez <= @fecha))) " +
                                    "WHERE(Numero IN(SELECT Grafico " +
                                    "                FROM DiasCalendario " +
                                    "                WHERE DiaFecha = @fecha AND Grafico > 0) " +
                                    "OR Numero IN(SELECT GraficoVinculado " +
                                    "             FROM DiasCalendario " +
                                    "             WHERE DiaFecha = @fecha AND GraficoVinculado > 0)) " +
                                    "AND Numero<> @comodin " +
                                    "ORDER BY Numero";

                conexion.Open();
                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    Comando.Parameters.AddWithValue("@fecha", fechaDia);
                    Comando.Parameters.AddWithValue("@comodin", App.Global.PorCentro.Comodin);
                    OleDbDataReader lector = Comando.ExecuteReader();
                    while (lector.Read()) {
                        GraficoFecha Gb = new GraficoFecha(lector);
                        Gb.Fecha = fechaDia;
                        lista.Add(Gb);
                    }
                    lector.Close();
                }
            }
            return lista;
        }


        /*================================================================================
        * GET ESTADÍSTICAS GRUPO GRÁFICOS
        * Ok
        *================================================================================*/
        public static List<GraficosPorDia> GetGraficosByDia(DateTime fecha) {

            return App.Global.Repository.GetGraficosByDia(fecha).ToList();

            List<GraficosPorDia> lista = new List<GraficosPorDia>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.GetCadenaConexion(App.Global.CentroActual))) {

                //string comandoSQL = "SELECT Numero " +
                //                    "FROM Graficos " +
                //                    "WHERE IdGrupo = (SELECT Id " +
                //                    "                 FROM GruposGraficos " +
                //                    "                 WHERE Validez = (SELECT Max(Validez) " +
                //                    "                                  FROM GruposGraficos " +
                //                    "                                  WHERE Validez <= @fecha)) " +
                //                    "AND Numero >= @del " +
                //                    "AND Numero <= @al " +
                //                    "ORDER BY Numero";
                string comandoSQL = "SELECT Numero " +
                                    "FROM Graficos " +
                                    "WHERE IdGrupo = (SELECT Id " +
                                    "                 FROM GruposGraficos " +
                                    "                 WHERE Validez = (SELECT Max(Validez) " +
                                    "                                  FROM GruposGraficos " +
                                    "                                  WHERE Validez <= @fecha)) " +
                                    "AND DiaSemana = @diasemana " +
                                    "ORDER BY Numero";

                conexion.Open();
                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    //int del;
                    //int al;
                    string diasemana;
                    switch (fechaDia.DayOfWeek) {
                        case DayOfWeek.Sunday:
                            //del = App.Global.PorCentro.DomDel;
                            //al = App.Global.PorCentro.DomAl;
                            diasemana = "F";
                            break;
                        case DayOfWeek.Saturday:
                            //del = App.Global.PorCentro.SabDel;
                            //al = App.Global.PorCentro.SabAl;
                            diasemana = "S";
                            break;
                        case DayOfWeek.Friday:
                            //del = App.Global.PorCentro.VieDel;
                            //al = App.Global.PorCentro.VieAl;
                            diasemana = "V";
                            break;
                        default:
                            //del = App.Global.PorCentro.LunDel;
                            //al = App.Global.PorCentro.LunAl;
                            diasemana = "L";
                            if (App.Global.CalendariosVM.EsFestivo(fechaDia.AddDays(1))) diasemana = "V";
                            break;
                    }
                    if (App.Global.CalendariosVM.EsFestivo(fechaDia)) {
                        //del = App.Global.PorCentro.DomDel;
                        //al = App.Global.PorCentro.DomAl;
                        diasemana = "F";
                    }
                    Comando.Parameters.AddWithValue("@fecha", fechaDia);
                    Comando.Parameters.AddWithValue("@diasemana", diasemana);
                    //Comando.Parameters.AddWithValue("@del", del);
                    //Comando.Parameters.AddWithValue("@al", al);
                    OleDbDataReader lector = Comando.ExecuteReader();
                    GraficosPorDia Gpd = new GraficosPorDia();
                    Gpd.Fecha = fechaDia;
                    while (lector.Read()) {
                        Gpd.Lista.Add(lector.ToInt16("Numero"));
                    }
                    lector.Close();
                    lista.Add(Gpd);
                }
            }
            return lista;
        }


        /*================================================================================
        * GET ESTADÍSTICAS GRUPO GRÁFICOS
        * Ok
        *================================================================================*/
        public static List<DescansosPorDia> GetDescansosByDia(DateTime fecha) {

            return App.Global.Repository.GetDescansosByDia(fecha).ToList();

            List<DescansosPorDia> lista = new List<DescansosPorDia>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.GetCadenaConexion(App.Global.CentroActual))) {

                string comandoSQL = "SELECT Count(*) " +
                                    "FROM DiasCalendario " +
                                    "WHERE DiaFecha = @fecha AND (Grafico = -2 OR Grafico = -3)";

                conexion.Open();
                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    Comando.Parameters.AddWithValue("@fecha", fechaDia);
                    DescansosPorDia Dpd = new DescansosPorDia();
                    Dpd.Fecha = fechaDia;
                    object res = Comando.ExecuteScalar();
                    Dpd.Descansos = Convert.ToInt32(res);
                    lista.Add(Dpd);
                }
            }
            return lista;
        }







    }
}
