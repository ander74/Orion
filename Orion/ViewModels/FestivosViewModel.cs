#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows.Data;
    using Orion.Config;
    using Orion.DataModels;
    using Orion.Models;
    using Orion.Servicios;

    public class FestivosViewModel : NotifyBase {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private IMensajes mensajes;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public FestivosViewModel(IMensajes servicioMensajes) {
            mensajes = servicioMensajes;
            CargarDatos();
            AñoActual = DateTime.Now.Year;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void CargarDatos() {
            if (App.Global.CadenaConexion == null) {
                ListaFestivos?.Clear();
                return;
            }
            ListaFestivos = new NotifyCollection<Festivo>(BdFestivos.GetFestivos());
            VistaFestivos = new ListCollectionView(ListaFestivos);
        }


        public void GuardarDatos() {
            HayCambios = false;
            if (ListaFestivos != null && ListaFestivos.Count > 0) {
                BdFestivos.GuardarFestivos(ListaFestivos.Where(f => f.Nuevo || f.Modificado));
            }
            if (ListaFestivos != null && ListaFestivos.Any(f => f.Borrado)) {
                BdFestivos.BorrarFestivos(ListaFestivos.Where(f => f.Borrado));
            }
        }


        public void GuardarTodo() {
            GuardarDatos();
            HayCambios = false;
        }


        public void Reiniciar() {
            CargarDatos();
            AñoActual = DateTime.Now.Year;
            if (VistaFestivos != null) VistaFestivos.Filter = f => (f as Festivo).Año == _añoactual;
            HayCambios = false;
        }


        internal bool EsFestivo(DateTime fecha) {
            return ListaFestivos.Any(f => f.Fecha.Equals(fecha));
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void Listafestivos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (Festivo festivo in e.NewItems) {
                    festivo.Año = AñoActual;
                    festivo.Nuevo = true;
                    HayCambios = true;
                }
            }

            PropiedadCambiada(nameof(ListaFestivos));
        }


        private void Listafestivos_ItemPropertyChanged(object sender, ItemChangedEventArgs<Festivo> e) {
            HayCambios = true;
            if (e.PropertyName == "Borrado") VistaFestivos.Refresh();
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private NotifyCollection<Festivo> _listafestivos;
        public NotifyCollection<Festivo> ListaFestivos {
            get { return _listafestivos; }
            set {
                if (SetValue(ref _listafestivos, value)) {
                    _listafestivos.CollectionChanged += Listafestivos_CollectionChanged;
                    _listafestivos.ItemPropertyChanged += Listafestivos_ItemPropertyChanged;
                }
            }
        }


        private ListCollectionView _vistafestivos;
        public ListCollectionView VistaFestivos {
            get { return _vistafestivos; }
            set { SetValue(ref _vistafestivos, value); }
        }


        private int _añoactual;
        public int AñoActual {
            get { return _añoactual; }
            set {
                if (SetValue(ref _añoactual, value)) {
                    if (VistaFestivos != null) VistaFestivos.Filter = f => (f as Festivo).Año == _añoactual && (f as Festivo).Borrado == false;
                }
            }
        }


        private bool _haycambios;
        public bool HayCambios {
            get { return _haycambios; }
            set { SetValue(ref _haycambios, value); }
        }



        #endregion
        // ====================================================================================================




    }
}
