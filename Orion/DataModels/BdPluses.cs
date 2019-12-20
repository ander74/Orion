#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.DataModels {
    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Linq;
    using Orion.Config;
    using Orion.Models;

    public static class BdPluses {


        /*================================================================================
		* GET PLUSES
        * Ok
		*================================================================================*/
        public static FullObservableCollection<Pluses> GetPluses() {

            return new FullObservableCollection<Pluses>(App.Global.Repository.GetPluses());

            // Creamos la lista y el comando que extrae las líneas.
            FullObservableCollection<Pluses> lista = new FullObservableCollection<Pluses>();

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                string comandoSQL = "SELECT * FROM Pluses ORDER BY Año;";

                OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
                OleDbDataReader lector = null;

                try {
                    conexion.Open();

                    lector = comando.ExecuteReader();

                    while (lector.Read()) {
                        Pluses pluses = new Pluses(lector);
                        lista.Add(pluses);
                        pluses.Nuevo = false;
                        pluses.Modificado = false;
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdPluses.GetPluses", ex);
                } finally {
                    lector?.Close();
                }
            }
            return lista;
        }



        /*================================================================================
		* GUARDAR PLUSES
        * Ok
		*================================================================================*/
        public static void GuardarPluses(IEnumerable<Pluses> lista) {

            App.Global.Repository.GuardarPluses(lista);
            return;

            // Si la lista está vacía, salimos.
            if (lista == null || lista.Count() == 0) return;

            using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

                string SQLInsertar = "INSERT INTO Pluses (Año, ImporteDietas, ImporteSabados, ImporteFestivos, " +
                                     "                    PlusNocturnidad, DietaMenorDescanso, PlusLimpieza, PlusPaqueteria, PlusNavidad) " +
                                     "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);";

                string SQLActualizar = "UPDATE Pluses SET Año=?, ImporteDietas=?, ImporteSabados=?, ImporteFestivos=?, PlusNocturnidad=?, " +
                                       "                  DietaMenorDescanso=?, PlusLimpieza=?, PlusPaqueteria=?, PlusNavidad=? " +
                                       "WHERE Id=?;";

                try {
                    conexion.Open();

                    foreach (Pluses pluses in lista) {
                        if (pluses.Nuevo) {
                            OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
                            Pluses.ParseToCommand(comando, pluses);
                            comando.ExecuteNonQuery();
                            pluses.Nuevo = false;
                            pluses.Modificado = false;
                        } else if (pluses.Modificado) {
                            OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
                            Pluses.ParseToCommand(comando, pluses);
                            comando.ExecuteNonQuery();
                            pluses.Modificado = false;
                        }
                    }
                } catch (Exception ex) {
                    Utils.VerError("BdPluses.GuardarPluses", ex);
                }
            }
        }







    }
}
