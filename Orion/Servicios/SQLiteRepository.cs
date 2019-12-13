#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;
using Orion.Interfaces;
using Orion.Models;

namespace Orion.Servicios {

    public class SQLiteRepository {


        // ====================================================================================================
        #region CONSTANTES
        // ====================================================================================================

        public const string Identity = "SELECT last_insert_rowid()";


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public SQLiteRepository(string cadenaConexion) {
            this.CadenaConexion = cadenaConexion;
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public string CadenaConexion { get; set; }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        protected int GetDbVersion() {
            return GetIntScalar(new SQLiteExpression("PRAGMA user_version")); ;
        }


        protected void SetDbVersion(int version) {
            ExecureNonQuery(new SQLiteExpression($"PRAGMA user_version = { version }"));
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================






        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS NON QUERY
        // ====================================================================================================

        public void ExecureNonQuery(SQLiteExpression consulta) {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                using (var comando = consulta.GetCommand(conexion)) {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS GUARDAR
        // ====================================================================================================

        protected int GuardarItem<T>(SqliteConnection conexion, T item, bool ignorarLista = false) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open || item == null) return -1;
            if (item.Nuevo) {
                using (var comando = new SqliteCommand(item.ComandoInsertar, conexion)) {
                    comando.Parameters.AddRange(item.Parametros);
                    comando.ExecuteNonQuery();
                    using (var comando2 = new SqliteCommand(Identity, conexion)) {
                        int id = Convert.ToInt32(comando2.ExecuteScalar());
                        item.Id = id < 0 ? 0 : id;
                    }
                }
            } else if (item.Modificado) {
                using (var comando = new SqliteCommand(item.ComandoActualizar, conexion)) {
                    comando.Parameters.AddRange(item.Parametros);
                    comando.ExecuteNonQuery();
                }
            }
            if (!ignorarLista && item.HasList) {
                foreach (var item2 in item.Lista) {
                    item2.ForeignId = item.Id;
                    GuardarItem(conexion, item2, ignorarLista);
                }
            }
            return item.Id;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GUARDAR
        // ====================================================================================================

        public int GuardarItem<T>(T item, bool ignorarLista = false) where T : ISQLItem {
            if (item == null) return -1;
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GuardarItem(conexion, item, ignorarLista);
            }
        }


        protected void GuardarItems<T>(IEnumerable<T> lista, bool ignorarLista = false) where T : ISQLItem {
            if (lista == null) return;
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                foreach (var item in lista) {
                    GuardarItem(conexion, item, ignorarLista);
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS BORRAR
        // ====================================================================================================

        protected bool BorrarItem<T>(SqliteConnection conexion, T item, bool ignorarLista = false) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open || item == null) return false;
            int borrados = 0;
            using (var comando = new SqliteCommand($"DELETE * FROM {item.TableName} WHERE _id=@id;", conexion)) {
                comando.Parameters.AddWithValue("@id", item.Id);
                borrados = comando.ExecuteNonQuery();
                if (!ignorarLista && item.HasList) {
                    foreach (var item2 in item.Lista) {
                        BorrarItem(conexion, item2, ignorarLista);
                    }
                }
            }
            return borrados == 1;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS BORRAR
        // ====================================================================================================


        public bool BorrarItem<T>(T item, bool ignorarLista = false) where T : ISQLItem {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return BorrarItem(conexion, item, ignorarLista);
            }
        }


        public bool BorrarItems<T>(IEnumerable<T> lista, bool ignorarLista = false) where T : ISQLItem {
            if (lista == null) return false;
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                int borrados = 0;
                foreach (var item in lista) {
                    if (BorrarItem(conexion, item, ignorarLista)) borrados++;
                }
                return borrados == lista.Count();
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS GET
        // ====================================================================================================

        protected T GetItem<T>(SqliteConnection conexion, int id, bool ignorarLista = false) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            T item = new T();
            using (var comando = new SqliteCommand($"SELECT * FROM {item.TableName} WHERE _id=@id;", conexion)) {
                comando.Parameters.AddWithValue("@id", id);
                using (var lector = comando.ExecuteReader()) {
                    if (lector.Read()) {
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        return item;
                    }
                }
            }
            return default;
        }


        protected T GetItem<T>(SqliteConnection conexion, SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    if (lector.Read()) {
                        T item = new T();
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        return item;
                    }
                }
            }
            return default;
        }


