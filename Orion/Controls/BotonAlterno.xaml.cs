#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public partial class BotonAlterno : ToggleButton {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public BotonAlterno() {
            InitializeComponent();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public ImageSource ImagenActivo {
            get { return (ImageSource)GetValue(ImagenActivoProperty); }
            set { SetValue(ImagenActivoProperty, value); }
        }
        public static readonly DependencyProperty ImagenActivoProperty =
            DependencyProperty.Register("ImagenActivo", typeof(ImageSource), typeof(BotonAlterno), new PropertyMetadata());

        public ImageSource ImagenInactivo {
            get { return (ImageSource)GetValue(ImagenInactivoProperty); }
            set { SetValue(ImagenInactivoProperty, value); }
        }
        public static readonly DependencyProperty ImagenInactivoProperty =
            DependencyProperty.Register("ImagenInactivo", typeof(ImageSource), typeof(BotonAlterno), new PropertyMetadata());

        public string TituloActivo {
            get { return (string)GetValue(TituloActivoProperty); }
            set { SetValue(TituloActivoProperty, value); }
        }
        public static readonly DependencyProperty TituloActivoProperty =
            DependencyProperty.Register("TituloActivo", typeof(string), typeof(BotonAlterno), new PropertyMetadata());

        public string TituloInactivo {
            get { return (string)GetValue(TituloInactivoProperty); }
            set { SetValue(TituloInactivoProperty, value); }
        }
        public static readonly DependencyProperty TituloInactivoProperty =
            DependencyProperty.Register("TituloInactivo", typeof(string), typeof(BotonAlterno), new PropertyMetadata());


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void Item_Checked(object sender, RoutedEventArgs e) {
            if (IsChecked == true) {
                Imagen.Source = ImagenActivo;
                Texto.Text = TituloActivo;
            } else {
                Imagen.Source = ImagenInactivo;
                Texto.Text = TituloInactivo;
            }
        }


        #endregion
        // ====================================================================================================

    }
}
