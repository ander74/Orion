#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Orion.Interfaces;

namespace Orion.Servicios {

    public class DbService {


        // ====================================================================================================
        #region CREACIÓN DE TABLAS
        // ====================================================================================================

        public const string CrearTablaCalendarios = "CREATE TABLE IF NOT EXISTS Calendarios (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdConductor INTEGER DEFAULT 0, " +
            "Fecha TEXT, " +
            "Descuadre INTEGER DEFAULT 0, " +
            "ExcesoJornadaCobrada REAL DEFAULT 0, " +
            "Notas TEXT DEFAULT ''" +
            ");";


        public const string CrearTablaConductores = "CREATE TABLE IF NOT EXISTS Conductores (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nombre TEXT DEFAULT '', " +
            "Apellidos TEXT DEFAULT '', " +
            "Indefinido INTEGER DEFAULT 0, " +
            "Telefono TEXT DEFAULT '', " +
            "Email TEXT DEFAULT '', " +
            "Acumuladas REAL DEFAULT 0, " +
            "Descansos REAL DEFAULT 0, " +
            "DescansosNoDisfrutados REAL DEFAULT 0, " +
            "PlusDistancia REAL DEFAULT 0, " +
            "ReduccionJornada INTEGER DEFAULT 0, " +
            "Notas TEXT DEFAULT ''" +
            ");";


        public const string CrearTablaDiasCalendario = "CREATE TABLE IF NOT EXISTS DiasCalendario (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdCalendario INTEGER DEFAULT 0, " +
            "Dia INTEGER DEFAULT 1, " +
            "DiaFecha TEXT, " +
            "Grafico INTEGER DEFAULT 0, " +
            "Codigo INTEGER DEFAULT 0, " +
            "ExcesoJornada REAL DEFAULT 0, " +
            "BloquearExcesoJornada INTEGER DEFAULT 0, " +
            "Descuadre INTEGER DEFAULT 0, " +
            "BloquearDescuadre INTEGER DEFAULT 0, " +
            "FacturadoPaqueteria REAL DEFAULT 0, " +
            "Limpieza INTEGER DEFAULT 0, " +
            "GraficoVinculado INTEGER DEFAULT 0, " +
            "TurnoAlt INTEGER DEFAULT 0, " +
            "InicioAlt INTEGER DEFAULT 0, " +
            "FinalAlt INTEGER DEFAULT 0, " +
            "InicioPartidoAlt INTEGER DEFAULT 0, " +
            "FinalPartidoAlt INTEGER DEFAULT 0, " +
            "TrabajadasAlt INTEGER DEFAULT 0, " +
            "AcumuladasAlt INTEGER DEFAULT 0, " +
            "NocturnasAlt INTEGER DEFAULT 0, " +
            "DesayunoAlt REAL DEFAULT 0, " +
            "ComidaAlt REAL DEFAULT 0, " +
            "CenaAlt REAL DEFAULT 0, " +
            "PlusCenaAlt REAL DEFAULT 0, " +
            "PlusLimpiezaAlt INTEGER DEFAULT 0, " +
            "PlusPaqueteriaAlt INTEGER DEFAULT 0, " +
            "Notas TEXT DEFAULT ''" +
            ");";


        public const string CrearTablaFestivos = "CREATE TABLE IF NOT EXISTS Festivos (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Año INTEGER DEFAULT 0, " +
            "Fecha TEXT " +
            ");";


        public const string CrearTablaGraficos = "CREATE TABLE IF NOT EXISTS Graficos (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdGrupo INTEGER DEFAULT 0, " +
            "NoCalcular INTEGER DEFAULT 0, " +
            "Numero INTEGER DEFAULT 0, " +
            "DiaSemana TEXT DEFAULT '', " +
            "Turno INTEGER DEFAULT 0, " +
            "DescuadreInicio INTEGER DEFAULT 0, " +
            "Inicio INTEGER, " +
            "Final INTEGER, " +
            "DescuadreFinal INTEGER DEFAULT 0, " +
            "InicioPartido, " +
            "FinalPartido, " +
            "Valoracion INTEGER DEFAULT 0, " +
            "Trabajadas INTEGER DEFAULT 0, " +
            "Acumuladas INTEGER DEFAULT 0, " +
            "Nocturnas INTEGER DEFAULT 0, " +
            "Desayuno REAL DEFAULT 0, " +
            "Comida REAL DEFAULT 0, " +
            "Cena REAL DEFAULT 0, " +
            "PlusCena REAL DEFAULT 0, " +
            "PlusLimpieza INTEGER DEFAULT 0, " +
            "PlusPaqueteria INTEGER DEFAULT 0 " +
            ");";


