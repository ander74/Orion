using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

    public class DiaCalendarioConductor : DiaCalendarioBase {


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================



        private int matriculaConductor;
        public int MatriculaConductor {
            get => matriculaConductor;
            set => SetValue(ref matriculaConductor, value);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region INTERFAZ ISQLITEITEM
        // ====================================================================================================

        public override void FromReader(DbDataReader lector) {
            base.FromReader(lector);
            matriculaConductor = lector.ToInt32("MatriculaConductor");
        }


        #endregion
        // ====================================================================================================


    }
}
