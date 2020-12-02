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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Convertidores;
    using Models;
    using Newtonsoft.Json;
    using OfficeOpenXml;
    using Orion.Config;
    using Orion.PdfExcel;
    using PrintModel;
    using Servicios;
    using Views;
    using static Orion.Servicios.ExcelService;

    public partial class CalendariosViewModel {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
        private static ConvertidorSuperHora cnvSuperHora = new ConvertidorSuperHora();
        private static ConvertidorNumeroGrafico cnvNumeroGrafico = new ConvertidorNumeroGrafico();

        #endregion
        // ====================================================================================================


        #region RETROCEDE MES
        private ICommand _cmdretrocedemes;
        public ICommand cmdRetrocedeMes {
            get {
                if (_cmdretrocedemes == null) _cmdretrocedemes = new RelayCommand(p => RetrocedeMes());
                return _cmdretrocedemes;
            }
        }

        private void RetrocedeMes() {
            FechaActual = FechaActual.AddMonths(-1);
        }
        #endregion


        #region AVANZA MES
        private ICommand _cmdavanzames;
        public ICommand cmdAvanzaMes {
            get {
                if (_cmdavanzames == null) _cmdavanzames = new RelayCommand(p => AvanzaMes());
                return _cmdavanzames;
            }
        }

        private void AvanzaMes() {
            FechaActual = FechaActual.AddMonths(1);
        }
        #endregion


        #region BORRAR CALENDARIO
        private ICommand _cmdborrarcalendario;
        public ICommand cmdBorrarCalendario {
            get {
                if (_cmdborrarcalendario == null) _cmdborrarcalendario = new RelayCommand(p => BorrarCalendario(), p => PuedeBorrarCalendario());
                return _cmdborrarcalendario;
            }
        }

        private bool PuedeBorrarCalendario() {
            return CalendarioSeleccionado != null;
        }

        private void BorrarCalendario() {
            _listaborrados.Add(CalendarioSeleccionado);
            _listacalendarios.Remove(CalendarioSeleccionado);
            HayCambios = true;
        }
        #endregion


        #region BORRAR TODOS
        private ICommand _cmdborrartodos;
        public ICommand cmdBorrarTodos {
            get {
                if (_cmdborrartodos == null) _cmdborrartodos = new RelayCommand(p => BorrarTodos(), p => PuedeBorrarTodos());
                return _cmdborrartodos;
            }
        }

        private bool PuedeBorrarTodos() {
            return VistaCalendarios.Count > 1;
        }

        private void BorrarTodos() {
            string titulo = "Borrar Calendarios";
            string mensaje = $"ATENCIÓN.\n\nSe van a borrar todos los calendarios.\n\n¿Desea continuar?";
            if (Mensajes.VerMensaje(mensaje, titulo, true) == true) {
                foreach (var cal in _listacalendarios) {
                    _listaborrados.Add(cal);
                }
                _listacalendarios.Clear();
                HayCambios = true;
            }
        }
        #endregion


        #region DESHACER BORRAR
        private ICommand _cmddeshacerborrar;
        public ICommand cmdDeshacerBorrar {
            get {
                if (_cmddeshacerborrar == null) _cmddeshacerborrar = new RelayCommand(p => DeshacerBorrar(), p => PuedeDeshacerBorrar());
                return _cmddeshacerborrar;
            }
        }

        private bool PuedeDeshacerBorrar() {
            return _listaborrados.Count > 0;
        }

        private void DeshacerBorrar() {
            if (_listaborrados == null) return;
            foreach (Calendario calendario in _listaborrados) {
                if (calendario.Nuevo) {
                    _listacalendarios.Add(calendario);
                } else {
                    _listacalendarios.Add(calendario);
                    calendario.Nuevo = false;
                }
                HayCambios = true;
            }
            _listaborrados.Clear();
        }
        #endregion


        #region QUITAR FILTRO
        private ICommand _cmdquitarfiltro;
        public ICommand cmdQuitarFiltro {
            get {
                if (_cmdquitarfiltro == null) _cmdquitarfiltro = new RelayCommand(p => QuitarFiltro());
                return _cmdquitarfiltro;
            }
        }

        private void QuitarFiltro() {
            if (VistaCalendarios != null) VistaCalendarios.Filter = null;
            FiltroAplicado = "Ninguno";
        }
        #endregion


        #region APLICAR FILTRO
        private ICommand _cmdaplicarfiltro;
        public ICommand cmdAplicarFiltro {
            get {
                if (_cmdaplicarfiltro == null) _cmdaplicarfiltro = new RelayCommand(p => AplicarFiltro(p), p => PuedeAplicarFiltro());
                return _cmdaplicarfiltro;
            }
        }

        private bool PuedeAplicarFiltro() => ListaCalendarios.Count > 0;


        private void AplicarFiltro(object parametro) {
            string filtro = parametro as string;
            if (filtro == null) return;
            switch (filtro) {
                case "Indefinidos":
                    VistaCalendarios.Filter = (c) => { return (c as Calendario).ConductorIndefinido; };
                    FiltroAplicado = "Conductores Indefinidos";
                    break;
                case "Eventuales":
                    VistaCalendarios.Filter = (c) => { return !(c as Calendario).ConductorIndefinido; };
                    FiltroAplicado = "Conductores Eventuales";
                    break;
                case "EventualesParcial":
                    VistaCalendarios.Filter = (c) => {
                        Calendario cal = c as Calendario;
                        int dias = DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month);
                        return (!cal.ConductorIndefinido && cal.ListaDias.Count(cc => cc.Grafico != 0) != dias);
                    };
                    FiltroAplicado = "Conductores Eventuales Parcial";
                    break;
            }
        }
        #endregion


        #region ABRIR PIJAMA

        private ICommand _cmdabrirpijama;
        public ICommand cmdAbrirPijama {
            get {
                if (_cmdabrirpijama == null) _cmdabrirpijama = new RelayCommand(p => AbrirPijama(), p => PuedeAbrirPijama());
                return _cmdabrirpijama;
            }
        }

        private bool PuedeAbrirPijama() {
            return CalendarioSeleccionado != null;
        }

        private void AbrirPijama() {
            if (HayCambios) GuardarCalendarios();
            IdConductorPijama = CalendarioSeleccionado.MatriculaConductor;
            Pijama = new Pijama.HojaPijama(CalendarioSeleccionado, Mensajes);
        }

        #endregion


        #region CERRAR PIJAMA
        private ICommand _cmdcerrarpijama;
        public ICommand cmdCerrarPijama {
            get {
                if (_cmdcerrarpijama == null) _cmdcerrarpijama = new RelayCommand(p => CerrarPijama());
                return _cmdcerrarpijama;
            }
        }

        private void CerrarPijama() {
            Pijama = null;
            DiaCalendarioSeleccionado = null;
        }
        #endregion


        #region CREAR PDF PIJAMA

        private ICommand _cmdcrearpdfpijama;
        public ICommand cmdCrearPdfPijama {
            get {
                if (_cmdcrearpdfpijama == null) _cmdcrearpdfpijama = new RelayCommand(p => CrearPdfPijama());
                return _cmdcrearpdfpijama;
            }
        }

        private async void CrearPdfPijama() {
            // Creamos el libro a usar.
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el nombre de archivo
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1}.pdf", FechaActual, Pijama.TextoTrabajador.Trim()).Replace(":", "");
                string ruta = Informes.GetRutaArchivo(TiposInforme.Pijama, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente, Pijama.TextoTrabajador.Replace(":", " -"));
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Hoja Pijama");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{Pijama.Trabajador.Matricula} - {Pijama.Fecha.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await PijamaPrintModel.CrearPijamaEnPdf(doc, Pijama);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.CrearPdfPijama", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }
        }


        #endregion


        #region PIJAMAS EN PDF

        private ICommand _cmdpijamasenpdf;
        public ICommand cmdPijamasEnPDF {
            get {
                if (_cmdpijamasenpdf == null) _cmdpijamasenpdf = new RelayCommand(p => PijamasEnPdf(), p => PuedePijamasEnPdf());
                return _cmdpijamasenpdf;
            }
        }

        private bool PuedePijamasEnPdf() {
            if (VistaCalendarios == null) return false;
            return VistaCalendarios.Count > 1;
        }

        private async void PijamasEnPdf() {

            try {
                // Pedimos el nombre de archivo
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM}", FechaActual);
                nombreArchivo += FiltroAplicado == "Ninguno" ? " - Todos" : $" - {FiltroAplicado}";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.Pijama, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Hojas Pijama");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await PijamaPrintModel.CrearTodosPijamasEnPdf(doc, VistaCalendarios);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.PijamasEnPDF", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }

        #endregion


        #region PIJAMAS SEPARADOS EN PDF

        private ICommand _cmdpijamasseparadosenpdf;
        public ICommand cmdPijamasSeparadosEnPdf {
            get {
                if (_cmdpijamasseparadosenpdf == null) _cmdpijamasseparadosenpdf = new RelayCommand(p => PijamasSeparadosEnPdf(), p => PuedePijamasSeparadosEnPdf());
                return _cmdpijamasseparadosenpdf;
            }
        }

        private bool PuedePijamasSeparadosEnPdf() {
            if (VistaCalendarios == null) return false;
            return VistaCalendarios.Count > 1;
        }

        private async void PijamasSeparadosEnPdf() {
            List<Pijama.HojaPijama> listaPijamas = new List<Pijama.HojaPijama>();
            try {
                double num = 1;
                await Task.Run(() => {
                    App.Global.IniciarProgreso($"Recopilando...");
                    // Recorremos todos los calendarios
                    foreach (object obj in VistaCalendarios) {
                        double valor = num / VistaCalendarios.Count * 100;
                        App.Global.ValorBarraProgreso = valor;
                        num++;
                        Calendario cal = obj as Calendario;
                        if (cal == null) continue;
                        Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(cal, new MensajesServicio());
                        hojapijama.Fecha = FechaActual;
                        listaPijamas.Add(hojapijama);
                    }
                });
                App.Global.IniciarProgreso($"Creando PDFs...");
                num = 1;
                foreach (Pijama.HojaPijama hojaPijama in listaPijamas) {
                    double valor = num / listaPijamas.Count * 100;
                    App.Global.ValorBarraProgreso = valor;
                    num++;
                    // Creamos la ruta de archivo
                    string nombreArchivo = String.Format($"{FechaActual:yyyy}-{FechaActual:MM} - {hojaPijama.TextoTrabajador.Replace(":", "")}.pdf");
                    string ruta = Informes.GetRutaArchivo(TiposInforme.Pijama, nombreArchivo, true, hojaPijama.TextoTrabajador.Replace(":", " -"));
                    if (ruta != "") {
                        iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                        doc.GetPdfDocument().GetDocumentInfo().SetTitle("Hoja Pijama");
                        doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{hojaPijama.Trabajador.Matricula} - {hojaPijama.Fecha.ToString("MMMM-yyyy").ToUpper()}");
                        doc.SetMargins(25, 25, 25, 25);
                        await PijamaPrintModel.CrearPijamaEnPdf(doc, hojaPijama);
                        doc.Close();
                    }
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.PijamasSeparadosEnPdf", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }
        }


        #endregion


        #region  RESUMEN AÑO

        private ICommand _cmdmostrarresumenaño;
        public ICommand cmdMostrarResumenAño {
            get {
                if (_cmdmostrarresumenaño == null) _cmdmostrarresumenaño = new RelayCommand(p => MostrarResumenAño(), p => PuedeMostrarResumenAño());
                return _cmdmostrarresumenaño;
            }
        }

        private bool PuedeMostrarResumenAño() {
            return CalendarioSeleccionado != null;
        }

        private void MostrarResumenAño() {
            // Cargamos los calendarios del año del conductor.
            List<Calendario> listaCalendarios = App.Global.Repository.GetCalendariosConductor(FechaActual.Year, CalendarioSeleccionado.MatriculaConductor).ToList();
            // Creamos el diccionario que contendrá las hojas pijama.
            Dictionary<int, Pijama.HojaPijama> pijamasAño = new Dictionary<int, Pijama.HojaPijama>();
            // Cargamos las hojas pijama disponibles.
            foreach (Calendario cal in listaCalendarios) {
                pijamasAño.Add(cal.Fecha.Month, new Pijama.HojaPijama(cal, Mensajes));
            }
            //TODO: Crear la ventana y un viewmodel que contendrá la tabla con los datos de cada calendario, al que se pasará la lista de
            //      calendarios o ya veremos el qué.



        }
        #endregion


        #region DETECTAR FALLOS

        private ICommand _cmddetectarfallos;
        public ICommand cmdDetectarFallos {
            get {
                if (_cmddetectarfallos == null) _cmddetectarfallos = new RelayCommand(p => DetectarFallos());
                return _cmddetectarfallos;
            }
        }

        private void DetectarFallos() {
            Task.Run(() => {
                try {
                    // Mostramos la barra de progreso y le asignamos el texto
                    App.Global.IniciarProgreso("Buscando errores...");
                    double num = 1;
                    foreach (Calendario cal in ListaCalendarios) {
                        double valor = num / ListaCalendarios.Count * 100;
                        App.Global.ValorBarraProgreso = valor;
                        num++;
                        Pijama.HojaPijama hoja = new Pijama.HojaPijama(cal, Mensajes);
                        cal.Informe = hoja.GetInformeFallos();
                    }
                } finally {
                    App.Global.FinalizarProgreso();
                }
            });


        }

        #endregion


        #region CAMBIAR JD POR DS

        private ICommand _cmdcambiarjdpords;
        public ICommand cmdCambiarJDPorDS {
            get {
                if (_cmdcambiarjdpords == null) _cmdcambiarjdpords = new RelayCommand(p => CambiarJDPorDS());
                return _cmdcambiarjdpords;
            }
        }

        private void CambiarJDPorDS() {

            int numero = 0;
            foreach (object obj in VistaCalendarios) {
                if (obj is Calendario cal) {
                    for (int i = 1; i < cal.ListaDias.Count - 1; i++) {
                        if (cal.ListaDias[i].Grafico == -2) {
                            if (cal.ListaDias[i - 1].Grafico > 0 && cal.ListaDias[i + 1].Grafico > 0) {
                                cal.ListaDias[i].Grafico = -5;
                                numero++;
                            }
                        }
                    }
                }
            }
            Mensajes.VerMensaje($"Se han cambiado {numero} JDs.", "JDs cambiados a DS");
        }
        #endregion


        #region CAMBIAR JD POR FN

        private ICommand _cmdcambiarjdporfn;
        public ICommand cmdCambiarJDPorFN {
            get {
                if (_cmdcambiarjdporfn == null) _cmdcambiarjdporfn = new RelayCommand(p => CambiarJDPorFN());
                return _cmdcambiarjdporfn;
            }
        }

        private void CambiarJDPorFN() {

            int numero = 0;
            foreach (object obj in VistaCalendarios) {
                if (obj is Calendario cal) {
                    for (int i = 0; i < cal.ListaDias.Count; i++) {
                        var diaSemana = cal.ListaDias[i].DiaFecha.DayOfWeek;
                        if (diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday) {
                            if (cal.ListaDias[i].Grafico == -2) {
                                cal.ListaDias[i].Grafico = -3;
                                numero++;
                            }
                            if (cal.ListaDias[i].Grafico == -10) {
                                cal.ListaDias[i].Grafico = -11;
                                numero++;
                            }
                            if (cal.ListaDias[i].Grafico == -12) {
                                cal.ListaDias[i].Grafico = -13;
                                numero++;
                            }
                            if (cal.ListaDias[i].Grafico == -17) {
                                cal.ListaDias[i].Grafico = -18;
                                numero++;
                            }
                        }
                    }
                }
            }
            Mensajes.VerMensaje($"Se han cambiado {numero} JDs.", "JDs cambiados a DS");
        }
        #endregion


        #region REGULAR HORAS

        private ICommand _cmdregularhoras;
        public ICommand cmdRegularHoras {
            get {
                if (_cmdregularhoras == null) _cmdregularhoras = new RelayCommand(p => RegularHoras(), p => PuedeRegularHoras());
                return _cmdregularhoras;
            }
        }

        private bool PuedeRegularHoras() {
            if (CalendarioSeleccionado == null) return false;
            if (Pijama == null) return false;
            if (Pijama.AcumuladasHastaAñoAnterior < App.Global.Convenio.JornadaMedia) return false;
            if (Pijama.Fecha.Month != 12) return false;
            return true;
        }

        private void RegularHoras() {
            TimeSpan horasdisponibles = Pijama.AcumuladasHastaAñoAnterior;
            decimal dcs = Math.Round(horasdisponibles.ToDecimal() / App.Global.Convenio.JornadaMedia.ToDecimal(), 4);
            string disponibles = (string)cnvSuperHoraMixta.Convert(horasdisponibles, null, null, null);
            string titulo = "Regulación de Horas";
            string mensaje = $"ATENCIÓN.\n\nSe van a regular {disponibles} horas en {dcs} DCs.\n\n¿Desea continuar?";
            if (Mensajes.VerMensaje(mensaje, titulo, true) == true) {
                RegulacionConductor regulacion = new RegulacionConductor();
                regulacion.Codigo = 2;
                regulacion.Descansos = dcs;
                regulacion.Horas = new TimeSpan(horasdisponibles.Ticks * -1);
                regulacion.IdConductor = Pijama.Trabajador.Id;
                regulacion.Fecha = new DateTime(Pijama.Fecha.Year, 11, 30);
                regulacion.Motivo = $"Horas reguladas del año {Pijama.Fecha.Year}";
                //App.Global.ConductoresVM.InsertarRegulacion(regulacion);
                Pijama.Trabajador.ListaRegulaciones.Add(regulacion);
                App.Global.ConductoresVM.GuardarTodo();
                ActualizarPijama();
            }
        }

        #endregion


        #region COBRAR HORAS
        // Comando
        private ICommand _cmdcobrarhoras;
        public ICommand cmdCobrarHoras {
            get {
                if (_cmdcobrarhoras == null) _cmdcobrarhoras = new RelayCommand(p => CobrarHoras(), p => PuedeCobrarHoras());
                return _cmdcobrarhoras;
            }
        }

        // Se puede ejecutar
        private bool PuedeCobrarHoras() {
            if (DiaCalendarioSeleccionado == null) return false;
            if (Pijama.AcumuladasHastaMes.Ticks <= 0) return false;
            return true;
        }

        // Ejecución del comando
        private void CobrarHoras() {
            int dia = DiaCalendarioSeleccionado.Dia;

            // Creamos el view-model para la ventana.
            VentanaCobrarHorasVM contexto = new VentanaCobrarHorasVM(Mensajes);
            // Introducimos las opciones del view-model.
            if (Pijama != null) {
                contexto.HorasDisponibles = Pijama.AcumuladasHastaMes;
                contexto.HorasCobradas = Pijama.HorasCobradasAño;
            }
            // Creamos la ventana y le añadimos el view-model.
            VentanaCobrarHoras ventana = new VentanaCobrarHoras() { DataContext = contexto };

            if (ventana.ShowDialog() == true) {
                if (contexto.HorasACobrar.HasValue) {
                    RegulacionConductor regulacion = new RegulacionConductor();
                    regulacion.Fecha = new DateTime(FechaActual.Year, FechaActual.Month, dia);
                    regulacion.IdConductor = Pijama.Trabajador.Id;
                    regulacion.Motivo = "Horas Cobradas";
                    regulacion.Codigo = 1;
                    regulacion.Horas = new TimeSpan(contexto.HorasACobrar.Value.Ticks * -1);
                    //BdRegulacionConductor.InsertarRegulacion(regulacion);
                    //App.Global.ConductoresVM.InsertarRegulacion(regulacion);
                    Pijama.Trabajador.ListaRegulaciones.Add(regulacion);
                    App.Global.ConductoresVM.GuardarTodo();
                    ActualizarPijama();
                } else {
                    Mensajes.VerMensaje("Debes escribir las horas que quieres cobrar.", "ATENCIÓN");
                }
            }
        }
        #endregion


        #region CALENDARIOS EN PDF

        private ICommand _cmdcalendariosenpdf;
        public ICommand cmdCalendariosEnPDF {
            get {
                if (_cmdcalendariosenpdf == null) _cmdcalendariosenpdf = new RelayCommand(p => CalendariosEnPDF(), p => PuedeCalendariosEnPdf());
                return _cmdcalendariosenpdf;
            }
        }

        private bool PuedeCalendariosEnPdf() => VistaCalendarios.Count > 1;

        private async void CalendariosEnPDF() {

            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1}", FechaActual, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.Calendarios, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Calendarios");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await CalendarioPrintModel.CrearCalendariosEnPdf(doc, VistaCalendarios, FechaActual);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.CalendariosEnPDF", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }

        #endregion


        #region CALENDARIOS EN PDF CON FALLOS

        private ICommand _cmdcalendariosenpdfconfallos;
        public ICommand cmdCalendariosEnPdfConFallos {
            get {
                if (_cmdcalendariosenpdfconfallos == null) _cmdcalendariosenpdfconfallos =
                        new RelayCommand(p => CalendariosEnPdfConFallos(), p => PuedeCalendariosEnPdfConFallos());
                return _cmdcalendariosenpdfconfallos;
            }
        }

        private bool PuedeCalendariosEnPdfConFallos() => VistaCalendarios.Count > 1;

        private async void CalendariosEnPdfConFallos() {

            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1} - Fallos", FechaActual, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.FallosCalendarios, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Fallos de Calendario");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await CalendarioPrintModel.FallosEnCalendariosEnPdf(doc, VistaCalendarios, FechaActual);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.CalendariosEnPDFConFallos", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }

        #endregion


        #region APLICAR ACCION LOTES

        private ICommand _cmdaplicaraccionlotes;
        public ICommand cmdAplicarAccionLotes {
            get {
                if (_cmdaplicaraccionlotes == null) _cmdaplicaraccionlotes = new RelayCommand(p => AplicarAccionLotes());
                return _cmdaplicaraccionlotes;
            }
        }

        private void AplicarAccionLotes() {
            // Si hemos elegido aplicar a todos los días, avisar antes de hacerlo.
            if (AccionesLotesVM.Grafico == -9999) {
                if (Mensajes.VerMensaje("Va a realizar una acción que afecta a todos los días de los calendarios.\n\n¿Desea continuar?", "ATENCIÓN", true) == false) return;
            }
            foreach (object obj in VistaCalendarios) {
                if (obj is Calendario cal) {
                    foreach (DiaCalendario dia in cal.ListaDias) {
                        if (dia.Grafico == 0) continue;
                        if (dia.Dia >= AccionesLotesVM.DelDia && dia.Dia <= AccionesLotesVM.AlDia) {
                            if (AccionesLotesVM.Grafico == -9999 || dia.Grafico == AccionesLotesVM.Grafico) {
                                if (AccionesLotesVM.Codigo == 4 || (AccionesLotesVM.Codigo == dia.Codigo)) {
                                    switch (AccionesLotesVM.CodigoAccion) {
                                        case 0: // Exceso Jornada
                                            dia.ExcesoJornada = AccionesLotesVM.SumarValor ? dia.ExcesoJornada + AccionesLotesVM.Horas : AccionesLotesVM.Horas;
                                            break;
                                        case 1: // Facturado Paqueteria
                                            dia.FacturadoPaqueteria = AccionesLotesVM.SumarValor ? dia.FacturadoPaqueteria + AccionesLotesVM.Importe : AccionesLotesVM.Importe;
                                            break;
                                        case 2: // Limpieza
                                            dia.Limpieza = true;
                                            break;
                                        case 3: // Media Limpieza
                                            dia.Limpieza = null;
                                            break;
                                        case 4: // Quitar Limpieza
                                            dia.Limpieza = false;
                                            break;
                                        case 5: // Cambiar Número
                                            if (AccionesLotesVM.NuevoGrafico != 0)
                                                dia.Grafico = AccionesLotesVM.NuevoGrafico;
                                            if (AccionesLotesVM.NuevoCodigo != 4)
                                                dia.Codigo = AccionesLotesVM.NuevoCodigo;
                                            break;
                                        case 6: // Borrar Notas
                                            dia.Notas = "";
                                            break;
                                    }
                                    if (!String.IsNullOrWhiteSpace(AccionesLotesVM.Notas) && AccionesLotesVM.CodigoAccion != 7) {
                                        if (String.IsNullOrWhiteSpace(dia.Notas)) {
                                            dia.Notas = AccionesLotesVM.Notas;
                                        } else {
                                            dia.Notas = AccionesLotesVM.SumarValor ? dia.Notas + "\n" + AccionesLotesVM.Notas : AccionesLotesVM.Notas;
                                        }
                                    }
                                    HayCambios = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion


        #region MARCAR ACCIONES LOTES

        private ICommand _cmdmarcaraccioneslotes;
        public ICommand cmdMarcarAccionesLotes {
            get {
                if (_cmdmarcaraccioneslotes == null) _cmdmarcaraccioneslotes = new RelayCommand(p => MarcarAccionesLotes());
                return _cmdmarcaraccioneslotes;
            }
        }

        private void MarcarAccionesLotes() {
            foreach (object obj in VistaCalendarios) {
                if (obj is Calendario cal) {
                    foreach (DiaCalendario dia in cal.ListaDias) {
                        if (dia.Grafico == 0) continue;
                        if (dia.Dia >= AccionesLotesVM.DelDia && dia.Dia <= AccionesLotesVM.AlDia) {
                            if (AccionesLotesVM.Grafico == 0 || dia.Grafico == AccionesLotesVM.Grafico) {
                                if (AccionesLotesVM.Codigo == 4 || (AccionesLotesVM.Codigo == dia.Codigo)) {
                                    dia.Resaltar = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion


        #region BORRAR ACCIONES POR LOTES

        private ICommand _cmdborraraccionlotes;
        public ICommand cmdBorrarAccionLotes {
            get {
                if (_cmdborraraccionlotes == null) _cmdborraraccionlotes = new RelayCommand(p => BorrarAccionLotes());
                return _cmdborraraccionlotes;
            }
        }

        private void BorrarAccionLotes() {
            AccionesLotesVM = new AccionesLotesCalendariosVM();
            foreach (object obj in VistaCalendarios) {
                if (obj is Calendario cal) {
                    foreach (DiaCalendario dia in cal.ListaDias) {
                        dia.Resaltar = false;
                    }
                }
            }

        }
        #endregion


        #region RECLAMACIÓN

        private ICommand _cmdreclamacion;
        public ICommand cmdReclamacion {
            get {
                if (_cmdreclamacion == null) _cmdreclamacion = new RelayCommand(p => Reclamacion(), p => PuedeReclamacion());
                return _cmdreclamacion;
            }
        }

        private bool PuedeReclamacion() {
            return (Pijama != null);
        }

        private async void Reclamacion() {
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("Reclamación {0:yyyy}-{0:MM} - {1:000}.pdf", Pijama.Fecha, Pijama.Trabajador.Matricula);
                string ruta = Informes.GetRutaArchivo(TiposInforme.Reclamacion, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente, Pijama.TextoTrabajador.Replace(":", " -"));
                if (ruta != "") {
                    if (File.Exists(ruta)) File.Delete(ruta);
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Reclamación");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(40, 40, 40, 40);
                    // Extraemos las reclamaciones.
                    List<Reclamacion> listaReclamaciones = new List<Reclamacion>();
                    var totalTiempo = TimeSpan.Zero;
                    var totalDietas = 0m;
                    foreach (var dia in Pijama.ListaDias) {
                        // ACUMULADAS
                        if (dia.AcumuladasAlt.HasValue && dia.AcumuladasAlt.Value < dia.GraficoOriginal.Acumuladas) {
                            listaReclamaciones.Add(new Reclamacion {
                                Concepto = $"Horas acumuladas del día {dia.DiaFecha.ToString("dd-MM-yyyy")}",
                                EnPijama = dia.AcumuladasAlt.ToTexto(),
                                Real = dia.GraficoOriginal.Acumuladas.ToTexto(),
                                Diferencia = (dia.GraficoOriginal.Acumuladas - dia.AcumuladasAlt.Value).ToTexto()
                            });
                            totalTiempo += (dia.GraficoOriginal.Acumuladas - dia.AcumuladasAlt.Value);
                        }
                        // DESAYUNO
                        if (dia.DesayunoAlt.HasValue && dia.DesayunoAlt.Value < dia.GraficoOriginal.Desayuno) {
                            listaReclamaciones.Add(new Reclamacion {
                                Concepto = $"Dieta de desayuno del día {dia.DiaFecha.ToString("dd-MM-yyyy")}",
                                EnPijama = dia.DesayunoAlt.Value.ToString("0.00"),
                                Real = dia.GraficoOriginal.Desayuno.ToString("0.00"),
                                Diferencia = (dia.GraficoOriginal.Desayuno - dia.DesayunoAlt.Value).ToString("0.00") +
                                $" ({(dia.GraficoOriginal.Desayuno - dia.DesayunoAlt.Value) * App.Global.Convenio.ImporteDietas:0.00} €)",
                            });
                            totalDietas += (dia.GraficoOriginal.Desayuno - dia.DesayunoAlt.Value);
                        }
                        // COMIDA
                        if (dia.ComidaAlt.HasValue && dia.ComidaAlt.Value < dia.GraficoOriginal.Comida) {
                            listaReclamaciones.Add(new Reclamacion {
                                Concepto = $"Dieta de comida del día {dia.DiaFecha.ToString("dd-MM-yyyy")}",
                                EnPijama = dia.ComidaAlt.Value.ToString("0.00"),
                                Real = dia.GraficoOriginal.Comida.ToString("0.00"),
                                Diferencia = (dia.GraficoOriginal.Comida - dia.ComidaAlt.Value).ToString("0.00") +
                                $" ({(dia.GraficoOriginal.Comida - dia.ComidaAlt.Value) * App.Global.Convenio.ImporteDietas:0.00} €)",
                            });
                            totalDietas += (dia.GraficoOriginal.Comida - dia.ComidaAlt.Value);
                        }
                        // CENA
                        if (dia.CenaAlt.HasValue && dia.CenaAlt.Value < dia.GraficoOriginal.Cena) {
                            listaReclamaciones.Add(new Reclamacion {
                                Concepto = $"Dieta de cena del día {dia.DiaFecha.ToString("dd-MM-yyyy")}",
                                EnPijama = dia.CenaAlt.Value.ToString("0.00"),
                                Real = dia.GraficoOriginal.Cena.ToString("0.00"),
                                Diferencia = (dia.GraficoOriginal.Cena - dia.CenaAlt.Value).ToString("0.00") +
                                $" ({(dia.GraficoOriginal.Cena - dia.CenaAlt.Value) * App.Global.Convenio.ImporteDietas:0.00} €)",
                            });
                            totalDietas += (dia.GraficoOriginal.Cena - dia.CenaAlt.Value);
                        }
                        // PLUS CENA
                        if (dia.PlusCenaAlt.HasValue && dia.PlusCenaAlt.Value < dia.GraficoOriginal.PlusCena) {
                            listaReclamaciones.Add(new Reclamacion {
                                Concepto = $"Plus Cena del día {dia.DiaFecha.ToString("dd-MM-yyyy")}",
                                EnPijama = dia.PlusCenaAlt.Value.ToString("0.00"),
                                Real = dia.GraficoOriginal.PlusCena.ToString("0.00"),
                                Diferencia = (dia.GraficoOriginal.PlusCena - dia.PlusCenaAlt.Value).ToString("0.00") +
                                $" ({(dia.GraficoOriginal.PlusCena - dia.PlusCenaAlt.Value) * App.Global.Convenio.ImporteDietas:0.00} €)",
                            });
                            totalDietas += (dia.GraficoOriginal.PlusCena - dia.PlusCenaAlt.Value);
                        }
                    }
                    var notas = string.Empty;
                    if (totalTiempo.Ticks > 0) notas += $"     Horas = {totalTiempo.ToTexto()}\n";
                    if (totalDietas > 0) notas += $"     Dietas = {totalDietas:0.00} ({totalDietas * App.Global.Convenio.ImporteDietas:0.00} €)\n";
                    if (notas.Length > 0) notas = "TOTAL A RECLAMAR\n\n" + notas;
                    await PijamaPrintModel.CrearReclamacionEnPdf(doc, Pijama.Fecha, Pijama.Trabajador, listaReclamaciones, notas);
                    doc.Close();
                    Process.Start(ruta);

                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.Reclamacion", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }


        #endregion


        #region PDF ESTADÍSTICAS

        private ICommand _cmdpdfestadisticas;
        public ICommand cmdPdfEstadisticas {
            get {
                if (_cmdpdfestadisticas == null) _cmdpdfestadisticas = new RelayCommand(p => PdfEsatdisticas(), p => PuedePdfEsatdisticas());
                return _cmdpdfestadisticas;
            }
        }

        private bool PuedePdfEsatdisticas() => VistaCalendarios?.Count > 1;

        private async void PdfEsatdisticas() {
            List<EstadisticaCalendario> listaEstadisticas = new List<EstadisticaCalendario>();
            try {
                double num = 1;
                App.Global.IniciarProgreso($"Recopilando...");

                await Task.Run(() => {
                    // Llenamos la lista de estadísticas
                    for (int dia = 1; dia <= DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month); dia++) {
                        EstadisticaCalendario e = new EstadisticaCalendario() { Dia = dia };
                        listaEstadisticas.Add(e);
                    }
                    // Recorremos todos los calendarios
                    foreach (object obj in VistaCalendarios) {
                        double valor = (num / VistaCalendarios.Count * 100) / 2;
                        App.Global.ValorBarraProgreso = valor;
                        num++;
                        Calendario cal = obj as Calendario;
                        if (cal == null) continue;
                        Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(cal, new MensajesServicio());
                        // Añadimos los datos del pijama a las estadisticas.
                        for (int dia = 0; dia < DateTime.DaysInMonth(FechaActual.Year, FechaActual.Month); dia++) {
                            // Cogemos el día de la lista.
                            Pijama.DiaPijama dp = hojapijama.ListaDias[dia];
                            EstadisticaCalendario estadistica = listaEstadisticas[dia];
                            // Si es un día activo
                            if (dp.Grafico != 0) estadistica.TotalDias += 1;
                            // Si se ha trabajado y no hemos tenido día de comite
                            if (dp.Grafico > 0 && dp.Codigo != 1 && dp.Codigo != 2) {
                                estadistica.TotalJornadas += 1;
                                switch (dp.GraficoTrabajado.Turno) {
                                    case 1: estadistica.Turno1 += 1; break;
                                    case 2: estadistica.Turno2 += 1; break;
                                    case 3: estadistica.Turno3 += 1; break;
                                    case 4: estadistica.Turno4 += 1; break;
                                }
                                // Horas
                                estadistica.Trabajadas += dp.TrabajadasReales;
                                estadistica.Acumuladas += dp.GraficoTrabajado.Acumuladas;
                                estadistica.Nocturnas += dp.GraficoTrabajado.Nocturnas;
                                estadistica.TiempoPartido += Calculos.Horas.TiempoPartido(dp.GraficoTrabajado.InicioPartido, dp.GraficoTrabajado.FinalPartido);
                                //estadistica.TiempoPartido += dp.GraficoTrabajado.TiempoPartido;
                                // Si es menor de 7:20 se añade a menores, si no, a mayores.
                                if (dp.TrabajadasReales < App.Global.Convenio.JornadaMedia) {
                                    estadistica.JornadasMenoresMedia += 1;
                                    estadistica.HorasNegativas += App.Global.Convenio.JornadaMedia - estadistica.Trabajadas;
                                } else {
                                    estadistica.JornadasMayoresMedia += 1;
                                }
                            }
                            // Dietas
                            estadistica.Desayuno += Math.Round(dp.GraficoTrabajado.Desayuno, 2);
                            estadistica.Comida += Math.Round(dp.GraficoTrabajado.Comida, 2);
                            estadistica.Cena += Math.Round(dp.GraficoTrabajado.Cena, 2);
                            estadistica.PlusCena += Math.Round(dp.GraficoTrabajado.PlusCena, 2);
                            estadistica.ImporteDesayuno += Math.Round((dp.GraficoTrabajado.Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImporteComida += Math.Round(dp.GraficoTrabajado.Comida * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImporteCena += Math.Round(dp.GraficoTrabajado.Cena * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImportePlusCena += Math.Round(dp.GraficoTrabajado.PlusCena * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            // Pluses
                            estadistica.PlusMenorDescanso += Math.Round(dp.PlusMenorDescanso, 2);
                            estadistica.PlusNocturnidad += Math.Round(dp.PlusNocturnidad, 2);
                            estadistica.PlusNavidad += Math.Round(dp.PlusNavidad, 2);
                            estadistica.PlusLimipeza += Math.Round(dp.PlusLimpieza, 2);
                            estadistica.PlusPaqueteria += Math.Round(dp.PlusPaqueteria, 2);

                        }
                    }
                    // Establecemos los globales
                    foreach (EstadisticaCalendario estadistica in listaEstadisticas) {
                        if (estadistica.TotalJornadas != 0) {
                            estadistica.MediaTrabajadas = new TimeSpan(estadistica.Trabajadas.Ticks / estadistica.TotalJornadas);
                        }
                    }

                });
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1} - Estadisticas Calendarios", FechaActual, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasCalendarios, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas de Calendario");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await CalendarioPrintModel.EstadisticasCalendariosEnPdf(doc, listaEstadisticas, FechaActual);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.PdfEstadisticas", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }
        #endregion


        #region PDF ESTADÍSTICAS MES

        private ICommand _cmdpdfestadisticasmes;
        public ICommand cmdPdfEstadisticasMes {
            get {
                if (_cmdpdfestadisticasmes == null) _cmdpdfestadisticasmes = new RelayCommand(p => PdfEstadisticasMes(), p => PuedePdfEstadisticasMes());
                return _cmdpdfestadisticasmes;
            }
        }

        private bool PuedePdfEstadisticasMes() => VistaCalendarios?.Count > 1;

        [STAThread]
        private async void PdfEstadisticasMes() {
            try {
                // Creamos las listas que se van a usar.
                List<GraficoFecha> listaGraficos = null;
                List<GraficosPorDia> listaNumeros = null;
                List<DescansosPorDia> listaDescansos = null;
                // Llenamos las listas
                App.Global.IniciarProgreso("Recopilando...");
                await Task.Run(() => {
                    App.Global.ValorBarraProgreso = 33;
                    listaGraficos = App.Global.Repository.GetGraficosFromDiaCalendario(FechaActual, App.Global.PorCentro.Comodin).ToList();
                    App.Global.ValorBarraProgreso = 66;
                    listaNumeros = App.Global.Repository.GetGraficosByDia(FechaActual).ToList();
                    App.Global.ValorBarraProgreso = 95;
                    listaDescansos = App.Global.Repository.GetDescansosByDia(FechaActual).ToList();
                });

                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM} - {1} - Estadisticas Mes", FechaActual, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasCalendarios, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas Mes");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await CalendarioPrintModel.EstadisticasMesEnPdf(doc, listaGraficos, listaNumeros, listaDescansos);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.EstadisticasMes", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }


        }
        #endregion


        #region PIJAMA MES MENOS

        private ICommand cmdpijamamesmenos;
        public ICommand cmdPijamaMesMenos {
            get {
                if (cmdpijamamesmenos == null) cmdpijamamesmenos = new RelayCommand(p => PijamaMesMenos());
                return cmdpijamamesmenos;
            }
        }

        private void PijamaMesMenos() {
            int conductor = IdConductorPijama;
            FechaActual = FechaActual.AddMonths(-1);
            CalendarioSeleccionado = ListaCalendarios.FirstOrDefault(c => c.MatriculaConductor == conductor);
            if (CalendarioSeleccionado != null) {
                Pijama = new Pijama.HojaPijama(CalendarioSeleccionado, Mensajes);
                Pijama.PropiedadCambiada("");
            } else {
                Pijama = null;
            }
        }

        #endregion


        #region PIJAMA MES MÁS

        private ICommand cmdpijamamesmas;
        public ICommand cmdPijamaMesMas {
            get {
                if (cmdpijamamesmas == null) cmdpijamamesmas = new RelayCommand(p => PijamaMesMas());
                return cmdpijamamesmas;
            }
        }

        private void PijamaMesMas() {
            int conductor = IdConductorPijama;
            FechaActual = FechaActual.AddMonths(1);
            CalendarioSeleccionado = ListaCalendarios.FirstOrDefault(c => c.MatriculaConductor == conductor);
            if (CalendarioSeleccionado != null) {
                Pijama = new Pijama.HojaPijama(CalendarioSeleccionado, Mensajes);
                Pijama.PropiedadCambiada("");
            } else {
                Pijama = null;
            }
        }

        #endregion


        #region ACTUALIZAR PIJAMA

        private ICommand actualizarpijama;
        public ICommand cmdActualizarPijama {
            get {
                if (actualizarpijama == null) actualizarpijama = new RelayCommand(p => ActualizarPijama());
                return actualizarpijama;
            }
        }

        private void ActualizarPijama() {
            if (CalendarioSeleccionado != null) {
                if (HayCambios) GuardarCalendarios();
                Pijama = new Pijama.HojaPijama(CalendarioSeleccionado, Mensajes);
                Pijama.PropiedadCambiada("");
            } else {
                Pijama = null;
            }
        }

        #endregion


        #region COMANDO COMPARAR CALENDARIOS

        // Comando
        private ICommand _compararCalendarioexcel;
        public ICommand cmdCompararCalendarioExcel {
            get {
                if (_compararCalendarioexcel == null) _compararCalendarioexcel = new RelayCommand(p => CompararCalendarioExcel(), p => PuedeCompararCalendarioExcel());
                return _compararCalendarioexcel;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeCompararCalendarioExcel() {
            return ListaCalendarios.Count > 0;
        }

        // Ejecución del comando
        private void CompararCalendarioExcel() {

            // Definir el nombre del excel de destino.
            string nombreArchivo = $"Comparación Calendarios {FechaActual.ToString("MM - yyyy")} - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Pedimos el archivo excel que se va a comparar.
            string titulo = "Seleccione el excel con los calendarios a comparar.";
            string excel = FileService.FileDialog(titulo, @"C:\");
            if (excel == "") return;

            ExcelService.getInstance().GenerarComparacionCalendarios(rutaDestino, ListaCalendarios, excel, FechaActual.Year, FechaActual.Month);

            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


        #region COMANDO COMPARAR CALENDARIOS ANUAL

        // Comando
        private ICommand compararCalendariosExcelAnual;
        public ICommand cmdCompararCalendariosExcelAnual {
            get {
                if (compararCalendariosExcelAnual == null) compararCalendariosExcelAnual = new RelayCommand(p => CompararCalendariosExcelAnual(), p => PuedeCompararCalendariosExcelAnual());
                return compararCalendariosExcelAnual;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeCompararCalendariosExcelAnual() {
            return true;
        }

        // Ejecución del comando
        private void CompararCalendariosExcelAnual() {
            // Definir el nombre del excel de destino.
            string nombreArchivo = $"Comparación Calendarios {FechaActual.ToString("yyyy")} - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Pedimos el archivo excel que se va a comparar.
            string titulo = "Seleccione el excel con los calendarios a comparar.";
            string excel = FileService.FileDialog(titulo, @"C:\");
            if (excel == "") return;

            ExcelService.getInstance().GenerarComparacionCalendariosAnual(rutaDestino, excel, FechaActual.Year);

            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


        #region COMANDO ESTADISTICAS AÑO

        // Comando
        public ICommand cmdPdfEstadisticasAño => new RelayCommand(p => PdfEstadisticasAño());
        private async void PdfEstadisticasAño() {
            List<EstadisticaCalendario> listaEstadisticas = new List<EstadisticaCalendario>();
            try {
                double num = 1;
                App.Global.IniciarProgreso($"Recopilando...");

                await Task.Run(() => {
                    // Llenamos la lista de estadísticas y generamos los calendarios.
                    var listaCalendarios = new List<Calendario>();
                    for (int mes = 1; mes <= 12; mes++) {
                        EstadisticaCalendario e = new EstadisticaCalendario() { Dia = mes };
                        listaEstadisticas.Add(e);
                        listaCalendarios.AddRange(App.Global.Repository.GetCalendarios(FechaActual.Year, mes));
                    }

                    foreach (var cal in listaCalendarios) {
                        Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(cal, new MensajesServicio());
                        double valor = (num * 100 / listaCalendarios.Count) / 2;
                        App.Global.ValorBarraProgreso = valor;
                        num++;
                        // Añadimos los datos del pijama a las estadisticas.
                        for (int dia = 0; dia < DateTime.DaysInMonth(cal.Fecha.Year, cal.Fecha.Month); dia++) {
                            // Cogemos el día de la lista.
                            Pijama.DiaPijama dp = hojapijama.ListaDias[dia];
                            EstadisticaCalendario estadistica = listaEstadisticas[cal.Fecha.Month - 1];
                            // Si es un día activo
                            if (dp.Grafico != 0) estadistica.TotalDias += 1;
                            // Si se ha trabajado y no hemos tenido día de comite
                            if (dp.Grafico > 0 && dp.Codigo != 1 && dp.Codigo != 2) {
                                estadistica.TotalJornadas += 1;
                                switch (dp.GraficoTrabajado.Turno) {
                                    case 1: estadistica.Turno1 += 1; break;
                                    case 2: estadistica.Turno2 += 1; break;
                                    case 3: estadistica.Turno3 += 1; break;
                                    case 4: estadistica.Turno4 += 1; break;
                                }
                                // Horas
                                estadistica.Trabajadas += dp.TrabajadasReales;
                                estadistica.Acumuladas += dp.GraficoTrabajado.Acumuladas;
                                estadistica.Nocturnas += dp.GraficoTrabajado.Nocturnas;
                                estadistica.TiempoPartido += Calculos.Horas.TiempoPartido(dp.GraficoTrabajado.InicioPartido, dp.GraficoTrabajado.FinalPartido);
                                //estadistica.TiempoPartido += dp.GraficoTrabajado.TiempoPartido;
                                // Si es menor de 7:20 se añade a menores, si no, a mayores.
                                if (dp.TrabajadasReales < App.Global.Convenio.JornadaMedia) {
                                    estadistica.JornadasMenoresMedia += 1;
                                    estadistica.HorasNegativas += App.Global.Convenio.JornadaMedia - dp.TrabajadasReales;
                                } else {
                                    estadistica.JornadasMayoresMedia += 1;
                                }
                            }
                            // Dietas
                            estadistica.Desayuno += Math.Round(dp.GraficoTrabajado.Desayuno, 2);
                            estadistica.Comida += Math.Round(dp.GraficoTrabajado.Comida, 2);
                            estadistica.Cena += Math.Round(dp.GraficoTrabajado.Cena, 2);
                            estadistica.PlusCena += Math.Round(dp.GraficoTrabajado.PlusCena, 2);
                            estadistica.ImporteDesayuno += Math.Round((dp.GraficoTrabajado.Desayuno * App.Global.Convenio.PorcentajeDesayuno / 100) * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImporteComida += Math.Round(dp.GraficoTrabajado.Comida * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImporteCena += Math.Round(dp.GraficoTrabajado.Cena * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            estadistica.ImportePlusCena += Math.Round(dp.GraficoTrabajado.PlusCena * App.Global.OpcionesVM.GetPluses(dp.DiaFecha.Year).ImporteDietas, 2);
                            // Pluses
                            estadistica.PlusMenorDescanso += Math.Round(dp.PlusMenorDescanso, 2);
                            estadistica.PlusNocturnidad += Math.Round(dp.PlusNocturnidad, 2);
                            estadistica.PlusNavidad += Math.Round(dp.PlusNavidad, 2);
                            estadistica.PlusLimipeza += Math.Round(dp.PlusLimpieza, 2);
                            estadistica.PlusPaqueteria += Math.Round(dp.PlusPaqueteria, 2);

                        }
                    }
                    // Establecemos los globales
                    foreach (EstadisticaCalendario estadistica in listaEstadisticas) {
                        if (estadistica.TotalJornadas != 0) {
                            estadistica.MediaTrabajadas = new TimeSpan(estadistica.Trabajadas.Ticks / estadistica.TotalJornadas);
                        }
                    }

                });
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy} - {1} - Estadisticas Calendarios", FechaActual, App.Global.CentroActual.ToString());
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasCalendarios, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas de Calendario");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await CalendarioPrintModel.EstadisticasCalendariosAñoEnPdf(doc, listaEstadisticas, FechaActual);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("CalendariosCommands.PdfEstadisticas", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }
        #endregion


        #region COMANDO LIMPIAR ALTERNATIVOS

        // Comando
        private ICommand cmdlimpiarAlternativos;
        public ICommand cmdLimpiarAlternativos {
            get {
                if (cmdlimpiarAlternativos == null) cmdlimpiarAlternativos = new RelayCommand(p => LimpiarAlternativos(), p => PuedeLimpiarAlternativos());
                return cmdlimpiarAlternativos;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeLimpiarAlternativos() {
            return DiaCalendarioSeleccionado != null;
        }

        // Ejecución del comando
        private void LimpiarAlternativos() {
            DiaCalendarioSeleccionado.TurnoAlt = null;
            DiaCalendarioSeleccionado.InicioAlt = null;
            DiaCalendarioSeleccionado.FinalAlt = null;
            DiaCalendarioSeleccionado.InicioPartidoAlt = null;
            DiaCalendarioSeleccionado.FinalPartidoAlt = null;
            DiaCalendarioSeleccionado.TrabajadasAlt = null;
            DiaCalendarioSeleccionado.AcumuladasAlt = null;
            DiaCalendarioSeleccionado.NocturnasAlt = null;
            DiaCalendarioSeleccionado.DesayunoAlt = null;
            DiaCalendarioSeleccionado.ComidaAlt = null;
            DiaCalendarioSeleccionado.CenaAlt = null;
            DiaCalendarioSeleccionado.PlusCenaAlt = null;
            //DiaCalendarioSeleccionado.PlusLimpiezaAlt = null;
            DiaCalendarioSeleccionado.PlusPaqueteriaAlt = null;
        }
        #endregion


        #region COMANDO APLICAR CORRECCIONES

        // Comando
        private ICommand aplicarCorrecciones;
        public ICommand cmdAplicarCorrecciones {
            get {
                if (aplicarCorrecciones == null) aplicarCorrecciones = new RelayCommand(p => AplicarCorrecciones(), p => PuedeAplicarCorrecciones());
                return aplicarCorrecciones;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeAplicarCorrecciones() {
            return CalendarioSeleccionado != null;
        }

        // Ejecución del comando
        private void AplicarCorrecciones() {

            var matricula = CalendarioSeleccionado.MatriculaConductor;
            var response = Mensajes.VerMensaje($"Va a ajustar el calendario de {matricula:000} según el excel compartido.\n\n¿Desea Continuar?", "ATENCIÓN", true);
            if (response != true) return;
            var archivo = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Horas Conductores\\{matricula:000}\\{matricula:000} - {FechaActual.Year}.xlsx");
            if (!File.Exists(archivo)) {
                Mensajes.VerMensaje("No hay comparaciones del conductor.", "ATENCIÓN");
                return;
            }
            var horas = ExcelService.getInstance().GetHorasConductor(archivo, FechaActual.Year, FechaActual.Month);
            if (horas == null) {
                Mensajes.VerMensaje("No hay comparaciones del conductor.", "ATENCIÓN");
                return;
            }
            foreach (var hora in horas) {
                var dia = CalendarioSeleccionado.ListaDias.FirstOrDefault(d => d.DiaFecha.Day == hora.Dia.Day);
                if (dia.Grafico != hora.Grafico) dia.Grafico = hora.Grafico;
                if (dia.Grafico > 0) {
                    var grafico = App.Global.Repository.GetGrafico(dia.Grafico, dia.DiaFecha);
                    if (grafico != null) {
                        if (hora.Trabajadas.Ticks >= 0 && grafico.Trabajadas + dia.ExcesoJornada != hora.Trabajadas) dia.TrabajadasAlt = hora.Trabajadas;
                        if (hora.Acumuladas.Ticks >= 0 && grafico.Acumuladas != hora.Acumuladas) dia.AcumuladasAlt = hora.Acumuladas;
                        if (hora.Desayuno >= 0 && grafico.Desayuno != hora.Desayuno) dia.DesayunoAlt = hora.Desayuno;
                        if (hora.Comida >= 0 && grafico.Comida != hora.Comida) dia.ComidaAlt = hora.Comida;
                        if (hora.Cena >= 0 && grafico.Cena != hora.Cena) dia.CenaAlt = hora.Cena;
                        if (hora.PlusCena >= 0 && grafico.PlusCena != hora.PlusCena) dia.PlusCenaAlt = hora.PlusCena;
                    }
                }
            }
        }
        #endregion


        #region COMANDO COMPARAR CORRECCIONES

        // Comando
        private ICommand compararCorrecciones;
        public ICommand cmdCompararCorrecciones {
            get {
                if (compararCorrecciones == null) compararCorrecciones = new RelayCommand(p => CompararCorrecciones(), p => PuedeCompararCorrecciones());
                return compararCorrecciones;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeCompararCorrecciones() {
            return true;
        }

        // Ejecución del comando
        private void CompararCorrecciones() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            var matricula = CalendarioSeleccionado.MatriculaConductor;
            var archivo = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Horas Conductores\\{matricula:000}\\{matricula:000} - {FechaActual.Year}.xlsx");
            if (!File.Exists(archivo)) {
                Mensajes.VerMensaje("No hay comparaciones del conductor.", "ATENCIÓN");
                return;
            }
            var horas = ExcelService.getInstance().GetHorasConductor(archivo, FechaActual.Year, FechaActual.Month);
            if (horas == null) {
                Mensajes.VerMensaje("No hay comparaciones del conductor.", "ATENCIÓN");
                return;
            }
            var pijama = new List<HorasConductor>();
            foreach (var dia in Pijama.ListaDias) {
                pijama.Add(new HorasConductor {
                    Dia = dia.DiaFecha,
                    Grafico = dia.GraficoTrabajado.Numero,
                    Trabajadas = dia.GraficoTrabajado.Trabajadas,
                    Acumuladas = dia.GraficoTrabajado.Acumuladas,
                    Desayuno = dia.GraficoTrabajado.Desayuno,
                    Comida = dia.GraficoTrabajado.Comida,
                    Cena = dia.GraficoTrabajado.Cena,
                    PlusCena = dia.GraficoTrabajado.PlusCena,
                });
            }
            var pdf = Path.Combine(Path.GetTempPath(), $"Comparacion Correcciones-{matricula}-{FechaActual.Month:00}.pdf");
            iText.Layout.Document doc = Informes.GetNuevoPdf(pdf);
            doc.SetMargins(25, 25, 25, 25);
            var tabla = PdfExcelService.GetInstance().GetComparacionCorreccionesConductor(matricula, pijama, horas);
            doc.Add(PdfExcelHelper.GetInstance().GetPdfTable(tabla));
            doc.Close();
            Process.Start(pdf);
        }
        #endregion


        #region COMANDO REGENERAR GRÁFICO

        // Comando
        private ICommand cmdRegenerarCalendario;
        public ICommand CmdRegenerarCalendario {
            get {
                if (cmdRegenerarCalendario == null) cmdRegenerarCalendario = new RelayCommand(p => RegenerarCalendario(), p => PuedeRegenerarCalendario());
                return cmdRegenerarCalendario;
            }
        }
        private bool PuedeRegenerarCalendario() => CalendarioSeleccionado != null;

        private void RegenerarCalendario() {
            foreach (var dia in CalendarioSeleccionado.ListaDias) RegenerarDiaCalendario(dia);
            CalendarioSeleccionado.Recalcular();
        }
        #endregion


        #region COMANDO REGENERAR TODOS (CALENDARIOS)

        // Comando
        private ICommand cmdRegenerarTodos;
        public ICommand CmdRegenerarTodos {
            get {
                if (cmdRegenerarTodos == null) cmdRegenerarTodos = new RelayCommand(p => RegenerarTodos(), p => PuedeRegenerarTodos());
                return cmdRegenerarTodos;
            }
        }
        private bool PuedeRegenerarTodos() => VistaCalendarios.Count > 1;

        private void RegenerarTodos() {
            foreach (var objeto in VistaCalendarios) {
                if (objeto is Calendario calendario) {
                    foreach (var dia in ((Calendario)calendario).ListaDias) RegenerarDiaCalendario(dia);
                    RecalcularCalendario(calendario);
                }
            }
        }
        #endregion


        #region ABRIR CALENDARIOS ANUALES 

        // Comando
        private ICommand cmdAbrirAnuales;
        public ICommand CmdAbrirAnuales {
            get {
                if (cmdAbrirAnuales == null) cmdAbrirAnuales = new RelayCommand(p => AbrirAnuales());
                return cmdAbrirAnuales;
            }
        }
        private void AbrirAnuales() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            var nombreArchivo = $"{FechaActual.Year}-Calendarios Anuales-{App.Global.CentroActual}.xlsx";
            var carpeta = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var ruta = Path.Combine(carpeta, nombreArchivo);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            var titulo = $"CALENDARIOS ANUALES {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            if (!File.Exists(ruta)) {
                ExcelService.getInstance().GenerarPlantillaCalendariosExcel(ruta, FechaActual, titulo);
            } else {
                using (var excelApp = new ExcelPackage(new FileInfo(ruta))) {
                    excelApp.Workbook.Worksheets[mesesAbr[FechaActual.Month]].Select();
                    excelApp.Save();
                }
            }
            Process.Start(ruta);
        }
        #endregion


        #region ABRIR CALENDARIOS VIRTUALES 

        // Comando
        private ICommand cmdAbrirVirtuales;
        public ICommand CmdAbrirVirtuales {
            get {
                if (cmdAbrirVirtuales == null) cmdAbrirVirtuales = new RelayCommand(p => AbrirVirtuales());
                return cmdAbrirVirtuales;
            }
        }
        private void AbrirVirtuales() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            var nombreArchivo = $"{FechaActual.Year}-Calendarios Virtuales-{App.Global.CentroActual}.xlsx";
            var carpeta = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var ruta = Path.Combine(carpeta, nombreArchivo);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            var titulo = $"CALENDARIOS VIRTUALES {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            if (!File.Exists(ruta)) {
                ExcelService.getInstance().GenerarPlantillaCalendariosExcel(ruta, FechaActual, titulo);
            } else {
                using (var excelApp = new ExcelPackage(new FileInfo(ruta))) {
                    excelApp.Workbook.Worksheets[mesesAbr[FechaActual.Month]].Select();
                    excelApp.Save();
                }
            }
            Process.Start(ruta);
        }
        #endregion


        #region COMPARAR CALENDARIO CON ANUAL 

        // Comando
        private ICommand cmdCompararConAnual;
        public ICommand CmdCompararConAnual {
            get {
                if (cmdCompararConAnual == null) cmdCompararConAnual = new RelayCommand(p => CompararConAnual());
                return cmdCompararConAnual;
            }
        }
        private void CompararConAnual() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            // Definir el nombre del excel de destino.
            string nombreArchivo = $"{FechaActual.ToString("yyyy-MM")} - Comparación Con Anual - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Establecemos el archivo excel que se va a comparar.
            var nombreAnual = $"{FechaActual.Year}-Calendarios Anuales-{App.Global.CentroActual}.xlsx";
            var carpetaAnual = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var rutaAnual = Path.Combine(carpetaAnual, nombreAnual);

            var titulo = $"COMPARACION CON ANUAL - {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            ExcelService.getInstance().GenerarComparacionCalendario(rutaDestino, rutaAnual, ListaCalendarios, FechaActual, titulo, "Anual");

            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


        #region COMPARAR CALENDARIO CON VIRTUAL 

        // Comando
        private ICommand cmdCompararConVirtual;
        public ICommand CmdCompararConVirtual {
            get {
                if (cmdCompararConVirtual == null) cmdCompararConVirtual = new RelayCommand(p => CompararConVirtual());
                return cmdCompararConVirtual;
            }
        }
        private void CompararConVirtual() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            // Definir el nombre del excel de destino.
            string nombreArchivo = $"{FechaActual.ToString("yyyy-MM")} - Comparación Con Virtual - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Establecemos el archivo excel que se va a comparar.
            var nombreAnual = $"{FechaActual.Year}-Calendarios Virtuales-{App.Global.CentroActual}.xlsx";
            var carpetaAnual = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var rutaAnual = Path.Combine(carpetaAnual, nombreAnual);

            var titulo = $"COMPARACION CON VIRTUAL - {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            ExcelService.getInstance().GenerarComparacionCalendario(rutaDestino, rutaAnual, ListaCalendarios, FechaActual, titulo, "Virtual");

            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


        #region COMPARAR TODOS LOS CALENDARIOS CON EL ANUAL 

        // Comando
        private ICommand cmdCompararTodosConAnual;
        public ICommand CmdCompararTodosConAnual {
            get {
                if (cmdCompararTodosConAnual == null) cmdCompararTodosConAnual = new RelayCommand(p => CompararTodosConAnual());
                return cmdCompararTodosConAnual;
            }
        }
        private void CompararTodosConAnual() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            // Definir el nombre del excel de destino.
            string nombreArchivo = $"{FechaActual.ToString("yyyy")} - Comparación Todos Con Anual - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Establecemos el archivo excel que se va a comparar.
            var nombreAnual = $"{FechaActual.Year}-Calendarios Anuales-{App.Global.CentroActual}.xlsx";
            var carpetaAnual = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var rutaAnual = Path.Combine(carpetaAnual, nombreAnual);

            var titulo = $"COMPARACION CON ANUAL - {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            ExcelService.getInstance().GenerarComparacionCalendarioAnual(rutaDestino, rutaAnual, FechaActual, titulo, "Anual");
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


        #region COMPARAR TODOS LOS CALENDARIOS CON EL VIRTUAL 

        // Comando
        private ICommand cmdCompararTodosConVirtual;
        public ICommand CmdCompararTodosConVirtual {
            get {
                if (cmdCompararTodosConVirtual == null) cmdCompararTodosConVirtual = new RelayCommand(p => CompararTodosConVirual());
                return cmdCompararTodosConVirtual;
            }
        }
        private void CompararTodosConVirual() {
            if (string.IsNullOrEmpty(App.Global.Configuracion.CarpetaAvanza)) {
                Mensajes.VerMensaje("Por favor\n\nDefina la ruta de la carpeta AVANZA en las opciones.", "ATENCIÓN");
                return;
            }
            // Definir el nombre del excel de destino.
            string nombreArchivo = $"{FechaActual.ToString("yyyy")} - Comparación Todos Con Virtual - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Calendarios");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Establecemos el archivo excel que se va a comparar.
            var nombreAnual = $"{FechaActual.Year}-Calendarios Virtuales-{App.Global.CentroActual}.xlsx";
            var carpetaAnual = Path.Combine(App.Global.Configuracion.CarpetaAvanza, $"Calendarios/{App.Global.CentroActual}");
            var rutaAnual = Path.Combine(carpetaAnual, nombreAnual);

            var titulo = $"COMPARACION CON VIRTUAL - {App.Global.CentroActual.ToString().ToUpper()} - {FechaActual.Year}";
            ExcelService.getInstance().GenerarComparacionCalendarioAnual(rutaDestino, rutaAnual, FechaActual, titulo, "Virtual");
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion



        #region COMANDO TEMPORAL REGENERAR TODO

        // Comando
        private ICommand cmdRegenerarTodo;
        public ICommand CmdRegenerarTodo {
            get {
                if (cmdRegenerarTodo == null) cmdRegenerarTodo = new RelayCommand(p => RegenerarTodo());
                return cmdRegenerarTodo;
            }
        }
        private void RegenerarTodo() {

            for (int año = 2017; año <= 2021; año++) {
                var lista = App.Global.Repository.GetCalendarios(año);
                foreach (var calendario in lista) {
                    foreach (var dia in calendario.ListaDias) RegenerarDiaCalendarioAislado(dia);
                    RecalcularCalendario(calendario);
                }
                App.Global.Repository.GuardarCalendarios(lista);
            }



        }
        #endregion




    }// Fin de la clase.
}
