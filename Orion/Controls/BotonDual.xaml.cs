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
    using System.Windows.Media;

    public partial class BotonDual : Button {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public BotonDual() {
            InitializeComponent();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public string TextoNormal {
            get { return (string)GetValue(TextoNormalProperty); }
            set { SetValue(TextoNormalProperty, value); }
        }
        public static readonly DependencyProperty TextoNormalProperty =
            DependencyProperty.Register("TextoNormal", typeof(string), typeof(BotonDual), new PropertyMetadata(null, OnIsActivoChanged));


        public string TextoActivo {
            get { return (string)GetValue(TextoActivoProperty); }
            set { SetValue(TextoActivoProperty, value); }
        }
        public static readonly DependencyProperty TextoActivoProperty =
            DependencyProperty.Register("TextoActivo", typeof(string), typeof(BotonDual), new PropertyMetadata(null, OnIsActivoChanged));


        public ImageSource ImagenNormal {
            get { return (ImageSource)GetValue(ImagenNormalProperty); }
            set { SetValue(ImagenNormalProperty, value); }
        }
        public static readonly DependencyProperty ImagenNormalProperty =
            DependencyProperty.Register("ImagenNormal", typeof(ImageSource), typeof(BotonDual), new PropertyMetadata(null, OnIsActivoChanged));


        public ImageSource ImagenActivo {
            get { return (ImageSource)GetValue(ImagenActivoProperty); }
            set { SetValue(ImagenActivoProperty, value); }
        }
        public static readonly DependencyProperty ImagenActivoProperty =
            DependencyProperty.Register("ImagenActivo", typeof(ImageSource), typeof(BotonDual), new PropertyMetadata(null, OnIsActivoChanged));


        private ImageSource ImagenFinal {
            get { return (ImageSource)GetValue(ImagenFinalProperty); }
            set { SetValue(ImagenFinalProperty, value); }
        }
        private static readonly DependencyProperty ImagenFinalProperty =
            DependencyProperty.Register("ImagenFinal", typeof(ImageSource), typeof(BotonDual), new PropertyMetadata(null, null, OnCoerceImagenFinal));


        private string TextoFinal {
            get { return (string)GetValue(TextoFinalProperty); }
            set { SetValue(TextoFinalProperty, value); }
        }
        private static readonly DependencyProperty TextoFinalProperty =
            DependencyProperty.Register("TextoFinal", typeof(string), typeof(BotonDual), new PropertyMetadata(null, null, OnCoerceTextoFinal));


        public bool IsActivo {
            get { return (bool)GetValue(IsActivoProperty); }
            set { SetValue(IsActivoProperty, value); }
        }
        public static readonly DependencyProperty IsActivoProperty =
            DependencyProperty.Register("IsActivo", typeof(bool), typeof(BotonDual), new PropertyMetadata(false, OnIsActivoChanged));


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static void OnIsActivoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.CoerceValue(ImagenFinalProperty);
            d.CoerceValue(TextoFinalProperty);
        }


        private static object OnCoerceImagenFinal(DependencyObject d, object baseValue) {
            if (d.GetValue(IsActivoProperty) is bool isActivo && isActivo) {
                return d.GetValue(ImagenActivoProperty);
            }
            return d.GetValue(ImagenNormalProperty);

        }


        private static object OnCoerceTextoFinal(DependencyObject d, object baseValue) {
            if (d.GetValue(IsActivoProperty) is bool isActivo && isActivo) {
                return d.GetValue(TextoActivoProperty);
            }
            return d.GetValue(TextoNormalProperty);

        }


        #endregion
        // ====================================================================================================


    }
}
