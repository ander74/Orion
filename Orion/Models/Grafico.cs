#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;

namespace Orion.Models {

    public class Grafico : GraficoBase {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public Grafico() : base() {
            PropertyChanged += LlamarPropiedadCambiada;
            _valoraciones.CollectionChanged += ListaValoraciones_CollectionChanged;
        }


        public Grafico(OleDbDataReader lector) : base(lector) {
            PropertyChanged += LlamarPropiedadCambiada;
            _valoraciones.CollectionChanged += ListaValoraciones_CollectionChanged;
        }
        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void BorrarValorPorHeader(string header) {
            switch (header) {
                case "#": NoCalcular = false; break;
                case "Número": Numero = 0; break;
                case "D": DiaSemana = ""; break;
                case "Turno": Turno = 1; break;
                case "Inicio": Inicio = null; break;
                case "Final": Final = null; break;
                case "Inicio Partido": InicioPartido = null; break;
                case "Final Partido": FinalPartido = null; break;
                case "Valoración": Valoracion = TimeSpan.Zero; break;
                case "Trabajadas": Trabajadas = TimeSpan.Zero; break;
                case "Acumuladas": Acumuladas = TimeSpan.Zero; break;
                case "Nocturnas": Nocturnas = TimeSpan.Zero; break;
                case "Desayuno": Desayuno = 0; break;
                case "Comida": Comida = 0; break;
                case "Cena": Cena = 0; break;
                case "Plus Cena": PlusCena = 0; break;
                case "Limpieza": PlusLimpieza = false; break;
                case "Paquetería": PlusPaqueteria = false; break;
            }
        }


        public TimeSpan? HoraFinalValoraciones() {
            TimeSpan? hora = ListaValoraciones.Max(p => p.Final);
            return hora;
        }


        public void NotificarCambioValoraciones() {
            PropiedadCambiada(nameof(ListaValoraciones));
            PropiedadCambiada(nameof(TiempoValoraciones));
        }
        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================
        private void ListaValoraciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (ValoracionGrafico valoracion in e.NewItems) {
                    valoracion.IdGrafico = this.Id;
                    valoracion.Nuevo = true;
                    valoracion.ObjetoCambiado += ObjetoCambiadoEventHandler;
                }
                Modificado = true;
            }


            if (e.OldItems != null) {
                foreach (ValoracionGrafico valoracion in e.OldItems) {
                    valoracion.ObjetoCambiado -= ObjetoCambiadoEventHandler;
                }
                Modificado = true;
            }


            PropiedadCambiada(nameof(ListaValoraciones));
            PropiedadCambiada(nameof(TiempoValoraciones));
        }


        private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
            Modificado = true;
        }


        private void LlamarPropiedadCambiada(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Valoracion):
                case nameof(Trabajadas):
                    PropiedadCambiada(nameof(ValoracionDiferente));
                    PropiedadCambiada(nameof(DiferenciaValoracion));
                    break;
                case nameof(TiempoPartido):
                case nameof(TrabajadasPartido):
                    PropiedadCambiada(nameof(ErrorGraficoPartido));
                    break;
            }
        }

        #endregion


        // ====================================================================================================
        #region  PROPIEDADES
        // ====================================================================================================

        private ObservableCollection<ValoracionGrafico> _valoraciones = new ObservableCollection<ValoracionGrafico>();
        public ObservableCollection<ValoracionGrafico> ListaValoraciones {
            get { return _valoraciones; }
            set {
                if (_valoraciones != value) {
                    _valoraciones = value;
                    _valoraciones.CollectionChanged += ListaValoraciones_CollectionChanged;
                    foreach (ValoracionGrafico v in _valoraciones) {
                        v.ObjetoCambiado += ObjetoCambiadoEventHandler;
                    }
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private bool _diferente;
        public bool Diferente {
            get { return _diferente; }
            set {
                if (_diferente != value) {
                    _diferente = value;
                    PropiedadCambiada();
                }
            }
        }


        public bool ValoracionDiferente {
            get {
                if (Valoracion == App.Global.Convenio.JornadaMedia && Trabajadas <= Valoracion) return false;
                return Valoracion != Trabajadas;
            }
        }


        public bool ErrorGraficoPartido {
            get {
                if (Turno == 4) {
                    if (TiempoPartido < App.Global.Convenio.MaxHorasParticionGraficoPartido) return true;
                    if (TrabajadasPartido > App.Global.Convenio.MaxHorasTotalGraficoPartido) return true;
                }
                return false;
            }
        }






        public TimeSpan DiferenciaValoracion {
            get {
                TimeSpan diferencia = TimeSpan.Zero;
                if (ValoracionDiferente) {
                    diferencia = Valoracion - Trabajadas;
                }
                return diferencia.Ticks < 0 ? new TimeSpan(diferencia.Ticks * -1) : diferencia;
            }
        }


        public TimeSpan? TiempoValoraciones {
            get {
                if (_valoraciones == null) return null;
                TimeSpan? inicio = _valoraciones.Min(v => v.Inicio);
                TimeSpan? final = _valoraciones.Max(v => v.Final);
                if (inicio.HasValue && final.HasValue) return final - inicio;
                return null;
            }
        }
        #endregion



    } //Final de la clase.
}
