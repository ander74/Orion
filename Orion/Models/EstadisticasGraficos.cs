#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Models {

    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Data.SQLite;
    using Orion.Interfaces;

    public class EstadisticasGraficos : ISQLiteItem {


        // ====================================================================================================
        #region  CONSTRUCTORES
        // ====================================================================================================

        public EstadisticasGraficos() { }


        public EstadisticasGraficos(OleDbDataReader lector) {
            FromReader(lector);
        }

        #endregion


        // ====================================================================================================
        #region  MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void FromReader(OleDbDataReader lector) {
            _validez = lector.ToDateTime("xValidez");
            _turno = lector.ToInt16("xTurno");
            _numerograficos = lector.ToInt32("xNumero");
            _valoracion = lector.ToTimeSpanNulable("xValoracion").Value;
            _trabajadas = lector.ToTimeSpanNulable("xTrabajadas").Value;
            _acumuladas = lector.ToTimeSpanNulable("xAcumuladas").Value;
            _nocturnas = lector.ToTimeSpanNulable("xNocturnas").Value;
            _desayuno = lector.ToDecimal("xDesayuno");
            _comida = lector.ToDecimal("xComida");
            _cena = lector.ToDecimal("xCena");
            _pluscena = lector.ToDecimal("xPlusCena");
            _limpieza = (int)lector.ToDouble("xLimpieza");
            _paqueteria = (int)lector.ToDouble("xPaqueteria");
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region  PROPIEDADES
        // ====================================================================================================


        private Centros _centro;
        public Centros Centro {
            get { return _centro; }
            set { _centro = value; }
        }


        private DateTime _validez;
        public DateTime Validez {
            get { return _validez; }
            set { _validez = value; }
        }


        private int _turno;
        public int Turno {
            get { return _turno; }
            set { _turno = value; }
        }


        private int _numerograficos;
        public int NumeroGraficos {
            get { return _numerograficos; }
            set { _numerograficos = value; }
        }


        private TimeSpan _valoracion;
        public TimeSpan Valoracion {
            get { return _valoracion; }
            set { _valoracion = value; }
        }


        private TimeSpan _trabajadas;
        public TimeSpan Trabajadas {
            get { return _trabajadas; }
            set { _trabajadas = value; }
        }

        private TimeSpan _acumuladas;
        public TimeSpan Acumuladas {
            get { return _acumuladas; }
            set { _acumuladas = value; }
        }


        private TimeSpan _nocturnas;
        public TimeSpan Nocturnas {
            get { return _nocturnas; }
            set { _nocturnas = value; }
        }


        private decimal _desayuno;
        public decimal Desayuno {
            get { return _desayuno; }
            set { _desayuno = value; }
        }


        private decimal _comida;
        public decimal Comida {
            get { return _comida; }
            set { _comida = value; }
        }


        private decimal _cena;
        public decimal Cena {
            get { return _cena; }
            set { _cena = value; }
        }


        private decimal _pluscena;
        public decimal PlusCena {
            get { return _pluscena; }
            set { _pluscena = value; }
        }


        public decimal TotalDietas {
            get {
                return (_desayuno * App.Global.Convenio.PorcentajeDesayuno / 100m) + Comida + Cena + PlusCena;
            }
        }


        private int _limpieza;
        public int Limpieza {
            get { return _limpieza; }
            set { _limpieza = value; }
        }


        private int _paqueteria;
        public int Paqueteria {
            get { return _paqueteria; }
            set { _paqueteria = value; }
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES Y MÉTODOS ISQLITEM
        // ====================================================================================================

        // Propiedades que no se van a usar.
        public int Id { get; set; }
        public bool Nuevo { get; set; }
        public bool Modificado { get; set; }



        public void FromReader(SQLiteDataReader lector) {
            _validez = lector.ToDateTime("xValidez");
            _turno = lector.ToInt32("xTurno");
            _numerograficos = lector.ToInt32("xNumero");
            _valoracion = lector.ToTimeSpanNulable("xValoracion").Value;
            _trabajadas = lector.ToTimeSpanNulable("xTrabajadas").Value;
            _acumuladas = lector.ToTimeSpanNulable("xAcumuladas").Value;
            _nocturnas = lector.ToTimeSpanNulable("xNocturnas").Value;
            _desayuno = lector.ToDecimal("xDesayuno");
            _comida = lector.ToDecimal("xComida");
            _cena = lector.ToDecimal("xCena");
            _pluscena = lector.ToDecimal("xPlusCena");
            _limpieza = (int)lector.ToDouble("xLimpieza");
            _paqueteria = (int)lector.ToDouble("xPaqueteria");
        }


        public IEnumerable<SQLiteParameter> Parametros { get; }


        public IEnumerable<ISQLiteItem> Lista { get; }


        public bool HasList { get => false; }


        public void InicializarLista() { }


        public void AddItemToList(ISQLiteItem item) { }


        public int ForeignId { get; set; }


        public string ForeignIdName { get => ""; }


        public string OrderBy { get => $""; }


        public string TableName { get => ""; }


        public string ComandoInsertar {
            get => "";
        }


        public string ComandoActualizar {
            get => "";
        }


        #endregion
        // ====================================================================================================





    }
}
