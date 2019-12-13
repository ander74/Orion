#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SQLite;
using Orion.Config;
using Orion.Models;

namespace Orion.Servicios {

    public class OrionRepository : SQLiteRepository {


        /*
            strftime('%Y-%m-%d', Fecha) = strftime('%Y-%m-%d', @fecha)  --->  La fecha entera
            
            strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha)  --->  Mes y año
            
            strftime('%Y', Fecha) = strftime('%Y', @fecha)  --->  Año
            
            strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)  --->  La fecha es menor que
            
            CAST(strftime('%d', Fecha) AS INTEGER) <= @dia  --->  Compara el día de la fecha con un día.
            
            CAST(strftime('%m', Fecha) AS INTEGER) <= @mes  --->  Compara el mes de la fecha con un mes.
            
            CAST(strftime('%Y', Fecha) AS INTEGER) <= @año  --->  Compara el año de la fecha con un año.
        */



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

        public const string SqlActualizarCalendario = "UPDATE Calendarios SET IdConductor = @idConductor, Fecha = @fecha, Notas = @notas WHERE __id=@id;";


        public const string SqlActualizarConductor = "UPDATE Conductores SET " +
            "Nombre = @nombre, Apellidos = @apellidos, Indefinido = @indefinido, Telefono = @telefono, Email = @email, " +
            "Acumuladas = @acumuladas, Descansos = @descansos, DescansosNoDisfrutados = @descansosNoDisfrutados, PlusDistancia = @plusDistancia, " +
            "ReduccionJornada = @reduccionJornada, Notas = @notas WHERE __id=@id;";


        public const string SqlActualizarDiaCalendario = "UPDATE DiasCalendario SET " +
            "IdCalendario = @idCalendario, Dia = @dia, DiaFecha = @diaFecha, Grafico = @grafico, Codigo = @codigo, ExcesoJornada = @excesoJornada, " +
            "FacturadoPaqueteria = @facturadoPaqueteria, Limpieza = @limpieza, GraficoVinculado = @graficoVinculado, Notas = @notas WHERE __id=@id;";


        public const string SqlActualizarFestivo = "UPDATE Festivos SET Año = @año, Fecha = @fecha WHERE __id=@id;";


        public const string SqlActualizarGrafico = "UPDATE Graficos SET " +
            "IdGrupo = @idGrupo, NoCalcular = @noCalcular, Numero = @numero, Turno = @turno, DescuadreInicio = @descuadreInicio, Inicio = @inicio, " +
            "Final = @final, DescuadreFinal = @descuadreFinal, InicioPartido = @inicioPartido, FinalPartido = @finalPartido, Valoracion = @valoracion, " +
            "Trabajadas = @trabajadas, Acumuladas = @acumuladas, Nocturnas = @nocturnas, Desayuno = @desayuno, Comida = @comida, Cena = @cena, " +
            "PlusCena = @plusCena, PlusLimpieza = @plusLimpieza, PlusPaqueteria = @plusPaqueteria WHERE _id=@id;";


        public const string SqlActualizarGrupo = "UPDATE GruposGraficos SET Validez = @validez, Notas = @notas WHERE _id=@id;";


        public const string SqlActualizarItinerario = "UPDATE Itinerarios SET " +
            "IdLinea = @idLinea, Nombre = @nombre, Descripcion = @descripcion, TiempoReal = @tiempoReal, TiempoPago = @tiempoPago WHERE _id=@id;";


        public const string SqlActualizarLinea = "UPDATE Lineas SET Nombre = @nombre, Descripcion = @descripcion WHERE _id=@id;";


        public const string SqlActualizarParada = "UPDATE Paradas SET " +
            "IdItinerario = @idItinerario, Orden = @orden, Descripcion = @descripcion, Tiempo = @tiempo WHERE _id=@id;";


        public const string SqlActualizarRegulacion = "UPDATE Regulaciones SET " +
            "IdConductor = @idConductor, Codigo = @codigo, Fecha = @fecha, Horas = @horas, Descansos = @descansos, Motivo = @motivo WHERE _id=@id;";


