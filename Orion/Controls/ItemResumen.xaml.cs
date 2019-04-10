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
    using System.Windows.Media;

    public partial class ItemResumen : UserControl {
        public ItemResumen() {
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
            DependencyProperty.Register("Descripcion", typeof(string), typeof(ItemResumen), new PropertyMetadata("",null,new CoerceValueCallback(OnDescripcionCoerceValue)));

        public string Valor {
            get { return (string)GetValue(ValorProperty); }
            set { SetValue(ValorProperty, value); }
        }
        public static readonly DependencyProperty ValorProperty =
            DependencyProperty.Register("Valor", typeof(string), typeof(ItemResumen), new PropertyMetadata());

        public bool LineaPuntos {
            get { return (bool)GetValue(LineaPuntosProperty); }
            set { SetValue(LineaPuntosProperty, value); }
        }
        public static readonly DependencyProperty LineaPuntosProperty =
            DependencyProperty.Register("LineaPuntos", typeof(bool), typeof(ItemResumen), new PropertyMetadata(false, new PropertyChangedCallback(OnLineaPuntosChanged)));

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static object OnDescripcionCoerceValue(DependencyObject d, object baseValue) {
            if (baseValue is string s && d.GetValue(LineaPuntosProperty) is bool b && b == true) {
                return s += $" {new string('.', 50)}";
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
