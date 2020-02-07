#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Linq;

namespace Orion.Models {

    public class EstadisticaPorTurnos : NotifyBase {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public EstadisticaPorTurnos() {
            Dias = new int[5];
            Trabajadas = new TimeSpan[5];
            Acumuladas = new TimeSpan[5];
            Nocturnas = new TimeSpan[5];
            ImporteDietas = new decimal[5];
            DiasFestivos = new int[5];

        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public Conductor Conductor { get; set; }

        public bool EventualParcial { get; set; }

        public int[] Dias { get; set; }
        public int TotalDias { get => Dias.Sum(); }


        public TimeSpan[] Trabajadas { get; set; }
        public TimeSpan TotalTrabajadas { get => new TimeSpan(Trabajadas.Sum(h => h.Ticks)); }


        public TimeSpan[] Acumuladas { get; set; }
        public TimeSpan TotalAcumuladas { get => new TimeSpan(Acumuladas.Sum(h => h.Ticks)); }


        public TimeSpan[] Nocturnas { get; set; }
        public TimeSpan TotalNocturnas { get => new TimeSpan(Nocturnas.Sum(h => h.Ticks)); }


        public decimal[] ImporteDietas { get; set; }
        public decimal TotalImporteDietas { get => ImporteDietas.Sum(); }


        public int[] DiasFestivos { get; set; }
        public int TotalDiasFestivos { get => DiasFestivos.Sum(); }


        public int DiasDescanso { get; set; }

        public decimal FindesDescansados { get; set; }

        public decimal PlusPaqueteria { get; set; }

        public decimal PlusMenorDescanso { get; set; }

        #endregion
        // ====================================================================================================


    }
}