        public const string SqlActualizarValoracion = "UPDATE Valoraciones SET " +
            "IdGrafico = @idGrafico, Inicio = @inicio, Linea = @linea, Descripcion = @descripcion, Final = @final, Tiempo = @tiempo WHERE _id=@id;";


        // CONSULTA NO FUNCIONAL
        public const string SqlCambiarFechasDiasCalendario = "UPDATE (SELECT DiasCalendario.DiaFecha AS F, DateSerial(Year(Calendarios.Fecha),Month(Calendarios.Fecha),DiasCalendario.Dia) AS FF FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario) SET F = FF;";


        public const string SqlInsertarCalendario = "INSERT INTO Calendarios (IdConductor, Fecha, Notas) VALUES (@idConductor, @fecha, @notas);";


        public const string SqlInsertarConductor = "INSERT INTO Conductores " +
            "(Nombre, Apellidos, Indefinido, Telefono, Email, Acumuladas, Descansos, DescansosNoDisfrutados, PlusDistancia, ReduccionJornada, Notas, _id ) " +
            "VALUES " +
            "(@nombre, @apellidos, @indefinido, @telefono, @email, @acumuladas, @descansos, @descansosNoDisfrutados, @plusDistancia, @reduccionJornada, " +
            "@notas, @id);";


        public const string SqlInsertarConductorDesconocido = "INSERT INTO Conductores (_id, Nombre) VALUES (@id, 'Desconocido');";


        public const string SqlInsertarDiaCalendario = "INSERT INTO DiasCalendario " +
            "(IdCalendario, Dia, DiaFecha, Grafico, Codigo, ExcesoJornada, FacturadoPaqueteria, Limpieza, GraficoVinculado, Notas) " +
            "VALUES " +
            "(@idCalendario, @dia, @diaFecha, @grafico, @codigo, @excesoJornada, @facturadoPaqueteria, @limpieza, @graficoVinculado, @notas);";


        public const string SqlInsertarFestivo = "INSERT INTO Festivos (Año, Fecha) VALUES (@año, @fecha);";


        public const string SqlInsertarGrafico = "INSERT INTO Graficos " +
            "(IdGrupo, NoCalcular, Numero, Turno, DescuadreInicio, Inicio, Final, DescuadreFinal, InicioPartido, FinalPartido, " +
            "Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, PlusLimpieza, PlusPaqueteria) " +
            "VALUES " +
            "(@idGrupo, @noCalcular, @numero, @turno, @descuadreInicio, @inicio, @final, @descuadreFinal, @inicioPartido, @finalPartido, " +
            "@valoracion, @trabajadas, @acumuladas, @nocturnas, @desayuno, @comida, @cena, @plusCena, @plusLimpieza, @plusPaqueteria);";


        public const string SqlInsertarGrupo = "INSERT INTO GruposGraficos (Validez, Notas) VALUES (@validez, @notas);";


        public const string SqlInsertarItinerario = "INSERT INTO Itinerarios " +
            "(IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago) " +
            "VALUES " +
            "(@idLinea, @nombre, @descripcion, @tiempoReal, @tiempoPago);";


        public const string SqlInsertarLinea = "INSERT INTO Lineas (Nombre, Descripcion) VALUES (@nombre, @descripcion);";


        public const string SqlInsertarParada = "INSERT INTO Paradas " +
            "(IdItinerario, Orden, Descripcion, Tiempo) " +
            "VALUES " +
            "(@idItinerario, @orden, @descripcion, @tiempo);";


        public const string SqlInsertarRegulacion = "INSERT INTO Regulaciones " +
            "(IdConductor, Codigo, Fecha, Horas, Descansos, Motivo) " +
            "VALUES " +
            "(@idConductor, @codigo, @fecha, @horas, @descansos, @motivo);";


