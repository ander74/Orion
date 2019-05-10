#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    using System;
    using System.Data.OleDb;

    public class DiaCalendarioBase : NotifyBase {


        // ====================================================================================================
        #region  CONSTRUCTORES
        // ====================================================================================================
        public DiaCalendarioBase() {
            PlusLimpiezaAlt = null;
            PlusPaqueteriaAlt = null;
        }


        public DiaCalendarioBase(OleDbDataReader lector) {
            PlusLimpiezaAlt = null;
            PlusPaqueteriaAlt = null;
            ParseFromReader(lector, this);
        }


        public DiaCalendarioBase(DiaCalendarioBase dia) {
            _idcalendario = dia.IdCalendario;
            _dia = dia.Dia;
            _diafecha = dia.DiaFecha;
            _grafico = dia.Grafico;
            _codigo = dia.Codigo;
            _excesojornada = dia.ExcesoJornada;
            _facturadopaqueteria = dia.FacturadoPaqueteria;
            _limpieza = dia.Limpieza;
            _graficovinculado = dia.GraficoVinculado;
            _notas = dia.Notas;
            turnoalt = dia.TurnoAlt;
            inicioalt = dia.InicioAlt;
            finalalt = dia.FinalAlt;
            iniciopartidoalt = dia.InicioPartidoAlt;
            finalpartidoalt = dia.FinalPartidoAlt;
            trabajadasalt = dia.TrabajadasAlt;
            acumuladasalt = dia.AcumuladasAlt;
            nocturnasalt = dia.NocturnasAlt;
            desayunoalt = dia.DesayunoAlt;
            comidaalt = dia.ComidaAlt;
            cenaalt = dia.CenaAlt;
            pluscenaalt = dia.PlusCenaAlt;
            pluslimpiezaalt = dia.PlusLimpiezaAlt;
            pluspaqueteriaalt = dia.PlusPaqueteriaAlt;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, DiaCalendarioBase diacalendario) {
            diacalendario.Id = lector.ToInt32("Id");
            diacalendario.IdCalendario = lector.ToInt32("IdCalendario");
            diacalendario.Dia = lector.ToInt16("Dia");
            diacalendario.DiaFecha = lector.ToDateTime("DiaFecha");
            diacalendario.Grafico = lector.ToInt16("Grafico");
            diacalendario.Codigo = lector.ToInt16("Codigo");
            diacalendario.ExcesoJornada = lector.ToTimeSpan("ExcesoJornada");
            diacalendario.FacturadoPaqueteria = lector.ToDecimal("FacturadoPaqueteria");
            diacalendario.Limpieza = lector["Limpieza"] is DBNull ? null : (bool?)lector["Limpieza"];
            diacalendario.GraficoVinculado = lector.ToInt16("GraficoVinculado");
            diacalendario.Notas = lector.ToString("Notas");
            diacalendario.TurnoAlt = lector.ToInt16Nulable("TurnoAlt");
            diacalendario.InicioAlt = lector.ToTimeSpanNulable("InicioAlt");
            diacalendario.FinalAlt = lector.ToTimeSpanNulable("FinalAlt");
            diacalendario.InicioPartidoAlt = lector.ToTimeSpanNulable("InicioPartidoAlt");
            diacalendario.FinalPartidoAlt = lector.ToTimeSpanNulable("FinalPartidoAlt");
            diacalendario.TrabajadasAlt = lector.ToTimeSpanNulable("TrabajadasAlt");
            diacalendario.AcumuladasAlt = lector.ToTimeSpanNulable("AcumuladasAlt");
            diacalendario.NocturnasAlt = lector.ToTimeSpanNulable("NocturnasAlt");
            diacalendario.DesayunoAlt = lector.ToDecimalNulable("DesayunoAlt");
            diacalendario.ComidaAlt = lector.ToDecimalNulable("ComidaAlt");
            diacalendario.CenaAlt = lector.ToDecimalNulable("CenaAlt");
            diacalendario.PlusCenaAlt = lector.ToDecimalNulable("PlusCenaAlt");
            diacalendario.PlusLimpiezaAlt = lector.ToBoolNulable("PlusLimpiezaAlt");
            diacalendario.PlusPaqueteriaAlt = lector.ToBoolNulable("PlusPaqueteriaAlt");
        }


        public static void ParseToCommand(OleDbCommand Comando, DiaCalendarioBase diacalendario) {
            Comando.Parameters.AddWithValue("@IdCalendario", diacalendario.IdCalendario);
            Comando.Parameters.AddWithValue("@Dia", diacalendario.Dia);
            Comando.Parameters.AddWithValue("@DiaFecha", diacalendario.DiaFecha.ToString("yyyy-MM-dd"));
            Comando.Parameters.AddWithValue("@Grafico", diacalendario.Grafico);
            Comando.Parameters.AddWithValue("@Codigo", diacalendario.Codigo);
            Comando.Parameters.AddWithValue("@ExcesoJornada", diacalendario.ExcesoJornada.Ticks);
            Comando.Parameters.AddWithValue("@FacturadoPaqueteria", diacalendario.FacturadoPaqueteria.ToString("0.0000"));
            Comando.Parameters.AddWithValue("@Limpieza", diacalendario.Limpieza == null ? (object)DBNull.Value : diacalendario.Limpieza);
            Comando.Parameters.AddWithValue("@GraficoVinculado", diacalendario.GraficoVinculado);
            Comando.Parameters.AddWithValue("@Notas", diacalendario.Notas.TrimEnd(new char[] { ' ', '\n', '\r', '\t' }));
            Comando.Parameters.AddWithValue("TurnoAlt", diacalendario.TurnoAlt.HasValue ? diacalendario.TurnoAlt : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("InicioAlt", diacalendario.InicioAlt.HasValue ? diacalendario.InicioAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("FinalAlt", diacalendario.FinalAlt.HasValue ? diacalendario.FinalAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("InicioPartidoAlt", diacalendario.InicioPartidoAlt.HasValue ? diacalendario.InicioPartidoAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("FinalPartidoAlt", diacalendario.FinalPartidoAlt.HasValue ? diacalendario.FinalPartidoAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("TrabajadasAlt", diacalendario.TrabajadasAlt.HasValue ? diacalendario.TrabajadasAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("AcumuladasAlt", diacalendario.AcumuladasAlt.HasValue ? diacalendario.AcumuladasAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("NocturnasAlt", diacalendario.NocturnasAlt.HasValue ? diacalendario.NocturnasAlt.Value.Ticks : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("DesayunoAlt", diacalendario.DesayunoAlt.HasValue ? diacalendario.DesayunoAlt.Value.ToString("0.0000") : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("ComidaAlt", diacalendario.ComidaAlt.HasValue ? diacalendario.ComidaAlt.Value.ToString("0.0000") : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("CenaAlt", diacalendario.CenaAlt.HasValue ? diacalendario.CenaAlt.Value.ToString("0.0000") : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("PluscenaAlt", diacalendario.PlusCenaAlt.HasValue ? diacalendario.PlusCenaAlt.Value.ToString("0.0000") : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("PluslimpiezaAlt", diacalendario.PlusLimpiezaAlt.HasValue ? diacalendario.PlusLimpiezaAlt.Value : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("PluspaqueteriaAlt", diacalendario.PlusPaqueteriaAlt.HasValue ? diacalendario.PlusPaqueteriaAlt.Value : (object)DBNull.Value);
            Comando.Parameters.AddWithValue("@Id", diacalendario.Id);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private int _id;
        public int Id {
            get { return _id; }
            set {
                _id = value;
                Modificado = true;
                PropiedadCambiada();
            }
        }


        private int _idcalendario;
        public int IdCalendario {
            get { return _idcalendario; }
            set {
                if (_idcalendario != value) {
                    _idcalendario = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private int _dia;
        public int Dia {
            get { return _dia; }
            set {
                if (_dia != value) {
                    _dia = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private DateTime _diafecha;
        public DateTime DiaFecha {
            get { return _diafecha; }
            set {
                if (_diafecha != value) {
                    _diafecha = value;
                    PropiedadCambiada();
                }
            }
        }


        private int _grafico = 0;
        public int Grafico {
            get { return _grafico; }
            set {
                if (_grafico != value) {
                    if (value == -4 && _grafico == -2) {
                        _grafico = -10;
                    } else if (value == -4 && _grafico == -3) {
                        _grafico = -11;
                    } else if (value == -1 && _grafico == -2) {
                        _grafico = -12;
                    } else if (value == -1 && _grafico == -3) {
                        _grafico = -13;
                    } else {
                        _grafico = value;
                    }
                    Modificado = true;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(ComboGrafico));
                }
            }
        }


        private int _codigo;
        public int Codigo {
            get { return _codigo; }
            set {
                if (_codigo != value) {
                    _codigo = value;
                    Modificado = true;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(ComboGrafico));
                }
            }
        }


        public Tuple<int, int> ComboGrafico {
            get { return new Tuple<int, int>(Grafico, Codigo); }
            set {
                if (value.Item1 == 0 && value.Item2 == 0) {
                    Grafico = 0;
                    Codigo = 0;
                } else {
                    if (Grafico != value.Item1 && value.Item1 != 0) {
                        Grafico = value.Item1;
                    }
                    if (Codigo != value.Item2) {
                        Codigo = value.Item2;
                    }
                }
            }
        }


        private TimeSpan _excesojornada;
        public TimeSpan ExcesoJornada {
            get { return _excesojornada; }
            set {
                if (_excesojornada != value) {
                    _excesojornada = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _facturadopaqueteria;
        public decimal FacturadoPaqueteria {
            get { return _facturadopaqueteria; }
            set {
                if (_facturadopaqueteria != value) {
                    _facturadopaqueteria = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private bool? _limpieza = false;
        public bool? Limpieza {
            get { return _limpieza; }
            set {
                if (_limpieza != value) {
                    _limpieza = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private int _graficovinculado;
        public int GraficoVinculado {
            get { return _graficovinculado; }
            set {
                if (_graficovinculado != value) {
                    _graficovinculado = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private string _notas = "";
        public string Notas {
            get { return _notas; }
            set {
                if (_notas != value) {
                    _notas = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }

        #endregion
        // ====================================================================================================

        // ====================================================================================================
        #region PROPIEDADES ALTERNATIVO
        // ====================================================================================================

        private int? turnoalt = null;
        public int? TurnoAlt {
            get { return turnoalt; }
            set {
                if (value != turnoalt) {
                    turnoalt = (value < 1 || value > 4) ? null : value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? inicioalt = null;
        public TimeSpan? InicioAlt {
            get { return inicioalt; }
            set {
                if (!value.Equals(inicioalt)) {
                    inicioalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? finalalt = null;
        public TimeSpan? FinalAlt {
            get { return finalalt; }
            set {
                if (!value.Equals(finalalt)) {
                    finalalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? iniciopartidoalt = null;
        public TimeSpan? InicioPartidoAlt {
            get { return iniciopartidoalt; }
            set {
                if (!value.Equals(iniciopartidoalt)) {
                    iniciopartidoalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? finalpartidoalt = null;
        public TimeSpan? FinalPartidoAlt {
            get { return finalpartidoalt; }
            set {
                if (!value.Equals(finalpartidoalt)) {
                    finalpartidoalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? trabajadasalt = null;
        public TimeSpan? TrabajadasAlt {
            get { return trabajadasalt; }
            set {
                if (!value.Equals(trabajadasalt)) {
                    trabajadasalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? acumuladasalt = null;
        public TimeSpan? AcumuladasAlt {
            get { return acumuladasalt; }
            set {
                if (!value.Equals(acumuladasalt)) {
                    acumuladasalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan? nocturnasalt = null;
        public TimeSpan? NocturnasAlt {
            get { return nocturnasalt; }
            set {
                if (!value.Equals(nocturnasalt)) {
                    nocturnasalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal? desayunoalt = null;
        public decimal? DesayunoAlt {
            get { return desayunoalt; }
            set {
                if (value != desayunoalt) {
                    desayunoalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal? comidaalt = null;
        public decimal? ComidaAlt {
            get { return comidaalt; }
            set {
                if (value != comidaalt) {
                    comidaalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal? cenaalt = null;
        public decimal? CenaAlt {
            get { return cenaalt; }
            set {
                if (value != cenaalt) {
                    cenaalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal? pluscenaalt = null;
        public decimal? PlusCenaAlt {
            get { return pluscenaalt; }
            set {
                if (value != pluscenaalt) {
                    pluscenaalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private bool? pluslimpiezaalt = null;
        public bool? PlusLimpiezaAlt {
            get { return pluslimpiezaalt; }
            set {
                if (pluslimpiezaalt != value) {
                    pluslimpiezaalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private bool? pluspaqueteriaalt = null;
        public bool? PlusPaqueteriaAlt {
            get { return pluspaqueteriaalt; }
            set {
                if (pluspaqueteriaalt != value) {
                    pluspaqueteriaalt = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }

        #endregion
        // ====================================================================================================

    }

}
