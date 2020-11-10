#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Orion.Controls {

    public class VerticalButton : Button {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public VerticalButton() : base() {
            TitleVisibility = (string.IsNullOrEmpty(Title)) ? Visibility.Collapsed : Visibility.Visible;
            ImageVisibility = (Image == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(VerticalButton), new PropertyMetadata(null, null, OnCoerceTitle));


        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(VerticalButton), new PropertyMetadata(null, null, OnCoerceImage));


        public Visibility ImageVisibility {
            get { return (Visibility)GetValue(ImageVisibilityProperty); }
            set { SetValue(ImageVisibilityProperty, value); }
        }
        public static readonly DependencyProperty ImageVisibilityProperty =
            DependencyProperty.Register("ImageVisibility", typeof(Visibility), typeof(VerticalButton), new PropertyMetadata(Visibility.Visible));


        private Visibility TitleVisibility {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }
        private static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(VerticalButton), new PropertyMetadata(Visibility.Visible));


        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(VerticalButton), new PropertyMetadata(new CornerRadius(2)));



        public Brush OnMouseBrush {
            get { return (Brush)GetValue(OnMouseBrushProperty); }
            set { SetValue(OnMouseBrushProperty, value); }
        }
        public static readonly DependencyProperty OnMouseBrushProperty =
            DependencyProperty.Register("OnMouseBrush", typeof(Brush), typeof(VerticalButton), new PropertyMetadata(Brushes.SandyBrown));



        public Brush OnPressBrush {
            get { return (Brush)GetValue(OnPressBrushProperty); }
            set { SetValue(OnPressBrushProperty, value); }
        }
        public static readonly DependencyProperty OnPressBrushProperty =
            DependencyProperty.Register("OnPressBrush", typeof(Brush), typeof(VerticalButton), new PropertyMetadata(Brushes.OldLace));



        public Brush DisabledBrush {
            get { return (Brush)GetValue(DisabledBrushProperty); }
            set { SetValue(DisabledBrushProperty, value); }
        }
        public static readonly DependencyProperty DisabledBrushProperty =
            DependencyProperty.Register("DisabledBrush", typeof(Brush), typeof(VerticalButton), new PropertyMetadata(Brushes.Silver));




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static object OnCoerceTitle(DependencyObject d, object baseValue) {
            if (baseValue == null) {
                d.SetValue(TitleVisibilityProperty, Visibility.Collapsed);
            } else {
                d.SetValue(TitleVisibilityProperty, Visibility.Visible);
            }
            return baseValue;
        }

        private static object OnCoerceImage(DependencyObject d, object baseValue) {
            if (baseValue == null) {
                d.SetValue(ImageVisibilityProperty, Visibility.Collapsed);
            } else {
                d.SetValue(ImageVisibilityProperty, Visibility.Visible);
            }
            return baseValue;
        }
    }
    #endregion
    // ====================================================================================================


}

