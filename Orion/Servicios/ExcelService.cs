#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Orion.Config;
using Orion.Convertidores;
using Orion.Models;

namespace Orion.Servicios {

    public class ExcelService {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private static ExcelService instance;

        private static string[] meses = { "Desconocido", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================


        private ExcelService() {

        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        public static ExcelService getInstance() {
            if (instance == null) instance = new ExcelService();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================


        /// <summary>
        /// Extrae los gráficos del excel proporcionado con el acumula y las dietas.
        /// La hoja debe llamarse 'graficos'
        /// </summary>
        private List<GraficoBase> getGraficosDietas(string excelFile) {
            var fileInfo = new FileInfo(excelFile);
            var lista = new List<GraficoBase>();
            using (var excel = new ExcelPackage(fileInfo)) {
                var hoja = excel.Workbook.Worksheets.FirstOrDefault(h => h.Name == "graficos");
                if (hoja != null && hoja.Dimension != null) {
                    for (int fila = 2; fila <= hoja.Dimension.End.Row; fila++) {
                        var grafico = new GraficoBase();
                        grafico.Numero = hoja.Cells[fila, 1].GetValue<int>();
                        var valoracion = hoja.Cells[fila, 2].GetValue<DateTime>();
                        grafico.Valoracion = new TimeSpan(valoracion.Hour, valoracion.Minute, 0);
                        var acumula = hoja.Cells[fila, 3].GetValue<decimal>();
                        int horas = (int)Math.Abs(acumula);
                        decimal minutosAntes = acumula - horas;
                        decimal minutos = minutosAntes * 60;
                        if (minutos < 0) minutos = -minutos;
                        grafico.Acumuladas = new TimeSpan(horas, (int)minutos, 0);
                        grafico.Desayuno = hoja.Cells[fila, 4].GetValue<decimal>();
                        grafico.Comida = hoja.Cells[fila, 5].GetValue<decimal>();
                        grafico.Cena = hoja.Cells[fila, 6].GetValue<decimal>();
                        lista.Add(grafico);
                    }
                }
            }
            return lista;
        }

        private string parseDiaSemana(string tipoDia) {
            switch (tipoDia) {
                case "L":
                    return "Lunes a Jueves";
                case "V":
                    return "Viernes";
                case "S":
                    return "Sábados";
                case "F":
                    return "Festivos";
            }
            return "Desconocido";
        }


        private List<Calendario> getCalendarios(string excelFile, int año, int mes) {

            var fileInfo = new FileInfo(excelFile);
            int diasMes = DateTime.DaysInMonth(año, mes);
            List<Calendario> lista = new List<Calendario>();
            ConvertidorNumeroGraficoCalendario converter = new ConvertidorNumeroGraficoCalendario();

            using (var excel = new ExcelPackage(fileInfo)) {
                if (excel.Workbook.Worksheets.Any(h => h.Name == meses[mes])) {
                    var hoja = excel.Workbook.Worksheets[meses[mes]];
                    for (int fila = 2; fila <= hoja.Dimension.End.Row; fila++) {
                        Calendario calendario = new Calendario();
                        calendario.Fecha = new DateTime(año, mes, 1);
                        calendario.MatriculaConductor = hoja.Cells[fila, 1].GetValue<int>();
                        calendario.ListaDias = new NotifyCollection<DiaCalendario>();
                        for (int dia = 1; dia <= diasMes; dia++) {
                            DiaCalendario diaCalendario = new DiaCalendario();
                            diaCalendario.Dia = dia;
                            var valor = hoja.Cells[fila, 1 + dia].GetValue<string>() ?? "";
                            diaCalendario.ComboGrafico = (Tuple<int, int>)converter.ConvertBack(valor, null, null, null);
                            calendario.ListaDias.Add(diaCalendario);
                        }
                        lista.Add(calendario);
                    }
                }
            }
            return lista;
        }


        private void rellenarHojaComparacionCalendarios(ref ExcelWorksheet hoja, IEnumerable<Calendario> listaCalendarios, IEnumerable<Calendario> listaExcel, int año, int mes) {

            int diasMes = DateTime.DaysInMonth(año, mes);
            ConvertidorNumeroGraficoCalendario converter = new ConvertidorNumeroGraficoCalendario();

            // Título y Leyenda
            hoja.Cells["A1"].Value = $"Comparación Calendarios ({meses[mes]} - {año})";
            hoja.Cells["A1"].Style.Font.Size = 20;
            hoja.Cells["L1:U1"].Merge = true;
            hoja.Cells["L1"].Style.Font.Size = 10;
            hoja.Cells["L1"].Style.WrapText = true;
            hoja.Cells["L1"].Style.Indent = 1;
            string texto1 = "Fila superior";
            string texto2 = " = Calendarios en Orión.\r\n";
            string texto3 = "Fila inferior";
            string texto4 = " = Calendarios en el excel.";
            hoja.Cells["L1"].IsRichText = true;
            hoja.Cells["L1"].RichText.Add(texto1).Bold = true;
            hoja.Cells["L1"].RichText.Add(texto2).Bold = false;
            hoja.Cells["L1"].RichText.Add(texto3).Bold = true;
            hoja.Cells["L1"].RichText.Add(texto4).Bold = false;

            // Encabezados.
            hoja.Cells["A3"].Value = "Nombre";
            hoja.Cells["B3"].Value = "Matrícula";
            for (int dia = 1; dia <= diasMes; dia++) {
                hoja.Cells[3, dia + 2].Value = dia;
            }

            // Recorremos los gráficos.
            int filas = 0;
            int fila = 1;

            foreach (var calendario in listaCalendarios) {
                var calendarioExcel = listaExcel.FirstOrDefault(c => c.MatriculaConductor == calendario.MatriculaConductor);
                if (calendarioExcel == null) continue;
                Conductor conductor = App.Global.ConductoresVM.GetConductor(calendario.MatriculaConductor);
                string nombre = conductor == null ? "Desconocido" : $"{conductor.Nombre} {conductor.Apellidos}";
                hoja.Cells[fila + 3, 1, fila + 4, 1].Merge = true;
                hoja.Cells[fila + 3, 2, fila + 4, 2].Merge = true;
                hoja.Cells[fila + 3, 1].Value = nombre;
                hoja.Cells[fila + 3, 2].Value = calendario.MatriculaConductor;
                for (int dia = 1; dia <= diasMes; dia++) {
                    //string txtOrion = (string)converter.Convert(calendario.ListaDias[dia - 1].ComboGrafico, null, null, null);
                    string txtOrion = (string)converter.Convert(calendario.ListaDias.First(d => d.Dia == dia).ComboGrafico, null, null, null);
                    string txtExcel = (string)converter.Convert(calendarioExcel.ListaDias[dia - 1].ComboGrafico, null, null, null);
                    if (int.TryParse(txtOrion, out int intOrion)) {
                        hoja.Cells[fila + 3, dia + 2].Value = intOrion;
                    } else {
                        hoja.Cells[fila + 3, dia + 2].Value = txtOrion;
                    }
                    if (int.TryParse(txtExcel, out int intExcel)) {
                        hoja.Cells[fila + 4, dia + 2].Value = intExcel;
                    } else {
                        hoja.Cells[fila + 4, dia + 2].Value = txtExcel;
                    }
                    if (!txtOrion.ToLower().Equals(txtExcel.ToLower())) {
                        hoja.Cells[fila + 3, dia + 2, fila + 4, dia + 2].Style.Font.Color.SetColor(Color.Red);
                    }
                }
                hoja.Cells[fila + 3, 1, fila + 4, diasMes + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (filas % 2 == 0) {
                    hoja.Cells[fila + 3, 1, fila + 4, diasMes + 2].Style.Fill.BackgroundColor.SetColor(Color.White);
                } else {
                    hoja.Cells[fila + 3, 1, fila + 4, diasMes + 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(226, 239, 218));
                }


                filas++;
                fila = fila + 2;
            }

            // Formato Encabezados
            hoja.Cells[3, 1, 3, diasMes + 2].Style.Font.Color.SetColor(Color.White);
            hoja.Cells[3, 1, 3, diasMes + 2].Style.Font.Bold = true;
            hoja.Cells[3, 1, 3, diasMes + 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            hoja.Cells[3, 1, 3, diasMes + 2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(84, 130, 53));

            // Formato global
            hoja.Cells[3, 1, 3 + (filas * 2), diasMes + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            hoja.Cells[3, 1, 3 + (filas * 2), diasMes + 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            hoja.Row(1).Height = 35;
            hoja.Row(3).Height = 17;
            for (int i = 3; i <= filas * 2 + 3; i++) hoja.Row(i).Height = 16;
            hoja.Column(1).Width = 25;
            hoja.Column(2).Width = 10;
            for (int i = 1; i <= diasMes; i++) hoja.Column(i + 2).Width = 6;

            // Bordes
            hoja.Cells[3, 1, 3 + filas * 2, 2 + diasMes].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            hoja.Cells[3, 1, 3 + filas * 2, 2 + diasMes].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            hoja.Cells[3, 1, 3 + filas * 2, 2 + diasMes].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            hoja.Cells[3, 1, 3 + filas * 2, 2 + diasMes].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            hoja.Cells[3, 1, 3 + filas * 2, 2 + diasMes].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
            hoja.Cells[3, 1, 3, 2 + diasMes].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);


        }




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Genera un excel con la comparación de los gráficos de dos archivos.
        /// </summary>
        /// <param name="destino"></param>
        /// <param name="excelGraficosSimples"></param>
        /// <param name="excelGraficosDietas"></param>
        public void GenerarComparacionGraficos(string destino, IEnumerable<GraficoBase> listaGraficos, string excelGraficosDietas) {
            // Cargamos los gráficos del excel
            var graficosDietas = getGraficosDietas(excelGraficosDietas);

            using (var excelApp = new ExcelPackage()) {
                var hoja = excelApp.Workbook.Worksheets.Add($"Comparacion Dietas");

                // Título y Leyenda
                hoja.Cells["A1"].Value = $"COMPARACIÓN DIETAS";
                hoja.Cells["A1"].Style.Font.Size = 20;
                hoja.Cells["F1:P1"].Merge = true;
                hoja.Cells["F1"].Style.Font.Size = 10;
                hoja.Cells["F1"].Style.WrapText = true;
                hoja.Cells["F1"].Style.Indent = 1;
                string texto1 = "Real";
                string texto2 = " = Calculado usando el inicio y final.\r\n";
                string texto3 = "Empresa";
                string texto4 = " = Datos del informe de la empresa.";
                hoja.Cells["F1"].IsRichText = true;
                hoja.Cells["F1"].RichText.Add(texto1).Bold = true;
                hoja.Cells["F1"].RichText.Add(texto2).Bold = false;
                hoja.Cells["F1"].RichText.Add(texto3).Bold = true;
                hoja.Cells["F1"].RichText.Add(texto4).Bold = false;

                // Encabezados.
                hoja.Cells["A3"].Value = "DATOS GRÁFICO";
                hoja.Cells["A3:E3"].Merge = true;
                hoja.Cells["F3"].Value = "VALORACION";
                hoja.Cells["F3:H3"].Merge = true;
                hoja.Cells["I3"].Value = "ACUMULA";
                hoja.Cells["I3:J3"].Merge = true;
                hoja.Cells["K3"].Value = "DESAYUNO";
                hoja.Cells["K3:L3"].Merge = true;
                hoja.Cells["M3"].Value = "COMIDA";
                hoja.Cells["M3:N3"].Merge = true;
                hoja.Cells["O3"].Value = "CENA";
                hoja.Cells["O3:P3"].Merge = true;
                hoja.Cells["A4"].Value = "Centro";
                hoja.Cells["B4"].Value = "Tipo Día";
                hoja.Cells["C4"].Value = "Gráfico";
                hoja.Cells["D4"].Value = "Inicio";
                hoja.Cells["E4"].Value = "Final";
                hoja.Cells["F4"].Value = "Real";
                hoja.Cells["G4"].Value = "Empresa";
                hoja.Cells["I4"].Value = "Real";
                hoja.Cells["H4"].Value = "Diff.";
                hoja.Cells["J4"].Value = "Empresa";
                hoja.Cells["K4"].Value = "Real";
                hoja.Cells["L4"].Value = "Empresa";
                hoja.Cells["M4"].Value = "Real";
                hoja.Cells["N4"].Value = "Empresa";
                hoja.Cells["O4"].Value = "Real";
                hoja.Cells["P4"].Value = "Empresa";


                // Recorremos los gráficos.
                int filas = 0;

                foreach (var grafico in listaGraficos) {
                    grafico.Recalcular();
                    hoja.Cells[filas + 5, 1].Value = App.Global.CentroActual.ToString();
                    hoja.Cells[filas + 5, 2].Value = parseDiaSemana(grafico.DiaSemana);
                    hoja.Cells[filas + 5, 3].Value = grafico.Numero;
                    hoja.Cells[filas + 5, 4].Value = grafico.Inicio;
                    hoja.Cells[filas + 5, 5].Value = grafico.Final;
                    GraficoBase gr = graficosDietas.FirstOrDefault(g => g.Numero == grafico.Numero);
                    if (gr != null) {
                        hoja.Cells[filas + 5, 7].Value = gr.Valoracion;
                        hoja.Cells[filas + 5, 10].Value = gr.Acumuladas.ToDecimal();
                        hoja.Cells[filas + 5, 12].Value = gr.Desayuno;
                        hoja.Cells[filas + 5, 14].Value = gr.Comida;
                        hoja.Cells[filas + 5, 16].Value = gr.Cena;

                        TimeSpan diferencia = gr.Valoracion - grafico.TrabajadasConvenio;
                        if (diferencia.Ticks < 0) diferencia = -diferencia;

                        hoja.Cells[filas + 5, 6].Value = grafico.TrabajadasConvenio;
                        hoja.Cells[filas + 5, 8].Value = diferencia;
                        hoja.Cells[filas + 5, 9].Value = grafico.Acumuladas.ToDecimal();
                        hoja.Cells[filas + 5, 11].Value = grafico.Desayuno;
                        hoja.Cells[filas + 5, 13].Value = grafico.Comida;
                        hoja.Cells[filas + 5, 15].Value = grafico.Cena + grafico.PlusCena;

                        // Colores Valoracion
                        if (gr.Valoracion.Ticks != grafico.TrabajadasConvenio.Ticks) hoja.Cells[filas + 5, 6, filas + 5, 8].Style.Font.Color.SetColor(Color.Red);
                        // Colores Acumula
                        if (Math.Round(gr.Acumuladas.ToDecimal(), 2) != Math.Round(grafico.Acumuladas.ToDecimal(), 2)) hoja.Cells[filas + 5, 9, filas + 5, 10].Style.Font.Color.SetColor(Color.Red);
                        // Colores Desayuno
                        if (Math.Round(gr.Desayuno, 2) != Math.Round(grafico.Desayuno, 2)) hoja.Cells[filas + 5, 11, filas + 5, 12].Style.Font.Color.SetColor(Color.Red);
                        // Colores Comida
                        if (Math.Round(gr.Comida, 2) != Math.Round(grafico.Comida, 2)) hoja.Cells[filas + 5, 13, filas + 5, 14].Style.Font.Color.SetColor(Color.Red);
                        // Colores Cena
                        if (Math.Round(gr.Cena, 2) != Math.Round(grafico.Cena + grafico.PlusCena, 2)) hoja.Cells[filas + 5, 15, filas + 5, 16].Style.Font.Color.SetColor(Color.Red);
                    }


                    filas++;
                }

                // Formato Encabezados
                hoja.Cells["A3:P4"].Style.Font.Color.SetColor(Color.White);
                hoja.Cells["A3:P4"].Style.Font.Bold = true;
                hoja.Cells["A3:E4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["A3:E4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(123, 123, 123));
                hoja.Cells["F3:H4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["F3:H4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 143, 0));
                hoja.Cells["I3:J4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["I3:J4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(198, 89, 17));
                hoja.Cells["K3:L4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["K3:L4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(47, 117, 181));
                hoja.Cells["M3:N4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["M3:N4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(84, 130, 53));
                hoja.Cells["O3:P4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                hoja.Cells["O3:P4"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));

                // Formato Datos
                hoja.Cells[$"D5:H{5 + filas}"].Style.Numberformat.Format = "hh:mm";
                hoja.Cells[$"I5:O{5 + filas}"].Style.Numberformat.Format = "0.00";

                // Formato global
                hoja.Cells[$"A3:p{5 + filas}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                for (int i = 1; i <= 15; i++) hoja.Column(i).Width = i > 2 ? 10 : 14;
                hoja.Cells[$"A1:p{5 + filas}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                hoja.Row(1).Height = 35;
                for (int i = 3; i <= 5 + filas; i++) hoja.Row(i).Height = i > 4 ? 16 : 17;

                // Bordes
                hoja.Cells[$"A3:P{5 + filas}"].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                hoja.Cells[$"A3:P{5 + filas}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                hoja.Cells[$"A3:P{5 + filas}"].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                hoja.Cells[$"A3:P{5 + filas}"].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                hoja.Cells[$"A3:P{5 + filas}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                hoja.Cells[$"A3:P4"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

                // Guardamos el libro y cerramos la aplicación.
                excelApp.SaveAs(new FileInfo(destino));


            }
        }


        public void GenerarComparacionCalendarios(string destino, IEnumerable<Calendario> listaCalendarios, string excelCalendarios, int año, int mes) {
            var listaExcel = getCalendarios(excelCalendarios, año, mes);

            using (var excelApp = new ExcelPackage()) {
                // Creamos la hoja del mes
                var hoja = excelApp.Workbook.Worksheets.Add($"{meses[mes]}");
                // Rellenamos la hoja
                rellenarHojaComparacionCalendarios(ref hoja, listaCalendarios, listaExcel, año, mes);
                // Guardamos el libro y cerramos la aplicación.
                excelApp.SaveAs(new FileInfo(destino));
            }
        }


        public void GenerarComparacionCalendariosAnual(string destino, string excelCalendarios, int año) {

            using (var excelApp = new ExcelPackage()) {
                for (int mes = 1; mes <= 12; mes++) {
                    var listaCalendarios = App.Global.Repository.GetCalendarios(año, mes);
                    var listaExcel = getCalendarios(excelCalendarios, año, mes);
                    // Creamos la hoja del mes
                    var hoja = excelApp.Workbook.Worksheets.Add($"{meses[mes]}");
                    // Rellenamos la hoja
                    rellenarHojaComparacionCalendarios(ref hoja, listaCalendarios, listaExcel, año, mes);
                }

                // Guardamos el libro y cerramos la aplicación.
                excelApp.SaveAs(new FileInfo(destino));
            }
        }


        public void GenerarEstadisticasCalendario(string destino, IEnumerable<EstadisticaPorTurnos> listaEstadisticas, string titulo) {
            // Variables que se van a utilizar.
            var centroActual = App.Global.CentroActual.ToString();
            // Colores de encabezado rotativos.
            var listaColores = new List<Color> {
                Color.FromArgb(191, 143, 0),
                Color.FromArgb(198, 89, 17),
                Color.FromArgb(84, 130, 53),
                Color.FromArgb(47, 117, 181),
                // Se repiten, pero podrían cambiarse.
                Color.FromArgb(191, 143, 0),
                Color.FromArgb(198, 89, 17),
                Color.FromArgb(84, 130, 53),
                Color.FromArgb(47, 117, 181),
            };
            var columnaActual = 4;
            var filaActual = 6;
            var colorActual = 0;
            var mostrarDiasTrabajados = App.Global.EstadisticasTurnosVM.MostrarDiasTrabajados;
            var mostrarHorasTrabajadas = App.Global.EstadisticasTurnosVM.MostrarHorasTrabajadas;
            var mostrarHorasAcumuladas = App.Global.EstadisticasTurnosVM.MostrarHorasAcumuladas;
            var mostrarHorasNocturnas = App.Global.EstadisticasTurnosVM.MostrarHorasNocturnas;
            var mostrarImporteDietas = App.Global.EstadisticasTurnosVM.MostrarImporteDietas;
            var mostrarFestivosTrabajados = App.Global.EstadisticasTurnosVM.MostrarFestivosTrabajados;
            var mostrarDescansos = App.Global.EstadisticasTurnosVM.MostrarDescansos;
            var mostrarPluses = App.Global.EstadisticasTurnosVM.MostrarPluses;


            using (var excelApp = new ExcelPackage()) {
                // Añadimos una hoja con las estadísticas.
                var hoja = excelApp.Workbook.Worksheets.Add($"Estadisticas Calendarios");
                // Título
                hoja.Cells["A1"].Value = $"ESTADÍSTICAS CALENDARIOS {centroActual.ToUpper()}";
                hoja.Cells["A1"].Style.Font.Size = 20;
                hoja.Cells["A2"].Value = $"{titulo}";
                hoja.Cells["A2"].Style.Font.Size = 16;

                // Encabezados.
                hoja.Cells["A4"].Value = "CONDUCTOR";
                hoja.Cells["A4:C4"].Merge = true;
                hoja.Cells["A5"].Value = "Centro";
                hoja.Cells["B5"].Value = "Matrícula";
                hoja.Cells["C5"].Value = "Apellidos";
                hoja.Cells["A4:C5"].Style.Font.Color.SetColor(Color.White);
                hoja.Cells["A4:C5"].Style.Font.Bold = true;
                hoja.Cells["A4:C5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                hoja.Cells["A4:C5"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(123, 123, 123));
                hoja.Cells["A4:C5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                hoja.Cells["A4:C5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                hoja.Cells["A4:C5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                hoja.Cells["A4:C5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                hoja.Cells["A4:C5"].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                // Encabezados Días Trabajados
                if (mostrarDiasTrabajados) {
                    hoja.Cells[4, columnaActual].Value = "DÍAS TRABAJADOS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Días 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Días 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Días 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Días 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Días";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Horas Trabajadas
                if (mostrarHorasTrabajadas) {
                    hoja.Cells[4, columnaActual].Value = "HORAS TRABAJADAS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Trab. 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Trab. 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Trab. 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Trab. 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Trab.";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Horas Acumuladas
                if (mostrarHorasAcumuladas) {
                    hoja.Cells[4, columnaActual].Value = "HORAS ACUMULADAS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Acum. 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Acum. 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Acum. 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Acum. 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Acum.";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Horas Nocturnas
                if (mostrarHorasNocturnas) {
                    hoja.Cells[4, columnaActual].Value = "HORAS NOCTURNAS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Noct. 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Noct. 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Noct. 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Noct. 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Noct";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Importe Dietas
                if (mostrarImporteDietas) {
                    hoja.Cells[4, columnaActual].Value = "IMPORTE DIETAS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Imp. 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Imp. 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Imp. 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Imp. 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Imp.";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Festivos Trabajados
                if (mostrarFestivosTrabajados) {
                    hoja.Cells[4, columnaActual].Value = "FESTIVOS TRABAJADOS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 4].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Fest. 1";
                    hoja.Cells[5, columnaActual + 1].Value = "Fest. 2";
                    hoja.Cells[5, columnaActual + 2].Value = "Fest. 3";
                    hoja.Cells[5, columnaActual + 3].Value = "Fest. 4";
                    hoja.Cells[5, columnaActual + 4].Value = "Total Fest.";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                    colorActual++;
                }
                // Encabezados Descansos
                if (mostrarDescansos) {
                    hoja.Cells[4, columnaActual].Value = "DESCANSOS";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 1].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Días";
                    hoja.Cells[5, columnaActual + 1].Value = "Findes";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 2;
                    colorActual++;
                }
                // Encabezados Pluses
                if (mostrarPluses) {
                    hoja.Cells[4, columnaActual].Value = "PLUSES";
                    hoja.Cells[4, columnaActual, 4, columnaActual + 1].Merge = true;
                    hoja.Cells[5, columnaActual].Value = "Paquet.";
                    hoja.Cells[5, columnaActual + 1].Value = "M.Desc";
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Font.Bold = true;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    hoja.Cells[4, columnaActual, 5, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 2;
                    colorActual++;
                }

                // DATOS
                foreach (var estadistica in listaEstadisticas) {
                    // Si el conductor es matricula cero, saltarselo.
                    if (estadistica.Conductor.Matricula == 0) continue;

                    hoja.Cells[filaActual, 1].Value = centroActual;
                    hoja.Cells[filaActual, 2].Value = estadistica.Conductor.Matricula;
                    hoja.Cells[filaActual, 3].Value = estadistica.Conductor.Apellidos;

                    columnaActual = 4;
                    //Dias Trabajados
                    if (mostrarDiasTrabajados) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.Dias[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.Dias[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.Dias[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.Dias[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = estadistica.Dias.Sum();
                        columnaActual += 5;
                    }
                    //Horas Trabajadas
                    if (mostrarHorasTrabajadas) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.Trabajadas[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.Trabajadas[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.Trabajadas[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.Trabajadas[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(estadistica.Trabajadas.Sum(t => t.Ticks));
                        hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                        columnaActual += 5;
                    }
                    //Horas Acumuladas
                    if (mostrarHorasAcumuladas) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.Acumuladas[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.Acumuladas[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.Acumuladas[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.Acumuladas[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(estadistica.Acumuladas.Sum(t => t.Ticks));
                        hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                        columnaActual += 5;
                    }
                    //Horas Nocturnas
                    if (mostrarHorasNocturnas) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.Nocturnas[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.Nocturnas[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.Nocturnas[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.Nocturnas[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(estadistica.Nocturnas.Sum(t => t.Ticks));
                        hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                        columnaActual += 5;
                    }
                    //Importe Dietas
                    if (mostrarImporteDietas) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.ImporteDietas[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.ImporteDietas[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.ImporteDietas[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.ImporteDietas[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = estadistica.ImporteDietas.Sum();
                        hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "0.00";
                        columnaActual += 5;
                    }
                    //Festivos Trabajados
                    if (mostrarFestivosTrabajados) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.DiasFestivos[1];
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.DiasFestivos[2];
                        hoja.Cells[filaActual, columnaActual + 2].Value = estadistica.DiasFestivos[3];
                        hoja.Cells[filaActual, columnaActual + 3].Value = estadistica.DiasFestivos[4];
                        hoja.Cells[filaActual, columnaActual + 4].Value = estadistica.DiasFestivos.Sum();
                        columnaActual += 5;
                    }
                    //Descansos
                    if (mostrarDescansos) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.DiasDescanso;
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.FindesDescansados;
                        hoja.Cells[filaActual, columnaActual + 1].Style.Numberformat.Format = "0.00";
                        columnaActual += 2;
                    }
                    //Pluses
                    if (mostrarPluses) {
                        hoja.Cells[filaActual, columnaActual].Value = estadistica.PlusPaqueteria;
                        hoja.Cells[filaActual, columnaActual + 1].Value = estadistica.PlusMenorDescanso;
                        hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Numberformat.Format = "0.00";
                        columnaActual += 2;
                    }
                    filaActual++;
                }

                // Bordes Celdas de Datos y Encabezados Juntos
                hoja.Cells[6, 1, filaActual - 1, columnaActual - 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                hoja.Cells[6, 1, filaActual - 1, columnaActual - 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                hoja.Cells[6, 1, filaActual - 1, columnaActual - 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                hoja.Cells[6, 1, filaActual - 1, columnaActual - 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                hoja.Cells[4, 1, 5, columnaActual - 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                hoja.Cells[6, 1, filaActual - 1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                columnaActual = 4;
                if (mostrarDiasTrabajados) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarHorasTrabajadas) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarHorasAcumuladas) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarHorasNocturnas) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarImporteDietas) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarFestivosTrabajados) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 5;
                }
                if (mostrarDescansos) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 2;
                }
                if (mostrarPluses) {
                    hoja.Cells[6, columnaActual, filaActual - 1, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    columnaActual += 2;
                }
                hoja.Cells[6, 1, filaActual - 1, columnaActual - 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                // Configuracion filas y columnas
                hoja.Cells[4, 1, filaActual, columnaActual - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                hoja.Cells[1, 1, filaActual, columnaActual - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                for (int i = 1; i <= 37; i++) hoja.Column(i).Width = i == 3 ? 30 : 12;
                hoja.Row(1).Height = 30;
                hoja.Row(2).Height = 25;
                for (int i = 4; i <= filaActual - 1; i++) hoja.Row(i).Height = i > 5 ? 16 : 17;

                // Creamos una tabla con los datos existentes.
                var rango = hoja.Cells[5, 1, filaActual - 1, columnaActual - 1];
                var tabla = hoja.Tables.Add(rango, "Estadisticas");
                tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium1;

                // Fila Totales
                hoja.Cells[filaActual, 1, filaActual, 3].Merge = true;
                hoja.Cells[filaActual, 1, filaActual, 3].Value = "TOTALES";

                columnaActual = 4;
                //Dias Trabajados
                if (mostrarDiasTrabajados) {
                    hoja.Cells[filaActual, columnaActual].Value = listaEstadisticas.Sum(e => e.Dias[1]);
                    hoja.Cells[filaActual, columnaActual + 1].Value = listaEstadisticas.Sum(e => e.Dias[2]);
                    hoja.Cells[filaActual, columnaActual + 2].Value = listaEstadisticas.Sum(e => e.Dias[3]);
                    hoja.Cells[filaActual, columnaActual + 3].Value = listaEstadisticas.Sum(e => e.Dias[4]);
                    hoja.Cells[filaActual, columnaActual + 4].Value = listaEstadisticas.Sum(e => e.Dias.Sum());
                    columnaActual += 5;
                }

                //Horas Trabajadas
                if (mostrarHorasTrabajadas) {
                    hoja.Cells[filaActual, columnaActual].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Trabajadas[1].Ticks));
                    hoja.Cells[filaActual, columnaActual + 1].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Trabajadas[2].Ticks));
                    hoja.Cells[filaActual, columnaActual + 2].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Trabajadas[3].Ticks));
                    hoja.Cells[filaActual, columnaActual + 3].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Trabajadas[4].Ticks));
                    hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Trabajadas.Sum(t => t.Ticks)));
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                    columnaActual += 5;
                }

                //Horas Acumuladas
                if (mostrarHorasAcumuladas) {
                    hoja.Cells[filaActual, columnaActual].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Acumuladas[1].Ticks));
                    hoja.Cells[filaActual, columnaActual + 1].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Acumuladas[2].Ticks));
                    hoja.Cells[filaActual, columnaActual + 2].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Acumuladas[3].Ticks));
                    hoja.Cells[filaActual, columnaActual + 3].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Acumuladas[4].Ticks));
                    hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Acumuladas.Sum(a => a.Ticks)));
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                    columnaActual += 5;
                }

                //Horas Nocturnas
                if (mostrarHorasNocturnas) {
                    hoja.Cells[filaActual, columnaActual].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Nocturnas[1].Ticks));
                    hoja.Cells[filaActual, columnaActual + 1].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Nocturnas[2].Ticks));
                    hoja.Cells[filaActual, columnaActual + 2].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Nocturnas[3].Ticks));
                    hoja.Cells[filaActual, columnaActual + 3].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Nocturnas[4].Ticks));
                    hoja.Cells[filaActual, columnaActual + 4].Value = new TimeSpan(listaEstadisticas.Sum(e => e.Nocturnas.Sum(a => a.Ticks)));
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "[hh]:mm";
                    columnaActual += 5;
                }

                //Importe Dietas
                if (mostrarImporteDietas) {
                    hoja.Cells[filaActual, columnaActual].Value = listaEstadisticas.Sum(e => e.ImporteDietas[1]);
                    hoja.Cells[filaActual, columnaActual + 1].Value = listaEstadisticas.Sum(e => e.ImporteDietas[2]);
                    hoja.Cells[filaActual, columnaActual + 2].Value = listaEstadisticas.Sum(e => e.ImporteDietas[3]);
                    hoja.Cells[filaActual, columnaActual + 3].Value = listaEstadisticas.Sum(e => e.ImporteDietas[4]);
                    hoja.Cells[filaActual, columnaActual + 4].Value = listaEstadisticas.Sum(e => e.ImporteDietas.Sum());
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Numberformat.Format = "0.00";
                    columnaActual += 5;
                }

                //Festivos Trabajados
                if (mostrarFestivosTrabajados) {
                    hoja.Cells[filaActual, columnaActual].Value = listaEstadisticas.Sum(e => e.DiasFestivos[1]);
                    hoja.Cells[filaActual, columnaActual + 1].Value = listaEstadisticas.Sum(e => e.DiasFestivos[2]);
                    hoja.Cells[filaActual, columnaActual + 2].Value = listaEstadisticas.Sum(e => e.DiasFestivos[3]);
                    hoja.Cells[filaActual, columnaActual + 3].Value = listaEstadisticas.Sum(e => e.DiasFestivos[4]);
                    hoja.Cells[filaActual, columnaActual + 4].Value = listaEstadisticas.Sum(e => e.DiasFestivos.Sum());
                    columnaActual += 5;
                }

                //Descansos
                if (mostrarDescansos) {
                    hoja.Cells[filaActual, columnaActual].Value = listaEstadisticas.Sum(e => e.DiasDescanso);
                    hoja.Cells[filaActual, columnaActual + 1].Value = listaEstadisticas.Sum(e => e.FindesDescansados);
                    hoja.Cells[filaActual, columnaActual + 1].Style.Numberformat.Format = "0.00";
                    columnaActual += 2;
                }

                //Pluses
                if (mostrarPluses) {
                    hoja.Cells[filaActual, columnaActual].Value = listaEstadisticas.Sum(e => e.PlusPaqueteria);
                    hoja.Cells[filaActual, columnaActual + 1].Value = listaEstadisticas.Sum(e => e.PlusMenorDescanso);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Numberformat.Format = "0.00";
                    columnaActual += 2;
                }

                // Formato Fila Total
                hoja.Row(filaActual).Height = 20;
                hoja.Cells[filaActual, 1, filaActual, 3].Style.Font.Color.SetColor(Color.White);
                hoja.Cells[filaActual, 1, filaActual, 3].Style.Font.Bold = true;
                hoja.Cells[filaActual, 1, filaActual, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                hoja.Cells[filaActual, 1, filaActual, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(123, 123, 123));

                hoja.Cells[filaActual, 1, filaActual, columnaActual - 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                hoja.Cells[filaActual, 1, filaActual, columnaActual - 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                hoja.Cells[filaActual, 1, filaActual, columnaActual - 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                hoja.Cells[filaActual, 1, filaActual, columnaActual - 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                hoja.Cells[filaActual, 1, filaActual, 3].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                columnaActual = 4;
                colorActual = 0;
                if (mostrarDiasTrabajados) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarHorasTrabajadas) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarHorasAcumuladas) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarHorasNocturnas) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarImporteDietas) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarFestivosTrabajados) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 4].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 5;
                }
                if (mostrarDescansos) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 2;
                }
                if (mostrarPluses) {
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Font.Color.SetColor(Color.White);
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Font.Bold = true;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hoja.Cells[filaActual, columnaActual, filaActual, columnaActual + 1].Style.Fill.BackgroundColor.SetColor(listaColores[colorActual]);
                    colorActual++;
                    columnaActual += 2;
                }
                hoja.Cells[filaActual, 1, filaActual, columnaActual - 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                // Inmovilizar paneles -- Por el momento no incluirlo.
                //hoja.View.FreezePanes(6, 4);

                // Guardamos el libro y cerramos la aplicación.
                excelApp.SaveAs(new FileInfo(destino));

            }
        }



        #endregion
        // ====================================================================================================




    }
}
