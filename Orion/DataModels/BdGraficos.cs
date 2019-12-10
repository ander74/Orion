#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using Orion.Config;
using Orion.Models;
using Orion.Servicios;

namespace Orion.DataModels {

    /// <summary>
    /// Contiene los métodos necesarios para trabajar con gráficos en la base de datos.
    /// </summary>
    public static class BdGraficos {


        /*================================================================================
		* GET GRÁFICOS
        * Ok
		*================================================================================*/
        /// <summary>
        /// Devuelve una colección con los gráficos pertenecientes a un grupo
        /// </summary>
        /// <param name="IdGrupo">Id del grupo al que pertenecen los gráficos.</param>
        /// <returns>Colección de gráficos.</returns>
        public static ObservableCollection<Grafico> getGraficos(long IdGrupo) {

            ObservableCollection<Grafico> lista = new ObservableCollection<Grafico>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                OleDbDataReader lector = null;
                OleDbDataReader lectorValoraciones = null;

                try {

                    // Ejecutamos el comando y extraemos los gráficos del lector a la lista.
                    conexion.Open();
                    OleDbTransaction transaccion = conexion.BeginTransaction();

                    string comandoSQL = "SELECT * FROM Graficos WHERE IdGrupo=? ORDER BY Numero";
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion, transaccion);
                    Comando.Parameters.AddWithValue("idgrupo", IdGrupo);

                    lector = Comando.ExecuteReader();

                    while (lector.Read()) {
                        Grafico grafico = new Grafico(lector);
                        grafico.ListaValoraciones = new ObservableCollection<ValoracionGrafico>();
                        string comandoSQLValoraciones = "SELECT * FROM Valoraciones WHERE IdGrafico=? ORDER BY Inicio, Id";
                        OleDbCommand ComandoValoraciones = new OleDbCommand(comandoSQLValoraciones, conexion, transaccion);
                        ComandoValoraciones.Parameters.AddWithValue("idgrafico", grafico.Id);
                        lectorValoraciones = ComandoValoraciones.ExecuteReader();
                        while (lectorValoraciones.Read()) {
                            ValoracionGrafico valoracion = new ValoracionGrafico(lectorValoraciones);
                            grafico.ListaValoraciones.Add(valoracion);
                            valoracion.Nuevo = false;
                            valoracion.Modificado = false;
                        }
                        //grafico.ListaValoraciones = BdValoracionesGraficos.getValoraciones(grafico.Id);
                        lista.Add(grafico);
                        grafico.Nuevo = false;
                        grafico.Modificado = false;
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.getGraficos", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;

        }



        /*================================================================================
		* GUARDAR GRÁFICOS
        * Ok
		*================================================================================*/
        /// <summary>
        /// Guarda la lista de gráficos que se le pasa en la base de datos, actualizando los modificados e insertando los nuevos.
        /// </summary>
        /// <param name="lista">Lista con los gráficos a guardar.</param>
        public static void GuardarGraficos(IEnumerable<Grafico> lista) {

            // Si la lista está vacía, salimos.
            if (lista == null || lista.Count() == 0) return;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
                string SQLInsertar = "INSERT INTO Graficos (IdGrupo, NoCalcular, Numero, DiaSemana, Turno, Inicio, Final, InicioPartido, " +
                                     "FinalPartido, Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, PlusLimpieza, PlusPaqueteria) " +
                                     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

                string SQLActualizar = "UPDATE Graficos SET IdGrupo=?, NoCalcular=?, Numero=?, DiaSemana=?, Turno=?, Inicio=?, Final=?, " +
                                       "InicioPartido=?, FinalPartido=?, Valoracion=?, Trabajadas=?, " +
                                       "Acumuladas=?, Nocturnas=?, Desayuno=?, Comida=?, Cena=?, " +
                                       "PlusCena=?, PlusLimpieza=?, PlusPaqueteria=? WHERE Id=?;";
                string SQLGetID = "SELECT @@IDENTITY;";

                try {

                    conexion.Open();

                    foreach (Grafico grafico in lista) {
                        if (grafico.Nuevo) {
                            OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
                            Grafico.ParseToCommand(comando, grafico);
                            comando.ExecuteNonQuery();
                            comando.CommandText = SQLGetID;
                            int idgrafico = (int)comando.ExecuteScalar();
                            foreach (ValoracionGrafico valoracion in grafico.ListaValoraciones) {
                                valoracion.IdGrafico = idgrafico;
                            }
                            grafico.Id = idgrafico;
                            grafico.Nuevo = false;
                            grafico.Modificado = false;
                        } else if (grafico.Modificado) {
                            OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
                            Grafico.ParseToCommand(comando, grafico);
                            comando.ExecuteNonQuery();
                            grafico.Modificado = false;
                        }
                        BdValoracionesGraficos.GuardarValoraciones(grafico.ListaValoraciones.Where(v => v.Nuevo || v.Modificado));
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.GuardarGraficos", ex);
                }
            }
        }



        /*================================================================================
		* INSERTAR GRÁFICO
        * Ok
		*================================================================================*/
        /// <summary>
        /// Guarda la lista de gráficos que se le pasa en la base de datos, actualizando los modificados e insertando los nuevos.
        /// </summary>
        /// <param name="lista">Lista con los gráficos a guardar.</param>
        public static int InsertarGrafico(Grafico grafico) {

            int idgraficonuevo = -1;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
                string SQLInsertar = "INSERT INTO Graficos (IdGrupo, NoCalcular, Numero, DiaSemana, Turno, Inicio, Final, InicioPartido, " +
                                 "FinalPartido, Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, PlusLimpieza, PlusPaqueteria) " +
                                 "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

                OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
                Grafico.ParseToCommand(comando, grafico);
                try {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    grafico.Nuevo = false;
                    grafico.Modificado = false;
                    comando.CommandText = "SELECT @@IDENTITY";
                    idgraficonuevo = (int)comando.ExecuteScalar();
                    foreach (ValoracionGrafico valoracion in grafico.ListaValoraciones) {
                        valoracion.IdGrafico = idgraficonuevo;
                        valoracion.Nuevo = true;
                    }
                    BdValoracionesGraficos.GuardarValoraciones(grafico.ListaValoraciones.Where(v => v.Nuevo || v.Modificado));
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.InsertarGrafico", ex);
                }
            }
            return idgraficonuevo;
        }