        public const string CrearTablaGruposGraficos = "CREATE TABLE IF NOT EXISTS GruposGraficos (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Validez TEXT, " +
            "Notas TEXT DEFAULT '' " +
            ");";


        public const string CrearTablaPluses = "CREATE TABLE IF NOT EXISTS Pluses (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Año INTEGER DEFAULT 0, " +
            "ImporteDietas REAL DEFAULT 0, " +
            "ImporteSabados REAL DEFAULT 0, " +
            "ImporteFestivos REAL DEFAULT 0, " +
            "PlusNocturnidad REAL DEFAULT 0, " +
            "DietaMenorDescanso REAL DEFAULT 0, " +
            "PlusLimpieza REAL DEFAULT 0, " +
            "PlusPaqueteria REAL DEFAULT 0, " +
            "PlusNavidad REAL DEFAULT 0 " +
            ");";


        public const string CrearTablaRegulaciones = "CREATE TABLE IF NOT EXISTS Regulaciones (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdConductor INTEGER DEFAULT 0, " +
            "Codigo INTEGER DEFAULT 0, " +
            "Fecha TEXT, " +
            "Horas INTEGER DEFAULT 0, " +
            "Descansos REAL DEFAULT 0, " +
            "Dnds REAL DEFAULT 0, " +
            "Motivo TEXT DEFAULT '' " +
            ");";


        public const string CrearTablaValoraciones = "CREATE TABLE IF NOT EXISTS Valoraciones (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdGrafico INTEGER DEFAULT 0, " +
            "Inicio INTEGER DEFAULT 0, " +
            "Linea REAL DEFAULT 0, " +
            "Descripcion TEXT DEFAULT '', " +
            "Final INTEGER DEFAULT 0, " +
            "Tiempo INTEGER DEFAULT 0 " +
            ");";


        public const string CrearTablaItinerarios = "CREATE TABLE IF NOT EXISTS Itinerarios (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdLinea INTEGER DEFAULT 0, " +
            "Nombre REAL DEFAULT 0, " +
            "Descripcion TEXT DEFAULT '', " +
            "TiempoReal INTEGER DEFAULT 0, " +
            "TiempoPago INTEGER DEFAULT 0 " +
            ");";


        public const string CrearTablaLineas = "CREATE TABLE IF NOT EXISTS Lineas (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Nombre TEXT DEFAULT '', " +
            "Descripcion TEXT DEFAULT '' " +
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

        public const string ActualizarCalendario = "UPDATE Calendarios SET IdConductor = ?, Fecha = ?, Notas = ? WHERE _id=?;";


        public const string ActualizarConductor = "UPDATE Conductores SET Nombre = ?, Apellidos = ?, Indefinido = ?, Telefono = ?, Email = ?, " +
            "Acumuladas = ?, Descansos = ?, DescansosNoDisfrutados = ?, PlusDistancia = ?, ReduccionJornada = ?, Notas = ? WHERE _id=?;";


        public const string ActualizarDiaCalendario = "UPDATE DiasCalendario SET IdCalendario = ?, Dia = ?, DiaFecha = ?, Grafico = ?, " +
            "Codigo = ?, ExcesoJornada = ?, FacturadoPaqueteria = ?, Limpieza = ?, GraficoVinculado = ?, Notas = ? WHERE _id=?;";


        public const string ActualizarFestivo = "UPDATE Festivos SET Año = ?, Fecha = ? WHERE _id=?;";


        public const string ActualizarGrafico = "UPDATE Graficos SET IdGrupo = ?, NoCalcular = ?, Numero = ?, " +
            "Turno = ?, DescuadreInicio = ?, Inicio = ?, Final = ?, DescuadreFinal = ?, " +
            "InicioPartido = ?, FinalPartido = ?, Valoracion = ?, Trabajadas = ?, " +
            "Acumuladas = ?, Nocturnas = ?, Desayuno = ?, Comida = ?, Cena = ?, PlusCena = ?, " +
            "PlusLimpieza = ?, PlusPaqueteria = ? WHERE _id=?;";