        public const string SqlInsertarValoracion = "INSERT INTO Valoraciones " +
            "(IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo) " +
            "VALUES " +
            "(@idGrafico, @inicio, @linea, @descripcion, @final, @tiempo);";


        public const string SqlBorrarCalendario = "DELETE * FROM Calendarios WHERE _id=@id;";


        public const string SqlBorrarConductor = "DELETE * FROM Conductores WHERE _id=@id;";


        public const string SqlBorrarDiaCalendario = "DELETE * FROM DiasCalendario WHERE _id=@id;";


        public const string SqlBorrarFestivo = "DELETE * FROM Festivos WHERE _id=@id;";


        public const string SqlBorrarGrafico = "DELETE * FROM Graficos WHERE _id=@id;";


        public const string SqlBorrarGrupo = "DELETE * FROM GruposGraficos WHERE _id=@id;";


        public const string SqlBorrarItinerario = "DELETE * FROM Itinerarios WHERE _id=@id;";


        public const string SqlBorrarLinea = "DELETE * FROM Lineas WHERE _id=@id;";


        public const string SqlBorrarParada = "DELETE * FROM Paradas WHERE _id=@id;";


        public const string SqlBorrarRegulacion = "DELETE * FROM Regulaciones WHERE _id=@id;";


        public const string SqlBorrarValoracion = "DELETE * FROM Valoraciones WHERE _id=@id;";


        // CONSULTA NO FUNCIONAL
        public const string SqlConsultaPrueba = "SELECT (Sum(Graficos.Inicio) + Sum(Calendarios._id)) AS Resultado FROM Graficos, Calendarios WHERE Graficos._id = 123 AND Calendarios._id = 190;";


        public const string SqlExisteConductor = "SELECT Count(_id) FROM Conductores WHERE _id=@id;";


        public const string SqlExisteGrupo = "SELECT Count(*) FROM GruposGraficos WHERE Validez=@validez;";


        public const string SqlGetCalendarios = "SELECT Calendarios.*, Conductores.Indefinido " +
            "FROM Calendarios " +
            "LEFT JOIN Conductores " +
            "ON Calendarios.IdConductor = Conductores._id " +
            "WHERE Year(Calendarios.Fecha)=@año And Month(Calendarios.Fecha)=@mes ORDER BY Calendarios.IdConductor;";


        public const string SqlGetConductor = "SELECT * FROM Conductores WHERE _id=@id;";


        public const string SqlGetConductores = "SELECT * FROM Conductores ORDER BY _id;";


        // CONSULTA NO FUNCIONAL
        public const string SqlGetConsultas = "SELECT Name FROM MSysObjects WHERE Type = 5 ORDER BY Name;";


        public const string SqlGetDiasCalendario = "SELECT * FROM DiasCalendario WHERE IdCalendario=@idCalendario ORDER BY Dia;";


        public const string SqlGetDiasCalendarioAño = "SELECT * FROM DiasCalendario " +
            "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @idConductor AND Year(Fecha) = @año);";


        public const string SqlGetDiasCalendarioConBloqueos = "SELECT * FROM DiasCalendario " +
            "WHERE (ExcesoJornada <> 0 OR Descuadre <> 0) AND IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @idConductor);";


        public const string SqlGetDiasPijama = "SELECT * FROM " +
            "(SELECT * FROM Graficos WHERE IdGrupo = " +
            "(SELECT _id FROM GruposGraficos WHERE Validez = " +
            "(SELECT Max(Validez) FROM GruposGraficos WHERE Validez <= @validez))) " +
            "WHERE Numero=@numero;";


        public const string SqlGetEstadisticasGraficos = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
            "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
            "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
            "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
            "FROM GruposGraficos " +
            "LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
            "WHERE IdGrupo=@idGrupo GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string SqlGetEstadisticasGraficosDesdeFecha = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
            "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
            "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
            "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
            "FROM GruposGraficos " +
            "LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
            "WHERE GruposGraficos.Validez >= @fecha GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string SqlGetFestivos = "SELECT * FROM Festivos ORDER BY Fecha;";


