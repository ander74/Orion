#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Data.Common;

namespace Orion.Models {
    public class DiaCalendarioEstadistica {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public DiaCalendarioEstadistica() { }


        public DiaCalendarioEstadistica(DbDataReader lector) {
            FromReader(lector);
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        public DateTime Fecha { get; set; }

        public int MatriculaConductor { get; set; }

        public int NumeroGrafico { get; set; }

        public int Codigo { get; set; }

        public int Turno { get; set; }

        public TimeSpan Trabajadas { get; set; }

        public TimeSpan Acumuladas { get; set; }

        public TimeSpan Nocturnas { get; set; }

        public decimal Desayuno { get; set; }

        public decimal Comida { get; set; }

        public decimal Cena { get; set; }

        public decimal PlusCena { get; set; }

        public bool PlusLimpieza { get; set; }

        public bool PlusPaqueteria { get; set; }


        public int TurnoAlt { get; set; }

        public TimeSpan TrabajadasAlt { get; set; }

        public TimeSpan AcumuladasAlt { get; set; }

        public TimeSpan NocturnasAlt { get; set; }

        public decimal DesayunoAlt { get; set; }

        public decimal ComidaAlt { get; set; }

        public decimal CenaAlt { get; set; }

        public decimal PlusCenaAlt { get; set; }

        public bool PlusLimpiezaAlt { get; set; }

        public bool PlusPaqueteriaAlt { get; set; }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public virtual void FromReader(DbDataReader lector) {
            Fecha = lector.ToDateTime("Fecha");
            MatriculaConductor = lector.ToInt32("MatriculaConductor");
            NumeroGrafico = lector.ToInt32("NumeroGrafico");
            Codigo = lector.ToInt32("Codigo");
            Turno = lector.ToInt32("Turno");
            Trabajadas = lector.ToTimeSpan("Trabajadas");
            Acumuladas = lector.ToTimeSpan("Acumuladas");
            Nocturnas = lector.ToTimeSpan("Nocturnas");
            Desayuno = lector.ToDecimal("Desayuno");
            Comida = lector.ToDecimal("Comida");
            Cena = lector.ToDecimal("Cena");
            PlusCena = lector.ToDecimal("PlusCena");
            PlusLimpieza = lector.ToBool("PlusLimpieza");
            PlusPaqueteria = lector.ToBool("PlusPaqueteria");
            TurnoAlt = lector.ToInt32("TurnoAlt");
            TrabajadasAlt = lector.ToTimeSpan("TrabajadasAlt");
            AcumuladasAlt = lector.ToTimeSpan("AcumuladasAlt");
            NocturnasAlt = lector.ToTimeSpan("NocturnasAlt");
            DesayunoAlt = lector.ToDecimal("DesayunoAlt");
            ComidaAlt = lector.ToDecimal("ComidaAlt");
            CenaAlt = lector.ToDecimal("CenaAlt");
            PlusCenaAlt = lector.ToDecimal("PlusCenaAlt");
            PlusLimpiezaAlt = lector.ToBool("PlusLimpiezaAlt");
            PlusPaqueteriaAlt = lector.ToBool("PlusPaqueteriaAlt");
        }


        #endregion
        // ====================================================================================================




    }
}
