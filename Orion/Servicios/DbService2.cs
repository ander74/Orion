#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.Servicios {
    public class DbService2 {


        // ====================================================================================================
        #region CONSULTAS ACCESS
        // ====================================================================================================

        public const string ActualizarCalendario = "UPDATE Calendarios SET IdConductor = @IdConductor, Fecha = @Fecha, Notas = @Notas WHERE Id=@Id;";


        public const string ActualizarConductor = "UPDATE Conductores SET Nombre = @Nombre, Apellidos = @Apellidos, Indefinido = @Indefinido, Telefono = @Telefono, Email = @Email, Acumuladas = @Acumuladas, Descansos = @Descansos, DescansosNoDisfrutados = @DescansosNoDisfrutados, PlusDistancia = @PlusDistancia, ReduccionJornada = @ReduccionJornada, Notas = @Notas WHERE Id=@Id;";


        public const string ActualizarDiaCalendario = "UPDATE DiasCalendario SET IdCalendario = @IdCalendario, Dia = @Dia, DiaFecha = @DiaFecha, Grafico = @Grafico, Codigo = @Codigo, ExcesoJornada = @ExcesoJornada, FacturadoPaqueteria = @FacturadoPaqueteria, Limpieza = @Limpieza, GraficoVinculado = @GraficoVinculado, Notas = @Notas WHERE Id=@Id;";


        public const string ActualizarFestivo = "UPDATE Festivos SET Año = @Año, Fecha = @Fecha WHERE Id=@Id;";


        public const string ActualizarGrafico = "UPDATE Graficos SET IdGrupo = @IdGrupo, NoCalcular = @NoCalcular, Numero = @Numero, Turno = @Turno, DescuadreInicio = @DescuadreInicio, Inicio = @Inicio, Final = @Final, DescuadreFinal = @DescuadreFinal, InicioPartido = @InicioPartido, FinalPartido = @FinalPartido, Valoracion = @Valoracion, Trabajadas = @Trabajadas, Acumuladas = @Acumuladas, Nocturnas = @Nocturnas, Desayuno = @Desayuno, Comida = @Comida, Cena = @Cena, PlusCena = @PlusCena, PlusLimpieza = @PlusLimpieza, PlusPaqueteria = @ WHERE Id=@Id;";


        public const string ActualizarGrupo = "UPDATE GruposGraficos SET Validez = @Validez, Notas = @Notas WHERE Id=@Id;";


        public const string ActualizarItinerario = "UPDATE Itinerarios SET IdLinea = @IdLinea, Nombre = @Nombre, Descripcion = @Descripcion, TiempoReal = @TiempoReal, TiempoPago = @TiempoPago WHERE Id=@Id;";


        public const string ActualizarLinea = "UPDATE Lineas SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id=@Id;";


        public const string ActualizarParada = "UPDATE Paradas SET IdItinerario = @IdItinerario, Orden = @Orden, Descripcion = @Descripcion, Tiempo = @Tiempo WHERE Id=@Id;";


        public const string ActualizarRegulacion = "UPDATE Regulaciones SET IdConductor = @IdConductor, Codigo = @Codigo, Fecha = @Fecha, Horas = @Horas, Descansos = @Descansos, Motivo = @Motivo WHERE Id=@Id;";


        public const string ActualizarValoracion = "UPDATE Valoraciones SET IdGrafico = @IdGrafico, Inicio = @Inicio, Linea = @Linea, Descripcion = @Descripcion, Final = @Final, Tiempo = @Tiempo WHERE Id=@;";


        public const string CambiarFechasDiasCalendario = "UPDATE (SELECT DiasCalendario.DiaFecha AS F, DateSerial(Year(Calendarios.Fecha),Month(Calendarios.Fecha),DiasCalendario.Dia) AS FF FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario) SET F = FF;";


        public const string InsertarCalendario = "INSERT INTO Calendarios ( IdConductor, Fecha, Notas ) VALUES (@IdConductor, @Fecha, @Notas);";


        public const string InsertarConductor = "INSERT INTO Conductores ( Nombre, Apellidos, Indefinido, Telefono, Email, Acumuladas, Descansos, DescansosNoDisfrutados, PlusDistancia, ReduccionJornada, Notas, Id ) VALUES (@Nombre, @Apellidos, @Indefinido, @Telefono, @Email, @Acumuladas, @Descansos, @DescansosNoDisfrutados, @PlusDistancia, @ReduccionJornada, @Notas, @Id);";


        public const string InsertarConductorDesconocido = "INSERT INTO Conductores ( Id, Nombre ) VALUES (@Id, 'Desconocido');";


        public const string InsertarDiaCalendario = "INSERT INTO DiasCalendario ( IdCalendario, Dia, DiaFecha, Grafico, Codigo, ExcesoJornada, FacturadoPaqueteria, Limpieza, GraficoVinculado, Notas ) VALUES (@IdCalendario, @Dia, @DiaFecha, @Grafico, @Codigo, @ExcesoJornada, @FacturadoPaqueteria, @Limpieza, @GraficoVinculado, @Notas);";


        public const string InsertarFestivo = "INSERT INTO Festivos ( Año, Fecha ) VALUES (@Año, @Fecha);";


        public const string InsertarGrafico = "INSERT INTO Graficos ( IdGrupo, NoCalcular, Numero, Turno, DescuadreInicio, Inicio, Final, DescuadreFinal, InicioPartido, FinalPartido, Valoracion, Trabajadas, Acumuladas, Nocturnas, Desayuno, Comida, Cena, PlusCena, PlusLimpieza, PlusPaqueteria ) VALUES (@IdGrupo, @NoCalcular, @Numero, @Turno, @DescuadreInicio, @Inicio, @Final, @DescuadreFinal, @InicioPartido, @FinalPartido, @Valoracion, @Trabajadas, @Acumuladas, @Nocturnas, @Desayuno, @Comida, @Cena, @PlusCena, @PlusLimpieza, @PlusPaqueteria);";


        public const string InsertarGrupo = "INSERT INTO GruposGraficos ( Validez, Notas ) VALUES (@Validez, @Notas);";


        public const string InsertarItinerario = "INSERT INTO Itinerarios ( IdLinea, Nombre, Descripcion, TiempoReal, TiempoPago ) VALUES (@IdLinea, @Nombre, @Descripcion, @TiempoReal, @TiempoPago);";


        public const string InsertarLinea = "INSERT INTO Lineas ( Nombre, Descripcion ) VALUES (@Nombre, @Descripcion);";


        public const string InsertarParada = "INSERT INTO Paradas ( IdItinerario, Orden, Descripcion, Tiempo ) VALUES (@IdItinerario, @Orden, @Descripcion, @Tiempo);";


        public const string InsertarRegulacion = "INSERT INTO Regulaciones ( IdConductor, Codigo, Fecha, Horas, Descansos, Motivo ) VALUES (@IdConductor, @Codigo, @Fecha, @Horas, @Descansos, @Motivo);";


        public const string InsertarValoracion = "INSERT INTO Valoraciones ( IdGrafico, Inicio, Linea, Descripcion, Final, Tiempo ) VALUES (@IdGrafico, @Inicio, @Linea, @Descripcion, @Final, @Tiempo);";


        public const string BorrarCalendario = "DELETE * FROM Calendarios WHERE Id=@Id;";


        public const string BorrarConductor = "DELETE * FROM Conductores WHERE Id=@Id;";


        public const string BorrarDiaCalendario = "DELETE * FROM DiasCalendario WHERE Id=@Id;";


        public const string BorrarFestivo = "DELETE * FROM Festivos WHERE Id=@Id;";


        public const string BorrarGrafico = "DELETE * FROM Graficos WHERE Id=@Id;";


        public const string BorrarGrupo = "DELETE * FROM GruposGraficos WHERE Id=@Id;";


        public const string BorrarItinerario = "DELETE * FROM Itinerarios WHERE Id=@Id;";


        public const string BorrarLinea = "DELETE * FROM Lineas WHERE Id=@Id;";


        public const string BorrarParada = "DELETE * FROM Paradas WHERE Id=@Id;";


        public const string BorrarRegulacion = "DELETE * FROM Regulaciones WHERE Id=@Id;";


        public const string BorrarValoracion = "DELETE * FROM Valoraciones WHERE Id=@Id;";


        public const string ConsultaPrueba = "SELECT (Sum(Graficos.Inicio) + Sum(Calendarios.Id)) AS Resultado FROM Graficos, Calendarios WHERE Graficos.Id = 123 AND Calendarios.Id = 190;";


        public const string ExisteConductor = "SELECT Count(Id) FROM Conductores WHERE Id=@Id;";


        public const string ExisteGrupo = "SELECT Count(*) FROM GruposGraficos WHERE Validez=@Validez;";


        public const string GetCalendarios = "SELECT Calendarios.*, Conductores.Indefinido FROM Calendarios LEFT JOIN Conductores ON Calendarios.IdConductor = Conductores.Id WHERE Year(Calendarios.Fecha)=@Año And Month(Calendarios.Fecha)=@Mes ORDER BY Calendarios.IdConductor;";


        public const string GetConductor = "SELECT * FROM Conductores WHERE Id=@Id;";


        public const string GetConductores = "SELECT * FROM Conductores ORDER BY Id;";


        public const string GetConsultas = "SELECT Name FROM MSysObjects WHERE Type = 5 ORDER BY Name;";


        public const string GetDiasCalendario = "SELECT * FROM DiasCalendario WHERE IdCalendario=@IdCalendario ORDER BY Dia;";


        public const string GetDiasCalendarioAño = "SELECT * FROM DiasCalendario WHERE IdCalendario IN (SELECT Id FROM Calendarios WHERE IdConductor = @IdConductor AND Year(Fecha) = @Año);";


        public const string GetDiasCalendarioConBloqueos = "SELECT * FROM DiasCalendario WHERE (ExcesoJornada <> 0 OR Descuadre <> 0) AND IdCalendario IN (SELECT Id FROM Calendarios WHERE IdConductor = @IdConductor);";


        public const string GetDiasPijama = "SELECT * FROM (SELECT * FROM Graficos WHERE IdGrupo = (SELECT Id FROM GruposGraficos WHERE Validez = (SELECT Max(Validez) FROM GruposGraficos WHERE Validez <= @Validez))) WHERE Numero=@Numero;";


        public const string GetEstadisticasGraficos = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@JornadaMedia AND NOT NoCalcular,@JornadaMedia,Trabajadas)) AS xTrabajadas, Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos.Id = Graficos.IdGrupo WHERE IdGrupo=@IdGrupo GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string GetEstadisticasGraficosDesdeFecha = "SELECT GruposGraficos.Validez AS xValidez, Turno AS xTurno, Count(Numero) AS xNumero, Sum(Valoracion) AS xValoracion, Sum(IIf(Trabajadas<@JornadaMedia AND NOT NoCalcular,@JornadaMedia,Trabajadas)) AS xTrabajadas, Sum(Acumuladas) AS xAcumuladas, Sum(Nocturnas) AS xNocturnas, Sum(Desayuno) AS xDesayuno, Sum(Comida) AS xComida, Sum(Cena) AS xCena, Sum(PlusCena) AS xPlusCena, Sum(iif(PlusLimpieza=True,1,0)) AS xLimpieza, Sum(iif(PlusPaqueteria=True,1,0)) AS xPaqueteria FROM GruposGraficos LEFT JOIN Graficos ON GruposGraficos.Id = Graficos.IdGrupo WHERE GruposGraficos.Validez >= @Fecha GROUP BY GruposGraficos.Validez, Turno ORDER BY GruposGraficos.Validez, Turno;";


        public const string GetFestivos = "SELECT * FROM Festivos ORDER BY Fecha;";


        public const string GetFestivosPorAño = "SELECT * FROM Festivos WHERE Año=@Año ORDER BY Fecha;";


        public const string GetFestivosPorMes = "SELECT * FROM Festivos WHERE Año=@Año And Month(Fecha)=@Mes ORDER BY Fecha;";


        public const string GetGraficos = "SELECT * FROM Graficos WHERE IdGrupo=@IdGrupo ORDER BY Numero;";


        public const string GetGraficosGrupoPorFecha = "SELECT * FROM Graficos WHERE IdGrupo = (SELECT Id FROM GruposGraficos WHERE Validez = @Validez);";


        public const string GetGrupos = "SELECT * FROM GruposGraficos ORDER BY Validez DESC;";


        public const string GetItinerarioPorNombre = "SELECT * FROM Itinerarios WHERE Name=@Nombre;";


        public const string GetItinerarios = "SELECT * FROM Itinerarios WHERE IdLinea=@ ORDER BY Nombre;";


        public const string GetLineas = "SELECT * FROM Lineas ORDER BY Nombre;";


        public const string GetParadas = "SELECT * FROM Paradas WHERE IdItinerario=@IdItinerario ORDER BY Orden;";


        public const string GetRegulaciones = "SELECT * FROM Regulaciones WHERE IdConductor=@IdConductor ORDER BY Fecha, Id;";


        public const string GetValoraciones = "SELECT * FROM Valoraciones WHERE IdGrafico=@IdGrafico ORDER BY Inicio, Id;";


        public const string Temporal = "SELECT DiasCalendario.Dia, Calendarios.IdConductor, DiasCalendario.Fecha, DiasCalendario.ExcesoJornada FROM Calendarios LEFT JOIN DiasCalendario ON Calendarios.Id = DiasCalendario.IdCalendario WHERE Calendarios.IdConductor = 450 AND ExcesoJornada > 0;";


        public const string Identity = "SELECT last_insert_rowid()";









        #endregion
        // ====================================================================================================







    }
}
