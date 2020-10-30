#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Windows.Input;
using Orion.Models;
using Orion.Servicios;
using Orion.ViewModels.PageViewModels;

namespace Orion.ViewModels {

    public class InformesViewModel : NotifyBase {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private readonly IMensajes mensajes;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public InformesViewModel(IMensajes mensajes) {
            this.mensajes = mensajes;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private PageViewModelBase currentPageViewModel;
        public PageViewModelBase CurrentPageViewModel {
            get => currentPageViewModel;
            set => SetValue(ref currentPageViewModel, value);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================


        #region COMANDO 
        // Comando
        private ICommand cmdCambiarViewModel;
        public ICommand CmdCambiarViewModel {
            get {
                if (cmdCambiarViewModel == null) cmdCambiarViewModel = new RelayCommand(p => CambiarViewModel(p));
                return cmdCambiarViewModel;
            }
        }
        // Ejecución del comando
        private void CambiarViewModel(object parametro) {
            if (parametro is string s) {
                switch (s) {
                    case "CompararGrupos":
                        CurrentPageViewModel = CompararGraficosPageVM.GetInstance();
                        break;
                    default:
                        CurrentPageViewModel = null;
                        break;
                }
            }
        }
        #endregion


        #endregion
        // ====================================================================================================


    }
}
