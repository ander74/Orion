#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using iText.Layout.Element;
using iText.Layout.Properties;
using Orion.Models;
using Orion.PdfExcel;

namespace Orion.ViewModels.PageViewModels {

    public class ResumenAnualPageVM : PageViewModelBase {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        // DESTINO DEL RESUMEN
        private const int DESTINO_UNO_SOLO = 0;
        private const int DESTINO_TODOS = 1;
        private const int DESTINO_INDEFINIDOS = 2;
        private const int DESTINO_EVENTUALES = 3;

        // FILTRADO DE FECHAS
        private const int FILTRO_AÑO = 0;
        private const int FILTRO_AÑO_POR_CONDUCTORES = 1;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static ResumenAnualPageVM instance;

        private ResumenAnualPageVM() { }

        public static ResumenAnualPageVM GetInstance() {
            if (instance == null) instance = new ResumenAnualPageVM();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        // Mapeado a los viewmodels que necesitemos usar.
        // ----------------------------------------------
        public ConductoresViewModel ConductoresVM => App.Global.ConductoresVM;
        public CalendariosViewModel CalendariosVM => App.Global.CalendariosVM;




        private PdfTableData tablaDatos;
        public PdfTableData TablaDatos {
            get => tablaDatos;
            set => SetValue(ref tablaDatos, value);
        }


        private int destinoResumen;
        public int DestinoResumen {
            get => destinoResumen;
            set => SetValue(ref destinoResumen, value);
        }


        private bool incluirTaquillas;
        public bool IncluirTaquillas {
            get => incluirTaquillas;
            set => SetValue(ref incluirTaquillas, value);
        }


        private Conductor conductorSeleccionado;
        public Conductor ConductorSeleccionado {
            get => conductorSeleccionado;
            set => SetValue(ref conductorSeleccionado, value);
        }


        private int filtradoFechas;
        public int Filtrado {
            get => filtradoFechas;
            set => SetValue(ref filtradoFechas, value);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES CHECKBOXES
        // ====================================================================================================


        //HORAS

        private bool incluirHorasTrabajadas;
        public bool IncluirHorasTrabajadas {
            get => incluirHorasTrabajadas;
            set => SetValue(ref incluirHorasTrabajadas, value);
        }


        private bool incluirHorasAcumuladas;
        public bool IncluirHorasAcumuladas {
            get => incluirHorasAcumuladas;
            set => SetValue(ref incluirHorasAcumuladas, value);
        }


        private bool incluirHorasNocturnas;
        public bool IncluirHorasNocturnas {
            get => incluirHorasNocturnas;
            set => SetValue(ref incluirHorasNocturnas, value);
        }


        private bool incluirHorasCobradas;
        public bool IncluirHorasCobradas {
            get => incluirHorasCobradas;
            set => SetValue(ref incluirHorasCobradas, value);
        }


        private bool incluirOtrasHoras;
        public bool IncluirOtrasHoras {
            get => incluirOtrasHoras;
            set => SetValue(ref incluirOtrasHoras, value);
        }


        private bool incluirExcesosJornada;
        public bool IncluirExcesosJornada {
            get => incluirExcesosJornada;
            set => SetValue(ref incluirExcesosJornada, value);
        }


        // FINDES

        private bool incluirSabadosTrabajados;
        public bool IncluirSabadosTrabajados {
            get => incluirSabadosTrabajados;
            set => SetValue(ref incluirSabadosTrabajados, value);
        }


        private bool incluirSabadosTrabajadosConvenio;
        public bool IncluirSabadosTrabajadosConvenio {
            get => incluirSabadosTrabajadosConvenio;
            set => SetValue(ref incluirSabadosTrabajadosConvenio, value);
        }


        private bool incluirSabadosDescansados;
        public bool IncluirSabadosDescansados {
            get => incluirSabadosDescansados;
            set => SetValue(ref incluirSabadosDescansados, value);
        }


        private bool incluirDomingosTrabajados;
        public bool IncluirDomingosTrabajados {
            get => incluirDomingosTrabajados;
            set => SetValue(ref incluirDomingosTrabajados, value);
        }


        private bool incluirDomingosTrabajadosConvenio;
        public bool IncluirDomingosTrabajadosConvenio {
            get => incluirDomingosTrabajadosConvenio;
            set => SetValue(ref incluirDomingosTrabajadosConvenio, value);
        }


        private bool incluirDomingosDescansados;
        public bool IncluirDomingosDescansados {
            get => incluirDomingosDescansados;
            set => SetValue(ref incluirDomingosDescansados, value);
        }


        private bool incluirFestivosTrabajados;
        public bool IncluirFestivosTrabajados {
            get => incluirFestivosTrabajados;
            set => SetValue(ref incluirFestivosTrabajados, value);
        }


        private bool incluirFestivosTrabajadosConvenio;
        public bool IncluirFestivosTrabajadosConvenio {
            get => incluirFestivosTrabajadosConvenio;
            set => SetValue(ref incluirFestivosTrabajadosConvenio, value);
        }


        private bool incluirFestivosDescansados;
        public bool IncluirFestivosDescansados {
            get => incluirFestivosDescansados;
            set => SetValue(ref incluirFestivosDescansados, value);
        }


        private bool incluirFindesCompletos;
        public bool IncluirFindesCompletos {
            get => incluirFindesCompletos;
            set => SetValue(ref incluirFindesCompletos, value);
        }


        // PLUSES TODO: Añadir los pluses que faltan.

        private bool incluirPlusSabado;
        public bool IncluirPlusSabado {
            get => incluirPlusSabado;
            set => SetValue(ref incluirPlusSabado, value);
        }


        private bool incluirPlusFestivo;
        public bool IncluirPlusFestivo {
            get => incluirPlusFestivo;
            set => SetValue(ref incluirPlusFestivo, value);
        }


        private bool incluirPlusNocturnidad;
        public bool IncluirPlusNocturnidad {
            get => incluirPlusNocturnidad;
            set => SetValue(ref incluirPlusNocturnidad, value);
        }


        private bool incluirPlusLimpieza;
        public bool IncluirPlusLimpieza {
            get => incluirPlusLimpieza;
            set => SetValue(ref incluirPlusLimpieza, value);
        }


        private bool incluirQuebrantoMoneda;
        public bool IncluirQuebrantoMoneda {
            get => incluirQuebrantoMoneda;
            set => SetValue(ref incluirQuebrantoMoneda, value);
        }


        private bool incluirVariablesVacaciones;
        public bool IncluirVariablesVacaciones {
            get => incluirVariablesVacaciones;
            set => SetValue(ref incluirVariablesVacaciones, value);
        }


        //DIAS

        private bool incluirDiasActivos;
        public bool IncluirDiasActivos {
            get => incluirDiasActivos;
            set => SetValue(ref incluirDiasActivos, value);
        }


        private bool incluirDiasInactivos;
        public bool IncluirDiasInactivos {
            get => incluirDiasInactivos;
            set => SetValue(ref incluirDiasInactivos, value);
        }


        private bool incluirDiasTrabajadosConvenio;
        public bool IncluirDiasTrabajadosConvenio {
            get => incluirDiasTrabajadosConvenio;
            set => SetValue(ref incluirDiasTrabajadosConvenio, value);
        }


        private bool incluirDiasTrabajados;
        public bool IncluirDiasTrabajados {
            get => incluirDiasTrabajados;
            set => SetValue(ref incluirDiasTrabajados, value);
        }


        private bool incluirDiasDescansados;
        public bool IncluirDiasDescansados {
            get => incluirDiasDescansados;
            set => SetValue(ref incluirDiasDescansados, value);
        }


        private bool incluirDiasJD;
        public bool IncluirDiasJD {
            get => incluirDiasJD;
            set => SetValue(ref incluirDiasJD, value);
        }


        private bool incluirDiasFN;
        public bool IncluirDiasFN {
            get => incluirDiasFN;
            set => SetValue(ref incluirDiasFN, value);
        }


        private bool incluirDiasDS;
        public bool IncluirDiasDS {
            get => incluirDiasDS;
            set => SetValue(ref incluirDiasDS, value);
        }


        private bool incluirDiasTrabajoJD;
        public bool IncluirDiasTrabajoJD {
            get => incluirDiasTrabajoJD;
            set => SetValue(ref incluirDiasTrabajoJD, value);
        }


        private bool incluirDiasOV;
        public bool IncluirDiasOV {
            get => incluirDiasOV;
            set => SetValue(ref incluirDiasOV, value);
        }


        private bool incluirDiasDC;
        public bool IncluirDiasDC {
            get => incluirDiasDC;
            set => SetValue(ref incluirDiasDC, value);
        }


        private bool incluirDiasDND;
        public bool IncluirDiasDND {
            get => incluirDiasDND;
            set => SetValue(ref incluirDiasDND, value);
        }


        private bool incluirDiasE;
        public bool IncluirDiasE {
            get => incluirDiasE;
            set => SetValue(ref incluirDiasE, value);
        }


        private bool incluirDiasPER;
        public bool IncluirDiasPER {
            get => incluirDiasPER;
            set => SetValue(ref incluirDiasPER, value);
        }


        private bool incluirDiasF6;
        public bool IncluirDiasF6 {
            get => incluirDiasF6;
            set => SetValue(ref incluirDiasF6, value);
        }


        private bool incluirDiasComite;
        public bool IncluirDiasComite {
            get => incluirDiasComite;
            set => SetValue(ref incluirDiasComite, value);
        }


        private bool incluirDiasComiteJD;
        public bool IncluirDiasComiteJD {
            get => incluirDiasComiteJD;
            set => SetValue(ref incluirDiasComiteJD, value);
        }


        private bool incluirDiasComiteDC;
        public bool IncluirDiasComiteDC {
            get => incluirDiasComiteDC;
            set => SetValue(ref incluirDiasComiteDC, value);
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private ResumenCalendarios GenerarResumen(IEnumerable<Calendario> listaCalendarios, DateTime fecha) {
            var resumen = new ResumenCalendarios();
            resumen.Fecha = fecha;
            if (listaCalendarios == null || listaCalendarios.Count() == 0) return resumen;
            //resumen.Fecha = listaCalendarios.First().Fecha;
            resumen.MatriculaConductor = listaCalendarios.First().MatriculaConductor;
            
            foreach (var calendario in listaCalendarios) {
                // Días Activo
                resumen.DiasActivo += calendario.DiasActivo;
                // Días Inactivo
                resumen.DiasInactivo += calendario.DiasInactivo;
                // Días Trabajo
                resumen.DiasTrabajo += calendario.DiasTrabajo;
                // Días JD
                resumen.DiasJD += calendario.DiasJD;
                // Días DS
                resumen.DiasDS += calendario.DiasDS;
                // Días FN
                resumen.DiasFN += calendario.DiasFN;
                // Findes Completos
                resumen.FindesCompletos += calendario.FindesCompletos;
                // Sábados + Plus Sábado
                resumen.SabadosTrabajados += 
                    calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Saturday && d.Grafico > 0);
                resumen.SabadosDescansados += 
                    calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Saturday && d.EsDiaDeDescanso);
                var plusSabado = resumen.SabadosTrabajados * App.Global.Convenio.ImporteSabados;
                resumen.PlusSabado += plusSabado;
                // Sábados trabajados por convenio
                resumen.SabadosTrabajadosConvenio = calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Saturday && d.EsDiaTrabajoConvenio);
                // Domingos
                resumen.DomingosTrabajados +=
                    calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Sunday && d.Grafico > 0);
                resumen.DomingosDescansados +=
                    calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Sunday && d.EsDiaDeDescanso);
                // Domingos trabajados por convenio
                resumen.DomingosTrabajadosConvenio = calendario.ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Sunday && d.EsDiaTrabajoConvenio);
                // Festivos + Plus Festivos
                resumen.FestivosTrabajados +=
                    calendario.ListaDias.Count(d => CalendariosVM.EsFestivo(d.DiaFecha) && d.Grafico > 0);
                resumen.FestivosDescansados +=
                    calendario.ListaDias.Count(d => CalendariosVM.EsFestivo(d.DiaFecha) && d.EsDiaDeDescanso);
                var plusFestivo = (resumen.DomingosTrabajados + resumen.FestivosTrabajados) * App.Global.Convenio.ImporteFestivos;
                resumen.PlusFestivo += plusFestivo;
                // Festivos trabajados por convenio
                resumen.FestivosTrabajadosConvenio = calendario.ListaDias.Count(d => App.Global.FestivosVM.ListaFestivos.Any(f => f.Fecha == d.DiaFecha) && d.EsDiaTrabajoConvenio);
                // Días DC
                resumen.DiasDC += calendario.DiasDC;
                // Días DND
                resumen.DiasDND += calendario.DiasDND;
                // Días TrabajoJD
                resumen.DiasTrabajoJD += calendario.DiasTrabajoJD;
                // Días PER
                resumen.DiasPER += calendario.DiasPER;
                // Días F6
                resumen.DiasF6 += calendario.DiasF6;
                resumen.DiasF6DC += calendario.DiasF6DC;
                // Dias Comite
                resumen.DiasComite += calendario.DiasCO + calendario.DiasCE;
                // Comité en JD
                resumen.DiasComiteJD += 
                    calendario.ListaDias.Count(d => (d.Codigo == 1 || d.Codigo == 2) && d.EsDiaDeDescanso);
                // Comité en DC
                resumen.DiasComiteDC +=
                    calendario.ListaDias.Count(d => (d.Codigo == 1 || d.Codigo == 2) && d.Grafico == Incidencia.DC);
                // Días OV
                resumen.DiasOV += calendario.DiasOV;
                resumen.DiasOVFN += calendario.DiasOVFN;
                resumen.DiasOVJD += calendario.DiasOVJD;
                // Días E
                resumen.DiasE += calendario.DiasE;
                resumen.DiasEJD += calendario.DiasEJD;
                resumen.DiasEFN += calendario.DiasEFN;
                // Días OVA
                resumen.DiasOVA += calendario.DiasOVA;
                resumen.DiasOVAJD += calendario.DiasOVAJD;
                resumen.DiasOVAFN += calendario.DiasOVAFN;
                // Trabajadas
                resumen.TrabajadasConvenio += calendario.TrabajadasConvenio;
                // Acumuladas
                resumen.Acumuladas += calendario.Acumuladas;
                // Excesos Jornada
                resumen.ExcesosJornada += new TimeSpan(calendario.ListaDias.Sum(d => d.ExcesoJornada.Ticks));
                // Nocturnas
                resumen.Nocturnas += calendario.Nocturnas;
                // Horas Cobradas y Otras Horas
                var conductor = ConductoresVM.ListaConductores.FirstOrDefault(c => c.Matricula == calendario.MatriculaConductor);
                if (conductor != null) {
                    resumen.HorasCobradas = new TimeSpan(conductor.ListaRegulaciones
                        .Where(r => r.Codigo == 1 && r.Fecha.Year == calendario.Fecha.Year && r.Fecha.Month == calendario.Fecha.Month)
                        .Sum(h => h.Horas.Ticks));
                    resumen.OtrasHoras = new TimeSpan(conductor.ListaRegulaciones
                        .Where(r => r.Codigo != 1 && r.Fecha.Year == calendario.Fecha.Year && r.Fecha.Month == calendario.Fecha.Month)
                        .Sum(h => h.Horas.Ticks));
                }
                // Plus Nocturnidad
                var plusNocturnidad =
                    calendario.ListaDias.Count(d => (d.TurnoAlt.HasValue && d.TurnoAlt == 3) || d.Turno == 3) * 
                    App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).PlusNocturnidad;
                resumen.PlusNocturnidad += plusNocturnidad;
                // Plus Limpieza
                var plusLimpieza = 0m;
                if (App.Global.PorCentro.PagarLimpiezas) {
                    if (conductor?.Indefinido ?? false) {
                        var dias = App.Global.PorCentro.NumeroLimpiezas;
                        dias -= (calendario.ListaDias.Count(d => d.Grafico == Incidencia.E || 
                                                                 d.Grafico == Incidencia.DND || 
                                                                 d.Grafico == Incidencia.F6 ||
                                                                 d.Grafico == Incidencia.F6_DC ||
                                                                 d.Grafico == Incidencia.PER));
                        if (dias > 0) plusLimpieza = dias * App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).PlusLimpieza;
                    } else {
                        var dias = calendario.ListaDias.Count(d => d.Grafico > 0 || d.Grafico == Incidencia.DC);
                        if (dias > 21) dias = 21;
                        if (dias > 0) plusLimpieza = dias * App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).PlusLimpieza;
                    }
                } else {
                    plusLimpieza =
                        calendario.ListaDias.Count(d => d.Limpieza == null) * App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).PlusLimpieza / 2m +
                        calendario.ListaDias.Count(d => d.Limpieza == true) * App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).PlusLimpieza;
                }
                resumen.PlusLimpieza += plusLimpieza;

                // Cálculos Deberia (REALES, COGIDOS DEL EXCEL ANUAL
                var diasMes = DateTime.DaysInMonth(calendario.Fecha.Year, calendario.Fecha.Month);
                var descansosMes = calendario.ListaDias.Count(c => c.DiaFecha.DayOfWeek == DayOfWeek.Saturday || c.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                descansosMes += App.Global.FestivosVM.ListaFestivos.Count(f => f.Fecha.Month == calendario.Fecha.Month);
                decimal vacacionesPertenecen = App.Global.Convenio.VacacionesAnuales / 12m;
                decimal descansosPertenecen = (diasMes - vacacionesPertenecen) * descansosMes / diasMes;
                // Días Deberia Descanso
                //resumen.DiasDeberiaDescanso += calendario.DiasActivo * diasFindeFestivos / diasMes;
                decimal deberiaDescanso = Math.Round(calendario.DiasActivo * descansosPertenecen / diasMes, 4);
                resumen.DiasDeberiaDescanso += deberiaDescanso;
                // Días Deberia Vacaciones
                //resumen.DiasDeberiaVacaciones += calendario.DiasActivo * vacacionesPorMes / diasMes;
                decimal deberiaVacaciones = Math.Round(calendario.DiasActivo * vacacionesPertenecen / diasMes, 4);
                resumen.DiasDeberiaVacaciones += deberiaVacaciones;
                // Días Debería Trabajo
                //resumen.DiasDeberiaTrabajo += Math.Round(calendario.DiasActivo * ATrabajarMes / diasMes, 4);
                decimal deberiaTrabajo = Math.Round(calendario.DiasActivo - deberiaDescanso - deberiaVacaciones, 4);
                resumen.DiasDeberiaTrabajo += deberiaTrabajo;

                // Quebranto de Moneda
                var importeQuebranto = (calendario.DiasTrabajo + calendario.DiasDC) * App.Global.OpcionesVM.GetPluses(calendario.Fecha.Year).QuebrantoMoneda;
                resumen.QuebrantoMoneda += importeQuebranto;
                // Variables Vacaciones.
                var importeValorar = plusSabado + plusFestivo + plusNocturnidad + plusLimpieza + importeQuebranto;
                if (conductor.Indefinido) {
                    resumen.VariablesVacaciones += Math.Round(importeValorar / 11, 4); //TODO: Comprobar que es a cuatro decimales y no a dos.
                } else {
                    resumen.VariablesVacaciones += Math.Round(importeValorar / 11 /31, 4) * resumen.DiasDeberiaVacaciones;
                }
            }
            return resumen;
        }


        private IEnumerable<ResumenCalendarios> CrearTablaConductor(int matriculaConductor) {
            List<ResumenCalendarios> listaResumen = new List<ResumenCalendarios>();
            var lista = App.Global.Repository.GetCalendariosConductor(CalendariosVM.FechaActual.Year, matriculaConductor);
            if (lista == null || lista.Count() == 0) return listaResumen;
            // Recorremos los meses de uno en uno.
            for (int mes = 1; mes <= 12; mes++) {
                var listaMes = lista.Where(d => d.Fecha.Month == mes);
                // Generamos el resumen del mes.
                var fecha = new DateTime(CalendariosVM.FechaActual.Year, mes, 1);
                var resumen = GenerarResumen(listaMes, fecha);
                listaResumen.Add(resumen);
            }
            return listaResumen;
        }


        private IEnumerable<ResumenCalendarios> CrearTablaAnual() {
            List<ResumenCalendarios> listaResumen = new List<ResumenCalendarios>();
            for (int mes = 1; mes <= 12; mes++) {
                var fecha = new DateTime(CalendariosVM.FechaActual.Year, mes, 1);
                var lista = App.Global.Repository.GetCalendarios(CalendariosVM.FechaActual.Year, mes);
                if (lista == null || lista.Count() == 0) {
                    var resumenVacio = new ResumenCalendarios();
                    resumenVacio.Fecha = fecha;
                    listaResumen.Add(resumenVacio);
                    continue;
                }
                // Filtramos si no hay que incluir taquillas.
                if (!IncluirTaquillas) lista = lista.Where(d => d.CategoriaConductor.ToUpper() == "C");
                // Filtramos si son eventuales o indefinidos o todos.
                switch (DestinoResumen) {
                    case DESTINO_INDEFINIDOS:
                        lista = lista.Where(d => d.ConductorIndefinido);
                        break;
                    case DESTINO_EVENTUALES:
                        lista = lista.Where(d => !d.ConductorIndefinido);
                        break;
                }
                var resumen = GenerarResumen(lista, fecha);
                listaResumen.Add(resumen);
            }
            return listaResumen;
        }


        private List<PdfCellInfo> AddDatosToPdfCellList(string[,] datos, int indice, string rowHeader) {
            var fila = new List<PdfCellInfo>();
            fila.Add(new PdfCellInfo(rowHeader) { IsBold = true });
            for (int i = 0; i < 12; i++) {
                var texto = datos[indice, i];
                fila.Add(new PdfCellInfo(datos[indice, i]));
            }
            return fila;
        }


        private string[,] GetDatos(IEnumerable<ResumenCalendarios> listaResumen) {
            var datos = new string[46, 12];
            foreach (var resumen in listaResumen) {
                datos[00, resumen.Fecha.Month - 1] = resumen.DiasActivo.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasActivo);
                datos[01, resumen.Fecha.Month - 1] = resumen.DiasInactivo.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasInactivo);
                datos[02, resumen.Fecha.Month - 1] = resumen.DiasTrabajo.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasTrabajo);
                datos[03, resumen.Fecha.Month - 1] = resumen.DiasJD.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasJD);
                datos[04, resumen.Fecha.Month - 1] = resumen.DiasDS.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDS);
                datos[05, resumen.Fecha.Month - 1] = resumen.DiasFN.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasFN);
                datos[06, resumen.Fecha.Month - 1] = resumen.FindesCompletos.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.FindesCompletos).ToString("0.00");
                datos[07, resumen.Fecha.Month - 1] = resumen.SabadosTrabajados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.SabadosTrabajados);
                datos[08, resumen.Fecha.Month - 1] = resumen.SabadosDescansados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.SabadosDescansados);
                datos[09, resumen.Fecha.Month - 1] = resumen.DomingosTrabajados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DomingosTrabajados);
                datos[10, resumen.Fecha.Month - 1] = resumen.DomingosDescansados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DomingosDescansados);
                datos[11, resumen.Fecha.Month - 1] = resumen.FestivosTrabajados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.FestivosTrabajados);
                datos[12, resumen.Fecha.Month - 1] = resumen.FestivosDescansados.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.FestivosDescansados);
                datos[13, resumen.Fecha.Month - 1] = resumen.DiasDC.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDC);
                datos[14, resumen.Fecha.Month - 1] = resumen.DiasDND.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDND);
                datos[15, resumen.Fecha.Month - 1] = resumen.DiasTrabajoJD.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasTrabajoJD);
                datos[16, resumen.Fecha.Month - 1] = resumen.DiasPER.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasPER);
                datos[17, resumen.Fecha.Month - 1] = resumen.DiasF6.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasF6);
                datos[18, resumen.Fecha.Month - 1] = resumen.DiasComite.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasComite);
                datos[19, resumen.Fecha.Month - 1] = resumen.DiasComiteJD.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasComiteJD);
                datos[20, resumen.Fecha.Month - 1] = resumen.DiasComiteDC.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasComiteDC);
                datos[21, resumen.Fecha.Month - 1] = resumen.DiasOV.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasOV);
                datos[22, resumen.Fecha.Month - 1] = resumen.DiasE.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasE);
                datos[23, resumen.Fecha.Month - 1] = resumen.TrabajadasConvenio.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.TrabajadasConvenio.Ticks)).ToHorasMinutos();
                datos[24, resumen.Fecha.Month - 1] = resumen.Acumuladas.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.Acumuladas.Ticks)).ToHorasMinutos();
                datos[25, resumen.Fecha.Month - 1] = resumen.ExcesosJornada.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.ExcesosJornada.Ticks)).ToHorasMinutos();
                datos[26, resumen.Fecha.Month - 1] = resumen.Nocturnas.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.Nocturnas.Ticks)).ToHorasMinutos();
                datos[27, resumen.Fecha.Month - 1] = resumen.HorasCobradas.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.HorasCobradas.Ticks)).ToHorasMinutos();
                datos[28, resumen.Fecha.Month - 1] = resumen.OtrasHoras.ToHorasMinutos() + "\n" + new TimeSpan(listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.OtrasHoras.Ticks)).ToHorasMinutos();
                datos[29, resumen.Fecha.Month - 1] = resumen.PlusSabado.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.PlusSabado).ToString("0.00");
                datos[30, resumen.Fecha.Month - 1] = resumen.PlusFestivo.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.PlusFestivo).ToString("0.00");
                datos[31, resumen.Fecha.Month - 1] = resumen.PlusNocturnidad.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.PlusNocturnidad).ToString("0.00");
                datos[32, resumen.Fecha.Month - 1] = resumen.PlusLimpieza.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.PlusLimpieza).ToString("0.00");
                datos[33, resumen.Fecha.Month - 1] = resumen.VariablesVacaciones.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.VariablesVacaciones).ToString("0.00");
                // Datos Debería para eventuales.
                datos[34, resumen.Fecha.Month - 1] = resumen.DiasDeberiaTrabajo.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDeberiaTrabajo).ToString("0.00");
                datos[35, resumen.Fecha.Month - 1] = resumen.DiasDeberiaDescanso.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDeberiaDescanso).ToString("0.00");
                datos[36, resumen.Fecha.Month - 1] = resumen.DiasDeberiaVacaciones.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDeberiaVacaciones).ToString("0.00");
                // Descansos Convenio
                datos[37, resumen.Fecha.Month - 1] = resumen.DiasDescansoConvenio.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasDescansoConvenio).ToString();
                // Días Trabajo Convenio
                datos[38, resumen.Fecha.Month - 1] = resumen.DiasTrabajoConvenio.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiasTrabajoConvenio).ToString();
                // Quebranto Moneda
                datos[39, resumen.Fecha.Month - 1] = resumen.QuebrantoMoneda.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.QuebrantoMoneda).ToString("0.00");
                // Sábados, domingos y festivos trabajados por convenio
                datos[40, resumen.Fecha.Month - 1] = resumen.SabadosTrabajadosConvenio.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.SabadosTrabajadosConvenio).ToString();
                datos[41, resumen.Fecha.Month - 1] = resumen.DomingosTrabajadosConvenio.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DomingosTrabajadosConvenio).ToString();
                datos[42, resumen.Fecha.Month - 1] = resumen.FestivosTrabajadosConvenio.ToString() + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.FestivosTrabajadosConvenio).ToString();
                // Diferencia Debería para eventuales
                datos[43, resumen.Fecha.Month - 1] = resumen.DiferenciaDeberiaTrabajo.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiferenciaDeberiaTrabajo).ToString("0.00");
                datos[44, resumen.Fecha.Month - 1] = resumen.DiferenciaDeberiaDescanso.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiferenciaDeberiaDescanso).ToString("0.00");
                datos[45, resumen.Fecha.Month - 1] = resumen.DiferenciaDeberiaVacaciones.ToString("0.00") + "\n" + listaResumen.Where(r => r.Fecha.Month <= resumen.Fecha.Month).Sum(r => r.DiferenciaDeberiaVacaciones).ToString("0.00");
            }
            return datos;
        }


        private PdfTableData CrearTablaResumen(IEnumerable<ResumenCalendarios> listaResumen, string titulo, bool esEventual = false) {
            // Extraemos los datos
            var datos = GetDatos(listaResumen);
            // Crear la tabla.
            var tabla = new PdfTableData(titulo);
            tabla.ColumnWidths = new List<float> { 16, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f };
            tabla.AlternateRowsCount = 1; //TODO Usar dos filas si fuera necesario.
            tabla.LogoPath = App.Global.Configuracion.RutaLogoSindicato;
            tabla.TableTheme = PdfTableTheme.PURPURA;
            // Encabezados
            tabla.Headers = new List<PdfCellInfo> {
                new PdfCellInfo("Conceptos"),
                new PdfCellInfo("Enero"),
                new PdfCellInfo("Febrero"),
                new PdfCellInfo("Marzo"),
                new PdfCellInfo("Abril"),
                new PdfCellInfo("Mayo"),
                new PdfCellInfo("Junio"),
                new PdfCellInfo("Julio"),
                new PdfCellInfo("Agosto"),
                new PdfCellInfo("Septiembre"),
                new PdfCellInfo("Octubre"),
                new PdfCellInfo("Noviembre"),
                new PdfCellInfo("Diciembre"),
            };

            // Ir concepto a concepto rellenando una fila si está marcada su casilla correspondiente.
            var filas = new List<List<PdfCellInfo>>();
            if (IncluirDiasActivos) filas.Add(AddDatosToPdfCellList(datos, 00, "Días activo"));
            if (IncluirDiasInactivos) filas.Add(AddDatosToPdfCellList(datos, 01, "Días inactivo"));
            if (IncluirDiasTrabajadosConvenio) {
                filas.Add(AddDatosToPdfCellList(datos, 38, "Días trabajo convenio\n(Trab+E+DC+F6+DND+PER+FOR+LAC)"));
                if (esEventual) {
                    filas.Add(AddDatosToPdfCellList(datos, 34, "Debería Trabajar"));
                    filas.Add(AddDatosToPdfCellList(datos, 43, "Diferencia Debería Trabajar"));
                }
            }
            if (IncluirDiasTrabajados) filas.Add(AddDatosToPdfCellList(datos, 02, "Días trabajo"));
            if (IncluirDiasDescansados) {
                filas.Add(AddDatosToPdfCellList(datos, 37, "Días Descanso Convenio\n(JD+FN+DS)"));
                if (esEventual) {
                    filas.Add(AddDatosToPdfCellList(datos, 35, "Debería Descansar"));
                    filas.Add(AddDatosToPdfCellList(datos, 44, "Diferencia Debería Descansar"));
                }
            }
            if (IncluirDiasJD) filas.Add(AddDatosToPdfCellList(datos, 03, "Descansos (JD)"));
            if (IncluirDiasDS) filas.Add(AddDatosToPdfCellList(datos, 04, "Descansos sueltos (DS) "));
            if (IncluirDiasFN) filas.Add(AddDatosToPdfCellList(datos, 05, "Descansos fin semana (FN"));
            if (IncluirFindesCompletos) filas.Add(AddDatosToPdfCellList(datos, 06, "Findes completos"));
            if (IncluirSabadosTrabajados) filas.Add(AddDatosToPdfCellList(datos, 07, "Sáb. trabajados"));
            if (incluirSabadosTrabajadosConvenio) filas.Add(AddDatosToPdfCellList(datos, 40, "Sáb. Trab. Convenio"));
            if (IncluirSabadosDescansados) filas.Add(AddDatosToPdfCellList(datos, 08, "Sáb. descansados"));
            if (IncluirDomingosTrabajados) filas.Add(AddDatosToPdfCellList(datos, 09, "Dom. trabajados"));
            if (IncluirDomingosTrabajadosConvenio) filas.Add(AddDatosToPdfCellList(datos, 41, "Dom. Trab. Convenio"));
            if (IncluirDomingosDescansados) filas.Add(AddDatosToPdfCellList(datos, 10, "Dom. descansados"));
            if (IncluirFestivosTrabajados) filas.Add(AddDatosToPdfCellList(datos, 11, "Fest. trabajados"));
            if (incluirFestivosTrabajadosConvenio) filas.Add(AddDatosToPdfCellList(datos, 42, "Fest. Trab. Convenio"));
            if (IncluirFestivosDescansados) filas.Add(AddDatosToPdfCellList(datos, 12, "Fest. descansados"));
            if (IncluirDiasDC) filas.Add(AddDatosToPdfCellList(datos, 13, "Desc. compensatorios"));
            if (IncluirDiasDND) filas.Add(AddDatosToPdfCellList(datos, 14, "Desc. no disfrutados"));
            if (IncluirDiasTrabajoJD) filas.Add(AddDatosToPdfCellList(datos, 15, "Días trabajo en JD"));
            if (IncluirDiasPER) filas.Add(AddDatosToPdfCellList(datos, 16, "Días permiso"));
            if (IncluirDiasF6) filas.Add(AddDatosToPdfCellList(datos, 17, "Días de horas (F6)"));
            if (IncluirDiasComite) filas.Add(AddDatosToPdfCellList(datos, 18, "Días comité"));
            if (IncluirDiasComiteJD) filas.Add(AddDatosToPdfCellList(datos, 19, "Días comité en JD"));
            if (IncluirDiasComiteDC) filas.Add(AddDatosToPdfCellList(datos, 20, "Días comité en DC"));
            if (IncluirDiasOV) {
                filas.Add(AddDatosToPdfCellList(datos, 21, "Días vacaciones (OV)"));
                if (esEventual) {
                    filas.Add(AddDatosToPdfCellList(datos, 36, "Debería Vacaciones"));
                    filas.Add(AddDatosToPdfCellList(datos, 45, "Diferencia Debería Vacaciones"));
                }
            }
            if (IncluirDiasE) filas.Add(AddDatosToPdfCellList(datos, 22, "Días enfermo (E)"));
            if (IncluirHorasTrabajadas) filas.Add(AddDatosToPdfCellList(datos, 23, "Horas trabajadas"));
            if (IncluirHorasAcumuladas) filas.Add(AddDatosToPdfCellList(datos, 24, "Horas acumuladas"));
            if (IncluirExcesosJornada) filas.Add(AddDatosToPdfCellList(datos, 25, "Excesos jornada"));
            if (IncluirHorasNocturnas) filas.Add(AddDatosToPdfCellList(datos, 26, "Horas nocturnas"));
            if (IncluirHorasCobradas) filas.Add(AddDatosToPdfCellList(datos, 27, "Horas cobradas"));
            if (IncluirOtrasHoras) filas.Add(AddDatosToPdfCellList(datos, 28, "Otras horas"));
            if (IncluirPlusSabado) filas.Add(AddDatosToPdfCellList(datos, 29, "Plus sábado"));
            if (IncluirPlusFestivo) filas.Add(AddDatosToPdfCellList(datos, 30, "Plus festivo"));
            if (IncluirPlusNocturnidad) filas.Add(AddDatosToPdfCellList(datos, 31, "Plus nocturnidad"));
            if (IncluirPlusLimpieza) filas.Add(AddDatosToPdfCellList(datos, 32, "Plus limpieza"));
            if (IncluirQuebrantoMoneda) filas.Add(AddDatosToPdfCellList(datos, 39, "Quebranto de moneda"));
            if (IncluirVariablesVacaciones) filas.Add(AddDatosToPdfCellList(datos, 33, "Variables vacaciones"));
            tabla.Data = filas;
            return tabla;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================


        #region COMANDO CREAR PDF

        // Comando
        private ICommand cmdCrearPDF;
        public ICommand CmdCrearPDF {
            get {
                if (cmdCrearPDF == null) cmdCrearPDF = new RelayCommand(p => CrearPDF());
                return cmdCrearPDF;
            }
        }
        private void CrearPDF() {

            // Definimos el PDF
            var carpetaInformes = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Informes\\{App.Global.CentroActual}");
            var ruta = Path.Combine(carpetaInformes, $"Calendarios\\Resumenes");
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            var nombreArchivo = string.Empty;
            var archivo = string.Empty;

            if (DestinoResumen == DESTINO_UNO_SOLO) {
                if (ConductorSeleccionado == null) return;
                var resumen = CrearTablaConductor(ConductorSeleccionado.Matricula);
                var titulo = $"RESUMEN ANUAL {CalendariosVM.FechaActual.Year}\n{ConductorSeleccionado.MatriculaApellidos}";
                var tabla = CrearTablaResumen(resumen, titulo, !ConductorSeleccionado.Indefinido);
                nombreArchivo = $"{CalendariosVM.FechaActual.Year}-Resumen Anual - {ConductorSeleccionado.Matricula}";
                archivo = Path.Combine(ruta, $"{nombreArchivo}.pdf");
                var docPdf = App.Global.Informes.GetNuevoPdf(archivo, true);
                docPdf.GetPdfDocument().GetDocumentInfo().SetTitle("Resumen Anual");
                docPdf.GetPdfDocument().GetDocumentInfo().SetSubject($"{nombreArchivo}");
                docPdf.SetMargins(25, 25, 25, 25);
                docPdf.Add(PdfExcelHelper.GetInstance().GetPdfTable(tabla));
                docPdf.Close();
            } else {
                if (Filtrado == FILTRO_AÑO) {
                    var resumen = CrearTablaAnual();
                    var titulo = $"RESUMEN ANUAL {CalendariosVM.FechaActual.Year}\n";
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year}-Resumen Anual";
                    titulo += IncluirTaquillas ? " CONDUCCIÓN + TAQUILLAS" : " CONDUCCIÓN";
                    nombreArchivo += IncluirTaquillas ? " Con Taquillas" : " Conduccion";
                    switch (DestinoResumen) {
                        case DESTINO_INDEFINIDOS:
                            titulo += " (INDEFINIDOS)";
                            nombreArchivo += "-Indefinidos";
                            break;
                        case DESTINO_EVENTUALES:
                            titulo += " (EVENTUALES)";
                            nombreArchivo += "-Eventuales";
                            break;
                    }
                    var tabla = CrearTablaResumen(resumen, titulo, DestinoResumen == DESTINO_EVENTUALES);
                    archivo = Path.Combine(ruta, $"{nombreArchivo}.pdf");
                    var docPdf = App.Global.Informes.GetNuevoPdf(archivo, true);
                    docPdf.GetPdfDocument().GetDocumentInfo().SetTitle("Resumen Anual");
                    docPdf.GetPdfDocument().GetDocumentInfo().SetSubject($"{nombreArchivo}");
                    docPdf.SetMargins(25, 25, 25, 25);
                    docPdf.Add(PdfExcelHelper.GetInstance().GetPdfTable(tabla));
                    docPdf.Close();
                } else {
                    var listaConductores = ConductoresVM.ListaConductores.ToList();
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year}-Resumen Anual Individual";
                    nombreArchivo += IncluirTaquillas ? " Con Taquillas" : " Conduccion";
                    if (!IncluirTaquillas) {
                        listaConductores = listaConductores.Where(c => c.Categoria.ToUpper() == "C").ToList();
                    }
                    switch (DestinoResumen) {
                        case DESTINO_INDEFINIDOS:
                            listaConductores = listaConductores.Where(c => c.Indefinido).ToList();
                            nombreArchivo += "-Indefinidos";
                            break;
                        case DESTINO_EVENTUALES:
                            listaConductores = listaConductores.Where(c => !c.Indefinido).ToList();
                            nombreArchivo += "-Eventuales";
                            break;
                    }
                    archivo = Path.Combine(ruta, $"{nombreArchivo}.pdf");
                    var docPdf = App.Global.Informes.GetNuevoPdf(archivo, true);
                    docPdf.GetPdfDocument().GetDocumentInfo().SetTitle("Resumen Anual");
                    docPdf.GetPdfDocument().GetDocumentInfo().SetSubject($"{nombreArchivo}");
                    docPdf.SetMargins(25, 25, 25, 25);
                    foreach (var conductor in listaConductores) {
                        var resumen = CrearTablaConductor(conductor.Matricula);
                        var titulo = $"RESUMEN ANUAL {CalendariosVM.FechaActual.Year}\n{conductor.MatriculaApellidos}";
                        var tabla = CrearTablaResumen(resumen, titulo, !conductor.Indefinido);
                        docPdf.Add(PdfExcelHelper.GetInstance().GetPdfTable(tabla));
                        docPdf.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    }
                    docPdf.Close();
                }
            }
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(archivo);

        }
        #endregion



        #endregion
        // ====================================================================================================


    }
}
