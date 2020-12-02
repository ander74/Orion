#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using Newtonsoft.Json;
using Orion.Interfaces;

namespace Orion.Models {

    public class ResumenCalendarios : NotifyBase, ISQLiteItem {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private int id;
        public int Id {
            get => id;
            set => SetValue(ref id, value);
        }


        private int matriculaConductor;
        public int MatriculaConductor {
            get => matriculaConductor;
            set => SetValue(ref matriculaConductor, value);
        }


        private DateTime fecha;
        public DateTime Fecha {
            get => fecha;
            set => SetValue(ref fecha, value);
        }


        private int diasOV;
        public int DiasOV {
            get => diasOV;
            set => SetValue(ref diasOV, value);
        }


        private int diasJD;
        public int DiasJD {
            get => diasJD;
            set => SetValue(ref diasJD, value);
        }


        private int diasFN;
        public int DiasFN {
            get => diasFN;
            set => SetValue(ref diasFN, value);
        }


        private int diasE;
        public int DiasE {
            get => diasE;
            set => SetValue(ref diasE, value);
        }


        private int diasDS;
        public int DiasDS {
            get => diasDS;
            set => SetValue(ref diasDS, value);
        }


        private int diasDC;
        public int DiasDC {
            get => diasDC;
            set => SetValue(ref diasDC, value);
        }


        private int diasF6;
        public int DiasF6 {
            get => diasF6;
            set => SetValue(ref diasF6, value);
        }


        private int diasDND;
        public int DiasDND {
            get => diasDND;
            set => SetValue(ref diasDND, value);
        }


        private int diasPER;
        public int DiasPER {
            get => diasPER;
            set => SetValue(ref diasPER, value);
        }


        private int diasEJD;
        public int DiasEJD {
            get => diasEJD;
            set => SetValue(ref diasEJD, value);
        }


        private int diasEFN;
        public int DiasEFN {
            get => diasEFN;
            set => SetValue(ref diasEFN, value);
        }


        private int diasOVJD;
        public int DiasOVJD {
            get => diasOVJD;
            set => SetValue(ref diasOVJD, value);
        }


        private int diasOVFN;
        public int DiasOVFN {
            get => diasOVFN;
            set => SetValue(ref diasOVFN, value);
        }


        private int diasF6DC;
        public int DiasF6DC {
            get => diasF6DC;
            set => SetValue(ref diasF6DC, value);
        }


        private int diasFOR;
        public int DiasFOR {
            get => diasFOR;
            set => SetValue(ref diasFOR, value);
        }


        private int diasOVA;
        public int DiasOVA {
            get => diasOVA;
            set => SetValue(ref diasOVA, value);
        }


        private int diasOVAJD;
        public int DiasOVAJD {
            get => diasOVAJD;
            set => SetValue(ref diasOVAJD, value);
        }


        private int diasOVAFN;
        public int DiasOVAFN {
            get => diasOVAFN;
            set => SetValue(ref diasOVAFN, value);
        }


        private int diasLAC;
        public int DiasLAC {
            get => diasLAC;
            set => SetValue(ref diasLAC, value);
        }


        private int diasCO;
        public int DiasCO {
            get => diasCO;
            set => SetValue(ref diasCO, value);
        }


        private int diasCE;
        public int DiasCE {
            get => diasCE;
            set => SetValue(ref diasCE, value);
        }


        private int diasTrabajo;
        public int DiasTrabajo {
            get => diasTrabajo;
            set => SetValue(ref diasTrabajo, value);
        }


        private int diasTrabajoJD;
        public int DiasTrabajoJD {
            get => diasTrabajoJD;
            set => SetValue(ref diasTrabajoJD, value);
        }


        private int diasActivo;
        public int DiasActivo {
            get => diasActivo;
            set => SetValue(ref diasActivo, value);
        }


        private int diasInactivo;
        public int DiasInactivo {
            get => diasInactivo;
            set => SetValue(ref diasInactivo, value);
        }


        private TimeSpan trabajadasConvenio;
        public TimeSpan TrabajadasConvenio {
            get => trabajadasConvenio;
            set => SetValue(ref trabajadasConvenio, value);
        }


        private TimeSpan acumuladas;
        public TimeSpan Acumuladas {
            get => acumuladas;
            set => SetValue(ref acumuladas, value);
        }


        private TimeSpan nocturnas;
        public TimeSpan Nocturnas {
            get => nocturnas;
            set => SetValue(ref nocturnas, value);
        }


        public int DiasTrabajoConvenio {
            get => DiasTrabajo + DiasE + DiasDC + DiasF6 + DiasDND + DiasPER + DiasF6DC + DiasFOR + DiasLAC;
        }

        public int DiasDescansoConvenio {
            get => DiasJD + DiasFN + DiasDS + DiasEJD + DiasEFN + DiasOVAJD + DiasOVAFN;
        }

