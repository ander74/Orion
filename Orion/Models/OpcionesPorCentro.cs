#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.IO;
using Newtonsoft.Json;
using Orion.Config;

namespace Orion.Models {

    public class OpcionesPorCentro : NotifyBase {


        // ====================================================================================================
        #region GRÁFICOS
        // ====================================================================================================

        private int _comodin = 999;
        public int Comodin {
            get { return _comodin; }
            set {
                if (_comodin != value) {
                    _comodin = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region NUEVOS GRAFICOS
        // ====================================================================================================



        private string lun = "3000-3499";
        public string Lun {
            get => lun;
            set => SetValue(ref lun, value);
        }


        private string vie = "3500-3599";
        public string Vie {
            get => vie;
            set => SetValue(ref vie, value);
        }


        private string sab = "3600-3699";
        public string Sab {
            get => sab;
            set => SetValue(ref sab, value);
        }


        private string dom = "3700-3799";
        public string Dom {
            get => dom;
            set => SetValue(ref dom, value);
        }


        private string res = "3900-3999";
        public string Res {
            get => res;
            set => SetValue(ref res, value);
        }


        [JsonIgnore]
        public IntRangoCollection RangoLun { get => new IntRangoCollection(Lun); }

        [JsonIgnore]
        public IntRangoCollection RangoVie { get => new IntRangoCollection(Vie); }

        [JsonIgnore]
        public IntRangoCollection RangoSab { get => new IntRangoCollection(Sab); }

        [JsonIgnore]
        public IntRangoCollection RangoDom { get => new IntRangoCollection(Dom); }

        [JsonIgnore]
        public IntRangoCollection RangoRes { get => new IntRangoCollection(Res); }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region LIMPIEZAS
        // ====================================================================================================

        private bool _pagarlimpiezas = false;
        public bool PagarLimpiezas {
            get { return _pagarlimpiezas; }
            set {
                if (_pagarlimpiezas != value) {
                    _pagarlimpiezas = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _numerolimpiezas = 21;
        public int NumeroLimpiezas {
            get { return _numerolimpiezas; }
            set {
                if (_numerolimpiezas != value) {
                    _numerolimpiezas = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PLUS VIAJE
        // ====================================================================================================

        private bool _pagarplusviaje = false;
        public bool PagarPlusViaje {
            get { return _pagarplusviaje; }
            set {
                if (_pagarplusviaje != value) {
                    _pagarplusviaje = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusviaje = 0;
        public decimal PlusViaje {
            get { return _plusviaje; }
            set {
                if (_plusviaje != value) {
                    _plusviaje = value;
                    PropiedadCambiada();
                }
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        public void Guardar(string ruta) {
            string datos = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ruta, datos, System.Text.Encoding.UTF8);
        }

        public void Cargar(string ruta) {
            if (File.Exists(ruta)) {
                string datos = File.ReadAllText(ruta, System.Text.Encoding.UTF8);
                JsonConvert.PopulateObject(datos, this);
                PropiedadCambiada("");
            }
        }

        #endregion
        // ====================================================================================================





    }
}
