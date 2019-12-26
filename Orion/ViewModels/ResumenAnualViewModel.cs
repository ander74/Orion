#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Orion.DataModels;
    using Orion.Models;
    using Orion.Servicios;

    public partial class ResumenAnualViewModel : NotifyBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private IMensajes mensajes;
        private InformesServicio informes;

        private Dictionary<int, Pijama.HojaPijama> pijamasAño;


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        public ResumenAnualViewModel(IMensajes servicioMensajes, InformesServicio servicioInformes) {
            this.mensajes = servicioMensajes;
            this.informes = servicioInformes;
            // Llenamos la lista de conductores.
            ListaConductores = App.Global.ConductoresVM.ListaConductores;
            AñoActual = DateTime.Now.Year;

            // Creamos el formato de las etiquetas de las horas trabajadas
            FormatoTrabajadas = valor => {
                decimal total = App.Global.Convenio.HorasAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 2) : 0;
                return $"{valor:0.00}\n {porcentaje:0.00} %".Replace(".", ",");
            };

            // Creamos el formato de las etiquetas de los dias trabajados
            FormatoDiasTrabajo = valor => {
                decimal total = App.Global.Convenio.TrabajoAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 2) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };

            // Creamos el formato de las etiquetas de los dias descansados
            FormatoDiasDescanso = valor => {
                decimal total = App.Global.Convenio.DescansosAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 2) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };

            // Creamos el formato de las etiquetas de los dias de vacaciones
            FormatoDiasVacaciones = valor => {
                decimal total = App.Global.Convenio.VacacionesAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 2) : 0;
                return $"{valor:00}\n {porcentaje:0.00} %".Replace(".", ",");
            };

            // Creamos el formato de las etiquetas de los fines de semana
            FormatoTotalFindes = valor => {
                decimal total = App.Global.Convenio.FindesCompletosAnuales;
                decimal porcentaje = total > 0 ? Math.Round(Convert.ToDecimal(valor) * 100m / total, 2) : 0;
                return $"{valor:0.00}\n {porcentaje:0.00} %".Replace(".", ",");
            };

        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public void Reiniciar() {
            ConductorActual = null;
            ListaConductores = App.Global.ConductoresVM.ListaConductores;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private async void CargarDatos() {

            // Si no existe el conductor activo, salimos.
            if (ConductorActual == null) return;

            // Iniciamos la recepción de los pijamas del conductor.
            try {
                // Desactivamos los botones.
                BotonesActivos = false;
                // Iniciamos la barra de progreso.
                App.Global.IniciarProgreso("Recopilando datos...");
                // Solicitamos los pijamas
                pijamasAño = await GetPijamas(AñoActual, ConductorActual.Matricula);


                //// Cargamos los calendarios del año del conductor.
                //List<Calendario> listaCalendarios = BdCalendarios.GetCalendariosConductor(AñoActual, ConductorActual.Id);
                //// Inicializamos la lista de pijamas del año.
                //pijamasAño = new Dictionary<int, Pijama.HojaPijama>();
                //// Iniciamos el valor para la barra de progreso.
                //double num = 1;
                //// Cargamos las hojas pijama disponibles.
                //foreach (Calendario cal in listaCalendarios)
                //{
                //	// Incrementamos la barra de progreso.
                //	App.Global.ValorBarraProgreso = num / listaCalendarios.Count * 100;
                //	// Cargamos un pijama nuevo.
                //	Pijama.HojaPijama hoja = null;
                //	await Task.Run(() => { hoja = new Pijama.HojaPijama(cal, mensajes); });
                //	// Añadimos el pijama a la lista.
                //	pijamasAño.Add(cal.Fecha.Month, hoja);
                //	// Incrementamos el valor de la barra de progreso.
                //	num++;
                //}


                // Llenar los datos con los resultados de los pijamas.
                LlenarListaResumen(AñoActual);

            } catch (Exception ex) {
                mensajes.VerError("ResumenAnualViewModel.CargarDatos", ex);
            } finally {
                BotonesActivos = true;
                App.Global.FinalizarProgreso();
            }


        }


        private async Task<Dictionary<int, Pijama.HojaPijama>> GetPijamas(int año, int matriculaConductor) {
            return await Task.Run(() => {
                // Cargamos los calendarios del año del conductor.
                List<Calendario> listaCalendarios = BdCalendarios.GetCalendariosConductor(año, matriculaConductor);
                // Creamos el diccionario que contendrá las hojas pijama.
                Dictionary<int, Pijama.HojaPijama> pijamasAño = new Dictionary<int, Pijama.HojaPijama>();
                // Iniciamos el valor para la barra de progreso.
                double num = 1;
                // Cargamos las hojas pijama disponibles.
                foreach (Calendario cal in listaCalendarios) {
                    // Incrementamos la barra de progreso.
                    App.Global.ValorBarraProgreso = num / listaCalendarios.Count * 100;
                    // Añadimos el pijama a la lista.
                    pijamasAño.Add(cal.Fecha.Month, new Pijama.HojaPijama(cal, mensajes));
                    // Incrementamos el valor de la barra de progreso.
                    num++;
                }
                return pijamasAño;
            });
        }


        private void LlenarListaResumen(int año) {
            // Reiniciamos la lista y los valores totales.
            ListaResumen = new List<ItemResumenAnual>();
            TotalTrabajadas = 0;
            TotalDiasTrabajo = 0;
            TotalDiasTrabajoEventual = 0;
            TotalDiasDescanso = 0;
            TotalDiasDescansoEventual = 0;
            TotalDiasVacaciones = 0;
            TotalDiasVacacionesEventual = 0;
            TotalFindes = 0;
            DNDsPendientes = 0;
            DCsPendientes = 0;
            AcumuladasPendientes = TimeSpan.Zero;
            DCsGenerados = 0;

            // Definimos los items a usar.
            ItemResumenAnual Trabajadas = new ItemResumenAnual("Horas Trabajadas");
            ItemResumenAnual Acumuladas = new ItemResumenAnual("Horas Acumuladas");
            ItemResumenAnual Nocturnas = new ItemResumenAnual("Horas Nocturnas");
            ItemResumenAnual Cobradas = new ItemResumenAnual("Horas Cobradas");
            ItemResumenAnual ExcesoJornada = new ItemResumenAnual("Excesos de Jornada");
            ItemResumenAnual OtrasHoras = new ItemResumenAnual("Otras Horas");
            ItemResumenAnual DiasTrabajo = new ItemResumenAnual("Dias Trabajados");
            ItemResumenAnual DiasJD = new ItemResumenAnual("Días J-D");
            ItemResumenAnual DiasOV = new ItemResumenAnual("Días Vacaciones");
            if (!ConductorActual.Indefinido) {
                DiasTrabajo.Descripcion = "Dias Trabajados (Event.)";
                DiasJD.Descripcion = "Días J-D (Event.)";
                DiasOV.Descripcion = "Días Vacaciones (Event.)";
            }
            ItemResumenAnual DiasDND = new ItemResumenAnual("Descansos No Disfrutados");
            ItemResumenAnual DiasTrabajoJD = new ItemResumenAnual("Días Trabajados en JD");
            ItemResumenAnual DiasFN = new ItemResumenAnual("Descansos en Fin de Semana");
            ItemResumenAnual DiasE = new ItemResumenAnual("Días Enfermo");
            ItemResumenAnual DiasDS = new ItemResumenAnual("Descansos Sueltos");
            ItemResumenAnual DiasDC = new ItemResumenAnual("Descansos Compensatorios");
            ItemResumenAnual DiasPER = new ItemResumenAnual("Días de Permiso");
            ItemResumenAnual DiasF6 = new ItemResumenAnual("Días Libre Disposición (F6)");
            ItemResumenAnual DiasComite = new ItemResumenAnual("Días de Comité");
            ItemResumenAnual DiasComiteJD = new ItemResumenAnual("Días de Comité en JD");
            ItemResumenAnual DiasComiteDC = new ItemResumenAnual("Días de Comité en DC");
            ItemResumenAnual SabadosTrabajados = new ItemResumenAnual("Sábados Trabajados");
            ItemResumenAnual SabadosDescansados = new ItemResumenAnual("Sábados Descansados");
            ItemResumenAnual DomingosTrabajados = new ItemResumenAnual("Domingos Trabajados");
            ItemResumenAnual DomingosDescansados = new ItemResumenAnual("Domingos Descansados");
            ItemResumenAnual FestivosTrabajados = new ItemResumenAnual("Festivos Trabajados");
            ItemResumenAnual FestivosDescansados = new ItemResumenAnual("Festivos Descansados");
            ItemResumenAnual FindesCompletos = new ItemResumenAnual("Fines de Semana Completos");
            for (int mes = 1; mes < 13; mes++) {
                // Si la lista de pijamas no tiene el mes, continuamos.
                if (!pijamasAño.ContainsKey(mes)) continue;
                // HORAS
                Trabajadas.SetDato(mes, pijamasAño[mes].Trabajadas);
                TotalTrabajadas += pijamasAño[mes].Trabajadas.ToDecimal();
                Acumuladas.SetDato(mes, pijamasAño[mes].Acumuladas);
                Nocturnas.SetDato(mes, pijamasAño[mes].Nocturnas);
                Cobradas.SetDato(mes, pijamasAño[mes].HorasCobradas);
                ExcesoJornada.SetDato(mes, pijamasAño[mes].ExcesoJornada);
                OtrasHoras.SetDato(mes, pijamasAño[mes].OtrasHoras);
                // DÍAS
                if (ConductorActual.Indefinido) {
                    DiasTrabajo.SetDato(mes, pijamasAño[mes].Trabajo);
                } else {
                    DiasTrabajo.SetDatoEventual(mes, pijamasAño[mes].Trabajo, pijamasAño[mes].DiasComputoTrabajo);
                }
                TotalDiasTrabajo += pijamasAño[mes].Trabajo; // Añadimos al total de trabajados.
                if (ConductorActual.Indefinido) {
                    DiasJD.SetDato(mes, pijamasAño[mes].Descanso + pijamasAño[mes].DescansoEnFinde + pijamasAño[mes].DescansoSuelto);
                } else {
                    DiasJD.SetDatoEventual(mes, pijamasAño[mes].Descanso + pijamasAño[mes].DescansoEnFinde + pijamasAño[mes].DescansoSuelto, pijamasAño[mes].DiasComputoDescanso);
                }
                TotalDiasDescanso += pijamasAño[mes].Descanso; // Añadimos al total de descansos.
                TotalDiasDescanso += pijamasAño[mes].DescansoEnFinde;
                TotalDiasDescanso += pijamasAño[mes].DescansoSuelto;
                if (ConductorActual.Indefinido) {
                    DiasOV.SetDato(mes, pijamasAño[mes].Vacaciones);
                } else {
                    DiasOV.SetDatoEventual(mes, pijamasAño[mes].Vacaciones, pijamasAño[mes].DiasComputoVacaciones);
                }
                TotalDiasVacaciones += pijamasAño[mes].Vacaciones;
                DiasDND.SetDato(mes, pijamasAño[mes].DescansosNoDisfrutados);
                TotalDiasDescanso += pijamasAño[mes].DescansosNoDisfrutados; // Añadimos DND al total de descansos. **** DUDA ****
                DiasTrabajoJD.SetDato(mes, pijamasAño[mes].TrabajoEnDescanso);
                DiasFN.SetDato(mes, pijamasAño[mes].DescansoEnFinde);
                TotalDiasDescanso += pijamasAño[mes].DescansoEnFinde; // Añadimos al total de descansos.
                DiasE.SetDato(mes, pijamasAño[mes].Enfermo);
                TotalDiasTrabajo += pijamasAño[mes].Enfermo; // Añadimos E al total de trabajados
                DiasDS.SetDato(mes, pijamasAño[mes].DescansoSuelto);
                TotalDiasDescanso += pijamasAño[mes].DescansoSuelto; // Añadimos DS al total de descansos.
                DiasDC.SetDato(mes, pijamasAño[mes].DescansoCompensatorio);
                TotalDiasTrabajo += pijamasAño[mes].DescansoCompensatorio; // Añadimos DC al total de trabajados.
                DiasPER.SetDato(mes, pijamasAño[mes].Permiso);
                TotalDiasTrabajo += pijamasAño[mes].Permiso; // Añadimos PER al total de trabajados.
                DiasF6.SetDato(mes, pijamasAño[mes].LibreDisposicionF6);
                TotalDiasTrabajo += pijamasAño[mes].LibreDisposicionF6; // Añadimos F6 al total de trabajados.
                TotalDiasDescanso += pijamasAño[mes].EnfermoEnJD; // Añadimos E(JD) al total de descansos.
                TotalDiasDescanso += pijamasAño[mes].EnfermoEnFN; // Añadimos E(FN) al total de descansos.
                                                                  // COMITÉ
                DiasComite.SetDato(mes, pijamasAño[mes].Comite);
                DiasComiteJD.SetDato(mes, pijamasAño[mes].ComiteEnDescanso);
                DiasComiteDC.SetDato(mes, pijamasAño[mes].ComiteEnDC);
                // FINES DE SEMANA
                SabadosTrabajados.SetDato(mes, pijamasAño[mes].SabadosTrabajados);
                SabadosDescansados.SetDato(mes, pijamasAño[mes].SabadosDescansados);
                DomingosTrabajados.SetDato(mes, pijamasAño[mes].DomingosTrabajados);
                DomingosDescansados.SetDato(mes, pijamasAño[mes].DomingosDescansados);
                FestivosTrabajados.SetDato(mes, pijamasAño[mes].FestivosTrabajados);
                FestivosDescansados.SetDato(mes, pijamasAño[mes].FestivosDescansados);
                // FINES DE SEMANA COMPLETOS
                decimal findes = pijamasAño[mes].FindesCompletos;
                // Último día del mes.
                if (mes > 1) {
                    bool sabadoDescanso = false;
                    bool domingoDescanso = false;
                    int ultimoDiaMesAnterior = DateTime.DaysInMonth(año, mes - 1);
                    DayOfWeek ultimoDiaSemanaMesAnterior = new DateTime(año, mes - 1, ultimoDiaMesAnterior).DayOfWeek;
                    DayOfWeek primerDiaSemanaMes = new DateTime(año, mes, 1).DayOfWeek;
                    // Si el último día del mes anterior es sábado y descanso, lo marcamos.
                    if (ultimoDiaSemanaMesAnterior == DayOfWeek.Saturday && EsDescanso(pijamasAño[mes - 1].ListaDias[ultimoDiaMesAnterior - 1].Grafico)) {
                        sabadoDescanso = true;
                    }
                    // Si el primer día del mes es domingo y descanso, lo marcamos.
                    if (primerDiaSemanaMes == DayOfWeek.Sunday && EsDescanso(pijamasAño[mes].ListaDias[0].Grafico)) {
                        domingoDescanso = true;
                    }
                    // Si uno de los dos días del fin de semana dividido entre dos meses es descanso, pero el otro no, 
                    // eliminamos el 0,5 que se ha generado en el pijama.
                    if (sabadoDescanso && !domingoDescanso) findes = findes > 0 ? findes - 0.5m : 0m;
                    if (domingoDescanso && !sabadoDescanso) findes = findes > 0 ? findes - 0.5m : 0m;
                }
                FindesCompletos.SetDato(mes, findes);
                TotalFindes += findes;

                // RESUMEN HASTA MES ACTUAL
                DNDsPendientes = pijamasAño[mes].DNDsPendientesHastaMes;
                DCsPendientes = pijamasAño[mes].DCsPendientesHastaMes;
                AcumuladasPendientes = pijamasAño[mes].AcumuladasHastaMes;
                DCsGenerados = pijamasAño[mes].DCsGeneradosHastaMes;
            }
            // Añadimos los items a la lista.
            ListaResumen.Add(Trabajadas);
            ListaResumen.Add(Acumuladas);
            ListaResumen.Add(Nocturnas);
            ListaResumen.Add(Cobradas);
            ListaResumen.Add(ExcesoJornada);
            ListaResumen.Add(OtrasHoras);
            ListaResumen.Add(DiasTrabajo);
            ListaResumen.Add(DiasJD);
            ListaResumen.Add(DiasOV);
            ListaResumen.Add(DiasDND);
            ListaResumen.Add(DiasTrabajoJD);
            ListaResumen.Add(DiasFN);
            ListaResumen.Add(DiasE);
            ListaResumen.Add(DiasDS);
            ListaResumen.Add(DiasDC);
            ListaResumen.Add(DiasPER);
            ListaResumen.Add(DiasF6);
            ListaResumen.Add(DiasComite);
            ListaResumen.Add(DiasComiteJD);
            ListaResumen.Add(DiasComiteDC);
            ListaResumen.Add(SabadosTrabajados);
            ListaResumen.Add(SabadosDescansados);
            ListaResumen.Add(DomingosTrabajados);
            ListaResumen.Add(DomingosDescansados);
            ListaResumen.Add(FestivosTrabajados);
            ListaResumen.Add(FestivosDescansados);
            ListaResumen.Add(FindesCompletos);
        }


        private bool EsDescanso(int grafico) {
            return (grafico == -2 || grafico == -3 || grafico == -10 || grafico == -11 || grafico == -12 || grafico == -13);
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        private ObservableCollection<Conductor> listaConductores;
        public ObservableCollection<Conductor> ListaConductores {
            get { return listaConductores; }
            set {
                if (listaConductores != value) {
                    listaConductores = value;
                    PropiedadCambiada();
                }
            }
        }


        private Conductor conductorActual;
        public Conductor ConductorActual {
            get { return conductorActual; }
            set {
                if (conductorActual != value) {
                    conductorActual = value;
                    CargarDatos();
                    PropiedadCambiada();
                }
            }
        }


        private int añoActual;
        public int AñoActual {
            get { return añoActual; }
            set {
                if (añoActual != value) {
                    añoActual = value;
                    CargarDatos();
                    PropiedadCambiada();
                }
            }
        }


        private List<ItemResumenAnual> listaResumen;
        public List<ItemResumenAnual> ListaResumen {
            get { return listaResumen; }
            set {
                if (listaResumen != value) {
                    listaResumen = value;
                    PropiedadCambiada();
                }
            }
        }


        private bool botonesActivos = true;
        public bool BotonesActivos {
            get { return botonesActivos; }
            set {
                if (botonesActivos != value) {
                    botonesActivos = value;
                    PropiedadCambiada();
                }
            }
        }



        private decimal totalTrabajadas;
        public decimal TotalTrabajadas {
            get { return totalTrabajadas; }
            set {
                if (totalTrabajadas != value) {
                    totalTrabajadas = value;
                    PropiedadCambiada();
                }
            }
        }


        private int totalDiasTrabajo;
        public int TotalDiasTrabajo {
            get { return totalDiasTrabajo; }
            set {
                if (totalDiasTrabajo != value) {
                    totalDiasTrabajo = value;
                    PropiedadCambiada();
                }
            }
        }



        private int totalDiasTrabajoEventual;
        public int TotalDiasTrabajoEventual {
            get { return totalDiasTrabajoEventual; }
            set { SetValue(ref totalDiasTrabajoEventual, value); }
        }



        private int totalDiasDescanso;
        public int TotalDiasDescanso {
            get { return totalDiasDescanso; }
            set {
                if (totalDiasDescanso != value) {
                    totalDiasDescanso = value;
                    PropiedadCambiada();
                }
            }
        }


        private int totalDiasDescansoEventual;
        public int TotalDiasDescansoEventual {
            get { return totalDiasDescansoEventual; }
            set { SetValue(ref totalDiasDescansoEventual, value); }
        }


        private int totalDiasVacaciones;
        public int TotalDiasVacaciones {
            get { return totalDiasVacaciones; }
            set {
                if (totalDiasVacaciones != value) {
                    totalDiasVacaciones = value;
                    PropiedadCambiada();
                }
            }
        }


        private int totalDiasVacacionesEventual;
        public int TotalDiasVacacionesEventual {
            get { return totalDiasVacacionesEventual; }
            set { SetValue(ref totalDiasVacacionesEventual, value); }
        }


        private decimal totalFindes;
        public decimal TotalFindes {
            get { return totalFindes; }
            set {
                if (totalFindes != value) {
                    totalFindes = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatotrabajadas;
        public Func<double, string> FormatoTrabajadas {
            get { return _formatotrabajadas; }
            set {
                if (_formatotrabajadas != value) {
                    _formatotrabajadas = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatodiastrabajo;
        public Func<double, string> FormatoDiasTrabajo {
            get { return _formatodiastrabajo; }
            set {
                if (_formatodiastrabajo != value) {
                    _formatodiastrabajo = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatodiasdescanso;
        public Func<double, string> FormatoDiasDescanso {
            get { return _formatodiasdescanso; }
            set {
                if (_formatodiasdescanso != value) {
                    _formatodiasdescanso = value;
                    PropiedadCambiada();
                }
            }
        }

        private Func<double, string> _formatodiasvacaciones;
        public Func<double, string> FormatoDiasVacaciones {
            get { return _formatodiasvacaciones; }
            set {
                if (_formatodiasvacaciones != value) {
                    _formatodiasvacaciones = value;
                    PropiedadCambiada();
                }
            }
        }


        private Func<double, string> _formatototalfindes;
        public Func<double, string> FormatoTotalFindes {
            get { return _formatototalfindes; }
            set {
                if (_formatototalfindes != value) {
                    _formatototalfindes = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal dcspendientes; //Cambiamos el tipo int por decimal.
        public decimal DCsPendientes { //Cambiamos el tipo int por decimal.
            get { return dcspendientes; }
            set {
                if (dcspendientes != value) {
                    dcspendientes = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal dndspendientes;
        public decimal DNDsPendientes {
            get { return dndspendientes; }
            set {
                if (dndspendientes != value) {
                    dndspendientes = value;
                    PropiedadCambiada();
                }
            }
        }


        private TimeSpan acumuladaspendientes;
        public TimeSpan AcumuladasPendientes {
            get { return acumuladaspendientes; }
            set {
                if (acumuladaspendientes != value) {
                    acumuladaspendientes = value;
                    PropiedadCambiada();
                }
            }
        }


        private decimal dcsgenerados;
        public decimal DCsGenerados {
            get { return dcsgenerados; }
            set {
                if (dcsgenerados != value) {
                    dcsgenerados = value;
                    PropiedadCambiada();
                }
            }
        }



        #endregion
        // ====================================================================================================




    }
}
