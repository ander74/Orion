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
    using System.Data.Common;
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
            Validez = lector.ToDateTime("xValidez");
            Turno = lector.ToInt16("xTurno");
            NumeroGraficos = lector.ToInt32("xNumero");
            Valoracion = lector.ToTimeSpanNulable("xValoracion").Value;
            Trabajadas = lector.ToTimeSpanNulable("xTrabajadas").Value;
            Acumuladas = lector.ToTimeSpanNulable("xAcumuladas").Value;
            Nocturnas = lector.ToTimeSpanNulable("xNocturnas").Value;
            Desayuno = lector.ToDecimal("xDesayuno");
            Comida = lector.ToDecimal("xComida");
            Cena = lector.ToDecimal("xCena");
            PlusCena = lector.ToDecimal("xPlusCena");
            Limpieza = (int)lector.ToDouble("xLimpieza");
            Paqueteria = (int)lector.ToDouble("xPaqueteria");
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region  PROPIEDADES
        // ====================================================================================================


        public Centros Centro { get; set; }
        public DateTime Validez { get; set; }


        private int _turno;
        public int Turno {
            get { return _turno; }
            set { _turno = value; }
        }

        public int NumeroGraficos { get; set; }


        public TimeSpan Valoracion { get; set; }


        public TimeSpan Trabajadas { get; set; }


        public TimeSpan Acumuladas { get; set; }


        public TimeSpan Nocturnas { get; set; }


        public decimal Desayuno { get; set; }


        public decimal Comida { get; set; }


        public decimal Cena { get; set; }
        public decimal PlusCena { get; set; }


        public decimal TotalDietas {
            get {
                return (Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100m) + Comida + Cena + PlusCena;
            }
        }


        public int Limpieza { get; set; }


        public int Paqueteria { get; set; }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES Y MÉTODOS ISQLITEM
        // ====================================================================================================

        // Propiedades que no se van a usar.
        public int Id { get; set; }
        public bool Nuevo { get; set; }
        public bool Modificado { get; set; }



        public void FromReader(DbDataReader lector) {
            Validez = lector.ToDateTime("xValidez");
            Turno = lector.ToInt32("xTurno");
            NumeroGraficos = lector.ToInt32("xNumero");
            Valoracion = lector.ToTimeSpanNulable("xValoracion").Value;
            Trabajadas = lector.ToTimeSpanNulable("xTrabajadas").Value;
            Acumuladas = lector.ToTimeSpanNulable("xAcumuladas").Value;
            Nocturnas = lector.ToTimeSpanNulable("xNocturnas").Value;
            Desayuno = lector.ToDecimal("xDesayuno");
            Comida = lector.ToDecimal("xComida");
            Cena = lector.ToDecimal("xCena");
            PlusCena = lector.ToDecimal("xPlusCena");
            Limpieza = (int)lector.ToDouble("xLimpieza");
            Paqueteria = (int)lector.ToDouble("xPaqueteria");
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
