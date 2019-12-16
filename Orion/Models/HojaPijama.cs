#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Orion.Convertidores;
using Orion.DataModels;
using Orion.Servicios;

namespace Orion.Models {

    [Obsolete("Usar la HojaPijama del espacio de nombres Pijama.")]
    public class HojaPijama : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private IMensajes mensajes;
        private static ConvertidorNumeroGraficoPijama cnvNumGraficoPijama = new ConvertidorNumeroGraficoPijama();
        private static ConvertidorHora cnvHora = new ConvertidorHora();
        private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
        private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
        private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();

        #endregion


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================
        public HojaPijama(Calendario calendario, IMensajes _mensajes) {
            mensajes = _mensajes;
            CalendarioPijama = calendario;//Obsoleto. No se utiliza.
                                          // Definimos la fecha.
            Fecha = calendario.Fecha;
            try {
                // Extraemos el trabajador.
                //Trabajador = BdConductores.GetConductor(calendario.IdConductor);
                Trabajador = App.Global.ConductoresVM.GetConductor(calendario.IdConductor);
                if (Trabajador != null) {
                    // Extraemos la lista de los días del calendario.
                    ListaDias = BdPijamas.GetDiasPijama(Fecha, calendario.ListaDias, Trabajador.ReduccionJornada);//TODO: Esto es uno.
                    // Extraemos la lista de festivos del mes del calendario.
                    //ListaFestivos = BdFestivos.GetFestivosPorMes(Fecha.Year, Fecha.Month).ToList();

                    // Extraemos las horas hasta el mes actual
                    HastaMesActual = new HorasHojaPijamaHastaMes(Fecha.Year, Fecha.Month, Trabajador.Id);

                    // Extraemos las horas
                    HorasCobradas = BdCalendarios.GetHorasCobradasMes(Fecha.Year, Fecha.Month, Trabajador.Id);
                    HorasCobradas = new TimeSpan(HorasCobradas.Ticks * -1);
                    HorasCobradasAño = BdCalendarios.GetHorasCobradasAño(Fecha.Year, Fecha.Month, Trabajador.Id);
                    HorasCobradasAño = new TimeSpan(HorasCobradasAño.Ticks * -1);
                    HorasReguladasAño = BdCalendarios.GetHorasReguladasAño(Fecha.Year, Trabajador.Id);
                    HorasReguladasAño = new TimeSpan(HorasReguladasAño.Ticks * -1);

                    // Extraemos los días DC y DND
                    DiasDCAñoActual = BdCalendarios.GetDCDisfrutadosAño(Trabajador.Id, Fecha.Year, Fecha.Month);
                    DiasDNDAñoActual = BdCalendarios.GetDNDDisfrutadosAño(Trabajador.Id, Fecha.Year, Fecha.Month);
                    DCsReguladosAñoActual = BdCalendarios.GetDescansosReguladosAño(Fecha.Year, Trabajador.Id);

                    // Extramos las horas acumuladas del año anterior.
                    DateTime fechaanterior = Fecha.Month == 12 ? new DateTime(Fecha.Year, 11, 30) : new DateTime(Fecha.Year - 1, 11, 30);
                    HastaAñoAnterior = new HorasHojaPijamaHastaMes(fechaanterior.Year, fechaanterior.Month, Trabajador.Id);
                    //HastaAñoAnterior.ExcesoJornadaCobrada -= CalendarioPijama.ExcesoJornadaCobrada;//TODO: Comprobar

                    // Establecemos el tooltip de Otros Pluses
                    string tooltip = "";
                    if (PlusNocturnidad != 0) tooltip += $"Plus Nocturnidad = {PlusNocturnidad:0.00} €\n";
                    if (PlusViaje != 0) tooltip += $"Plus Viaje = {PlusViaje:0.00} €\n";
                    if (PlusNavidad != 0) tooltip += $"Plus Navidad = {PlusNavidad:0.00} €\n";
                    ToolTipOtrosPluses = tooltip;


                }
            } catch (Exception ex) {
                mensajes.VerError("HojaPijama.Constructor", ex);
            }

        }

        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region MÉTODOS ESTÁTICOS
        // ====================================================================================================

