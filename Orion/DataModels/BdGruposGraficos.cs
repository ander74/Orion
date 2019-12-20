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

namespace Orion.DataModels {


    /// <summary>
    /// Contiene los métodos necesarios para trabajar con grupos de gráficos en la base de datos.
    /// </summary>
    public static class BdGruposGraficos {



        /*================================================================================
		* GET GRUPOS
        * Ok
		*================================================================================*/
        public static ObservableCollection<GrupoGraficos> getGrupos(OleDbConnection conexion = null) {

            return new ObservableCollection<GrupoGraficos>(App.Global.Repository.GetGrupos());

            // Creamos la lista.
            ObservableCollection<GrupoGraficos> lista = new ObservableCollection<GrupoGraficos>();

            if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
            using (conexion) {

                string comandoSQL = "SELECT * FROM GruposGraficos ORDER BY Validez DESC";
                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                OleDbDataReader lector = null;

                try {
                    conexion.Open();
                    lector = Comando.ExecuteReader();

                    while (lector.Read()) {
                        GrupoGraficos grupo = new GrupoGraficos(lector);
                        lista.Add(grupo);
                        grupo.Nuevo = false;
                        grupo.Modificado = false;
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.getGrupos", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;
        }



        /*================================================================================
		* GUARDAR GRUPOS
        * Ok
		*================================================================================*/
        public static void GuardarGrupos(IEnumerable<GrupoGraficos> lista) {

            App.Global.Repository.GuardarGrupos(lista);
            return;

            // Si la lista está vacía, salimos.
            if (lista == null || lista.Count() == 0) return;

            string SQLInsertar = "INSERT INTO GruposGraficos (Validez, Notas) VALUES (?, ?)";

            string SQLActualizar = "UPDATE GruposGraficos SET Validez=?, Notas=? WHERE Id=?";

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                try {
                    conexion.Open();

                    foreach (GrupoGraficos grupo in lista) {
                        if (grupo.Nuevo) {
                            OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
                            GrupoGraficos.ParseToCommand(comando, grupo);
                            comando.ExecuteNonQuery();
                            grupo.Nuevo = false;
                            grupo.Modificado = false;
                        } else if (grupo.Modificado) {
                            OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
                            GrupoGraficos.ParseToCommand(comando, grupo);
                            comando.ExecuteNonQuery();
                            grupo.Modificado = false;
                        }
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.GuardarGrupos", ex);
                }
            }
        }



        /*================================================================================
		 * BORRAR GRUPO POR ID
         * Ok
		 *================================================================================*/
        public static void BorrarGrupoPorId(long idgrupo) {

            App.Global.Repository.BorrarGrupoPorId(Convert.ToInt32(idgrupo));
            return;


            string SQLBorrar = "DELETE FROM GruposGraficos WHERE Id=?";

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
                comando.Parameters.AddWithValue("id", idgrupo);
                try {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.BorrarGrupoPorId", ex);
                }
            }
        }


        /*================================================================================
		 * NUEVO GRUPO
         * Ok
		 *================================================================================*/
        public static int NuevoGrupo(DateTime fecha, string notas) {

            return App.Global.Repository.NuevoGrupo(fecha, notas);

            int idgruponuevo = -1;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                string SQLNuevo = "INSERT INTO GruposGraficos (Validez, Notas) VALUES (?, ?)";

                OleDbCommand comando = new OleDbCommand(SQLNuevo, conexion);
                comando.Parameters.AddWithValue("validez", fecha.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("notas", String.IsNullOrEmpty(notas) ? "" : notas);
                try {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                    comando.CommandText = "SELECT @@IDENTITY";
                    idgruponuevo = (int)comando.ExecuteScalar();
                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.NuevoGrupo", ex);
                }
            }

            return idgruponuevo;
        }


        /*================================================================================
		 * EXISTE GRUPO
         * Ok
		 *================================================================================*/
        public static bool ExisteGrupo(DateTime fecha) {

            return App.Global.Repository.ExisteGrupo(fecha);

            Int32 numero = 0;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
                string comandoSQL = "SELECT Count(*) FROM GruposGraficos WHERE Validez=?";
                OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
                comando.Parameters.AddWithValue("validez", fecha.ToString("yyyy-MM-dd"));

                try {
                    // Ejecutamos el comando y extraemos los gráficos del lector a la lista.
                    conexion.Open();
                    numero = (Int32)comando.ExecuteScalar();
                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.ExisteGrupo", ex);
                }
            }

            return numero > 0;
        }


        /*================================================================================
		 * GET ÚLTIMO GRUPO
         * Ok
		 *================================================================================*/
        public static GrupoGraficos GetUltimoGrupo(OleDbConnection conexion = null) {

            return App.Global.Repository.GetUltimoGrupo();

            GrupoGraficos grupo = null;

            if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
            using (conexion) {
                string comandoSQL = "SELECT * FROM GruposGraficos WHERE Validez=(SELECT Max(Validez) FROM GruposGraficos);";
                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);

                try {
                    // Ejecutamos el comando y extraemos los gráficos del lector a la lista.
                    conexion.Open();
                    OleDbDataReader lector = Comando.ExecuteReader();

                    if (lector.Read()) {
                        grupo = new GrupoGraficos(lector);
                    }
                    lector.Close();

                } catch (Exception ex) {
                    Utils.VerError("BdGruposGraficos.GetUltimoGrupo", ex);
                }
            }

            return grupo;

        }



    } //Fin de clase
}
