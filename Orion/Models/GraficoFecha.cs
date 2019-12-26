#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Data.OleDb;
using System.Data.SQLite;

namespace Orion.Models {

    public class GraficoFecha : GraficoBase {


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


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public override void FromReader(SQLiteDataReader lector) {
            base.FromReader(lector);
            _fecha = lector.ToDateTime("Fecha");
            Nuevo = false;
            Modificado = false;
        }


        #endregion
        // ====================================================================================================

    }
}
