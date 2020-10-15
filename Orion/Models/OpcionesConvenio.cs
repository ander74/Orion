#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.IO;
using Newtonsoft.Json;

namespace Orion.Models {

    public class OpcionesConvenio : NotifyBase {


        // ====================================================================================================
        #region GENERALES
        // ====================================================================================================

        private TimeSpan _jornadamedia = new TimeSpan(7, 20, 0);
        public TimeSpan JornadaMedia {
            get { return _jornadamedia; }
            set {
                if (_jornadamedia != value) {
                    _jornadamedia = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _importedietas = 15.14m;
        public decimal ImporteDietas {
            get { return _importedietas; }
            set {
                if (_importedietas != value) {
                    _importedietas = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _maxhorascobradas = 40;
        public int MaxHorasCobradas {
            get { return _maxhorascobradas; }
            set {
                if (_maxhorascobradas != value) {
                    _maxhorascobradas = value;
                    PropiedadCambiada();
                }
            }
        }



        private int horasanuales = 1650;
        public int HorasAnuales {
            get { return horasanuales; }
            set {
                if (horasanuales != value) {
                    horasanuales = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _trabajoanuales = 225;
        public int TrabajoAnuales {
            get { return _trabajoanuales; }
            set {
                if (_trabajoanuales != value) {
                    _trabajoanuales = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _descansosanuales = 109;
        public int DescansosAnuales {
            get { return _descansosanuales; }
            set {
                if (_descansosanuales != value) {
                    _descansosanuales = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _vacacionesanuales = 31;
        public int VacacionesAnuales {
            get { return _vacacionesanuales; }
            set {
                if (_vacacionesanuales != value) {
                    _vacacionesanuales = value;
                    PropiedadCambiada();
                }
            }
        }


        private int findescompletosanuales = 21;
        public int FindesCompletosAnuales {
            get { return findescompletosanuales; }
            set {
                if (findescompletosanuales != value) {
                    findescompletosanuales = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _maxhoraslaborables = new TimeSpan(8, 30, 0);
        public TimeSpan MaxHorasLaborables {
            get { return _maxhoraslaborables; }
            set {
                if (_maxhoraslaborables != value) {
                    _maxhoraslaborables = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _maxhorasfinessemana = new TimeSpan(9, 0, 0);
        public TimeSpan MaxHorasFinesSemana {
            get { return _maxhorasfinessemana; }
            set {
                if (_maxhorasfinessemana != value) {
                    _maxhorasfinessemana = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _maxhorastotalgraficopartido = new TimeSpan(10, 0, 0);
        public TimeSpan MaxHorasTotalGraficoPartido {
            get { return _maxhorastotalgraficopartido; }
            set {
                if (_maxhorastotalgraficopartido != value) {
                    _maxhorastotalgraficopartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _maxhorasparticiongraficopartido = new TimeSpan(3, 0, 0);
        public TimeSpan MaxHorasParticionGraficoPartido {
            get { return _maxhorasparticiongraficopartido; }
            set {
                if (_maxhorasparticiongraficopartido != value) {
                    _maxhorasparticiongraficopartido = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region HORAS
        // ====================================================================================================

        private TimeSpan _inicionocturnas = new TimeSpan(22, 0, 0);
        public TimeSpan InicioNocturnas {
            get { return _inicionocturnas; }
            set {
                if (_inicionocturnas != value) {
                    _inicionocturnas = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _finalnocturnas = new TimeSpan(6, 0, 0);
        public TimeSpan FinalNocturnas {
            get { return _finalnocturnas; }
            set {
                if (_finalnocturnas != value) {
                    _finalnocturnas = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _horadesayuno = new TimeSpan(6, 0, 0);
        public TimeSpan HoraDesayuno {
            get { return _horadesayuno; }
            set {
                if (_horadesayuno != value) {
                    _horadesayuno = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _iniciocomidacontinuo = new TimeSpan(12, 30, 0);
        public TimeSpan InicioComidaContinuo {
            get { return _iniciocomidacontinuo; }
            set {
                if (_iniciocomidacontinuo != value) {
                    _iniciocomidacontinuo = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _finalcomidacontinuo = new TimeSpan(15, 30, 0);
        public TimeSpan FinalComidaContinuo {
            get { return _finalcomidacontinuo; }
            set {
                if (_finalcomidacontinuo != value) {
                    _finalcomidacontinuo = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _iniciocomidapartido = new TimeSpan(11, 45, 0);
        public TimeSpan InicioComidaPartido {
            get { return _iniciocomidapartido; }
            set {
                if (_iniciocomidapartido != value) {
                    _iniciocomidapartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _finalcomidapartido = new TimeSpan(15, 15, 0);
        public TimeSpan FinalComidaPartido {
            get { return _finalcomidapartido; }
            set {
                if (_finalcomidapartido != value) {
                    _finalcomidapartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _iniciocena = new TimeSpan(22, 0, 0);
        public TimeSpan InicioCena {
            get { return _iniciocena; }
            set {
                if (_iniciocena != value) {
                    _iniciocena = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _finalcena = new TimeSpan(23, 0, 0);
        public TimeSpan FinalCena {
            get { return _finalcena; }
            set {
                if (_finalcena != value) {
                    _finalcena = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _iniciopluscena = new TimeSpan(0, 0, 0);
        public TimeSpan InicioPlusCena {
            get { return _iniciopluscena; }
            set {
                if (_iniciopluscena != value) {
                    _iniciopluscena = value;
                    PropiedadCambiada();
                }
            }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PORCENTAJES
        // ====================================================================================================

        private int _porcentajedesayuno = 80;
        public int PorcentajeDesayuno {
            get { return _porcentajedesayuno; }
            set {
                if (_porcentajedesayuno != value) {
                    _porcentajedesayuno = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _porcentajepluscena = 25;
        public int PorcentajePlusCena {
            get { return _porcentajepluscena; }
            set {
                if (_porcentajepluscena != value) {
                    _porcentajepluscena = value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PLUSES
        // ====================================================================================================

        private decimal _importefestivos = 41.78m;
        public decimal ImporteFestivos {
            get { return _importefestivos; }
            set {
                if (_importefestivos != value) {
                    _importefestivos = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _importesabados = 10.45m;
        public decimal ImporteSabados {
            get { return _importesabados; }
            set {
                if (_importesabados != value) {
                    _importesabados = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _dietamenordescanso = 8.68m;
        public decimal DietaMenorDescanso {
            get { return _dietamenordescanso; }
            set {
                if (_dietamenordescanso != value) {
                    _dietamenordescanso = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _pluslimpieza = 9.83m;
        public decimal PlusLimpieza {
            get { return _pluslimpieza; }
            set {
                if (_pluslimpieza != value) {
                    _pluslimpieza = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _pluspaqueteria = 7.75m;
        public decimal PlusPaqueteria {
            get { return _pluspaqueteria; }
            set {
                if (_pluspaqueteria != value) {
                    _pluspaqueteria = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusnocturnidad = 11m;
        public decimal PlusNocturnidad {
            get { return _plusnocturnidad; }
            set {
                if (_plusnocturnidad != value) {
                    _plusnocturnidad = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusnavidad = 160.63m;
        public decimal PlusNavidad {
            get { return _plusnavidad; }
            set {
                if (_plusnavidad != value) {
                    _plusnavidad = value;
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
