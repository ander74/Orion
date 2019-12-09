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
using System.Data.SQLite;
using System.Linq;
using Orion.Interfaces;

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

        protected string CadenaConexion = "";

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
        #region MÉTODOS PRIVADOS
        // ====================================================================================================


        protected int GetDbVersion() {
            int version = 0;
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                using (var comando = new SQLiteCommand("PRAGMA user_version", conexion)) {
                    conexion.Open();
                    object resultado = comando.ExecuteScalar();
                    version = resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
                }
            }
            return version;
        }


        protected void SetDbVersion(int version) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                using (var comando = new SQLiteCommand($"PRAGMA user_version = {version}", conexion)) {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================






        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS GUARDAR
        // ====================================================================================================

        protected int GuardarItem<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return -1;
            if (item.Nuevo) {
                using var comando = new SQLiteCommand(item.ComandoInsertar, conexion);
                comando.Parameters.AddRange(item.Parametros.ToArray());
                comando.ExecuteNonQuery();
                using var comando2 = new SQLiteCommand(Identity, conexion);
                int id = Convert.ToInt32(comando2.ExecuteScalar());
                item.Id = id < 0 ? 0 : id;
            } else if (item.Modificado) {
                using var comando = new SQLiteCommand(item.ComandoActualizar, conexion);
                comando.Parameters.AddRange(item.Parametros.ToArray());
                comando.ExecuteNonQuery();
            }
            return item.Id;
        }


        protected int GuardarItemConLista<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return -1;
            int id = GuardarItem(item, conexion);
            if (item.Lista != null) {
                foreach (var item2 in item.Lista) {
                    item2.ForeignId = id;
                    GuardarItemConLista(item2, conexion);
                }
            }
            return item.Id;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GUARDAR
        // ====================================================================================================

        public int GuardarItem<T>(T item) where T : ISQLItem {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return GuardarItem(item, conexion);
        }


        public int GuardarItemConLista<T>(T item) where T : ISQLItem {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return GuardarItemConLista(item, conexion);
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS BORRAR
        // ====================================================================================================

        protected bool BorrarItem<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return false;
            int borrados = 0;
            using var comando = new SQLiteCommand($"DELETE * FROM {item.TableName} WHERE _id=@id;", conexion);
            comando.Parameters.AddWithValue("@id", item.Id);
            borrados = comando.ExecuteNonQuery();
            return borrados == 1;
        }


        protected bool BorrarItemConLista<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return false;
            bool borrado = BorrarItem(item, conexion);
            if (item.Lista != null) {
                foreach (var item2 in item.Lista) {
                    BorrarItemConLista(item2, conexion);
                }
            }
            return borrado;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS BORRAR
        // ====================================================================================================


        public bool BorrarItem<T>(T item) where T : ISQLItem {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return BorrarItem(item, conexion);
        }


        public bool BorrarItemConLista<T>(T item) where T : ISQLItem {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return BorrarItemConLista(item, conexion);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS GET
        // ====================================================================================================


        protected T GetItem<T>(int id, SQLiteConnection conexion) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            T item = new T();
            using var comando = new SQLiteCommand($"SELECT * FROM {item.TableName} WHERE _id=@id;", conexion);
            comando.Parameters.AddWithValue("@id", id);
            using var lector = comando.ExecuteReader();
            if (lector.Read()) {
                item.FromReader(lector);
                return item;
            }
            return default;
        }


        protected IEnumerable<T> GetItems<T>(SQLiteConnection conexion, string consulta, IEnumerable<SQLiteParameter> paramsWhere = null) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            List<T> lista = new List<T>();
            T item = new T();
            using var comando = new SQLiteCommand(consulta, conexion);
            if (paramsWhere != null) comando.Parameters.AddRange(paramsWhere.ToArray());
            using var lector = comando.ExecuteReader();
            while (lector.Read()) {
                item = new T();
                item.FromReader(lector);
                lista.Add(item);
            }
            return lista;
        }


        protected T GetItemConLista<T>(int id, SQLiteConnection conexion) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            T item = GetItem<T>(id, conexion);
            if (item.HasList) AddListaToItem(ref item, conexion);
            return item;
        }


        protected IEnumerable<T> GetItemsConLista<T>(SQLiteConnection conexion, string consulta, IEnumerable<SQLiteParameter> paramsWhere = null) where T : ISQLItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            List<T> lista = new List<T>();
            T item = new T();
            using var comando = new SQLiteCommand(consulta, conexion);
            if (paramsWhere != null) comando.Parameters.AddRange(paramsWhere.ToArray());
            using var lector = comando.ExecuteReader();
            while (lector.Read()) {
                item = new T();
                item.FromReader(lector);
                if (item.HasList) AddListaToItem(ref item, conexion);
                lista.Add(item);
            }
            return lista;
        }


        protected void AddListaToItem<T>(ref T item, SQLiteConnection conexion) where T : ISQLItem {
            var tipo = item.Lista.GetType().GetGenericArguments()[0];
            var item2 = (ISQLItem)Activator.CreateInstance(tipo);
            string where = $"{item.ForeignIdName} = @id";
            string order = item2.OrderBy.ToUpper().Replace("ORDER BY", "");
            using var comando = new SQLiteCommand($"SELECT * FROM {item2.TableName} WHERE {where} ORDER BY {order};", conexion);
            comando.Parameters.AddWithValue("@id", item.Id);
            using var lector = comando.ExecuteReader();
            item.InicializarLista();
            while (lector.Read()) {
                item2 = (ISQLItem)Activator.CreateInstance(tipo);
                item2.FromReader(lector);
                item.AddItemToList(item2);
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GET
        // ====================================================================================================


        public T GetItem<T>(int id) where T : ISQLItem, new() {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return GetItem<T>(id, conexion);
        }


        public IEnumerable<T> GetItems<T>(string whereCondition = "", string orderBy = "", IEnumerable<SQLiteParameter> paramsWhere = null) where T : ISQLItem, new() {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            var item = new T();
            string where = whereCondition == "" ? "" : $"WHERE {whereCondition}";
            string order = orderBy == "" ? "" : $"ORDER BY {orderBy}";
            string consulta = $"SELECT * FROM {item.TableName} {where} {order};";
            return GetItems<T>(conexion, consulta, paramsWhere);
        }


        public T GetItemConLista<T>(int id) where T : ISQLItem, new() {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return GetItemConLista<T>(id, conexion);
        }


        public IEnumerable<T> GetItemsConLista<T>(string whereCondition = "", string orderBy = "", IEnumerable<SQLiteParameter> paramsWhere = null) where T : ISQLItem, new() {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            var item = new T();
            string where = whereCondition == "" ? "" : $"WHERE {whereCondition}";
            string order = orderBy == "" ? "" : $"ORDER BY {orderBy}";
            string consulta = $"SELECT * FROM {item.TableName} {where} {order};";
            return GetItemsConLista<T>(conexion, consulta, paramsWhere);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS ESCALAR
        // ====================================================================================================

        protected object GetScalar(SQLiteConnection conexion, string consulta, IEnumerable<SQLiteParameter> parametros = null) {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            using var comando = new SQLiteCommand(consulta, conexion);
            if (parametros != null) comando.Parameters.AddRange(parametros.ToArray());
            var resultado = comando.ExecuteScalar();
            return resultado == DBNull.Value ? null : resultado;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS ESCALAR
        // ====================================================================================================

        public object GetScalar(string consulta, IEnumerable<SQLiteParameter> parametros = null) {
            using var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            return GetScalar(conexion, consulta, parametros);
        }


        public int GetIntScalar(string consulta, IEnumerable<SQLiteParameter> parametros = null) {
            var resultado = GetScalar(consulta, parametros);
            return resultado == null ? 0 : Convert.ToInt32(resultado);
        }


        public decimal GetDecimalScalar(string consulta, IEnumerable<SQLiteParameter> parametros = null) {
            var resultado = GetScalar(consulta, parametros);
            return resultado == null ? 0 : Convert.ToDecimal(resultado);
        }


        public TimeSpan GetTimeSpanScalar(string consulta, IEnumerable<SQLiteParameter> parametros = null) {
            var resultado = GetIntScalar(consulta, parametros);
            return new TimeSpan(resultado);
        }


        public int GetCount<T>(string whereCondition = "", IEnumerable<SQLiteParameter> parametros = null) where T : ISQLItem, new() {
            string where = whereCondition == "" ? "" : $"WHERE {whereCondition}";
            var item = new T();
            string consulta = $"SELECT Count(*) FROM {item.TableName} {where}";
            return GetIntScalar(consulta, parametros);
        }


        #endregion
        // ====================================================================================================



    }
}
