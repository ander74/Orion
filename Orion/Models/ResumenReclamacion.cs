#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;

namespace Orion.Models {

    public class ResumenReclamacion {

        public string RowHeader { get; set; }

        public TimeSpan TotalAcumuladas { get; set; }

        public decimal TotalDesayunos { get; set; }

        public decimal ImporteDesayunos { get; set; }

        public decimal TotalComidas { get; set; }

        public decimal ImporteComidas { get; set; }

        public decimal TotalCenas { get; set; }

        public decimal ImporteCenas { get; set; }

        public decimal TotalPlusCenas { get; set; }

        public decimal ImportePlusCenas { get; set; }

        public bool IsEmpty {
            get {
                return TotalAcumuladas == TimeSpan.Zero && TotalDesayunos == 0 && TotalComidas == 0 && TotalCenas == 0 && TotalPlusCenas == 0;
            }
        }

    }
}
