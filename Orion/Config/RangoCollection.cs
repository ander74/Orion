#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;

namespace Orion.Config {

    public class RangoCollection<T> : List<Rango<T>> where T : IComparable {


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public bool Validar(T valor) {
            foreach (var rango in this) {
                if (rango.Contiene(valor)) return true;
            }
            return false;
        }

        #endregion
        // ====================================================================================================


    }
}
