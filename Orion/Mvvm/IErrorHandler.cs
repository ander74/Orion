#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.MVVM {

    using System;

    public interface IErrorHandler {
        void HandleError(Exception ex);
    }

}
