#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    //public class DiaPijama : NotifyBase {


    //    // ====================================================================================================
    //    #region  CONSTRUCTORES
    //    // ====================================================================================================
    //    public DiaPijama() {

    //    }

    //    #endregion


    //    // ====================================================================================================
    //    #region MÉTODOS PRIVADOS
    //    // ====================================================================================================

    //    private bool IsServicioCompleto() {
    //        if (Turno < 1 || Turno > 4) return false;
    //        if (!Inicio.HasValue || !Final.HasValue) return false;
    //        if (Inicio.Value.Ticks < 0 || Final.Value.Ticks < 0) return false;
    //        return true;
    //    }


    //    private void CalcularServicio() {
    //        TimeSpan trabajadas = Calculos.Horas.Trabajadas(_inicio.Value, _final.Value.Add(ExcesoJornada), _iniciopartido, _finalpartido);
    //        if (trabajadas < App.Global.Convenio.JornadaMedia) trabajadas = App.Global.Convenio.JornadaMedia;
    //        TimeSpan acumuladas = Calculos.Horas.Acumuladas(trabajadas);
    //        TimeSpan nocturnas = Calculos.Horas.Nocturnas(_inicio.Value, _final.Value.Add(ExcesoJornada), _turno);
    //        decimal desayuno = Calculos.Dietas.Desayuno(_inicio.Value, _turno);
    //        decimal comida = Calculos.Dietas.Comida(_inicio.Value, _final.Value.Add(ExcesoJornada), _turno, _iniciopartido, _finalpartido);
    //        decimal cena = Calculos.Dietas.Cena(_inicio.Value, _final.Value.Add(ExcesoJornada), _turno);
    //        decimal pluscena = Calculos.Dietas.PlusCena(_inicio.Value, _final.Value.Add(ExcesoJornada), _turno);
    //        bool notificar = false;
    //        if (_horas != trabajadas) { _horas = trabajadas; notificar = true; }
    //        if (_acumuladas != acumuladas) { _acumuladas = acumuladas; notificar = true; }
    //        if (_nocturnas != nocturnas) { _nocturnas = nocturnas; notificar = true; }
    //        if (_desayuno != desayuno) { _desayuno = desayuno; notificar = true; }
    //        if (_comida != comida) { _comida = comida; notificar = true; }
    //        if (_cena != cena) { _cena = cena; notificar = true; }
    //        if (_pluscena != pluscena) { _pluscena = pluscena; notificar = true; }
    //        if (notificar) { Modificado = true; PropiedadCambiada(""); }
    //    }


    //    public void Recalcular() {
    //        if (IsServicioCompleto()) {
    //            CalcularServicio();
    //        }
    //    }




    //    #endregion


    //    // ====================================================================================================
    //    #region MÉTODOS ESTÁTICOS
    //    // ====================================================================================================


    //    #endregion


    //    // ====================================================================================================
    //    #region PROPIEDADES
    //    // ====================================================================================================
    //    private int _id;
    //    public int Id {
    //        get { return _id; }
    //        set {
    //            _id = value;
    //            PropiedadCambiada();
    //        }
    //    }


    //    private int _dia;
    //    public int Dia {
    //        get { return _dia; }
    //        set {
    //            if (_dia != value) {
    //                _dia = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private int _grafico;
    //    public int Grafico {
    //        get { return _grafico; }
    //        set {
    //            if (_grafico != value) {
    //                _grafico = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(ComboGrafico));
    //            }
    //        }
    //    }


    //    private int _codigo;
    //    public int Codigo {
    //        get { return _codigo; }
    //        set {
    //            if (_codigo != value) {
    //                _codigo = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(ComboGrafico));
    //            }
    //        }
    //    }


    //    public Tuple<int, int> ComboGrafico {
    //        get { return new Tuple<int, int>(Grafico, Codigo); }
    //        set {
    //            if (Grafico != value.Item1) {
    //                Grafico = value.Item1;
    //            }
    //            if (Codigo != value.Item2) {
    //                Codigo = value.Item2;
    //            }
    //        }
    //    }


    //    private TimeSpan _excesojornada;
    //    public TimeSpan ExcesoJornada {
    //        get { return _excesojornada; }
    //        set {
    //            if (_excesojornada != value) {
    //                _excesojornada = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private bool _bloquearexcesojornada;
    //    public bool BloquearExcesoJornada {
    //        get { return _bloquearexcesojornada; }
    //        set {
    //            if (_bloquearexcesojornada != value) {
    //                _bloquearexcesojornada = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private int _descuadre;
    //    public int Descuadre {
    //        get { return _descuadre; }
    //        set {
    //            if (_descuadre != value) {
    //                _descuadre = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private bool _bloqueardescuadre;
    //    public bool BloquearDescuadre {
    //        get { return _bloqueardescuadre; }
    //        set {
    //            if (_bloqueardescuadre != value) {
    //                _bloqueardescuadre = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    public TimeSpan AñadirAFinal {
    //        get {
    //            TimeSpan resultado = TimeSpan.Zero;
    //            if (!BloquearExcesoJornada) resultado = resultado.Add(ExcesoJornada);
    //            if (!BloquearDescuadre) resultado = resultado.Add(new TimeSpan(0, Descuadre, 0));
    //            return resultado;
    //        }
    //    }


    //    private decimal _facturadopaqueteria;
    //    public decimal FacturadoPaqueteria {
    //        get { return _facturadopaqueteria; }
    //        set {
    //            if (_facturadopaqueteria != value) {
    //                _facturadopaqueteria = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private bool? _limpieza = false;
    //    public bool? Limpieza {
    //        get { return _limpieza; }
    //        set {
    //            if (_limpieza != value) {
    //                _limpieza = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private string _notas;
    //    public string Notas {
    //        get { return _notas; }
    //        set {
    //            if (_notas != value) {
    //                _notas = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(HayNotas));
    //            }
    //        }
    //    }


    //    public string HayNotas {
    //        get {
    //            if (String.IsNullOrEmpty(Notas)) return "";
    //            return "...";
    //        }
    //    }


    //    private int _turno;
    //    public int Turno {
    //        get { return _turno; }
    //        set {
    //            if (_turno != value) {
    //                _turno = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private TimeSpan? _inicio;
    //    public TimeSpan? Inicio {
    //        get { return _inicio; }
    //        set {
    //            if (_inicio != value) {
    //                _inicio = value;
    //                if (Final.HasValue && (Final.Value < Inicio.Value)) Final = Final.Value.Add(new TimeSpan(1, 0, 0, 0));
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TextoInicioFinal));
    //            }
    //        }
    //    }


    //    private TimeSpan? _final;
    //    public TimeSpan? Final {
    //        get { return _final; }
    //        set {
    //            if (_final != value) {
    //                _final = value;
    //                if (Final.HasValue && (Final.Value < Inicio.Value)) Final = Final.Value.Add(new TimeSpan(1, 0, 0, 0));
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TextoInicioFinal));
    //            }
    //        }
    //    }


    //    private TimeSpan? _iniciopartido;
    //    public TimeSpan? InicioPartido {
    //        get { return _iniciopartido; }
    //        set {
    //            if (_iniciopartido != value) {
    //                _iniciopartido = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TextoInicioFinal));
    //            }
    //        }
    //    }


    //    private TimeSpan? _finalpartido;
    //    public TimeSpan? FinalPartido {
    //        get { return _finalpartido; }
    //        set {
    //            if (_finalpartido != value) {
    //                _finalpartido = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TextoInicioFinal));
    //            }
    //        }
    //    }


    //    public string TextoInicioFinal {
    //        get {
    //            string texto = null;
    //            if (Grafico > 0) {
    //                if (Inicio.HasValue && Final.HasValue)
    //                    texto = String.Format("{0} - {1}", Inicio.Value.ToTexto(), Final.Value.ToTexto());
    //                if (InicioPartido.HasValue && FinalPartido.HasValue)
    //                    texto += string.Format("    ({0} - {1})", InicioPartido.Value.ToTexto(), FinalPartido.Value.ToTexto());
    //            }
    //            return texto;
    //        }
    //    }


    //    private TimeSpan _horas;

    //    public TimeSpan Horas {
    //        get { return _horas; }
    //        set {
    //            if (_horas != value) {
    //                _horas = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private TimeSpan _acumuladas;

    //    public TimeSpan Acumuladas {
    //        get { return _acumuladas; }
    //        set {
    //            if (_acumuladas != value) {
    //                _acumuladas = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private TimeSpan _nocturnas;

    //    public TimeSpan Nocturnas {
    //        get { return _nocturnas; }
    //        set {
    //            if (_nocturnas != value) {
    //                _nocturnas = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private decimal _desayuno;

    //    public decimal Desayuno {
    //        get { return _desayuno; }
    //        set {
    //            if (_desayuno != value) {
    //                _desayuno = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TotalDietas));
    //            }
    //        }
    //    }


    //    private decimal _comida;

    //    public decimal Comida {
    //        get { return _comida; }
    //        set {
    //            if (_comida != value) {
    //                _comida = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TotalDietas));
    //            }
    //        }
    //    }


    //    private decimal _cena;

    //    public decimal Cena {
    //        get { return _cena; }
    //        set {
    //            if (_cena != value) {
    //                _cena = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TotalDietas));
    //            }
    //        }
    //    }


    //    private decimal _pluscena;

    //    public decimal PlusCena {
    //        get { return _pluscena; }
    //        set {
    //            if (_pluscena != value) {
    //                _pluscena = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(TotalDietas));
    //            }
    //        }
    //    }

    //    private decimal _plusmenordescanso;
    //    public decimal PlusMenorDescanso {
    //        get { return _plusmenordescanso; }
    //        set {
    //            if (_plusmenordescanso != value) {
    //                _plusmenordescanso = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private decimal _pluspaqueteria;
    //    public decimal PlusPaqueteria {
    //        get { return _pluspaqueteria; }
    //        set {
    //            if (_pluspaqueteria != value) {
    //                _pluspaqueteria = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private decimal _pluslimpieza;
    //    public decimal PlusLimpieza {
    //        get { return _pluslimpieza; }
    //        set {
    //            if (_pluslimpieza != value) {
    //                _pluslimpieza = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private decimal _plusnocturnidad;
    //    public decimal PlusNocturnidad {
    //        get { return _plusnocturnidad; }
    //        set {
    //            if (_plusnocturnidad != value) {
    //                _plusnocturnidad = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(OtrosPluses));
    //            }
    //        }
    //    }


    //    private decimal _plusnavidad;
    //    public decimal PlusNavidad {
    //        get { return _plusnavidad; }
    //        set {
    //            if (_plusnavidad != value) {
    //                _plusnavidad = value;
    //                PropiedadCambiada();
    //                PropiedadCambiada(nameof(OtrosPluses));
    //            }
    //        }
    //    }


    //    public decimal OtrosPluses {
    //        get {
    //            return PlusNocturnidad + PlusNavidad;
    //        }
    //    }


    //    public decimal TotalDietas {
    //        get {
    //            return (Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) + Comida + Cena + PlusCena;
    //        }
    //    }


    //    // ESTA PROPIEDAD NO SE ENLAZA CON LA BASE DE DATOS, SI NO QUE SE ESTABLECE SOLA.
    //    private DateTime _fecha;
    //    public DateTime Fecha {
    //        get { return _fecha; }
    //        set {
    //            if (_fecha != value) {
    //                _fecha = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }


    //    private bool _esfestivo = false;
    //    public bool EsFestivo {
    //        get { return _esfestivo; }
    //        set {
    //            if (_esfestivo != value) {
    //                _esfestivo = value;
    //                PropiedadCambiada();
    //            }
    //        }
    //    }

    //    #endregion


    //}
}
