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
    using System.Windows.Controls;
    using System.Windows.Input;
    using iText.Kernel.Pdf;
    using Microsoft.Office.Interop.Excel;
    using Convertidores;
    using DataModels;
    using Models;
    using PrintModel;
    using Servicios;
    using Views;

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

        private bool PuedeAplicarFiltro() {
            if (ListaCalendarios.Count == 0) return false;
            return true;
        }

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
            GuardarCalendarios();
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
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{Pijama.Trabajador.Id} - {Pijama.Fecha.ToString("MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await PijamaPrintModel.CrearPijamaEnPdf_7(doc, Pijama);
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
            return VistaCalendarios.Count > 0;
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
                    await PijamaPrintModel.CrearTodosPijamasEnPdf_7(doc, VistaCalendarios);
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
            return VistaCalendarios.Count > 0;
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
                        doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{hojaPijama.Trabajador.Id} - {hojaPijama.Fecha.ToString("MMMM-yyyy").ToUpper()}");
                        doc.SetMargins(25, 25, 25, 25);
                        await PijamaPrintModel.CrearPijamaEnPdf_7(doc, hojaPijama);
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
            List<Calendario> listaCalendarios = BdCalendarios.GetCalendariosConductor(FechaActual.Year, CalendarioSeleccionado.IdConductor);
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
                Calendario cal = obj as Calendario;
                if (cal == null) continue;
                for (int i = 1; i < cal.ListaDias.Count - 1; i++) {
                    if (cal.ListaDias[i].Grafico == -2) {
                        if (cal.ListaDias[i - 1].Grafico > 0 && cal.ListaDias[i + 1].Grafico > 0) {
                            cal.ListaDias[i].Grafico = -5;
                            numero++;
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
                App.Global.ConductoresVM.InsertarRegulacion(regulacion);
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
                    App.Global.ConductoresVM.InsertarRegulacion(regulacion);
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

        private bool PuedeCalendariosEnPdf() {
            if (VistaCalendarios == null) return false;
            return VistaCalendarios.Count > 0;
        }

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
                    await CalendarioPrintModel.CrearCalendariosEnPdf_7(doc, VistaCalendarios, FechaActual);
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

        private bool PuedeCalendariosEnPdfConFallos() {
            if (VistaCalendarios == null) return false;
            return VistaCalendarios.Count > 0;
        }

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
                    await CalendarioPrintModel.FallosEnCalendariosEnPdf_7(doc, VistaCalendarios, FechaActual);
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
                Calendario cal = obj as Calendario;
                if (cal == null) continue;
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
            //TODO: Implementar.

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
                Calendario cal = obj as Calendario;
                if (cal == null) continue;
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
                Calendario cal = obj as Calendario;
                if (cal == null) continue;
                foreach (DiaCalendario dia in cal.ListaDias) {
                    dia.Resaltar = false;
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
                string nombreArchivo = String.Format("Reclamación {0:yyyy}-{0:MM} - {1:000}.pdf", Pijama.Fecha, Pijama.Trabajador.Id);
                string ruta = Informes.GetRutaArchivo(TiposInforme.Reclamacion, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente, Pijama.TextoTrabajador.Replace(":", " -"));
                if (ruta != "") {
                    if (File.Exists(ruta)) {
                        Process.Start(ruta);
                    } else {
                        iText.Layout.Document doc = Informes.GetNuevoPdf(ruta);
                        doc.GetPdfDocument().GetDocumentInfo().SetTitle("Reclamación");
                        doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaActual.ToString("MMMM-yyyy").ToUpper()}");
                        doc.SetMargins(40, 40, 40, 40);
                        await PijamaPrintModel.CrearReclamacionEnPdf(doc, Pijama.Fecha, Pijama.Trabajador);
                        doc.Close();
                        Process.Start(ruta);
                    }
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

        private bool PuedePdfEsatdisticas() {
            return true;
        }

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
                        double valor = num / VistaCalendarios.Count * 100;
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
                                estadistica.Trabajadas += dp.GraficoTrabajado.TrabajadasReales; //TODO: Verificar si son reales o ajustadas a 7:20
                                estadistica.Acumuladas += dp.GraficoTrabajado.Acumuladas;
                                estadistica.Nocturnas += dp.GraficoTrabajado.Nocturnas;
                                //TODO: Añadir dato: 
                                estadistica.TiempoPartido += dp.GraficoTrabajado.TiempoPartido;
                                // Si es menor de 7:20 se añade a menores, si no, a mayores.
                                if (estadistica.Trabajadas < App.Global.Convenio.JornadaMedia) {
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
                //TODO: Cambiar tipo de informe.
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

        private bool PuedePdfEstadisticasMes() {
            return ListaCalendarios.Count > 0;
        }

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
                    listaGraficos = BdEstadisticas.GetGraficosFromDiaCalendario(FechaActual);
                    App.Global.ValorBarraProgreso = 66;
                    listaNumeros = BdEstadisticas.GetGraficosByDia(FechaActual);
                    App.Global.ValorBarraProgreso = 95;
                    listaDescansos = BdEstadisticas.GetDescansosByDia(FechaActual);
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
            int conductor = CalendarioSeleccionado.IdConductor;
            FechaActual = FechaActual.AddMonths(-1);
            CalendarioSeleccionado = ListaCalendarios.FirstOrDefault(c => c.IdConductor == conductor);
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
            int conductor = CalendarioSeleccionado.IdConductor;
            FechaActual = FechaActual.AddMonths(1);
            CalendarioSeleccionado = ListaCalendarios.FirstOrDefault(c => c.IdConductor == conductor);
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






    }// Fin de la clase.
}
