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
    using System.Windows.Input;
    using DataModels;
    using Models;
    using PrintModel;
    using Servicios;
    using Views;

    public partial class GraficosViewModel {


        #region AÑADIR GRAFICO
        // Comando
        private ICommand _cmdañadirgrafico;
        public ICommand cmdAñadirGrafico {
            get {
                if (_cmdañadirgrafico == null) _cmdañadirgrafico = new RelayCommand(p => AñadirGrafico(), p => PuedeAñadirGrafico());
                return _cmdañadirgrafico;
            }
        }

        // Se puede ejecutar
        private bool PuedeAñadirGrafico() {
            return HayGrupo;
        }

        // Ejecución del comando
        private void AñadirGrafico() {

            // Creamos la ventana
            VentanaAñadirGrafico ventana;

            // Creamos las variables auxiliares
            bool incrementar = true;
            bool deducir = true;
            int numero = 0;
            if (ListaGraficos.Count > 0) numero = ListaGraficos.Max(g => g.Numero);

            // Iniciamos el bucle para mostrar la ventana.
            do {
                // Instanciamos la ventana y la vinculamos al ViewModel correspondiente.
                ventana = new VentanaAñadirGrafico();
                ventana.DataContext = new AñadirGraficoViewModel(Mensajes);
                ((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado = incrementar;
                ((AñadirGraficoViewModel)ventana.DataContext).DeducirTurnoMarcado = deducir;
                ((AñadirGraficoViewModel)ventana.DataContext).Numero = ((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado ? numero + 1 : numero;
                // Si al mostrar la ventana, pulsamos aceptar...
                if (ventana.ShowDialog() == true) {
                    // Creamos el gráfico y lo guardamos en la lista.
                    Grafico grafico = ((AñadirGraficoViewModel)ventana.DataContext).GraficoActual;
                    //grafico.IdGrupo = GrupoSeleccionado.Id;
                    grafico.Validez = GrupoSeleccionado.Validez;
                    grafico.DiaSemana = "";
                    ListaGraficos.Add(grafico);
                    // Establecemos las variables.
                    incrementar = ((AñadirGraficoViewModel)ventana.DataContext).IncrementarNumeroMarcado;
                    deducir = ((AñadirGraficoViewModel)ventana.DataContext).DeducirTurnoMarcado;
                    numero = ((AñadirGraficoViewModel)ventana.DataContext).Numero;
                } else {
                    return;
                }
            } while (true);

        }
        #endregion


        #region BORRAR GRAFICO
        // Comando
        private ICommand _cmdborrargrafico;
        public ICommand cmdBorrarGrafico {
            get {
                if (_cmdborrargrafico == null) _cmdborrargrafico = new RelayCommand(p => BorrarGrafico(), p => PuedeBorrarGrafico());
                return _cmdborrargrafico;
            }
        }

        // Se puede ejecutar
        private bool PuedeBorrarGrafico() {
            return GraficoSeleccionado != null;
        }

        // Ejecución del comando
        private void BorrarGrafico() {
            _listaborrados.Add(GraficoSeleccionado);
            _listagraficos.Remove(GraficoSeleccionado);
            HayCambios = true;
            PropiedadCambiada(nameof(Detalle));
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
            foreach (Grafico grafico in _listaborrados) {
                if (grafico.Nuevo) {
                    _listagraficos.Add(grafico);
                } else {
                    _listagraficos.Add(grafico);
                    grafico.Nuevo = false;
                }
                HayCambios = true;
                PropiedadCambiada(nameof(Detalle));
            }
            _listaborrados.Clear();
        }
        #endregion


        #region CALCULAR TABLA
        private ICommand _cmdcalculartabla;
        public ICommand cmdCalcularTabla {
            get {
                if (_cmdcalculartabla == null) _cmdcalculartabla = new RelayCommand(p => CalcularTabla(), p => PuedeCalcularTabla());
                return _cmdcalculartabla;
            }
        }

        private bool PuedeCalcularTabla() {
            return _listagraficos.Count > 0;
        }

        private void CalcularTabla() {
            foreach (Grafico grafico in _listagraficos) {
                grafico.Recalcular();
                HayCambios = true;
            }
        }
        #endregion


        #region BORRAR GRUPO
        //Comando
        private ICommand _cmdborrargrupo;
        public ICommand cmdBorrarGrupo {
            get {
                if (_cmdborrargrupo == null) _cmdborrargrupo = new RelayCommand(p => BorrarGrupo(), p => PuedeBorrarGrupo());
                return _cmdborrargrupo;
            }
        }

        // Puede ejecutarse
        private bool PuedeBorrarGrupo() {
            return GrupoSeleccionado != null;
        }

        // Ejecución del comando
        private void BorrarGrupo() {
            bool? resultado = Mensajes.VerMensaje("Va a borrar el grupo " + GrupoSeleccionado.Validez.ToString("dd-MM-yyyy") +
                                                         " con " + _listagraficos.Count.ToString() + " gráficos." +
                                                         "\n\n¿Desea continuar?", "BORRAR GRUPO",
                                                         true);

            if (resultado != true) return;
            try {
                //BdGruposGraficos.BorrarGrupoPorId(GrupoSeleccionado.Id);
                App.Global.Repository.BorrarGrupoPorFecha(GrupoSeleccionado.Validez);
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.BorrarGrupo", ex);
            }
            CargarGrupos();
            HayCambios = true;
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
            if (VistaGraficos != null) VistaGraficos.Filter = null;
            FiltroAplicado = "Ninguno";
            PropiedadCambiada(nameof(Detalle));
        }
        #endregion


        #region APLICAR FILTRO
        // Comando
        private ICommand _cmdaplicarfiltro;
        public ICommand cmdAplicarFiltro {
            get {
                if (_cmdaplicarfiltro == null) _cmdaplicarfiltro = new RelayCommand(p => AplicarFiltro(p), p => PuedeAplicarFiltro());
                return _cmdaplicarfiltro;
            }
        }

        // Puede ejecutarse
        private bool PuedeAplicarFiltro() {
            return (HayGrupo && ListaGraficos.Count > 0 && !(VistaGraficos.IsAddingNew || VistaGraficos.IsEditingItem));
        }


        private void AplicarFiltro(object parametro) {
            switch (parametro as string) {
                case "LunJue":
                    VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "L"); };
                    FiltroAplicado = "Lunes a Jueves";
                    break;
                case "Vie":
                    VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "V"); };
                    FiltroAplicado = "Viernes";
                    break;
                case "Sab":
                    VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "S"); };
                    FiltroAplicado = "Sábados";
                    break;
                case "Fes":
                    VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "F"); };
                    FiltroAplicado = "Domingos y Festivos";
                    break;
                case "ReduccionJornada":
                    VistaGraficos.Filter = (g) => { return ((g as Grafico).DiaSemana == "R"); };
                    FiltroAplicado = "Reducciones de Jornada";
                    break;
                case "Modificados":
                    VistaGraficos.Filter = (g) => { return (g as Grafico).Diferente == true; };
                    FiltroAplicado = "Gráficos Modificados";
                    break;
                default:
                    if (VistaGraficos != null) VistaGraficos.Filter = null;
                    FiltroAplicado = "Ninguno";
                    break;
            }
            PropiedadCambiada(nameof(Detalle));
        }
        #endregion


        #region AÑADIR VALORACION
        // Comando
        private ICommand _cmdañadirvaloracion;
        public ICommand cmdAñadirValoracion {
            get {
                if (_cmdañadirvaloracion == null) _cmdañadirvaloracion = new RelayCommand(p => AñadirValoracion(), p => PuedeAñadirValoracion());
                return _cmdañadirvaloracion;
            }
        }

        // Se puede ejecutar
        private bool PuedeAñadirValoracion() {
            if (GraficoSeleccionado == null) return false;
            return HayGrupo;
        }

        // Ejecución del comando
        private void AñadirValoracion() {

            // Creamos la ventana
            VentanaAñadirValoracionGrafico ventana;

            // Creamos las variables auxiliares
            TimeSpan? Inicio = GraficoSeleccionado.HoraFinalValoraciones();
            if (!Inicio.HasValue) {
                Inicio = GraficoSeleccionado.Inicio;
            }

            // Iniciamos el bucle para mostrar la ventana.
            do {
                // Instanciamos la ventana y la vinculamos al ViewModel correspondiente.
                ventana = new VentanaAñadirValoracionGrafico();
                AñadirValoracionGraficoViewModel contextoVentana = new AñadirValoracionGraficoViewModel(Mensajes);
                ventana.DataContext = contextoVentana;

                // Asignamos el inicio de la ventana y el número de gráfico
                contextoVentana.Inicio = Inicio;
                contextoVentana.NumeroGrafico = GraficoSeleccionado.Numero;

                // Creamos la valoración a añadir.
                ValoracionGrafico valoracion = new ValoracionGrafico();

                // Si al mostrar la ventana, pulsamos aceptar...
                if (ventana.ShowDialog() == true) {
                    //Extraemos la línea.
                    decimal Linea = contextoVentana.Linea;
                    // Ponemos el inicio a la valoracion.
                    valoracion.Inicio = contextoVentana.Inicio;
                    // Si la línea es menor que cero...
                    if (Linea < 0) {
                        // Cambiamos el signo.
                        Linea = Linea * -1;
                        // Si está entre 1000 y 1999...
                        if (Linea >= 1000 && Linea < 2000) {
                            // Extraemos el descanso.
                            int descanso = (int)(Linea - 1000);
                            valoracion.Descripcion = "Descanso " + descanso.ToString() + " minutos.";
                            valoracion.Final = contextoVentana.Inicio + new TimeSpan(0, descanso, 0);
                        }
                        // Si la línea es mayor que cero.
                    } else if (Linea > 0) {
                        // Creamos un itinerario.
                        Itinerario itinerario = null;
                        // Buscamos el itinerario.
                        try {
                            itinerario = BdItinerarios.GetItinerarioByNombre(Linea);
                        } catch (Exception ex) {
                            Mensajes.VerError("GraficosViewModel.AñadirValoracion", ex);
                        }

                        // Si el itinerario no es nulo (existe)...
                        if (itinerario != null) {
                            valoracion.Linea = Linea;
                            valoracion.Descripcion = itinerario.Descripcion;
                            valoracion.Final = contextoVentana.Inicio + new TimeSpan(0, itinerario.TiempoReal, 0);
                        } else {
                            valoracion.Linea = Linea;
                            valoracion.Descripcion = "Línea desconocida.";
                            valoracion.Final = contextoVentana.Inicio;
                        }

                        // Si la línea es cero.
                    } else {
                        valoracion.Final = contextoVentana.Inicio;
                    }
                    // Guardamos la valoracion
                    valoracion.IdGrafico = GraficoSeleccionado.Id;
                    GraficoSeleccionado.ListaValoraciones.Add(valoracion);
                    HayCambios = true;
                    // Ponemos Inicio en la hora final
                    Inicio = valoracion.Final;
                } else {
                    return;
                }
            } while (true);

        }
        #endregion


        #region BORRAR VALORACION
        // Comando
        private ICommand _cmdborrarvaloracion;
        public ICommand cmdBorrarValoracion {
            get {
                if (_cmdborrarvaloracion == null) _cmdborrarvaloracion = new RelayCommand(p => BorrarValoracion(), p => PuedeBorrarValoracion());
                return _cmdborrarvaloracion;
            }
        }

        // Se puede ejecutar
        private bool PuedeBorrarValoracion() {
            return ValoracionSeleccionada != null;
        }

        // Ejecución del comando
        private void BorrarValoracion() {
            _valoracionesborradas.Add(ValoracionSeleccionada);
            GraficoSeleccionado.ListaValoraciones.Remove(ValoracionSeleccionada);
            HayCambios = true;
        }
        #endregion


        #region DESHACER BORRAR VALORACIONES
        private ICommand _cmddeshacerborrarvaloraciones;
        public ICommand cmdDeshacerBorrarValoraciones {
            get {
                if (_cmddeshacerborrarvaloraciones == null) {
                    _cmddeshacerborrarvaloraciones = new RelayCommand(p => DeshacerBorrarValoraciones(), p => PuedeDeshacerBorrarValoraciones());
                }
                return _cmddeshacerborrarvaloraciones;
            }
        }

        private bool PuedeDeshacerBorrarValoraciones() {
            return _valoracionesborradas.Count > 0;
        }

        private void DeshacerBorrarValoraciones() {
            if (_valoracionesborradas == null) return;
            foreach (ValoracionGrafico valoracion in _valoracionesborradas) {
                if (valoracion.Nuevo) {
                    GraficoSeleccionado.ListaValoraciones.Add(valoracion);
                } else {
                    GraficoSeleccionado.ListaValoraciones.Add(valoracion);
                    valoracion.Nuevo = false;
                }
                HayCambios = true; ;
            }
            _valoracionesborradas.Clear();
            //Propiedades.DatosModificados = true;
        }
        #endregion


        #region CALCULAR VALORACIONES
        private ICommand _cmdcalcularvaloraciones;
        public ICommand cmdCalcularValoraciones {
            get {
                if (_cmdcalcularvaloraciones == null) _cmdcalcularvaloraciones = new RelayCommand(p => CalcularValoraciones(), p => PuedeCalcularValoraciones());
                return _cmdcalcularvaloraciones;
            }
        }

        private bool PuedeCalcularValoraciones() {
            if (GraficoSeleccionado == null) return false;
            return GraficoSeleccionado.ListaValoraciones.Count > 0;
        }

        private void CalcularValoraciones() {
            foreach (ValoracionGrafico valoracion in GraficoSeleccionado.ListaValoraciones) {
                valoracion.Calcular();
                HayCambios = true;
            }
            //Propiedades.DatosModificados = true;
        }
        #endregion


        #region NUEVO GRUPO
        //Comando
        private ICommand _cmdnuevogrupo;
        public ICommand cmdNuevoGrupo {
            get {
                if (_cmdnuevogrupo == null) _cmdnuevogrupo = new RelayCommand(p => NuevoGrupo());
                return _cmdnuevogrupo;
            }
        }

        // Ejecución del comando
        private void NuevoGrupo() {
            VentanaNuevoGrupoVM ventanaVM = new VentanaNuevoGrupoVM(new MensajesServicio()) { FechaActual = DateTime.Now };
            ventanaVM.ListaGrupos = ListaGrupos.ToList();
            if (ListaGrupos.Count > 0) ventanaVM.GrupoSeleccionado = ListaGrupos[ListaGrupos.Count - 1];
            VentanaNuevoGrupo ventana = new VentanaNuevoGrupo { DataContext = ventanaVM };
            if (ventana.ShowDialog() == true) CargarGrupos();
            ventana = null;
            ventanaVM = null;
        }
        #endregion


        #region COMPARAR GRUPO
        private ICommand _cmdcomparargrupo;
        public ICommand cmdCompararGrupo {
            get {
                if (_cmdcomparargrupo == null) _cmdcomparargrupo = new RelayCommand(p => CompararGrupo(), p => PuedeCompararGrupo());
                return _cmdcomparargrupo;
            }
        }

        private bool PuedeCompararGrupo() {
            return HayGrupo && GrupoComparacionSeleccionado != null;
        }

        private void CompararGrupo() {

            if (GrupoSeleccionado.Validez.Ticks == GrupoComparacionSeleccionado.Validez.Ticks) return;

            List<Grafico> anteriores = null;
            try {
                anteriores = BdGraficos.getGraficos(GrupoComparacionSeleccionado.Id).ToList();
                if (anteriores.Count == 0) return;
            } catch (Exception ex) {
                Mensajes.VerError("GraficosViewModel.CompararGrupo", ex);
            }

            foreach (Grafico grafico in ListaGraficos) {
                Grafico graficoanterior = anteriores.Find(g => g.Numero == grafico.Numero);
                if (grafico.Equals(graficoanterior)) {
                    grafico.Diferente = false;
                } else {
                    grafico.Diferente = true;
                }
            }
        }
        #endregion


        #region MODIFICAR FECHA GRUPO
        // Comando
        private ICommand _modificarfechagrupo;
        public ICommand cmdModificarFechaGrupo {
            get {
                if (_modificarfechagrupo == null) _modificarfechagrupo = new RelayCommand(p => ModificarFechaGrupo(), p => PuedeModificarFechaGrupo());
                return _modificarfechagrupo;
            }
        }

        // Se puede ejecutar
        private bool PuedeModificarFechaGrupo() {
            if (GrupoSeleccionado == null) return false;
            return HayGrupo;
        }

        // Ejecución del comando
        private void ModificarFechaGrupo() {

            // Avisamos de que se va a cambiar la fecha, y si contestamos no, salimos.
            if (Mensajes.VerMensaje("Vas a cambiar la fecha de validez del grupo.\n" +
                                       "Esto puede alterar los calendarios que dependían de la fecha anterior\n\n" +
                                       "¿Desea continuar?", "Cambiar fecha del grupo", true) == false) return;

        }
        #endregion


        #region SELECCIONAR FECHA GRUPO

        private ICommand _seleccionarfechagrupo;
        public ICommand cmdSeleccionarFechaGrupo {
            get {
                if (_seleccionarfechagrupo == null) _seleccionarfechagrupo = new RelayCommand(p => SeleccionarFechaGrupo(), p => PuedeSeleccionarFechaGrupo());
                return _seleccionarfechagrupo;
            }
        }

        private bool PuedeSeleccionarFechaGrupo() {
            if (GrupoSeleccionado == null) return false;
            return HayGrupo;
        }

        private void SeleccionarFechaGrupo() {
            HayCambios = true;
        }
        #endregion


        #region GRÁFICOS EN PDF

        private ICommand _cmdgraficosenpdf;
        public ICommand cmdGraficosEnPdf {
            get {
                if (_cmdgraficosenpdf == null) _cmdgraficosenpdf = new RelayCommand(p => GraficosEnPDF(), p => PuedeGraficosEnPdf());
                return _cmdgraficosenpdf;
            }
        }

        private bool PuedeGraficosEnPdf() {
            return ListaGraficos.Count > 0;
        }

        private async void GraficosEnPDF() {
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1}", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.Graficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Gráficos");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{GrupoSeleccionado.Validez.ToString("dd-MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await GraficosPrintModel.CrearGraficosEnPdf(doc, VistaGraficos, GrupoSeleccionado.Validez);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }


        #endregion


        #region GRÁFICOS INDIVIDUALES EN PDF
        // Comando
        private ICommand _cmdgraficosindividualesenpdf;
        public ICommand cmdGraficosIndividualesEnPdf {
            get {
                if (_cmdgraficosindividualesenpdf == null) _cmdgraficosindividualesenpdf = new RelayCommand(p => GraficosIndividualesEnPDF(),
                                                                                                            p => PuedeGraficosIndividualesEnPdf());
                return _cmdgraficosindividualesenpdf;
            }
        }

        private bool PuedeGraficosIndividualesEnPdf() {
            return ListaGraficos.Count > 0;
        }

        private async void GraficosIndividualesEnPDF() {

            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Definimos el nombre del archivo a guardar.
                string nombreArchivo = String.Format("{0:yyyy}-{0:MM}-{0:dd} - {1} (Individuales)", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
                if (FiltroAplicado != "Ninguno") nombreArchivo += $" - ({FiltroAplicado})";
                nombreArchivo += ".pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.GraficoIndividual, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdfA5(ruta);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Gráficos Individuales");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{GrupoSeleccionado.Validez.ToString("dd-MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await GraficosPrintModel.CrearGraficosIndividualesEnPdf(doc, VistaGraficos, GrupoSeleccionado.Validez);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.GraficosEnPDF", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }

        #endregion


        #region SOLO AÑO ACTUAL EN GRUPOS
        // Comando
        private ICommand _soloañoactualengrupos;
        public ICommand cmdSoloAñoActualEnGrupos {
            get {
                if (_soloañoactualengrupos == null) _soloañoactualengrupos = new RelayCommand(p => SoloAñoActualEnGrupos());
                return _soloañoactualengrupos;
            }
        }

        private void SoloAñoActualEnGrupos() {
            if (VistaGrupos == null) return;
            if (App.Global.Configuracion.SoloAñoActualEnGruposGraficos) {
                VistaGrupos.Filter = (g) => { return (g as GrupoGraficos).Validez.Year == DateTime.Now.Year; };
            } else {
                VistaGrupos.Filter = null;
            }
        }
        #endregion


        #region ESTADÍSTICAS GRÁFICOS EN PDF

        private ICommand _cmdestadisticasgraficos;
        public ICommand cmdEstadisticasGraficos {
            get {
                if (_cmdestadisticasgraficos == null) _cmdestadisticasgraficos = new RelayCommand(p => EstadisticasGraficos(), p => PuedeEstadisticasGraficos());
                return _cmdestadisticasgraficos;
            }
        }

        private bool PuedeEstadisticasGraficos() {
            return VistaGraficos?.Count > 0;
        }

        private async void EstadisticasGraficos() {
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("Estadisticas Gráficos {0:yyyy}-{0:MM}-{0:dd} - {1}.pdf", GrupoSeleccionado.Validez, App.Global.CentroActual.ToString());
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas Gráficos");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{GrupoSeleccionado.Validez.ToString("dd-MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await GraficosPrintModel.CrearEstadisticasGraficosEnPdf(doc, GrupoSeleccionado.Validez);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.EstadisticasGraficos", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }
        }

        #endregion


        #region ESTADÍSTICAS GRUPOS GRÁFICOS EN PDF

        private ICommand _cmdestadisticasgruposgraficos;
        public ICommand cmdEstadisticasGruposGraficos {
            get {
                if (_cmdestadisticasgruposgraficos == null) _cmdestadisticasgruposgraficos = new RelayCommand(p => EstadisticasGruposGraficos(), p => PuedeEstadisticasGruposGraficos());
                return _cmdestadisticasgruposgraficos;
            }
        }

        private bool PuedeEstadisticasGruposGraficos() {
            return ListaGrupos.Count > 0;
        }

        private async void EstadisticasGruposGraficos() {
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = String.Format("Estadisticas Gráficos - {0}.pdf", App.Global.CentroActual.ToString());
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas Gráficos");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaEstadisticas.ToString("dd-MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);
                    await GraficosPrintModel.CrearEstadisticasGruposGraficosEnPdf(doc, FechaEstadisticas);
                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.EstadisticasGruposGraficos", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }


        #endregion


        #region ESTADÍSTICAS GRÁFICOS POR CENTROS

        private ICommand _cmdestadisticasgraficosporcentros;
        public ICommand cmdEstadisticasGraficosPorCentros {
            get {
                if (_cmdestadisticasgraficosporcentros == null) _cmdestadisticasgraficosporcentros = new RelayCommand(p => EstadisticasGraficosPorCentros());
                return _cmdestadisticasgraficosporcentros;
            }
        }

        private async void EstadisticasGraficosPorCentros() {
            try {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Creando PDF...");
                // Pedimos el archivo donde guardarlo.
                string nombreArchivo = "Estadisticas Gráficos Por Centros.pdf";
                string ruta = Informes.GetRutaArchivo(TiposInforme.EstadisticasGraficos, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente);
                if (ruta != "") {
                    iText.Layout.Document doc = Informes.GetNuevoPdf(ruta, true);
                    doc.GetPdfDocument().GetDocumentInfo().SetTitle("Estadísticas Gráficos");
                    doc.GetPdfDocument().GetDocumentInfo().SetSubject($"{FechaEstadisticas.ToString("dd-MMMM-yyyy").ToUpper()}");
                    doc.SetMargins(25, 25, 25, 25);


                    List<EstadisticasGraficos> lista = new List<EstadisticasGraficos>();
                    List<EstadisticasGraficos> listaTemporal;
                    GrupoGraficos grupo = null;
                    // Estadisticas de Bilbao
                    var repoTemporal = new OrionRepository(App.Global.GetCadenaConexionSQL(Centros.Bilbao));
                    grupo = repoTemporal.GetUltimoGrupo();
                    if (grupo != null) {
                        listaTemporal = repoTemporal.GetEstadisticasGrupoGraficos(grupo.Validez, App.Global.Convenio.JornadaMedia).ToList();
                        foreach (EstadisticasGraficos e in listaTemporal) {
                            e.Centro = Centros.Bilbao;
                            lista.Add(e);
                        }
                    }
                    // Estadisticas de Donosti
                    repoTemporal = new OrionRepository(App.Global.GetCadenaConexionSQL(Centros.Donosti));
                    grupo = repoTemporal.GetUltimoGrupo();
                    if (grupo != null) {
                        listaTemporal = repoTemporal.GetEstadisticasGrupoGraficos(grupo.Validez, App.Global.Convenio.JornadaMedia).ToList();
                        foreach (EstadisticasGraficos e in listaTemporal) {
                            e.Centro = Centros.Donosti;
                            lista.Add(e);
                        }
                    }
                    // Estadisticas de Arrasate
                    repoTemporal = new OrionRepository(App.Global.GetCadenaConexionSQL(Centros.Arrasate));
                    grupo = repoTemporal.GetUltimoGrupo();
                    if (grupo != null) {
                        listaTemporal = repoTemporal.GetEstadisticasGrupoGraficos(grupo.Validez, App.Global.Convenio.JornadaMedia).ToList();
                        foreach (EstadisticasGraficos e in listaTemporal) {
                            e.Centro = Centros.Arrasate;
                            lista.Add(e);
                        }
                    }
                    // Estadisticas de Vitoria
                    repoTemporal = new OrionRepository(App.Global.GetCadenaConexionSQL(Centros.Vitoria));
                    grupo = repoTemporal.GetUltimoGrupo();
                    if (grupo != null) {
                        listaTemporal = repoTemporal.GetEstadisticasGrupoGraficos(grupo.Validez, App.Global.Convenio.JornadaMedia).ToList();
                        foreach (EstadisticasGraficos e in listaTemporal) {
                            e.Centro = Centros.Vitoria;
                            lista.Add(e);
                        }
                    }

                    await GraficosPrintModel.CrearEstadisticasGraficosPorCentros(doc, lista);

                    doc.Close();
                    if (App.Global.Configuracion.AbrirPDFs) Process.Start(ruta);
                }
            } catch (Exception ex) {
                Mensajes.VerError("GraficosCommands.EstadisticasGruposGraficos", ex);
            } finally {
                App.Global.FinalizarProgreso();
            }

        }

        #endregion


        #region COMANDO DEDUCIR DÍA SEMANA

        private ICommand _cmddeducirdiasemana;
        public ICommand cmdDeducirDiaSemana {
            get {
                if (_cmddeducirdiasemana == null) _cmddeducirdiasemana = new RelayCommand(p => DeducirDiaSemana(), p => PuedeDeducirDiaSemana());
                return _cmddeducirdiasemana;
            }
        }

        private bool PuedeDeducirDiaSemana() {
            return ListaGraficos.Any();
        }

        private void DeducirDiaSemana() {
            foreach (Grafico grafico in _listagraficos) {
                if (string.IsNullOrWhiteSpace(grafico.DiaSemana)) {
                    //    if (grafico.Numero >= App.Global.PorCentro.LunDel && grafico.Numero <= App.Global.PorCentro.LunAl) grafico.DiaSemana = "L";
                    //    if (grafico.Numero >= App.Global.PorCentro.VieDel && grafico.Numero <= App.Global.PorCentro.VieAl) grafico.DiaSemana = "V";
                    //    if (grafico.Numero >= App.Global.PorCentro.SabDel && grafico.Numero <= App.Global.PorCentro.SabAl) grafico.DiaSemana = "S";
                    //    if (grafico.Numero >= App.Global.PorCentro.DomDel && grafico.Numero <= App.Global.PorCentro.DomAl) grafico.DiaSemana = "F";
                    if (App.Global.PorCentro.RangoLun.Validar(grafico.Numero)) grafico.DiaSemana = "L";
                    if (App.Global.PorCentro.RangoVie.Validar(grafico.Numero)) grafico.DiaSemana = "V";
                    if (App.Global.PorCentro.RangoSab.Validar(grafico.Numero)) grafico.DiaSemana = "S";
                    if (App.Global.PorCentro.RangoDom.Validar(grafico.Numero)) grafico.DiaSemana = "F";
                }
            }
        }
        #endregion


        #region COMANDO COMPARAR DIETAS

        // Comando
        private ICommand _compararDietasexcel;
        public ICommand cmdCompararDietasExcel {
            get {
                if (_compararDietasexcel == null) _compararDietasexcel = new RelayCommand(p => CompararDietasExcel(), p => PuedeCompararDietasExcel());
                return _compararDietasexcel;
            }
        }


        // Se puede ejecutar el comando
        private bool PuedeCompararDietasExcel() {
            return ListaGraficos.Count > 0;
        }

        // Ejecución del comando
        private void CompararDietasExcel() {

            // Definir el nombre del excel de destino.
            string nombreArchivo = $"Comparación Dietas {GrupoSeleccionado.Validez.ToString("yyyy-MM-dd")} - {App.Global.CentroActual}.xlsx";
            string rutaInformes = Path.Combine(App.Global.Configuracion.CarpetaInformes, "Comparación Dietas");
            if (!Directory.Exists(rutaInformes)) Directory.CreateDirectory(rutaInformes);
            string rutaDestino = Path.Combine(rutaInformes, nombreArchivo);

            // Pedimos el archivo excel que se va a comparar.
            string titulo = "Seleccione un excel con las dietas de la empresa.";
            string excel = FileService.FileDialog(titulo, @"C:\");
            if (excel == "") return;

            ExcelService.getInstance().GenerarComparacionGraficos(rutaDestino, ListaGraficos, excel);

            if (App.Global.Configuracion.AbrirPDFs) Process.Start(rutaDestino);
        }
        #endregion


    } //Fin de la clase.
}