        public const string SqlGetFestivosPorAño = "SELECT * FROM Festivos WHERE Año=@año ORDER BY Fecha;";


        public const string SqlGetFestivosPorMes = "SELECT * FROM Festivos WHERE Año=@año And Month(Fecha)=@mes ORDER BY Fecha;";


        public const string SqlGetGraficos = "SELECT * FROM Graficos WHERE IdGrupo=@idGrupo ORDER BY Numero;";


        public const string SqlGetGraficosGrupoPorFecha = "SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id FROM GruposGraficos WHERE Validez = @validez);";


        public const string SqlGetGrupos = "SELECT * FROM GruposGraficos ORDER BY Validez DESC;";


        public const string SqlGetItinerarioPorNombre = "SELECT * FROM Itinerarios WHERE Name=@nombre;";


        public const string SqlGetItinerarios = "SELECT * FROM Itinerarios WHERE IdLinea=@ ORDER BY Nombre;";


        public const string SqlGetLineas = "SELECT * FROM Lineas ORDER BY Nombre;";


        public const string SqlGetParadas = "SELECT * FROM Paradas WHERE IdItinerario=@idItinerario ORDER BY Orden;";


        public const string SqlGetRegulaciones = "SELECT * FROM Regulaciones WHERE IdConductor=@idConductor ORDER BY Fecha, _id;";


        public const string SqlGetValoraciones = "SELECT * FROM Valoraciones WHERE IdGrafico=@idGrafico ORDER BY Inicio, _id;";


        // CONSULTA NO FUNCIONAL
        public const string SqlTemporal = "SELECT DiasCalendario.Dia, Calendarios.IdConductor, DiasCalendario.Fecha, DiasCalendario.ExcesoJornada FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario WHERE Calendarios.IdConductor = 450 AND ExcesoJornada > 0;";











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

        public OrionRepository(string cadenaConexion) : base(cadenaConexion) {
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
        #region BD CALENDARIOS
        // ====================================================================================================


        public ObservableCollection<Calendario> GetCalendarios(int año, int mes) {

            try {
                var consulta = new SQLiteExpression(SqlGetCalendarios).AddParameter("@año", año).AddParameter("@mes", mes);
                var lista = GetItems<Calendario>(consulta);
                //TODO: Añadir si el conductor es o no indefinido y quitar la parte Indefinido de la consulta.
                return new ObservableCollection<Calendario>(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetCalendarios), ex);
            }
            return new ObservableCollection<Calendario>();
        }


