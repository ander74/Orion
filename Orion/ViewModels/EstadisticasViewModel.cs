#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using LiveCharts;
using Orion.DataModels;
using Orion.Models;
using Orion.Servicios;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
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
            // Activamos los View Models
            _tablerograficosvm = new TableroGraficosViewModel(mensajes, informes);
            BtGraficosActivo = true;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private void OcultarBotones() {
            _btgraficosactivo = false;
            _btcalendariosactivo = false;
        }

        private void OcultarTableros() {
            TableroGraficosVM.VerTablero = false;
        }

        private void NotificarBotones() {
            PropiedadCambiada(nameof(BtGraficosActivo));
            PropiedadCambiada(nameof(BtCalendariosActivo));
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES VISUALES
        // ====================================================================================================

        private bool _btgraficosactivo;
        public bool BtGraficosActivo {
            get { return _btgraficosactivo; }
            set {
                if (_btgraficosactivo != value) {
                    if (value) {
                        OcultarBotones();
                        OcultarTableros();
                        TableroGraficosVM.VerTablero = true;
                        TableroGraficosVM.CargarDatos();
                    }
                    _btgraficosactivo = value;
                    NotificarBotones();
                }
            }
        }


        private bool _btcalendariosactivo;
        public bool BtCalendariosActivo {
            get { return _btcalendariosactivo; }
            set {
                if (_btcalendariosactivo != value) {
                    if (value) OcultarBotones();
                    _btcalendariosactivo = value;
                    NotificarBotones();
                }
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region VIEW MODELS SECUNDARIOS
        // ====================================================================================================

        TableroGraficosViewModel _tablerograficosvm = null;
        public TableroGraficosViewModel TableroGraficosVM {
            get {
                return _tablerograficosvm;
            }
        }




        #endregion
        // ====================================================================================================


    }
}
