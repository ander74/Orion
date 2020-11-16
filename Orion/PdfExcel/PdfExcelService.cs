#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Orion.Convertidores;
using static Orion.Servicios.ExcelService;

namespace Orion.PdfExcel {
    public class PdfExcelService {

        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static PdfExcelService instance;

        private PdfExcelService() { }

        public static PdfExcelService GetInstance() {
            if (instance == null) instance = new PdfExcelService();
            return instance;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CALENDARIOS
        // ====================================================================================================

        public PdfTableData GetComparacionCorreccionesConductor(int matricula, List<HorasConductor> pijama, List<HorasConductor> horas) {
            var datos = new List<List<PdfCellInfo>>();
            var cnvNumGraf = new ConvertidorNumeroGrafico();
            foreach (var dia in pijama) {
                var celdas1 = new List<PdfCellInfo>();
                var celdas2 = new List<PdfCellInfo>();
                var diferente = false;
                var hora = horas.FirstOrDefault(h => h.Dia.Day == dia.Dia.Day);
                if (hora == null) continue;
                // DÍA
                // Celdas1 se inserta al final poner el color si hay diferencias.
                celdas2.Add(new PdfCellInfo() { IsMerged = true });
                // ORIGEN
                celdas1.Add(new PdfCellInfo("Orión"));
                celdas2.Add(new PdfCellInfo("Conductor"));
                // GRÁFICO
                celdas1.Add(new PdfCellInfo($"{cnvNumGraf.Convert(dia.Grafico, typeof(string), null, null)}") {
                    TextFontColor = dia.Grafico == hora.Grafico ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo($"{cnvNumGraf.Convert(hora.Grafico, typeof(string), null, null)}") {
                    TextFontColor = dia.Grafico == hora.Grafico ? Colors.DimGray : Colors.Red,
                });
                if (dia.Grafico != hora.Grafico) diferente = true;
                // TRABAJADAS
                celdas1.Add(new PdfCellInfo($"{dia.Trabajadas.ToTexto()}") {
                    TextFontColor = dia.Trabajadas.Ticks == hora.Trabajadas.Ticks ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo($"{hora.Trabajadas.ToTexto()}") {
                    TextFontColor = dia.Trabajadas.Ticks == hora.Trabajadas.Ticks ? Colors.DimGray : Colors.Red,
                });
                if (dia.Trabajadas.Ticks != hora.Trabajadas.Ticks) diferente = true;
                // ACUMULADAS
                celdas1.Add(new PdfCellInfo($"{dia.Acumuladas.ToTexto()}") {
                    TextFontColor = dia.Acumuladas.Ticks == hora.Acumuladas.Ticks ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo($"{hora.Acumuladas.ToTexto()}") {
                    TextFontColor = dia.Acumuladas.Ticks == hora.Acumuladas.Ticks ? Colors.DimGray : Colors.Red,
                });
                if (dia.Acumuladas.Ticks != hora.Acumuladas.Ticks) diferente = true;
                // DESAYUNO
                celdas1.Add(new PdfCellInfo(dia.Desayuno.ToString("0.00")) {
                    TextFontColor = dia.Desayuno == hora.Desayuno ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo(hora.Desayuno.ToString("0.00")) {
                    TextFontColor = dia.Desayuno == hora.Desayuno ? Colors.DimGray : Colors.Red,
                });
                if (dia.Desayuno != hora.Desayuno) diferente = true;
                // COMIDA
                celdas1.Add(new PdfCellInfo(dia.Comida.ToString("0.00")) {
                    TextFontColor = dia.Comida == hora.Comida ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo(hora.Comida.ToString("0.00")) {
                    TextFontColor = dia.Comida == hora.Comida ? Colors.DimGray : Colors.Red,
                });
                if (dia.Comida != hora.Comida) diferente = true;
                // CENA
                celdas1.Add(new PdfCellInfo(dia.Cena.ToString("0.00")) {
                    TextFontColor = dia.Cena == hora.Cena ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo(hora.Cena.ToString("0.00")) {
                    TextFontColor = dia.Cena == hora.Cena ? Colors.DimGray : Colors.Red,
                });
                if (dia.Cena != hora.Cena) diferente = true;
                // PLUS CENA
                celdas1.Add(new PdfCellInfo(dia.PlusCena.ToString("0.00")) {
                    TextFontColor = dia.PlusCena == hora.PlusCena ? Colors.DimGray : Colors.Red,
                });
                celdas2.Add(new PdfCellInfo(hora.PlusCena.ToString("0.00")) {
                    TextFontColor = dia.PlusCena == hora.PlusCena ? Colors.DimGray : Colors.Red,
                });
                if (dia.PlusCena != hora.PlusCena) diferente = true;
                // DIA
                celdas1.Insert(0, new PdfCellInfo(dia.Dia.Day.ToString("00")) {
                    MergedCells = (2, 1),
                    IsBold = true,
                    TextFontColor = diferente ? Colors.Red : Colors.DimGray,
                    TextFontSize = 12,
                });
                // Añadimos las dos filas a la lista
                datos.Add(celdas1);
                datos.Add(celdas2);
            }
            // Preparamos el objeto PdfTableData
            var titulo = $"COMPARACIÓN CALENDARIOS\n" +
                $"CONDUCTOR: {matricula:000} - {pijama[0].Dia.ToString("MMMM-yyyy").ToUpper()}";
            var tabla = new PdfTableData(titulo);
            tabla.SetUniformWidths(9);
            tabla.AlternateRowsCount = 2;
            tabla.TableTheme = PdfTableTheme.AZUL_OSCURO;
            tabla.LogoPath = App.Global.Configuracion.RutaLogoSindicato;
            // Creamos los encabezados.
            tabla.Headers = new List<PdfCellInfo>() {
                new PdfCellInfo("DÍAS"),
                new PdfCellInfo("ORIGEN"),
                new PdfCellInfo("GRÁFICOS"),
                new PdfCellInfo("TRABAJADAS"),
                new PdfCellInfo("ACUMULADAS"),
                new PdfCellInfo("DESAYUNO"),
                new PdfCellInfo("COMIDA"),
                new PdfCellInfo("CENA"),
                new PdfCellInfo("PLUS CENA"),
            };
            // Añadimos los datos.
            tabla.Data = datos;
            // Devolvemos la tabla.
            return tabla;

        }

        #endregion
        // ====================================================================================================


    }
}
