﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;

namespace Orion.Config {

    public class Rango<T> where T : IComparable {

        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public Rango(T valorUnico) {
            ValorMinimo = valorUnico;
            ValorMaximo = valorUnico;
        }


        public Rango(T minimo, T maximo) {
            ValorMinimo = minimo;
            ValorMaximo = maximo;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public bool Contiene(T valor) {
            return ValorMinimo.CompareTo(valor) <= 0 & ValorMaximo.CompareTo(valor) >= 0;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public T ValorMinimo { get; set; }

        public T ValorMaximo { get; set; }

        #endregion
        // ====================================================================================================



    }


}
