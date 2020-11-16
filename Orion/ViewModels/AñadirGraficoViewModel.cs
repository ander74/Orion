﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Windows;
using Orion.Models;
using Orion.Servicios;

namespace Orion.ViewModels {
    public partial class AñadirGraficoViewModel : NotifyBase {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private IMensajes mensajes;

        #endregion


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================
        public AñadirGraficoViewModel(IMensajes servicioMensajes) {
            mensajes = servicioMensajes;
            IncrementarNumeroMarcado = true;
            DeducirTurnoMarcado = true;
            DeducirDiaSemanaMarcado = true;
        }
        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private Grafico _graficoActual;
        public Grafico GraficoActual {
            get {
                _graficoActual = new Grafico {
                    Numero = this.Numero,
                    DiaSemana = this.DiaSemana,
                    Turno = this.Turno,
                    Valoracion = this.Valoracion,
                    Inicio = this.Inicio,
                    Final = this.Final,
                    InicioPartido = this.InicioPartido,
                    FinalPartido = this.FinalPartido,
                    TiempoVacio = this.TiempoVacio,
                };
                return _graficoActual;
            }
        }


        private int _numero;
        public int Numero {
            get { return _numero; }
            set {
                if (_numero != value) {
                    _numero = value;
                    PropiedadCambiada();
                    if (DeducirTurnoMarcado) { Turno = (value % 2 == 0) ? 2 : 1; }
                    DiaSemana = "";
                    if (DeducirDiaSemanaMarcado) {
                        if (App.Global.PorCentro.RangoLun.Validar(_numero)) DiaSemana = "L";
                        if (App.Global.PorCentro.RangoVie.Validar(_numero)) DiaSemana = "V";
                        if (App.Global.PorCentro.RangoSab.Validar(_numero)) DiaSemana = "S";
                        if (App.Global.PorCentro.RangoDom.Validar(_numero)) DiaSemana = "F";
                        if (App.Global.PorCentro.RangoRes.Validar(_numero)) DiaSemana = "R";
                    }
                }
            }
        }


        private int _turno = 1;
        public int Turno {
            get { return _turno; }
            set {
                if (_turno != value) {
                    _turno = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(VerPartido));
                }
            }
        }


        private string _diasemana;
        public string DiaSemana {
            get { return _diasemana; }
            set { SetValue(ref _diasemana, value); }
        }


        private TimeSpan _valoracion;
        public TimeSpan Valoracion {
            get { return _valoracion; }
            set {
                if (_valoracion != value) {
                    _valoracion = value;
                    Final = _inicio + _valoracion;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? _inicio;
        public TimeSpan? Inicio {
            get { return _inicio; }
            set {
                if (_inicio != value) {
                    _inicio = value;
                    Final = _inicio + _valoracion;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? _final;
        public TimeSpan? Final {
            get { return _final; }
            set {
                if (_final != value) {
                    _final = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan tiempoVacio;
        public TimeSpan TiempoVacio {
            get => tiempoVacio;
            set => SetValue(ref tiempoVacio, value);
        }


        private TimeSpan? _inicioPartido;
        public TimeSpan? InicioPartido {
            get { return _inicioPartido; }
            set {
                if (_inicioPartido != value) {
                    _inicioPartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? _finalPartido;
        public TimeSpan? FinalPartido {
            get { return _finalPartido; }
            set {
                if (_finalPartido != value) {
                    _finalPartido = value;
                    PropiedadCambiada();
                }
            }
        }


        public Visibility VerPartido {
            get { return (GraficoActual.Turno == 4) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IncrementarNumeroMarcado { get; set; }

        public bool DeducirTurnoMarcado { get; set; }

        public bool DeducirDiaSemanaMarcado { get; set; }

        public bool ResetTiempoVacioMarcado { get; set; }


        #endregion

    }
}
