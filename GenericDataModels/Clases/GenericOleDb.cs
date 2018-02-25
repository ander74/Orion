#region COPYRIGHT
// ===========================================================
//     Copyright 2018 - GenericDataModels 1.0 - A. Herrero    
// -----------------------------------------------------------
//        Vea el archivo Licencia.txt para más detalles 
// ===========================================================
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDataModels {


	/// <summary>
	/// Esta clase provee métodos para el intercambio de datos entre objetos y un proveedor de datos OleDb.
	/// Los objetos deberán implementar la interfaz IModelOleDb o sus derivadas para poder ser utilizados por
	/// los métodos de la clase.
	/// </summary>
	public class GenericOleDb {


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================

		/// <summary>
		/// Inicializa una nueva instancia de la clase GenericDataModels.GenericOleDb.
		/// </summary>
		public GenericOleDb(string cadenaConexion) {
			_cadenaconexion = cadenaConexion;
		}

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================
		public event ErrorHandler ErrorProducido;

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region MÉTODOS GET MODELS
		// ====================================================================================================

		/// <summary>
		/// Devuelve una lista del modelo especificado en T extraido de la base de datos por medio del comando SQL.
		/// </summary>
		public ObservableCollection<T> GetModelos<T>(string comandoSql) where T : IModelOleDb, new() {

			ObservableCollection<T> lista = new ObservableCollection<T>();
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					conexion.Open();
					OleDbDataReader lector = comando.ExecuteReader();
					if (lector.HasRows) {
						while (lector.Read()) {
							T modelo = new T();
							modelo.SetDataFromReader(lector);
							lista.Add(modelo);
							modelo.Nuevo = false;
							modelo.Modificado = false;
						}
					}
					lector.Close();
				} catch (Exception ex) {
					string localizacion = $"GetModelos<{typeof(T).Name}> => {comandoSql}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return lista;
		}


		/// <summary>
		/// Devuelve una lista del modelo especificado en T extraido de la base de datos por medio del comando SQL.
		/// Los modelos deben estar relacionados por medio del Id relacionado.
		/// </summary>
		public ObservableCollection<T> GetModelosPorIdRelacionado<T>(string comandoSql, int idRelacionado) where T : IModelOleDb, new() {

			ObservableCollection<T> lista = new ObservableCollection<T>();
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					comando.Parameters.AddWithValue("@IdRelacionado", idRelacionado);
					conexion.Open();
					OleDbDataReader lector = comando.ExecuteReader();
					if (lector.HasRows) {
						while (lector.Read()) {
							T modelo = new T();
							modelo.SetDataFromReader(lector);
							lista.Add(modelo);
							modelo.Nuevo = false;
							modelo.Modificado = false;
						}
					}
					lector.Close();
				} catch (Exception ex) {
					string localizacion = $"GetModelosPorIdRelacionado<{typeof(T).Name}> => {comandoSql}, Id={idRelacionado}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return lista;
		}


		/// <summary>
		/// Devuelve una lista del modelo especificado en T de la base de datos por medio del comando SQL.
		/// T, a su vez, contiene una lista de TL que también será extraida.
		/// </summary>
		public ObservableCollection<T> GetModelosConLista<T, TL>(string comandoSql, string comandoSqlLista)
																		where T : IModelOleDbConLista, new() where TL : IModelOleDb, new() {

			ObservableCollection<T> lista = new ObservableCollection<T>();
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					conexion.Open();
					OleDbDataReader lector = comando.ExecuteReader();
					if (lector.HasRows) {
						while (lector.Read()) {
							T modeloLista = new T();
							modeloLista.SetDataFromReader(lector);
							modeloLista.ListaInterna = GetModelosPorIdRelacionado<TL>(comandoSqlLista, modeloLista.IdRelacionado)
																													as ObservableCollection<IModelOleDb>;
							lista.Add(modeloLista);
							modeloLista.Nuevo = false;
							modeloLista.Modificado = false;
						}
					}
					lector.Close();
				} catch (Exception ex) {
					string localizacion = $"GetModelosConLista<{typeof(T).Name},{typeof(TL).Name}> => {comandoSql}, {comandoSqlLista}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return lista;
		}


		/// <summary>
		/// Devuelve una lista del modelo especificado en T extraido de la base de datos por medio del comando SQL.
		/// T, a su vez, contiene una lista de TL que también será extraida.
		/// Los modelos deben estar relacionados por medio del Id relacionado.
		/// </summary>
		public ObservableCollection<T> GetModelosConListaPorIdRelacionado<T, TL>(string comandoSql, string comandoSqlLista, int idRelacionado)
																						where T : IModelOleDbConLista, new() where TL : IModelOleDb, new() {

			ObservableCollection<T> lista = new ObservableCollection<T>();
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					comando.Parameters.AddWithValue("@IdRelacionado", idRelacionado);
					conexion.Open();
					OleDbDataReader lector = comando.ExecuteReader();
					if (lector.HasRows) {
						while (lector.Read()) {
							T modeloLista = new T();
							modeloLista.SetDataFromReader(lector);
							modeloLista.ListaInterna = GetModelosPorIdRelacionado<TL>(comandoSqlLista, modeloLista.IdRelacionado)
																													as ObservableCollection<IModelOleDb>;
							lista.Add(modeloLista);
							modeloLista.Nuevo = false;
							modeloLista.Modificado = false;
						}
					}
					lector.Close();
				} catch (Exception ex) {
					string localizacion = $"GetModelosPorIdRelacionado<{typeof(T).Name}> => {comandoSql}, Id={idRelacionado}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return lista;
		}


		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region MÉTODOS GUARDAR MODELOS
		// ====================================================================================================

		/// <summary>
		/// Inserta o actualiza los modelos de tipo T de la lista pasada usando los comandos insertar y actualizar pasados.
		/// </summary>
		public void GuardarModelos<T>(ObservableCollection<T> lista, string comandoInsertar, string comandoActualizar) where T : IModelOleDb, new() {

			if (lista == null || lista.Count == 0) return;
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand { Connection = conexion, CommandType = CommandType.StoredProcedure };
					conexion.Open();
					foreach (T modelo in lista) {
						if (modelo.Nuevo) {
							comando.CommandText = comandoInsertar;
							modelo.GetDataToCommand(ref comando);
							comando.ExecuteNonQuery();
							comando.CommandText = "SELECT @@IDENTITY;";
							comando.CommandType = CommandType.Text;
							int idNuevo = (int)comando.ExecuteScalar();
							modelo.Id = idNuevo;
							modelo.Nuevo = false;
							modelo.Modificado = false;
						} else if (modelo.Modificado) {
							comando.CommandText = comandoActualizar;
							modelo.GetDataToCommand(ref comando);
							comando.ExecuteNonQuery();
							modelo.Modificado = false;
						}
					}
				} catch (Exception ex) {
					string localizacion = $"GuardarModelos<{typeof(T).Name}> => {comandoInsertar}, {comandoActualizar}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
		}


		/// <summary>
		/// Inserta o actualiza los modelos de tipo T de la lista pasada usando los comandos insertar y actualizar pasados.
		/// T, a su vez, contiene una lista en la que se actualizará el Id relacionado al id nuevo del modelo T.
		/// </summary>
		public void GuardarModelosConLista<T, TL>(ObservableCollection<T> lista, string comandoInsertar, string comandoActualizar)
																					where T : IModelOleDbConLista, new() where TL : IModelOleDb, new() {

			if (lista == null || lista.Count == 0) return;
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand { Connection = conexion, CommandType = CommandType.StoredProcedure };
					conexion.Open();
					foreach (T modelo in lista) {
						if (modelo.Nuevo) {
							comando.CommandText = comandoInsertar;
							modelo.GetDataToCommand(ref comando);
							comando.ExecuteNonQuery();
							comando.CommandText = "SELECT @@IDENTITY;";
							comando.CommandType = CommandType.Text;
							int idNuevo = (int)comando.ExecuteScalar();
							foreach (TL modeloLista in modelo.ListaInterna) {
								modeloLista.IdRelacionado = idNuevo;
							}
							modelo.Id = idNuevo;
							modelo.Nuevo = false;
							modelo.Modificado = false;
						} else if (modelo.Modificado) {
							comando.CommandText = comandoActualizar;
							modelo.GetDataToCommand(ref comando);
							comando.ExecuteNonQuery();
							modelo.Modificado = false;
						}
					}
				} catch (Exception ex) {
					string localizacion = $"GuardarModelos<{typeof(T).Name}> => {comandoInsertar}, {comandoActualizar}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
		}



		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region MÉTODOS GET VALOR
		// ====================================================================================================

		/// <summary>
		/// Devuelve un objeto con el resultado de una consulta sql pasando una lista indeterminada de parámetros.
		/// </summary>
		public object GetValor<T>(string comandoSql, params T[] parametros) {

			object resultado = null;
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					if (parametros != null) {
						for (int i = 0; i < parametros.Length; i++) {
							comando.Parameters.AddWithValue($"@Param{i}", parametros[i]);
						}
					}
					conexion.Open();
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					string localizacion = $"GetValorHastaMes<{typeof(T).Name}> => {comandoSql}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return resultado == DBNull.Value ? null : resultado;
		}


		/// <summary>
		/// Devuelve un objeto con el resultado de una consulta sql pasando dos parámetros de distinto tipo.
		/// </summary>
		public object GetValor<T, TT>(string comandoSql, T param1, TT param2) {

			object resultado = null;
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					comando.Parameters.AddWithValue("@Param1", param1);
					comando.Parameters.AddWithValue("@Param2", param2);
					conexion.Open();
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					string localizacion = $"GetValorHastaMes<{typeof(T).Name}> => {comandoSql}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return resultado == DBNull.Value ? null : resultado;
		}


		/// <summary>
		/// Devuelve un objeto con el resultado de una consulta sql pasando tres parámetros de distinto tipo.
		/// </summary>
		public object GetValor<T, TT, TTT>(string comandoSql, T param1, TT param2, TTT param3) {

			object resultado = null;
			using (OleDbConnection conexion = new OleDbConnection(CadenaConexion)) {
				try {
					OleDbCommand comando = new OleDbCommand(comandoSql, conexion) { CommandType = CommandType.StoredProcedure };
					comando.Parameters.AddWithValue("@Param1", param1);
					comando.Parameters.AddWithValue("@Param2", param2);
					comando.Parameters.AddWithValue("@Param3", param3);
					conexion.Open();
					resultado = comando.ExecuteScalar();
				} catch (Exception ex) {
					string localizacion = $"GetValorHastaMes<{typeof(T).Name}> => {comandoSql}";
					ErrorProducido?.Invoke(this, new ErrorProducidoEventArgs(localizacion, ex));
				}
			}
			return resultado == DBNull.Value ? null : resultado;
		}



		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private string _cadenaconexion;
		public string CadenaConexion {
			get { return _cadenaconexion; }
			set {
				if (_cadenaconexion != value) {
					_cadenaconexion = value;
				}
			}
		}


		#endregion
		// ====================================================================================================






	}
}
