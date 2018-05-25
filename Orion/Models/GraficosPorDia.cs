using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

    public class GraficosPorDia {

        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public List<int> Lista { get; set; } = new List<int>();


        public DateTime Fecha { get; set; }


        public int Dia {
            get { return Fecha.Day; }
        }


        #endregion
        // ====================================================================================================

    }
}
