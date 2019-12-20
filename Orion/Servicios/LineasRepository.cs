﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Orion.Config;
using Orion.Models;

namespace Orion.Servicios {

    public class LineasRepository : SQLiteRepository {


        // ====================================================================================================
        #region CREACIÓN DE TABLAS
        // ====================================================================================================


        public const string CrearTablaLineas = "CREATE TABLE IF NOT EXISTS Lineas (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nombre TEXT DEFAULT '', " +
            "Descripcion TEXT DEFAULT '' " +
            ");";


        public const string CrearTablaItinerarios = "CREATE TABLE IF NOT EXISTS Itinerarios (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdLinea INTEGER DEFAULT 0, " +
            "Nombre REAL DEFAULT 0, " +
            "Descripcion TEXT DEFAULT '', " +
            "TiempoReal INTEGER DEFAULT 0, " +
            "TiempoPago INTEGER DEFAULT 0 " +
            ");";


        public const string CrearTablaParadas = "CREATE TABLE IF NOT EXISTS Paradas (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdItinerario INTEGER DEFAULT 0, " +
            "Orden INTEGER DEFAULT 0, " +
            "Descripcion TEXT DEFAULT '', " +
            "Tiempo INTEGER DEFAULT 0 " +
            ");";


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSULTAS SQLITE
        // ====================================================================================================



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private const int DB_VERSION = 1;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public LineasRepository(string cadenaConexion) : base(cadenaConexion) {
            InicializarBasesDatos();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private void CrearDBs() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                using (var comando = new SQLiteCommand(CrearTablaLineas, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaItinerarios, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaParadas, conexion)) comando.ExecuteNonQuery();
            }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Crea el archivo de base de datos si no existe.
        /// En caso de que la versión de la base de datos no sea correcta, crea o modifica las tablas en consecuencia.
        /// </summary>
        public void InicializarBasesDatos() {
            switch (GetDbVersion()) {
                case 0: // BASE DE DATOS NUEVA: Creamos las tablas y actualizamos el número de versión al correcto.
                    CrearDBs();
                    SetDbVersion(DB_VERSION);
                    break;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD LINEAS
        // ====================================================================================================

        public IEnumerable<Linea> GetLineas() {
            try {
                return GetItems<Linea>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetLineas), ex);
            }
            return new List<Linea>();
        }


        public void GuardarLineas(IEnumerable<Linea> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarLineas), ex);
            }
        }


        public void BorrarLineas(IEnumerable<Linea> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarLineas), ex);
            }
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD ITINERARIOS
        // ====================================================================================================

        public Itinerario GetItinerarioByNombre(decimal nombre) {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM Itinerarios WHERE Nombre = @nombre").AddParameter("@nombre", nombre);
                return GetItem<Itinerario>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetItinerarioByNombre), ex);
            }
            return new Itinerario();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD PARADAS
        // ====================================================================================================

        // Al gestionarse las paradas desde los itinerarios, no son necesarios métodos en este apartado.

        #endregion
        // ====================================================================================================






    }
}
