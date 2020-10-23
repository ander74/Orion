#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Controls {

    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Config;
    using Orion.ViewModels;

    public class QDataGrid : DataGrid {


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================


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
            DependencyProperty.Register("ElementoSeleccionado", typeof(object), typeof(QDataGrid), new PropertyMetadata(null, null, null));



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

        // Comando
        private ICommand comandoCopiar;
        public ICommand ComandoCopiar {
            get {
                if (comandoCopiar == null) comandoCopiar = new RelayCommand(p => Copiar(), p => PuedeCopiar());
                return comandoCopiar;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeCopiar() {
            return CurrentCell != null && SelectionUnit == DataGridSelectionUnit.Cell;
        }

        // Ejecución del comando
        private void Copiar() {
            if (ApplicationCommands.Copy.CanExecute(null, this)) ApplicationCommands.Copy.Execute(null, this);
        }
        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDO PEGAR
        // ====================================================================================================

        private ICommand comandoPegar;
        public ICommand ComandoPegar {
            get {
                if (comandoPegar == null) comandoPegar = new RelayCommand(p => Pegar(), p => PuedePegar());
                return comandoPegar;
            }
        }


        private bool PuedePegar() {
            return CurrentCell != null && SelectionUnit == DataGridSelectionUnit.Cell;
        }


        private void Pegar() {
            // Recuperamos las filas del portapapeles.
            List<string[]> portapapeles = Utils.parseClipboard();
            if (portapapeles == null) return;
            // Establecemos la fila y la columna actual.
            int filagrid = Items.IndexOf(CurrentItem);
            if (filagrid == -1) return;
            int columnagrid = Columns.IndexOf(CurrentColumn);
            bool hayNuevaFila = false;
            ICollectionView cv = CollectionViewSource.GetDefaultView(Items);
            IEditableCollectionView iecv = cv as IEditableCollectionView;
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
                case Key.C:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        if (PuedeCopiar()) Copiar();
                        e.Handled = true;
                    }
                    break;
                case Key.V:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        if (PuedePegar()) Pegar();
                        e.Handled = true;
                    }
                    break;
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
