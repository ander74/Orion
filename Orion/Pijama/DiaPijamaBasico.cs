#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Data.Common;

namespace Orion.Pijama {

    /// <summary>
    /// Esta clase contiene los datos de un día completo, combinando los datos de los días de calendario con los grupos de gráficos.<br/>
    /// Si en el día del calendario hay datos alternativos, estos sustituirán a los datos del gráfico.<br/>
    /// Es una clase que contendrá datos de sólo lectura, con lo que no se incluye el patrón observable.
    /// </summary>
    public class DiaPijamaBasico {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================


        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        //public DiaPijamaBasico() { }

        public DiaPijamaBasico(DbDataReader lector) {
            FromReader(lector);
            // Si hay una hora final, se le añade el exceso de jornada.
            if (Final.HasValue) Final += ExcesoJornada; //TODO: Habría que recalcular las horas.
            // TODO: Añadir la reducción de jornada del conductor.
            // Si no hay reducción de jornada y se trabajan menos de la jornada media, establecemos Trabajadas como jornada media.
            if (!ReduccionJornada && NumeroGrafico > 0 && Trabajadas < App.Global.Convenio.JornadaMedia) Trabajadas = App.Global.Convenio.JornadaMedia;
            // Si la incidencia es Enfermo (-4), DC (-6), F6 (-7), DND (-8), Permiso (-9), F6(DC) (-14), Formación (-15) o OVA (-16) Trabajadas = jornada media.
            if (NumeroGrafico == -4 || NumeroGrafico == -6 || NumeroGrafico == -7 || NumeroGrafico == -8 ||
                NumeroGrafico == -9 || NumeroGrafico == -14 || NumeroGrafico == -15 || NumeroGrafico == -16) Trabajadas = App.Global.Convenio.JornadaMedia;
            // Si es día de comite, se añade una dieta de comida.
            if ((Codigo == 1 || Codigo == 2) && Comida < 1) Comida = 1m;
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public virtual void FromReader(DbDataReader lector) {

            MatriculaConductor = lector.ToInt32("MatriculaConductor");
            Fecha = lector.ToDateTime("Fecha");
            NumeroGrafico = lector.ToInt32("NumeroGrafico");
            Codigo = lector.ToInt32("Codigo");
            ExcesoJornada = lector.ToTimeSpan("ExcesoJornada");
            FacturadoPaqueteria = lector.ToDecimal("FacturadoPaqueteria");
            Limpieza = lector.ToBoolNulable("Limpieza");
            NumeroGraficoVinculado = lector.ToInt32("NumeroGraficoVinculado");
            ValidezGrafico = lector.ToDateTime("ValidezGrafico");
            DiaSemanaGrafico = lector.ToString("DiaSemanaGrafico");
            Turno = lector.ToInt32("Turno");
            Inicio = lector.ToTimeSpanNulable("Inicio");
            Final = lector.ToTimeSpanNulable("Final");
            InicioPartido = lector.ToTimeSpanNulable("InicioPartido");
            FinalPartido = lector.ToTimeSpanNulable("FinalPartido");
            ValoracionGrafico = lector.ToTimeSpan("ValoracionGrafico");
            Trabajadas = lector.ToTimeSpan("Trabajadas");
            Acumuladas = lector.ToTimeSpan("Acumuladas");
            Nocturnas = lector.ToTimeSpan("Nocturnas");
            Desayuno = lector.ToDecimal("Desayuno");
            Comida = lector.ToDecimal("Comida");
            Cena = lector.ToDecimal("Cena");
            PlusCena = lector.ToDecimal("PlusCena");
            PlusLimpieza = lector.ToBool("PlusLimpieza");
            PlusPaqueteria = lector.ToBool("PlusPaqueteria");

        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================


        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public int MatriculaConductor { get; set; }

        public bool ReduccionJornada { get; set; }

        public DateTime Fecha { get; set; }

        public int NumeroGrafico { get; set; }

        public int Codigo { get; set; }

        public TimeSpan ExcesoJornada { get; set; }

        public decimal FacturadoPaqueteria { get; set; }

        public bool? Limpieza { get; set; }

        public int NumeroGraficoVinculado { get; set; } //TODO: Plantearse eliminar este campo, ya que no se utilizará.

        public DateTime ValidezGrafico { get; set; }

        public string DiaSemanaGrafico { get; set; }

        public int Turno { get; set; }

        public TimeSpan? Inicio { get; set; }

        public TimeSpan? Final { get; set; }

        public TimeSpan? InicioPartido { get; set; }

        public TimeSpan? FinalPartido { get; set; }

        public TimeSpan ValoracionGrafico { get; set; }

        public TimeSpan Trabajadas { get; set; }

        public TimeSpan Acumuladas { get; set; }

        public TimeSpan Nocturnas { get; set; }

        public decimal Desayuno { get; set; }

        public decimal Comida { get; set; }

        public decimal Cena { get; set; }

        public decimal PlusCena { get; set; }

        public bool PlusLimpieza { get; set; }

        public bool PlusPaqueteria { get; set; }







        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES CALCULADAS (SOLO LECTURA)
        // ====================================================================================================


        public decimal ImporteDesayuno => (Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;

        public decimal ImporteComida => Comida * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;

        public decimal ImporteCena => Cena * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;

        public decimal ImportePlusCena => PlusCena * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;

        public decimal ImporteTotalDietas => ImporteDesayuno + ImporteComida + ImporteCena + ImportePlusCena;

        public TimeSpan TiempoPartido => Config.Calculos.Horas.TiempoPartido(InicioPartido, FinalPartido);

        public bool EsFestivo => App.Global.CalendariosVM.EsFestivo(Fecha);

        public decimal ImportePlusPaqueteria => PlusPaqueteria ? App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusPaqueteria : FacturadoPaqueteria * 0.10m;

        public decimal ImportePlusLimpieza {
            get {
                if (PlusLimpieza || Limpieza == true) {
                    return App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                }
                return Limpieza == null ? App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza / 2m : 0m;
            }
        }


        public decimal ImportePlusNocturnidad => Turno == 3 ? App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusNocturnidad : 0m;

        public decimal ImportePlusNavidad {
            get {
                if (NumeroGrafico > 0 && ((Fecha.Month == 12 && Fecha.Day == 25) || (Fecha.Month == 1 && Fecha.Day == 1))) {
                    return App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusNavidad;
                }
                return 0;
            }
        }

        #endregion
        // ====================================================================================================


    }
}
