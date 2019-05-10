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
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class NavegadorMes : UserControl {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public NavegadorMes() {
            InitializeComponent();
            this.MesTxt.Text = Fecha.ToString("MMMM").ToUpper();
            this.AñoTxt.Text = Fecha.Year.ToString();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void RetrocedeMes(object sender, RoutedEventArgs e) {
            SetValue(FechaProperty, Fecha.AddMonths(-1));
            if (RetrocedeMesCommand?.CanExecute(RetrocedeMesCommandParameter) ?? false) {
                RetrocedeMesCommand.Execute(RetrocedeMesCommandParameter);
            }

        }

        private void AvanzaMes(object sender, RoutedEventArgs e) {
            SetValue(FechaProperty, Fecha.AddMonths(1));
            if (AvanzaMesCommand?.CanExecute(AvanzaMesCommandParameter) ?? false) {
                AvanzaMesCommand.Execute(AvanzaMesCommandParameter);
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES DE DEPENDENCIA
        // ====================================================================================================

        public DateTime Fecha {
            get { return (DateTime)GetValue(FechaProperty); }
            set { SetValue(FechaProperty, value); }
        }
        public static readonly DependencyProperty FechaProperty =
            DependencyProperty.Register("Fecha",
                                        typeof(DateTime),
                                        typeof(NavegadorMes),
                                        new PropertyMetadata(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), OnFechaChanged));

        public double SeparacionTitulo {
            get { return (double)GetValue(SeparacionTituloProperty); }
            set { SetValue(SeparacionTituloProperty, value); }
        }
        public static readonly DependencyProperty SeparacionTituloProperty =
            DependencyProperty.Register("SeparacionTitulo", typeof(double), typeof(NavegadorMes), new PropertyMetadata(10d));



        public double AnchoTitulo {
            get { return (double)GetValue(AnchoTituloProperty); }
            set { SetValue(AnchoTituloProperty, value); }
        }
        public static readonly DependencyProperty AnchoTituloProperty =
            DependencyProperty.Register("AnchoTitulo", typeof(double), typeof(NavegadorMes), new PropertyMetadata(150d));


        public double TamañoFlechas {
            get { return (double)GetValue(TamañoFlechasProperty); }
            set { SetValue(TamañoFlechasProperty, value); }
        }
        public static readonly DependencyProperty TamañoFlechasProperty =
            DependencyProperty.Register("TamañoFlechas", typeof(double), typeof(NavegadorMes), new PropertyMetadata(24d));


        public Brush ColorFlechas {
            get { return (Brush)GetValue(ColorFlechasProperty); }
            set { SetValue(ColorFlechasProperty, value); }
        }
        public static readonly DependencyProperty ColorFlechasProperty =
            DependencyProperty.Register("ColorFlechas", typeof(Brush), typeof(NavegadorMes), new PropertyMetadata(Brushes.RoyalBlue));


        public double AnchoBotones {
            get { return (double)GetValue(AnchoBotonesProperty); }
            set { SetValue(AnchoBotonesProperty, value); }
        }
        public static readonly DependencyProperty AnchoBotonesProperty =
            DependencyProperty.Register("AnchoBotones", typeof(double), typeof(NavegadorMes), new PropertyMetadata(58d));



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================

        public ICommand RetrocedeMesCommand {
            get { return (ICommand)GetValue(RetrocedeMesCommandProperty); }
            set { SetValue(RetrocedeMesCommandProperty, value); }
        }
        public static readonly DependencyProperty RetrocedeMesCommandProperty =
            DependencyProperty.Register("RetrocedeMesCommand", typeof(ICommand), typeof(NavegadorMes), new PropertyMetadata());


        public object RetrocedeMesCommandParameter {
            get { return (object)GetValue(RetrocedeMesCommandParameterProperty); }
            set { SetValue(RetrocedeMesCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty RetrocedeMesCommandParameterProperty =
            DependencyProperty.Register("RetrocedeMesCommandParameter", typeof(object), typeof(NavegadorMes), new PropertyMetadata());


        public ICommand AvanzaMesCommand {
            get { return (ICommand)GetValue(AvanzaMesCommandProperty); }
            set { SetValue(AvanzaMesCommandProperty, value); }
        }
        public static readonly DependencyProperty AvanzaMesCommandProperty =
            DependencyProperty.Register("AvanzaMesCommand", typeof(ICommand), typeof(NavegadorMes), new PropertyMetadata());


        public object AvanzaMesCommandParameter {
            get { return (object)GetValue(AvanzaMesCommandParameterProperty); }
            set { SetValue(AvanzaMesCommandParameterProperty, value); }
        }
        public static readonly DependencyProperty AvanzaMesCommandParameterProperty =
            DependencyProperty.Register("AvanzaMesCommandParameter", typeof(object), typeof(NavegadorMes), new PropertyMetadata());





        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS
        // ====================================================================================================

        private static void OnFechaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            NavegadorMes este = (NavegadorMes)d;
            DateTime fecha = (DateTime)e.NewValue;
            este.MesTxt.Text = fecha.ToString("MMMM").ToUpper();
            este.AñoTxt.Text = fecha.Year.ToString();
        }



        #endregion
        // ====================================================================================================


    }
}