        public List<Calendario> GetCalendariosConductor(int año, int matricula) {

            try {
                DateTime fecha = new DateTime(año, 1, 1);
                string comandoSQL = "SELECT Calendarios.*, Conductores.Indefinido " +
                    "FROM Calendarios LEFT JOIN Conductores ON Calendarios.IdConductor = Conductores._id " +
                    "WHERE strftime('%Y', Calendarios.Fecha) = strftime('%Y', @fecha) AND Calendarios.IdConductor = @matricula " +
                    "ORDER BY Calendarios.Fecha;";
                var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
                var lista = GetItems<Calendario>(consulta);
                //TODO: Añadir si el conductor es o no indefinido y quitar la parte Indefinido de la consulta.
                return lista.ToList();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetCalendariosConductor), ex);
            }
            return new List<Calendario>();
        }


        public List<Calendario> GetCalendariosConductor(int año, int mes, int matricula) {

            try {
                DateTime fecha = new DateTime(año, mes, 1);
                string comandoSQL = "SELECT Calendarios.*, Conductores.Indefinido " +
                                    "FROM Calendarios LEFT JOIN Conductores ON Calendarios.IdConductor = Conductores._id " +
                                    "WHERE strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Calendarios.IdConductor = @matricula ;";
                var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
                var lista = GetItems<Calendario>(consulta);
                //TODO: Añadir si el conductor es o no indefinido y quitar la parte Indefinido de la consulta.
                return lista.ToList();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetCalendariosConductor), ex);
            }
            return new List<Calendario>();
        }


        public void GuardarCalendarios(IEnumerable<Calendario> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarCalendarios), ex);
            }
        }


        public void BorrarCalendarios(IEnumerable<Calendario> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarCalendarios), ex);
            }
        }


        public int GetDescansosReguladosHastaMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosReguladosHastaMes), ex);
            }
            return 0;
        }


        public int GetDescansosReguladosAño(int año, int idconductor) {
            string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y', Fecha) = strftime('%Y', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosReguladosAño), ex);
            }
            return 0;
        }


        public int GetDescansosDisfrutadosHastaMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND IdConductor = @matricula)" +
                                "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosDisfrutadosHastaMes), ex);
            }
            return 0;
        }


        public int GetDCDisfrutadosAño(int idconductor, int año) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND IdConductor = @matricula)" +
                                "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";

            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDCDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDCDisfrutadosAño(int idconductor, int año, int mes) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios " +
                                      "WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND strftime('%m', Fecha) <= strftime('%m', @fecha) AND IdConductor = @matricula)" +
                                      "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            //TODO: Comprobar que no se necesita meter otro parámetro fecha, al haber dos en la consulta. Yo creo que si.
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDCDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDiasF6HastaMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                            "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND IdConductor = @matricula)" +
                            "      AND Grafico = -7; ";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasF6HastaMes), ex);
            }
            return 0;
        }


        public TimeSpan GetHorasReguladasHastaMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasHastaMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasReguladasMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 0;";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCambiadasPorDCsMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCambiadasPorDCsMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasReguladasAño(int año, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y', Fecha) = strftime('%Y', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasAño), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCobradasMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 1;";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCobradasMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCobradasAño(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @matricula AND strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND Codigo = 1;";
            DateTime fechainicio = mes == 12 ? new DateTime(año, 11, 30) : new DateTime(año - 1, 11, 30);
            DateTime fechafinal = mes == 12 ? new DateTime(año + 1, 12, 1) : new DateTime(año, 12, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor)
                .AddParameter("@fechaInicio", fechainicio)
                .AddParameter("@fechaFinal", fechafinal);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCobradasAño), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetExcesoJornadaCobradaHastaMes(int año, int mes, int idconductor) {
            string comandoSQL = "SELECT Sum(ExcesoJornadaCobrada) FROM Calendarios WHERE IdConductor = @matricula AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetExcesoJornadaCobradaHastaMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetAcumuladasHastaMes(int año, int mes, int idconductor) {
            TimeSpan acumuladas = TimeSpan.Zero;
            string comandoDias = "SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha " +
                 "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios._id " +
                 "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND IdConductor = @matricula) " +
                 "      AND Grafico > 0 " +
                 "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;";

            string comandoSQL = "SELECT Acumuladas " +
                                "FROM (SELECT * " +
                                "      FROM Graficos" +
                                "      WHERE IdGrupo = (SELECT _id " +
                                "                       FROM GruposGraficos " +
                                "                       WHERE Validez = (SELECT Max(Validez) " +
                                "                                        FROM GruposGraficos " +
                                "                                        WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
                                "WHERE Numero = @numero";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            try {
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    var consulta = new SQLiteExpression(comandoDias).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                int dia = lector.ToInt32("Dia");
                                int grafico = lector.ToInt32("Grafico");
                                int graficoVinculado = lector.ToInt32("GraficoVinculado");
                                DateTime fecha2 = lector.ToDateTime("Fecha");
                                if (graficoVinculado != 0 && grafico == App.Global.PorCentro.Comodin) grafico = graficoVinculado;
                                if (dia > DateTime.DaysInMonth(fecha2.Year, fecha2.Month)) continue;
                                DateTime fechadia = new DateTime(fecha2.Year, fecha2.Month, dia);
                                var consulta2 = new SQLiteExpression(comandoSQL).AddParameter("@validez", fechadia).AddParameter("@numero", grafico);
                                acumuladas += GetTimeSpanScalar(conexion, consulta2);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetAcumuladasHastaMes), ex);
            }
            return acumuladas;
        }


        public int GetComiteEnDescansoHastaMes(int idconductor, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.IdConductor = @matricula AND " +
                                "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5) AND " +
                                "	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
                                "      strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @fecha);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetComiteEnDescansoHastaMes), ex);
            }
            return 0;
        }


        public int GetTrabajoEnDescansoHastaMes(int idconductor, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.IdConductor = @matricula AND " +
                                "      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @validez)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetTrabajoEnDescansoHastaMes), ex);
            }
            return 0;
        }


        public int GetDNDHastaMes(int idconductor, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.IdConductor = @matricula AND " +
                                "      DiasCalendario.Grafico = -8 AND " +
                                "      WHERE strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @validez)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idconductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDNDHastaMes), ex);
            }
            return 0;
        }


        public int GetDNDDisfrutadosAño(int idconductor, int año) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND IdConductor = @matricula)" +
                                "      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDNDDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDNDDisfrutadosAño(int idconductor, int año, int mes) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios " +
                                      "WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND strftime('%m', Fecha) <= strftime('%m', Fecha) AND IdConductor = @matricula)" +
                                      "      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", idconductor);
            //TODO: Comprobar que no se necesita un segundo parámetro fecha, ya que hay dos en la consulta. Yo creo que si.
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDNDDisfrutadosAño), ex);
            }
            return 0;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD CONDUCTORES
        // ====================================================================================================


        public IEnumerable<Conductor> GetConductores() {
            try {
                return GetItems<Conductor>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetConductores), ex);
            }
            return new List<Conductor>();
        }


        public void GuardarConductores(IEnumerable<Conductor> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarConductores), ex);
            }
        }


        public void BorrarConductores(IEnumerable<Conductor> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarConductores), ex);
            }
        }


        public bool ExisteConductor(int idconductor) {
            try {
                var whereCondition = new SQLiteExpression("_id = @id").AddParameter("@id", idconductor);
                return GetCount<Conductor>(whereCondition) > 0;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.ExisteConductor), ex);
            }
            return false;
        }


        public void InsertarConductorDesconocido(int idconductor) {
            var conductor = new Conductor();
            conductor.Id = idconductor;
            conductor.Nombre = "Desconocido";
            conductor.Notas = "Conductor insertado automáticamente por el sistema.";
            try {
                GuardarItem(conductor, true);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.InsertarConductorDesconocido), ex);
            }

        }


        public Conductor GetConductor(int idconductor) {
            try {
                return GetItem<Conductor>(idconductor);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetConductor), ex);
            }
            return null;
        }







        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD DÍAS CALENDARIO
        // ====================================================================================================

        public DiaCalendarioBase GetDiaCalendario(int idConductor, DateTime fecha) {

            string comandoSQL = "SELECT * FROM DiasCalendario WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @matricula) AND DiaFecha = @fecha;";
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetItem<DiaCalendario>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarCalendarios), ex);
            }
            return null;
        }


        #endregion
        // ====================================================================================================



        public List<Conductor> GetPrueba(string path) {

            try {
                string comando1 = $"ATTACH '{path}' AS Arr";
                string comandoSQL = "SELECT * FROM Conductores UNION ALL SELECT * FROM Arr.Conductores ORDER BY _id ASC;";
                //string comandoSQL = "SELECT * FROM Arr.Conductores;";
                var consulta1 = new SQLiteExpression(comando1);
                var consulta2 = new SQLiteExpression(comandoSQL);
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    using (var comando = consulta1.GetCommand(conexion)) {
                        comando.ExecuteNonQuery();
                        var lista = GetItems<Conductor>(conexion, consulta2);
                        return lista.ToList();
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetPrueba), ex);
            }
            return new List<Conductor>();
        }






    }
}
