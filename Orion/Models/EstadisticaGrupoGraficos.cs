#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SQLite;
using Orion.Interfaces;

namespace Orion.Models {

    public class EstadisticaGrupoGraficos : ISQLItem {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public EstadisticaGrupoGraficos() { }


        public EstadisticaGrupoGraficos(OleDbDataReader lector) {
            FromReader(lector);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region  MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void FromReader(OleDbDataReader lector, Centros centro = Centros.Desconocido) {
            Validez = lector.ToDateTime("Validez");
            CantidadGraficos = lector.ToInt32("Cantidad");
            CantidadTurnos1 = lector.ToInt32("Turnos1");
            CantidadTurnos2 = lector.ToInt32("Turnos2");
            CantidadTurnos3 = lector.ToInt32("Turnos3");
            CantidadTurnos4 = lector.ToInt32("Turnos4");
            Valoracion = lector.ToTimeSpan("Valoraciones");
            Trabajadas = lector.ToTimeSpan("H_Trabajadas");
            TrabajadasTurno1 = lector.ToTimeSpan("TrabajadasTurno1");
            TrabajadasTurno2 = lector.ToTimeSpan("TrabajadasTurno2");
            TrabajadasTurno3 = lector.ToTimeSpan("TrabajadasTurno3");
            TrabajadasTurno4 = lector.ToTimeSpan("TrabajadasTurno4");
            Acumuladas = lector.ToTimeSpan("H_Acumuladas");
            Nocturnas = lector.ToTimeSpan("H_Nocturnas");
            Desayuno = lector.ToDecimal("Desayunos");
            Comida = lector.ToDecimal("Comidas");
            Cena = lector.ToDecimal("Cenas");
            PlusCena = lector.ToDecimal("PlusesCena");
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public Centros Centro { get; set; }

        public DateTime Validez { get; set; }

        public int CantidadGraficos { get; set; }

        public int CantidadTurnos1 { get; set; }

        public int CantidadTurnos2 { get; set; }

        public int CantidadTurnos3 { get; set; }

        public int CantidadTurnos4 { get; set; }

        public decimal PorcentajeTurnos1 {
            get { return CantidadGraficos == 0 ? 0 : Math.Round(CantidadTurnos1 * 100m / CantidadGraficos, 2); }
        }

        public decimal PorcentajeTurnos2 {
            get { return CantidadGraficos == 0 ? 0 : Math.Round(CantidadTurnos2 * 100m / CantidadGraficos, 2); }
        }

        public decimal PorcentajeTurnos3 {
            get { return CantidadGraficos == 0 ? 0 : Math.Round(CantidadTurnos3 * 100m / CantidadGraficos, 2); }
        }

        public decimal PorcentajeTurnos4 {
            get { return CantidadGraficos == 0 ? 0 : Math.Round(CantidadTurnos4 * 100m / CantidadGraficos, 2); }
        }

        public TimeSpan Valoracion { get; set; }

        public TimeSpan Trabajadas { get; set; }

        public TimeSpan TrabajadasTurno1 { get; set; }

        public TimeSpan TrabajadasTurno2 { get; set; }

        public TimeSpan TrabajadasTurno3 { get; set; }

        public TimeSpan TrabajadasTurno4 { get; set; }

        public TimeSpan MediaTrabajadas {
            get { return CantidadGraficos == 0 ? TimeSpan.Zero : new TimeSpan(Trabajadas.Ticks / CantidadGraficos); }
        }

        public TimeSpan MediaTrabajadasTurno1 {
            get { return CantidadTurnos1 == 0 ? TimeSpan.Zero : new TimeSpan(TrabajadasTurno1.Ticks / CantidadTurnos1); }
        }

        public TimeSpan MediaTrabajadasTurno2 {
            get { return CantidadTurnos2 == 0 ? TimeSpan.Zero : new TimeSpan(TrabajadasTurno2.Ticks / CantidadTurnos2); }
        }

        public TimeSpan MediaTrabajadasTurno3 {
            get { return CantidadTurnos3 == 0 ? TimeSpan.Zero : new TimeSpan(TrabajadasTurno3.Ticks / CantidadTurnos3); }
        }

        public TimeSpan MediaTrabajadasTurno4 {
            get { return CantidadTurnos4 == 0 ? TimeSpan.Zero : new TimeSpan(TrabajadasTurno4.Ticks / CantidadTurnos4); }
        }

        public TimeSpan Acumuladas { get; set; }

        public TimeSpan Nocturnas { get; set; }

        public decimal Desayuno { get; set; }

        public decimal Comida { get; set; }

        public decimal Cena { get; set; }

        public decimal PlusCena { get; set; }




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
            Validez = lector.ToDateTime("Validez");
            CantidadGraficos = lector.ToInt32("Cantidad");
            CantidadTurnos1 = lector.ToInt32("Turnos1");
            CantidadTurnos2 = lector.ToInt32("Turnos2");
            CantidadTurnos3 = lector.ToInt32("Turnos3");
            CantidadTurnos4 = lector.ToInt32("Turnos4");
            Valoracion = lector.ToTimeSpan("Valoraciones");
            Trabajadas = lector.ToTimeSpan("H_Trabajadas");
            TrabajadasTurno1 = lector.ToTimeSpan("TrabajadasTurno1");
            TrabajadasTurno2 = lector.ToTimeSpan("TrabajadasTurno2");
            TrabajadasTurno3 = lector.ToTimeSpan("TrabajadasTurno3");
            TrabajadasTurno4 = lector.ToTimeSpan("TrabajadasTurno4");
            Acumuladas = lector.ToTimeSpan("H_Acumuladas");
            Nocturnas = lector.ToTimeSpan("H_Nocturnas");
            Desayuno = lector.ToDecimal("Desayunos");
            Comida = lector.ToDecimal("Comidas");
            Cena = lector.ToDecimal("Cenas");
            PlusCena = lector.ToDecimal("PlusesCena");
        }


        public IEnumerable<SQLiteParameter> Parametros { get; }


        public IEnumerable<ISQLItem> Lista { get; }


        public bool HasList { get => false; }


        public void InicializarLista() { }


        public void AddItemToList(ISQLItem item) { }


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