        public const string ActualizarGrupo = "UPDATE GruposGraficos SET Validez = ?, Notas = ? WHERE _id=?;";


        public const string ActualizarItinerario = "UPDATE Itinerarios SET IdLinea = ?, Nombre = ?, Descripcion = ?, " +
            "TiempoReal = ?, TiempoPago = ? WHERE _id=?;";


        public const string ActualizarLinea = "UPDATE Lineas SET Nombre = ?, Descripcion = ? WHERE _id=?;";


        public const string ActualizarParada = "UPDATE Paradas SET IdItinerario = ?, Orden = ?, Descripcion = ?, Tiempo = ? WHERE _id=?;";


        public const string ActualizarRegulacion = "UPDATE Regulaciones SET IdConductor = ?, Codigo = ?, Fecha = ?, " +
            "Horas = ?, Descansos = ?, Dnds = ?, Motivo = ? WHERE _id=?;";


        public const string ActualizarValoracion = "UPDATE Valoraciones SET IdGrafico = ?, Inicio = ?, Linea = ?, Descripcion = ?, " +
            "Final = ?, Tiempo = ? WHERE _id=?;";


        public const string CambiarFechasDiasCalendario = "UPDATE (SELECT DiasCalendario.DiaFecha AS F, " +
            "DateSerial(Year(Calendarios.Fecha),Month(Calendarios.Fecha),DiasCalendario.Dia) AS FF " +
            "FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario) SET F = FF;";


        public const string InsertarCalendario = "INSERT INTO Calendarios ( IdConductor, Fecha, Notas ) VALUES (?, ?, ?);";


        public const string InsertarConductor = "INSERT INTO Conductores ( Nombre, Apellidos, Indefinido, Telefono, Email, Acumuladas, " +
            "Descansos, DescansosNoDisfrutados, PlusDistancia, ReduccionJornada, Notas, _id ) VALUES (?, ?, ?, " +
            "?, ?, ?, ?, ?, ?, ?, ?, ?);";


        public const string InsertarConductorDesconocido = "INSERT INTO Conductores ( _id, Nombre ) VALUES (?, 'Desconocido');";


        public const string InsertarDiaCalendario = "INSERT INTO DiasCalendario ( IdCalendario, Dia, DiaFecha, Grafico, Codigo, ExcesoJornada, " +
            "FacturadoPaqueteria, Limpieza, GraficoVinculado, Notas ) VALUES (?, ?, ?, ?, ?, ?, " +
            "?, ?, ?, ?);";


        public const string InsertarFestivo = "INSERT INTO Festivos ( Año, Fecha ) VALUES (?, ?);";


        public const string InsertarGrafico = "INSERT INTO Graficos ( IdGrupo, NoCalcular, Numero, Turno, DescuadreInicio, Inicio, Final, " +
            "DescuadreFinal, InicioPartido, FinalPartido, Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, " +
            "PlusLimpieza, PlusPaqueteria ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, " +
            "?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, " +
            "?);";


        public const string InsertarGrupo = "INSERT INTO GruposGraficos ( Validez, Notas ) VALUES (?, ?);";


        public const string InsertarItinerario = "INSERT INTO Itinerarios ( IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago ) " +
            "VALUES (?, ?, ?, ?, ?);";


        public const string InsertarLinea = "INSERT INTO Lineas ( Nombre, Descripcion ) VALUES (?, ?);";


        public const string InsertarParada = "INSERT INTO Paradas ( IdItinerario, Orden, Descripcion, Tiempo ) " +
            "VALUES (?, ?, ?, ?);";


        public const string InsertarRegulacion = "INSERT INTO Regulaciones ( IdConductor, Codigo, Fecha, Horas, Descansos, Dnds, Motivo ) " +
            "VALUES (?, ?, ?, ?, ?, ?, ?);";


        public const string InsertarValoracion = "INSERT INTO Valoraciones ( IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo ) " +
            "VALUES (?, ?, ?, ?, ?, ?);";


        public const string BorrarCalendario = "DELETE * FROM Calendarios WHERE _id=?;";


