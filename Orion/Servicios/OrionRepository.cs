#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
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
            "MatriculaConductor INTEGER DEFAULT 0, " +
            "Fecha TEXT, " +
            "Descuadre INTEGER DEFAULT 0, " +
            "ExcesoJornadaCobrada REAL DEFAULT 0, " +
            "Notas TEXT DEFAULT ''" +
            ");";


        public const string CrearTablaConductores = "CREATE TABLE IF NOT EXISTS Conductores (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Activo INTEGER DEFAULT 1, " +
            "Categoria TEXT DEFAULT 'C', " +
            "Matricula INTEGER DEFAULT 0, " +
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
            "Limpieza INTEGER DEFAULT NULL, " +
            "GraficoVinculado INTEGER DEFAULT 0, " +
            "TurnoAlt INTEGER DEFAULT NULL, " +
            "InicioAlt INTEGER DEFAULT NULL, " +
            "FinalAlt INTEGER DEFAULT NULL, " +
            "InicioPartidoAlt INTEGER DEFAULT NULL, " +
            "FinalPartidoAlt INTEGER DEFAULT NULL, " +
            "TrabajadasAlt INTEGER DEFAULT NULL, " +
            "AcumuladasAlt INTEGER DEFAULT NULL, " +
            "NocturnasAlt INTEGER DEFAULT NULL, " +
            "DesayunoAlt REAL DEFAULT NULL, " +
            "ComidaAlt REAL DEFAULT NULL, " +
            "CenaAlt REAL DEFAULT NULL, " +
            "PlusCenaAlt REAL DEFAULT NULL, " +
            "PlusLimpiezaAlt INTEGER DEFAULT NULL, " +
            "PlusPaqueteriaAlt INTEGER DEFAULT NULL, " +
            "Notas TEXT DEFAULT '', " +
            //NUEVAS
            "CategoriaGrafico TEXT DEFAULT 'C', " +
            "Turno INTEGER DEFAULT 0, " +
            "Inicio INTEGER DEFAULT NULL, " +
            "Final INTEGER DEFAULT NULL, " +
            "InicioPartido INTEGER DEFAULT NULL, " +
            "FinalPartido INTEGER DEFAULT NULL, " +
            "TrabajadasPartido INTEGER DEFAULT 0, " +
            "TiempoPartido INTEGER DEFAULT 0, " +
            "Valoracion INTEGER DEFAULT 0, " +
            "Trabajadas INTEGER DEFAULT 0, " +
            "TrabajadasConvenio INTEGER DEFAULT 0, " +
            "TrabajadasReales INTEGER DEFAULT 0, " +
            "TiempoVacio INTEGER DEFAULT 0, " +
            "Acumuladas INTEGER DEFAULT 0, " +
            "Nocturnas INTEGER DEFAULT 0, " +
            "Desayuno REAL DEFAULT 0, " +
            "Comida REAL DEFAULT 0, " +
            "Cena REAL DEFAULT 0, " +
            "PlusCena REAL DEFAULT 0, " +
            "PlusLimpieza INTEGER DEFAULT 0, " +
            "PlusPaqueteria INTEGER DEFAULT 0 " +

            ");";


        public const string CrearTablaFestivos = "CREATE TABLE IF NOT EXISTS Festivos (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Año INTEGER DEFAULT 0, " +
            "Fecha TEXT " +
            ");";


        public const string CrearTablaGraficos = "CREATE TABLE IF NOT EXISTS Graficos (" +
            "_id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "IdGrupo INTEGER DEFAULT 0, " +
            "Validez TEXT DEFAULT '', " +
            "NoCalcular INTEGER DEFAULT 0, " +
            "Categoria TEXT DEFAULT 'C', " +
            "Numero INTEGER DEFAULT 0, " +
            "DiaSemana TEXT DEFAULT '', " +
            "Turno INTEGER DEFAULT 0, " +
            "Inicio INTEGER DEFAULT NULL, " +
            "Final INTEGER DEFAULT NULL, " +
            "InicioPartido INTEGER DEFAULT NULL, " +
            "FinalPartido INTEGER DEFAULT NULL, " +
            "Valoracion INTEGER DEFAULT 0, " +
            "Trabajadas INTEGER DEFAULT 0, " +
            "TiempoVacio INTEGER DEFAULT 0, " +
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
            "Inicio INTEGER DEFAULT NULL, " +
            "Linea REAL DEFAULT 0, " +
            "Descripcion TEXT DEFAULT '', " +
            "Final INTEGER DEFAULT NULL, " +
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
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private const int DB_VERSION = 1;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public OrionRepository(string cadenaConexion) : base(cadenaConexion) {
            //Task.Run(async () => await InicializarBaseDatosAsync());
            // No podemos crear la base de datos de forma asíncrona, porque el programa intenta acceder a ella
            // mientras se está creando. Por eso debemos esperar...

            // No inicializamos las bases de datos desde aquí, ya que deben actualizarse las 5 bases de datos
            // a la vez, con lo que lo tengo que hacer yo desde la pestaña programador.
            if (CadenaConexion != null) InicializarBaseDatosAsync().Wait();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private async Task CrearTablasAsync() {
            using (var conexion = new SQLiteConnection(CadenaConexion)) {
                conexion.Open();
                using (var comando = new SQLiteCommand(CrearTablaCalendarios, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaConductores, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaDiasCalendario, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaFestivos, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaGraficos, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaGruposGraficos, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaPluses, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaRegulaciones, conexion)) await comando.ExecuteNonQueryAsync();
                using (var comando = new SQLiteCommand(CrearTablaValoraciones, conexion)) await comando.ExecuteNonQueryAsync();
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
        public async Task InicializarBaseDatosAsync() {
            int version = await GetDbVersionAsync();
            switch (version) {
                case 0: // BASE DE DATOS NUEVA: Creamos las tablas y actualizamos el número de versión al correcto.
                    await CrearTablasAsync();
                    await SetDbVersionAsync(DB_VERSION);

                    break;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD CALENDARIOS
        // ====================================================================================================


        public IEnumerable<Calendario> GetCalendarios(int año, int mes) {

            try {
                DateTime fecha = new DateTime(año, mes, 1);
                var consulta = new SQLiteExpression("SELECT * FROM Calendarios WHERE strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) ORDER BY MatriculaConductor;");
                consulta.AddParameter("@fecha", fecha);
                var lista = GetItems<Calendario>(consulta);
                return lista;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetCalendarios), ex);
            }
            return new List<Calendario>();
        }


        public IEnumerable<Calendario> GetCalendariosConductor(int año, int matricula) {

            try {
                DateTime fecha = new DateTime(año, 1, 1);
                string comandoSQL = "SELECT * FROM Calendarios " +
                    "WHERE strftime('%Y', Calendarios.Fecha) = strftime('%Y', @fecha) AND Calendarios.MatriculaConductor = @matricula " +
                    "ORDER BY Calendarios.Fecha;";
                var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
                var lista = GetItems<Calendario>(consulta);
                return lista;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetCalendariosConductor), ex);
            }
            return new List<Calendario>();
        }


        public List<Calendario> GetCalendariosConductor(int año, int mes, int matricula) {

            try {
                DateTime fecha = new DateTime(año, mes, 1);
                string comandoSQL = "SELECT Calendarios.* FROM Calendarios " +
                                    "WHERE strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Calendarios.MatriculaConductor = @matricula ;";
                var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
                var lista = GetItems<Calendario>(consulta);
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


        public int GetDescansosReguladosHastaMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosReguladosHastaMes), ex);
            }
            return 0;
        }


        public int GetDescansosReguladosAño(int año, int matricula) {
            string comandoSQL = "SELECT Sum(Descansos) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y', Fecha) = strftime('%Y', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, 1, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosReguladosAño), ex);
            }
            return 0;
        }


        public int GetDescansosDisfrutadosHastaMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula)" +
                                "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDescansosDisfrutadosHastaMes), ex);
            }
            return 0;
        }


        public int GetDCDisfrutadosAño(int matricula, int año) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND MatriculaConductor = @matricula)" +
                                "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";

            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDCDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDCDisfrutadosAño(int matricula, int año, int mes) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios " +
                                      "WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND strftime('%m', Fecha) <= strftime('%m', @fecha) AND MatriculaConductor = @matricula)" +
                                      "      AND Grafico = -6 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDCDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDiasF6HastaMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                            "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula)" +
                            "      AND Grafico = -7; ";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasF6HastaMes), ex);
            }
            return 0;
        }


        public TimeSpan GetHorasReguladasHastaMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasHastaMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasReguladasMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 0;";
            DateTime fecha = new DateTime(año, mes, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCambiadasPorDCsMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, mes, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCambiadasPorDCsMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasReguladasAño(int año, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y', Fecha) = strftime('%Y', @fecha) AND Codigo = 2;";
            DateTime fecha = new DateTime(año, 1, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasReguladasAño), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCobradasMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m', Fecha) = strftime('%Y-%m', @fecha) AND Codigo = 1;";
            DateTime fecha = new DateTime(año, mes, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCobradasMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetHorasCobradasAño(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND Codigo = 1;";
            DateTime fechainicio = mes == 12 ? new DateTime(año, 11, 30) : new DateTime(año - 1, 11, 30);
            DateTime fechafinal = mes == 12 ? new DateTime(año + 1, 12, 1) : new DateTime(año, 12, 1);
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@idConductor", idConductor)
                .AddParameter("@fechaInicio", fechainicio)
                .AddParameter("@fechaFinal", fechafinal);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetHorasCobradasAño), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetExcesoJornadaCobradaHastaMes(int año, int mes, int matricula) {
            string comandoSQL = "SELECT Sum(ExcesoJornadaCobrada) FROM Calendarios WHERE MatriculaConductor = @matricula AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", matricula).AddParameter("@fecha", fecha);
            try {
                return GetTimeSpanScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetExcesoJornadaCobradaHastaMes), ex);
            }
            return TimeSpan.Zero;
        }


        public TimeSpan GetAcumuladasHastaMes(int año, int mes, int matricula, int comodin) {
            TimeSpan acumuladas = TimeSpan.Zero;
            string comandoDias = "SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha " +
                 "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios._id " +
                 "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula) " +
                 "      AND Grafico > 0 " +
                 "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;";

            var comandoSQL = "SELECT Acumuladas FROM " +
                "    (SELECT * FROM Graficos " +
                "     WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(strftime('%Y-%m-%d', Validez)) FROM Graficos " +
                "                                                                 WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
                "WHERE Numero = @numero";

            //string comandoSQL = "SELECT Acumuladas " +
            //                    "FROM (SELECT * " +
            //                    "      FROM Graficos" +
            //                    "      WHERE IdGrupo = (SELECT _id " +
            //                    "                       FROM GruposGraficos " +
            //                    "                       WHERE Validez = (SELECT Max(Validez) " +
            //                    "                                        FROM GruposGraficos " +
            //                    "                                        WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
            //                    "WHERE Numero = @numero";

            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            try {
                if (CadenaConexion == null) return TimeSpan.Zero;
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    var consulta = new SQLiteExpression(comandoDias).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
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


        public int GetComiteEnDescansoHastaMes(int matricula, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5) AND " +
                                "	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
                                "      strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @fecha);";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", matricula).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetComiteEnDescansoHastaMes), ex);
            }
            return 0;
        }


        public int GetTrabajoEnDescansoHastaMes(int matricula, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                "      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @validez)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", matricula).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetTrabajoEnDescansoHastaMes), ex);
            }
            return 0;
        }


        public int GetDNDHastaMes(int matricula, int año, int mes) {
            string comandoSQL = "SELECT Count(DiasCalendario.Grafico) " +
                                "FROM Calendarios INNER JOIN DiasCalendario " +
                                "ON Calendarios._id = DiasCalendario.IdCalendario " +
                                "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                "      DiasCalendario.Grafico = -8 AND " +
                                "      WHERE strftime('%Y-%m-%d', Validez) < strftime('%Y-%m-%d', @validez)";
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", matricula).AddParameter("@fecha", fecha);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDNDHastaMes), ex);
            }
            return 0;
        }


        public int GetDNDDisfrutadosAño(int matricula, int año) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND MatriculaConductor = @matricula)" +
                                "      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, 1, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
            try {
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDNDDisfrutadosAño), ex);
            }
            return 0;
        }


        public int GetDNDDisfrutadosAño(int matricula, int año, int mes) {
            string comandoSQL = "SELECT Count(Grafico) FROM DiasCalendario " +
                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios " +
                                      "WHERE strftime('%Y', Fecha) = strftime('%Y', @fecha) AND strftime('%m', Fecha) <= strftime('%m', Fecha) AND MatriculaConductor = @matricula)" +
                                      "      AND Grafico = -8 AND (Codigo = 0 OR Codigo Is Null);";
            DateTime fecha = new DateTime(año, mes, 1);
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@fecha", fecha).AddParameter("@matricula", matricula);
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

        public DiaCalendarioBase GetDiaCalendario(int matricula, DateTime fecha) {

            string comandoSQL = "SELECT * FROM DiasCalendario WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE MatriculaConductor = @matricula) AND strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha);";
            var consulta = new SQLiteExpression(comandoSQL).AddParameter("@matricula", matricula).AddParameter("@fecha", fecha);
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

        [Obsolete("Ya no hay grupos de gráficos.")]
        public IEnumerable<Grafico> GetGraficos(int idGrupo) {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM Graficos WHERE IdGrupo = @idGrupo ORDER BY Numero");
                consulta.AddParameter("@idGrupo", idGrupo);
                return GetItems<Grafico>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficos), ex);
            }
            return new List<Grafico>();
        }


        public IEnumerable<Grafico> GetGraficos(DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT * FROM Graficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez) AND Numero <> 0 ORDER BY Numero");
                consulta.AddParameter("@validez", fecha);
                return GetItems<Grafico>(consulta);
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


        public void BorrarGraficosPorFecha(DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("DELETE FROM Graficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez)");
                consulta.AddParameter("@validez", fecha);
                ExecureNonQuery(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGraficos), ex);
            }
        }


        public IEnumerable<Grafico> GetGraficosGrupoPorFecha(DateTime fecha) {
            try {
                //var consulta = new SQLiteExpression("SELECT * FROM Graficos WHERE IdGrupo = (SELECT _id FROM GruposGraficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez))");
                var consulta = new SQLiteExpression("SELECT * FROM Graficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez)");
                consulta.AddParameter("@validez", fecha);
                return GetItems<Grafico>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficosGrupoPorFecha), ex);
            }
            return new List<Grafico>();
        }


        [Obsolete("Ya no hay grupos de gráficos.")]
        public IEnumerable<EstadisticasGraficos> GetEstadisticasGrupoGraficos2(long IdGrupo, TimeSpan jornadaMedia) {
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
                var xxx = GetItems<EstadisticasGraficos>(consulta);
                return GetItems<EstadisticasGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGrupoGraficos), ex);
            }
            return new List<EstadisticasGraficos>();
        }


        public IEnumerable<EstadisticasGraficos> GetEstadisticasGrupoGraficos(DateTime fecha, TimeSpan jornadaMedia) {
            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "Validez AS xValidez, " +
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
                    "FROM Graficos " +
                    "WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez) AND Numero > 0 GROUP BY xValidez, xTurno ORDER BY xValidez, xTurno;");
                consulta.AddParameter("@jornadaMedia", jornadaMedia);
                consulta.AddParameter("@validez", fecha);
                return GetItems<EstadisticasGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGrupoGraficos), ex);
            }
            return new List<EstadisticasGraficos>();
        }


        //[Obsolete("Ya no hay grupos de gráficos.")]
        //public IEnumerable<EstadisticasGraficos> GetEstadisticasGraficosDesdeFecha(DateTime fecha, TimeSpan jornadaMedia) {
        //    try {
        //        var consulta = new SQLiteExpression("SELECT " +
        //            "GruposGraficos.Validez AS xValidez, " +
        //            "Turno AS xTurno, " +
        //            "Count(Numero) AS xNumero, " +
        //            "Sum(Valoracion) AS xValoracion, " +
        //            "Sum(CASE WHEN Trabajadas<@jornadaMedia AND NOT NoCalcular THEN @jornadaMedia ELSE Trabajadas END) AS xTrabajadas, " +
        //            "Sum(Acumuladas) AS xAcumuladas, " +
        //            "Sum(Nocturnas) AS xNocturnas, " +
        //            "Sum(Desayuno) AS xDesayuno, " +
        //            "Sum(Comida) AS xComida, " +
        //            "Sum(Cena) AS xCena, " +
        //            "Sum(PlusCena) AS xPlusCena, " +
        //            "Sum(PlusLimpieza) AS xLimpieza, " +
        //            "Sum(PlusPaqueteria) AS xPaqueteria " +
        //            "FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos._id = Graficos.IdGrupo " +
        //            "WHERE strftime('%Y-%m-%d', GruposGraficos.Validez) >= strftime('%Y-%m-%d', @fecha) " +
        //            "GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;");
        //        consulta.AddParameter("@jornadaMedia", jornadaMedia);
        //        consulta.AddParameter("@fecha", fecha);
        //        return GetItems<EstadisticasGraficos>(consulta);
        //    } catch (Exception ex) {
        //        Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
        //    }
        //    return new List<EstadisticasGraficos>();
        //}


        public IEnumerable<EstadisticasGraficos> GetEstadisticasGraficosDesdeFecha(DateTime fecha, TimeSpan jornadaMedia) {
            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "Validez AS xValidez, " +
                    "Turno AS xTurno, " +
                    "Count(Numero) AS xNumero, " +
                    "Sum(Valoracion) AS xValoracion, " +
                    "Sum(CASE WHEN Trabajadas<@jornadaMedia AND NOT NoCalcular THEN @jornadaMedia ELSE Trabajadas END) AS xTrabajadas, " +
                    "Sum(Acumuladas) AS xAcumuladas, " +
                    "Sum(Nocturnas) AS xNocturnas, " +
                    "Sum(Desayuno) AS xDesayuno, " +
                    "Sum(Comida) AS xComida, " +
                    "Sum(Cena) AS xCena, " +
                    "Sum(PlusCena) AS xPlusCena, " +
                    "Sum(PlusLimpieza) AS xLimpieza, " +
                    "Sum(PlusPaqueteria) AS xPaqueteria " +
                    "FROM Graficos " +
                    "WHERE strftime('%Y-%m-%d', Validez) >= strftime('%Y-%m-%d', @fecha) AND Numero > 0 " +
                    "GROUP BY Validez, Turno ORDER BY Validez, Turno;");
                consulta.AddParameter("@jornadaMedia", jornadaMedia);
                consulta.AddParameter("@fecha", fecha);
                return GetItems<EstadisticasGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
            }
            return new List<EstadisticasGraficos>();
        }


        //[Obsolete("Ya no hay grupos de gráficos.")]
        //public GraficoBase GetGrafico(int numero, DateTime fecha) {
        //    try {
        //        var consulta = new SQLiteExpression("SELECT * " +
        //                            "FROM (SELECT * " +
        //                            "      FROM Graficos" +
        //                            "      WHERE IdGrupo = (SELECT _id " +
        //                            "                       FROM GruposGraficos " +
        //                            "                       WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) " +
        //                            "                                        FROM GruposGraficos " +
        //                            "                                        WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez)))))" +
        //                            "WHERE Numero = @numero");
        //        consulta.AddParameter("@validez", fecha);
        //        consulta.AddParameter("@numero", numero);
        //        return GetItem<GraficoBase>(consulta);
        //    } catch (Exception ex) {
        //        Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
        //    }
        //    return new GraficoBase();
        //}


        public GraficoBase GetGrafico(int numero, DateTime fecha) {
            try {
                var consulta = new SQLiteExpression("SELECT * " +
                                    "FROM (SELECT * " +
                                    "      FROM Graficos" +
                                    "      WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) " +
                                    "                                                                  FROM Graficos" +
                                    "                                                                  WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
                                    "WHERE Numero = @numero");
                consulta.AddParameter("@validez", fecha);
                consulta.AddParameter("@numero", numero);
                return GetItem<GraficoBase>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGraficosDesdeFecha), ex);
            }
            return new GraficoBase();
        }


        public IEnumerable<GraficoBase> GetGraficosVariasFechas(IEnumerable<GrupoGraficos> grupos) {
            try {
                var subconsulta = string.Empty;
                foreach (var grupo in grupos) {
                    subconsulta += $"strftime('%Y-%m-%d', '{grupo.Validez:yyyy-MM-dd}'), ";
                }
                subconsulta = subconsulta.Substring(0, subconsulta.Count() - 2);
                var consulta = new SQLiteExpression($"SELECT * FROM Graficos WHERE strftime('%Y-%m-%d', Validez) IN ({subconsulta}) AND Numero <> 0 ORDER BY Numero");
                return GetItems<GraficoBase>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficos), ex);
            }
            return new List<GraficoBase>();

        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD GRUPOS GRÁFICOS
        // ====================================================================================================

        public IEnumerable<GrupoGraficos> GetGrupos() {
            try {
                var consulta = new SQLiteExpression("SELECT DISTINCT Validez FROM Graficos ORDER BY Validez DESC");
                return GetItems<GrupoGraficos>(consulta);
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


        [Obsolete("Ya no hay grupos.")]
        public void BorrarGrupoPorId(int idGrupo) {
            try {
                var consulta = new SQLiteExpression("DELETE FROM GruposGraficos WHERE _id=@id");
                consulta.AddParameter("@id", idGrupo);
                ExecureNonQuery(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGrupoPorId), ex);
            }
        }


        public void BorrarGrupoPorFecha(DateTime fecha) {
            try {
                BorrarGraficosPorFecha(fecha);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.BorrarGrupoPorId), ex);
            }
        }


        [Obsolete("Ya no hay grupos.")]
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


        public int NuevoGrupo(DateTime fecha) {
            try {
                var graficoCero = new Grafico();
                graficoCero.Validez = fecha;
                graficoCero.Numero = 0;
                return GuardarItem(graficoCero);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.NuevoGrupo), ex);
            }
            return -1;
        }


        public bool ExisteGrupo(DateTime fecha) {
            try {
                //var whereCondition = new SQLiteExpression("strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez)").AddParameter("@validez", fecha);
                var consulta = new SQLiteExpression("SELECT Count(*) FROM Graficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', @validez)");
                consulta.AddParameter("@validez", fecha);
                return GetIntScalar(consulta) > 0;
            } catch (Exception ex) {
                Utils.VerError(nameof(this.ExisteConductor), ex);
            }
            return false;
        }


        public GrupoGraficos GetUltimoGrupo() {
            try {
                //var consulta = new SQLiteExpression("SELECT * FROM GruposGraficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) FROM GruposGraficos));");
                var consulta = new SQLiteExpression("SELECT Max(Validez) AS Validez FROM Graficos;");
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
        #region BD ESTADÍSTICAS
        // ====================================================================================================

        public EstadisticaGrupoGraficos GetEstadisticasUltimoGrupoGraficos(Centros centro) {

            try {
                var consulta = new SQLiteExpression("SELECT  " +
                    "G.Validez, " +
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
                    "FROM Graficos G " +
                    "WHERE G.Numero > 0 AND strftime('%Y-%m-%d', G.Validez) = strftime('%Y-%m-%d', (SELECT Max(Validez) FROM Graficos))" +
                    "GROUP BY G.Validez " +
                    "ORDER BY G.Validez");
                return GetItem<EstadisticaGrupoGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasUltimoGrupoGraficos), ex);
            }
            return new EstadisticaGrupoGraficos();
        }


        public EstadisticaGrupoGraficos GetEstadisticasGrupoGraficos(DateTime fecha, Centros centro) {

            try {
                var consulta = new SQLiteExpression("SELECT " +
                    "G.Validez, " +
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
                    "FROM Graficos G " +
                    "WHERE G.Numero > 0 AND strftime('%Y-%m-%d', G.Validez) = strftime('%Y-%m-%d', @validez) " +
                    "GROUP BY G.Validez " +
                    "ORDER BY G.Validez");
                consulta.AddParameter("@validez", fecha);
                return GetItem<EstadisticaGrupoGraficos>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetEstadisticasGrupoGraficos), ex);
            }
            return new EstadisticaGrupoGraficos();
        }


        public IEnumerable<GraficoFecha> GetGraficosFromDiaCalendario(DateTime fecha, int comodin) {
            var lista = new List<GraficoFecha>();
            try {
                var consultaSQL = "" +
                    "WITH " +
                    "maxValidez AS (SELECT Max(Validez) FROM Graficos WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @fecha)), " +
                    "graficos1 AS (SELECT Grafico FROM DiasCalendario WHERE strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha) AND Grafico > 0) " +
                    "" +
                    "SELECT @fecha as Fecha, * FROM (SELECT * FROM Graficos WHERE (Validez in maxValidez))" +
                    "WHERE Numero > 0 AND (Numero IN graficos1) " +
                    "ORDER BY Numero; ";


                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    var consulta = new SQLiteExpression(consultaSQL);
                    consulta.AddParameter("@fecha", fechaDia);
                    var items = GetItems<GraficoFecha>(consulta);
                    lista.AddRange(items);
                }

                consultaSQL = "" +
                    "WITH " +
                    "maxValidez AS (SELECT Max(Validez) FROM Graficos WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @fecha)), " +
                    "graficos1 AS (SELECT Numero FROM Graficos WHERE (Validez in maxValidez) AND Numero > 0) " +
                    "" +
                    "SELECT * FROM DiasCalendario " +
                    "WHERE strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha) AND Grafico NOT IN graficos1 AND Grafico > 0 " +
                    "ORDER BY Grafico; ";

                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    DateTime fechaDia = new DateTime(fecha.Year, fecha.Month, dia);
                    var consulta = new SQLiteExpression(consultaSQL);
                    consulta.AddParameter("@fecha", fechaDia);
                    var items = GetItems<DiaCalendarioBase>(consulta);
                    foreach (var item in items) {
                        var grafico = new GraficoFecha();
                        grafico.Fecha = fechaDia;
                        grafico.Numero = item.Grafico;
                        grafico.Trabajadas = item.TrabajadasAlt ?? App.Global.Convenio.JornadaMedia;
                        grafico.Acumuladas = item.AcumuladasAlt ?? TimeSpan.Zero;
                        grafico.Nocturnas = item.NocturnasAlt ?? TimeSpan.Zero;
                        grafico.Turno = item.TurnoAlt ?? 1;
                        grafico.Desayuno = item.DesayunoAlt ?? 0;
                        grafico.Comida = item.ComidaAlt ?? 0;
                        grafico.Cena = item.CenaAlt ?? 0;
                        grafico.PlusCena = item.PlusCenaAlt ?? 0;
                        lista.Add(grafico);
                    }
                }

            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetGraficosFromDiaCalendario), ex);
            }
            return lista;
        }


        public IEnumerable<GraficosPorDia> GetGraficosByDia(DateTime fecha) {
            if (CadenaConexion == null) return null;
            try {
                var consultaSQL = "" +
                    "WITH " +
                    "maxValidez AS(SELECT Max(Validez) FROM Graficos WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @fecha)) " +
                    "" +
                    "SELECT Numero " +
                    "FROM Graficos " +
                    "WHERE Numero > 0 AND Validez in maxValidez AND (DiaSemana = @diaSemana OR DiaSemana = 'R' OR DiaSemana = '' OR DiaSemana IS NULL) " +
                    "ORDER BY Numero; ";

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
                    var consulta = new SQLiteExpression(consultaSQL);
                    consulta.AddParameter("@fecha", fechaDia);
                    consulta.AddParameter("@diaSemana", diasemana);
                    var Gpd = new GraficosPorDia();
                    Gpd.Fecha = fechaDia;
                    using (var conexion = new SQLiteConnection(CadenaConexion)) {
                        conexion.Open();
                        using (var comando = consulta.GetCommand(conexion)) {
                            using (var lector = comando.ExecuteReader()) {
                                while (lector.Read()) {
                                    Gpd.Lista.Add(lector.ToInt32("Numero"));
                                }
                            }
                        }
                    }
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
                List<DescansosPorDia> lista = new List<DescansosPorDia>();

                for (int dia = 1; dia <= DateTime.DaysInMonth(fecha.Year, fecha.Month); dia++) {
                    var consulta = new SQLiteExpression("SELECT Count(*) " +
                                        "FROM DiasCalendario " +
                                        "WHERE strftime('%Y-%m-%d', DiaFecha) = strftime('%Y-%m-%d', @fecha) AND (Grafico = -2 OR Grafico = -3)");

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


        public List<DiaCalendarioEstadistica> GetDiasCalendarioEstadisticaMes(int año, int mes, int matricula) {
            var lista = new List<DiaCalendarioEstadistica>();
            var fecha = new DateTime(año, mes, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    G.Turno AS Turno, " +
                    "    G.Trabajadas AS Trabajadas, " +
                    "    G.Acumuladas AS Acumuladas, " +
                    "    G.Nocturnas AS Nocturnas, " +
                    "    G.Desayuno AS Desayuno, " +
                    "    G.Comida AS Comida, " +
                    "    G.Cena AS Cena, " +
                    "    G.PlusCena AS PlusCena, " +
                    "    G.PlusLimpieza AS PlusLimpieza, " +
                    "    G.PlusPaqueteria AS PlusPaqueteria, " +
                    "    DC.TurnoAlt AS TurnoAlt, " +
                    "    DC.TrabajadasAlt AS TrabajadasAlt, " +
                    "    DC.AcumuladasAlt AS AcumuladasAlt, " +
                    "    DC.NocturnasAlt AS NocturnasAlt, " +
                    "    DC.DesayunoAlt AS DesayunoAlt, " +
                    "    DC.ComidaAlt AS ComidaAlt, " +
                    "    DC.CenaAlt AS CenaAlt, " +
                    "    DC.PlusCenaAlt AS PlusCenaAlt, " +
                    "    DC.PlusLimpiezaAlt AS PlusLimpiezaAlt, " +
                    "    DC.PlusPaqueteriaAlt AS PlusPaqueteriaAlt " +
                    "FROM DiasCalendario As DC " +
                    "    LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "    LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE C.MatriculaConductor = @conductor AND strftime('%Y-%m', C.Fecha) = strftime('%Y-%m', @fecha) " +
                    "ORDER BY DC.DiaFecha;");
                consulta.AddParameter("@conductor", matricula);
                consulta.AddParameter("@fecha", fecha);
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaCalendarioEstadistica(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasCalendarioEstadisticaMes), ex);
            }
            return lista;
        }


        public List<DiaCalendarioEstadistica> GetDiasCalendarioEstadisticaAño(int año, int matricula) {
            var lista = new List<DiaCalendarioEstadistica>();
            var fecha = new DateTime(año, 1, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    G.Turno AS Turno, " +
                    "    G.Trabajadas AS Trabajadas, " +
                    "    G.Acumuladas AS Acumuladas, " +
                    "    G.Nocturnas AS Nocturnas, " +
                    "    G.Desayuno AS Desayuno, " +
                    "    G.Comida AS Comida, " +
                    "    G.Cena AS Cena, " +
                    "    G.PlusCena AS PlusCena, " +
                    "    G.PlusLimpieza AS PlusLimpieza, " +
                    "    G.PlusPaqueteria AS PlusPaqueteria, " +
                    "    DC.TurnoAlt AS TurnoAlt, " +
                    "    DC.TrabajadasAlt AS TrabajadasAlt, " +
                    "    DC.AcumuladasAlt AS AcumuladasAlt, " +
                    "    DC.NocturnasAlt AS NocturnasAlt, " +
                    "    DC.DesayunoAlt AS DesayunoAlt, " +
                    "    DC.ComidaAlt AS ComidaAlt, " +
                    "    DC.CenaAlt AS CenaAlt, " +
                    "    DC.PlusCenaAlt AS PlusCenaAlt, " +
                    "    DC.PlusLimpiezaAlt AS PlusLimpiezaAlt, " +
                    "    DC.PlusPaqueteriaAlt AS PlusPaqueteriaAlt " +
                    "FROM DiasCalendario As DC " +
                    "    LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "    LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE C.MatriculaConductor = @conductor AND strftime('%Y', C.Fecha) = strftime('%Y', @fecha) " +
                    "ORDER BY DC.DiaFecha;");
                consulta.AddParameter("@conductor", matricula);
                consulta.AddParameter("@fecha", fecha);
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaCalendarioEstadistica(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasCalendarioEstadisticaMes), ex);
            }
            return lista;
        }









        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD PIJAMAS
        // ====================================================================================================

        public IEnumerable<Pijama.DiaPijama> GetDiasPijama(IEnumerable<DiaCalendarioBase> listadias, int comodin) {
            if (CadenaConexion == null) return null;
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

                        var consulta = new SQLiteExpression(
                            "SELECT * FROM " +
                            "    (SELECT * FROM Graficos " +
                            "     WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(strftime('%Y-%m-%d', Validez)) FROM Graficos " +
                            "                                                                 WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
                            "WHERE Numero = @numero");

                        //var consulta = new SQLiteExpression("" +
                        //    "SELECT * FROM Graficos " +
                        //    "WHERE Numero = @numero AND Validez = (SELECT Max(Validez) FROM Graficos WHERE Numero = @numero AND Validez < @validez);");


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


        public IEnumerable<DiaPijama> GetDiasPijama(DateTime fecha, int matriculaConductor) {
            var consulta = new SQLiteExpression("" +
                "SELECT " +
                "DiasCalendario.*, " +
                "Graficos.* " +
                "FROM DiasCalendario LEFT JOIN Graficos " +
                "ON DiasCalendario.Grafico = Graficos.Numero " + //Si falla aquí probar a añadir DiasCalendario.Grafico > 0 AND ...
                "AND strftime('%Y-%m-%d', Graficos.Validez) = strftime('%Y-%m-%d', (SELECT Max(Graficos.Validez) FROM Graficos WHERE strftime('%Y-%m-%d', Graficos.Validez) <= strftime('%Y-%m-%d', DiasCalendario.DiaFecha))) " +
                "" +
                "WHERE IdCalendario = (SELECT _id " +
                "                      FROM Calendarios " +
                "                      WHERE Calendarios.MatriculaConductor = @matricula AND strftime('%Y-%m', Calendarios.Fecha) = strftime('%Y-%m', @fecha)) " +
                "ORDER BY DiasCalendario.Dia");
            consulta.AddParameter("@matricula", matriculaConductor);
            consulta.AddParameter("@fecha", fecha);
            try {
                return GetItems<DiaPijama>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetResumenHastaMes), ex);
            }
            return new List<Pijama.DiaPijama>();
        }



        public ResumenPijama GetResumenHastaMes(int año, int mes, int matricula, int comodin) {
            if (CadenaConexion == null) return null;
            // Inicializamos el resultado.
            ResumenPijama resultado = new ResumenPijama();
            // Establecemos la fecha del día 1 del siguiente mes al indicado.
            DateTime fecha = new DateTime(año, mes, 1).AddMonths(1);
            // Establecemos el id del conductor
            int idConductor = App.Global.ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == matricula)?.Id ?? 0;
            try {
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    conexion.Open();
                    //----------------------------------------------------------------------------------------------------
                    // HORAS ACUMULADAS
                    //----------------------------------------------------------------------------------------------------
                    var consulta = new SQLiteExpression("SELECT DiasCalendario.Dia, DiasCalendario.Grafico, DiasCalendario.GraficoVinculado, Calendarios.Fecha, " +
                                   "DiasCalendario.ExcesoJornada, DiasCalendario.AcumuladasAlt " +
                                   "FROM DiasCalendario LEFT JOIN Calendarios ON DiasCalendario.IdCalendario = Calendarios._id " +
                                   "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE Fecha < @fecha AND MatriculaConductor = @matricula) " +
                                   "      AND Grafico > 0 " +
                                   "ORDER BY Calendarios.Fecha, DiasCalendario.Dia;");
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@matricula", matricula);
                    using (var lector = consulta.GetCommand(conexion).ExecuteReader()) {
                        // Por cada día, sumamos las horas acumuladas.
                        while (lector.Read()) {
                            int dia = lector.ToInt32("Dia");
                            int grafico = lector.ToInt32("Grafico");
                            int graficoVinculado = lector.ToInt32("GraficoVinculado");
                            TimeSpan? acumuladasAlt = lector.ToTimeSpanNulable("AcumuladasAlt");
                            TimeSpan excesoJornada = lector.ToTimeSpan("ExcesoJornada");
                            if (graficoVinculado != 0 && grafico == comodin) grafico = graficoVinculado;
                            DateTime fechaCalendario = lector.ToDateTime("Fecha");
                            if (dia > DateTime.DaysInMonth(fechaCalendario.Year, fechaCalendario.Month)) continue;
                            DateTime fechadia = new DateTime(fechaCalendario.Year, fechaCalendario.Month, dia);
                            var consulta2 = new SQLiteExpression("SELECT * " +
                                                  "FROM (SELECT * FROM Graficos WHERE strftime('%Y-%m-%d', Validez) = strftime('%Y-%m-%d', (SELECT Max(strftime('%Y-%m-%d', Validez)) " +
                                                  "										         FROM Graficos " +
                                                  "											     WHERE strftime('%Y-%m-%d', Validez) <= strftime('%Y-%m-%d', @validez))))" +
                                                  "WHERE Numero = @numero");
                            consulta2.AddParameter("validez", fechadia);
                            consulta2.AddParameter("numero", grafico);
                            var graficoTrabajado = GetItem<GraficoBase>(conexion, consulta2, true);
                            if (graficoTrabajado == null) {
                                graficoTrabajado = new GraficoBase();
                            } else {
                                graficoTrabajado.Final += excesoJornada;
                            }
                            if (acumuladasAlt.HasValue) graficoTrabajado.Acumuladas = acumuladasAlt.Value;
                            resultado.HorasAcumuladas += graficoTrabajado.Acumuladas;
                        }
                    }

                    //----------------------------------------------------------------------------------------------------
                    // HORAS REGULADAS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Horas) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@idConductor", idConductor);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.HorasReguladas = GetTimeSpanScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DIAS F6
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                              "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula)" +
                                              "      AND Grafico = -7;");
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@matricula", matricula);
                    resultado.DiasLibreDisposicionF6 = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DIAS F6DC
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                              "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula)" +
                                              "      AND Grafico = -14;");
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@matricula", matricula);
                    resultado.DiasF6DC = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DCS REGULADOS 
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Descansos) FROM Regulaciones WHERE idConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@idConductor", idConductor);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.DCsRegulados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DCS DISFRUTADOS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                                      "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha) AND MatriculaConductor = @matricula)" +
                                                      "      AND Grafico = -6;");
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@matricula", matricula);
                    resultado.DCsDisfrutados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DNDs REGULADOS 
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Sum(Dnds) FROM Regulaciones WHERE IdConductor = @idConductor AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@idConductor", idConductor);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.DNDsRegulados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DNDS DISFRUTADOS
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                       "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                       "WHERE Calendarios.MatriculaConductor = @matricula AND DiasCalendario.Grafico = -8 AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.DNDsDisfrutados = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS COMITÉ EN DESCANSO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -5 OR " +
                                                      "       DiasCalendario.Grafico = -6 OR DiasCalendario.Grafico = -1) AND " +
                                                      "	   (DiasCalendario.Codigo = 1 OR DiasCalendario.Codigo = 2) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.DiasComiteEnDescanso = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS TRABAJO EN DESCANSO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                       "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                       "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                       "      DiasCalendario.Grafico > 0 AND DiasCalendario.Codigo = 3 AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    resultado.DiasTrabajoEnDescanso = GetDecimalScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS TRABAJADOS AÑO
                    //----------------------------------------------------------------------------------------------------
                    var fechaInicio = new DateTime(año, 1, 1);
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                       "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                       "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                       "      DiasCalendario.Grafico > 0 AND strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasTrabajados = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS JD AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -2 OR DiasCalendario.Grafico = -10 OR " +
                                                      "       DiasCalendario.Grafico = -12 OR DiasCalendario.Grafico = -17) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: ELiminar si es así el OVA-JD y el OVA-FN (-17 y -18)
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasJD = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS FN AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -3 OR DiasCalendario.Grafico = -11 OR " +
                                                      "       DiasCalendario.Grafico = -13 OR DiasCalendario.Grafico = -18) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: ELiminar si es así el OVA-JD y el OVA-FN (-17 y -18)
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasFN = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS DS AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -5) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: ELiminar si es así el OVA-JD y el OVA-FN (-17 y -18)
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasDS = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS PERMISO AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -9) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: ELiminar si es así el OVA-JD y el OVA-FN (-17 y -18)
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasPermiso = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS F6 AÑO
                    //----------------------------------------------------------------------------------------------------
                    fechaInicio = mes == 12 ? new DateTime(año, 12, 1) : new DateTime(año - 1, 12, 1);
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -7 OR DiasCalendario.Grafico = -14) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: ELiminar si es así el OVA-JD y el OVA-FN (-17 y -18)
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasF6 = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS VACACIONES AÑO
                    //----------------------------------------------------------------------------------------------------
                    fechaInicio = new DateTime(año, 1, 1);
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -1 OR DiasCalendario.Grafico = -12 OR DiasCalendario.Grafico = -13) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasVacaciones = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS ENFERMO AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -4 OR DiasCalendario.Grafico = -10 OR DiasCalendario.Grafico = -11) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    //TODO: Si no se computan los días de descanso, hay que eliminar los codigos -10 y -11.
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasEnfermo = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // DÍAS DC AÑO
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico = -6) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    resultado.DiasDC = GetIntScalar(conexion, consulta);
                    //----------------------------------------------------------------------------------------------------
                    // HORAS ANUALES
                    //----------------------------------------------------------------------------------------------------
                    consulta = new SQLiteExpression("SELECT Count(DiasCalendario.Grafico) " +
                                                      "FROM Calendarios INNER JOIN DiasCalendario ON Calendarios._id = DiasCalendario.IdCalendario " +
                                                      "WHERE Calendarios.MatriculaConductor = @matricula AND " +
                                                      "      (DiasCalendario.Grafico > 0 OR DiasCalendario.Grafico = -4 OR " +
                                                      "       DiasCalendario.Grafico = -6 OR DiasCalendario.Grafico = -7 OR " +
                                                      "       DiasCalendario.Grafico = -8 OR DiasCalendario.Grafico = -9 OR " +
                                                      "       DiasCalendario.Grafico = -14 OR DiasCalendario.Grafico = -15 OR DiasCalendario.Grafico = -16) AND " +
                                                      "      strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fecha)" +
                                                       "       AND strftime('%Y-%m-%d', Fecha) >= strftime('%Y-%m-%d', @fechaInicio)");
                    // Si los días de OVA (código -16) no se computan, eliminarlos.
                    // Si hay que incluir los OVA-JD y OVA-FN (-17 y -18), añadirlos.
                    consulta.AddParameter("@matricula", matricula);
                    consulta.AddParameter("@fecha", fecha);
                    consulta.AddParameter("@fechaInicio", fechaInicio);
                    var dias = GetIntScalar(conexion, consulta);
                    resultado.HorasAnuales = new TimeSpan(dias * App.Global.Convenio.JornadaMedia.Ticks);

                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetResumenHastaMes), ex);
            }
            return resultado;
        }


        //TODO: El método siguiente devuelve todos los dias del año hasta el mes indicado. En el método anterior,
        // se mezcla hasta el mes actual desde el inicio de Orión con hasta el mes actual desde el primer día del año.
        // Revisarlo y modificar lo que haga falta.
        public ResumenDias GetDiasAcumuladoAño(int año, int mes, int matricula) {
            if (CadenaConexion == null) return null;
            // Inicializamos el resultado.
            ResumenDias resultado = null;
            // Establecemos la fecha del día 1 del año y la fecha del día 1 del siguiente mes al indicado.
            DateTime fechaInicio = new DateTime(año, 1, 1);
            DateTime fechaFinal = new DateTime(año, mes, 1).AddMonths(1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "Count(CASE WHEN DC.Grafico > 0 THEN 1 ELSE NULL END) AS DiasTrabajados, " +
                    "Count(CASE WHEN DC.Grafico = -1 THEN 1 ELSE NULL END) AS DiasOV, " +
                    "Count(CASE WHEN DC.Grafico = -2 THEN 1 ELSE NULL END) AS DiasJD, " +
                    "Count(CASE WHEN DC.Grafico = -3 THEN 1 ELSE NULL END) AS DiasFN, " +
                    "Count(CASE WHEN DC.Grafico = -4 THEN 1 ELSE NULL END) AS DiasE, " +
                    "Count(CASE WHEN DC.Grafico = -5 THEN 1 ELSE NULL END) AS DiasDS, " +
                    "Count(CASE WHEN DC.Grafico = -6 THEN 1 ELSE NULL END) AS DiasDC, " +
                    "Count(CASE WHEN DC.Grafico = -7 THEN 1 ELSE NULL END) AS DiasF6, " +
                    "Count(CASE WHEN DC.Grafico = -8 THEN 1 ELSE NULL END) AS DiasDND, " +
                    "Count(CASE WHEN DC.Grafico = -9 THEN 1 ELSE NULL END) AS DiasPER, " +
                    "Count(CASE WHEN DC.Grafico = -10 THEN 1 ELSE NULL END) AS DiasEJD, " +
                    "Count(CASE WHEN DC.Grafico = -11 THEN 1 ELSE NULL END) AS DiasEFN, " +
                    "Count(CASE WHEN DC.Grafico = -12 THEN 1 ELSE NULL END) AS DiasOVJD, " +
                    "Count(CASE WHEN DC.Grafico = -13 THEN 1 ELSE NULL END) AS DiasOVFN, " +
                    "Count(CASE WHEN DC.Grafico = -14 THEN 1 ELSE NULL END) AS DiasF6DC, " +
                    "Count(CASE WHEN DC.Grafico = -15 THEN 1 ELSE NULL END) AS DiasFOR, " +
                    "Count(CASE WHEN DC.Grafico = -16 THEN 1 ELSE NULL END) AS DiasOVA, " +
                    "Count(CASE WHEN DC.Grafico = -17 THEN 1 ELSE NULL END) AS DiasOVAJD, " +
                    "Count(CASE WHEN DC.Grafico = -18 THEN 1 ELSE NULL END) AS DiasOVAFN, " +
                    "Count(CASE WHEN DC.Codigo = 1 THEN 1 ELSE NULL END) AS DiasCO, " +
                    "Count(CASE WHEN DC.Codigo = 2 THEN 1 ELSE NULL END) AS DiasCE, " +
                    "Count(CASE WHEN DC.Codigo = 3 THEN 1 ELSE NULL END) AS DiasJDTrabajados " +
                    "FROM " +
                    "Calendarios AS C INNER JOIN DiasCalendario AS DC ON C._id = DC.IdCalendario " +
                    "WHERE " +
                    "C.MatriculaConductor = @matricula AND " +
                    "strftime('%Y-%m-%d', C.Fecha) >= strftime('%Y-%m-%d', @fechaInicio) AND " +
                    "strftime('%Y-%m-%d', C.Fecha) < strftime('%Y-%m-%d', @fechaFinal)");
                consulta.AddParameter("@matricula", matricula);
                consulta.AddParameter("@fechaInicio", fechaInicio);
                consulta.AddParameter("@fechaFinal", fechaFinal);
                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            resultado = new ResumenDias(lector);
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasAcumuladoAño), ex);
            }
            return resultado;
        }


        public int GetDiasTrabajadosHastaMesEnAño(int año, int mes, int matricula) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        MatriculaConductor = @matricula) " +
                                    "      AND Grafico > 0;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@matricula", matricula);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasTrabajadosHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasDescansoHastaMesEnAño(int año, int mes, int matricula) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        MatriculaConductor = @matricula) " +
                                    "      AND (Grafico = -2 OR Grafico = -10 OR Grafico = -12);");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@matricula", matricula);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasDescansoHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasVacacionesHastaMesEnAño(int año, int mes, int matricula) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        MatriculaConductor = @matricula) " +
                                    "      AND Grafico = -1;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@matricula", matricula);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasVacacionesHastaMesEnAño), ex);
            }
            return 0;
        }


        public int GetDiasInactivoHastaMesEnAño(int año, int mes, int matricula) {
            try {
                DateTime fechainicio = new DateTime(año, 1, 1).AddDays(-1);
                DateTime fechafinal = new DateTime(año, mes, DateTime.DaysInMonth(año, mes)).AddDays(1);
                var consulta = new SQLiteExpression("SELECT Count(Grafico) FROM DiasCalendario " +
                                    "WHERE IdCalendario IN (SELECT _id FROM Calendarios WHERE strftime('%Y-%m-%d', Fecha) > strftime('%Y-%m-%d', @fechaInicio) AND " +
                                    "                                                        strftime('%Y-%m-%d', Fecha) < strftime('%Y-%m-%d', @fechaFinal) AND " +
                                    "                                                        MatriculaConductor = @matricula) " +
                                    "      AND Grafico = 0;");
                consulta.AddParameter("@fechaInicio", fechainicio);
                consulta.AddParameter("@fechaFinal", fechafinal);
                consulta.AddParameter("@matricula", matricula);
                return GetIntScalar(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasInactivoHastaMesEnAño), ex);
            }
            return 0;
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region BD DIAS PIJAMA BÁSICOS
        // ====================================================================================================

        public List<DiaPijamaBasico> GetDiasPijamaBasicoMes(int año, int mes, int matricula) {
            var lista = new List<DiaPijamaBasico>();
            var fecha = new DateTime(año, mes, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    DC.ExcesoJornada AS ExcesoJornada, " +
                    "    DC.FacturadoPaqueteria AS FacturadoPaqueteria, " +
                    "    DC.Limpieza AS Limpieza, " +
                    "    DC.GraficoVinculado AS NumeroGraficoVinculado, " +
                    "    G.Validez AS ValidezGrafico, " +
                    "    G.DiaSemana AS DiaSemanaGrafico, " +
                    "    CASE WHEN DC.TurnoAlt > 0 THEN DC.TurnoAlt ELSE G.Turno END AS Turno, " +
                    "    CASE WHEN DC.InicioAlt > 0 THEN DC.InicioAlt ELSE G.Inicio END AS Inicio, " +
                    "    CASE WHEN DC.FinalAlt > 0 THEN DC.FinalAlt ELSE G.Final END AS Final, " +
                    "    CASE WHEN DC.InicioPartidoAlt > 0 THEN DC.InicioPartidoAlt ELSE G.InicioPartido END AS InicioPartido, " +
                    "    CASE WHEN DC.FinalPartidoAlt > 0 THEN DC.FinalPartidoAlt ELSE G.FinalPartido END AS FinalPartido, " +
                    "    G.Valoracion AS ValoracionGrafico, " +
                    "    CASE WHEN DC.TrabajadasAlt > 0 THEN DC.TrabajadasAlt ELSE G.Trabajadas END AS Trabajadas, " +
                    "    CASE WHEN DC.AcumuladasAlt > 0 THEN DC.AcumuladasAlt ELSE G.Acumuladas END AS Acumuladas, " +
                    "    CASE WHEN DC.NocturnasAlt > 0 THEN DC.NocturnasAlt ELSE G.Nocturnas END AS Nocturnas, " +
                    "    CASE WHEN DC.DesayunoAlt > 0 THEN DC.DesayunoAlt ELSE G.Desayuno END AS Desayuno, " +
                    "    CASE WHEN DC.ComidaAlt > 0 THEN DC.ComidaAlt ELSE G.Comida END AS Comida, " +
                    "    CASE WHEN DC.CenaAlt > 0 THEN DC.CenaAlt ELSE G.Cena END AS Cena, " +
                    "    CASE WHEN DC.PlusCenaAlt > 0 THEN DC.PlusCenaAlt ELSE G.PlusCena END AS PlusCena, " +
                    "    CASE WHEN DC.PlusLimpiezaAlt > 0 THEN DC.PlusLimpiezaAlt ELSE G.PlusLimpieza END AS PlusLimpieza, " +
                    "    CASE WHEN DC.PlusPaqueteriaAlt > 0 THEN DC.PlusPaqueteriaAlt ELSE G.PlusPaqueteria END AS PlusPaqueteria, " +
                    "    DC.Notas AS Notas " +
                    "FROM DiasCalendario As DC " +
                    "LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE C.MatriculaConductor = @conductor AND strftime('%Y-%m', C.Fecha) = strftime('%Y-%m', @fecha) " +
                    "ORDER BY C.MatriculaConductor, DC.DiaFecha; ");

                consulta.AddParameter("@conductor", matricula);
                consulta.AddParameter("@fecha", fecha);

                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaPijamaBasico(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasPijamaBasicoMes), ex);
            }
            return lista;
        }


        public List<DiaPijamaBasico> GetDiasPijamaBasicoAño(int año, int matricula) {
            var lista = new List<DiaPijamaBasico>();
            var fecha = new DateTime(año, 1, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    DC.ExcesoJornada AS ExcesoJornada, " +
                    "    DC.FacturadoPaqueteria AS FacturadoPaqueteria, " +
                    "    DC.Limpieza AS Limpieza, " +
                    "    DC.GraficoVinculado AS NumeroGraficoVinculado, " +
                    "    G.Validez AS ValidezGrafico, " +
                    "    G.DiaSemana AS DiaSemanaGrafico, " +
                    "    CASE WHEN DC.TurnoAlt > 0 THEN DC.TurnoAlt ELSE G.Turno END AS Turno, " +
                    "    CASE WHEN DC.InicioAlt > 0 THEN DC.InicioAlt ELSE G.Inicio END AS Inicio, " +
                    "    CASE WHEN DC.FinalAlt > 0 THEN DC.FinalAlt ELSE G.Final END AS Final, " +
                    "    CASE WHEN DC.InicioPartidoAlt > 0 THEN DC.InicioPartidoAlt ELSE G.InicioPartido END AS InicioPartido, " +
                    "    CASE WHEN DC.FinalPartidoAlt > 0 THEN DC.FinalPartidoAlt ELSE G.FinalPartido END AS FinalPartido, " +
                    "    G.Valoracion AS ValoracionGrafico, " +
                    "    CASE WHEN DC.TrabajadasAlt > 0 THEN DC.TrabajadasAlt ELSE G.Trabajadas END AS Trabajadas, " +
                    "    CASE WHEN DC.AcumuladasAlt > 0 THEN DC.AcumuladasAlt ELSE G.Acumuladas END AS Acumuladas, " +
                    "    CASE WHEN DC.NocturnasAlt > 0 THEN DC.NocturnasAlt ELSE G.Nocturnas END AS Nocturnas, " +
                    "    CASE WHEN DC.DesayunoAlt > 0 THEN DC.DesayunoAlt ELSE G.Desayuno END AS Desayuno, " +
                    "    CASE WHEN DC.ComidaAlt > 0 THEN DC.ComidaAlt ELSE G.Comida END AS Comida, " +
                    "    CASE WHEN DC.CenaAlt > 0 THEN DC.CenaAlt ELSE G.Cena END AS Cena, " +
                    "    CASE WHEN DC.PlusCenaAlt > 0 THEN DC.PlusCenaAlt ELSE G.PlusCena END AS PlusCena, " +
                    "    CASE WHEN DC.PlusLimpiezaAlt > 0 THEN DC.PlusLimpiezaAlt ELSE G.PlusLimpieza END AS PlusLimpieza, " +
                    "    CASE WHEN DC.PlusPaqueteriaAlt > 0 THEN DC.PlusPaqueteriaAlt ELSE G.PlusPaqueteria END AS PlusPaqueteria, " +
                    "    DC.Notas AS Notas " +
                    "FROM DiasCalendario As DC " +
                    "LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE C.MatriculaConductor = @conductor AND strftime('%Y', C.Fecha) = strftime('%Y', @fecha) " +
                    "ORDER BY C.MatriculaConductor, DC.DiaFecha; ");

                consulta.AddParameter("@conductor", matricula);
                consulta.AddParameter("@fecha", fecha);

                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaPijamaBasico(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasPijamaBasicoMes), ex);
            }
            return lista;
        }


        public List<DiaPijamaBasico> GetTodosDiasPijamaBasicoMes(int año, int mes) {
            var lista = new List<DiaPijamaBasico>();
            var fecha = new DateTime(año, mes, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    DC.ExcesoJornada AS ExcesoJornada, " +
                    "    DC.FacturadoPaqueteria AS FacturadoPaqueteria, " +
                    "    DC.Limpieza AS Limpieza, " +
                    "    DC.GraficoVinculado AS NumeroGraficoVinculado, " +
                    "    G.Validez AS ValidezGrafico, " +
                    "    G.DiaSemana AS DiaSemanaGrafico, " +
                    "    CASE WHEN DC.TurnoAlt > 0 THEN DC.TurnoAlt ELSE G.Turno END AS Turno, " +
                    "    CASE WHEN DC.InicioAlt > 0 THEN DC.InicioAlt ELSE G.Inicio END AS Inicio, " +
                    "    CASE WHEN DC.FinalAlt > 0 THEN DC.FinalAlt ELSE G.Final END AS Final, " +
                    "    CASE WHEN DC.InicioPartidoAlt > 0 THEN DC.InicioPartidoAlt ELSE G.InicioPartido END AS InicioPartido, " +
                    "    CASE WHEN DC.FinalPartidoAlt > 0 THEN DC.FinalPartidoAlt ELSE G.FinalPartido END AS FinalPartido, " +
                    "    G.Valoracion AS ValoracionGrafico, " +
                    "    CASE WHEN DC.TrabajadasAlt > 0 THEN DC.TrabajadasAlt ELSE G.Trabajadas END AS Trabajadas, " +
                    "    CASE WHEN DC.AcumuladasAlt > 0 THEN DC.AcumuladasAlt ELSE G.Acumuladas END AS Acumuladas, " +
                    "    CASE WHEN DC.NocturnasAlt > 0 THEN DC.NocturnasAlt ELSE G.Nocturnas END AS Nocturnas, " +
                    "    CASE WHEN DC.DesayunoAlt > 0 THEN DC.DesayunoAlt ELSE G.Desayuno END AS Desayuno, " +
                    "    CASE WHEN DC.ComidaAlt > 0 THEN DC.ComidaAlt ELSE G.Comida END AS Comida, " +
                    "    CASE WHEN DC.CenaAlt > 0 THEN DC.CenaAlt ELSE G.Cena END AS Cena, " +
                    "    CASE WHEN DC.PlusCenaAlt > 0 THEN DC.PlusCenaAlt ELSE G.PlusCena END AS PlusCena, " +
                    "    CASE WHEN DC.PlusLimpiezaAlt > 0 THEN DC.PlusLimpiezaAlt ELSE G.PlusLimpieza END AS PlusLimpieza, " +
                    "    CASE WHEN DC.PlusPaqueteriaAlt > 0 THEN DC.PlusPaqueteriaAlt ELSE G.PlusPaqueteria END AS PlusPaqueteria, " +
                    "    DC.Notas AS Notas " +
                    "FROM DiasCalendario As DC " +
                    "LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE strftime('%Y-%m', C.Fecha) = strftime('%Y-%m', @fecha) " +
                    "ORDER BY C.MatriculaConductor, DC.DiaFecha; ");

                consulta.AddParameter("@fecha", fecha);

                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaPijamaBasico(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasPijamaBasicoMes), ex);
            }
            return lista;
        }


        public List<DiaPijamaBasico> GetTodosDiasPijamaBasicoAño(int año) {
            var lista = new List<DiaPijamaBasico>();
            var fecha = new DateTime(año, 1, 1);
            try {
                var consulta = new SQLiteExpression("" +
                    "SELECT " +
                    "    C.MatriculaConductor AS MatriculaConductor, " +
                    "    DC.DiaFecha AS Fecha, " +
                    "    DC.Grafico AS NumeroGrafico, " +
                    "    DC.Codigo AS Codigo, " +
                    "    DC.ExcesoJornada AS ExcesoJornada, " +
                    "    DC.FacturadoPaqueteria AS FacturadoPaqueteria, " +
                    "    DC.Limpieza AS Limpieza, " +
                    "    DC.GraficoVinculado AS NumeroGraficoVinculado, " +
                    "    G.Validez AS ValidezGrafico, " +
                    "    G.DiaSemana AS DiaSemanaGrafico, " +
                    "    CASE WHEN DC.TurnoAlt > 0 THEN DC.TurnoAlt ELSE G.Turno END AS Turno, " +
                    "    CASE WHEN DC.InicioAlt > 0 THEN DC.InicioAlt ELSE G.Inicio END AS Inicio, " +
                    "    CASE WHEN DC.FinalAlt > 0 THEN DC.FinalAlt ELSE G.Final END AS Final, " +
                    "    CASE WHEN DC.InicioPartidoAlt > 0 THEN DC.InicioPartidoAlt ELSE G.InicioPartido END AS InicioPartido, " +
                    "    CASE WHEN DC.FinalPartidoAlt > 0 THEN DC.FinalPartidoAlt ELSE G.FinalPartido END AS FinalPartido, " +
                    "    G.Valoracion AS ValoracionGrafico, " +
                    "    CASE WHEN DC.TrabajadasAlt > 0 THEN DC.TrabajadasAlt ELSE G.Trabajadas END AS Trabajadas, " +
                    "    CASE WHEN DC.AcumuladasAlt > 0 THEN DC.AcumuladasAlt ELSE G.Acumuladas END AS Acumuladas, " +
                    "    CASE WHEN DC.NocturnasAlt > 0 THEN DC.NocturnasAlt ELSE G.Nocturnas END AS Nocturnas, " +
                    "    CASE WHEN DC.DesayunoAlt > 0 THEN DC.DesayunoAlt ELSE G.Desayuno END AS Desayuno, " +
                    "    CASE WHEN DC.ComidaAlt > 0 THEN DC.ComidaAlt ELSE G.Comida END AS Comida, " +
                    "    CASE WHEN DC.CenaAlt > 0 THEN DC.CenaAlt ELSE G.Cena END AS Cena, " +
                    "    CASE WHEN DC.PlusCenaAlt > 0 THEN DC.PlusCenaAlt ELSE G.PlusCena END AS PlusCena, " +
                    "    CASE WHEN DC.PlusLimpiezaAlt > 0 THEN DC.PlusLimpiezaAlt ELSE G.PlusLimpieza END AS PlusLimpieza, " +
                    "    CASE WHEN DC.PlusPaqueteriaAlt > 0 THEN DC.PlusPaqueteriaAlt ELSE G.PlusPaqueteria END AS PlusPaqueteria, " +
                    "    DC.Notas AS Notas " +
                    "FROM DiasCalendario As DC " +
                    "LEFT JOIN Calendarios AS C ON C._id = DC.IdCalendario " +
                    "LEFT JOIN Graficos AS G ON G.Validez = (SELECT Max(Validez) FROM Graficos WHERE DC.Grafico = G.Numero AND Validez <= DC.DiaFecha) " +
                    "WHERE strftime('%Y', C.Fecha) = strftime('%Y', @fecha) " +
                    "ORDER BY C.MatriculaConductor, DC.DiaFecha; ");

                consulta.AddParameter("@fecha", fecha);

                using (var conexion = new SQLiteConnection(CadenaConexion)) {
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            while (lector.Read()) {
                                var dia = new DiaPijamaBasico(lector);
                                lista.Add(dia);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetDiasPijamaBasicoMes), ex);
            }
            return lista;
        }





        #endregion
        // ====================================================================================================







        /// <summary>
        /// Prueba de cómo se pueden acceder a dos bases de datos diferentes a la vez.
        /// </summary>
        public IEnumerable<Conductor> GetPrueba(string path) {
            if (CadenaConexion == null) return null;
            try {
                //string comandoSQL = $"ATTACH '{path}' AS Arr; SELECT * FROM Conductores UNION ALL SELECT * FROM Arr.Conductores ORDER BY Matricula ASC;";
                string comandoSQL = $"ATTACH '{path}' AS Arr; SELECT * FROM Conductores; SELECT * FROM Arr.Conductores;";
                var consulta = new SQLiteExpression(comandoSQL);
                using (var conexion = new SQLiteConnection(App.Global.CadenaConexionSQL)) {
                    conexion.Open();
                    using (var comando = consulta.GetCommand(conexion)) {
                        using (var lector = comando.ExecuteReader()) {
                            var lista = new List<Conductor>();
                            do {
                                while (lector.Read()) {
                                    var conductor = new Conductor();
                                    conductor.FromReader(lector);
                                    lista.Add(conductor);
                                }
                            } while (lector.NextResult());
                            return lista;
                        }
                    }
                }
                //return GetItems<Conductor>(consulta);
            } catch (Exception ex) {
                Utils.VerError(nameof(this.GetPrueba), ex);
            }
            return new List<Conductor>();
        }






    }
}