        /*================================================================================
		 * BORRAR GRAFICOS
         * Ok
		 *================================================================================*/
        /// <summary>
        /// Elimina de la base de datos los gráficos pasados en la lista.
        /// </summary>
        /// <param name="lista">Lista con los gráficos a borrar.</param>
        public static void BorrarGraficos(List<Grafico> lista) {

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
                string SQLBorrar = "DELETE FROM Graficos WHERE Id=?";
                try {
                    conexion.Open();
                    foreach (Grafico grafico in lista) {
                        OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
                        comando.Parameters.AddWithValue("id", grafico.Id);
                        comando.ExecuteNonQuery();
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.BorrarGraficos", ex);
                }
            }
        }



        /*================================================================================
		 * GET GRÁFICOS GRUPO POR FECHA
         * Ok
		 *================================================================================*/
        public static ObservableCollection<Grafico> getGraficosGrupoPorFecha(DateTime fecha) {

            // Creamos la lista y el comando que extrae los gráficos.
            ObservableCollection<Grafico> lista = new ObservableCollection<Grafico>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                string comandoSQL = "SELECT * FROM Graficos WHERE IdGrupo = " +
                                "(SELECT Id FROM GruposGraficos WHERE Validez = ?)";

                OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
                comando.Parameters.AddWithValue("validez", fecha.ToString("yyyy-MM-dd"));
                OleDbDataReader lector = null;
                try {
                    conexion.Open();
                    lector = comando.ExecuteReader();

                    while (lector.Read()) {
                        Grafico grafico = new Grafico(lector);
                        lista.Add(grafico);
                        grafico.Nuevo = false;
                        grafico.Modificado = false;
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.getGraficosGrupoPorFecha", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;
        }



        /*================================================================================
		* GET ESTADISTICAS GRÁFICOS
        * Ok
		*================================================================================*/
        /// <summary>
        /// Devuelve una colección con los gráficos pertenecientes a un grupo
        /// </summary>
        /// <param name="IdGrupo">Id del grupo al que pertenecen los gráficos.</param>
        /// <returns>Colección de gráficos.</returns>
        public static List<EstadisticasGraficos> GetEstadisticasGrupoGraficos(long IdGrupo, OleDbConnection conexion = null) {

            List<EstadisticasGraficos> lista = new List<EstadisticasGraficos>();

            if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
            using (conexion) {
                OleDbDataReader lector = null;
                try {
                    string comandoSQL = DbService2.GetEstadisticasGraficos;
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    Comando.Parameters.AddWithValue("@JornadaMedia", App.Global.Convenio.JornadaMedia.Ticks);
                    Comando.Parameters.AddWithValue("@IdGrupo", IdGrupo);
                    conexion.Open();
                    lector = Comando.ExecuteReader();
                    if (lector.HasRows) {
                        while (lector.Read()) {
                            EstadisticasGraficos estadistica = new EstadisticasGraficos(lector);
                            lista.Add(estadistica);
                        }
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.GetEstadisticasGrupoGraficos", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;
        }


        /*================================================================================
		* GET ESTADISTICAS GRÁFICOS
        * Ok
		*================================================================================*/
        /// <summary>
        /// Devuelve una colección con los gráficos pertenecientes a un grupo
        /// </summary>
        /// <param name="IdGrupo">Id del grupo al que pertenecen los gráficos.</param>
        /// <returns>Colección de gráficos.</returns>
        public static List<EstadisticasGraficos> GetEstadisticasGraficosDesdeFecha(DateTime fecha) {

            List<EstadisticasGraficos> lista = new List<EstadisticasGraficos>();
            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
                OleDbDataReader lector = null;
                try {
                    string comandoSQL = DbService2.GetEstadisticasGraficosDesdeFecha;
                    OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                    Comando.Parameters.AddWithValue("@JornadaMedia", App.Global.Convenio.JornadaMedia.Ticks);
                    Comando.Parameters.AddWithValue("@Fecha", fecha.ToString("yyyy-MM-dd"));
                    conexion.Open();
                    lector = Comando.ExecuteReader();
                    if (lector.HasRows) {
                        while (lector.Read()) {
                            EstadisticasGraficos estadistica = new EstadisticasGraficos(lector);
                            lista.Add(estadistica);
                        }
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGraficos.GetEstadisticasGraficosDesdeFecha", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;
        }


        /*================================================================================
		* GET GRÁFICO
        * Ok
		*================================================================================*/
        public static GraficoBase GetGrafico(int numero, DateTime fecha) {

            GraficoBase resultado = null;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                string comandoSQL = "SELECT * " +
                                    "FROM (SELECT * " +
                                    "      FROM Graficos" +
                                    "      WHERE IdGrupo = (SELECT Id " +
                                    "                       FROM GruposGraficos " +
                                    "                       WHERE Validez = (SELECT Max(Validez) " +
                                    "                                        FROM GruposGraficos " +
                                    "                                        WHERE Validez <= ?)))" +
                                    "WHERE Numero = ?";

                // Elementos para la consulta de calendarios y días de calendario.
                OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
                comando.Parameters.AddWithValue("@Validez", fecha.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@IdConductor", numero);
                OleDbDataReader lector = null;
                try {
                    conexion.Open();
                    lector = comando.ExecuteReader();
                    if (lector.Read()) resultado = new GraficoBase(lector);
                } catch (OleDbException ex) {
                    Utils.VerError("BdGraficos.GetGrafico", ex);
                } finally {
                    lector.Close();
                }
            }
            return resultado;
        }



    } //Fin de clase
}
