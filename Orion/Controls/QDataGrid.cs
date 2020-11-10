#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Config;
    using Orion.ViewModels;

    public class QDataGrid : DataGrid {



        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public QDataGrid() {

            // Establecemos las propiedades predeterminadas.
            this.AlternationCount = 2;
            this.AutoGenerateColumns = false;
            this.Background = Brushes.Transparent;
            this.BorderBrush = Brushes.Lavender;
            this.BorderThickness = new Thickness(0.5d);
            this.FontFamily = new FontFamily("Tahoma");
            this.FontSize = 13d;
            this.Foreground = Brushes.DimGray;
            this.HeadersVisibility = DataGridHeadersVisibility.Column;
            this.HorizontalGridLinesBrush = Brushes.Transparent;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.SelectionMode = DataGridSelectionMode.Extended;
            this.SelectionUnit = DataGridSelectionUnit.Cell;
            this.VerticalGridLinesBrush = Brushes.Lavender;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            // Asignamos el estilo.
            this.Style = (Style)App.Current.Resources["QDatagridStyle"];

            // Extraemos el Assembly Name
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;

            // Creamos el menú contextual.
            this.ContextMenu = new ContextMenu();

            this.ContextMenu.Items.Add(new MenuItem {
                Header = "Copiar",
                Icon = new Image { Source = new BitmapImage(new Uri($"/{assemblyName};component/Views/Imagenes/Copiar.png", UriKind.Relative)) },
                Command = CmdCopiar
            });

            this.ContextMenu.Items.Add(new MenuItem {
                Header = "Pegar",
                Icon = new Image { Source = new BitmapImage(new Uri($"/{assemblyName};component/Views/Imagenes/Pegar.png", UriKind.Relative)) },
                Command = CmdPegar
            });
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDAD FILA ACTUAL
        // ====================================================================================================

        public int FilaActual {
            get { return (int)GetValue(FilaActualProperty); }
            set { SetValue(FilaActualProperty, value); }
        }

        public static readonly DependencyProperty FilaActualProperty =
            DependencyProperty.Register("FilaActual", typeof(int), typeof(QDataGrid), new PropertyMetadata(-1));



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDAD COLUMNA ACTUAL
        // ====================================================================================================

        public int ColumnaActual {
            get { return (int)GetValue(ColumnaActualProperty); }
            set { SetValue(ColumnaActualProperty, value); }
        }

        public static readonly DependencyProperty ColumnaActualProperty =
            DependencyProperty.Register("ColumnaActual", typeof(int), typeof(QDataGrid), new PropertyMetadata(-1));

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDAD COMANDO DOBLE CLICK Y PARÁMETRO
        // ====================================================================================================

        public ICommand DobleClick {
            get { return (ICommand)GetValue(DobleClickProperty); }
            set { SetValue(DobleClickProperty, value); }
        }
        public static readonly DependencyProperty DobleClickProperty =
            DependencyProperty.Register("DobleClick", typeof(ICommand), typeof(QDataGrid), new PropertyMetadata());




        public object DobleClickParametro {
            get { return (object)GetValue(DobleClickParametroProperty); }
            set { SetValue(DobleClickParametroProperty, value); }
        }
        public static readonly DependencyProperty DobleClickParametroProperty =
            DependencyProperty.Register("DobleClickParametro", typeof(object), typeof(QDataGrid), new PropertyMetadata());




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDAD ELEMENTO SELECCIONADO
        // ====================================================================================================

        public object ElementoSeleccionado {
            get { return (object)GetValue(ElementoSeleccionadoProperty); }
            set { SetValue(ElementoSeleccionadoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementoSeleccionado.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementoSeleccionadoProperty =
            DependencyProperty.Register("ElementoSeleccionado", typeof(object), typeof(QDataGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDO ALTERNAR MODO SELECCION
        // ====================================================================================================


        // Comando
        private ICommand comandoAlternarModoSeleccion;
        public ICommand ComandoAlternarModoSeleccion {
            get {
                if (comandoAlternarModoSeleccion == null) comandoAlternarModoSeleccion = new RelayCommand(p => AlternarModoSeleccion());
                return comandoAlternarModoSeleccion;
            }
        }

        // Ejecución del comando
        private void AlternarModoSeleccion() {
            if (SelectionUnit == DataGridSelectionUnit.Cell) {
                SelectionUnit = DataGridSelectionUnit.FullRow;
            } else {
                SelectionUnit = DataGridSelectionUnit.Cell;
            }
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDO COPIAR
        // ====================================================================================================

        // Routed Command
        private ICommand cmdCopiar;
        public ICommand CmdCopiar {
            get {
                if (cmdCopiar == null) {
                    cmdCopiar = new RoutedUICommand("Copiar", "CmdCopiar", typeof(QDataGrid), new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control) });
                    this.CommandBindings.Add(new CommandBinding(cmdCopiar, Copiar, PuedeCopiar));
                }
                return cmdCopiar;
            }
        }
        // Relay Command
        private ICommand comandoCopiar;
        public ICommand ComandoCopiar {
            get {
                if (comandoCopiar == null) comandoCopiar = new RelayCommand(p => CmdCopiar.Execute(p), p => CmdCopiar.CanExecute(p));
                return comandoCopiar;
            }
        }

        // Métodos Compartidos
        private void PuedeCopiar(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;// SelectedCells?.Count > 0;
        }
        private void Copiar(object sender, ExecutedRoutedEventArgs e) {
            if (ApplicationCommands.Copy.CanExecute(null, this)) ApplicationCommands.Copy.Execute(null, this);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDO PEGAR
        // ====================================================================================================

        // Routed Command
        private ICommand cmdPegar;
        public ICommand CmdPegar {
            get {
                if (cmdPegar == null) {
                    cmdPegar = new RoutedUICommand("Pegar", "CmdPegar", typeof(QDataGrid), new InputGestureCollection { new KeyGesture(Key.V, ModifierKeys.Control) });
                    this.CommandBindings.Add(new CommandBinding(cmdPegar, Pegar, PuedePegar));
                }
                return cmdPegar;
            }
        }

        // Relay Command
        private ICommand comandoPegar;
        public ICommand ComandoPegar {
            get {
                if (comandoPegar == null) comandoPegar = new RelayCommand(p => CmdPegar.Execute(p), p => CmdPegar.CanExecute(p));
                return comandoPegar;
            }
        }

        // Métodos Compartidos
        private void PuedePegar(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = SelectedCells?.Count > 0 && IsReadOnly == false;
        }
        private void Pegar(object sender, ExecutedRoutedEventArgs e) {
            // Si no hay texto en el portapapeles, salimos
            if (!Clipboard.ContainsText()) return;
            bool hayNuevaFila = false;
            ICollectionView cv = CollectionViewSource.GetDefaultView(Items);
            IEditableCollectionView iecv = cv as IEditableCollectionView;
            // Si hay más de una celda seleccionada copiamos el portapapeles en cada celda.
            if (this.SelectedCells.Count > 1) {
                foreach (var celda in this.SelectedCells) {
                    var col = Columns.IndexOf(celda.Column);
                    var fil = Items.IndexOf(celda.Item);
                    // Si estamos en la última fila, añadimos una fila más.
                    if (fil == Items.Count) {
                        if (!CanUserAddRows) continue;
                        if (iecv != null) {
                            hayNuevaFila = true;
                            iecv.AddNew();
                            iecv.CommitNew();
                        }
                    }
                    DataGridColumn column = ColumnFromDisplayIndex(col);
                    column.OnPastingCellClipboardContent(celda.Item, Clipboard.GetText());
                    if (hayNuevaFila) iecv.CommitNew();
                }
            } else {
                // Recuperamos las filas del portapapeles.
                List<string[]> portapapeles = Utils.parseClipboard();
                if (portapapeles == null) return;
                // Establecemos la fila y la columna actual.
                int filagrid = Items.IndexOf(CurrentItem);
                if (filagrid == -1) return;
                int columnagrid = Columns.IndexOf(CurrentColumn);
                // Iteramos por las filas del portapapeles.
                foreach (string[] fila in portapapeles) {
                    // Si estamos en la última fila, añadimos una fila más.
                    if (filagrid == Items.Count) {
                        if (!CanUserAddRows) continue;
                        if (iecv != null) {
                            hayNuevaFila = true;
                            iecv.AddNew();
                            iecv.CommitNew();
                        }
                    }
                    // Establecemos la columna inicial en la que se va a pegar.
                    int columna = columnagrid;
                    // Iteramos por cada campo de la fila del portapapeles
                    foreach (string texto in fila) {
                        if (columna > Columns.Count - 1) continue;
                        DataGridColumn column = ColumnFromDisplayIndex(columna);
                        column.OnPastingCellClipboardContent(Items[filagrid], texto);
                        columna++;
                    }
                    filagrid++;
                    if (hayNuevaFila) iecv.CommitNew();
                }
            }
            // Hacemos commit a la edición del datagrid.
            this.CommitEdit();
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e) {
            if (e.AddedCells != null && e.AddedCells.Count > 0) {
                var cell = e.AddedCells[0];
                if (!cell.IsValid) return;
                ColumnaActual = cell.Column.DisplayIndex;
                FilaActual = Items.IndexOf(cell.Item);
                if (SelectedCells.Count == 1) ElementoSeleccionado = cell.Item; else ElementoSeleccionado = null;
            }
            base.OnSelectedCellsChanged(e);
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e) {

            switch (e.Key) {
                //case Key.C:
                //    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                //        if (PuedeCopiar()) Copiar();
                //        e.Handled = true;
                //    }
                //    break;
                //case Key.V:
                //    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                //        if (PuedePegar()) Pegar();
                //        e.Handled = true;
                //    }
                //    break;
                case Key.Delete:
                    if (SelectionUnit == DataGridSelectionUnit.FullRow) {
                        CurrentColumn?.OnPastingCellClipboardContent(Items[Items.IndexOf(CurrentItem)], string.Empty);
                    } else {
                        foreach (DataGridCellInfo celda in SelectedCells) {
                            celda.Column?.OnPastingCellClipboardContent(celda.Item, string.Empty);
                        }
                    }
                    e.Handled = true;
                    break;
                case Key.Up:
                case Key.Down:
                    if (e.OriginalSource is TextBox) CommitEdit();
                    break;
            }
        }


        protected override void OnMouseDoubleClick(MouseButtonEventArgs e) {
            if (DobleClick?.CanExecute(DobleClickParametro) ?? false) {
                DobleClick.Execute(DobleClickParametro);
            }
            base.OnMouseDoubleClick(e);
        }


        #endregion
        // ====================================================================================================

    }
}
