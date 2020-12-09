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
using iText.Forms;
using Orion.Models;
using Orion.PdfExcel;
using Orion.Servicios;

namespace Orion.ViewModels.PageViewModels {

    public class ReclamacionesPageVM : PageViewModelBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private enum tipoInforme {
            MES_ACTUAL,
            TODO_EL_AÑO,
            TODO_EL_AÑO_MES_A_MES,
        }

        private const int CONDUCTORES_TODOS = 0;
        private const int CONDUCTORES_INDEFINIDOS = 1;
        private const int CONDUCTORES_EVENTUALES = 2;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static ReclamacionesPageVM instance;

        private ReclamacionesPageVM() { }

        public static ReclamacionesPageVM GetInstance() {
            if (instance == null) instance = new ReclamacionesPageVM();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private IEnumerable<Reclamacion> GetReclamacionesMes(DateTime fecha) {
            var calendarios = App.Global.Repository.GetCalendarios(fecha.Year, fecha.Month, true);
            if (!incluirTaquillas) calendarios = calendarios.Where(c => c.CategoriaConductor.ToUpper() == "C");
            if (FiltroConductores == CONDUCTORES_INDEFINIDOS) calendarios = calendarios.Where(c => c.ConductorIndefinido);
            if (FiltroConductores == CONDUCTORES_EVENTUALES) calendarios = calendarios.Where(c => !c.ConductorIndefinido);
            return ExtraerReclamaciones(calendarios);
        }

        private IEnumerable<Reclamacion> GetReclamacionesMesConductor(DateTime fecha, int matriculaConductor) {
            var calendarios = App.Global.Repository.GetCalendariosConductor(fecha.Year, fecha.Month, matriculaConductor, true);
            return ExtraerReclamaciones(calendarios);
        }

        private IEnumerable<Reclamacion> GetReclamacionesAño(DateTime fecha) {
            var calendarios = App.Global.Repository.GetCalendarios(fecha.Year, true);
            if (!incluirTaquillas) calendarios = calendarios.Where(c => c.CategoriaConductor.ToUpper() == "C");
            if (FiltroConductores == CONDUCTORES_INDEFINIDOS) calendarios = calendarios.Where(c => c.ConductorIndefinido);
            if (FiltroConductores == CONDUCTORES_EVENTUALES) calendarios = calendarios.Where(c => !c.ConductorIndefinido);
            return ExtraerReclamaciones(calendarios);
        }

        private IEnumerable<Reclamacion> GetReclamacionesAñoConductor(int año, int matriculaConductor) {
            var calendarios = App.Global.Repository.GetCalendariosConductor(año, matriculaConductor, true);
            return ExtraerReclamaciones(calendarios);
        }

        private IEnumerable<Reclamacion> ExtraerReclamaciones(IEnumerable<Calendario> listaCalendarios) {
            var lista = new List<Reclamacion>();
            foreach (var calendario in listaCalendarios) {
                // Declaramos el archivo con las reclamaciones.
                string nombreArchivo = $"Reclamación {calendario.Fecha:yyyy-MM} - {calendario.MatriculaConductor:000}.pdf";
                var conductor = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);
                string ruta = App.Global.Informes.GetRutaArchivo(TiposInforme.Reclamacion, nombreArchivo, App.Global.Configuracion.CrearInformesDirectamente, conductor.MatriculaApellidos.Replace(":", " -"));
                if (ruta != "" && File.Exists(ruta)) {
                    // Extraemos el documento PDF
                    iText.Kernel.Pdf.PdfReader reader = new iText.Kernel.Pdf.PdfReader(ruta);
                    iText.Kernel.Pdf.PdfDocument docPDF = new iText.Kernel.Pdf.PdfDocument(reader);
                    // Extraemos el formulario del documento.
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(docPDF, true);
                    // Extraemos el número de la reclamación
                    var campos = form.GetFormFields();
                    // Llenamos una lista por cada campo que contenga un dato...  nombres = Fila00Columna0
                    for (int i = 0; i < docPDF.GetNumberOfPages(); i++) {
                        for (int f = 0; f < 17; f++) {
                            var fila = (i * 17) + f;
                            var reclamacion = new Reclamacion();
                            if (campos.ContainsKey($"Fila{fila:00}Columna0")) reclamacion.Concepto = campos[$"Fila{fila:00}Columna0"].GetValueAsString();
                            if (campos.ContainsKey($"Fila{fila:00}Columna1")) reclamacion.EnPijama = campos[$"Fila{fila:00}Columna1"].GetValueAsString();
                            if (campos.ContainsKey($"Fila{fila:00}Columna2")) reclamacion.Real = campos[$"Fila{fila:00}Columna2"].GetValueAsString();
                            var diferencia = campos[$"Fila{fila:00}Columna3"].GetValueAsString().Replace(" (", "/").Replace(" €)", "");
                            if (campos.ContainsKey($"Fila{fila:00}Columna3")) reclamacion.Diferencia = diferencia;
                            if (!string.IsNullOrEmpty(reclamacion.Concepto)) {
                                lista.Add(reclamacion);
                            }
                        }
                    }
                    docPDF.Close();
                }
            }
            return lista;
        }