        public const string BorrarConductor = "DELETE * FROM Conductores WHERE _id=?;";


        public const string BorrarDiaCalendario = "DELETE * FROM DiasCalendario WHERE _id=?;";


        public const string BorrarFestivo = "DELETE * FROM Festivos WHERE _id=?;";


        public const string BorrarGrafico = "DELETE * FROM Graficos WHERE _id=?;";


        public const string BorrarGrupo = "DELETE * FROM GruposGraficos WHERE _id=?;";


        public const string BorrarItinerario = "DELETE * FROM Itinerarios WHERE _id=?;";


        public const string BorrarLinea = "DELETE * FROM Lineas WHERE _id=?;";


        public const string BorrarParada = "DELETE * FROM Paradas WHERE _id=?;";


        public const string BorrarRegulacion = "DELETE * FROM Regulaciones WHERE _id=?;";


        public const string BorrarValoracion = "DELETE * FROM Valoraciones WHERE _id=?;";


        public const string ConsultaPrueba = "SELECT (Sum(Graficos.Inicio) + Sum(Calendarios.Id)) AS Resultado FROM Graficos, Calendarios " +
            "WHERE Graficos.Id = 123 AND Calendarios.Id = 190;";


        public const string ExisteConductor = "SELECT Count(_id) FROM Conductores WHERE _id=?;";


        public const string ExisteGrupo = "SELECT Count(*) FROM GruposGraficos WHERE Validez=?;";


        public const string GetCalendarios = "SELECT Calendarios.*, Conductores.Indefinido FROM Calendarios " +
            "LEFT JOIN Conductores ON Calendarios.IdConductor = Conductores._id " +
            "WHERE Year(Calendarios.Fecha)=? And Month(Calendarios.Fecha)=? ORDER BY Calendarios.IdConductor;";


        public const string GetConductor = "SELECT * FROM Conductores WHERE _id=?;";


        public const string GetConductores = "SELECT * FROM Conductores ORDER BY _id;";


        public const string GetConsultas = "SELECT Name FROM MSysObjects WHERE Type = 5 ORDER BY Name;";


        public const string GetDiasCalendario = "SELECT * FROM DiasCalendario WHERE IdCalendario=? ORDER BY Dia;";


        public const string GetDiasCalendarioAño = "SELECT * FROM DiasCalendario WHERE IdCalendario IN " +
            "(SELECT _id FROM Calendarios WHERE IdConductor = ? AND Year(Fecha) = ?);";


        public const string GetDiasCalendarioConBloqueos = "SELECT * FROM DiasCalendario WHERE " +
            "(ExcesoJornada <> 0 OR Descuadre <> 0) AND IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = ?);";


        public const string GetDiasPijama = "SELECT * FROM (SELECT * FROM Graficos WHERE IdGrupo = " +
            "(SELECT _id FROM GruposGraficos WHERE Validez = (SELECT Max(Validez) FROM GruposGraficos WHERE Validez <= ?))) WHERE Numero=?;";


        public const string GetEstadisticasGraficos = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
            "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<? AND NOT NoCalcular,?,Trabajadas)) AS xTrabajadas, " +
            "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
            "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
            "FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo WHERE IdGrupo=? GROUP BY GruposGraficos.Validez, " +
            "Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string GetEstadisticasGraficosDesdeFecha = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
            "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<? AND NOT NoCalcular,?,Trabajadas)) AS xTrabajadas, " +
            "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
            "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
            "FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo WHERE GruposGraficos.Validez >= ? " +
            "GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string GetFestivos = "SELECT * FROM Festivos ORDER BY Fecha;";


        public const string GetFestivosPorAño = "SELECT * FROM Festivos WHERE Año=? ORDER BY Fecha;";


        public const string GetFestivosPorMes = "SELECT * FROM Festivos WHERE Año=? And Month(Fecha)=? ORDER BY Fecha;";


        public const string GetGraficos = "SELECT * FROM Graficos WHERE IdGrupo=? ORDER BY Numero;";


        public const string GetGraficosGrupoPorFecha = "SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id FROM GruposGraficos WHERE Validez = ?);";


        public const string GetGrupos = "SELECT * FROM GruposGraficos ORDER BY Validez DESC;";


        public const string GetItinerarioPorNombre = "SELECT * FROM Itinerarios WHERE Name=?;";


