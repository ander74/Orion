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


        protected string GetInsertCommand(string tableName, IEnumerable<SQLiteParameter> parametros) {
            if (parametros == null || parametros.Count() == 0) return "";
            string campos = "";
            string valores = "";
            foreach (var parametro in parametros) {
                campos += $"{parametro.ParameterName}, ";
                valores += $"@{parametro.ParameterName.ToLower()}, ";
            }
            campos = campos.Substring(0, campos.Length - 2);
            valores = valores.Substring(0, valores.Length - 2);
            return $"INSERT INTO {tableName} ({campos}) VALUES ({valores});";
        }


        protected string GetInsertCommand<T>(T item) where T : ISQLiteItem {
            if (item.Parametros == null || item.Parametros.Count() == 0) return "";
            string campos = "";
            string valores = "";
            foreach (var parametro in item.Parametros) {
                campos += $"{parametro.ParameterName}, ";
                valores += $"@{parametro.ParameterName.ToLower()}, ";
            }
            campos = campos.Substring(0, campos.Length - 2);
            valores = valores.Substring(0, valores.Length - 2);
            return $"INSERT INTO {item.TableName} ({campos}) VALUES ({valores});";
        }


        protected string GetUpdateCommand(string tableName, IEnumerable<SQLiteParameter> parametros) {
            if (parametros == null || parametros.Count() == 0) return "";
            string datos = "";
            foreach (var parametro in parametros) {
                datos += $"{parametro.ParameterName} = @{parametro.ParameterName.ToLower()}, ";
            }
            datos = datos.Substring(0, datos.Length - 2);
            return $"UPDATE {tableName} SET {datos} WHERE _id = @id;";
        }


        protected string GetUpdateCommand<T>(T item) where T : ISQLiteItem {
            if (item.Parametros == null || item.Parametros.Count() == 0) return "";
            string datos = "";
            foreach (var parametro in item.Parametros) {
                datos += $"{parametro.ParameterName} = @{parametro.ParameterName.ToLower()}, ";
            }
            datos = datos.Substring(0, datos.Length - 2);
            return $"UPDATE {item.TableName} SET {datos} WHERE _id = @id;";
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
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
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

        protected int GuardarItem<T>(SQLiteConnection conexion, T item, bool ignorarLista = false) where T : ISQLiteItem {
            if (conexion == null || conexion.State != ConnectionState.Open || item == null) return -1;
            if (item.Id == 0) {
                using (var comando = new SQLiteCommand(GetInsertCommand(item), conexion)) {
                    comando.Parameters.AddRange(item.Parametros.ToArray());
                    comando.ExecuteNonQuery();
                    using (var comando2 = new SQLiteCommand(Identity, conexion)) {
                        int id = Convert.ToInt32(comando2.ExecuteScalar());
                        item.Id = id < 0 ? 0 : id;
                    }
                }
            } else {
                using (var comando = new SQLiteCommand(GetUpdateCommand(item), conexion)) {
                    comando.Parameters.AddRange(item.Parametros.ToArray());
                    comando.Parameters.AddWithValue("@id", item.Id);
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


        //protected int GuardarItem<T>(SQLiteConnection conexion, T item, bool ignorarLista = false) where T : ISQLiteItem {
        //    if (conexion == null || conexion.State != ConnectionState.Open || item == null) return -1;
        //    //var consulta = GetUpdateCommand(item.TableName, item.Parametros);
        //    using (var comando = new SQLiteCommand(item.ComandoInsertar, conexion)) {
        //        comando.Parameters.AddRange(item.Parametros.ToArray());
        //        comando.ExecuteNonQuery();
        //    }
        //    using (var comando2 = new SQLiteCommand(Identity, conexion)) {
        //        int id = Convert.ToInt32(comando2.ExecuteScalar());
        //        item.Id = id < 0 ? 0 : id;
        //    }
        //    if (!ignorarLista && item.HasList && item.Lista != null) {
        //        foreach (var item2 in item.Lista) {
        //            item2.ForeignId = item.Id;
        //            GuardarItem(conexion, item2, ignorarLista);
        //        }
        //    }
        //    return item.Id;
        //}


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GUARDAR
        // ====================================================================================================

        public int GuardarItem<T>(T item, bool ignorarLista = false) where T : ISQLiteItem {
            if (item == null) return -1;
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GuardarItem(conexion, item, ignorarLista);
            }
        }


        public void GuardarItems<T>(IEnumerable<T> lista, bool ignorarLista = false) where T : ISQLiteItem {
            DateTime d1 = DateTime.Now;
            if (lista == null) return;
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                foreach (var item in lista) {
                    GuardarItem(conexion, item, ignorarLista);
                }
            }
            DateTime d2 = DateTime.Now;
            var tiempo = d2 - d1;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS BORRAR
        // ====================================================================================================

        protected bool BorrarItem<T>(SQLiteConnection conexion, T item, bool ignorarLista = false) where T : ISQLiteItem {
            if (conexion == null || conexion.State != ConnectionState.Open || item == null) return false;
            int borrados = 0;
            using (var comando = new SQLiteCommand($"DELETE FROM {item.TableName} WHERE _id=@id;", conexion)) {
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


        public bool BorrarItem<T>(T item, bool ignorarLista = false) where T : ISQLiteItem {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return BorrarItem(conexion, item, ignorarLista);
            }
        }


        public bool BorrarItems<T>(IEnumerable<T> lista, bool ignorarLista = false) where T : ISQLiteItem {
            if (lista == null) return false;
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
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

        protected T GetItem<T>(SQLiteConnection conexion, int id, bool ignorarLista = false) where T : ISQLiteItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            T item = new T();
            using (var comando = new SQLiteCommand($"SELECT * FROM {item.TableName} WHERE _id=@id;", conexion)) {
                comando.Parameters.AddWithValue("@id", id);
                using (var lector = comando.ExecuteReader()) {
                    if (lector.Read()) {
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        item.Nuevo = false;
                        item.Modificado = false;
                        return item;
                    }
                }
            }
            return default;
        }


        protected T GetItem<T>(SQLiteConnection conexion, SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLiteItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    if (lector.Read()) {
                        T item = new T();
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        item.Nuevo = false;
                        item.Modificado = false;
                        return item;
                    }
                }
            }
            return default;
        }


        protected IEnumerable<T> GetItems<T>(SQLiteConnection conexion, SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLiteItem, new() {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            List<T> lista = new List<T>();
            T item = new T();
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    while (lector.Read()) {
                        item = new T();
                        item.FromReader(lector);
                        if (!ignorarLista && item.HasList) AddListaToItem(conexion, ref item);
                        item.Nuevo = false;
                        item.Modificado = false;
                        lista.Add(item);
                    }
                }
            }
            return lista;
        }


        protected void AddListaToItem<T>(SQLiteConnection conexion, ref T item) where T : ISQLiteItem {
            if (item == null) return;
            item.InicializarLista();
            var tipo = item.Lista.GetType().GetGenericArguments()[0];
            var item2 = (ISQLiteItem)Activator.CreateInstance(tipo);
            var consulta = new SQLiteExpression($"SELECT * FROM {item2.TableName} WHERE {item.ForeignIdName} = @id ORDER BY {item2.OrderBy};").AddParameter("@id", item.Id);
            using (var comando = consulta.GetCommand(conexion)) {
                using (var lector = comando.ExecuteReader()) {
                    item.InicializarLista();
                    while (lector.Read()) {
                        item2 = (ISQLiteItem)Activator.CreateInstance(tipo);
                        item2.FromReader(lector);
                        if (item2.HasList) AddListaToItem(conexion, ref item2);
                        item2.Nuevo = false;
                        item2.Modificado = false;
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


        public T GetItem<T>(int id, bool ignorarLista = false) where T : ISQLiteItem, new() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetItem<T>(conexion, id, ignorarLista);
            }
        }


        public T GetItem<T>(SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLiteItem, new() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetItem<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItems<T>(bool ignorarLista = false) where T : ISQLiteItem, new() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                var item = new T();
                var consulta = new SQLiteExpression($"SELECT * FROM {item.TableName} ORDER BY {item.OrderBy};");
                return GetItems<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItems<T>(SQLiteExpression consulta, bool ignorarLista = false) where T : ISQLiteItem, new() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                //var item = new T();
                return GetItems<T>(conexion, consulta, ignorarLista);
            }
        }


        public IEnumerable<T> GetItemsWhere<T>(SQLiteExpression whereCondition = null, string orderBy = "", bool ignorarLista = false) where T : ISQLiteItem, new() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
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

        protected object GetScalar(SQLiteConnection conexion, SQLiteExpression consulta) {
            if (conexion == null || conexion.State != ConnectionState.Open) return default;
            using (var comando = consulta.GetCommand(conexion)) {
                var resultado = comando.ExecuteScalar();
                return resultado == DBNull.Value ? null : resultado;
            }
        }


        protected int GetIntScalar(SQLiteConnection conexion, SQLiteExpression consulta) {
            var resultado = GetScalar(conexion, consulta);
            return resultado == null ? 0 : Convert.ToInt32(resultado);
        }


        protected long GetLongScalar(SQLiteConnection conexion, SQLiteExpression consulta) {
            var resultado = GetScalar(conexion, consulta);
            return resultado == null ? 0 : Convert.ToInt64(resultado);
        }


        protected decimal GetDecimalScalar(SQLiteConnection conexion, SQLiteExpression consulta) {
            var resultado = GetScalar(conexion, consulta);
            return resultado == null ? 0 : Convert.ToDecimal(resultado);
        }


        protected TimeSpan GetTimeSpanScalar(SQLiteConnection conexion, SQLiteExpression consulta) {
            if (conexion == null || conexion.State != ConnectionState.Open) return TimeSpan.Zero;
            var resultado = GetLongScalar(conexion, consulta);
            return new TimeSpan(resultado);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS ESCALAR
        // ====================================================================================================

        public object GetScalar(SQLiteExpression consulta) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetScalar(conexion, consulta);
            }
        }


        public int GetIntScalar(SQLiteExpression consulta) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetIntScalar(conexion, consulta);
            }
        }


        public long GetLongScalar(SQLiteExpression consulta) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetLongScalar(conexion, consulta);
            }
        }


        public decimal GetDecimalScalar(SQLiteExpression consulta) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetDecimalScalar(conexion, consulta);
            }
        }


        public TimeSpan GetTimeSpanScalar(SQLiteExpression consulta) {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                return GetTimeSpanScalar(conexion, consulta);
            }
        }


        public int GetCount<T>(SQLiteExpression whereCondition) where T : ISQLiteItem, new() {
            string where = whereCondition == null ? "" : $"WHERE {whereCondition.Expresion}";
            var item = new T();
            var consulta = new SQLiteExpression($"SELECT Count(*) FROM {item.TableName} {where}", whereCondition.Parametros);
            return GetIntScalar(consulta);
        }


        public int GetCount<T>() where T : ISQLiteItem, new() {
            var item = new T();
            var consulta = new SQLiteExpression($"SELECT Count(*) FROM {item.TableName}");
            return GetIntScalar(consulta);
        }


        #endregion
        // ====================================================================================================



    }
}