        private ResumenReclamacion GetResumenReclamaciones(IEnumerable<Reclamacion> listaReclamaciones, string rowHeader = "") {
            var resumen = new ResumenReclamacion();
            resumen.RowHeader = rowHeader;
            foreach (var reclamacion in listaReclamaciones) {
                resumen.Cantidad++;
                if (reclamacion.Concepto.StartsWith("Horas")) {
                    if (TimeSpan.TryParse(reclamacion.Diferencia, out TimeSpan dato)) resumen.TotalAcumuladas += dato;
                }
                if (reclamacion.Concepto.StartsWith("Dieta de desayuno")) {
                    var datos = reclamacion.Diferencia.Split('/');
                    if (decimal.TryParse(datos[0], out decimal dato)) resumen.TotalDesayunos += dato;
                    if (decimal.TryParse(datos[1], out decimal importeDato)) resumen.ImporteDesayunos += importeDato;
                }
                if (reclamacion.Concepto.StartsWith("Dieta de comida")) {
                    var datos = reclamacion.Diferencia.Split('/');
                    if (decimal.TryParse(datos[0], out decimal dato)) resumen.TotalComidas += dato;
                    if (decimal.TryParse(datos[1], out decimal importeDato)) resumen.ImporteComidas += importeDato;
                }
                if (reclamacion.Concepto.StartsWith("Dieta de cena")) {
                    var datos = reclamacion.Diferencia.Split('/');
                    if (decimal.TryParse(datos[0], out decimal dato)) resumen.TotalCenas += dato;
                    if (decimal.TryParse(datos[1], out decimal importeDato)) resumen.ImporteCenas += importeDato;
                }
                if (reclamacion.Concepto.StartsWith("Plus Cena")) {
                    var datos = reclamacion.Diferencia.Split('/');
                    if (decimal.TryParse(datos[0], out decimal dato)) resumen.TotalPlusCenas += dato;
                    if (decimal.TryParse(datos[1], out decimal importeDato)) resumen.ImportePlusCenas += importeDato;
                }
            }
            return resumen;
        }

