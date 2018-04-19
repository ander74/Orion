#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using Orion.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.ViewModels
{
    public partial class EstadisticasViewModel: NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private IMensajes mensajes;
        private InformesServicio informes;
        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public EstadisticasViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes) {
            this.mensajes = servicioMensajes;
            this.informes = servicioInformes;
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region PROPIEDADES VISUALES
        // ====================================================================================================


        private bool _vernadaquemostrar = true;
        public bool VerNadaQueMostrar {
            get { return _vernadaquemostrar; }
            set {
                if (_vernadaquemostrar != value)
                {
                    _vernadaquemostrar = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================



    }
}
