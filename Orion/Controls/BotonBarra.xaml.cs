#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orion.Controls {
    /// <summary>
    /// Lógica de interacción para BotonBarra.xaml
    /// </summary>
    public partial class BotonBarra : Button {
        public BotonBarra() {
            InitializeComponent();
        }

        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public string Titulo {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }
        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register("Titulo", typeof(string), typeof(BotonBarra), new PropertyMetadata(null, null, OnCoerceTitulo));



        public ImageSource Imagen {
            get { return (ImageSource)GetValue(ImagenProperty); }
            set { SetValue(ImagenProperty, value); }
        }
        public static readonly DependencyProperty ImagenProperty =
            DependencyProperty.Register("Imagen", typeof(ImageSource), typeof(BotonBarra), new PropertyMetadata(null, null, OnCoerceImagen));


        private Visibility VisibilidadImagen {
            get { return (Visibility)GetValue(VisibilidadImagenProperty); }
            set { SetValue(VisibilidadImagenProperty, value); }
        }
        private static readonly DependencyProperty VisibilidadImagenProperty =
            DependencyProperty.Register("VisibilidadImagen", typeof(Visibility), typeof(BotonBarra), new PropertyMetadata(Visibility.Visible));


        private Visibility VisibilidadTitulo {
            get { return (Visibility)GetValue(VisibilidadTituloProperty); }
            set { SetValue(VisibilidadTituloProperty, value); }
        }
        private static readonly DependencyProperty VisibilidadTituloProperty =
            DependencyProperty.Register("VisibilidadTitulo", typeof(Visibility), typeof(BotonBarra), new PropertyMetadata(Visibility.Visible));




        #endregion
        // ====================================================================================================

        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static object OnCoerceTitulo(DependencyObject d, object baseValue) {
            if (baseValue == null) {
                d.SetValue(VisibilidadTituloProperty, Visibility.Collapsed);
            } else {
                d.SetValue(VisibilidadTituloProperty, Visibility.Visible);
            }
            return baseValue;
        }

        private static object OnCoerceImagen(DependencyObject d, object baseValue) {
            if (baseValue == null) {
                d.SetValue(VisibilidadImagenProperty, Visibility.Collapsed);
            } else {
                d.SetValue(VisibilidadImagenProperty, Visibility.Visible);
            }
            return baseValue;
        }

        #endregion
        // ====================================================================================================


    }
}
