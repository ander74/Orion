#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using Newtonsoft.Json;
    using Orion.Interfaces;

    public class DiaCalendarioBase : NotifyBase, ISQLiteItem {


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


        // PROPIEDADES AÑADIDAS POSTERIORMENTE

        private string categoriaGrafico = "C";
        public string CategoriaGrafico {
            get => categoriaGrafico;
            set => SetValue(ref categoriaGrafico, value);
        }


        private int? turno;
        public int? Turno {
            get => turno;
            set => SetValue(ref turno, value);
        }


        private TimeSpan? inicio;
        public TimeSpan? Inicio {
            get => inicio;
            set => SetValue(ref inicio, value);
        }


        private TimeSpan? final;
        public TimeSpan? Final {
            get => final;
            set => SetValue(ref final, value);
        }


        private TimeSpan? inicioPartido;
        public TimeSpan? InicioPartido {
            get => inicioPartido;
            set => SetValue(ref inicioPartido, value);
        }


        private TimeSpan? finalPartido;
        public TimeSpan? FinalPartido {
            get => finalPartido;
            set => SetValue(ref finalPartido, value);
        }


        private TimeSpan trabajadasPartido;
        public TimeSpan TrabajadasPartido {
            get => trabajadasPartido;
            set {
                if (trabajadasPartido != value) {
                    trabajadasPartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan tiempopartido;
        public TimeSpan TiempoPartido {
            get { return tiempopartido; }
            set {
                if (tiempopartido != value) {
                    tiempopartido = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan valoracion;
        public TimeSpan Valoracion {
            get { return valoracion; }
            set {
                if (!value.Equals(valoracion)) {
                    valoracion = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan trabajadas;
        public TimeSpan Trabajadas {
            get => trabajadas;
            set => SetValue(ref trabajadas, value);
        }


        private TimeSpan trabajadasConvenio;
        public TimeSpan TrabajadasConvenio {
            get => trabajadasConvenio;
            set => SetValue(ref trabajadasConvenio, value);
        }


        private TimeSpan trabajadasReales;
        public TimeSpan TrabajadasReales {
            get => trabajadasReales;
            set => SetValue(ref trabajadasReales, value);
        }


        private TimeSpan tiempoVacio;
        public TimeSpan TiempoVacio {
            get => tiempoVacio;
            set => SetValue(ref tiempoVacio, value);
        }


        private TimeSpan acumuladas;
        public TimeSpan Acumuladas {
            get => acumuladas;
            set => SetValue(ref acumuladas, value);
        }


        private TimeSpan nocturnas;
        public TimeSpan Nocturnas {
            get => nocturnas;
            set => SetValue(ref nocturnas, value);
        }


        private decimal desayuno;
        public decimal Desayuno {
            get => desayuno;
            set => SetValue(ref desayuno, value);
        }


        private decimal comida;
        public decimal Comida {
            get => comida;
            set => SetValue(ref comida, value);
        }


        private decimal cena;
        public decimal Cena {
            get => cena;
            set => SetValue(ref cena, value);
        }


        private decimal plusCena;
        public decimal PlusCena {
            get => plusCena;
            set => SetValue(ref plusCena, value);
        }


        private bool plusLimpieza;
        public bool PlusLimpieza {
            get => plusLimpieza;
            set => SetValue(ref plusLimpieza, value);
        }


        private bool plusPaqueteria;
        public bool PlusPaqueteria {
            get => plusPaqueteria;
            set => SetValue(ref plusPaqueteria, value);
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


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public virtual void FromReader(DbDataReader lector) {
            _id = lector.ToInt32("_id");
            _idcalendario = lector.ToInt32("IdCalendario");
            _dia = lector.ToInt16("Dia");
            _diafecha = lector.ToDateTime("DiaFecha");
            _grafico = lector.ToInt16("Grafico");
            _codigo = lector.ToInt16("Codigo");
            _excesojornada = lector.ToTimeSpan("ExcesoJornada");
            _facturadopaqueteria = lector.ToDecimal("FacturadoPaqueteria");
            _limpieza = lector.ToBoolNulable("Limpieza");
            _graficovinculado = lector.ToInt16("GraficoVinculado");
            _notas = lector.ToString("Notas");
            turnoalt = lector.ToInt16Nulable("TurnoAlt");
            inicioalt = lector.ToTimeSpanNulable("InicioAlt");
            finalalt = lector.ToTimeSpanNulable("FinalAlt");
            iniciopartidoalt = lector.ToTimeSpanNulable("InicioPartidoAlt");
            finalpartidoalt = lector.ToTimeSpanNulable("FinalPartidoAlt");
            trabajadasalt = lector.ToTimeSpanNulable("TrabajadasAlt");
            acumuladasalt = lector.ToTimeSpanNulable("AcumuladasAlt");
            nocturnasalt = lector.ToTimeSpanNulable("NocturnasAlt");
            desayunoalt = lector.ToDecimalNulable("DesayunoAlt");
            comidaalt = lector.ToDecimalNulable("ComidaAlt");
            cenaalt = lector.ToDecimalNulable("CenaAlt");
            pluscenaalt = lector.ToDecimalNulable("PlusCenaAlt");
            pluslimpiezaalt = lector.ToBoolNulable("PlusLimpiezaAlt");
            pluspaqueteriaalt = lector.ToBoolNulable("PlusPaqueteriaAlt");
            //NUEVAS
            categoriaGrafico = lector.ToString("CategoriaGrafico");
            turno = lector.ToInt32("Turno");
            inicio = lector.ToTimeSpanNulable("Inicio");
            final = lector.ToTimeSpanNulable("Final");
            inicioPartido = lector.ToTimeSpanNulable("InicioPartido");
            finalPartido = lector.ToTimeSpanNulable("FinalPartido");
            trabajadasPartido = lector.ToTimeSpan("TrabajadasPartido");
            tiempopartido = lector.ToTimeSpan("TiempoPartido");
            valoracion = lector.ToTimeSpan("Valoracion");
            trabajadas = lector.ToTimeSpan("Trabajadas");
            trabajadasConvenio = lector.ToTimeSpan("TrabajadasConvenio");
            trabajadasReales = lector.ToTimeSpan("TrabajadasReales");
            tiempoVacio = lector.ToTimeSpan("TiempoVacio");
            acumuladas = lector.ToTimeSpan("Acumuladas");
            nocturnas = lector.ToTimeSpan("Nocturnas");
            desayuno = lector.ToDecimal("Desayuno");
            comida = lector.ToDecimal("Comida");
            cena = lector.ToDecimal("Cena");
            plusCena = lector.ToDecimal("PlusCena");
            plusLimpieza = lector.ToBool("PlusLimpieza");
            plusPaqueteria = lector.ToBool("PlusPaqueteria");

            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("IdCalendario", IdCalendario));
                lista.Add(new SQLiteParameter("Dia", Dia));
                lista.Add(new SQLiteParameter("DiaFecha", DiaFecha.ToString("yyyy-MM-dd")));
                lista.Add(new SQLiteParameter("Grafico", Grafico));
                lista.Add(new SQLiteParameter("Codigo", Codigo));
                lista.Add(new SQLiteParameter("ExcesoJornada", ExcesoJornada.Ticks));
                lista.Add(new SQLiteParameter("FacturadoPaqueteria", FacturadoPaqueteria.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("Limpieza", Limpieza == null ? (object)DBNull.Value : Limpieza == true ? 1 : 0));
                lista.Add(new SQLiteParameter("GraficoVinculado", GraficoVinculado));
                lista.Add(new SQLiteParameter("Notas", Notas.TrimEnd(new char[] { ' ', '\n', '\r', '\t' })));
                lista.Add(new SQLiteParameter("TurnoAlt", TurnoAlt.HasValue ? TurnoAlt : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("InicioAlt", InicioAlt.HasValue ? InicioAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("FinalAlt", FinalAlt.HasValue ? FinalAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("InicioPartidoAlt", InicioPartidoAlt.HasValue ? InicioPartidoAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("FinalPartidoAlt", FinalPartidoAlt.HasValue ? FinalPartidoAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("TrabajadasAlt", TrabajadasAlt.HasValue ? TrabajadasAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("AcumuladasAlt", AcumuladasAlt.HasValue ? AcumuladasAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("NocturnasAlt", NocturnasAlt.HasValue ? NocturnasAlt.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("DesayunoAlt", DesayunoAlt.HasValue ? DesayunoAlt.Value.ToString("0.0000").Replace(",", ".") : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("ComidaAlt", ComidaAlt.HasValue ? ComidaAlt.Value.ToString("0.0000").Replace(",", ".") : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("CenaAlt", CenaAlt.HasValue ? CenaAlt.Value.ToString("0.0000").Replace(",", ".") : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("PlusCenaAlt", PlusCenaAlt.HasValue ? PlusCenaAlt.Value.ToString("0.0000").Replace(",", ".") : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("PlusLimpiezaAlt", PlusLimpiezaAlt.HasValue ? PlusLimpiezaAlt.Value : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("PlusPaqueteriaAlt", PlusPaqueteriaAlt.HasValue ? PlusPaqueteriaAlt.Value : (object)DBNull.Value));
                //NUEVAS
                lista.Add(new SQLiteParameter("CategoriaGrafico", CategoriaGrafico));
                lista.Add(new SQLiteParameter("Turno", Turno));
                lista.Add(new SQLiteParameter("Inicio", Inicio.HasValue ? Inicio.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("Final", Final.HasValue ? Final.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("InicioPartido", InicioPartido.HasValue ? InicioPartido.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("FinalPartido", FinalPartido.HasValue ? FinalPartido.Value.Ticks : (object)DBNull.Value));
                lista.Add(new SQLiteParameter("TrabajadasPartido", TrabajadasPartido.Ticks));
                lista.Add(new SQLiteParameter("TiempoPartido", TiempoPartido.Ticks));
                lista.Add(new SQLiteParameter("Valoracion", Valoracion.Ticks));
                lista.Add(new SQLiteParameter("Trabajadas", Trabajadas.Ticks));
                lista.Add(new SQLiteParameter("TrabajadasConvenio", TrabajadasConvenio.Ticks));
                lista.Add(new SQLiteParameter("TrabajadasReales", TrabajadasReales.Ticks));
                lista.Add(new SQLiteParameter("TiempoVacio", TiempoVacio.Ticks));
                lista.Add(new SQLiteParameter("Acumuladas", Acumuladas.Ticks));
                lista.Add(new SQLiteParameter("Nocturnas", Nocturnas.Ticks));
                lista.Add(new SQLiteParameter("Desayuno", Desayuno.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("Comida", Comida.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("Cena", Cena.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusCena", PlusCena.ToString("0.0000").Replace(",", ".")));
                lista.Add(new SQLiteParameter("PlusLimpieza", PlusLimpieza));
                lista.Add(new SQLiteParameter("PlusPaqueteria", PlusPaqueteria));

                //lista.Add(new SQLiteParameter("Id", Id));
                return lista;
            }
        }


        [JsonIgnore]
        public virtual IEnumerable<ISQLiteItem> Lista { get; }


        [JsonIgnore]
        public virtual bool HasList { get => false; }


        public virtual void InicializarLista() { }


        public virtual void AddItemToList(ISQLiteItem item) { }


        [JsonIgnore]
        public virtual int ForeignId {
            get => IdCalendario;
            set => IdCalendario = value;
        }


        [JsonIgnore]
        public virtual string ForeignIdName { get => "IdCalendario"; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"Dia ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "DiasCalendario"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar {
            get => "INSERT OR REPLACE INTO DiasCalendario (" +
                "IdCalendario, " +
                "Dia, " +
                "DiaFecha, " +
                "Grafico, " +
                "Codigo, " +
                "ExcesoJornada, " +
                "FacturadoPaqueteria, " +
                "Limpieza, " +
                "GraficoVinculado, " +
                "Notas, " +
                "TurnoAlt, " +
                "InicioAlt, " +
                "FinalAlt, " +
                "InicioPartidoAlt, " +
                "FinalPartidoAlt, " +
                "TrabajadasAlt, " +
                "AcumuladasAlt, " +
                "NocturnasAlt, " +
                "DesayunoAlt, " +
                "ComidaAlt, " +
                "CenaAlt, " +
                "PlusCenaAlt, " +
                "PlusLimpiezaAlt, " +
                "PlusPaqueteriaAlt, " +
                "_id) " +
                "VALUES (" +
                "@idCalendario, " +
                "@dia, " +
                "@diaFecha, " +
                "@grafico, " +
                "@codigo, " +
                "@excesoJornada, " +
                "@facturadoPaqueteria, " +
                "@limpieza, " +
                "@graficoVinculado, " +
                "@notas, " +
                "@turnoAlt, " +
                "@inicioAlt, " +
                "@finalAlt, " +
                "@inicioPartidoAlt, " +
                "@finalPartidoAlt, " +
                "@trabajadasAlt, " +
                "@acumuladasAlt, " +
                "@nocturnasAlt, " +
                "@desayunoAlt, " +
                "@comidaAlt, " +
                "@cenaAlt, " +
                "@plusCenaAlt, " +
                "@plusLimpiezaAlt, " +
                "@plusPaqueteriaAlt, " +
                "@id);";
        }




        #endregion
        // ====================================================================================================





    }

}
