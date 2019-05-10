#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {

    public class GraficoFecha: GraficoBase {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public GraficoFecha() : base() {
        }


        public GraficoFecha(OleDbDataReader lector) : base(lector) {
        }


        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private DateTime _fecha;
        public DateTime Fecha {
            get { return _fecha; }
            set {
                if (_fecha != value) {
                    _fecha = value;
                    PropiedadCambiada();
                }
            }
        }


        public int Dia {
            get { return Fecha.Day; }
        }





        #endregion
        // ====================================================================================================


    }
}