        public const string GetItinerarios = "SELECT * FROM Itinerarios WHERE IdLinea=? ORDER BY Nombre;";


        public const string GetLineas = "SELECT * FROM Lineas ORDER BY Nombre;";


        public const string GetParadas = "SELECT * FROM Paradas WHERE IdItinerario=? ORDER BY Orden;";


        public const string GetRegulaciones = "SELECT * FROM Regulaciones WHERE IdConductor=? ORDER BY Fecha, _id;";


        public const string GetValoraciones = "SELECT * FROM Valoraciones WHERE IdGrafico=? ORDER BY Inicio, _id;";


        public const string Temporal = "SELECT DiasCalendario.Dia, Calendarios.IdConductor, DiasCalendario.Fecha, DiasCalendario.ExcesoJornada " +
            "FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario " +
            "WHERE Calendarios.IdConductor = 450 AND ExcesoJornada > 0;";


        public const string Identity = "SELECT last_insert_rowid()";









        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private const int DB_VERSION = 1;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR + SINGLETON
        // ====================================================================================================

        private static DbService instance;


        private DbService() {
            InicializarCentro();
        }


        public static DbService GetInstance() {
            if (instance == null) instance = new DbService();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================


        private int GetDbVersion() {
            int version = 0;
            using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                using (var comando = new SQLiteCommand("PRAGMA user_version", conexion)) {
                    conexion.Open();
                    object resultado = comando.ExecuteScalar();
                    version = resultado == DBNull.Value ? 0 : Convert.ToInt32(resultado);
                }
            }
            return version;
        }


        private void SetDbVersion(int version) {
            using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                using (var comando = new SQLiteCommand($"PRAGMA user_version = {version}", conexion)) {
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }


        private void CrearDBs() {
            using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                conexion.Open();
                using (var comando = new SQLiteCommand(CrearTablaCalendarios, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaConductores, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaDiasCalendario, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaFestivos, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaGraficos, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaGruposGraficos, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaPluses, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaRegulaciones, conexion)) comando.ExecuteNonQuery();
                using (var comando = new SQLiteCommand(CrearTablaValoraciones, conexion)) comando.ExecuteNonQuery();
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Inicializa un centro, creando el archivo de base de datos si no existe.
        /// En caso de que la versión de la base de datos no sea correcta, crea o modifica las tablas en consecuencia.
        /// </summary>
        public void InicializarCentro() {
            if (App.Global.CentroActual == Centros.Desconocido) return;
            switch (GetDbVersion()) {
                case 0: // BASE DE DATOS NUEVA
                    CrearDBs();
                    SetDbVersion(DB_VERSION);
                    break;
            }


        }





        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS GUARDAR
        // ====================================================================================================

        private int Guardar<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return -1;
            if (item.Id <= 0) {
                using (var comando = new SQLiteCommand(item.ComandoInsertar, conexion)) {
                    comando.Parameters.AddRange(item.Parametros.ToArray());
                    comando.ExecuteNonQuery();
                }
                using (var comando = new SQLiteCommand(Identity, conexion)) {
                    int id = Convert.ToInt32(comando.ExecuteScalar());
                    item.Id = id < 0 ? 0 : id;
                }
            } else {
                using (var comando = new SQLiteCommand(item.ComandoActualizar, conexion)) {
                    comando.Parameters.AddRange(item.Parametros.ToArray());
                    comando.ExecuteNonQuery();
                }
            }
            return item.Id;
        }


        private int GuardarConLista<T>(T item, SQLiteConnection conexion) where T : ISQLItem {
            if (conexion == null || conexion.State != ConnectionState.Open) return -1;
            int id = Guardar(item, conexion);
            if (item.Lista != null) {
                foreach (var item2 in item.Lista) {
                    item2.ForeignId = id;
                    GuardarConLista(item2, conexion);
                }
            }
            return item.Id;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS GUARDAR
        // ====================================================================================================

        public int Guardar<T>(T item) where T : ISQLItem {
            using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                conexion.Open();
                return Guardar(item, conexion);
            }
        }


        public int GuardarConLista<T>(T item) where T : ISQLItem {
            using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                conexion.Open();
                return GuardarConLista(item, conexion);
            }
        }

        #endregion
        // ====================================================================================================



    }
}
