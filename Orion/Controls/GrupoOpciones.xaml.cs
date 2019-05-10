#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

    using System.Windows.Media;
    using System.Windows;
    using System.Windows.Controls;

    public partial class GrupoOpciones : UserControl {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public GrupoOpciones() {
            InitializeComponent();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPEIDADES
        // ====================================================================================================
               
        public Brush ColorBorde {
            get { return (Brush)GetValue(ColorBordeProperty); }
            set { SetValue(ColorBordeProperty, value); }
        }
        public static readonly DependencyProperty ColorBordeProperty =
            DependencyProperty.Register("ColorBorde", typeof(Brush), typeof(GrupoOpciones), new PropertyMetadata(Brushes.DarkBlue));


        public object Titulo {
            get { return GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }
        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register("Titulo", typeof(object), typeof(GrupoOpciones), new PropertyMetadata());


        public object Contenido {
            get { return GetValue(ContenidoProperty); }
            set { SetValue(ContenidoProperty, value); }
        }
        public static readonly DependencyProperty ContenidoProperty =
            DependencyProperty.Register("Contenido", typeof(object), typeof(GrupoOpciones), new PropertyMetadata(0));


        public Brush FondoTitulo {
            get { return (Brush)GetValue(FondoTituloProperty); }
            set { SetValue(FondoTituloProperty, value); }
        }
        public static readonly DependencyProperty FondoTituloProperty =
            DependencyProperty.Register("FondoTitulo", typeof(Brush), typeof(GrupoOpciones), new PropertyMetadata());


        public HorizontalAlignment AlineacionTitulo {
            get { return (HorizontalAlignment)GetValue(AlineacionTituloProperty); }
            set { SetValue(AlineacionTituloProperty, value); }
        }
        public static readonly DependencyProperty AlineacionTituloProperty =
            DependencyProperty.Register("AlineacionTitulo", typeof(HorizontalAlignment), typeof(GrupoOpciones), new PropertyMetadata(HorizontalAlignment.Center));


        public int FontSizeTitulo {
            get { return (int)GetValue(FontSizeTituloProperty); }
            set { SetValue(FontSizeTituloProperty, value); }
        }
        public static readonly DependencyProperty FontSizeTituloProperty =
            DependencyProperty.Register("FontSizeTitulo", typeof(int), typeof(GrupoOpciones), new PropertyMetadata(16));


        #endregion
        // ====================================================================================================


    }
}
