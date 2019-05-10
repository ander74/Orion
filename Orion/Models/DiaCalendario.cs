#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    using System;
    using System.ComponentModel;
    using System.Data.OleDb;

    public class DiaCalendario : DiaCalendarioBase {


        // ====================================================================================================
        #region  CONSTRUCTORES
        // ====================================================================================================
        public DiaCalendario() : base() {
            PropertyChanged += LlamarPropiedadCambiada;
        }

        public DiaCalendario(OleDbDataReader lector) : base(lector) {
            PropertyChanged += LlamarPropiedadCambiada;
        }

        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void LlamarPropiedadCambiada(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(ExcesoJornada):
                case nameof(FacturadoPaqueteria):
                case nameof(Limpieza):
                case nameof(TurnoAlt):
                case nameof(InicioAlt):
                case nameof(FinalAlt):
                case nameof(InicioPartidoAlt):
                case nameof(FinalPartidoAlt):
                case nameof(TrabajadasAlt):
                case nameof(AcumuladasAlt):
                case nameof(NocturnasAlt):
                case nameof(DesayunoAlt):
                case nameof(ComidaAlt):
                case nameof(CenaAlt):
                case nameof(PlusCenaAlt):
                case nameof(PlusLimpiezaAlt):
                case nameof(PlusPaqueteriaAlt):
                    PropiedadCambiada(nameof(HayExtras));
                    break;
                case nameof(Notas):
                    PropiedadCambiada(nameof(HayNotas));
                    PropiedadCambiada(nameof(HayExtras));
                    break;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public string HayNotas {
            get {
                if (String.IsNullOrEmpty(Notas)) return "";
                return "...";
            }
        }


        public bool HayExtras {
            get {
                if (ExcesoJornada.Ticks != 0) return true;
                if (FacturadoPaqueteria != 0) return true;
                if (Limpieza != false) return true;
                if (!String.IsNullOrWhiteSpace(Notas)) return true;
                if (TurnoAlt != null || InicioAlt != null || InicioPartidoAlt != null) return true;
                if (PlusCenaAlt != null || FinalAlt != null || FinalPartidoAlt != null) return true;
                if (TrabajadasAlt != null || AcumuladasAlt != null || NocturnasAlt != null) return true;
                if (DesayunoAlt != null || ComidaAlt != null || CenaAlt != null) return true;
                if (PlusLimpiezaAlt != null || PlusPaqueteriaAlt != null) return true;
                return false;
            }
        }


        private bool _resaltar;
        public bool Resaltar {
            get { return _resaltar; }
            set {
                if (_resaltar != value) {
                    _resaltar = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion


    }
}
