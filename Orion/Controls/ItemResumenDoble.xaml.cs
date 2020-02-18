#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

    using System.Windows;
    using System.Windows.Controls;

    public partial class ItemResumenDoble : UserControl {
        public ItemResumenDoble() {
            InitializeComponent();
        }


        // ====================================================================================================
        #region PROPIEDADES DE DEPENDENCIA
        // ====================================================================================================

        public string Descripcion {
            get { return (string)GetValue(DescripcionProperty); }
            set { SetValue(DescripcionProperty, value); }
        }
        public static readonly DependencyProperty DescripcionProperty =
            DependencyProperty.Register("Descripcion", typeof(string), typeof(ItemResumenDoble), new PropertyMetadata("", null, new CoerceValueCallback(OnDescripcionCoerceValue)));

        public string Valor1 {
            get { return (string)GetValue(Valor1Property); }
            set { SetValue(Valor1Property, value); }
        }
        public static readonly DependencyProperty Valor1Property =
            DependencyProperty.Register("Valor1", typeof(string), typeof(ItemResumenDoble), new PropertyMetadata(""));

        public string Valor2 {
            get { return (string)GetValue(Valor2Property); }
            set { SetValue(Valor2Property, value); }
        }
        public static readonly DependencyProperty Valor2Property =
            DependencyProperty.Register("Valor2", typeof(string), typeof(ItemResumenDoble), new PropertyMetadata(""));

        public string Separador {
            get { return (string)GetValue(SeparadorProperty); }
            set { SetValue(SeparadorProperty, value); }
        }
        public static readonly DependencyProperty SeparadorProperty =
            DependencyProperty.Register("Separador", typeof(string), typeof(ItemResumenDoble), new PropertyMetadata(""));

        public bool LineaPuntos {
            get { return (bool)GetValue(LineaPuntosProperty); }
            set { SetValue(LineaPuntosProperty, value); }
        }
        public static readonly DependencyProperty LineaPuntosProperty =
            DependencyProperty.Register("LineaPuntos", typeof(bool), typeof(ItemResumenDoble), new PropertyMetadata(false, new PropertyChangedCallback(OnLineaPuntosChanged)));


        #endregion
        // ====================================================================================================

        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static object OnDescripcionCoerceValue(DependencyObject d, object baseValue) {
            if (baseValue is string s && d.GetValue(LineaPuntosProperty) is bool b && b == true) {
                return s += $" {new string('.', 100)}";
            }
            return baseValue;
        }

        private static void OnLineaPuntosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.CoerceValue(DescripcionProperty);
        }


        #endregion
        // ====================================================================================================


    }
}