        #endregion


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================
        public string GetInformeFallos() {

            // Variables a utilizar
            string resultado = "";
            int diastrabajados = 0;         // Días trabajados seguidos.
            int primerdiatrabajo = 0;       // Primer día de trabajo de la sección.
            int diasdescansados = 0;        // Días descansados seguidos.
            int primerdiadescanso = 0;      // Primer día de descanso de la secccion.
            int diasinactivos = 0;          // Días inactivos seguidos.
            int descansossueltos = 0;       // Días de descanso suelto.
            int findestrabajados = 0;       // Fines de semana trabajados.
            bool sabadotrabajado = false;   // Indica si el último sábado se ha trabajado.
            bool sabadodescansado = false;  // Indica si el último sábado se ha descansado.

            // Evaluamos los días.
            foreach (DiaPijama dia in ListaDias) {

                // DÍA DE TRABAJO.
                if (dia.Grafico > 0) {
                    // Se áñade a días trabajados.
                    diastrabajados++;
                    // Si es el primer día trabajado...
                    if (diastrabajados == 1) {
                        // Marcamos el día como el primero.
                        primerdiatrabajo = dia.Dia;
                        // Si sólo hay un descanso anterior, se marca el fallo.
                        if (diasdescansados == 1 && dia.Dia > 2 && diasinactivos == 0)
                            resultado += String.Format("Día {0:00}: Descanso suelto no computado.\n", primerdiadescanso);

                    }
                    // Si hay más de seis días trabajados, se añade un fallo.
                    if (diastrabajados > 6) resultado += String.Format("Día {0:00}: Más de seis días de trabajo.\n", primerdiatrabajo);

                    // Reiniciamos los descansos y los días inactivos.
                    diasdescansados = 0;
                    diasinactivos = 0;
                    // Establecemos el final anterior al final de hoy.
                    //if (dia.Final.HasValue) finalanterior = new TimeSpan(dia.Final.Value.Ticks);
                }


                // DÍA NO TRABAJADO
                if (dia.Grafico == -2 || dia.Grafico == -3) {
                    // Se añade a días de descanso.
                    diasdescansados++;
                    // Si es el primero, se marca como primer día descansado.
                    if (diasdescansados == 1) primerdiadescanso = dia.Dia;
                    // Reiniciamos los días trabajados, inactivos y borramos el final anterior.
                    diastrabajados = 0;
                    diasinactivos = 0;
                    //finalanterior = null;
                }


                // DESCANSO SUELTO
                if (dia.Grafico == -5) {
                    // Lo añadimos a descansos sueltos.
                    descansossueltos++;
                    // Si hay más de dos descansos sueltos, se añade el fallo.
                    if (descansossueltos > 2) resultado += String.Format("Día {0:00}: Más de dos descansos sueltos en el mes.\n", dia.Dia);
                    // Reiniciamos los días trabajados, inactivos y borramos el final anterior.
                    diastrabajados = 0;
                    diasinactivos = 0;
                    //finalanterior = null;
                }


                // DÍA INACTIVO
                if (dia.Grafico == 0) {
                    // Añadimos el día a los días inactivos.
                    diasinactivos++;
                    // Reiniciamos los días trabajados, descansados y borramos el final anterior.
                    diastrabajados = 0;
                    diasdescansados = 0;
                    //finalanterior = null;
                    // Borramos los fines de semana trabajados.
                    findestrabajados = 0;
                }



                // EXCESO DE HORAS TRABAJADAS EN LABORABLES Y FESTIVOS
                if (dia.Fecha.DayOfWeek != DayOfWeek.Saturday && dia.Fecha.DayOfWeek != DayOfWeek.Sunday) {
                    if (App.Global.FestivosVM.ListaFestivos.Count(x => x.Fecha.Ticks == dia.Fecha.Ticks) > 0) {
                        if (dia.Horas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en festivo.\n";
                    } else {
                        if (dia.Horas > App.Global.Convenio.MaxHorasLaborables) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en laborable.\n";
                    }
                }

                // CONTROL DE GRÁFICOS PARTIDOS
                if (dia.Turno == 4 && dia.Inicio.HasValue && dia.Final.HasValue) {
                    TimeSpan horafinal;
                    horafinal = dia.Final.Value;
                    if (horafinal < dia.Inicio.Value) horafinal = horafinal.Add(new TimeSpan(1, 0, 0, 0));
                    if (horafinal.TotalHours - dia.Inicio.Value.TotalHours > App.Global.Convenio.MaxHorasTotalGraficoPartido.TotalHours) {
                        resultado += $"Día {dia.Dia:00}: Exceso de horas totales en gráfico partido.\n";
                    }
                    if (dia.InicioPartido.HasValue && dia.FinalPartido.HasValue) {
                        if (dia.FinalPartido.Value.TotalHours - dia.InicioPartido.Value.TotalHours > App.Global.Convenio.MaxHorasParticionGraficoPartido.TotalHours) {
                            resultado += $"Día {dia.Dia:00}: Exceso de tiempo de partición en gráfico partido.\n";
                        }
                    }
                }


                // FINES DE SEMANA TRABAJADOS Y EXCESO HORAS EN FIN DE SEMANA

                // Si un sabado se trabaja, se añade el fin de semana a trabajados y se marca el sábado.
                if (dia.Fecha.DayOfWeek == DayOfWeek.Saturday && dia.Grafico > 0) {
                    findestrabajados++;
                    sabadotrabajado = true;
                    sabadodescansado = false;
                    if (dia.Horas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en sábado.\n";
                }
                // Si un sábado se descansa, se marca como descansado.
                if (dia.Fecha.DayOfWeek == DayOfWeek.Saturday && dia.Grafico <= 0) {
                    sabadotrabajado = false;
                    sabadodescansado = true;
                }
                // Si un domingo se trabaja...
                if (dia.Fecha.DayOfWeek == DayOfWeek.Sunday && dia.Grafico > 0) {
                    //... Si no se trabajo el sábado se añade el fin de semana a trabajados.
                    if (!sabadotrabajado) {
                        findestrabajados++;
                    }
                    sabadotrabajado = false;
                    if (dia.Horas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en domingo.\n";
                }
                // Si un domingo se descansa...
                if (dia.Fecha.DayOfWeek == DayOfWeek.Sunday && dia.Grafico <= 0) {
                    //... Si también se descansó el sábado, se ponen a cero los fines de semanas trabajados.
                    if (sabadodescansado) {
                        findestrabajados = 0;
                    }
                    sabadodescansado = false;
                }

                // Si hay más de dos fines de semana trabajados seguidos, se añade el fallo.
                if (dia.Fecha.DayOfWeek == DayOfWeek.Sunday && findestrabajados > 2)
                    resultado += String.Format("Día {0:00}: Más de dos fines de semana trabajados seguidos.\n", dia.Dia);

            }

            return resultado;
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        private DateTime _fecha;
        public DateTime Fecha {
            get { return _fecha; }
            set {
                if (_fecha != value) {
                    _fecha = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TextoMesActual));
                }
            }
        }


        public string TextoMesActual {
            get {
                return String.Format("{0:MMMM}", Fecha).ToUpper();
            }
        }


        private List<DiaPijama> _listadias;
        public List<DiaPijama> ListaDias {
            get { return _listadias; }
            set {
                if (_listadias != value) {
                    _listadias = value;
                    PropiedadCambiada("");
                }
            }
        }


        //private List<Festivo> _listafestivos;
        //public List<Festivo> ListaFestivos {
        //	get { return _listafestivos; }
        //	set {
        //		if (_listafestivos != value) {
        //			_listafestivos = value;
        //			PropiedadCambiada();
        //		}
        //	}
        //}


        private Conductor _trabajador;
        public Conductor Trabajador {
            get { return _trabajador; }
            set {
                if (_trabajador != value) {
                    _trabajador = value;
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(TextoTrabajador));
                    PropiedadCambiada(nameof(NoEsFijo));
                    PropiedadCambiada(nameof(TotalHorasAcumuladas));
                    PropiedadCambiada(nameof(DCsPendientesHastaMes));
                }
            }
        }


        private Calendario _calendariopijama;
        public Calendario CalendarioPijama {
            get { return _calendariopijama; }
            set {
                if (_calendariopijama != value) {
                    _calendariopijama = value;
                    PropiedadCambiada();
                }
            }
        }


        //public TimeSpan ExcesoJornadaCobrada {
        //	get { return CalendarioPijama.ExcesoJornadaCobrada; }
        //	set {
        //		if (CalendarioPijama.ExcesoJornadaCobrada != value) {
        //			CalendarioPijama.ExcesoJornadaCobrada = value;
        //			Modificado = true;
        //			PropiedadCambiada();
        //			PropiedadCambiada(nameof(ExcesoJornadaPendiente));
        //			PropiedadCambiada(nameof(TotalHorasAcumuladas));
        //		}
        //	}
        //}


        public string TextoTrabajador {
            get {
                return String.Format("{0:000}: {1}", Trabajador.Id, Trabajador.Apellidos);
            }
        }


        public bool NoEsFijo {
            get { return !Trabajador.Indefinido; }
        }


        public TimeSpan ExcesoJornada {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Where(x => !x.BloquearExcesoJornada).Sum(x => (x.ExcesoJornada.Ticks)));
            }
        }


        //public int Descuadre {
        //	get {
        //		if (ListaDias == null) return 0;
        //		return ListaDias.Where(x => !x.BloquearDescuadre).Sum(x => (x.Descuadre));
        //	}
        //}


        public decimal FacturadoPaqueteria {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Sum(x => (x.FacturadoPaqueteria));
            }
        }


        public decimal Limpieza { //TODO: Ignorar esto, ya que el plus se añade en BdPijamas.
            get {
                if (ListaDias == null) return 0;
                int limpiezas = ListaDias.Count(x => x.Limpieza == true);
                int mediaslimpiezas = ListaDias.Count(x => x.Limpieza == null);
                return limpiezas + (mediaslimpiezas / 2m);
            }
        }


        private string _tooltipotrospluses;
        public string ToolTipOtrosPluses {
            get { return _tooltipotrospluses; }
            set {
                if (_tooltipotrospluses != value) {
                    _tooltipotrospluses = string.IsNullOrWhiteSpace(value) ? null : value;
                    PropiedadCambiada();
                }
            }
        }


        #endregion


        // ====================================================================================================
        #region PROPIEDADES HORAS
        // ====================================================================================================

        private HorasHojaPijamaHastaMes _hastamesactual;
        public HorasHojaPijamaHastaMes HastaMesActual {
            get { return _hastamesactual; }
            set {
                if (_hastamesactual != value) {
                    _hastamesactual = value;
                    PropiedadCambiada();
                }
            }
        }


        private HorasHojaPijamaHastaMes _horashastaañoanterior;
        public HorasHojaPijamaHastaMes HastaAñoAnterior {
            get { return _horashastaañoanterior; }
            set {
                if (_horashastaañoanterior != value) {
                    _horashastaañoanterior = value;
                    PropiedadCambiada();
                }
            }
        }


        public TimeSpan HorasTrabajadas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.Horas.Ticks)));
            }
        }


        public TimeSpan HorasAcumuladas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.Acumuladas.Ticks)));
            }
        }


        public TimeSpan HorasNocturnas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.Nocturnas.Ticks)));
            }
        }


        private TimeSpan _horascobradas;
        public TimeSpan HorasCobradas {
            get { return _horascobradas; }
            set {
                if (_horascobradas != value) {
                    _horascobradas = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _horascobradasaño;
        public TimeSpan HorasCobradasAño {
            get { return _horascobradasaño; }
            set {
                if (_horascobradasaño != value) {
                    _horascobradasaño = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan _horasreguladasaño;
        public TimeSpan HorasReguladasAño {
            get { return _horasreguladasaño; }
            set {
                if (_horasreguladasaño != value) {
                    _horasreguladasaño = value;
                    PropiedadCambiada();
                }
            }
        }


        public TimeSpan TotalHorasHastaAñoAnterior {
            get {
                if (Trabajador == null) return TimeSpan.Zero;
                return HastaAñoAnterior.Acumuladas + HastaAñoAnterior.Reguladas + Trabajador.Acumuladas -
                       (new TimeSpan(App.Global.Convenio.JornadaMedia.Ticks * HastaAñoAnterior.DiasF6)) - HorasReguladasAño;
            }
        }


        public TimeSpan TotalHorasAcumuladas {
            get {
                if (Trabajador == null) return TimeSpan.Zero;
                return HastaMesActual.Acumuladas + HastaMesActual.Reguladas + Trabajador.Acumuladas -
                       (new TimeSpan(App.Global.Convenio.JornadaMedia.Ticks * HastaMesActual.DiasF6)) - TotalHorasHastaAñoAnterior;
            }
        }


        public TimeSpan TotalHorasAñoActual {
            get {
                if (Trabajador == null) return TimeSpan.Zero;
                return TotalHorasAcumuladas - TotalHorasHastaAñoAnterior;
            }
        }


        //public TimeSpan ExcesoJornadaPendiente {
        //	get {
        //		if (Trabajador == null) return TimeSpan.Zero;
        //		return HastaMesActual.ExcesoJornadaPendiente;
        //	}
        //}

        //public int DescuadrePendiente {
        //	get {
        //		if (Trabajador == null) return 0;
        //		return HastaMesActual.DescuadrePendiente;
        //	}
        //}



        #endregion


        // ====================================================================================================
        #region PROPIEDADES DIAS
        // ====================================================================================================

        public int DiasF6 {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -7);
            }
        }


        public int DiasComite {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public int DiasComiteEnJD {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3 || d.Grafico == -5) && (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public int DiasComiteEnDC {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -6) && (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public int DiasDND {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -8);
            }
        }


        private int _diasdndañoactual;
        public int DiasDNDAñoActual {
            get { return _diasdndañoactual; }
            set {
                if (_diasdndañoactual != value) {
                    _diasdndañoactual = value;
                    PropiedadCambiada();
                }
            }
        }


        public bool VerComite {
            get {
                if (DiasComite > 0) return true;
                if (DiasComiteEnJD > 0) return true;
                if (DiasComiteEnDC > 0) return true;
                return false;
            }
        }


        public int DiasTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico > 0);
            }
        }


        public int DiasInactivos {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == 0);
            }
        }


        public int DiasActivos {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico != 0);
            }
        }


        [Obsolete("Esta propiedad no se utiliza por el momento. Utilice las propiedades individuales.")]
        public string ComputoNoFijos {
            get {
                if (DiasInactivos == 0) {
                    int dias = ListaDias.Count(d => d.Fecha.DayOfWeek == DayOfWeek.Saturday || d.Fecha.DayOfWeek == DayOfWeek.Sunday);
                    return String.Format("(Tra. {0:0.00} | Des. {1:0.00} | Vac. {2:0.00})", TotalDias - dias, dias, 0); //TODO: Establecer las vacaciones.
                } else {
                    int dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                    decimal trabajados = App.Global.Convenio.TrabajoAnuales * (dias - DiasInactivos) / 365m;
                    decimal descansos = App.Global.Convenio.DescansosAnuales * (dias - DiasInactivos) / 365m;
                    decimal vacaciones = App.Global.Convenio.VacacionesAnuales * (dias - DiasInactivos) / 365m;
                    return String.Format("(Tra. {0:0.00} | Des. {1:0.00} | Vac. {2:0.00})", trabajados, descansos, vacaciones);
                }
            }
        }


        public decimal DiasComputoTrabajo {
            get {
                if (DiasInactivos == 0) {
                    int dias = ListaDias.Count(d => d.Fecha.DayOfWeek == DayOfWeek.Saturday || d.Fecha.DayOfWeek == DayOfWeek.Sunday);
                    dias += App.Global.FestivosVM.ListaFestivos.Count(d => d.Fecha.DayOfWeek != DayOfWeek.Saturday);
                    return TotalDias - dias;
                } else {
                    int dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                    return App.Global.Convenio.TrabajoAnuales * (dias - DiasInactivos) / 365m;
                }
            }
        }


        public decimal DiasComputoDescanso {
            get {
                if (DiasInactivos == 0) {
                    int dias = ListaDias.Count(d => d.Fecha.DayOfWeek == DayOfWeek.Saturday || d.Fecha.DayOfWeek == DayOfWeek.Sunday);
                    dias += App.Global.FestivosVM.ListaFestivos.Count(d => d.Fecha.DayOfWeek != DayOfWeek.Saturday);
                    return dias;
                } else {
                    int dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                    return App.Global.Convenio.DescansosAnuales * (dias - DiasInactivos) / 365m;
                }
            }
        }


        public decimal DiasComputoVacaciones {
            get {
                //if (DiasInactivos == 0) {
                //	int dias = ListaDias.Count(d => d.Fecha.DayOfWeek == DayOfWeek.Saturday || d.Fecha.DayOfWeek == DayOfWeek.Sunday);
                //	dias += ListaFestivos.Count(d => d.Fecha.DayOfWeek != DayOfWeek.Saturday);
                //	return 0; //TODO: Establecer las vacaciones.
                //} else {
                int dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                return App.Global.Convenio.VacacionesAnuales * (dias - DiasInactivos) / 365m;
                //}
            }
        }


        public string TextoComputoIndefinidos {
            get {
                return DiasInactivos == 0 ? "(Eventual Completo)" : "(Eventual Parcial)";
            }
        }

        public int DescansosOrdinarios {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -2 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public int DiasTrabajoEnDescanso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico > 0 && d.Codigo == 3);
            }
        }


        public int DiasVacaciones {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -1);
            }
        }


        public int DiasEnfermedad {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -4);
            }
        }


        public int DescansosSueltos {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -5 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public int DescansosCompensatorios {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -6 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        private int _diasDCAñoActual;
        public int DiasDCAñoActual {
            get { return _diasDCAñoActual; }
            set {
                if (_diasDCAñoActual != value) {
                    _diasDCAñoActual = value;
                    PropiedadCambiada();
                }
            }
        }


        public int DiasPermiso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -9);
            }
        }


        public int TotalDias {
            get {
                return DiasTrabajados + DiasInactivos + DescansosOrdinarios + DescansosFinDeSemana + DiasVacaciones + DescansosSueltos +
                       DescansosCompensatorios + DiasF6 + DiasDND + DiasComiteEnJD + DiasComiteEnDC + DiasPermiso;
            }
        }


        public decimal DCsPendientesHastaMes { //Cambiamos el tipo int por decimal.
            get {
                if (Trabajador == null) return 0;
                return Trabajador.Descansos + HastaMesActual.DcRegulados - HastaMesActual.DCs;
            }
        }


        public decimal DCsPendientesHastaAñoAnterior { //Cambiamos el tipo int por decimal.
            get {
                if (Trabajador == null) return 0;
                return Trabajador.Descansos + HastaAñoAnterior.DcRegulados - HastaAñoAnterior.DCs + DCsReguladosAñoActual;
            }
        }


        public decimal DNDsPendientesHastaMes {
            get {
                if (Trabajador == null) return 0;
                return Trabajador.DescansosNoDisfrutados + HastaMesActual.ComiteEnDescanso + HastaMesActual.TrabajoEnDescanso - HastaMesActual.DNDs;
            }
        }


        public decimal DNDsPendientesHastaAñoAnterior {
            get {
                if (Trabajador == null) return 0;
                return Trabajador.DescansosNoDisfrutados + HastaAñoAnterior.ComiteEnDescanso + HastaAñoAnterior.TrabajoEnDescanso - HastaAñoAnterior.DNDs;
            }
        }


        public decimal DCsAñoActual { //Cambiamos el tipo int por decimal.
            get {
                if (Trabajador == null) return 0;
                return DCsPendientesHastaAñoAnterior - DCsPendientesHastaMes;
            }
        }


        private decimal _dcsreguladosañoactual;
        public decimal DCsReguladosAñoActual {
            get { return _dcsreguladosañoactual; }
            set {
                if (_dcsreguladosañoactual != value) {
                    _dcsreguladosañoactual = value;
                    PropiedadCambiada();
                }
            }
        }


        public decimal DNDsAñoActual {
            get {
                if (Trabajador == null) return 0;
                return DNDsPendientesHastaAñoAnterior - DNDsPendientesHastaMes;
            }
        }






        #endregion


        // ====================================================================================================
        #region PROPIEDADES FINES DE SEMANA
        // ====================================================================================================

        public int DescansosFinDeSemana {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -3 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        public decimal FindesDisfrutados {
            get {
                decimal findes = 0m;
                bool sabado = false;
                foreach (DiaPijama dia in ListaDias) {
                    switch (dia.Fecha.DayOfWeek) {
                        case DayOfWeek.Saturday:
                            if (dia.Grafico == -2 || dia.Grafico == -3) sabado = true;
                            break;
                        case DayOfWeek.Sunday:
                            if (sabado) {
                                if (dia.Grafico == -2 || dia.Grafico == -3) findes += 1m;
                                sabado = false;
                            }
                            break;
                    }
                }
                // Si el día 1 es domingo y descanso, se marca medio finde.
                if (ListaDias[0].Fecha.DayOfWeek == DayOfWeek.Sunday) {
                    if (ListaDias[0].Grafico == -2 || ListaDias[0].Grafico == -3) findes += 0.5m;
                }
                // Si el último día es sábado y descanso, se marca medio finde.
                if (ListaDias[ListaDias.Count - 1].Fecha.DayOfWeek == DayOfWeek.Saturday) {
                    if (ListaDias[ListaDias.Count - 1].Grafico == -2 || ListaDias[ListaDias.Count - 1].Grafico == -3) findes += 0.5m;
                }
                return findes;
            }
        }


        public int SabadosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3) && d.Fecha.DayOfWeek == DayOfWeek.Saturday);
            }
        }


        public int DomingosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3) && d.Fecha.DayOfWeek == DayOfWeek.Sunday);
            }
        }


        public int FestivosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3) && d.EsFestivo);
            }
        }


        public int SabadosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.Fecha.DayOfWeek == DayOfWeek.Saturday);
            }
        }


        public decimal ImporteSabados {
            get {
                int sabadosTrabajoNoFestivos = ListaDias.Count(d => (d.Grafico > 0 && !d.EsFestivo) && d.Fecha.DayOfWeek == DayOfWeek.Saturday);
                return sabadosTrabajoNoFestivos * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteSabados;
            }
        }


        public int DomingosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.Fecha.DayOfWeek == DayOfWeek.Sunday);
            }
        }


        public decimal ImporteDomingos {
            get {
                return DomingosTrabajados * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteFestivos;
            }
        }


        public int FestivosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.EsFestivo);
            }
        }


        public decimal ImporteFestivos {
            get {
                return FestivosTrabajados * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteFestivos;
            }
        }


        public decimal ImporteDias {
            get {
                return ImporteSabados + ImporteDomingos + ImporteFestivos;
            }
        }

        #endregion


        // ====================================================================================================
        #region PROPIEDADES DIETAS Y PLUSES
        // ====================================================================================================

        public decimal DietaDesayuno {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.Desayuno);
            }
        }


        public decimal DietaComida {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.Comida);
            }
        }


        public decimal DietaCena {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.Cena);
            }
        }


        public decimal DietaPlusCena {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusCena);
            }
        }


        public decimal PlusMenorDescanso {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusMenorDescanso);
            }
        }


        public decimal PlusPaqueteria {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusPaqueteria);
            }
        }


        public decimal PlusLimpieza {
            get {
                if (App.Global.PorCentro.PagarLimpiezas == true) {
                    if (Trabajador.Indefinido) {
                        return App.Global.PorCentro.NumeroLimpiezas * App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    } else {
                        return DiasTrabajados * App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    }
                } else {
                    if (ListaDias == null) return 0m;
                    return ListaDias.Sum(x => x.PlusLimpieza);
                }
            }
        }


        public decimal PlusViaje {
            get {
                if (App.Global.PorCentro.PagarPlusViaje == true) return DiasTrabajados * App.Global.PorCentro.PlusViaje;
                return 0m;
            }
        }


        public decimal PlusNocturnidad {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusNocturnidad);
            }
        }


        public decimal PlusNavidad {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusNavidad);
            }
        }


        public decimal OtrosPluses {
            get {
                return PlusNocturnidad + PlusViaje + PlusNavidad;
            }
        }


        public decimal TotalDietas {
            get {
                if (ListaDias == null) return 0m;
                return (DietaDesayuno * App.Global.Convenio.PorcentajeDesayuno / 100) + DietaComida + DietaCena + DietaPlusCena;
            }
        }


        public decimal ImporteDietas {
            get {
                return TotalDietas * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;
            }
        }


        public decimal ImportePluses {
            get {
                if (ListaDias == null) return 0m;
                return PlusMenorDescanso + PlusPaqueteria + PlusLimpieza + PlusNocturnidad + PlusNavidad + PlusViaje;
            }
        }


        public decimal ImporteTotal {
            get {
                return ImporteDias + ImporteDietas + ImportePluses;
            }
        }



        #endregion



    }
}
