#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Data.Common;

namespace Orion.Pijama {
    public class ResumenDias {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public ResumenDias() { }


        public ResumenDias(DbDataReader lector) {
            DiasTrabajados = lector.ToInt32("DiasTrabajados");
            DiasOV = lector.ToInt32("DiasOV");
            DiasJD = lector.ToInt32("DiasJD");
            DiasFN = lector.ToInt32("DiasFN");
            DiasE = lector.ToInt32("DiasE");
            DiasDS = lector.ToInt32("DiasDS");
            DiasDC = lector.ToInt32("DiasDC");
            DiasF6 = lector.ToInt32("DiasF6");
            DiasDND = lector.ToInt32("DiasDND");
            DiasPER = lector.ToInt32("DiasPER");
            DiasEJD = lector.ToInt32("DiasEJD");
            DiasEFN = lector.ToInt32("DiasEFN");
            DiasOVJD = lector.ToInt32("DiasOVJD");
            DiasOVFN = lector.ToInt32("DiasOVFN");
            DiasF6DC = lector.ToInt32("DiasF6DC");
            DiasFOR = lector.ToInt32("DiasFOR");
            DiasOVA = lector.ToInt32("DiasOVA");
            DiasOVAJD = lector.ToInt32("DiasOVAJD");
            DiasOVAFN = lector.ToInt32("DiasOVAFN");
            DiasCO = lector.ToInt32("DiasCO");
            DiasCE = lector.ToInt32("DiasCE");
            DiasJDTrabajados = lector.ToInt32("DiasJDTrabajados");
        }

        #endregion
        // ====================================================================================================





        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public int DiasTrabajados { get; set; }

        public int DiasOV { get; set; }

        public int DiasJD { get; set; }

        public int DiasFN { get; set; }

        public int DiasE { get; set; }

        public int DiasDS { get; set; }

        public int DiasDC { get; set; }

        public int DiasF6 { get; set; }

        public int DiasDND { get; set; }

        public int DiasPER { get; set; }

        public int DiasEJD { get; set; }

        public int DiasEFN { get; set; }

        public int DiasOVJD { get; set; }

        public int DiasOVFN { get; set; }

        public int DiasF6DC { get; set; }

        public int DiasFOR { get; set; }

        public int DiasOVA { get; set; }

        public int DiasOVAJD { get; set; }

        public int DiasOVAFN { get; set; }

        public int DiasCO { get; set; }

        public int DiasCE { get; set; }

        public int DiasJDTrabajados { get; set; }


        #endregion
        // ====================================================================================================


    }
}
