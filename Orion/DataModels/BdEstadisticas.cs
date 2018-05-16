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
using System.Text;
using System.Threading.Tasks;

namespace Orion.DataModels {


    public class BdEstadisticas {


        /*================================================================================
        * GET ESTADÍSTICAS ÚLTIMO GRUPO GRÁFICOS
        *================================================================================*/
        public static EstadisticaGrupoGraficos GetEstadisticasUltimoGrupoGraficos(Centros centro) {

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
                
                try {
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    conexion.Open();
                    OleDbDataReader lector = Comando.ExecuteReader();
                    if (lector.Read()) resultado = new EstadisticaGrupoGraficos(lector);
                    lector.Close();
                } catch (Exception ex) {
                    Utils.VerError("BdEstadisticas.GetEstadisticasUltimoGrupoGraficos", ex);
                }
            }
            return resultado;

        }



        /*================================================================================
        * GET ESTADÍSTICAS GRUPO GRÁFICOS
        *================================================================================*/
        public static EstadisticaGrupoGraficos GetEstadisticasGrupoGraficos(long idGrupo, Centros centro) {

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

                try {
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    Comando.Parameters.AddWithValue("IdGrupo", idGrupo);
                    conexion.Open();
                    OleDbDataReader lector = Comando.ExecuteReader();
                    if (lector.Read()) resultado = new EstadisticaGrupoGraficos(lector);
                    lector.Close();
                } catch (Exception ex) {
                    Utils.VerError("BdEstadisticas.GetEstadisticasGrupoGraficos", ex);
                }
            }
            return resultado;

        }







    }
}
