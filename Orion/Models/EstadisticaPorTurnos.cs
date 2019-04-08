#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    public class EstadisticaPorTurnos : NotifyBase {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public EstadisticaPorTurnos() {
            Dias = new string[5];
            Trabajadas = new string[5];
            Acumuladas = new string[5];
            Nocturnas = new string[5];
            ImporteDietas = new string[5];
            DiasFestivos = new string[5];

        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public Conductor Conductor { get; set; }

        public string[] Dias { get; set; }

        public string[] Trabajadas { get; set; }

        public string[] Acumuladas { get; set; }

        public string[] Nocturnas { get; set; }

        public string[] ImporteDietas { get; set; }

        public string[] DiasFestivos { get; set; }

        public string DiasTrabajo { get; set; }

        public string DiasDescanso { get; set; }

        public string FindesTrabajados { get; set; }

        public string FindesDescansados { get; set; }

        public string PlusPaqueteria { get; set; }

        public string PlusMenorDescanso { get; set; }



        #endregion
        // ====================================================================================================


    }
}
