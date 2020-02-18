#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Threading.Tasks;

namespace Orion.MVVM {

    public static class TaskUtilities {

        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null) {
            try {
                await task;
            } catch (Exception ex) {
                handler?.HandleError(ex);
            }
        }

    }
}
