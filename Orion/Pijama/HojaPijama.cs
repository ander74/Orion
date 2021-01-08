﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.Pijama {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LiveCharts;
    using LiveCharts.Defaults;
    using Orion.Config;
    using Orion.Models;
    using Orion.Servicios;

    public class HojaPijama : NotifyBase {

        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public HojaPijama(Calendario calendario, IMensajes mensajes) {
            Mensajes = mensajes;

            try {
                // Establecemos la fecha del pijama.
                Fecha = calendario.Fecha;
                // Extraemos el conductor del calendario.
                Trabajador = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);
                // Si el trabajador no existe, salimos.
                if (Trabajador == null) return;
                // Extraemos la lista de los días pijama.
                var listaTemporal = App.Global.Repository.GetDiasPijama(Fecha, Trabajador.Matricula).ToList();

                //Filtrar la lista por el tipo de día que corresponde a cada fecha.
                ListaDias = new List<DiaPijama>();
                for (int i = 1; i<=DateTime.DaysInMonth(Fecha.Year, Fecha.Month); i++) {
                    if (listaTemporal.Count(d => d.DiaFecha.Day == i) > 1) {
                        var diaC = listaTemporal.FirstOrDefault(d => d.DiaFecha.Day == i && d.GraficoOriginal.DiaSemana == Utils.GetDiaSemanaGrafico(d.DiaFecha));
                        ListaDias.Add(diaC);
                    } else {
                        ListaDias.Add(listaTemporal.FirstOrDefault(d => d.DiaFecha.Day == i));
                    }
                }

                // Definimos el final anterior y lo establecemos al día anterior del primer día del mes.
                TimeSpan? finalAnterior = null;
                DateTime fecha = new DateTime(calendario.Fecha.Year, calendario.Fecha.Month, 1).AddDays(-1);
                DiaCalendarioBase diaAnterior = App.Global.Repository.GetDiaCalendario(calendario.MatriculaConductor, fecha);
                if (diaAnterior != null) {
                    GraficoBase graficoanterior = App.Global.Repository.GetGrafico(diaAnterior.Grafico, fecha);
                    if (graficoanterior != null && graficoanterior.Final.HasValue) finalAnterior = graficoanterior.Final.Value;
                }
                // Recorremos cada uno de los días para computar todos los valores.
                foreach (DiaPijama dia in ListaDias) {
                    // Establecemos si es festivo.
                    if (App.Global.CalendariosVM.EsFestivo(dia.DiaFecha)) dia.EsFestivo = true;
                    // Establecemos las horas trabajadas reales.
                    dia.TrabajadasReales = dia.GraficoTrabajado.Trabajadas;
                    // Ajustamos la jornada en función del exceso de jornada.
                    if (dia.ExcesoJornada != TimeSpan.Zero) {
                        if (dia.GraficoTrabajado != null) dia.GraficoTrabajado.Final += dia.ExcesoJornada;
                    }
                    // Si el conductor no tiene reducción de jornada, ajustamos las horas a la jornada media si es necesario.
                    if (dia.Grafico > 0) {
                        if (!Trabajador.ReduccionJornada && dia.GraficoTrabajado.Trabajadas < App.Global.Convenio.JornadaMedia) {
                            dia.GraficoTrabajado.Trabajadas = App.Global.Convenio.JornadaMedia;
                        }
                    }
                    // Establecemos el Plus de Paquetería
                    if (dia.GraficoTrabajado.PlusPaqueteria) {
                        dia.PlusPaqueteria = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusPaqueteria;
                    } else {
                        dia.PlusPaqueteria = dia.FacturadoPaqueteria * 0.10m;
                    }
                    // Establecemos el Plus de Limpieza
                    //if (dia.GraficoTrabajado.PlusLimpieza) {
                    //    dia.PlusLimpieza = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    //} else {
                        if (dia.Limpieza == null)
                            dia.PlusLimpieza = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza / 2m;
                        if (dia.Limpieza == true)
                            dia.PlusLimpieza = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    //}
                    // Establecemos el Plus de Nocturnidad
                    if (dia.GraficoTrabajado.Turno == 3) dia.PlusNocturnidad = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusNocturnidad;
                    // Establecemos el quebranto de moneda.
                    if (dia.GraficoTrabajado.Numero > 0 || dia.GraficoTrabajado.Numero == -6) {
                        dia.QuebrantoMoneda = App.Global.OpcionesVM.GetPluses(Fecha.Year).QuebrantoMoneda;
                    }
                    // Establcemos el Plus de Navidad
                    if (dia.GraficoTrabajado.Numero > 0 && dia.DiaFecha.Day == 25 && dia.DiaFecha.Month == 12) {
                        dia.PlusNavidad = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusNavidad;
                    }
                    if (dia.GraficoTrabajado.Numero > 0 && dia.DiaFecha.Day == 1 && dia.DiaFecha.Month == 1) {
                        dia.PlusNavidad = App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusNavidad;
                    }
                    // Establecemos el Plus por Menor Descanso, si el tiempo entre el final anterior y el inicio es menor de 12 horas.
                    if (finalAnterior.HasValue && dia.GraficoTrabajado.Inicio.HasValue) {
                        // Añadimos uno al inicio de hoy.
                        TimeSpan inicio = dia.GraficoTrabajado.Inicio.Value.Add(new TimeSpan(1, 0, 0, 0));
                        // Si el turno es 3 (Noche) y empieza más tarde de las 00:00h, se añade uno más
                        if (dia.GraficoTrabajado.Turno == 3 && inicio.Hours < 3) inicio = inicio.Add(new TimeSpan(1, 0, 0, 0));
                        // Si el final anterior es mayor que el inicio, añadimos otro día al inicio.
                        if (inicio < finalAnterior.Value) inicio = inicio.Add(new TimeSpan(1, 0, 0, 0));
                        // Si las horas que separan el inicio menos el final anterior son menos de doce...
                        decimal diferenciahoras = (decimal)(inicio - finalAnterior.Value).TotalHours;
                        TimeSpan df = new TimeSpan(0, 12, 0, 0) - (inicio - finalAnterior.Value);
                        if (diferenciahoras < 12 && diferenciahoras >= 9) {
                            //dia.TiempoMenorDescanso = (12 - diferenciahoras);
                            dia.TiempoMenorDescanso = $"{df.ToTexto()} ({df.ToDecimal():0.00})";
                            dia.PlusMenorDescanso = (12 - diferenciahoras) * App.Global.OpcionesVM.GetPluses(Fecha.Year).DietaMenorDescanso;
                        }
                    }
                    // Si es un día de permiso, se establecen las horas trabajadas en la jornada media.
                    if (dia.Grafico == Incidencia.PER) dia.GraficoTrabajado.Trabajadas = App.Global.Convenio.JornadaMedia;
                    // Si es un día de formación, se establecen las horas trabajadas en la jornada media.
                    if (dia.Grafico == Incidencia.FOR) dia.GraficoTrabajado.Trabajadas = App.Global.Convenio.JornadaMedia;
                    // Establecemos el final anterior, si existe.
                    if (dia.GraficoTrabajado != null && dia.GraficoTrabajado.Final.HasValue) {
                        finalAnterior = dia.GraficoTrabajado.Final.Value;
                    } else {
                        //Si es un DS cuentan 24h más (o sea, 36h) para el plus de menor descanso.
                        if (dia.Grafico != Incidencia.DS || (dia.Grafico == Incidencia.DS && dia.EsFestivo)) finalAnterior = null;
                    }
                    // Si es un día de comite, se añade una dieta de comida.
                    if ((dia.Codigo == Codigo.CO) || (dia.Codigo == Codigo.CE)) {
                        if (dia.GraficoTrabajado.Comida < 1) dia.GraficoTrabajado.Comida = 1m;
                    }
                }
                // Extraemos las horas de las regulaciones.
                HorasCobradas = App.Global.Repository.GetHorasCobradasMes(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                HorasCobradasAño = App.Global.Repository.GetHorasCobradasAño(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                HorasReguladas = App.Global.Repository.GetHorasCambiadasPorDCsMes(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                OtrasHoras = App.Global.Repository.GetHorasReguladasMes(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                DiasTrabajadosHastaMesEnAño = App.Global.Repository.GetDiasTrabajadosHastaMesEnAño(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                DiasDescansoHastaMesEnAño = App.Global.Repository.GetDiasDescansoHastaMesEnAño(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                DiasVacacionesHastaMesEnAño = App.Global.Repository.GetDiasVacacionesHastaMesEnAño(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                DiasInactivoHastaMesEnAño = App.Global.Repository.GetDiasInactivoHastaMesEnAño(Fecha.Year, Fecha.Month, Trabajador.Matricula);
                // Cambiamos el signo de las horas cobradas para que estén en positivo.
                HorasCobradas = new TimeSpan(HorasCobradas.Ticks * -1);
                HorasCobradasAño = new TimeSpan(HorasCobradasAño.Ticks * -1);
                // Extraemos los datos hasta el mes actual.
                HastaMesActual = App.Global.Repository.GetResumenHastaMes(Fecha.Year, Fecha.Month, Trabajador.Matricula, App.Global.PorCentro.Comodin);
                // Extramos los datos hasta el año anterior
                DateTime fechaanterior = Fecha.Month == 12 ? new DateTime(Fecha.Year, 12, 1) : new DateTime(Fecha.Year - 1, 12, 1);
                fechaanterior = fechaanterior.AddMonths(-1);
                HastaAñoAnterior = App.Global.Repository.GetResumenHastaMes(fechaanterior.Year, fechaanterior.Month, Trabajador.Matricula, App.Global.PorCentro.Comodin);

            } catch (Exception ex) {
                Mensajes.VerError("HojaPijama.Constructor", ex);
            }
            // Llamamos a todas las opciones del pijama, para que se reescriban.
            PropiedadCambiada("");

        }

        #endregion
        // ====================================================================================================


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
            int totalDiasDescanso = 0;
            int totalDiasInactivo = 0;

            TimeSpan? finalAnterior = TimeSpan.Zero; // Indica la hora final del servicio anterior.

            // TODO: Añadir función en el repositorio para cargar el calendario de un conductor y un mes en concreto.
            var listaDiasAnterior = new List<DiaCalendarioBase>();
            for (int dia = 1; dia <= 10; dia++) {
                var fechaAnterior = Fecha.AddDays(dia * -1);
                var diaAnterior = App.Global.Repository.GetDiaCalendario(Trabajador.Matricula, fechaAnterior);
                listaDiasAnterior.Add(diaAnterior);
            }
            foreach (var diaAnterior in listaDiasAnterior.OrderByDescending(d => d?.DiaFecha)) {
                if (diaAnterior.Grafico <= 0) break;
                diastrabajados++;
            }


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
                        if (diasdescansados == 1 && dia.DiaFecha.Day > 2 && diasinactivos == 0)
                            resultado += String.Format("Día {0:00}: Descanso suelto no computado.\n", primerdiadescanso);

                    }
                    // Si hay más de seis días trabajados, se añade un fallo.
                    //if (diastrabajados > 6) resultado += String.Format("Día {0:00}: Más de seis días de trabajo.\n", primerdiatrabajo);
                    if (diastrabajados > 6) resultado += String.Format("Día {0:00}: Más de seis días de trabajo.\n", dia.Dia);
                    // Reiniciamos los descansos y los días inactivos.
                    diasdescansados = 0;
                    diasinactivos = 0;
                }


                // DÍA NO TRABAJADO
                if (dia.Grafico == -2 || dia.Grafico == -3 ||
                    dia.Grafico == -10 || dia.Grafico == -11 ||
                    dia.Grafico == -12 || dia.Grafico == -13 ||
                    dia.Grafico == -17 || dia.Grafico == -18) {
                    // Se añade a días de descanso y al total de descansos.
                    diasdescansados++;
                    totalDiasDescanso++;
                    // Si es el primero, se marca como primer día descansado.
                    if (diasdescansados == 1) primerdiadescanso = dia.DiaFecha.Day;
                    // Reiniciamos los días trabajados, inactivos y borramos el final anterior.
                    diastrabajados = 0;
                    diasinactivos = 0;
                    //finalanterior = null;
                }


                // DESCANSO SUELTO
                if (dia.Grafico == -5) {
                    // Lo añadimos a descansos sueltos y al total de descansos.
                    descansossueltos++;
                    totalDiasDescanso++;
                    // Si hay más de dos descansos sueltos, se añade el fallo.
                    if (descansossueltos > 2) resultado += String.Format("Día {0:00}: Más de dos descansos sueltos en el mes.\n", dia.Dia);
                    // Reiniciamos los días trabajados, inactivos.
                    diastrabajados = 0;
                    diasinactivos = 0;
                }


                // DÍA INACTIVO
                if (dia.Grafico == 0) {
                    // Añadimos el día a los días inactivos.
                    diasinactivos++;
                    totalDiasInactivo++;
                    // Si es el primer día inactivo...
                    if (diasinactivos == 1) {
                        // Si sólo hay un descanso anterior, se marca el fallo.
                        if (diasdescansados == 1 && dia.DiaFecha.Day > 2)
                            resultado += String.Format("Día {0:00}: Descanso suelto no computado.\n", primerdiadescanso);
                    }
                    // Reiniciamos los días trabajados, descansados y borramos el final anterior.
                    diastrabajados = 0;
                    diasdescansados = 0;
                    //finalanterior = null;
                    // Borramos los fines de semana trabajados.
                    findestrabajados = 0;
                }

                // RESET DIAS TRABAJADOS
                if (dia.Grafico < 0) diastrabajados = 0;

                // EXCESO DE HORAS TRABAJADAS EN LABORABLES Y FESTIVOS
                if (dia.DiaFecha.DayOfWeek != DayOfWeek.Saturday && dia.DiaFecha.DayOfWeek != DayOfWeek.Sunday) {
                    if (dia.EsFestivo) {
                        if (dia.GraficoTrabajado.Trabajadas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en festivo.\n";
                    } else {
                        if (dia.GraficoTrabajado.Trabajadas > App.Global.Convenio.MaxHorasLaborables) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en laborable.\n";
                    }
                }

                // CONTROL DE GRÁFICOS PARTIDOS
                if (dia.GraficoTrabajado.Turno == 4 && dia.GraficoTrabajado.Inicio.HasValue && dia.GraficoTrabajado.Final.HasValue) {
                    TimeSpan horafinal;
                    horafinal = dia.GraficoTrabajado.Final.Value;
                    if (horafinal < dia.GraficoTrabajado.Inicio.Value) horafinal = horafinal.Add(new TimeSpan(1, 0, 0, 0));
                    if (horafinal.TotalHours - dia.GraficoTrabajado.Inicio.Value.TotalHours > App.Global.Convenio.MaxHorasTotalGraficoPartido.TotalHours) {
                        resultado += $"Día {dia.Dia:00}: Exceso de horas totales en gráfico partido.\n";
                    }
                    if (dia.GraficoTrabajado.InicioPartido.HasValue && dia.GraficoTrabajado.FinalPartido.HasValue) {
                        if (dia.GraficoTrabajado.FinalPartido.Value.TotalHours -
                            dia.GraficoTrabajado.InicioPartido.Value.TotalHours > App.Global.Convenio.MaxHorasParticionGraficoPartido.TotalHours) {
                            resultado += $"Día {dia.Dia:00}: Exceso de tiempo de partición en gráfico partido.\n";
                        }
                    }
                }


                // FINES DE SEMANA TRABAJADOS Y EXCESO HORAS EN FIN DE SEMANA

                // Si un sabado se trabaja, se añade el fin de semana a trabajados y se marca el sábado.
                if (dia.DiaFecha.DayOfWeek == DayOfWeek.Saturday && dia.Grafico > 0) {
                    findestrabajados++;
                    sabadotrabajado = true;
                    sabadodescansado = false;
                    if (dia.GraficoTrabajado.Trabajadas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en sábado.\n";
                }
                // Si un sábado se descansa, se marca como descansado.
                if (dia.DiaFecha.DayOfWeek == DayOfWeek.Saturday && dia.Grafico <= 0) {
                    sabadotrabajado = false;
                    sabadodescansado = true;
                }
                // Si un domingo se trabaja...
                if (dia.DiaFecha.DayOfWeek == DayOfWeek.Sunday && dia.Grafico > 0) {
                    //... Si no se trabajo el sábado se añade el fin de semana a trabajados.
                    if (!sabadotrabajado) {
                        findestrabajados++;
                    }
                    sabadotrabajado = false;
                    if (dia.GraficoTrabajado.Trabajadas > App.Global.Convenio.MaxHorasFinesSemana) resultado += $"Día {dia.Dia:00}: Exceso de horas trabajadas en domingo.\n";
                }
                // Si un domingo se descansa...
                if (dia.DiaFecha.DayOfWeek == DayOfWeek.Sunday && dia.Grafico <= 0) {
                    //... Si también se descansó el sábado, se ponen a cero los fines de semanas trabajados.
                    if (sabadodescansado) {
                        findestrabajados = 0;
                    }
                    sabadodescansado = false;
                }

                // Si hay más de dos fines de semana trabajados seguidos, se añade el fallo.
                if (dia.DiaFecha.DayOfWeek == DayOfWeek.Sunday && findestrabajados > 2)
                    resultado += String.Format("Día {0:00}: Más de dos fines de semana trabajados seguidos.\n", dia.Dia);

                // COMPROBAR QUE HAY MENOS DE 9 HORAS ENTRE SERVICIOS.
                if (dia.Grafico > 0) {
                    if (finalAnterior.HasValue && dia.GraficoTrabajado.Inicio.HasValue) {
                        // Añadimos uno al inicio de hoy.
                        TimeSpan inicio = dia.GraficoTrabajado.Inicio.Value.Add(new TimeSpan(1, 0, 0, 0));
                        // Si el turno es 3 (Noche) y empieza más tarde de las 00:00h, se añade uno más
                        if (dia.GraficoTrabajado.Turno == 3 && inicio.Hours < 3) inicio = inicio.Add(new TimeSpan(1, 0, 0, 0));
                        // Si el final anterior es mayor que el inicio, añadimos otro día al inicio.
                        if (inicio < finalAnterior.Value) inicio = inicio.Add(new TimeSpan(1, 0, 0, 0));
                        // Si las horas que separan el inicio menos el final anterior son menos de doce...
                        decimal diferenciahoras = (decimal)(inicio - finalAnterior.Value).TotalHours;
                        if (diferenciahoras < 9) resultado += String.Format("Día {0:00}: Menos de nueve horas entre jornadas.\n", dia.Dia);
                    }
                    finalAnterior = dia.GraficoTrabajado.Final;
                } else {
                    finalAnterior = null;
                }
            }

            // TOTAL DIAS DESCANSADOS MENOS DE 8 Y MAS DE 12
            if (totalDiasInactivo == 0) {
                if (totalDiasDescanso < 8) resultado += $"Menos de 8 descansos en el mes.\n";
                if (totalDiasDescanso > 12) resultado += $"Más de 12 descansos en el mes.\n";
            }

            return resultado;
        }






        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region EVENTOS
        // ====================================================================================================

        public event EventHandler<DiaCambiadoEventArgs> DiaCambiado;

        public class DiaCambiadoEventArgs : EventArgs {
            public DiaCambiadoEventArgs(DiaPijama dia) {
                Dia = dia;
            }

            public DiaPijama Dia { get; }
        }

        public void CambiaDia(DiaPijama dia) {
            DiaCambiado?.Invoke(this, new DiaCambiadoEventArgs(dia));
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES GENERALES
        // ====================================================================================================

        /// <summary>
        /// Proovedor de mensajes.
        /// </summary>
        private IMensajes _mensajeprovider;
        public IMensajes Mensajes {
            get { return _mensajeprovider; }
            set {
                if (_mensajeprovider != value) {
                    _mensajeprovider = value;
                }
            }
        }


        private DateTime _fecha;
        /// <summary>
        /// Contiene la fecha de la hoja pijama (Sólo nos interesa mes y año).
        /// </summary>
        public DateTime Fecha {
            get { return _fecha; }
            set {
                if (_fecha != value) {
                    _fecha = value;
                }
            }
        }


        private Conductor _trabajador;
        /// <summary>
        /// Conductor al que pertenece la hoja pijama.
        /// </summary>
        public Conductor Trabajador {
            get { return _trabajador; }
            set {
                if (_trabajador != value) {
                    _trabajador = value;
                }
            }
        }


        /// <summary>
        /// Devuelve true si el trabajador no es indefinido.
        /// </summary>
        public bool NoEsFijo {
            get { return !Trabajador.Indefinido; }
        }


        private List<DiaPijama> _listadias;
        /// <summary>
        /// Lista de los días pijama.
        /// </summary>
        public List<DiaPijama> ListaDias {
            get { return _listadias; }
            set {
                if (_listadias != value) {
                    _listadias = value;
                }
            }
        }


        private DiaPijama _diaseleccionado;
        public DiaPijama DiaSeleccionado {
            get { return _diaseleccionado; }
            set {
                if (_diaseleccionado != value) {
                    _diaseleccionado = value;
                    CambiaDia(_diaseleccionado);
                    PropiedadCambiada();
                    PropiedadCambiada(nameof(HayDiaSeleccionado));
                    _diaseleccionado.PropiedadCambiada(nameof(_diaseleccionado.GraficoTrabajado));
                }
            }
        }


        public bool HayDiaSeleccionado {
            get => DiaSeleccionado != null;
        }


        /// <summary>
        /// Mes actual en texto para poner en la cabecera de la hoja pijama.
        /// </summary>
        public string TextoMesActual {
            get {
                return $"{Fecha:MMMM}".ToUpper();
            }
        }


        /// <summary>
        /// Datos del trabajador para poner en la cabecera de la hoja pijama.
        /// </summary>
        public string TextoTrabajador {
            get {
                return $"{Trabajador.Matricula:000}: {Trabajador.Apellidos}";
            }
        }


        public bool VerComite {
            get {
                if (Comite > 0) return true;
                if (ComiteEnDescanso > 0) return true;
                if (ComiteEnDC > 0) return true;
                return false;
            }
        }


        /// <summary>
        /// Suma total de los importes de todos los pluses y dietas del mes actual.
        /// </summary>
        public decimal ImporteTotal {
            get {
                return ImporteTotalDietas + ImporteTotalPluses + ImporteTotalFindes;
            }
        }


        /// <summary>
        /// Texto que se mostrará al poner el ratón encima de 'Otros Pluses'.
        /// </summary>
        public string ToolTipOtrosPluses {
            get {
                string tooltip = "";
                if (PlusNocturnidad != 0) tooltip += $"Plus Nocturnidad = {PlusNocturnidad:0.00} €\n";
                if (PlusViaje != 0) tooltip += $"Plus Viaje = {PlusViaje:0.00} €\n";
                if (PlusNavidad != 0) tooltip += $"Plus Navidad = {PlusNavidad:0.00} €\n";
                return tooltip;
            }
        }








        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES GLOBALES
        // ====================================================================================================


        private ResumenPijama _hastamesactual;
        /// <summary>
        /// Resumen de valores hasta el mes actual.
        /// </summary>
        public ResumenPijama HastaMesActual {
            get { return _hastamesactual; }
            set {
                if (_hastamesactual != value) {
                    _hastamesactual = value;
                }
            }
        }


        private ResumenPijama _hastaañoanterior;
        /// <summary>
        /// Resumen de valores hasta el año anterior.
        /// </summary>
        public ResumenPijama HastaAñoAnterior {
            get { return _hastaañoanterior; }
            set {
                if (_hastaañoanterior != value) {
                    _hastaañoanterior = value;
                }
            }
        }


        /// <summary>
        /// Horas acumuladas hasta el año anterior.
        /// </summary>
        public TimeSpan AcumuladasHastaAñoAnterior {
            get {
                return Trabajador.Acumuladas +
                       HastaAñoAnterior.HorasAcumuladas +
                       HastaAñoAnterior.HorasReguladas -
                       new TimeSpan(Convert.ToInt64(HastaAñoAnterior.DiasLibreDisposicionF6 * App.Global.Convenio.JornadaMedia.Ticks));
                //return Trabajador.Acumuladas +
                //	   HastaAñoAnterior.HorasAcumuladas +
                //	   HastaAñoAnterior.HorasReguladas;
            }
        }


        /// <summary>
        /// Descansos compensatorios pendientes hasta el año anterior.
        /// </summary>
        public decimal DCsPendientesHastaAñoAnterior {
            get {
                return Trabajador.Descansos +
                       HastaAñoAnterior.DCsRegulados -
                       HastaAñoAnterior.DCsDisfrutados
                       - HastaAñoAnterior.DiasF6DC;
            }
        }


        /// <summary>
        /// Descansos no disfrutados pendientes hasta el año anterior.
        /// </summary>
        public decimal DNDsPendientesHastaAñoAnterior {
            get {
                return Trabajador.DescansosNoDisfrutados +
                       HastaAñoAnterior.DNDsRegulados +
                       HastaAñoAnterior.DiasComiteEnDescanso +
                       HastaAñoAnterior.DiasTrabajoEnDescanso -
                       HastaAñoAnterior.DNDsDisfrutados;
            }
        }


        /// <summary>
        /// Horas acumuladas hasta el mes actual, desde el inicio del año en curso.
        /// </summary>
        public TimeSpan AcumuladasHastaMes {
            get {
                var horas = Trabajador.Acumuladas + HastaMesActual.HorasAcumuladas + HastaMesActual.HorasReguladas;
                var diasF6 = HastaAñoAnterior.DiasLibreDisposicionF6;
                // Sólo se tienen en cuenta las horas de los F6 si es el mes de noviembre, que es cuando la empresa los descuenta.
                if (Fecha.Month == 11) {
                    diasF6 = HastaAñoAnterior.DiasLibreDisposicionF6 + HastaMesActual.DiasLibreDisposicionF6;
                }
                horas -= new TimeSpan(Convert.ToInt64(diasF6 * App.Global.Convenio.JornadaMedia.Ticks));
                return horas;
                //return Trabajador.Acumuladas +
                //       HastaMesActual.HorasAcumuladas +
                //       HastaMesActual.HorasReguladas -
                //       new TimeSpan(Convert.ToInt64(HastaMesActual.DiasLibreDisposicionF6 * App.Global.Convenio.JornadaMedia.Ticks));
            }
        }


        /// <summary>
        /// Descansos compensatorios pendientes hasta el mes actual.
        /// </summary>
        public decimal DCsPendientesHastaMes { // Cambiamos el tipo int por decimal
            get {
                return Trabajador.Descansos +
                       HastaMesActual.DCsRegulados -
                       HastaMesActual.DCsDisfrutados
                       - HastaMesActual.DiasF6DC;
            }
        }


        /// <summary>
        /// Descansos no disfrutados pendientes hasta el mes actual.
        /// </summary>
        public decimal DNDsPendientesHastaMes {
            get {
                return Trabajador.DescansosNoDisfrutados +
                       HastaMesActual.DNDsRegulados +
                       HastaMesActual.DiasComiteEnDescanso +
                       HastaMesActual.DiasTrabajoEnDescanso -
                       HastaMesActual.DNDsDisfrutados;
            }
        }


        /// <summary>
        /// Devuelve el total de DCs pendientes hasta el año anterior junto a las horas en decimal redondeado a 2 decimales.
        /// </summary>
        public decimal DCsGeneradosHastaAñoAnterior {
            get {
                decimal resultado = DCsPendientesHastaAñoAnterior;
                resultado += (decimal)AcumuladasHastaAñoAnterior.Ticks / App.Global.Convenio.JornadaMedia.Ticks;
                return Math.Round(resultado, 4);
            }
        }


        /// <summary>
        /// Devuelve el total de DCs pendientes hasta el mes actual junto a las horas en decimal redondeado a 2 decimales.
        /// </summary>
        public decimal DCsGeneradosHastaMes {
            get {
                decimal resultado = DCsPendientesHastaMes;
                //resultado += ((decimal)AcumuladasHastaMes.Ticks + AcumuladasHastaAñoAnterior.Ticks )/ App.Global.Convenio.JornadaMedia.Ticks;
                resultado += (decimal)AcumuladasHastaMes.Ticks / App.Global.Convenio.JornadaMedia.Ticks;
                return Math.Round(resultado, 4);
            }
        }



        private int diasDescansoHastaMesEnAño;
        public int DiasDescansoHastaMesEnAño {
            get => diasDescansoHastaMesEnAño;
            set => SetValue(ref diasDescansoHastaMesEnAño, value);
        }


        private int diasTrabajadosHastaMesEnAño;
        public int DiasTrabajadosHastaMesEnAño {
            get => diasTrabajadosHastaMesEnAño;
            set => SetValue(ref diasTrabajadosHastaMesEnAño, value);
        }


        private int diasVacacionesHastaMesEnAño;
        public int DiasVacacionesHastaMesEnAño {
            get => diasVacacionesHastaMesEnAño;
            set => SetValue(ref diasVacacionesHastaMesEnAño, value);
        }


        private int diasActivoHastaMesEnAño;
        public int DiasActivoHastaMesEnAño {
            get => diasActivoHastaMesEnAño;
            set => SetValue(ref diasActivoHastaMesEnAño, value);
        }


        private int diasInactivoHastaMesEnAño;
        public int DiasInactivoHastaMesEnAño {
            get => diasInactivoHastaMesEnAño;
            set => SetValue(ref diasInactivoHastaMesEnAño, value);
        }

        public decimal DiasTrabajoPendientes {
            get {
                return DiasTrabajadosHastaMesEnAño - DiasComputoTrabajoHastaMes;
            }
        }

        public decimal DescansosPendientes {
            get {
                return DiasComputoDescansoHastaMes - DiasDescansoHastaMesEnAño;
            }
        }

        public decimal VacacionesPendientes {
            get {
                return DiasComputoVacacionesHastaMes - DiasVacacionesHastaMesEnAño;
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES HORAS
        // ====================================================================================================

        private TimeSpan _horascobradas;
        /// <summary>
        /// Horas cobradas del mes actual.
        /// </summary>
        public TimeSpan HorasCobradas {
            get { return _horascobradas; }
            set {
                if (_horascobradas != value) {
                    _horascobradas = value;
                }
            }
        }


        private TimeSpan _horascobradasaño;
        /// <summary>
        /// Horas cobradas del año actual.
        /// </summary>
        public TimeSpan HorasCobradasAño {
            get { return _horascobradasaño; }
            set {
                if (_horascobradasaño != value) {
                    _horascobradasaño = value;
                }
            }
        }


        private TimeSpan _horasreguladas;
        /// <summary>
        /// Horas reguladas del mes actual (se han cambiado por DCs).
        /// </summary>
        public TimeSpan HorasReguladas {
            get { return _horasreguladas; }
            set {
                if (_horasreguladas != value) {
                    _horasreguladas = value;
                }
            }
        }


        private TimeSpan _otrashoras;
        /// <summary>
        /// Horas de las regulaciones del conductor que no son ni cobradas ni reguladas del mes actual.
        /// </summary>
        public TimeSpan OtrasHoras {
            get { return _otrashoras; }
            set {
                if (_otrashoras != value) {
                    _otrashoras = value;
                }
            }
        }


        /// <summary>
        /// Horas trabajadas del mes actual.
        /// </summary>
        public TimeSpan Trabajadas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.GraficoTrabajado.Trabajadas.Ticks)));
            }
        }


        /// <summary>
        /// Horas acumuladas del mes actual.
        /// </summary>
        public TimeSpan Acumuladas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                var xxx = new TimeSpan(ListaDias.Sum(x => (x.GraficoTrabajado.Acumuladas.Ticks)));
                return new TimeSpan(ListaDias.Sum(x => (x.GraficoTrabajado.Acumuladas.Ticks)));
            }
        }


        /// <summary>
        /// Horas nocturnas del mes actual.
        /// </summary>
        public TimeSpan Nocturnas {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.GraficoTrabajado.Nocturnas.Ticks)));
            }
        }


        /// <summary>
        /// Horas de exceso de jornada del mes actual.
        /// </summary>
        public TimeSpan ExcesoJornada {
            get {
                if (ListaDias == null) return TimeSpan.Zero;
                return new TimeSpan(ListaDias.Sum(x => (x.ExcesoJornada.Ticks)));
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES DÍAS
        // ====================================================================================================

        /// <summary>
        /// Días de trabajo del mes actual (gráfico mayor que 0).
        /// </summary>
        public int Trabajo {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico > 0);
            }
        }


        /// <summary>
        /// Días de descanso del mes actual (J-D => -2).
        /// </summary>
        public int Descanso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -10 || d.Grafico == -12) && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Días de vacaciones del mes actual (O-V => -1).
        /// </summary>
        public int Vacaciones {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -1 || d.Grafico == -12 || d.Grafico == -13);
            }
        }


        /// <summary>
        /// Descansos no disfrutados del mes actual (DND => -8).
        /// </summary>
        public int DescansosNoDisfrutados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -8);
            }
        }


        /// <summary>
        /// Trabajo en días de descanso del mes actual (gráfico mayor que cero y código 3).
        /// </summary>
        public int TrabajoEnDescanso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico > 0 && d.Codigo == 3);
            }
        }


        /// <summary>
        /// Descansos en fin de semana del mes actual (FN => -3).
        /// </summary>
        public int DescansoEnFinde {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -3 || d.Grafico == -11 || d.Grafico == -13) && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Días de enfermedad o baja del mes actual (E => -4).
        /// </summary>
        public int Enfermo {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -4 || d.Grafico == -10 || d.Grafico == -11);
            }
        }


        /// <summary>
        /// Días de enfermedad o baja del mes actual (E(JD) => -10).
        /// </summary>
        public int EnfermoEnJD {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -10);
            }
        }


        /// <summary>
        /// Días de enfermedad o baja del mes actual (E(FN) => -11).
        /// </summary>
        public int EnfermoEnFN {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -11);
            }
        }


        /// <summary>
        /// Descansos sueltos del mes actual (DS => -5).
        /// </summary>
        public int DescansoSuelto {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -5 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Descansos compensatorios del mes actual (DC => -6).
        /// </summary>
        public int DescansoCompensatorio {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -6 && !(d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Días de permiso del mes actual (PER => -9).
        /// </summary>
        public int Permiso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -9);
            }
        }


        /// <summary>
        /// Días de libre disposición del mes actual (F6 => -7).
        /// </summary>
        public int LibreDisposicionF6 {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -7);
            }
        }


        /// <summary>
        /// Días de libre disposición a cuenta de DCs del mes actual (F6(DC) => -14).
        /// </summary>
        public int F6DC {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == -14);
            }
        }


        /// <summary>
        /// Días de libre disposición del mes actual (F6 => -7).
        /// </summary>
        public int DiasF6Totales {
            get {
                return LibreDisposicionF6 + F6DC;
            }
        }


        /// <summary>
        /// Días de comité del mes actual (código 1 o 2).
        /// </summary>
        public int Comite {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Días de comité en descanso del mes actual (gráfico J-D o FN y códigos 1 o 2).
        /// </summary>
        public int ComiteEnDescanso {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3 || d.Grafico == -5) && (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Días de comité en descansos compensatorios del mes actual (gráfico en DC y códigos 1 o 2).
        /// </summary>
        public int ComiteEnDC {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -6) && (d.Codigo == 1 || d.Codigo == 2));
            }
        }


        /// <summary>
        /// Dias activo del mes actual (gráfico diferente de cero).
        /// </summary>
        public int DiasActivo {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico != 0);
            }
        }


        /// <summary>
        /// Días inactivo del mes actual (gráfico = 0).
        /// </summary>
        public int DiasInactivo {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => d.Grafico == 0);
            }
        }


        /// <summary>
        /// Total días del mes actual.
        /// </summary>
        public int DiasMes {
            get {
                return DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
            }
        }


        /// <summary>
        /// Establece el número de días que se debería trabajar en este mes, teniendo en cuenta los datos del calendario.
        /// </summary>
        public decimal DiasComputoTrabajo3 {
            get {
                decimal findes = ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Saturday || d.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                findes += ListaDias.Count(d => d.EsFestivo && d.DiaFecha.DayOfWeek != DayOfWeek.Saturday);
                decimal dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                decimal cVac = App.Global.Convenio.VacacionesAnuales * dias / (DateTime.IsLeapYear(Fecha.Year) ? 366m : 365m);
                decimal cTrab = dias - findes - cVac;
                decimal resultado = DiasActivo * cTrab / dias;
                return resultado;
            }
        }

        public decimal DiasComputoTrabajo {
            get {
                var diasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                var descansosMes = ListaDias.Count(c => c.DiaFecha.DayOfWeek == DayOfWeek.Saturday || c.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                descansosMes += App.Global.FestivosVM.ListaFestivos.Count(f => f.Fecha.Month == Fecha.Month);
                decimal vacacionesPertenecen = App.Global.Convenio.VacacionesAnuales / 12m;
                decimal descansosPertenecen = (diasMes - vacacionesPertenecen) * descansosMes / diasMes;
                decimal deberiaTrabajo = Math.Round(DiasActivo - DiasComputoDescanso - DiasComputoVacaciones, 4);
                return deberiaTrabajo;
            }
        }



        /// <summary>
        /// Establece el número de días que se debería trabajar en este mes, teniendo en cuenta los datos del calendario.
        /// </summary>
        public decimal DiasComputoTrabajoHastaMes {
            get {
                decimal dias = 0;
                for (int mes = 1; mes <= Fecha.Month; mes++) {
                    dias += DateTime.DaysInMonth(Fecha.Year, mes);
                }
                decimal findes = 0;
                for (int mes = 1; mes <= Fecha.Month; mes++) {
                    for (int dia = 1; dia <= DateTime.DaysInMonth(Fecha.Year, mes); dia++) {
                        DateTime fecha = new DateTime(Fecha.Year, mes, dia);
                        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday || App.Global.FestivosVM.EsFestivo(fecha)) {
                            findes++;
                        }
                        if (fecha.DayOfWeek == DayOfWeek.Saturday && App.Global.FestivosVM.EsFestivo(fecha)) {
                            findes++;
                        }
                    }
                }
                decimal cVac = App.Global.Convenio.VacacionesAnuales * dias / (DateTime.IsLeapYear(Fecha.Year) ? 366m : 365m);
                decimal cTrab = dias - findes - cVac;
                decimal resultado = (dias - DiasInactivoHastaMesEnAño) * cTrab / dias;
                return resultado;
            }
        }


        /// <summary>
        /// Establece el número de días que debería descansar este mes, teniendo en cuenta los datos del calendario.
        /// </summary>
        public decimal DiasComputoDescanso3 {
            get {
                decimal findes = ListaDias.Count(d => d.DiaFecha.DayOfWeek == DayOfWeek.Saturday || d.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                findes += ListaDias.Count(d => d.EsFestivo && d.DiaFecha.DayOfWeek != DayOfWeek.Saturday);
                decimal dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                decimal resultado = DiasActivo * findes / dias;
                return resultado;
            }
        }

        public decimal DiasComputoDescanso {
            get {
                var diasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                var descansosMes = ListaDias.Count(c => c.DiaFecha.DayOfWeek == DayOfWeek.Saturday || c.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                descansosMes += App.Global.FestivosVM.ListaFestivos.Count(f => f.Fecha.Month == Fecha.Month);
                decimal vacacionesPertenecen = App.Global.Convenio.VacacionesAnuales / 12m;
                decimal descansosPertenecen = (diasMes - vacacionesPertenecen) * descansosMes / diasMes;
                decimal deberiaDescanso = Math.Round(DiasActivo * descansosPertenecen / diasMes, 4);
                return deberiaDescanso;
            }
        }


        /// <summary>
        /// Establece el número de días que debería descansar este mes, teniendo en cuenta los datos del calendario.
        /// </summary>
        public decimal DiasComputoDescansoHastaMes {
            get {
                decimal dias = 0;
                for (int mes = 1; mes <= Fecha.Month; mes++) {
                    dias += DateTime.DaysInMonth(Fecha.Year, mes);
                }
                decimal findes = 0;
                for (int mes = 1; mes <= Fecha.Month; mes++) {
                    for (int dia = 1; dia <= DateTime.DaysInMonth(Fecha.Year, mes); dia++) {
                        DateTime fecha = new DateTime(Fecha.Year, mes, dia);
                        if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday || App.Global.FestivosVM.EsFestivo(fecha)) {
                            findes++;
                        }
                        if (fecha.DayOfWeek == DayOfWeek.Saturday && App.Global.FestivosVM.EsFestivo(fecha)) {
                            findes++;
                        }
                    }
                }
                decimal resultado = (dias - DiasInactivoHastaMesEnAño) * findes / dias;
                return resultado;

            }
        }


        /// <summary>
        /// Establece el número de días que debería tener vacaciones este mes, teniendo en cuenta los datos del calendario.
        /// </summary>
        public decimal DiasComputoVacaciones3 {
            get {
                int dias = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                decimal cVac = App.Global.Convenio.VacacionesAnuales * dias / (DateTime.IsLeapYear(Fecha.Year) ? 366m : 365m);
                decimal resultado = DiasActivo * cVac / dias;
                return resultado;
            }
        }


        public decimal DiasComputoVacaciones {
            get {
                var diasMes = DateTime.DaysInMonth(Fecha.Year, Fecha.Month);
                var descansosMes = ListaDias.Count(c => c.DiaFecha.DayOfWeek == DayOfWeek.Saturday || c.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
                descansosMes += App.Global.FestivosVM.ListaFestivos.Count(f => f.Fecha.Month == Fecha.Month);
                decimal vacacionesPertenecen = App.Global.Convenio.VacacionesAnuales / 12m;
                decimal descansosPertenecen = (diasMes - vacacionesPertenecen) * descansosMes / diasMes;
                decimal deberiaVacaciones = Math.Round(DiasActivo * vacacionesPertenecen / diasMes, 4);
                return deberiaVacaciones;
            }
        }


        public decimal DiasComputoVacacionesHastaMes {
            get {
                int dias = 0;
                for (int mes = 1; mes <= Fecha.Month; mes++) {
                    dias += DateTime.DaysInMonth(Fecha.Year, mes);
                }
                decimal cVac = App.Global.Convenio.VacacionesAnuales * dias / (DateTime.IsLeapYear(Fecha.Year) ? 366m : 365m);
                decimal resultado = (dias - DiasInactivoHastaMesEnAño) * cVac / dias;
                return resultado;
            }
        }


        /// <summary>
        /// Texto con el tipo de eventual que es, en función de si trabaja todos los días del mes o no.
        /// </summary>
        public string TextoComputoEventuales {
            get {
                return DiasInactivo == 0 ? "(Eventual Completo)" : "(Eventual Parcial)";
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES FINES DE SEMANA
        // ====================================================================================================

        /// <summary>
        /// Sábados descansados del mes actual.
        /// </summary>
        public int SabadosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3 || d.Grafico == -10 || d.Grafico == -11) && d.DiaFecha.DayOfWeek == DayOfWeek.Saturday);
            }
        }


        /// <summary>
        /// Sábados trabajados del mes actual.
        /// </summary>
        public int SabadosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.DiaFecha.DayOfWeek == DayOfWeek.Saturday);
            }
        }


        /// <summary>
        /// Plus de sábados del mes actual (si el sábado es festivo, este plus no se tiene en cuenta).
        /// </summary>
        public decimal PlusSabados {
            get {
                int sabadosTrabajoNoFestivos = ListaDias.Count(d => (d.Grafico > 0 && !d.EsFestivo) && d.DiaFecha.DayOfWeek == DayOfWeek.Saturday);
                return sabadosTrabajoNoFestivos * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteSabados;
            }
        }


        /// <summary>
        /// Domingos descansados del mes actual.
        /// </summary>
        public int DomingosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3 || d.Grafico == -10 || d.Grafico == -11) && d.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
            }
        }


        /// <summary>
        /// Domingos trabajados del mes actual.
        /// </summary>
        public int DomingosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.DiaFecha.DayOfWeek == DayOfWeek.Sunday);
            }
        }


        /// <summary>
        /// Plus de domingos del mes actual.
        /// </summary>
        public decimal PlusDomingos {
            get {
                return DomingosTrabajados * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteFestivos;
            }
        }


        /// <summary>
        /// Festivos descansados del mes actual.
        /// </summary>
        public int FestivosDescansados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico == -2 || d.Grafico == -3 || d.Grafico == -10 || d.Grafico == -11) && d.EsFestivo);
            }
        }


        /// <summary>
        /// Festivos trabajados del mes actual.
        /// </summary>
        public int FestivosTrabajados {
            get {
                if (ListaDias == null) return 0;
                return ListaDias.Count(d => (d.Grafico > 0) && d.EsFestivo);
            }
        }


        /// <summary>
        /// Plus de festivos del mes actual.
        /// </summary>
        public decimal PlusFestivos {
            get {
                return FestivosTrabajados * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteFestivos;
            }
        }


        /// <summary>
        /// Fines de semana completos descansados del mes actual.
        /// </summary>
        public decimal FindesCompletos {
            get {
                decimal findes = 0m;
                bool sabado = false;
                foreach (DiaPijama dia in ListaDias) {
                    switch (dia.DiaFecha.DayOfWeek) {
                        case DayOfWeek.Saturday:
                            if (dia.Grafico == -2 || dia.Grafico == -3 || dia.Grafico == -10 || dia.Grafico == -11) sabado = true;
                            break;
                        case DayOfWeek.Sunday:
                            if (sabado) {
                                if (dia.Grafico == -2 || dia.Grafico == -3 || dia.Grafico == -10 || dia.Grafico == -11) findes += 1m;
                                sabado = false;
                            }
                            break;
                    }
                }
                // Si el día 1 es domingo y descanso, se marca medio finde.
                if (ListaDias[0].DiaFecha.DayOfWeek == DayOfWeek.Sunday) {
                    if (ListaDias[0].Grafico == -2 || ListaDias[0].Grafico == -3 || ListaDias[0].Grafico == -10 || ListaDias[0].Grafico == -11) findes += 0.5m;
                }
                // Si el último día es sábado y descanso, se marca medio finde.
                if (ListaDias[ListaDias.Count - 1].DiaFecha.DayOfWeek == DayOfWeek.Saturday) {
                    if (ListaDias[ListaDias.Count - 1].Grafico == -2 || ListaDias[ListaDias.Count - 1].Grafico == -3 ||
                        ListaDias[ListaDias.Count - 1].Grafico == -10 || ListaDias[ListaDias.Count - 1].Grafico == -11) findes += 0.5m;
                }
                return findes;
            }
        }


        /// <summary>
        /// Importe total de los pluses de sábados, domingos y festivos del mes actual.
        /// </summary>
        public decimal ImporteTotalFindes {
            get {
                return PlusSabados + PlusDomingos + PlusFestivos;
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES DIETAS
        // ====================================================================================================

        /// <summary>
        /// Dieta de desayuno del mes actual (sin aplicar el porcentaje de descuento).
        /// </summary>
        public decimal DietaDesayuno {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.GraficoTrabajado.Desayuno);
            }
        }


        /// <summary>
        /// Dieta de comida del mes actual.
        /// </summary>
        public decimal DietaComida {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.GraficoTrabajado.Comida);
            }
        }


        /// <summary>
        /// Dieta de cena del mes actual.
        /// </summary>
        public decimal DietaCena {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.GraficoTrabajado.Cena);
            }
        }


        /// <summary>
        /// Dieta de Plus Cena del mes actual (aplicado ya el porcentaje).
        /// </summary>
        public decimal DietaPlusCena {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.GraficoTrabajado.PlusCena);
            }
        }


        /// <summary>
        /// Suma total de las dietas del mes actual.
        /// </summary>
        public decimal TotalDietas {
            get {
                if (ListaDias == null) return 0m;
                return (DietaDesayuno * App.Global.Convenio.PorcentajeDesayuno / 100) + DietaComida + DietaCena + DietaPlusCena;
            }
        }


        /// <summary>
        /// Importe total de las dietas del mes actual (el desayuno ya tiene aplicado el porcentaje de descuento).
        /// </summary>
        public decimal ImporteTotalDietas {
            get {
                return Math.Round(TotalDietas, 2) * App.Global.OpcionesVM.GetPluses(Fecha.Year).ImporteDietas;
            }
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES PLUSES
        // ====================================================================================================

        /// <summary>
        /// Plus por menor descanso del mes actual.
        /// </summary>
        public decimal PlusMenorDescanso {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusMenorDescanso);
            }
        }


        /// <summary>
        /// Plus de limpieza del mes actual.
        /// </summary>
        public decimal PlusLimpieza {
            get {
                if (App.Global.PorCentro.PagarLimpiezas == true) {
                    if (Trabajador.Indefinido) {
                        var dias = App.Global.PorCentro.NumeroLimpiezas;
                        dias -= (ListaDias.Count(d => d.Grafico == -4) + DescansosNoDisfrutados + LibreDisposicionF6 + Permiso);
                        return dias * App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    } else {
                        var dias = (Trabajo + DescansoCompensatorio);
                        if (dias > 21) dias = 21;
                        return dias * App.Global.OpcionesVM.GetPluses(Fecha.Year).PlusLimpieza;
                    }
                } else {
                    if (ListaDias == null) return 0m;
                    return ListaDias.Sum(x => x.PlusLimpieza);
                }
            }
        }


        /// <summary>
        /// Plus de paquetería del mes actual.
        /// </summary>
        public decimal PlusPaqueteria {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusPaqueteria);
            }
        }


        /// <summary>
        /// Plus de nocturnidad del mes actual.
        /// </summary>
        public decimal PlusNocturnidad {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusNocturnidad);
            }
        }


        /// <summary>
        /// Plus de viaje del mes actual.
        /// </summary>
        public decimal PlusViaje {
            get {
                if (App.Global.PorCentro.PagarPlusViaje == true) return Trabajo * App.Global.PorCentro.PlusViaje;
                return 0m;
            }
        }


        /// <summary>
        /// Plus de navidad del mes actual.
        /// </summary>
        public decimal PlusNavidad {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.PlusNavidad);
            }
        }


        /// <summary>
        /// Suma del quebranto de moneda de todo el mes.
        /// </summary>
        public decimal QuebrantoMoneda {
            get {
                if (ListaDias == null) return 0m;
                return ListaDias.Sum(x => x.QuebrantoMoneda);
            }
        }


        /// <summary>
        /// Suma de los pluses de Nocturnidad, viaje y navidad del mes actual.
        /// </summary>
        public decimal OtrosPluses {
            get {
                return PlusNocturnidad + PlusViaje + PlusNavidad;
            }
        }


        /// <summary>
        /// Suma total de los pluses del mes actual (excepto los pluses de fin de semana).
        /// </summary>
        public decimal ImporteTotalPluses {
            get {
                return PlusMenorDescanso + QuebrantoMoneda + PlusPaqueteria + PlusLimpieza + PlusNocturnidad + PlusNavidad + PlusViaje;
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES GRÁFICOS
        // ====================================================================================================


        public ChartValues<int> SerieTurnos {
            get => new ChartValues<int>(ListaDias.Select(d => d.Grafico > 0 ? d.TurnoAlt.HasValue ? d.TurnoAlt.Value : d.GraficoTrabajado.Turno : -1).ToArray());
        }



        public ChartValues<int> SerieInicios {
            get => new ChartValues<int>(ListaDias.Select(d => {
                int inicio = d.Grafico > 0 ? d.InicioAlt.HasValue ? (int)d.InicioAlt.Value.TotalMinutes : d.GraficoTrabajado.Inicio.HasValue ? (int)d.GraficoTrabajado.Inicio.Value.TotalMinutes : 0 : 0;
                return inicio;
            }));
        }


        //public ChartValues<int> SerieFinales {
        //    get => new ChartValues<int>(ListaDias.Select(d => {
        //        int inicio = d.Grafico > 0 ? d.InicioAlt.HasValue ? (int)d.InicioAlt.Value.TotalMinutes : (int)d.GraficoTrabajado.Inicio.Value.TotalMinutes : 0;
        //        int final = d.Grafico > 0 ? d.FinalAlt.HasValue ? (int)d.FinalAlt.Value.TotalMinutes : (int)d.GraficoTrabajado.Final.Value.TotalMinutes : 0;
        //        if (final < inicio) final += 1440;
        //        return (final - inicio);
        //    })); 
        //}
        public ChartValues<int> SerieFinales {
            get => new ChartValues<int>(ListaDias.Select(d => d.Grafico > 0 ? d.FinalAlt.HasValue ? (int)d.FinalAlt.Value.TotalMinutes : (int)d.GraficoTrabajado.Final.Value.TotalMinutes : 0));
        }


        public ChartValues<OhlcPoint> SerieHorarios {
            get => new ChartValues<OhlcPoint>(ListaDias.Select(d => {
                int inicio = 0;
                int final = 0;
                int turno = 0;
                if (d.Grafico > 0) {
                    inicio = d.InicioAlt.HasValue ? (int)d.InicioAlt.Value.TotalMinutes : d.GraficoTrabajado.Inicio.HasValue ? (int)d.GraficoTrabajado.Inicio.Value.TotalMinutes : 0;
                    final = d.FinalAlt.HasValue ? (int)d.FinalAlt.Value.TotalMinutes : d.GraficoTrabajado.Final.HasValue ? (int)d.GraficoTrabajado.Final.Value.TotalMinutes : 0;
                    turno = d.TurnoAlt.HasValue ? d.TurnoAlt.Value : d.GraficoTrabajado.Turno;
                }
                int ip = 2;
                int fp = 2;
                if (turno == 1) {
                    fp = 1;
                } else {
                    fp = 3;
                }
                return new OhlcPoint(ip, inicio, final, fp);
            }).ToArray());
        }


        public Func<double, string> TurnoFormatter {
            get => x => (x).ToString("0");
        }

        public Func<double, string> TimeFormatter {
            get => x => {
                return new TimeSpan(0, (int)x, 0).ToTexto();
            };
        }

        public Func<double, string> DiaFormatter {
            get => x => (x + 1).ToString("00");
        }

        #endregion
        // ====================================================================================================


    }
}