        private PdfTableData GetTablaReclamaciones(IEnumerable<ResumenReclamacion> lista, string titulo, string firstHeader) {
            // Creamos y configuramos la tabla
            PdfTableData tabla = new PdfTableData(titulo);
            //tabla.SetUniformWidths(12);
            tabla.ColumnWidths = new List<float> { 12f, 5.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f, 7.5f };
            tabla.AlternateRowsCount = 1;
            tabla.TableTheme = PdfTableTheme.MARRON_CLARO;
            tabla.LogoPath = App.Global.Configuracion.RutaLogoSindicato;
            tabla.HeadersFontSize = 12;
            tabla.CellsFontSize = 11;
            // Encabezados
            tabla.Headers = new List<PdfCellInfo>() {
                new PdfCellInfo(firstHeader),
                new PdfCellInfo("Cant."),
                new PdfCellInfo("Horas"),
                new PdfCellInfo("Desayuno"){ MergedCells = (0, 2) },
                new PdfCellInfo(""){ IsMerged = true },
                new PdfCellInfo("Comida"){ MergedCells = (0, 2) },
                new PdfCellInfo(""){ IsMerged = true },
                new PdfCellInfo("Cena"){ MergedCells = (0, 2) },
                new PdfCellInfo(""){ IsMerged = true },
                new PdfCellInfo("Plus Cena"){ MergedCells = (0, 2) },
                new PdfCellInfo(""){ IsMerged = true },
                new PdfCellInfo("Total Dietas"){ MergedCells = (0, 2), },
                new PdfCellInfo(""){ IsMerged = true },
            };
            // Datos
            var datos = new List<List<PdfCellInfo>>();
            foreach (var resumen in lista) {
                datos.Add(new List<PdfCellInfo> {
                    new PdfCellInfo(resumen.RowHeader),
                    new PdfCellInfo(resumen.Cantidad.ToString()),
                    new PdfCellInfo(resumen.TotalAcumuladas.ToTexto()),
                    new PdfCellInfo(resumen.TotalDesayunos.ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.ImporteDesayunos.ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.TotalComidas.ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.ImporteComidas.ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.TotalCenas.ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.ImporteCenas.ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.TotalPlusCenas.ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                    new PdfCellInfo(resumen.ImportePlusCenas.ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                    new PdfCellInfo((resumen.TotalDesayunos + resumen.TotalComidas + resumen.TotalCenas + resumen.TotalPlusCenas).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                    new PdfCellInfo((resumen.ImporteDesayunos + resumen.ImporteComidas + resumen.ImporteCenas + resumen.ImportePlusCenas).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                });
            }
            tabla.Data = datos;
            // TOTALES
            tabla.Totals = new List<PdfCellInfo>() {
                new PdfCellInfo("TOTALES"),
                new PdfCellInfo(lista.Sum(r => r.Cantidad).ToString()),
                new PdfCellInfo(new TimeSpan(lista.Sum(r => r.TotalAcumuladas.Ticks)).ToTexto()),
                new PdfCellInfo(lista.Sum(r => r.TotalDesayunos).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.ImporteDesayunos).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.TotalComidas).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.ImporteComidas).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.TotalCenas).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.ImporteCenas).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.TotalPlusCenas).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                new PdfCellInfo(lista.Sum(r => r.ImportePlusCenas).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
                new PdfCellInfo((lista.Sum(r => r.TotalDesayunos) +
                                lista.Sum(r => r.TotalComidas) +
                                lista.Sum(r => r.TotalCenas) +
                                lista.Sum(r => r.TotalPlusCenas)).ToString("0.00")){ Borders = (PdfBorder.DEFAULT, PdfBorder.DEFAULT,PdfBorder.NO_BORDER,PdfBorder.DEFAULT) },
                new PdfCellInfo((lista.Sum(r => r.ImporteDesayunos) +
                                lista.Sum(r => r.ImporteComidas) +
                                lista.Sum(r => r.ImporteCenas) +
                                lista.Sum(r => r.ImportePlusCenas)).ToString("0.00 €")){ Borders = (PdfBorder.NO_BORDER, PdfBorder.DEFAULT,PdfBorder.DEFAULT,PdfBorder.DEFAULT) },
            };

            return tabla;
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        // Mapeado a los viewmodels que necesitemos usar.
        // ----------------------------------------------
        public CalendariosViewModel CalendariosVM => App.Global.CalendariosVM;


        private int tipoInformeSeleccionado;
        public int TipoInformeSeleccionado {
            get => tipoInformeSeleccionado;
            set => SetValue(ref tipoInformeSeleccionado, value);
        }



        private int filtroConductores;
        public int FiltroConductores {
            get => filtroConductores;
            set => SetValue(ref filtroConductores, value);
        }


        private bool incluirTaquillas;
        public bool IncluirTaquillas {
            get => incluirTaquillas;
            set => SetValue(ref incluirTaquillas, value);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================


        #region GENERAR PDF 

        // Comando
        private ICommand cmdGenerarPdf;
        public ICommand CmdGenerarPdf {
            get {
                if (cmdGenerarPdf == null) cmdGenerarPdf = new RelayCommand(p => GenerarPdf());
                return cmdGenerarPdf;
            }
        }
        private void GenerarPdf() {
            // Generamos la tabla.
            PdfTableData tableData = null;
            // Generamos un PDF
            var ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, $"Conductores\\Reclamaciones");
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            var nombreArchivo = string.Empty;
            var titulo = string.Empty;
            var lista = new List<ResumenReclamacion>();
            var conductores = App.Global.ConductoresVM.ListaConductores.AsEnumerable();
            switch (TipoInformeSeleccionado) {
                case 0: // Por meses
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year} - Reclamaciones Por Mes - {App.Global.CentroActual}";
                    for (int mes = 1; mes <= 12; mes++) {
                        var fecha = new DateTime(CalendariosVM.FechaActual.Year, mes, 1);
                        titulo = "RECLAMACIONES POR MES";
                        switch (FiltroConductores) {
                            case CONDUCTORES_INDEFINIDOS:
                                titulo += " (Indefinidos)";
                                break;
                            case CONDUCTORES_EVENTUALES:
                                titulo += " (Eventuales)";
                                break;
                        }
                        titulo += $"\n{App.Global.CentroActual} - {fecha:yyyy}".ToUpper();
                        lista.Add(GetResumenReclamaciones(GetReclamacionesMes(fecha), $"{fecha:MMM}".Replace(".", "").ToUpper()));
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Mes");
                    break;
                case 1: // Por conductores Mes Actual
                    nombreArchivo = $"{CalendariosVM.FechaActual:yyyy-MM} - Reclamaciones Por Conductor - {App.Global.CentroActual}";
                    if (!incluirTaquillas) conductores = conductores.Where(c => c.Categoria.ToUpper() == "C");
                    if (FiltroConductores == CONDUCTORES_INDEFINIDOS) conductores = conductores.Where(c => c.Indefinido);
                    if (FiltroConductores == CONDUCTORES_EVENTUALES) conductores = conductores.Where(c => !c.Indefinido);
                    foreach (var conductor in conductores) {
                        titulo = "RECLAMACIONES POR CONDUCTOR";
                        switch (FiltroConductores) {
                            case CONDUCTORES_INDEFINIDOS:
                                titulo += " (Indefinidos)";
                                break;
                            case CONDUCTORES_EVENTUALES:
                                titulo += " (Eventuales)";
                                break;
                        }
                        titulo += $"\n{App.Global.CentroActual} - {CalendariosVM.FechaActual:MMMM-yyyy}".ToUpper();
                        var resumen = GetResumenReclamaciones(GetReclamacionesMesConductor(CalendariosVM.FechaActual, conductor.Matricula), $"{conductor.Matricula:000}");
                        if (!resumen.IsEmpty) lista.Add(resumen);
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Conductor");
                    break;
                case 2: // Por conductores Anual
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year} - Reclamaciones Por Conductor - {App.Global.CentroActual}";
                    conductores = App.Global.ConductoresVM.ListaConductores.AsEnumerable();
                    if (!incluirTaquillas) conductores = conductores.Where(c => c.Categoria.ToUpper() == "C");
                    if (FiltroConductores == CONDUCTORES_INDEFINIDOS) conductores = conductores.Where(c => c.Indefinido);
                    if (FiltroConductores == CONDUCTORES_EVENTUALES) conductores = conductores.Where(c => !c.Indefinido);
                    foreach (var conductor in conductores) {
                        titulo = "RECLAMACIONES POR CONDUCTOR";
                        switch (FiltroConductores) {
                            case CONDUCTORES_INDEFINIDOS:
                                titulo += " (Indefinidos)";
                                break;
                            case CONDUCTORES_EVENTUALES:
                                titulo += " (Eventuales)";
                                break;
                        }
                        titulo += $"\n{App.Global.CentroActual} - {CalendariosVM.FechaActual.Year}".ToUpper();
                        var resumen = GetResumenReclamaciones(GetReclamacionesAñoConductor(CalendariosVM.FechaActual.Year, conductor.Matricula), $"{conductor.Matricula:000}");
                        if (!resumen.IsEmpty) lista.Add(resumen);
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Conductor");
                    break;
            }
            var archivo = Path.Combine(ruta, $"{nombreArchivo}.pdf");
            var docPdf = App.Global.Informes.GetNuevoPdf(archivo, true);
            docPdf.GetPdfDocument().GetDocumentInfo().SetTitle("Reclamaciones");
            docPdf.GetPdfDocument().GetDocumentInfo().SetSubject($"{nombreArchivo}");
            docPdf.SetMargins(25, 25, 25, 25);
            docPdf.Add(PdfExcelHelper.GetInstance().GetPdfTable(tableData));
            docPdf.Close();
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(archivo);
        }
        #endregion



        #region GENERAR EXCEL 

        // Comando
        private ICommand cmdGenerarExcel;
        public ICommand CmdGenerarExcel {
            get {
                if (cmdGenerarExcel == null) cmdGenerarExcel = new RelayCommand(p => GenerarExcel());
                return cmdGenerarExcel;
            }
        }
        private void GenerarExcel() {
            // Generamos la tabla.
            PdfTableData tableData = null;
            // Generamos un EXCEL
            var ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, $"Conductores\\Reclamaciones");
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            var nombreArchivo = string.Empty;
            var titulo = string.Empty;
            var lista = new List<ResumenReclamacion>();
            switch (TipoInformeSeleccionado) {
                case 0: // Por meses
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year} - Reclamaciones Por Mes - {App.Global.CentroActual}";
                    for (int mes = 1; mes <= 12; mes++) {
                        var fecha = new DateTime(CalendariosVM.FechaActual.Year, mes, 1);
                        titulo = $"RECLAMACIONES POR MES\n{App.Global.CentroActual} - {fecha:yyyy}".ToUpper();
                        lista.Add(GetResumenReclamaciones(GetReclamacionesMes(fecha), $"{fecha:MMM}".Replace(".", "").ToUpper()));
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Mes");
                    break;
                case 1: // Por conductores Mes Actual
                    nombreArchivo = $"{CalendariosVM.FechaActual:yyyy-MM} - Reclamaciones Por Conductor - {App.Global.CentroActual}";
                    foreach (var conductor in App.Global.ConductoresVM.ListaConductores) {
                        titulo = $"RECLAMACIONES POR CONDUCTOR \n{App.Global.CentroActual} - {CalendariosVM.FechaActual:MMMM-yyyy}".ToUpper();
                        var resumen = GetResumenReclamaciones(GetReclamacionesMesConductor(CalendariosVM.FechaActual, conductor.Matricula), $"{conductor.Matricula:000}");
                        if (!resumen.IsEmpty) lista.Add(resumen);
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Conductor");
                    break;
                case 2: // Por conductores Anual
                    nombreArchivo = $"{CalendariosVM.FechaActual.Year} - Reclamaciones Por Conductor - {App.Global.CentroActual}";
                    foreach (var conductor in App.Global.ConductoresVM.ListaConductores) {
                        titulo = $"RECLAMACIONES POR CONDUCTOR \n{App.Global.CentroActual} - {CalendariosVM.FechaActual.Year}".ToUpper();
                        var resumen = GetResumenReclamaciones(GetReclamacionesAñoConductor(CalendariosVM.FechaActual.Year, conductor.Matricula), $"{conductor.Matricula:000}");
                        if (!resumen.IsEmpty) lista.Add(resumen);
                    }
                    tableData = GetTablaReclamaciones(lista, titulo, $"Conductor");
                    break;
            }
            var archivo = Path.Combine(ruta, $"{nombreArchivo}.xlsx");
            PdfExcelHelper.GetInstance().SaveAsExcel(tableData, archivo, "Reclamaciones");
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(archivo);

        }
        #endregion



        #endregion
        // ====================================================================================================


    }
}
