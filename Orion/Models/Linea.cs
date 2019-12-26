#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data.SQLite;
using Orion.Interfaces;

namespace Orion.Models {

    public class Linea : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public Linea() {
            _listaitinerarios.CollectionChanged += _listaitinerarios_CollectionChanged;
        }


        public Linea(OleDbDataReader lector) {
            FromReader(lector);
            _listaitinerarios.CollectionChanged += _listaitinerarios_CollectionChanged;
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void BorrarValorPorHeader(string header) {
            switch (header) {
                case "Línea": Nombre = ""; break;
                case "Descripción": Descripcion = ""; break;
            }
        }


        public void FromReader(OleDbDataReader lector) {
            _id = lector.ToInt32("Id");//(lector["Id"] is DBNull) ? 0 : (Int32)lector["Id"];
            _nombre = lector.ToString("Nombre");//(lector["Nombre"] is DBNull) ? "" : (string)lector["Nombre"];
            _descripcion = lector.ToString("Descripcion");//(lector["Descripcion"] is DBNull) ? "" : (string)lector["Descripcion"];
        }


        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, Linea linea) {
            linea.Id = lector.ToInt32("Id");
            linea.Nombre = lector.ToString("Nombre");
            linea.Descripcion = lector.ToString("Descripcion");
        }


        public static void ParseToCommand(OleDbCommand Comando, Linea linea) {
            Comando.Parameters.AddWithValue("nombre", linea.Nombre);
            Comando.Parameters.AddWithValue("descripcion", linea.Descripcion);
            Comando.Parameters.AddWithValue("id", linea.Id);
        }


        #endregion


        // ====================================================================================================
        #region MÉTODOS SOBRECARGADOS
        // ====================================================================================================

        public override bool Equals(object obj) {
            if (!(obj is Linea)) return false;
            Linea l = obj as Linea;
            if (l.Nombre != Nombre) return false;
            if (l.Descripcion != Descripcion) return false;
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void _listaitinerarios_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (Itinerario itinerario in e.NewItems) {
                    itinerario.IdLinea = this.Id;
                    itinerario.Nuevo = true;
                    itinerario.ObjetoCambiado += ObjetoCambiadoEventHandler;
                }
                Modificado = true;
            }


            if (e.OldItems != null) {
                foreach (Itinerario itinerario in e.OldItems) {
                    itinerario.ObjetoCambiado -= ObjetoCambiadoEventHandler;
                }
                Modificado = true;
            }

            PropiedadCambiada(nameof(ListaItinerarios));

        }


        private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
            Modificado = true;
        }
        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private int _id;
        public int Id {
            get { return _id; }
            set {
                if (_id != value) {
                    _id = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private string _nombre = "";
        public string Nombre {
            get { return _nombre; }
            set {
                if (_nombre != value) {
                    _nombre = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private string _descripcion = "";
        public string Descripcion {
            get { return _descripcion; }
            set {
                if (_descripcion != value) {
                    _descripcion = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private ObservableCollection<Itinerario> _listaitinerarios = new ObservableCollection<Itinerario>();
        public ObservableCollection<Itinerario> ListaItinerarios {
            get { return _listaitinerarios; }
            set {
                if (_listaitinerarios != value) {
                    _listaitinerarios = value;
                    Modificado = true;
                    foreach (Itinerario i in _listaitinerarios) {
                        i.ObjetoCambiado += ObjetoCambiadoEventHandler;
                    }
                    _listaitinerarios.CollectionChanged += _listaitinerarios_CollectionChanged;
                    PropiedadCambiada();
                }
            }
        }


        private List<Itinerario> _itinerariosborrados = new List<Itinerario>();
        public List<Itinerario> ItinerariosBorrados {
            get { return _itinerariosborrados; }
        }


        #endregion


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public void FromReader(SQLiteDataReader lector) {
            _id = lector.ToInt32("_id");
            _nombre = lector.ToString("Nombre");
            _descripcion = lector.ToString("Descripcion");
            Nuevo = false;
            Modificado = false;
        }


        public IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("Nombre", Nombre));
                lista.Add(new SQLiteParameter("Descripcion", Descripcion));
                //lista.Add(new SQLiteParameter("Id", Id));
                return lista;
            }
        }


        public IEnumerable<ISQLiteItem> Lista { get => ListaItinerarios; }


        public bool HasList { get => true; }


        public void InicializarLista() {
            ListaItinerarios = new ObservableCollection<Itinerario>();
        }


        public void AddItemToList(ISQLiteItem item) {
            ListaItinerarios.Add(item as Itinerario);
        }


        public int ForeignId { get; set; }


        public string ForeignIdName { get => "IdLinea"; }


        public string OrderBy { get => $"Nombre ASC"; }


        public string TableName { get => "Lineas"; }


        public string ComandoInsertar {
            get {
                if (Id == 0) {
                    return "INSERT OR REPLACE INTO Lineas (" +
                        "Nombre, " +
                        "Descripcion) " +
                        "VALUES (" +
                        "@nombre, " +
                        "@descripcion);";
                } else {
                    return "INSERT OR REPLACE INTO Lineas (" +
                        "Nombre, " +
                        "Descripcion, " +
                        "_id) " +
                        "VALUES (" +
                        "@nombre, " +
                        "@descripcion, " +
                        "@id);";
                }
            }
        }


        #endregion
        // ====================================================================================================








    }
}
