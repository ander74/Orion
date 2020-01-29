#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;
using Orion.Config;
using Orion.Interfaces;

namespace Orion.Models {

    public class Itinerario : NotifyBase, ISQLiteItem {


        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public Itinerario() {
            ParadasBorradas = new List<Parada>();
            ListaParadas = new NotifyCollection<Parada>();
        }


        public Itinerario(OleDbDataReader lector) {
            FromReader(lector);
            ParadasBorradas = new List<Parada>();
            ListaParadas = new NotifyCollection<Parada>();
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void BorrarValorPorHeader(string header) {
            switch (header) {
                case "Línea": Nombre = 0m; break;
                case "Descripción": Descripcion = ""; break;
                case "T.Real": TiempoReal = 0; break;
                case "T.Pago": TiempoPago = 0; break;
            }
        }


        public void FromReader(OleDbDataReader lector) {
            _id = lector.ToInt32("Id");
            _idlinea = lector.ToInt32("IdLinea");
            _nombre = lector.ToDecimal("Nombre");
            _descripcion = lector.ToString("Descripcion");
            _tiemporeal = lector.ToInt16("TiempoReal");
            _tiempopago = lector.ToInt16("TiempoPago");
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        public static void ParseFromReader(OleDbDataReader lector, Itinerario itinerario) {
            itinerario.Id = lector.ToInt32("Id");
            itinerario.IdLinea = lector.ToInt32("IdLinea");
            itinerario.Nombre = lector.ToDecimal("Nombre");
            itinerario.Descripcion = lector.ToString("Descripcion");
            itinerario.TiempoReal = lector.ToInt16("TiempoReal");
            itinerario.TiempoPago = lector.ToInt16("TiempoPago");
        }


        public static void ParseToCommand(OleDbCommand Comando, Itinerario itinerario) {
            Comando.Parameters.AddWithValue("idlinea", itinerario.IdLinea);
            Comando.Parameters.AddWithValue("nombre", itinerario.Nombre.ToString("0.0000"));
            Comando.Parameters.AddWithValue("descripcion", itinerario.Descripcion);
            Comando.Parameters.AddWithValue("TiempoReal", itinerario.TiempoReal);
            Comando.Parameters.AddWithValue("TiempoPago", itinerario.TiempoPago);
            Comando.Parameters.AddWithValue("id", itinerario.Id);
        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS SOBRECARGADOS
        // ====================================================================================================

        public override bool Equals(object obj) {
            if (!(obj is Itinerario)) return false;
            Itinerario i = obj as Itinerario;
            if (i.Nombre != Nombre) return false;
            if (i.Descripcion != Descripcion) return false;
            if (i.TiempoPago != TiempoPago || i.TiempoReal != TiempoReal) return false;
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }


        #endregion


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        private void Listaparadas_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.NewItems != null) {
                foreach (Parada parada in e.NewItems) {
                    parada.IdItinerario = this.Id;
                    parada.Nuevo = true;
                    //parada.ObjetoCambiado += ObjetoCambiadoEventHandler;
                }
                Modificado = true;
            }

            if (e.OldItems != null) {
                //foreach (Parada parada in e.OldItems) {
                //    parada.ObjetoCambiado += ObjetoCambiadoEventHandler;
                //}
                //Modificado = true;
            }

            PropiedadCambiada(nameof(ListaParadas));
        }


        private void Listaparadas_ItemPropertyChanged(object sender, ItemChangedEventArgs<Parada> e) {
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



        private int _idlinea;
        public int IdLinea {
            get { return _idlinea; }
            set {
                if (_idlinea != value) {
                    _idlinea = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private decimal _nombre;
        public decimal Nombre {
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
                    _descripcion = value.Replace("//", "\n");
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }



        private int _tiemporeal;
        public int TiempoReal {
            get { return _tiemporeal; }
            set {
                if (_tiemporeal != value) {
                    _tiemporeal = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private int _tiempopago;
        public int TiempoPago {
            get { return _tiempopago; }
            set {
                if (_tiempopago != value) {
                    _tiempopago = value;
                    Modificado = true;
                    PropiedadCambiada();
                }
            }
        }


        private NotifyCollection<Parada> _listaparadas;
        public NotifyCollection<Parada> ListaParadas {
            get { return _listaparadas; }
            set {
                if (_listaparadas != value) {
                    _listaparadas = value;
                    Modificado = true;
                    _listaparadas.CollectionChanged += Listaparadas_CollectionChanged;
                    _listaparadas.ItemPropertyChanged += Listaparadas_ItemPropertyChanged;
                    PropiedadCambiada();
                }
            }
        }

        public List<Parada> ParadasBorradas { get; private set; }


        #endregion


        // ====================================================================================================
        #region INTERFAZ SQLITE ITEM
        // ====================================================================================================


        public void FromReader(DbDataReader lector) {
            _id = lector.ToInt32("_id");
            _idlinea = lector.ToInt32("IdLinea");
            _nombre = lector.ToDecimal("Nombre");
            _descripcion = lector.ToString("Descripcion");
            _tiemporeal = lector.ToInt16("TiempoReal");
            _tiempopago = lector.ToInt16("TiempoPago");
            Nuevo = false;
            Modificado = false;
        }


        public IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();

                lista.Add(new SQLiteParameter("IdLinea", IdLinea));
                lista.Add(new SQLiteParameter("Nombre", Nombre));
                lista.Add(new SQLiteParameter("Descripcion", Descripcion));
                lista.Add(new SQLiteParameter("TiempoReal", TiempoReal));
                lista.Add(new SQLiteParameter("TiempoPago", TiempoPago));
                //lista.Add(new SQLiteParameter("Id", Id));
                return lista;
            }
        }


        public IEnumerable<ISQLiteItem> Lista { get => ListaParadas; }


        public bool HasList { get => true; }


        public void InicializarLista() {
            ListaParadas.Clear();
        }


        public void AddItemToList(ISQLiteItem item) {
            ListaParadas.Add(item as Parada);
        }


        public int ForeignId {
            get => IdLinea;
            set => IdLinea = value;
        }


        public string ForeignIdName { get => "IdItinerario"; }


        public string OrderBy { get => $"Nombre ASC"; }


        public string TableName { get => "Itinerarios"; }


        public string ComandoInsertar {
            get {
                if (Id == 0) {
                    return "INSERT OR REPLACE INTO Itinerarios (" +
                        "IdLinea, " +
                        "Nombre, " +
                        "Descripcion, " +
                        "TiempoReal, " +
                        "TiempoPago) " +
                        "VALUES (" +
                        "@idLinea, " +
                        "@nombre, " +
                        "@descripcion, " +
                        "@tiempoReal, " +
                        "@tiempoPago);";
                } else {
                    return "INSERT OR REPLACE INTO Itinerarios (" +
                        "IdLinea, " +
                        "Nombre, " +
                        "Descripcion, " +
                        "TiempoReal, " +
                        "TiempoPago, " +
                        "_id) " +
                        "VALUES (" +
                        "@idLinea, " +
                        "@nombre, " +
                        "@descripcion, " +
                        "@tiempoReal, " +
                        "@tiempoPago, " +
                        "@id);";
                }
            }
        }


        #endregion
        // ====================================================================================================




    }
}
