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
using System.Collections.ObjectModel;
using System.Data.OleDb;
using Orion.Config;
using System.Windows;

namespace Orion.DataModels {


	/// <summary>
	/// Contiene los métodos necesarios para trabajar con conductores en la base de datos.
	/// </summary>
	public static class BdConductores {


		/*================================================================================
		* GET CONDUCTORES
		*================================================================================*/
		public static List<Conductor> GetConductores() {

			// Creamos la lista y el comando que extrae los gráficos.
			List<Conductor> lista = new List<Conductor>();

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

				//string comandoSQL = "SELECT * FROM Conductores ORDER BY Id";

				string comandoSQL = "GetConductores";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.CommandType = System.Data.CommandType.StoredProcedure; //TODO: Eliminar tras probar.
				OleDbDataReader lector = null;

				try {
					conexion.Open();
					lector = Comando.ExecuteReader();

					while (lector.Read()) {
						Conductor conductor = new Conductor(lector);
						conductor.ListaRegulaciones = new NotifyCollection<RegulacionConductor>(BdRegulacionConductor.GetRegulaciones(conductor.Id));
						lista.Add(conductor);
						conductor.Nuevo = false;
						conductor.Modificado = false;
					}
				} catch (Exception ex) {
					Utils.VerError("BdConductores.GetConductores", ex);
				} finally {
					lector.Close();
				}
			}
			return lista;
		}


		/*================================================================================
		* GUARDAR CONDUCTORES
		*================================================================================*/
		public static void GuardarConductores(IEnumerable<Conductor> lista) {

			// Si la lista está vacía, salimos.
			if (lista == null || lista.Count() == 0) return;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

				//string SQLInsertar = "INSERT INTO Conductores (Nombre, Apellidos, Indefinido, Telefono, Email, Acumuladas, Descansos, " +
				//				 "DescansosNoDisfrutados, PlusDistancia, Notas) " +
				//				 "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

				//string SQLActualizar = "UPDATE Conductores SET Id=?, Nombre=?, Apellidos=?, Indefinido=?, " +
				//					   "Telefono=?, Email=?, Acumuladas=?, Descansos=?, DescansosNoDisfrutados=?, PlusDistancia=?, Notas=? WHERE Id=?";

				string SQLInsertar = "InsertarConductor";
				string SQLActualizar = "ActualizarConductor";

				try {
					conexion.Open();

					foreach (Conductor conductor in lista) {
						if (conductor.Nuevo) {
							// Si el conductor ya existe, saltarselo.
							if (ExisteConductor(conductor.Id)) {
								conductor.Id = 0;
								continue;
							}
							OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;
							Conductor.ParseToCommand(comando, conductor);
							comando.ExecuteNonQuery();
							foreach (RegulacionConductor regulacion in conductor.ListaRegulaciones) {
								regulacion.IdConductor = conductor.Id;
							}
							conductor.Nuevo = false;
							conductor.Modificado = false;
						} else if (conductor.Modificado) {
							OleDbCommand comando = new OleDbCommand(SQLActualizar, conexion);
							comando.CommandType = System.Data.CommandType.StoredProcedure;//TODO: Eliminar tras probar.
							Conductor.ParseToCommand(comando, conductor);
							comando.ExecuteNonQuery();
							conductor.Modificado = false;
						}
						BdRegulacionConductor.GuardarRegulaciones(conductor.ListaRegulaciones.Where(r => r.Nuevo || r.Modificado));
					}
				} catch (Exception ex) {
					Utils.VerError("BdConductores.GuardarConductores", ex);
				}
			}
		}


		/*================================================================================
		 * BORRAR CONDUCTORES
		 *================================================================================*/
		public static void BorrarConductores(IEnumerable<Conductor> lista) {

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

				//string SQLBorrar = "DELETE FROM Conductores WHERE Id=?";
				string SQLBorrar = "BorrarConductor";

				try {
					conexion.Open();

					foreach (Conductor conductor in lista) {
						OleDbCommand comando = new OleDbCommand(SQLBorrar, conexion);
						comando.CommandType = System.Data.CommandType.StoredProcedure;
						comando.Parameters.AddWithValue("id", conductor.Id);
						comando.ExecuteNonQuery();
					}
				} catch (Exception ex) {
					Utils.VerError("BdConductores.BorrarConductores", ex);
				}
			}
		}


		/*================================================================================
		 * EXISTE CONDUCTOR
		 *================================================================================*/
		public static bool ExisteConductor(int idconductor) {

			int numero = 0;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

				// Creamos el comando que lee el número de conductores que tienen el id
				//string comandoSQL = "SELECT Count(Id) FROM Conductores WHERE Id = ?";
				string comandoSQL = "ExisteConductor";
				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.CommandType = System.Data.CommandType.StoredProcedure;
				Comando.Parameters.AddWithValue("@Id", idconductor);
				try {
					conexion.Open();
					// Ejecutamos el comando.
					numero = (int)Comando.ExecuteScalar();
				} catch (Exception ex) {
					Utils.VerError("BdConductores.ExisteConductor", ex);
				}
			}
			// Devolvemos true si hay algún conductor.
			return numero > 0;
		}


		/*================================================================================
		 * INSERTAR CONDUCTOR DESCONOCIDO
		 *================================================================================*/
		public static void InsertarConductorDesconocido(int idconductor) {

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{
				//string SQLInsertar = "INSERT INTO Conductores (Id, Nombre) VALUES (?, 'Desconocido')";
				string SQLInsertar = "InsertarConductorDesconocido";
				OleDbCommand comando = new OleDbCommand(SQLInsertar, conexion);
				comando.CommandType = System.Data.CommandType.StoredProcedure;
				comando.Parameters.AddWithValue("@Id", idconductor);
				try {
					conexion.Open();
					comando.ExecuteNonQuery();
				} catch (Exception ex) {
					Utils.VerError("BdConductores.InsertarConductorDesconocido", ex);
				}
			}
		}


		/*================================================================================
		 * GET CONDUCTOR
		 *================================================================================*/
		 public static Conductor GetConductor(int idconductor) {

			Conductor conductor = null;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion))
			{

				// Creamos el comando que extrae el conductor
				//string comandoSQL = "SELECT * FROM Conductores WHERE Id = ?";
				string comandoSQL = "GetConductor";

				OleDbCommand Comando = new OleDbCommand(comandoSQL, conexion);
				Comando.CommandType = System.Data.CommandType.StoredProcedure;
				Comando.Parameters.AddWithValue("@Id", idconductor);
				OleDbDataReader lector = null;
				try {
					conexion.Open();
					lector = Comando.ExecuteReader();

					if (lector.Read()) {
						conductor = new Conductor(lector);
						conductor.ListaRegulaciones = new NotifyCollection<RegulacionConductor>(BdRegulacionConductor.GetRegulaciones(conductor.Id));
					}
				} catch (Exception ex) {
					Utils.VerError("BdConductores.GetConductor", ex);
				} finally {
					lector.Close();
				}
			}
			return conductor;

		}




	} //Final de clase
}
