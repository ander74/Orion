﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using Orion.Config;
using Orion.Models;
using Orion.Pijama;

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

            CASE Turno WHEN 1 THEN 'Turno1' ELSE 'Turno2' END MiTurno  --->  Condicional (MiTurno sería el AS MiTurno)

            CASE WHEN G.Turno = 1 AND Indefinido THEN 'Turno1' ELSE 'Turno2' END MiTurno  --->  Otra forma de condicional (MiTurno sería el AS MiTurno)
                                                                                                Esta se debe usar cuando entran en juego más de un campo.


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

        //public const string SqlActualizarCalendario = "UPDATE Calendarios SET IdConductor = @idConductor, Fecha = @fecha, Notas = @notas WHERE __id=@id;";


        //public const string SqlActualizarConductor = "UPDATE Conductores SET " +
        //    "Nombre = @nombre, Apellidos = @apellidos, Indefinido = @indefinido, Telefono = @telefono, Email = @email, " +
        //    "Acumuladas = @acumuladas, Descansos = @descansos, DescansosNoDisfrutados = @descansosNoDisfrutados, PlusDistancia = @plusDistancia, " +
        //    "ReduccionJornada = @reduccionJornada, Notas = @notas WHERE __id=@id;";


        //public const string SqlActualizarDiaCalendario = "UPDATE DiasCalendario SET " +
        //    "IdCalendario = @idCalendario, Dia = @dia, DiaFecha = @diaFecha, Grafico = @grafico, Codigo = @codigo, ExcesoJornada = @excesoJornada, " +
        //    "FacturadoPaqueteria = @facturadoPaqueteria, Limpieza = @limpieza, GraficoVinculado = @graficoVinculado, Notas = @notas WHERE __id=@id;";


        //public const string SqlActualizarFestivo = "UPDATE Festivos SET Año = @año, Fecha = @fecha WHERE __id=@id;";


        //public const string SqlActualizarGrafico = "UPDATE Graficos SET " +
        //    "IdGrupo = @idGrupo, NoCalcular = @noCalcular, Numero = @numero, Turno = @turno, DescuadreInicio = @descuadreInicio, Inicio = @inicio, " +
        //    "Final = @final, DescuadreFinal = @descuadreFinal, InicioPartido = @inicioPartido, FinalPartido = @finalPartido, Valoracion = @valoracion, " +
        //    "Trabajadas = @trabajadas, Acumuladas = @acumuladas, Nocturnas = @nocturnas, Desayuno = @desayuno, Comida = @comida, Cena = @cena, " +
        //    "PlusCena = @plusCena, PlusLimpieza = @plusLimpieza, PlusPaqueteria = @plusPaqueteria WHERE _id=@id;";


        //public const string SqlActualizarGrupo = "UPDATE GruposGraficos SET Validez = @validez, Notas = @notas WHERE _id=@id;";


        //public const string SqlActualizarItinerario = "UPDATE Itinerarios SET " +
        //    "IdLinea = @idLinea, Nombre = @nombre, Descripcion = @descripcion, TiempoReal = @tiempoReal, TiempoPago = @tiempoPago WHERE _id=@id;";


        //public const string SqlActualizarLinea = "UPDATE Lineas SET Nombre = @nombre, Descripcion = @descripcion WHERE _id=@id;";


        //public const string SqlActualizarParada = "UPDATE Paradas SET " +
        //    "IdItinerario = @idItinerario, Orden = @orden, Descripcion = @descripcion, Tiempo = @tiempo WHERE _id=@id;";


        //public const string SqlActualizarRegulacion = "UPDATE Regulaciones SET " +
        //    "IdConductor = @idConductor, Codigo = @codigo, Fecha = @fecha, Horas = @horas, Descansos = @descansos, Motivo = @motivo WHERE _id=@id;";


        //public const string SqlActualizarValoracion = "UPDATE Valoraciones SET " +
        //    "IdGrafico = @idGrafico, Inicio = @inicio, Linea = @linea, Descripcion = @descripcion, Final = @final, Tiempo = @tiempo WHERE _id=@id;";


        // CONSULTA NO FUNCIONAL
        //public const string SqlCambiarFechasDiasCalendario = "UPDATE (SELECT DiasCalendario.DiaFecha AS F, DateSerial(Year(Calendarios.Fecha),Month(Calendarios.Fecha),DiasCalendario.Dia) AS FF FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario) SET F = FF;";


        //public const string SqlInsertarCalendario = "INSERT INTO Calendarios (IdConductor, Fecha, Notas) VALUES (@idConductor, @fecha, @notas);";


        //public const string SqlInsertarConductor = "INSERT INTO Conductores " +
        //    "(Nombre, Apellidos, Indefinido, Telefono, Email, Acumuladas, Descansos, DescansosNoDisfrutados, PlusDistancia, ReduccionJornada, Notas, _id ) " +
        //    "VALUES " +
        //    "(@nombre, @apellidos, @indefinido, @telefono, @email, @acumuladas, @descansos, @descansosNoDisfrutados, @plusDistancia, @reduccionJornada, " +
        //    "@notas, @id);";


        //public const string SqlInsertarConductorDesconocido = "INSERT INTO Conductores (_id, Nombre) VALUES (@id, 'Desconocido');";


        //public const string SqlInsertarDiaCalendario = "INSERT INTO DiasCalendario " +
        //    "(IdCalendario, Dia, DiaFecha, Grafico, Codigo, ExcesoJornada, FacturadoPaqueteria, Limpieza, GraficoVinculado, Notas) " +
        //    "VALUES " +
        //    "(@idCalendario, @dia, @diaFecha, @grafico, @codigo, @excesoJornada, @facturadoPaqueteria, @limpieza, @graficoVinculado, @notas);";


        //public const string SqlInsertarFestivo = "INSERT INTO Festivos (Año, Fecha) VALUES (@año, @fecha);";


        //public const string SqlInsertarGrafico = "INSERT INTO Graficos " +
        //    "(IdGrupo, NoCalcular, Numero, Turno, DescuadreInicio, Inicio, Final, DescuadreFinal, InicioPartido, FinalPartido, " +
        //    "Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, PlusLimpieza, PlusPaqueteria) " +
        //    "VALUES " +
        //    "(@idGrupo, @noCalcular, @numero, @turno, @descuadreInicio, @inicio, @final, @descuadreFinal, @inicioPartido, @finalPartido, " +
        //    "@valoracion, @trabajadas, @acumuladas, @nocturnas, @desayuno, @comida, @cena, @plusCena, @plusLimpieza, @plusPaqueteria);";


        //public const string SqlInsertarGrupo = "INSERT INTO GruposGraficos (Validez, Notas) VALUES (@validez, @notas);";


        //public const string SqlInsertarItinerario = "INSERT INTO Itinerarios " +
        //    "(IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago) " +
        //    "VALUES " +
        //    "(@idLinea, @nombre, @descripcion, @tiempoReal, @tiempoPago);";


        //public const string SqlInsertarLinea = "INSERT INTO Lineas (Nombre, Descripcion) VALUES (@nombre, @descripcion);";


        //public const string SqlInsertarParada = "INSERT INTO Paradas " +
        //    "(IdItinerario, Orden, Descripcion, Tiempo) " +
        //    "VALUES " +
        //    "(@idItinerario, @orden, @descripcion, @tiempo);";


        //public const string SqlInsertarRegulacion = "INSERT INTO Regulaciones " +
        //    "(IdConductor, Codigo, Fecha, Horas, Descansos, Motivo) " +
        //    "VALUES " +
        //    "(@idConductor, @codigo, @fecha, @horas, @descansos, @motivo);";


        //public const string SqlInsertarValoracion = "INSERT INTO Valoraciones " +
        //    "(IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo) " +
        //    "VALUES " +
        //    "(@idGrafico, @inicio, @linea, @descripcion, @final, @tiempo);";


        //public const string SqlBorrarCalendario = "DELETE * FROM Calendarios WHERE _id=@id;";


        //public const string SqlBorrarConductor = "DELETE * FROM Conductores WHERE _id=@id;";


        //public const string SqlBorrarDiaCalendario = "DELETE * FROM DiasCalendario WHERE _id=@id;";


        //public const string SqlBorrarFestivo = "DELETE * FROM Festivos WHERE _id=@id;";


        //public const string SqlBorrarGrafico = "DELETE * FROM Graficos WHERE _id=@id;";


        //public const string SqlBorrarGrupo = "DELETE * FROM GruposGraficos WHERE _id=@id;";


        //public const string SqlBorrarItinerario = "DELETE * FROM Itinerarios WHERE _id=@id;";


        //public const string SqlBorrarLinea = "DELETE * FROM Lineas WHERE _id=@id;";


        //public const string SqlBorrarParada = "DELETE * FROM Paradas WHERE _id=@id;";


        //public const string SqlBorrarRegulacion = "DELETE * FROM Regulaciones WHERE _id=@id;";


        //public const string SqlBorrarValoracion = "DELETE * FROM Valoraciones WHERE _id=@id;";


        // CONSULTA NO FUNCIONAL
        //public const string SqlConsultaPrueba = "SELECT (Sum(Graficos.Inicio) + Sum(Calendarios._id)) AS Resultado FROM Graficos, Calendarios WHERE Graficos._id = 123 AND Calendarios._id = 190;";


        //public const string SqlExisteConductor = "SELECT Count(_id) FROM Conductores WHERE _id=@id;";


        //public const string SqlExisteGrupo = "SELECT Count(*) FROM GruposGraficos WHERE Validez=@validez;";


        //public const string SqlGetCalendarios = "SELECT Calendarios.*, Conductores.Indefinido " +
        //    "FROM Calendarios " +
        //    "LEFT JOIN Conductores " +
        //    "ON Calendarios.IdConductor = Conductores._id " +
        //    "WHERE Year(Calendarios.Fecha)=@año And Month(Calendarios.Fecha)=@mes ORDER BY Calendarios.IdConductor;";


        //public const string SqlGetConductor = "SELECT * FROM Conductores WHERE _id=@id;";


        //public const string SqlGetConductores = "SELECT * FROM Conductores ORDER BY _id;";


        // CONSULTA NO FUNCIONAL
        //public const string SqlGetConsultas = "SELECT Name FROM MSysObjects WHERE Type = 5 ORDER BY Name;";


        //public const string SqlGetDiasCalendario = "SELECT * FROM DiasCalendario WHERE IdCalendario=@idCalendario ORDER BY Dia;";


        //public const string SqlGetDiasCalendarioAño = "SELECT * FROM DiasCalendario " +
        //    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @idConductor AND Year(Fecha) = @año);";


        //public const string SqlGetDiasCalendarioConBloqueos = "SELECT * FROM DiasCalendario " +
        //    "WHERE (ExcesoJornada <> 0 OR Descuadre <> 0) AND IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @idConductor);";


        //public const string SqlGetDiasPijama = "SELECT * FROM " +
        //    "(SELECT * FROM Graficos WHERE IdGrupo = " +
        //    "(SELECT _id FROM GruposGraficos WHERE Validez = " +
        //    "(SELECT Max(Validez) FROM GruposGraficos WHERE Validez <= @validez))) " +
        //    "WHERE Numero=@numero;";


        //public const string SqlGetEstadisticasGraficos = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
        //    "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
        //    "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
        //    "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
        //    "FROM GruposGraficos " +
        //    "LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
        //    "WHERE IdGrupo=@idGrupo GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        //public const string SqlGetEstadisticasGraficosDesdeFecha = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, " +
        //    "Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
        //    "Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, " +
        //    "Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria " +
        //    "FROM GruposGraficos " +
        //    "LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
        //    "WHERE GruposGraficos.Validez >= @fecha GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        //public const string SqlGetFestivos = "SELECT * FROM Festivos ORDER BY Fecha;";


        //public const string SqlGetFestivosPorAño = "SELECT * FROM Festivos WHERE Año=@año ORDER BY Fecha;";


        //public const string SqlGetFestivosPorMes = "SELECT * FROM Festivos WHERE Año=@año And Month(Fecha)=@mes ORDER BY Fecha;";


        //public const string SqlGetGraficos = "SELECT * FROM Graficos WHERE IdGrupo=@idGrupo ORDER BY Numero;";


        //public const string SqlGetGraficosGrupoPorFecha = "SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id FROM GruposGraficos WHERE Validez = @validez);";


        //public const string SqlGetGrupos = "SELECT * FROM GruposGraficos ORDER BY Validez DESC;";


        //public const string SqlGetItinerarioPorNombre = "SELECT * FROM Itinerarios WHERE Name=@nombre;";


        //public const string SqlGetItinerarios = "SELECT * FROM Itinerarios WHERE IdLinea=@ ORDER BY Nombre;";


        //public const string SqlGetLineas = "SELECT * FROM Lineas ORDER BY Nombre;";


        //public const string SqlGetParadas = "SELECT * FROM Paradas WHERE IdItinerario=@idItinerario ORDER BY Orden;";


        //public const string SqlGetRegulaciones = "SELECT * FROM Regulaciones WHERE IdConductor=@idConductor ORDER BY Fecha, _id;";


        //public const string SqlGetValoraciones = "SELECT * FROM Valoraciones WHERE IdGrafico=@idGrafico ORDER BY Inicio, _id;";


        // CONSULTA NO FUNCIONAL
        //public const string SqlTemporal = "SELECT DiasCalendario.Dia, Calendarios.IdConductor, DiasCalendario.Fecha, DiasCalendario.ExcesoJornada FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario WHERE Calendarios.IdConductor = 450 AND ExcesoJornada > 0;";











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
                DateTime fecha = new DateTime(año, mes, 1);
                var consulta = new SQLiteExpression("SELECT * FROM Calendarios WHERE strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) ORDER BY IdConductor;");
                consulta.AddParameter("@fecha", fecha);
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
                string comandoSQL = "SELECT * FROM Calendarios " +
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
                string comandoSQL = "SELECT Calendarios.* FROM Calendarios " +
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


        public TimeSpan GetAcumuladasHastaMes(int año, int mes, int idconductor, int comodin) {
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
                                if (graficoVinculado != 0 && grafico == comodin) grafico = graficoVinculado;
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

            string comandoSQL = "SELECT * FROM DiasCalendario WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE IdConductor = @matricula) AND strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha);";
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


        // ====================================================================================================
        #region BD FESTIVOS
        // ====================================================================================================

        public IEnumerable<Festivo> GetFestivos() {
            try {
                return GetItems<Festivo>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetFestivos), ex);
            }
            return new List<Festivo>();
        }


        public void GuardarFestivos(IEnumerable<Festivo> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarFestivos), ex);
            }
        }


        public void BorrarFestivos(IEnumerable<Festivo> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarFestivos), ex);
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD GRÁFICOS
        // ====================================================================================================

        public IEnumerable<Grafico> GetGraficos() {
            try {
                return GetItems<Grafico>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficos), ex);
            }
            return new List<Grafico>();
        }


        public void GuardarGraficos(IEnumerable<Grafico> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarGraficos), ex);
            }
        }


        public int InsertarGrafico(Grafico item) {
            try {
                return GuardarItem(item);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.InsertarGrafico), ex);
            }
            return -1;
        }


        public void BorrarGraficos(IEnumerable<Grafico> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGraficos), ex);
            }
        }


        public IEnumerable<Grafico> GetGraficosGrupoPorFecha(DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id FROM GruposGraficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez))");
                consulta.AddParameter("@validez", fecha);
                return GetItems<Grafico>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficosGrupoPorFecha), ex);
            }
            return new List<Grafico>();
        }


        public IEnumerable<EstadisticasGraficos> GetEstadisticasGrupoGraficos(long IdGrupo, TimeSpan jornadaMedia) {
            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "GruposGraficos.Validez AS xValidez, " +
                    "Turno AS xTurno, " +
                    "Count(Numero) AS xNumero, " +
                    "Sum(Valoracion) AS xValoracion, " +
                    //"Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
                    "Sum(CASE WHEN Trabajadas<@jornadaMedia AND NOT NoCalcular THEN @jornadaMedia ELSE Trabajadas END) AS xTrabajadas, " + //Si no va, probar 'END xx'
                    "Sum(Acumuladas) AS xAcumuladas, " +
                    "Sum(Nocturnas) AS xNocturnas, " +
                    "Sum(Desayuno) AS xDesayuno, " +
                    "Sum(Comida) AS xComida, " +
                    "Sum(Cena) AS xCena, " +
                    "Sum(PlusCena) AS xPlusCena, " +
                    "Sum(PlusLimpieza) AS xLimpieza, " +
                    "Sum(PlusPaqueteria) AS xPaqueteria " +
                    "FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
                    "WHERE IdGrupo=@idGrupo GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;");
                consulta.AddParameter("@jornadaMedia", jornadaMedia);
                consulta.AddParameter("@idGrupo", IdGrupo);
                return GetItems<EstadisticasGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGrupoGraficos), ex);
            }
            return new List<EstadisticasGraficos>();
        }


        public IEnumerable<EstadisticasGraficos> GetEstadisticasGraficosDesdeFecha(DateTime fecha, TimeSpan jornadaMedia) {
            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "GruposGraficos.Validez AS xValidez, " +
                    "Turno AS xTurno, " +
                    "Count(Numero) AS xNumero, " +
                    "Sum(Valoracion) AS xValoracion, " +
                    //"Sum(IIf(Trabajadas<@jornadaMedia AND NOT NoCalcular,@jornadaMedia,Trabajadas)) AS xTrabajadas, " +
                    "Sum(CASE WHEN Trabajadas<@jornadaMedia AND NOT NoCalcular THEN @jornadaMedia ELSE Trabajadas END) AS xTrabajadas, " + //Si no va, probar 'END xx'
                    "Sum(Acumuladas) AS xAcumuladas, " +
                    "Sum(Nocturnas) AS xNocturnas, " +
                    "Sum(Desayuno) AS xDesayuno, " +
                    "Sum(Comida) AS xComida, " +
                    "Sum(Cena) AS xCena, " +
                    "Sum(PlusCena) AS xPlusCena, " +
                    "Sum(PlusLimpieza) AS xLimpieza, " +
                    "Sum(PlusPaqueteria) AS xPaqueteria " +
                    "FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
                    "WHERE strftime('%Y-%m-%d', GruposGraficos.Validez) = strftime('%Y-%m-%d', @fecha) " +
                    "GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;");
                consulta.AddParameter("@jornadaMedia", jornadaMedia);
                consulta.AddParameter("@idGrupo", fecha);
                return GetItems<EstadisticasGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
            }
            return new List<EstadisticasGraficos>();
        }


        public GraficoBase GetGrafico(int numero, DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT * " +
                                    "FROM (SELECT * " +
                                    "      FROM Graficos" +
                                    "      WHERE IdGrupo = (SELECT _id " +
                                    "                       FROM GruposGraficos " +
                                    "                       WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) " +
                                    "                                        FROM GruposGraficos " +
                                    "                                        WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez)))))" +
                                    "WHERE Numero = @idConductor");
                consulta.AddParameter("@validez", fecha);
                consulta.AddParameter("@idConductor", numero);
                return GetItem<GraficoBase>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
            }
            return new GraficoBase();
        }







        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD GRUPOS GRÁFICOS
        // ====================================================================================================

        public IEnumerable<GrupoGraficos> GetGrupos() {
            try {
                return GetItems<GrupoGraficos>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGrupos), ex);
            }
            return new List<GrupoGraficos>();
        }


        public void GuardarGrupos(IEnumerable<GrupoGraficos> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarGrupos), ex);
            }
        }


        public void BorrarGrupoPorId(int idGrupo) {
            try {
                var consulta = new SQLiteExpression("DELETE FROM GruposGraficos WHERE _id=@id");
                consulta.AddParameter("@id", idGrupo);
                ExecureNonQuery(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGrupoPorId), ex);
            }
        }


        public int NuevoGrupo(DateTime fecha, string notas) {
            try {
                var nuevoGrupo = new GrupoGraficos();
                nuevoGrupo.Validez = fecha;
                nuevoGrupo.Notas = notas;
                return GuardarItem(nuevoGrupo);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.NuevoGrupo), ex);
            }
            return -1;
        }


        public bool ExisteGrupo(DateTime fecha) {
            try {
                var whereCondition = new SQLiteExpression("strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez)").AddParameter("@validez", fecha);
                return GetCount<GrupoGraficos>(whereCondition) > 0;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.ExisteConductor), ex);
            }
            return false;
        }


        public GrupoGraficos GetUltimoGrupo() {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM GruposGraficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) FROM GruposGraficos));");
                return GetItem<GrupoGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGrupoPorId), ex);
            }
            return null;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD PLUSES
        // ====================================================================================================

        public IEnumerable<Pluses> GetPluses() {
            try {
                return GetItems<Pluses>();
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetPluses), ex);
            }
            return new List<Pluses>();
        }


        public void GuardarPluses(IEnumerable<Pluses> lista) {
            try {
                GuardarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GuardarPluses), ex);
            }
        }






        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD REGULACIONES CONDUCTOR
        // ====================================================================================================

        public void BorrarRegulaciones(IEnumerable<RegulacionConductor> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarRegulaciones), ex);
            }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD VALORACIONES GRÁFICOS
        // ====================================================================================================

        public IEnumerable<ValoracionGrafico> GetValoraciones(int idGrafico) {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM Valoraciones WHERE IdGrafico=@idGrafico ORDER BY Inicio, _id");
                consulta.AddParameter("@idGrafico", idGrafico);
                return GetItems<ValoracionGrafico>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetValoraciones), ex);
            }
            return new List<ValoracionGrafico>();
        }


        public void InsertarValoracion(ValoracionGrafico valoracion) {
            try {
                GuardarItem(valoracion);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.InsertarValoracion), ex);
            }
        }


        public void BorrarValoraciones(IEnumerable<ValoracionGrafico> lista) {
            try {
                BorrarItems(lista);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarValoraciones), ex);
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
        #region ITINERARIOS
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


        // ====================================================================================================
        #region BD ESTADÍSTICAS
        // ====================================================================================================

        public EstadisticaGrupoGraficos GetEstadisticasUltimoGrupoGraficos(Centros centro) {

            try {
                var consulta = new SQLiteExpression("SELECT  " +
                    "GG.Validez, " +
                    "Count(G._id) as Cantidad, " +

                    //"Count(IIf(G.Turno = 1, 1, null)) as Turnos1, " +
                    "Sum(CASE G.Turno WHEN 1 THEN 1 ELSE null END) AS Turnos1, " + //Si no va, probar 'END t1'
                                                                                   //"Count(IIf(G.Turno = 2, 1, null)) as Turnos2, " +
                    "Sum(CASE G.Turno WHEN 2 THEN 1 ELSE null END) AS Turnos2, " + //Si no va, probar 'END t2'
                                                                                   //"Count(IIf(G.Turno = 3, 1, null)) as Turnos3, " +
                    "Sum(CASE G.Turno WHEN 3 THEN 1 ELSE null END) AS Turnos3, " + //Si no va, probar 'END t3'
                                                                                   //"Count(IIf(G.Turno = 4, 1, null)) as Turnos4, " +
                    "Sum(CASE G.Turno WHEN 4 THEN 1 ELSE null END) AS Turnos4, " + //Si no va, probar 'END t4'

                    "Sum(G.Valoracion) As Valoraciones, " +
                    "Sum(G.Trabajadas) As H_Trabajadas, " +

                    //"Sum(IIf(G.Turno = 1, G.Trabajadas, 0)) As TrabajadasTurno1, " +
                    "Sum(CASE G.Turno WHEN 1 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno1, " + //Si no va, probar 'END tt1'
                                                                                                    //"Sum(IIf(G.Turno = 2, G.Trabajadas, 0)) As TrabajadasTurno2, " +
                    "Sum(CASE G.Turno WHEN 2 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno2, " + //Si no va, probar 'END tt2'
                                                                                                    //"Sum(IIf(G.Turno = 3, G.Trabajadas, 0)) As TrabajadasTurno3, " +
                    "Sum(CASE G.Turno WHEN 3 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno3, " + //Si no va, probar 'END tt3'
                                                                                                    //"Sum(IIf(G.Turno = 4, G.Trabajadas, 0)) As TrabajadasTurno4, " +
                    "Sum(CASE G.Turno WHEN 4 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno4, " + //Si no va, probar 'END tt4'

                    "Sum(G.Acumuladas) As H_Acumuladas, " +
                    "Sum(G.Nocturnas) As H_Nocturnas, " +
                    "Sum(G.Desayuno) As Desayunos, " +
                    "Sum(G.Comida) As Comidas, " +
                    "Sum(G.Cena) As Cenas, " +
                    "Sum(G.PlusCena) As PlusesCena " +
                    "FROM Graficos G LEFT JOIN GruposGraficos GG ON G.IdGrupo = GG._id " +
                    "WHERE strftime('%Y-%m-%d', GG.Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) FROM GruposGraficos))" +
                    "GROUP BY GG.Validez " +
                    "ORDER BY GG.Validez");
                return GetItem<EstadisticaGrupoGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasUltimoGrupoGraficos), ex);
            }
            return new EstadisticaGrupoGraficos();
        }


        public EstadisticaGrupoGraficos GetEstadisticasGrupoGraficos(long idGrupo, Centros centro) {

            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "GG.Validez, " +
                    "Count(G._id) as Cantidad, " +

                    //"Count(IIf(G.Turno = 1, 1, null)) as Turnos1, " +
                    "Sum(CASE G.Turno WHEN 1 THEN 1 ELSE null END) AS Turnos1, " + //Si no va, probar 'END t1'
                                                                                   //"Count(IIf(G.Turno = 2, 1, null)) as Turnos2, " +
                    "Sum(CASE G.Turno WHEN 2 THEN 1 ELSE null END) AS Turnos2, " + //Si no va, probar 'END t2'
                                                                                   //"Count(IIf(G.Turno = 3, 1, null)) as Turnos3, " +
                    "Sum(CASE G.Turno WHEN 3 THEN 1 ELSE null END) AS Turnos3, " + //Si no va, probar 'END t3'
                                                                                   //"Count(IIf(G.Turno = 4, 1, null)) as Turnos4, " +
                    "Sum(CASE G.Turno WHEN 4 THEN 1 ELSE null END) AS Turnos4, " + //Si no va, probar 'END t4'

                    "Sum(G.Valoracion) As Valoraciones, " +
                    "Sum(G.Trabajadas) As H_Trabajadas, " +

                    //"Sum(IIf(G.Turno = 1, G.Trabajadas, 0)) As TrabajadasTurno1, " +
                    "Sum(CASE G.Turno WHEN 1 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno1, " + //Si no va, probar 'END tt1'
                                                                                                    //"Sum(IIf(G.Turno = 2, G.Trabajadas, 0)) As TrabajadasTurno2, " +
                    "Sum(CASE G.Turno WHEN 2 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno2, " + //Si no va, probar 'END tt2'
                                                                                                    //"Sum(IIf(G.Turno = 3, G.Trabajadas, 0)) As TrabajadasTurno3, " +
                    "Sum(CASE G.Turno WHEN 3 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno3, " + //Si no va, probar 'END tt3'
                                                                                                    //"Sum(IIf(G.Turno = 4, G.Trabajadas, 0)) As TrabajadasTurno4, " +
                    "Sum(CASE G.Turno WHEN 4 THEN G.Trabajadas ELSE 0 END) AS TrabajadasTurno4, " + //Si no va, probar 'END tt4'

                    "Sum(G.Acumuladas) As H_Acumuladas, " +
                    "Sum(G.Nocturnas) As H_Nocturnas, " +
                    "Sum(G.Desayuno) As Desayunos, " +
                    "Sum(G.Comida) As Comidas, " +
                    "Sum(G.Cena) As Cenas, " +
                    "Sum(G.PlusCena) As PlusesCena " +
                    "FROM Graficos G LEFT JOIN GruposGraficos GG ON G.IdGrupo = GG._id " +
                    "WHERE GG._id = @id " +
                    "GROUP BY GG.Validez " +
                    "ORDER BY GG.Validez");
                consulta.AddParameter("@id", idGrupo);
                return GetItem<EstadisticaGrupoGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGrupoGraficos), ex);
            }
            return new EstadisticaGrupoGraficos();
        }


        //TODO: Este método no se puede procesar, porque se añade un campo no previsto (campo fecha). Modificar Model GraficoFecha.
        public IEnumerable<GraficoFecha> GetGraficosFromDiaCalendario(DateTime fecha, int comodin) {
            try {
                var consulta = new SQLiteExpression("SELECT @fecha AS Fecha, * " +
                                    "FROM(SELECT * " +
                                    "     FROM Graficos " +
                                    "     WHERE IdGrupo = (SELECT _id " +
                                    "                      FROM GruposGraficos " +
                                    "                      WHERE Validez = (SELECT Max(Validez) " +
                                    "                                       FROM GruposGraficos " +
                                    "                                       WHERE Validez <= @fecha))) " +
                                    "WHERE(Numero IN(SELECT Grafico " +
                                    "                FROM DiasCalendario " +
                                    "                WHERE DiaFecha = @fecha AND Grafico > 0) " +
                                    "OR Numero IN(SELECT GraficoVinculado " +
                                    "             FROM DiasCalendario " +
                                    "             WHERE DiaFecha = @fecha AND GraficoVinculado > 0)) " +
                                    "AND Numero <> @comodin " +
                                    "ORDER BY Numero");
                consulta.AddParameter("@fecha", fecha);
                consulta.AddParameter("@comodin", comodin);
                return GetItems<GraficoFecha>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficosFromDiaCalendario), ex);
            }
            return new List<GraficoFecha>();
        }


        public IEnumerable<GraficosPorDia> GetGraficosByDia(DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT Numero " +
                                    "FROM Graficos " +
                                    "WHERE IdGrupo = (SELECT _id " +
                                    "                 FROM GruposGraficos " +
                                    "                 WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) " +
                                    "                                  FROM GruposGraficos " +
                                    "                                  WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @fecha)))) " +
                                    "AND DiaSemana = @diaSemana " +
                                    "ORDER BY Numero");

                List<GraficosPorDia> lista = new List<GraficosPorDia>();

                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    string diasemana;
                    switch (fechaDia.DayOfWeek) {
                        case DayOfWeek.Sunday:
                            diasemana = "F";
                            break;
                        case DayOfWeek.Saturday:
                            diasemana = "S";
                            break;
                        case DayOfWeek.Friday:
                            diasemana = "V";
                            break;
                        default:
                            diasemana = "L";
                            if (App.Global.CalendariosVM.EsFestivo(fechaDia.AddDays(1))) diasemana = "V";
                            break;
                    }
                    if (App.Global.CalendariosVM.EsFestivo(fechaDia)) {
                        diasemana = "F";
                    }
                    consulta.AddParameter("@fecha", fechaDia);
                    consulta.AddParameter("@diaSemana", diasemana);
                    var Gpd = new GraficosPorDia();
                    Gpd.Fecha = fechaDia;
                    Gpd.Lista.Add(GetIntScalar(consulta));
                    lista.Add(Gpd);
                }
                return lista;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficosByDia), ex);
            }
            return new List<GraficosPorDia>();
        }


        public List<DescansosPorDia> GetDescansosByDia(DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT Count(*) " +
                                    "FROM DiasCalendario " +
                                    "WHERE strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha) AND (Grafico = -2 OR Grafico = -3)");

                List<DescansosPorDia> lista = new List<DescansosPorDia>();

                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    consulta.AddParameter("@fecha", fechaDia);
                    var Dpd = new DescansosPorDia();
                    Dpd.Fecha = fechaDia;
                    Dpd.Descansos = GetIntScalar(consulta);
                    lista.Add(Dpd);
                }
                return lista;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosByDia), ex);
            }
            return new List<DescansosPorDia>();
        }









        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD PIJAMAS
        // ====================================================================================================

        public IEnumerable<Pijama.DiaPijama> GetDiasPijama(IEnumerable<DiaCalendarioBase> listadias, int comodin) {
            // Creamos la lista que se devolverá.
            var lista = new List<Pijama.DiaPijama>();
            try {
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    foreach (var dia in listadias) {
                        // Creamos el día pijama a añadir a la lista.
                        Pijama.DiaPijama diaPijama = new Pijama.DiaPijama(dia);
                        // Si el día no pertenece al mes, continuamos el bucle al siguiente día.
                        if (dia.Dia > DateTime.DaysInMonth(dia.DiaFecha.Year, dia.DiaFecha.Month)) continue;
                        // Establecemos el gráfico a buscar, por si está seleccionado el comodín.
                        int GraficoBusqueda = dia.Grafico;
                        if (dia.GraficoVinculado != 0 && dia.Grafico == comodin) GraficoBusqueda = dia.GraficoVinculado;
                        // Creamos el comando SQL y añadimos los parámetros
                        var consulta = new SQLiteExpression("SELECT * " +
                                                  "FROM (SELECT * " +
                                                  "      FROM Graficos" +
                                                  "      WHERE IdGrupo = (SELECT _id " +
                                                  "                       FROM GruposGraficos " +
                                                  "                       WHERE Validez = (SELECT Max(Validez) " +
                                                  "                                        FROM GruposGraficos " +
                                                  "                                        WHERE Validez <= @validez)))" +
                                                  "WHERE Numero = @numero");
                        consulta.AddParameter("@validez", dia.DiaFecha);
                        consulta.AddParameter("@numero", GraficoBusqueda);
                        // Ejecutamos el comando y extraemos el gráfico.
                        var grafico = GetItem<GraficoBase>(conexion, consulta, true);
                        if (grafico == null) grafico = new GraficoBase() { Numero = GraficoBusqueda };
                        diaPijama.GraficoTrabajado = new GraficoBase(grafico);
                        diaPijama.GraficoOriginal = new GraficoBase(grafico);
                        // Modificamos los parámetros del gráfico trabajado en función de si existen en el DiaCalendarioBase.
                        if (dia.TurnoAlt.HasValue) diaPijama.GraficoTrabajado.Turno = dia.TurnoAlt.Value;
                        if (dia.InicioAlt.HasValue) diaPijama.GraficoTrabajado.Inicio = new TimeSpan(dia.InicioAlt.Value.Ticks);
                        if (dia.FinalAlt.HasValue) diaPijama.GraficoTrabajado.Final = new TimeSpan(dia.FinalAlt.Value.Ticks);
                        if (dia.InicioPartidoAlt.HasValue) diaPijama.GraficoTrabajado.InicioPartido = new TimeSpan(dia.InicioPartidoAlt.Value.Ticks);
                        if (dia.FinalPartidoAlt.HasValue) diaPijama.GraficoTrabajado.FinalPartido = new TimeSpan(dia.FinalPartidoAlt.Value.Ticks);
                        if (dia.TrabajadasAlt.HasValue) diaPijama.GraficoTrabajado.Trabajadas = new TimeSpan(dia.TrabajadasAlt.Value.Ticks);
                        if (dia.AcumuladasAlt.HasValue) diaPijama.GraficoTrabajado.Acumuladas = new TimeSpan(dia.AcumuladasAlt.Value.Ticks);
                        if (dia.NocturnasAlt.HasValue) diaPijama.GraficoTrabajado.Nocturnas = new TimeSpan(dia.NocturnasAlt.Value.Ticks);
                        if (dia.DesayunoAlt.HasValue) diaPijama.GraficoTrabajado.Desayuno = dia.DesayunoAlt.Value;
                        if (dia.ComidaAlt.HasValue) diaPijama.GraficoTrabajado.Comida = dia.ComidaAlt.Value;
                        if (dia.CenaAlt.HasValue) diaPijama.GraficoTrabajado.Cena = dia.CenaAlt.Value;
                        if (dia.PlusCenaAlt.HasValue) diaPijama.GraficoTrabajado.PlusCena = dia.PlusCenaAlt.Value;
                        if (dia.PlusLimpiezaAlt.HasValue) diaPijama.GraficoTrabajado.PlusLimpieza = dia.PlusLimpiezaAlt.Value;
                        if (dia.PlusPaqueteriaAlt.HasValue) diaPijama.GraficoTrabajado.PlusPaqueteria = dia.PlusPaqueteriaAlt.Value;
                        // Añadimos el día pijama a la lista.
                        lista.Add(diaPijama);

                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasPijama), ex);
            }
            return lista;
        }


        public ResumenPijama GetResumenHastaMes(int año, int mes, int idconductor, int comodin) {
            // Inicializamos el resultado.
            ResumenPijama resultado = new ResumenPijama();
            // Establecemos la fecha del día 1 del siguiente mes al indicado.
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            try {
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    //----------------------------------------------------------------------------------------------------
                    // HORAS ACUMULADAS
                    //----------------------------------------------------------------------------------------------------
                    var consulta = new SQLiteExpression("SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha, " +
                                                      "DiasCalendario.ExcesoJornada, DiasCalendario.AcumuladasAlt " +
                                                      "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios._id " +
                                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE Fecha < @fecha AND IdConductor = @idConductor) " +
                                                      "      AND Grafico > 0 " +
                                                      "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;");
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@idConductor", idconductor);
                    using (var lector = consulta.GetCommand(conexion).ExecuteReader()) {
                        // Por cada día, sumamos las horas acumuladas.
                        while (lector.Read()) {
                            int d = (lector["Dia"] is DBNull) ? 0 : (Int16)lector["Dia"];
                            int g = (lector["Grafico"] is DBNull) ? 0 : (Int16)lector["Grafico"];
                            int v = (lector["GraficoVinculado"] is DBNull) ? 0 : (Int16)lector["GraficoVinculado"];
                            TimeSpan? acumuladasAlt = lector.ToTimeSpanNulable("AcumuladasAlt");
                            TimeSpan ej = lector.ToTimeSpan("ExcesoJornada");
                            if (v != 0 && g == comodin) g = v;
                            DateTime f = (lector["Fecha"] is DBNull) ? new DateTime(0) : (DateTime)lector["Fecha"];
                            if (d > DateTime.DaysInMonth(f.Year, f.Month)) continue;
                            DateTime fechadia = new DateTime(f.Year, f.Month, d);
                            var consulta2 = new SQLiteExpression("SELECT * " +
                                                  "FROM (SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id " +
                                                  "										         FROM GruposGraficos " +
                                                  "												 WHERE Validez = (SELECT Max(Validez) " +
                                                  "																  FROM GruposGraficos " +
                                                  "														          WHERE Validez <= @validez)))" +
                                                  "WHERE Numero = @numero");
                            consulta2.AddParameter("validez", fechadia);
                            consulta2.AddParameter("numero", g);
                            var grafico = GetItem<GraficoBase>(conexion, consulta2, true);
                            if (grafico == null) {
                                grafico = new GraficoBase();
                            } else {
                                grafico.Final += ej;
                            }
                            if (acumuladasAlt.HasValue) grafico.Acumuladas = acumuladasAlt.Value;
                            resultado.HorasAcumuladas += grafico.Acumuladas;
                        }
                    }

                    //----------------------------------------------------------------------------------------------------
                    // HORAS REGULADAS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.HorasReguladas = GetTimeSpanScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DIAS F6
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                              "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE Fecha < @fecha AND IdConductor = @idConductor)" +
                                              "      AND Grafico = -7;");
                    consulta.AddParameter("fecha", fecha);
                    consulta.AddParameter("idconductor", idconductor);
                    resultado.DiasLibreDisposicionF6 = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DIAS F6DC
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                              "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE Fecha < @fecha AND IdConductor = @idConductor)" +
                                              "      AND Grafico = -14;");
                    consulta.AddParameter("fecha", fecha);
                    consulta.AddParameter("idconductor", idconductor);
                    resultado.DiasF6DC = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DCS REGULADOS 
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = @idConductor AND Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.DCsRegulados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DCS DISFRUTADOS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE Fecha < @fecha AND IdConductor = @idConductor)" +
                                                      "      AND Grafico = -6;");
                    consulta.AddParameter("fecha", fecha);
                    consulta.AddParameter("idconductor", idconductor);
                    resultado.DCsDisfrutados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DNDs REGULADOS 
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Dnds) FROM Regulaciones WHERE IdConductor = @idConductor AND Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.DNDsRegulados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DNDS DISFRUTADOS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                       "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                       "WHERE Calendarios.IdConductor = @idConductor AND DiasCalendario.Grafico = -8 AND Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.DNDsDisfrutados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS COMITÉ EN DESCANSO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.IdConductor = @idConductor AND " +
                                                      "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5 OR " +
                                                      "       DiasCalendario.Grafico = -6 OR DiasCalendario.Grafico = -1) AND " +
                                                      "	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
                                                      "      Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.DiasComiteEnDescanso = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS TRABAJO EN DESCANSO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                       "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                       "WHERE Calendarios.IdConductor = @idConductor AND " +
                                                       "      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND Fecha < @fecha");
                    consulta.AddParameter("idconductor", idconductor);
                    consulta.AddParameter("fecha", fecha);
                    resultado.DiasTrabajoEnDescanso = GetDecimalScalar(conexion, consulta);

                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetResumenHastaMes), ex);
            }
            return resultado;
        }


        [Obsolete("Ya hay un método que hace esto en el apartado Calendarios.")]
        public TimeSpan GetHorasCobradasMes2(int año, int mes, int idconductor) {
            //Referirse al GetHorasCobradasMes de Calendarios
            return TimeSpan.Zero;
        }


        [Obsolete("Ya hay un método que hace esto en el apartado Calendarios.")]
        public TimeSpan GetHorasCobradasAño2(int año, int mes, int idconductor) {
            //Referirse al GetHorasCobradasAño de Calendarios
            return TimeSpan.Zero;
        }


        [Obsolete("Ya hay un método que hace esto en el apartado Calendarios.")]
        public static TimeSpan GetHorasCambiadasPorDCsMes2(int año, int mes, int idconductor) {
            //Referirse al GetHorasCambiadasPorDCsMes de Calendarios
            return TimeSpan.Zero;
        }


        [Obsolete("Ya hay un método que hace esto en el apartado Calendarios.")]
        public TimeSpan GetHorasReguladasMes2(int año, int mes, int idconductor) {
            //Referirse al GetHorasReguladasMes de Calendarios
            return TimeSpan.Zero;
        }


        [Obsolete("Ya hay un método que hace esto en el apartado Calendarios.")]
        public GraficoBase GetGrafico2(int numero, DateTime validez) {
            //Referirse al GetGrafico de Calendarios
            return null;
        }


        public int GetDiasTrabajadosHastaMesEnAño(int año, int mes, int idconductor) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        IdConductor = @idConductor) " +
                                    "      AND Grafico > 0;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@idConductor", idconductor);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasTrabajadosHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasDescansoHastaMesEnAño(int año, int mes, int idconductor) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        IdConductor = @idConductor) " +
                                    "      AND (Grafico = -2 OR Grafico = -10 OR Grafico = -12);");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@idConductor", idconductor);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasDescansoHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasVacacionesHastaMesEnAño(int año, int mes, int idconductor) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        IdConductor = @idConductor) " +
                                    "      AND Grafico = -1;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@idConductor", idconductor);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasVacacionesHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasInactivoHastaMesEnAño(int año, int mes, int idconductor) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        IdConductor = @idConductor) " +
                                    "      AND Grafico = 0;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@idConductor", idconductor);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasInactivoHastaMesEnAño), ex);
            }
            return 0;
        }



        #endregion
        // ====================================================================================================







        /// <summary>
        /// Prueba de cómo se pueden acceder a dos bases de datos diferentes a la vez.
        /// </summary>
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