        protected IEnumerable<T> GetItems<T>(SqliteConnection conexion, SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            List<T> lista = new List<T>();
            T item = new T();
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    while (lector.Read()) {
                        item = new T();
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        lista.Add(item);
                    }
                }
            }
            return lista;
        }


        protected void AddListaToItem<T>(SqliteConnection conexion, ref T item) where T : ISQLItem {
            if (item == null) return;
            var tipo = item.Lista.GetType().GetGenericArguments()[0];
            var item2 = (ISQLItem)Activator.CreateInstance(tipo);
            var consulta = new SQLiteExpression($"SELECT * FROM {item2.TableName} WHERE {item.ForeignIdName} = @id ORDER BY {item2.OrderBy};").AddParameter("@id", item.Id);
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    item.InicializarLista();
                    while (lector.Read()) {
                        item2 = (ISQLItem)Activator.CreateInstance(tipo);
                        item2.FromReader(lector);
                        item.AddItemToList(item2);
                    }
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GET
        // ====================================================================================================


        public T GetItem<T>(int id, bool ignorarLista = false) where T : ISQLItem, new() {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetItem<T>(conexion, id, ignorarLista);
            }
        }


        public T GetItem<T>(SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLItem, new() {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetItem<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItems<T>(bool ignorarLista = false) where T : ISQLItem, new() {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                var item = new T();
                var consulta = new SQLiteExpression($"SELECT * FROM {item.TableName} ORDER BY {item.OrderBy};");
                return GetItems<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItems<T>(SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLItem, new() {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                var item = new T();
                return GetItems<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItemsWhere<T>(SQLiteExpression whereCondition = null, string orderBy = "", bool ignorarLista = false) where T : ISQLItem, new() {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                var item = new T();
                string where = whereCondition == null ? "" : $"WHERE {whereCondition.Expresion}";
                string order = orderBy == "" ? "" : $"ORDER BY {orderBy}";
                var consulta = new SQLiteExpression($"SELECT * FROM {item.TableName} {where} {order};", whereCondition.Parametros);
                return GetItems<T>(conexion, consulta, ignorarLista);
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS ESCALAR
        // ====================================================================================================

        protected object GetScalar(SqliteConnection conexion, SQLiteExpression consulta) {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            using (var comando = consulta.GetCommand(conexion)) {
                var resultado = comando.ExecuteScalar();
                return resultado == DBNull.Value ? null : resultado;
            }
        }


        protected int GetIntScalar(SqliteConnection conexion, SQLiteExpression consulta) {
            var resultado = GetScalar(conexion, consulta);
            return resultado == null ? 0 : Convert.ToInt32(resultado);
        }


        protected decimal GetDecimalScalar(SqliteConnection conexion, SQLiteExpression consulta) {
            var resultado = GetScalar(conexion, consulta);
            return resultado == null ? 0 : Convert.ToDecimal(resultado);
        }


        protected TimeSpan GetTimeSpanScalar(SqliteConnection conexion, SQLiteExpression consulta) {
            if (conexion == null || conexion.State != ConnectionState.Open) return TimeSpan.Zero;
            var resultado = GetIntScalar(conexion, consulta);
            return new TimeSpan(resultado);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS ESCALAR
        // ====================================================================================================

        public object GetScalar(SQLiteExpression consulta) {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetScalar(conexion, consulta);
            }
        }


        public int GetIntScalar(SQLiteExpression consulta) {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetIntScalar(conexion, consulta);
            }
        }


        public decimal GetDecimalScalar(SQLiteExpression consulta) {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetDecimalScalar(conexion, consulta);
            }
        }


        public TimeSpan GetTimeSpanScalar(SQLiteExpression consulta) {
            using (var conexion = new SqliteConnection(CadenaConexion)) {
                conexion.Open();
                return GetTimeSpanScalar(conexion, consulta);
            }
        }


        public int GetCount<T>(SQLiteExpression whereCondition = null) where T : ISQLItem, new() {
            string where = whereCondition == null ? "" : $"WHERE {whereCondition.Expresion}";
            var item = new T();
            var consulta = new SQLiteExpression($"SELECT Count(*) FROM {item.TableName} {where}", whereCondition.Parametros);
            return GetIntScalar(consulta);
        }


        #endregion
        // ====================================================================================================



    }
}