        public int DescansosFindeConvenio {
            get => DiasFN + DiasEFN + DiasOVAFN;
        }

        public int DiasVacacionesConvenio {
            get => DiasOV + DiasOVJD + DiasOVFN;
        }

        public int DiasEnfermoConvenio {
            get => DiasE + DiasEJD + DiasEFN;
        }


        private decimal findesCompletos;
        public decimal FindesCompletos {
            get => findesCompletos;
            set => SetValue(ref findesCompletos, value);
        }


        // NO EN BASE DE DATOS (Por el momento)

        private int sabadosTrabajados;
        public int SabadosTrabajados {
            get => sabadosTrabajados;
            set => SetValue(ref sabadosTrabajados, value);
        }


        private int sabadosDescansados;
        public int SabadosDescansados {
            get => sabadosDescansados;
            set => SetValue(ref sabadosDescansados, value);
        }


        private int domingosTrabajados;
        public int DomingosTrabajados {
            get => domingosTrabajados;
            set => SetValue(ref domingosTrabajados, value);
        }


        private int domingosDescansados;
        public int DomingosDescansados {
            get => domingosDescansados;
            set => SetValue(ref domingosDescansados, value);
        }


        private int festivosTrabajados;
        public int FestivosTrabajados {
            get => festivosTrabajados;
            set => SetValue(ref festivosTrabajados, value);
        }


        private int festivosDescansados;
        public int FestivosDescansados {
            get => festivosDescansados;
            set => SetValue(ref festivosDescansados, value);
        }


        private int diasComite;
        public int DiasComite {
            get => diasComite;
            set => SetValue(ref diasComite, value);
        }


        private int diasComiteJD;
        public int DiasComiteJD {
            get => diasComiteJD;
            set => SetValue(ref diasComiteJD, value);
        }


        private int diasComiteDC;
        public int DiasComiteDC {
            get => diasComiteDC;
            set => SetValue(ref diasComiteDC, value);
        }


        private TimeSpan excesosJornada;
        public TimeSpan ExcesosJornada {
            get => excesosJornada;
            set => SetValue(ref excesosJornada, value);
        }


        private TimeSpan horasCobradas;
        public TimeSpan HorasCobradas {
            get => horasCobradas;
            set => SetValue(ref horasCobradas, value);
        }


        private TimeSpan otrasHoras;
        public TimeSpan OtrasHoras {
            get => otrasHoras;
            set => SetValue(ref otrasHoras, value);
        }


        private decimal plusSabado;
        public decimal PlusSabado {
            get => plusSabado;
            set => SetValue(ref plusSabado, value);
        }


        private decimal plusFestivo;
        public decimal PlusFestivo {
            get => plusFestivo;
            set => SetValue(ref plusFestivo, value);
        }


        private decimal plusNocturnidad;
        public decimal PlusNocturnidad {
            get => plusNocturnidad;
            set => SetValue(ref plusNocturnidad, value);
        }


        private decimal plusLimpieza;
        public decimal PlusLimpieza {
            get => plusLimpieza;
            set => SetValue(ref plusLimpieza, value);
        }


        private decimal variablesVacaciones;
        public decimal VariablesVacaciones {
            get => variablesVacaciones;
            set => SetValue(ref variablesVacaciones, value);
        }



        private decimal diasDeberiaTrabajo;
        public decimal DiasDeberiaTrabajo {
            get => diasDeberiaTrabajo;
            set => SetValue(ref diasDeberiaTrabajo, value);
        }


        private decimal diasDeberiaDescanso;
        public decimal DiasDeberiaDescanso {
            get => diasDeberiaDescanso;
            set => SetValue(ref diasDeberiaDescanso, value);
        }


