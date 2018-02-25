﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orion.Models;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Windows;
using Orion.Config;

namespace Orion.DataModels {


	/// <summary>
	/// Contiene los métodos necesarios para trabajar con grupos de gráficos en la base de datos.
	/// </summary>
	public static class BdGruposGraficos {



		/*================================================================================
		* GET GRUPOS
		*================================================================================*/
		public static ObservableCollection<GrupoGraficos> getGrupos(OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista.
			ObservableCollection<GrupoGraficos> lista = new ObservableCollection<GrupoGraficos>();

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
					lector.Close();
				} catch (Exception ex) {
					Utils.VerError("BdGruposGraficos.getGrupos", ex);
				}
			}
			return lista;
		}



		/*================================================================================
		* GUARDAR GRUPOS
		*================================================================================*/
		public static void GuardarGrupos(ObservableCollection<GrupoGraficos> lista, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count == 0) return;

			string SQLInsertar = "INSERT INTO GruposGraficos (Validez, Notas) VALUES (?, ?)";

			string SQLActualizar = "UPDATE GruposGraficos SET Validez=?, Notas=? WHERE Id=?";

			using (conexion) {

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
		 * BORRAR GRUPOS
		 *================================================================================*/
		[Obsolete("Esté método no se usa en ningun sitio.")]
		public static void BorrarGrupo(List<GrupoGraficos> lista) {

			string SQLBorrar = "DELETE FROM GruposGraficos WHERE Id=?";

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
				try {
					foreach (GrupoGraficos grupo in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.Parameters.AddWithValue("id", grupo.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex){
					Utils.VerError("BdGruposGraficos.BorrarGrupo", ex);
				}
			}
		}


		/*================================================================================
		 * BORRAR GRUPO POR ID
		 *================================================================================*/
		public static void BorrarGrupoPorId(long idgrupo, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			string SQLBorrar = "DELETE FROM GruposGraficos WHERE Id=?";

			using (conexion) {

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
		 *================================================================================*/
		public static int NuevoGrupo(DateTime fecha, string notas, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			int idgruponuevo = -1;

			using (conexion) {

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
		 *================================================================================*/
		public static bool ExisteGrupo(DateTime fecha, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			Int32 numero = 0;

			using (conexion) {
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
		 *================================================================================*/
		 public static GrupoGraficos GetUltimoGrupo(OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
			GrupoGraficos grupo = null;

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
