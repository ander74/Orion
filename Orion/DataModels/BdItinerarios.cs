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
    /// Contiene los métodos necesarios para extraer los itinerarios de la base de datos.
    /// </summary>
    public static class BdItinerarios {


        /*================================================================================
		 * GET ITINERARIOS
         * Ok
		 *================================================================================*/
        public static ObservableCollection<Itinerario> GetItinerarios(long IdLinea) {

            // Creamos la lista.
            ObservableCollection<Itinerario> lista = new ObservableCollection<Itinerario>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
                string comandoSQL = "SELECT * FROM Itinerarios WHERE IdLinea=? ORDER BY Nombre";

                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                Comando.Parameters.AddWithValue("idlinea", IdLinea);
                OleDbDataReader lector = null;

                try {
                    conexion.Open();
                    // Ejecutamos el comando y extraemos los gráficos del lector a la lista.
                    lector = Comando.ExecuteReader();
                    while (lector.Read()) {
                        Itinerario itinerario = new Itinerario(lector);
                        itinerario.ListaParadas = new NotifyCollection<Parada>(BdParadas.GetParadas(itinerario.Id));
                        lista.Add(itinerario);
                        itinerario.Nuevo = false;
                        itinerario.Modificado = false;
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdItinerarios.GetItinerarios", ex);
                } finally {
                    lector.Close();
                }
            }
            return lista;
        }


        /*================================================================================
		 * GUARDAR ITINERARIOS
         * Ok
		 *================================================================================*/
        public static void GuardarItinerarios(IEnumerable<Itinerario> lista) {

            // Si la lista está vacía, salimos.
            if (lista == null || lista.Count() == 0) return;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
                string SQLInsertar = "INSERT INTO Itinerarios (IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago) " +
                                     "VALUES (?, ?, ?, ?, ?);";

                string SQLActualizar = "UPDATE Itinerarios SET IdLinea=?, Nombre=?, Descripcion=?, TiempoReal=?, TiempoPago=? WHERE Id=?;";
                string SQLGetID = "SELECT @@IDENTITY;";

                try {
                    conexion.Open();

                    foreach (Itinerario itinerario in lista) {
                        if (itinerario.Nuevo) {
                            OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
                            Itinerario.ParseToCommand(comando, itinerario);
                            comando.ExecuteNonQuery();
                            comando.CommandText = SQLGetID;
                            int iditinerario = (int)comando.ExecuteScalar();
                            foreach (Parada parada in itinerario.ListaParadas) {
                                parada.IdItinerario = iditinerario;
                            }
                            itinerario.Id = iditinerario;
                            itinerario.Nuevo = false;
                            itinerario.Modificado = false;
                        } else if (itinerario.Modificado) {
                            OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
                            Itinerario.ParseToCommand(comando, itinerario);
                            comando.ExecuteNonQuery();
                            itinerario.Modificado = false;
                        }
                        BdParadas.GuardarParadas(itinerario.ListaParadas.Where(item => item.Nuevo || item.Modificado));
                        BdParadas.BorrarParadas(itinerario.ParadasBorradas);
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdItinerarios.GuardarItinerarios", ex);
                }
            }
        }


        /*================================================================================
		 * BORRAR ITINERARIOS
         * Ok
		 *================================================================================*/
        public static void BorrarItinerarios(List<Itinerario> lista) {

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
                string SQLBorrar = "DELETE FROM Itinerarios WHERE Id=?";

                try {
                    conexion.Open();

                    foreach (Itinerario itinerario in lista) {
                        OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
                        comando.Parameters.AddWithValue("id", itinerario.Id);
                        comando.ExecuteNonQuery();
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdItinerarios.BorrarItinerarios", ex);
                }
            }
        }


        /*================================================================================
		 * GET ITINERARIO BY NOMBRE
         * Ok
		 *================================================================================*/
        public static Itinerario GetItinerarioByNombre(decimal nombre) {

            return App.Global.LineasRepo.GetItinerarioByNombre(nombre);

            Itinerario itinerario = null;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
                // Creamos el comando que extrae el itinerario
                string comandoSQL = "SELECT * FROM Itinerarios WHERE Nombre = ?";

                OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
                Comando.Parameters.AddWithValue("nombre", nombre.ToString());
                OleDbDataReader lector = null;

                try {
                    conexion.Open();

                    lector = Comando.ExecuteReader();

                    if (lector.Read()) {
                        itinerario = new Itinerario(lector);
                        itinerario.ListaParadas = new NotifyCollection<Parada>(BdParadas.GetParadas(itinerario.Id));
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdItinerarios.GetItinerarioByNombre", ex);
                } finally {
                    lector.Close();
                }
            }

            return itinerario;

        }



    }
}