        private decimal diasDeberiaVacaciones;
        public decimal DiasDeberiaVacaciones {
            get => diasDeberiaVacaciones;
            set => SetValue(ref diasDeberiaVacaciones, value);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region INTERFAZ ISQLITEITEM
        // ====================================================================================================

        public virtual void FromReader(DbDataReader lector) {
            //id = lector.ToInt32("_id");
            matriculaConductor = lector.ToInt32("MatriculaConductor");
            //fecha = lector.ToDateTime("Fecha");
            diasOV = lector.ToInt32("DiasOV");
            diasJD = lector.ToInt32("DiasJD");
            diasFN = lector.ToInt32("DiasFN");
            diasE = lector.ToInt32("DiasE");
            diasDS = lector.ToInt32("DiasDS");
            diasDC = lector.ToInt32("DiasDC");
            diasF6 = lector.ToInt32("DiasF6");
            diasDND = lector.ToInt32("DiasDND");
            diasPER = lector.ToInt32("DiasPER");
            diasEJD = lector.ToInt32("DiasEJD");
            diasEFN = lector.ToInt32("DiasEFN");
            diasOVJD = lector.ToInt32("DiasOVJD");
            diasOVFN = lector.ToInt32("DiasOVFN");
            diasF6DC = lector.ToInt32("DiasF6DC");
            diasFOR = lector.ToInt32("DiasFOR");
            diasOVA = lector.ToInt32("DiasOVA");
            diasOVAJD = lector.ToInt32("DiasOVAJD");
            diasOVAFN = lector.ToInt32("DiasOVAFN");
            diasLAC = lector.ToInt32("DiasLAC");
            diasCO = lector.ToInt32("DiasCO");
            diasCE = lector.ToInt32("DiasCE");
            diasTrabajo = lector.ToInt32("DiasTrabajo");
            diasTrabajoJD = lector.ToInt32("DiasTrabajoJD");
            diasActivo = lector.ToInt32("DiasActivo");
            diasInactivo = lector.ToInt32("DiasInactivo");
            trabajadasConvenio = lector.ToTimeSpan("TrabajadasConvenio");
            acumuladas = lector.ToTimeSpan("Acumuladas");
            nocturnas = lector.ToTimeSpan("Nocturnas");
            findesCompletos = lector.ToDecimal("FindesCompletos");

            Nuevo = false;
            Modificado = false;
        }


        [JsonIgnore]
        public virtual IEnumerable<SQLiteParameter> Parametros {
            get {
                var lista = new List<SQLiteParameter>();
                lista.Add(new SQLiteParameter("MatriculaConductor", MatriculaConductor));
                //lista.Add(new SQLiteParameter("Fecha", Fecha.ToString("yyyy-MM-dd")));
                lista.Add(new SQLiteParameter("DiasOV", DiasOV));
                lista.Add(new SQLiteParameter("DiasJD", DiasJD));
                lista.Add(new SQLiteParameter("DiasFN", DiasFN));
                lista.Add(new SQLiteParameter("DiasE", DiasE));
                lista.Add(new SQLiteParameter("DiasDS", DiasDS));
                lista.Add(new SQLiteParameter("DiasDC", DiasDC));
                lista.Add(new SQLiteParameter("DiasF6", DiasF6));
                lista.Add(new SQLiteParameter("DiasDND", DiasDND));
                lista.Add(new SQLiteParameter("DiasPER", DiasPER));
                lista.Add(new SQLiteParameter("DiasEJD", DiasEJD));
                lista.Add(new SQLiteParameter("DiasEFN", DiasEFN));
                lista.Add(new SQLiteParameter("DiasOVJD", DiasOVJD));
                lista.Add(new SQLiteParameter("DiasOVFN", DiasOVFN));
                lista.Add(new SQLiteParameter("DiasF6DC", DiasF6DC));
                lista.Add(new SQLiteParameter("DiasFOR", DiasFOR));
                lista.Add(new SQLiteParameter("DiasOVA", DiasOVA));
                lista.Add(new SQLiteParameter("DiasOVAJD", DiasOVAJD));
                lista.Add(new SQLiteParameter("DiasOVAFN", DiasOVAFN));
                lista.Add(new SQLiteParameter("DiasLAC", DiasLAC));
                lista.Add(new SQLiteParameter("DiasCO", DiasCO));
                lista.Add(new SQLiteParameter("DiasCE", DiasCE));
                lista.Add(new SQLiteParameter("DiasTrabajo", DiasTrabajo));
                lista.Add(new SQLiteParameter("DiasTrabajoJD", DiasTrabajoJD));
                lista.Add(new SQLiteParameter("DiasActivo", DiasActivo));
                lista.Add(new SQLiteParameter("DiasInactivo", DiasInactivo));
                lista.Add(new SQLiteParameter("TrabajadasConvenio", TrabajadasConvenio.Ticks));
                lista.Add(new SQLiteParameter("Acumuladas", Acumuladas.Ticks));
                lista.Add(new SQLiteParameter("Nocturnas", Nocturnas.Ticks));
                lista.Add(new SQLiteParameter("FindesCompletos", FindesCompletos.ToString("0.0000").Replace(",", ".")));
                return lista;
            }
        }


        [JsonIgnore]
        public virtual IEnumerable<ISQLiteItem> Lista { get => null; }


        [JsonIgnore]
        public virtual bool HasList { get => false; }


        public virtual void InicializarLista() { }


        public virtual void AddItemToList(ISQLiteItem item) { }


        [JsonIgnore]
        public virtual int ForeignId { get; set; }


        [JsonIgnore]
        public virtual string ForeignIdName { get => null; }


        [JsonIgnore]
        public virtual string OrderBy { get => $"MatriculaConductor ASC"; }


        [JsonIgnore]
        public virtual string TableName { get => "ResumenCalendarios"; }


        [JsonIgnore]
        [Obsolete("No se utiliza en el repositorio, ya que se extrae la consulta de los parámetros.")]
        public virtual string ComandoInsertar { get => null; }



        #endregion
        // ====================================================================================================

    }
}
