#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.DataModels {


    /// <summary>
    /// Contiene los métodos necesarios para trabajar con conductores en la base de datos.
    /// </summary>
    //  public static class BdConductores {


    //      /*================================================================================
    //* GET CONDUCTORES
    //      * Ok
    //*================================================================================*/
    //      public static IEnumerable<Conductor> GetConductores() {

    //          return App.Global.Repository.GetConductores();
    //          //return App.Global.Repository.GetPrueba(App.Global.GetArchivoDatos(Centros.Arrasate)).OrderBy(c => c.Matricula).ToList();

    //          // Creamos la lista y el comando que extrae los gráficos.
    //          List<Conductor> lista = new List<Conductor>();

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

    //              //string comandoSQL = "SELECT * FROM Conductores ORDER BY Id";
    //              string comandoSQL = DbService2.GetConductores;

    //              OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
    //              OleDbDataReader lector = null;

    //              try {
    //                  conexion.Open();
    //                  lector = Comando.ExecuteReader();

    //                  while (lector.Read()) {
    //                      Conductor conductor = new Conductor(lector);
    //                      conductor.ListaRegulaciones = new NotifyCollection<RegulacionConductor>(BdRegulacionConductor.GetRegulaciones(conductor.Id));
    //                      lista.Add(conductor);
    //                      conductor.Nuevo = false;
    //                      conductor.Modificado = false;
    //                  }
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.GetConductores", ex);
    //              } finally {
    //                  lector.Close();
    //              }
    //          }
    //          return lista;
    //      }


    //      /*================================================================================
    //* GUARDAR CONDUCTORES
    //      * Ok
    //*================================================================================*/
    //      public static void GuardarConductores(IEnumerable<Conductor> lista) {

    //          App.Global.Repository.GuardarConductores(lista);
    //          return;

    //          // Si la lista está vacía, salimos.
    //          if (lista == null || lista.Count() == 0) return;

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

    //              string SQLInsertar = DbService2.InsertarConductor;
    //              string SQLActualizar = DbService2.ActualizarConductor;

    //              try {
    //                  conexion.Open();

    //                  App.Global.Repository.GuardarConductores(lista);

    //                  foreach (Conductor conductor in lista) {
    //                      if (conductor.Nuevo) {
    //                          // Si el conductor ya existe, saltarselo.
    //                          if (ExisteConductor(conductor.Id)) {
    //                              conductor.Id = 0;
    //                              continue;
    //                          }
    //                          OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
    //                          Conductor.ParseToCommand(comando, conductor);
    //                          comando.ExecuteNonQuery();
    //                          foreach (RegulacionConductor regulacion in conductor.ListaRegulaciones) {
    //                              regulacion.IdConductor = conductor.Id;
    //                          }
    //                          conductor.Nuevo = false;
    //                          conductor.Modificado = false;
    //                      } else if (conductor.Modificado) {
    //                          OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
    //                          Conductor.ParseToCommand(comando, conductor);
    //                          comando.ExecuteNonQuery();
    //                          conductor.Modificado = false;
    //                      }
    //                      BdRegulacionConductor.GuardarRegulaciones(conductor.ListaRegulaciones.Where(r => r.Nuevo || r.Modificado));
    //                  }
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.GuardarConductores", ex);
    //              }
    //          }
    //      }


    //      /*================================================================================
    // * BORRAR CONDUCTORES
    //       * Ok
    // *================================================================================*/
    //      public static void BorrarConductores(IEnumerable<Conductor> lista) {

    //          App.Global.Repository.BorrarConductores(lista);
    //          return;

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

    //              //string SQLBorrar = "DELETE FROM Conductores WHERE Id=?";
    //              string SQLBorrar = DbService2.BorrarConductor;

    //              try {
    //                  conexion.Open();

    //                  foreach (Conductor conductor in lista) {
    //                      OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
    //                      comando.Parameters.AddWithValue("id", conductor.Id);
    //                      comando.ExecuteNonQuery();
    //                  }
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.BorrarConductores", ex);
    //              }
    //          }
    //      }


    //      /*================================================================================
    // * EXISTE CONDUCTOR
    //       * Ok
    // *================================================================================*/
    //      public static bool ExisteConductor(int idconductor) {

    //          return App.Global.Repository.ExisteConductor(idconductor);

    //          int numero = 0;

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

    //              // Creamos el comando que lee el número de conductores que tienen el id
    //              //string comandoSQL = "SELECT Count(Id) FROM Conductores WHERE Id = ?";
    //              string comandoSQL = DbService2.ExisteConductor;
    //              OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
    //              Comando.Parameters.AddWithValue("@Id", idconductor);
    //              try {
    //                  conexion.Open();
    //                  // Ejecutamos el comando.
    //                  numero = (int)Comando.ExecuteScalar();
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.ExisteConductor", ex);
    //              }
    //          }
    //          // Devolvemos true si hay algún conductor.
    //          return numero > 0;
    //      }


    //      /*================================================================================
    // * INSERTAR CONDUCTOR DESCONOCIDO
    //       * Ok
    // *================================================================================*/
    //      public static void InsertarConductorDesconocido(int idconductor) {

    //          App.Global.Repository.InsertarConductorDesconocido(idconductor);
    //          return;

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
    //              //string SQLInsertar = "INSERT INTO Conductores (Id, Nombre) VALUES (?, 'Desconocido')";
    //              string SQLInsertar = DbService2.InsertarConductorDesconocido;
    //              OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
    //              comando.Parameters.AddWithValue("@Id", idconductor);
    //              try {
    //                  conexion.Open();
    //                  comando.ExecuteNonQuery();
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.InsertarConductorDesconocido", ex);
    //              }
    //          }
    //      }


    //      /*================================================================================
    // * GET CONDUCTOR
    //       * Ok
    // *================================================================================*/
    //      public static Conductor GetConductor(int idconductor) {

    //          return App.Global.Repository.GetConductor(idconductor);

    //          Conductor conductor = null;

    //          using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {

    //              // Creamos el comando que extrae el conductor
    //              //string comandoSQL = "SELECT * FROM Conductores WHERE Id = ?";
    //              string comandoSQL = DbService2.GetConductor;

    //              OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
    //              Comando.Parameters.AddWithValue("@Id", idconductor);
    //              OleDbDataReader lector = null;
    //              try {
    //                  conexion.Open();
    //                  lector = Comando.ExecuteReader();

    //                  if (lector.Read()) {
    //                      conductor = new Conductor(lector);
    //                      conductor.ListaRegulaciones = new NotifyCollection<RegulacionConductor>(BdRegulacionConductor.GetRegulaciones(conductor.Id));
    //                  }
    //              } catch (Exception ex) {
    //                  Utils.VerError("BdConductores.GetConductor", ex);
    //              } finally {
    //                  lector.Close();
    //              }
    //          }
    //          return conductor;

    //      }




    //  } //Final de clase
}
