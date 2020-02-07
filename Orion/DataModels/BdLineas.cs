#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.DataModels {

    /// <summary>
    /// Contiene los métodos necesarios para extraer las líneas de la base de datos.
    /// </summary>
    public static class BdLineas {


        /*================================================================================
		* GET LÍNEAS
        * Ok
		*================================================================================*/
        //public static ObservableCollection<Linea> GetLineas() {

        //    return new ObservableCollection<Linea>(App.Global.LineasRepo.GetLineas());

        //    // Creamos la lista y el comando que extrae las líneas.
        //    ObservableCollection<Linea> lista = new ObservableCollection<Linea>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
        //        string comandoSQL = "SELECT * FROM Lineas ORDER BY Nombre;";

        //        OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();

        //            lector = Comando.ExecuteReader();

        //            while (lector.Read()) {
        //                Linea linea = new Linea(lector);
        //                linea.ListaItinerarios = new NotifyCollection<Itinerario>(BdItinerarios.GetItinerarios(linea.Id));
        //                lista.Add(linea);
        //                linea.Nuevo = false;
        //                linea.Modificado = false;
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdLineas.GetLineas", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    return lista;
        //}


        /*================================================================================
		* GUARDAR LÍNEAS
        * Ok
		*================================================================================*/
        //public static void GuardarLineas(IEnumerable<Linea> lista) {

        //    App.Global.LineasRepo.GuardarLineas(lista);
        //    return;

        //    // Si la lista está vacía, salimos.
        //    if (lista == null || lista.Count() == 0) return;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
        //        string SQLInsertar = "INSERT INTO Lineas (Nombre, Descripcion) VALUES (?, ?)";

        //        string SQLActualizar = "UPDATE Lineas SET Nombre=?, Descripcion=? WHERE Id=?";

        //        string SQLGetID = "SELECT @@IDENTITY;";

        //        try {
        //            conexion.Open();

        //            foreach (Linea linea in lista) {
        //                if (linea.Nuevo) {
        //                    OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
        //                    Linea.ParseToCommand(comando, linea);
        //                    comando.ExecuteNonQuery();
        //                    comando.CommandText = SQLGetID;
        //                    int idlinea = (int)comando.ExecuteScalar();
        //                    foreach (Itinerario itinerario in linea.ListaItinerarios) {
        //                        itinerario.IdLinea = idlinea;
        //                    }
        //                    linea.Nuevo = false;
        //                    linea.Modificado = false;
        //                } else if (linea.Modificado) {
        //                    OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
        //                    Linea.ParseToCommand(comando, linea);
        //                    comando.ExecuteNonQuery();
        //                    linea.Modificado = false;
        //                }
        //            }
        //            foreach (Linea linea in lista) {
        //                BdItinerarios.GuardarItinerarios(linea.ListaItinerarios.Where(item => item.Nuevo || item.Modificado));
        //                BdItinerarios.BorrarItinerarios(linea.ItinerariosBorrados);
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdLineas.GuardarLineas", ex);
        //        }
        //    }
        //}


        /*================================================================================
		* BORRAR LÍNEAS
        * Ok
		*================================================================================*/
        //public static void BorrarLineas(List<Linea> lista) {

        //    App.Global.LineasRepo.BorrarLineas(lista);
        //    return;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexionLineas)) {
        //        string SQLBorrar = "DELETE FROM Lineas WHERE Id=?";

        //        try {
        //            conexion.Open();

        //            foreach (Linea linea in lista) {
        //                OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
        //                comando.Parameters.AddWithValue("id", linea.Id);
        //                comando.ExecuteNonQuery();
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdLineas.BorrarLineas", ex);
        //        }
        //    }
        //}


    }
}
