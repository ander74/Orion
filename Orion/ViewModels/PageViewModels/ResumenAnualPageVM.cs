#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.ViewModels.PageViewModels {

    public class ResumenAnualPageVM : PageViewModelBase {


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static ResumenAnualPageVM instance;

        private ResumenAnualPageVM() { }

        public static ResumenAnualPageVM GetInstance() {
            if (instance == null) instance = new ResumenAnualPageVM();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        // Mapeado a los viewmodels que necesitemos usar.
        // ----------------------------------------------
        public ConductoresViewModel ConductoresVM => App.Global.ConductoresVM;
        public CalendariosViewModel CalendariosVM => App.Global.CalendariosVM;



        #endregion
        // ====================================================================================================



    }
}
