#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Data.Common;
using System.Data.OleDb;
using Orion.Models;

namespace Orion.Pijama {

    public class DiaPijama : DiaCalendarioBase {

        public enum Reclama {
            Nada = 0,
            EnContra = 1,
            AFavor = 2,
        }

        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public DiaPijama() : base() { }


        public DiaPijama(OleDbDataReader lector) : base(lector) { }


        public DiaPijama(DiaCalendarioBase dia) : base(dia) { }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private GraficoBase _graficotrabajado;
        public GraficoBase GraficoTrabajado {
            get { return _graficotrabajado; }
            set {
                if (_graficotrabajado != value) {
                    _graficotrabajado = value;
                    PropiedadCambiada();
                }
            }
        }


        private GraficoBase _graficooriginal;
        public GraficoBase GraficoOriginal {
            get { return _graficooriginal; }
            set { SetValue(ref _graficooriginal, value); }
        }



        private TimeSpan _trabajadasreales;
        public TimeSpan TrabajadasReales {
            get { return _trabajadasreales; }
            set {
                if (_trabajadasreales != value) {
                    _trabajadasreales = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _pluslimpieza;
        public decimal PlusLimpieza {
            get { return _pluslimpieza; }
            set {
                if (_pluslimpieza != value) {
                    _pluslimpieza = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _pluspaqueteria;
        public decimal PlusPaqueteria {
            get { return _pluspaqueteria; }
            set {
                if (_pluspaqueteria != value) {
                    _pluspaqueteria = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusmenordescanso;
        public decimal PlusMenorDescanso {
            get { return _plusmenordescanso; }
            set {
                if (_plusmenordescanso != value) {
                    _plusmenordescanso = value;
                    PropiedadCambiada();
                }
            }
        }


        private string _tiempomenordescanso;
        public string TiempoMenorDescanso {
            get { return _tiempomenordescanso; }
            set {
                if (_tiempomenordescanso != value) {
                    _tiempomenordescanso = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusnocturnidad;
        public decimal PlusNocturnidad {
            get { return _plusnocturnidad; }
            set {
                if (_plusnocturnidad != value) {
                    _plusnocturnidad = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _plusnavidad;
        public decimal PlusNavidad {
            get { return _plusnavidad; }
            set {
                if (_plusnavidad != value) {
                    _plusnavidad = value;
                    PropiedadCambiada();
                }
            }
        }


        public decimal OtrosPluses {
            get {
                return PlusNocturnidad + PlusNavidad;
            }
        }


        public decimal TotalDietas {
            get {
                return (GraficoTrabajado.Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) + GraficoTrabajado.Comida +
                        GraficoTrabajado.Cena + GraficoTrabajado.PlusCena;
            }
        }


        public string TextoInicioFinal {
            get {
                string texto = null;
                if (GraficoTrabajado != null && GraficoTrabajado.Numero > 0) {
                    if (GraficoTrabajado.Inicio.HasValue && GraficoTrabajado.Final.HasValue)
                        texto = String.Format($"{GraficoTrabajado.Inicio.Value.ToTexto()} - {GraficoTrabajado.Final.Value.ToTexto()}");
                    if (GraficoTrabajado.InicioPartido.HasValue && GraficoTrabajado.FinalPartido.HasValue)
                        texto += string.Format($"    ({GraficoTrabajado.InicioPartido.Value.ToTexto()} - {GraficoTrabajado.FinalPartido.Value.ToTexto()})");
                }
                return texto;
            }
        }


        private bool _esfestivo;
        public bool EsFestivo {
            get { return _esfestivo; }
            set {
                if (_esfestivo != value) {
                    _esfestivo = value;
                    PropiedadCambiada();
                }
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES DE DIETAS
        // ====================================================================================================


        public decimal ImporteDesayuno {
            get {
                return (GraficoTrabajado.Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) * App.Global.OpcionesVM.GetPluses(DiaFecha.Year).ImporteDietas;
            }
        }

        public decimal ImporteComida {
            get {
                return GraficoTrabajado.Comida * App.Global.OpcionesVM.GetPluses(DiaFecha.Year).ImporteDietas;
            }
        }

        public decimal ImporteCena {
            get {
                return GraficoTrabajado.Cena * App.Global.OpcionesVM.GetPluses(DiaFecha.Year).ImporteDietas;
            }
        }

        public decimal ImportePlusCena {
            get {
                return GraficoTrabajado.PlusCena * App.Global.OpcionesVM.GetPluses(DiaFecha.Year).ImporteDietas;
            }
        }

        public decimal ImporteTotalDietas {
            get {
                return TotalDietas * App.Global.OpcionesVM.GetPluses(DiaFecha.Year).ImporteDietas;
            }
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES GRÁFICO ALTERNATIVO
        // ====================================================================================================

        public bool TieneHorarioAlternativo {
            get {
                if (GraficoAlt.HasValue) return true;
                if (TurnoAlt.HasValue || InicioAlt.HasValue || FinalAlt.HasValue) return true;
                if (InicioPartidoAlt.HasValue || FinalPartidoAlt.HasValue) return true;
                return false;
            }
        }

        public Reclama ReclamaTrabajadas {
            get {
                if (TrabajadasAlt.HasValue) {
                    if (TrabajadasAlt.Value < GraficoOriginal.Trabajadas) {
                        return Reclama.EnContra;
                    }
                    if (TrabajadasAlt.Value > GraficoOriginal.Trabajadas) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaAcumuladas {
            get {
                if (AcumuladasAlt.HasValue) {
                    if (AcumuladasAlt.Value < GraficoOriginal.Acumuladas) {
                        return Reclama.EnContra;
                    }
                    if (AcumuladasAlt.Value > GraficoOriginal.Acumuladas) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaNocturnas {
            get {
                if (NocturnasAlt.HasValue) {
                    if (NocturnasAlt.Value < GraficoOriginal.Nocturnas) {
                        return Reclama.EnContra;
                    }
                    if (NocturnasAlt.Value > GraficoOriginal.Nocturnas) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaDesayuno {
            get {
                if (DesayunoAlt.HasValue) {
                    if (DesayunoAlt.Value < GraficoOriginal.Desayuno) {
                        return Reclama.EnContra;
                    }
                    if (DesayunoAlt.Value > GraficoOriginal.Desayuno) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaComida {
            get {
                if (ComidaAlt.HasValue) {
                    if (ComidaAlt.Value < GraficoOriginal.Comida) {
                        return Reclama.EnContra;
                    }
                    if (ComidaAlt.Value > GraficoOriginal.Comida) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaCena {
            get {
                if (CenaAlt.HasValue) {
                    if (CenaAlt.Value < GraficoOriginal.Cena) {
                        return Reclama.EnContra;
                    }
                    if (CenaAlt.Value > GraficoOriginal.Cena) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaPlusCena {
            get {
                if (PlusCenaAlt.HasValue) {
                    if (PlusCenaAlt.Value < GraficoOriginal.PlusCena) {
                        return Reclama.EnContra;
                    }
                    if (PlusCenaAlt.Value > GraficoOriginal.PlusCena) {
                        return Reclama.AFavor;
                    }
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaPaqueteria {
            get {
                if (PlusPaqueteriaAlt.HasValue && PlusPaqueteriaAlt.Value != GraficoOriginal.PlusPaqueteria) {
                    return PlusPaqueteriaAlt.Value == true ? Reclama.AFavor : Reclama.EnContra;
                }
                return Reclama.Nada;
            }
        }


        public Reclama ReclamaPlusLimpieza {
            get {
                if (PlusLimpiezaAlt.HasValue && PlusLimpiezaAlt.Value != GraficoOriginal.PlusLimpieza) {
                    return PlusLimpiezaAlt.Value == true ? Reclama.AFavor : Reclama.EnContra;
                }
                return Reclama.Nada;
            }
        }





        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================

        public override void FromReader(DbDataReader lector) {
            base.FromReader(lector);
            GraficoTrabajado = new GraficoBase();
            GraficoOriginal = new GraficoBase();
            GraficoTrabajado.FromReader(lector);
            if (GraficoTrabajado.Numero == 0) GraficoTrabajado.Numero = Grafico;
            GraficoOriginal.FromReader(lector);
            if (GraficoOriginal.Numero == 0) GraficoOriginal.Numero = Grafico;
            if (TurnoAlt.HasValue) GraficoTrabajado.Turno = TurnoAlt.Value;
            if (InicioAlt.HasValue) GraficoTrabajado.Inicio = new TimeSpan(InicioAlt.Value.Ticks);
            if (FinalAlt.HasValue) GraficoTrabajado.Final = new TimeSpan(FinalAlt.Value.Ticks);
            if (InicioPartidoAlt.HasValue) GraficoTrabajado.InicioPartido = new TimeSpan(InicioPartidoAlt.Value.Ticks);
            if (FinalPartidoAlt.HasValue) GraficoTrabajado.FinalPartido = new TimeSpan(FinalPartidoAlt.Value.Ticks);
            if (TrabajadasAlt.HasValue) GraficoTrabajado.Trabajadas = new TimeSpan(TrabajadasAlt.Value.Ticks);
            if (AcumuladasAlt.HasValue) GraficoTrabajado.Acumuladas = new TimeSpan(AcumuladasAlt.Value.Ticks);
            if (NocturnasAlt.HasValue) GraficoTrabajado.Nocturnas = new TimeSpan(NocturnasAlt.Value.Ticks);
            if (DesayunoAlt.HasValue) GraficoTrabajado.Desayuno = DesayunoAlt.Value;
            if (ComidaAlt.HasValue) GraficoTrabajado.Comida = ComidaAlt.Value;
            if (CenaAlt.HasValue) GraficoTrabajado.Cena = CenaAlt.Value;
            if (PlusCenaAlt.HasValue) GraficoTrabajado.PlusCena = PlusCenaAlt.Value;
            if (PlusLimpiezaAlt.HasValue) GraficoTrabajado.PlusLimpieza = PlusLimpiezaAlt.Value;
            if (PlusPaqueteriaAlt.HasValue) GraficoTrabajado.PlusPaqueteria = PlusPaqueteriaAlt.Value;
            Nuevo = false;
            Modificado = false;
        }




        #endregion
        // ====================================================================================================




    }
}
