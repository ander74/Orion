#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;

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

        public int[] Dias { get; set; }

        public TimeSpan[] Trabajadas { get; set; }

        public TimeSpan[] Acumuladas { get; set; }

        public TimeSpan[] Nocturnas { get; set; }

        public decimal[] ImporteDietas { get; set; }

        public int[] DiasFestivos { get; set; }

        public int DiasTrabajo { get; set; }

        public int DiasDescanso { get; set; }

        public decimal FindesTrabajados { get; set; }

        public decimal FindesDescansados { get; set; }

        public decimal PlusPaqueteria { get; set; }

        public decimal PlusMenorDescanso { get; set; }



        #endregion
        // ====================================================================================================


    }
}
