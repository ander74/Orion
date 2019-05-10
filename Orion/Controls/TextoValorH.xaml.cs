#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

    using System;
    using System.Windows;
    using System.Windows.Controls;

    public partial class TextoValorH : UserControl {


        // ====================================================================================================
        #region ENUMERACIONES
        // ====================================================================================================

        public enum Formato {
            Ninguno, Entero2, Entero3, Entero4, Decimal2, Decimal4, Decimal6, Euro, Hora, HoraPlus, HoraMixta, HoraPlusMixta, Fecha
        }

        public enum ModoCero {
            Mostrar, Oculto, Invisible
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public TextoValorH() {
            InitializeComponent();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES DE DEPENDENCIA
        // ====================================================================================================

        public string Texto {
            get { return (string)GetValue(TextoProperty); }
            set { SetValue(TextoProperty, value); }
        }
        public static readonly DependencyProperty TextoProperty =
            DependencyProperty.Register("Texto", typeof(string), typeof(TextoValorH), new PropertyMetadata("", null, new CoerceValueCallback(OnTextoCoerceValue)));

        public object Valor {
            get { return GetValue(ValorProperty); }
            set { SetValue(ValorProperty, value); }
        }
        public static readonly DependencyProperty ValorProperty =
            DependencyProperty.Register("Valor", typeof(object), typeof(TextoValorH), new PropertyMetadata(null, null, new CoerceValueCallback(OnValorCoerceValue)));

        public Formato FormatoValor {
            get { return (Formato)GetValue(FormatoValorProperty); }
            set { SetValue(FormatoValorProperty, value); }
        }
        public static readonly DependencyProperty FormatoValorProperty =
            DependencyProperty.Register("FormatoValor", typeof(Formato), typeof(TextoValorH), new PropertyMetadata(Formato.Ninguno, new PropertyChangedCallback(OnFormatoValorChanged)));

        public bool MostrarLineaPuntos {
            get { return (bool)GetValue(MostrarLineaPuntosProperty); }
            set { SetValue(MostrarLineaPuntosProperty, value); }
        }
        public static readonly DependencyProperty MostrarLineaPuntosProperty =
            DependencyProperty.Register("MostrarLineaPuntos", typeof(bool), typeof(TextoValorH), new PropertyMetadata(false, new PropertyChangedCallback(OnMostrarLineaPuntosChanged)));

        public ModoCero SiValorEsCero {
            get { return (ModoCero)GetValue(SiValorEsCeroProperty); }
            set { SetValue(SiValorEsCeroProperty, value); }
        }
        public static readonly DependencyProperty SiValorEsCeroProperty =
            DependencyProperty.Register("SiValorEsCero", typeof(ModoCero), typeof(TextoValorH), new PropertyMetadata(ModoCero.Mostrar, new PropertyChangedCallback(OnSiValorEsCeroChanged)));



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static object OnTextoCoerceValue(DependencyObject d, object baseValue) {
            if (baseValue is string s && d.GetValue(MostrarLineaPuntosProperty) is bool b && b == true) {
                return s += $" {new string('.', 50)}";
            }
            return baseValue;
        }

        private static void OnMostrarLineaPuntosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.CoerceValue(TextoProperty);
        }


        private static object OnValorCoerceValue(DependencyObject d, object baseValue) {
            string resultado = string.Empty;
            bool esCero = false;
            Visibility visibilidad = Visibility.Visible;
            switch (d.GetValue(FormatoValorProperty)) {
                case Formato.Entero2:
                    if (baseValue is int i1) {
                        resultado = i1.ToString("00");
                        if (i1 == 0) esCero = true;
                    }
                    break;
                case Formato.Entero3:
                    if (baseValue is int i2) {
                        resultado = i2.ToString("000");
                        if (i2 == 0) esCero = true;
                    }
                    break;
                case Formato.Entero4:
                    if (baseValue is int i3) {
                        resultado = i3.ToString("0000");
                        if (i3 == 0) esCero = true;
                    }
                    break;
                case Formato.Decimal2:
                    if (baseValue is decimal dd1) {
                        resultado = dd1.ToString("0.00");
                        if (dd1 == 0) esCero = true;
                    }
                    break;
                case Formato.Decimal4:
                    if (baseValue is decimal dd2) {
                        resultado = dd2.ToString("0.0000");
                        if (dd2 == 0) esCero = true;
                    }
                    break;
                case Formato.Decimal6:
                    if (baseValue is decimal dd3) {
                        resultado = dd3.ToString("0.000000");
                        if (dd3 == 0) esCero = true;
                    }
                    break;
                case Formato.Euro:
                    if (baseValue is decimal dd4) {
                        resultado = dd4.ToString("0.00 €");
                        if (dd4 == 0) esCero = true;
                    }
                    break;
                case Formato.Hora:
                    if (baseValue is TimeSpan ts1) {
                        resultado = ts1.ToTexto();
                        if (ts1.Ticks == 0) esCero = true;
                    }
                    //TODO: Añadir clase Tiempo.
                    break;
                case Formato.HoraPlus:
                    if (baseValue is TimeSpan ts2) {
                        resultado = ts2.ToTexto(true);
                        if (ts2.Ticks == 0) esCero = true;
                    }
                    //TODO: Añadir clase Tiempo.
                    break;
                case Formato.HoraMixta:
                    if (baseValue is TimeSpan ts3) {
                        resultado = $"{ts3.ToTexto()} ({ts3.ToDecimal().ToString("0.00")})";
                        if (ts3.Ticks == 0) esCero = true;
                    }
                    //TODO: Añadir clase Tiempo.
                    break;
                case Formato.HoraPlusMixta:
                    if (baseValue is TimeSpan ts4) {
                        resultado = $"{ts4.ToTexto(true)} ({ts4.ToDecimal().ToString("0.00")})";
                        if (ts4.Ticks == 0) esCero = true;
                    }
                    //TODO: Añadir clase Tiempo.
                    break;
                case Formato.Fecha:
                    if (baseValue is DateTime dt) {
                        resultado = dt.ToString("dd-MM-yyyy");
                        if (dt == DateTime.MinValue) esCero = true;
                    }
                    //TODO: Añadir clase Tiempo.
                    break;
                default:
                    if (baseValue is string s && string.IsNullOrEmpty(s.Trim())) {
                        esCero = true;
                    }
                    break;
            }

            if (esCero) {
                switch (d.GetValue(SiValorEsCeroProperty)) {
                    case ModoCero.Oculto:
                        resultado = "";
                        break;
                    case ModoCero.Invisible:
                        visibilidad = Visibility.Collapsed;
                        break;
                }
            }
            d.SetValue(VisibilityProperty, visibilidad);

            return resultado;
        }


        private static void OnFormatoValorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.CoerceValue(ValorProperty);
        }

        private static void OnSiValorEsCeroChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.CoerceValue(ValorProperty);
        }











        //private static void OnValorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

        //    if (e.NewValue is string s && s.Equals(d.GetValue(ValorParaOcultarProperty))) {
        //        d.SetValue(VisibilityProperty, Visibility.Collapsed);
        //    } else {
        //        d.SetValue(VisibilityProperty, Visibility.Visible);
        //    }
        //}







        #endregion
        // ====================================================================================================


    }
}
