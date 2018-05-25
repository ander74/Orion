using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

    public class DescansosPorDia {

        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public int Descansos { get; set; }


        public DateTime Fecha { get; set; }


        public int Dia {
            get { return Fecha.Day; }
        }



        #endregion
        // ====================================================================================================


    }
}
