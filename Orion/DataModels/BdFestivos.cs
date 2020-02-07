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
    public static class BdFestivos {


        /*================================================================================
		* GET FESTIVOS
        * Ok
		*================================================================================*/
        //public static List<Festivo> GetFestivos() {

        //    return App.Global.Repository.GetFestivos().ToList();

        //    // Creamos la lista y el comando que extrae las líneas.
        //    List<Festivo> lista = new List<Festivo>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        string comandoSQL = "SELECT * FROM Festivos ORDER BY Fecha;";

        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();
        //            lector = comando.ExecuteReader();
        //            while (lector.Read()) {
        //                Festivo festivo = new Festivo(lector);
        //                lista.Add(festivo);
        //                festivo.Nuevo = false;
        //                festivo.Modificado = false;
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdFestivos.GetFestivosPorMes", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    return lista;
        //}


        /*================================================================================
		* GET FESTIVOS POR MES
        * Ok
        * No tiene referencias
		*================================================================================*/
        //public static ObservableCollection<Festivo> GetFestivosPorMes(int año, int mes) {

        //    // Creamos la lista y el comando que extrae las líneas.
        //    ObservableCollection<Festivo> lista = new ObservableCollection<Festivo>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        //string comandoSQL = "SELECT * FROM Festivos WHERE Año=? AND Month(Fecha)=? ORDER BY Fecha;";
        //        string comandoSQL = DbService2.GetFestivosPorMes;

        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("Año", año);
        //        comando.Parameters.AddWithValue("Mes", mes);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();

        //            lector = comando.ExecuteReader();

        //            while (lector.Read()) {
        //                Festivo festivo = new Festivo(lector);
        //                lista.Add(festivo);
        //                festivo.Nuevo = false;
        //                festivo.Modificado = false;
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdFestivos.GetFestivosPorMes", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    return lista;
        //}


        /*================================================================================
		* GET FESTIVOS POR AÑO
        * Ok
        * No tiene referencias
		*================================================================================*/
        //public static ObservableCollection<Festivo> GetFestivosPorAño(int año) {

        //    // Creamos la lista y el comando que extrae las líneas.
        //    ObservableCollection<Festivo> lista = new ObservableCollection<Festivo>();

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        //string comandoSQL = "SELECT * FROM Festivos WHERE Año=? ORDER BY Fecha;";
        //        string comandoSQL = DbService2.GetFestivosPorAño;

        //        OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
        //        comando.Parameters.AddWithValue("Año", año);
        //        OleDbDataReader lector = null;

        //        try {
        //            conexion.Open();

        //            lector = comando.ExecuteReader();

        //            while (lector.Read()) {
        //                Festivo festivo = new Festivo(lector);
        //                lista.Add(festivo);
        //                festivo.Nuevo = false;
        //                festivo.Modificado = false;
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdFestivos.GetFestivosPorAño", ex);
        //        } finally {
        //            lector.Close();
        //        }
        //    }
        //    return lista;
        //}


        /*================================================================================
		* GUARDAR FESTIVOS
        * Ok
		*================================================================================*/
        //public static void GuardarFestivos(IEnumerable<Festivo> lista) {

        //    App.Global.Repository.GuardarFestivos(lista);
        //    return;

        //    // Si la lista está vacía, salimos.
        //    if (lista == null || lista.Count() == 0) return;

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        //string SQLInsertar = "INSERT INTO Festivos (Año, Fecha) VALUES (?, ?)";
        //        string SQLInsertar = DbService2.InsertarFestivo;

        //        //string SQLActualizar = "UPDATE Festivos SET Año=?, Fecha=? WHERE Id=?";
        //        string SQLActualizar = DbService2.ActualizarFestivo;

        //        try {
        //            conexion.Open();

        //            foreach (Festivo festivo in lista) {
        //                if (festivo.Nuevo) {
        //                    OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
        //                    Festivo.ParseToCommand(comando, festivo);
        //                    comando.ExecuteNonQuery();
        //                    festivo.Nuevo = false;
        //                    festivo.Modificado = false;
        //                } else if (festivo.Modificado) {
        //                    OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
        //                    Festivo.ParseToCommand(comando, festivo);
        //                    comando.ExecuteNonQuery();
        //                    festivo.Modificado = false;
        //                }
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdFestivos.GuardarFestivos", ex);
        //        }
        //    }
        //}


        /*================================================================================
		* BORRAR FESTIVOS
        * Ok
		*================================================================================*/
        //public static void BorrarFestivos(IEnumerable<Festivo> lista) {

        //    App.Global.Repository.BorrarFestivos(lista);

        //    using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

        //        //string SQLBorrar = "DELETE FROM Festivos WHERE Id=?";
        //        string SQLBorrar = DbService2.BorrarFestivo;

        //        try {
        //            conexion.Open();

        //            foreach (Festivo festivo in lista) {
        //                OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
        //                comando.Parameters.AddWithValue("id", festivo.Id);
        //                comando.ExecuteNonQuery();
        //            }
        //        } catch (Exception ex) {
        //            Utils.VerError("BdFestivos.BorrarFestivo", ex);
        //        }
        //    }
        //}


    }
}
