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
using System.Windows.Media;
using Orion.Models;
using Orion.PdfExcel;

namespace Orion.ViewModels.PageViewModels {

    public class CompararGraficosPageVM : PageViewModelBase {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private PdfTableData TableData;


        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static CompararGraficosPageVM instance;

        public static CompararGraficosPageVM GetInstance() {
            return instance ?? new CompararGraficosPageVM();
        }

        private CompararGraficosPageVM() {
            instance = this;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        // Mapeado a los viewmodels que necesitemos usar.
        // ----------------------------------------------
        public GraficosViewModel GraficosVM => App.Global.GraficosVM;



        private GrupoGraficos grupoSeleccionado1;
        public GrupoGraficos GrupoSeleccionado1 {
            get => grupoSeleccionado1;
            set {
                if (SetValue(ref grupoSeleccionado1, value)) Lista1 = null;
            }
        }


        private GrupoGraficos grupoSeleccionado2;
        public GrupoGraficos GrupoSeleccionado2 {
            get => grupoSeleccionado2;
            set {
                if (SetValue(ref grupoSeleccionado2, value)) Lista2 = null;
            }
        }


        private List<Tuple<int, string>> graficosComparados;
        public List<Tuple<int, string>> GraficosComparados {
            get => graficosComparados;
            set => SetValue(ref graficosComparados, value);
        }


        private IEnumerable<Grafico> lista1;
        public IEnumerable<Grafico> Lista1 {
            get => lista1;
            set => SetValue(ref lista1, value);
        }


        private IEnumerable<Grafico> lista2;
        public IEnumerable<Grafico> Lista2 {
            get => lista2;
            set => SetValue(ref lista2, value);
        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private PdfTableData GenerarPdfTableData() {
            // Unimos los gráficos de las dos listas, sin duplicados y ordenados.
            var graficos = Lista1.Select(g => g.Numero).Union(Lista2.Select(gg => gg.Numero)).OrderBy(n => n);
            if (graficos?.Any() != true) return null;
            // Creamos los datos que se van a guardar en la tabla.
            var datos = new List<List<PdfCellInfo>>();
            // Recorremos cada gráfico de la lista conjunta y los añadimos a la tabla.
            foreach (var grafico in graficos) {
                // Creamos las dos listas de celdas.
                var celdas1 = new List<PdfCellInfo>();
                var celdas2 = new List<PdfCellInfo>();
                // Extraemos el gráfico de las listas.
                var g1 = Lista1.FirstOrDefault(g => g.Numero == grafico);
                var g2 = Lista2.FirstOrDefault(g => g.Numero == grafico);
                // GRÁFICO
                celdas1.Add(new PdfCellInfo(grafico.ToString()) {
                    MergedCells = (2, 1),
                    IsBold = true,
                    TextFontColor = GraficosComparados.Any(g => g.Item1 == grafico) ? Colors.Red : Colors.DimGray,
                    TextFontSize = 12,
                });
                celdas2.Add(new PdfCellInfo() { IsMerged = true });
                // GRUPO
                celdas1.Add(new PdfCellInfo($"{GrupoSeleccionado1.Validez:dd-MM-yyyy}") {
                    TextFontColor = g1 == null ? Colors.Red : Colors.DimGray,
                });
                celdas2.Add(new PdfCellInfo($"{GrupoSeleccionado2.Validez:dd-MM-yyyy}") {
                    TextFontColor = g2 == null ? Colors.Red : Colors.DimGray,
                });
                // TURNO
                celdas1.Add(new PdfCellInfo(g1?.Turno.ToString() ?? string.Empty) {
                    TextFontColor = g1?.Turno == g2?.Turno ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Turno.ToString() ?? string.Empty) {
                    TextFontColor = g1?.Turno == g2?.Turno ? Colors.DimGray : Colors.Red
                });
                // INICIO
                celdas1.Add(new PdfCellInfo(g1?.Inicio?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Inicio == g2?.Inicio ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Inicio?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Inicio == g2?.Inicio ? Colors.DimGray : Colors.Red
                });
                // FINAL
                celdas1.Add(new PdfCellInfo(g1?.Final?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Final == g2?.Final ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Final?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Final == g2?.Final ? Colors.DimGray : Colors.Red
                });
                // INICIO PARTIDO
                celdas1.Add(new PdfCellInfo(g1?.InicioPartido?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.InicioPartido == g2?.InicioPartido ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.InicioPartido?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.InicioPartido == g2?.InicioPartido ? Colors.DimGray : Colors.Red
                });
                // FINAL PARTIDO
                celdas1.Add(new PdfCellInfo(g1?.FinalPartido?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.FinalPartido == g2?.FinalPartido ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.FinalPartido?.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.FinalPartido == g2?.FinalPartido ? Colors.DimGray : Colors.Red
                });
                // VALORACIÓN
                celdas1.Add(new PdfCellInfo(g1?.Valoracion.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Valoracion == g2?.Valoracion ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Valoracion.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Valoracion == g2?.Valoracion ? Colors.DimGray : Colors.Red
                });
                // TRABAJADAS
                celdas1.Add(new PdfCellInfo(g1?.Trabajadas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Trabajadas == g2?.Trabajadas ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Trabajadas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Trabajadas == g2?.Trabajadas ? Colors.DimGray : Colors.Red
                });
                // ACUMULADAS
                celdas1.Add(new PdfCellInfo(g1?.Acumuladas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Acumuladas == g2?.Acumuladas ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Acumuladas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Acumuladas == g2?.Acumuladas ? Colors.DimGray : Colors.Red
                });
                // NOCTURNAS
                celdas1.Add(new PdfCellInfo(g1?.Nocturnas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Nocturnas == g2?.Nocturnas ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Nocturnas.ToTexto() ?? string.Empty) {
                    TextFontColor = g1?.Nocturnas == g2?.Nocturnas ? Colors.DimGray : Colors.Red
                });
                // DESAYUNO
                celdas1.Add(new PdfCellInfo(g1?.Desayuno.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Desayuno == g2?.Desayuno ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Desayuno.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Desayuno == g2?.Desayuno ? Colors.DimGray : Colors.Red
                });
                // COMIDA
                celdas1.Add(new PdfCellInfo(g1?.Comida.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Comida == g2?.Comida ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Comida.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Comida == g2?.Comida ? Colors.DimGray : Colors.Red
                });
                // CENA
                celdas1.Add(new PdfCellInfo(g1?.Cena.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Cena == g2?.Cena ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.Cena.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.Cena == g2?.Cena ? Colors.DimGray : Colors.Red
                });
                // PLUS CENA
                celdas1.Add(new PdfCellInfo(g1?.PlusCena.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.PlusCena == g2?.PlusCena ? Colors.DimGray : Colors.Red
                });
                celdas2.Add(new PdfCellInfo(g2?.PlusCena.ToString("0.00") ?? string.Empty) {
                    TextFontColor = g1?.PlusCena == g2?.PlusCena ? Colors.DimGray : Colors.Red
                });
                // Añadimos las dos filas a la lista
                datos.Add(celdas1);
                datos.Add(celdas2);

            }
            // Preparamos el objeto PdfTableData
            var titulo = $"COMPARACIÓN GRÁFICOS {App.Global.CentroActual.ToString().ToUpper()}\n" +
                $"{GrupoSeleccionado1.Validez:dd-MM-yyyy}  -  {GrupoSeleccionado2.Validez:dd-MM-yyyy}";
            var tabla = new PdfTableData(titulo);
            tabla.SetUniformWidths(15);
            tabla.AlternateRowsCount = 2;
            tabla.TableTheme = PdfTableTheme.LIMA;
            tabla.LogoPath = App.Global.Configuracion.RutaLogoSindicato;
            // Creamos los encabezados.
            tabla.Headers = new List<PdfCellInfo>() {
                new PdfCellInfo("Gráfico"),
                new PdfCellInfo("Grupos"),
                new PdfCellInfo("Turno"),
                new PdfCellInfo("Inicio"),
                new PdfCellInfo("Final"),
                new PdfCellInfo("I.Partido"),
                new PdfCellInfo("F.Partido"),
                new PdfCellInfo("Valoración"),
                new PdfCellInfo("Trabajadas"),
                new PdfCellInfo("Acumuladas"),
                new PdfCellInfo("Nocturnas"),
                new PdfCellInfo("Desayuno"),
                new PdfCellInfo("Comida"),
                new PdfCellInfo("Cena"),
                new PdfCellInfo("Plus Cena"),
            };
            // Añadimos los datos.
            tabla.Data = datos;
            // Devolvemos la tabla.
            return tabla;
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region COMANDOS
        // ====================================================================================================

        #region COMPARAR
        // Comando
        private ICommand cmdComparar;
        public ICommand CmdComparar {
            get {
                if (cmdComparar == null) cmdComparar = new RelayCommand(p => Comparar(), p => PuedeComparar());
                return cmdComparar;
            }
        }
        // Puede
        private bool PuedeComparar() {
            return GrupoSeleccionado1 != null && GrupoSeleccionado2 != null && GrupoSeleccionado1 != GrupoSeleccionado2;
        }
        // Ejecución del comando
        private void Comparar() {
            try {
                Lista1 = App.Global.GetRepository(App.Global.CentroActual).GetGraficos(GrupoSeleccionado1.Validez);
                Lista2 = App.Global.GetRepository(App.Global.CentroActual).GetGraficos(GrupoSeleccionado2.Validez);
                if (Lista1 == null || Lista2 == null) return;
                var graficos = Lista1.Select(g => g.Numero).Union(Lista2.Select(gg => gg.Numero)).OrderBy(n => n);
                if (graficos == null || graficos.Count() == 0) return;
                var listaDiferencias = new List<Tuple<int, string>>();
                foreach (var numero in graficos) {
                    //int numero = grafico;
                    string diferencias = string.Empty;
                    // Comprobamos que el gráfico esté en las dos listas.
                    var g1 = Lista1.FirstOrDefault(g => g.Numero == numero);
                    var g2 = Lista2.FirstOrDefault(g => g.Numero == numero);
                    if (g1 == null) diferencias += $"No está en grupo <{GrupoSeleccionado1.Validez:dd-MM-yyyy}>.\n";
                    if (g2 == null) diferencias += $"No está en grupo <{GrupoSeleccionado2.Validez:dd-MM-yyyy}>.\n";
                    // Si está en los dos grupos, se compara.
                    if (g1 != null && g2 != null) {
                        if (g1.Turno != g2.Turno) diferencias += $"Turno diferente.\n";
                        if (g1.Inicio != g2.Inicio) diferencias += $"Inicio diferente.\n";
                        if (g1.Final != g2.Final) diferencias += $"Final diferente.\n";
                        if (g1.InicioPartido != g2.InicioPartido) diferencias += $"Inicio partido diferente.\n";
                        if (g1.FinalPartido != g2.FinalPartido) diferencias += $"Final partido diferente.\n";
                        if (g1.Valoracion != g2.Valoracion) diferencias += $"Valoración diferente.\n";
                    }
                    if (!string.IsNullOrEmpty(diferencias)) {
                        diferencias = diferencias.Substring(0, diferencias.Length - 1);
                        listaDiferencias.Add(new Tuple<int, string>(numero, diferencias));
                    }
                }
                GraficosComparados = listaDiferencias;
                TableData = null;

            } catch (System.Exception ex) {
                App.Global.Mensajes.VerError("CompararGraficosPageVM.Comparar", ex);
                return;
            }
        }


        #endregion



        #region GENERAR PDF 

        // Comando
        private ICommand cmdGenerarPdf;
        public ICommand CmdGenerarPdf {
            get {
                if (cmdGenerarPdf == null) cmdGenerarPdf = new RelayCommand(p => GenerarPdf(), p => PuedeGenerarPdf());
                return cmdGenerarPdf;
            }
        }
        // Se puede ejecutar el comando
        private bool PuedeGenerarPdf() {
            return Lista1?.Any() == true && Lista2?.Any() == true;
        }
        // Ejecución del comando
        private void GenerarPdf() {
            // Si no se ha generado la tabla antes, se genera.
            if (TableData == null) TableData = GenerarPdfTableData();
            // Generamos un PDF
            var ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, $"Graficos\\Comparaciones");
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            var archivo = Path.Combine(ruta, $"{App.Global.CentroActual}-{GrupoSeleccionado1.Validez:dd-MM-yyyy} - {GrupoSeleccionado2.Validez:dd-MM-yyyy}.pdf");
            var docPdf = App.Global.Informes.GetNuevoPdf(archivo, true);
            docPdf.GetPdfDocument().GetDocumentInfo().SetTitle("Comparación Gráficos");
            docPdf.GetPdfDocument().GetDocumentInfo().SetSubject($"Comparación Gráficos {GrupoSeleccionado1.Validez:dd-MM-yyyy} - {GrupoSeleccionado2.Validez:dd-MM-yyyy}");
            docPdf.SetMargins(25, 25, 25, 25);
            docPdf.Add(PdfExcelHelper.GetInstance().GetPdfTable(TableData));
            docPdf.Close();
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(archivo);
        }
        #endregion



        #region GENERAR EXCEL 

        // Comando
        private ICommand cmdGenerarExcel;
        public ICommand CmdGenerarExcel {
            get {
                if (cmdGenerarExcel == null) cmdGenerarExcel = new RelayCommand(p => GenerarExcel(), p => PuedeGenerarExcel());
                return cmdGenerarExcel;
            }
        }
        // Se puede ejecutar el comando
        private bool PuedeGenerarExcel() {
            return Lista1?.Any() == true && Lista2?.Any() == true;
        }
        // Ejecución del comando
        private void GenerarExcel() {
            // Si no se ha generado la tabla antes, se genera.
            if (TableData == null) TableData = GenerarPdfTableData();
            // Generamos el Excel
            var ruta = Path.Combine(App.Global.Configuracion.CarpetaInformes, $"Graficos\\Comparaciones");
            if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);
            var archivo = Path.Combine(ruta, $"{App.Global.CentroActual}-{GrupoSeleccionado1.Validez:dd-MM-yyyy} - {GrupoSeleccionado2.Validez:dd-MM-yyyy}.xlsx");
            PdfExcelHelper.GetInstance().SaveAsExcel(TableData, archivo, "Comparación");
            if (App.Global.Configuracion.AbrirPDFs) Process.Start(archivo);
        }
        #endregion


        #endregion
        // ====================================================================================================


    }
}
