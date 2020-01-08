#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.PrintModel {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Media;
    using Config;
    using Convertidores;
    using iText.Forms;
    using iText.Forms.Fields;
    using iText.Kernel.Colors;
    using iText.Kernel.Font;
    using iText.Kernel.Geom;
    using iText.Kernel.Pdf.Canvas;
    using iText.Layout;
    using iText.Layout.Borders;
    using iText.Layout.Element;
    using iText.Layout.Properties;
    using Models;
    using Servicios;

    public static class PijamaPrintModel {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private static ConvertidorNumeroGraficoPijama cnvNumGraficoPijama = new ConvertidorNumeroGraficoPijama();
        private static ConvertidorHora cnvHora = new ConvertidorHora();
        private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
        private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
        private static ConvertidorDecimalEuro cnvDecimalEuro = new ConvertidorDecimalEuro();
        private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
        private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();
        private static ConvertidorColorDiaPijama cnvColorDiaPijama = new ConvertidorColorDiaPijama();
        private static ConvertidorColorDiaSinGraficos cnvColorDiaSinGraficos = new ConvertidorColorDiaSinGraficos();
        private static ConvertidorColorColumnaPijama cnvColorColumnaPijama = new ConvertidorColorColumnaPijama();

        private static string rutaPlantillaPijama = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Pijama.xlsx");

        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private static Table GetTablaPijama(IEnumerable dias) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            iText.Layout.Style estiloTabla = new iText.Layout.Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(7);
            // Estilo de las celdas de encabezado.
            iText.Layout.Style estiloEncabezados = new iText.Layout.Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(102, 153, 255))
                             .SetBold();
            // Estilo de las celdas de dia.
            iText.Layout.Style estiloDia = new iText.Layout.Style();
            estiloDia.SetBorderLeft(new SolidBorder(1))
                     .SetBorderRight(new SolidBorder(1))
                     .SetBold();
            // Estilo con negrita.
            iText.Layout.Style estiloColumnaPijama = new iText.Layout.Style();
            // Creamos la tabla que tendrá la Hoja Pijama.
            float[] columnas = new float[] { 4f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f, 6.9f };
            Table tabla = new Table(UnitValue.CreatePercentArray(columnas));
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Día")).AddStyle(estiloEncabezados).SetBorder(new SolidBorder(1)));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Gráfico")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Horas")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Ex.Jornada")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Plus Cena")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Menor Des.")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Paquetería")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Otros")).AddStyle(estiloEncabezados));
            // Añadimos las celdas de cada día de la lista.
            foreach (Pijama.DiaPijama dia in dias) {
                if (dia.Dia > DateTime.DaysInMonth(dia.DiaFecha.Year, dia.DiaFecha.Month)) continue;
                // Establecemos el color de fondo de las filas
                iText.Layout.Style estiloCeldas = new iText.Layout.Style().SetBackgroundColor(ColorConstants.WHITE);
                if (dia.Dia % 2 == 0) estiloCeldas.SetBackgroundColor(new DeviceRgb(204, 236, 255));
                // Deducimos los colores de la primera y segunda columnas.
                System.Windows.Media.Color c1 = Colors.Black;
                System.Windows.Media.Color c2 = Colors.Black;
                System.Windows.Media.Color c3 = Colors.Black;
                if (dia.Grafico != 0) {
                    object[] valores2 = new object[] { dia.ComboGrafico, dia.DiaFecha, dia.Dia, dia.TieneHorarioAlternativo };
                    c1 = ((SolidColorBrush)cnvColorDiaSinGraficos.Convert(dia.DiaFecha, null, null, null)).Color;
                    c2 = ((SolidColorBrush)cnvColorDiaPijama.Convert(valores2, null, null, null)).Color;
                }

                // COLUMNA DÍA
                tabla.AddCell(new Cell().Add(new Paragraph(dia.Dia.ToString("00"))
                                        .SetFontColor(new DeviceRgb(c1.R, c1.G, c1.B)))
                                        .AddStyle(estiloDia)
                                        .AddStyle(estiloCeldas));
                // COLUMNA GRÁFICO
                if (dia.TieneHorarioAlternativo) estiloColumnaPijama = new iText.Layout.Style().SetBold();
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvNumGraficoPijama.Convert(dia.ComboGrafico, null, null, null))
                                        .SetFontColor(new DeviceRgb(c2.R, c2.G, c2.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA TRABAJADAS
                if (dia.TrabajadasAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.GraficoTrabajado.Trabajadas, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA ACUMULADAS
                if (dia.AcumuladasAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora
                                        .Convert(dia.GraficoTrabajado.Acumuladas, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA NOCTURNAS
                if (dia.NocturnasAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.GraficoTrabajado.Nocturnas, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA EXCESO JORNADA
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvHora.Convert(dia.ExcesoJornada, null, VerValores.NoCeros, null)))
                                        .AddStyle(estiloCeldas));
                // COLUMNA DESAYUNO
                if (dia.DesayunoAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Desayuno, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA COMIDA
                if (dia.ComidaAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Comida, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA CENA
                if (dia.CenaAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.Cena, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA PLUS CENA
                if (dia.PlusCenaAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.GraficoTrabajado.PlusCena, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA TOTAL DIETAS
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimal.Convert(dia.TotalDietas, null, VerValores.NoCeros, null)))
                                        .AddStyle(estiloCeldas));
                // COLUMNA PLUS MENOR DESCANSO
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusMenorDescanso, null, VerValores.NoCeros, null)))
                                        .AddStyle(estiloCeldas));
                // COLUMNA PLUS PAQUETERÍA
                if (dia.PlusPaqueteriaAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusPaqueteria, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA PLUS LIMPIEZA
                if (dia.PlusLimpiezaAlt.HasValue) {
                    c3 = ((SolidColorBrush)cnvColorColumnaPijama.Convert(true, null, null, null)).Color;
                    estiloColumnaPijama = new iText.Layout.Style().SetBold();
                } else {
                    c3 = Colors.Black;
                    estiloColumnaPijama = new iText.Layout.Style();
                }
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.PlusLimpieza, null, VerValores.NoCeros, null))
                                        .SetFontColor(new DeviceRgb(c3.R, c3.G, c3.B)))
                                        .AddStyle(estiloCeldas)
                                        .AddStyle(estiloColumnaPijama));
                // COLUMNA OTROS PLUSES
                tabla.AddCell(new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(dia.OtrosPluses, null, VerValores.NoCeros, null)))
                                        .AddStyle(estiloCeldas));
            }
            // Añadimos el borde a la tabla y al encabezado
            tabla.SetBorder(new SolidBorder(1));
            tabla.GetHeader().SetBorder(new SolidBorder(1));
            // Devolvemos la tabla.
            return tabla;
        }


        private static Cell GetResumenPijama(Pijama.HojaPijama pijama) {

            // Definimos la celda a devolver.
            Cell celda = new Cell();
            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo Tabla
            iText.Layout.Style estiloTabla = new iText.Layout.Style().SetTextAlignment(TextAlignment.CENTER)
                                                                     .SetMargins(0, 0, 2, 0)
                                                                     .SetPaddings(1.5f, 0, 1.5f, 0)
                                                                     .SetBorder(new SolidBorder(1))
                                                                     .SetFont(arial)
                                                                     .SetWidth(UnitValue.CreatePercentValue(100))
                                                                     .SetFontSize(7);
            // Estilo Seccion
            iText.Layout.Style estiloSeccion = new iText.Layout.Style().SetTextAlignment(TextAlignment.LEFT)
                                                                       .SetFontColor(new DeviceRgb(192, 0, 0))
                                                                       .SetMargin(0)
                                                                       .SetBold()
                                                                       .SetFont(arial)
                                                                       .SetFontSize(9);
            // Estilo Encabezado
            iText.Layout.Style estiloEncabezado = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(153, 204, 255))
                                                                          .SetBold();
            // Estilo Resumen
            iText.Layout.Style estiloResumen = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(204, 236, 255))
                                                                       .SetPaddings(1.5f, 0, 1.5f, 0)
                                                                       .SetFont(arial)
                                                                       .SetFontSize(8);
            // Estilo Eventuales
            iText.Layout.Style estiloEventuales = new iText.Layout.Style().SetMargins(0, 0, 0, 0)
                                                                          .SetFontColor(new DeviceRgb(0, 112, 192));
            // Definimos la tabla y párrafo a usar.
            Table tabla;
            Paragraph parrafo;
            // SECCION HORAS MES ACTUAL
            celda.Add(new Paragraph("Horas Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
            tabla.AddStyle(estiloTabla);
            tabla.AddCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezado));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Trabajadas, null, null, null));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Acumuladas, null, null, null));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.Nocturnas, null, null, null));
            tabla.AddCell(new Cell().Add(new Paragraph("Cobradas")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Ex. Jornada")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Otras Horas")).AddStyle(estiloEncabezado));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.HorasCobradas, null, null, null));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.ExcesoJornada, null, null, null));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.OtrasHoras, null, null, null));
            celda.Add(tabla);
            //SECCION DÍAS MES ACTUAL
            celda.Add(new Paragraph("Días Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 16.6666f, 16.6666f, 16.6666f, 16.6666f, 16.6666f, 16.6666f }));
            tabla.AddStyle(estiloTabla);
            tabla.AddCell(new Cell().Add(new Paragraph("Trabajo")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("J-D")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("O-V")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("DND")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell(1, 2).Add(new Paragraph("Trabajo (JD)")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.Trabajo.ToString("00"));
            tabla.AddCell(pijama.Descanso.ToString("00"));
            tabla.AddCell(pijama.Vacaciones.ToString("00"));
            tabla.AddCell(pijama.DescansosNoDisfrutados.ToString("00"));
            tabla.AddCell(new Cell(1, 2).Add(new Paragraph(pijama.TrabajoEnDescanso.ToString("00"))));
            if (pijama.NoEsFijo) {
                tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoTrabajo.ToString("00.##"))).AddStyle(estiloEventuales));
                tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoDescanso.ToString("00.##"))).AddStyle(estiloEventuales));
                tabla.AddCell(new Cell().Add(new Paragraph(pijama.DiasComputoVacaciones.ToString("00.##"))).AddStyle(estiloEventuales));
                tabla.AddCell(new Cell(1, 3).Add(new Paragraph(pijama.TextoComputoEventuales)).AddStyle(estiloEventuales));
            }
            tabla.AddCell(new Cell().Add(new Paragraph("FN")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("E")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("DS")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("DC")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("PER")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("F6")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.DescansoEnFinde.ToString("00"));
            tabla.AddCell(pijama.Enfermo.ToString("00"));
            tabla.AddCell(pijama.DescansoSuelto.ToString("00"));
            tabla.AddCell(pijama.DescansoCompensatorio.ToString("00"));
            tabla.AddCell(pijama.Permiso.ToString("00"));
            tabla.AddCell(pijama.LibreDisposicionF6.ToString("00"));
            if (pijama.VerComite) {
                tabla.AddCell(new Cell(1, 2).Add(new Paragraph("Días Comité: " + pijama.Comite.ToString("00"))));
                tabla.AddCell(new Cell(1, 2).Add(new Paragraph("En JD: " + pijama.ComiteEnDescanso.ToString("00"))));
                tabla.AddCell(new Cell(1, 2).Add(new Paragraph("En DC: " + pijama.ComiteEnDC.ToString("00"))));
            }
            parrafo = new Paragraph();
            parrafo.Add(new Text("Total días: ").SetBold());
            parrafo.Add(new Text($"{pijama.DiasActivo:00} de {pijama.DiasMes:00}"));
            tabla.AddCell(new Cell(1, 6).Add(parrafo).AddStyle(estiloResumen));
            celda.Add(tabla);
            //SECCION FINES DE SEMANA MES ACTUAL
            celda.Add(new Paragraph("Fines de Semana Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }));
            tabla.AddStyle(estiloTabla);
            tabla.AddCell(new Cell().Add(new Paragraph("")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Sábados")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Domingos")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Festivos")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Descanso")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.SabadosDescansados.ToString("00"));
            tabla.AddCell(pijama.DomingosDescansados.ToString("00"));
            tabla.AddCell(pijama.FestivosDescansados.ToString("00"));
            tabla.AddCell(new Cell().Add(new Paragraph("Trabajo")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.SabadosTrabajados.ToString("00"));
            tabla.AddCell(pijama.DomingosTrabajados.ToString("00"));
            tabla.AddCell(pijama.FestivosTrabajados.ToString("00"));
            tabla.AddCell(new Cell().Add(new Paragraph("Pluses")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.PlusSabados.ToString("0.00 €"));
            tabla.AddCell(pijama.PlusDomingos.ToString("0.00 €"));
            tabla.AddCell(pijama.PlusFestivos.ToString("0.00 €"));
            parrafo = new Paragraph();
            parrafo.Add(new Text("Fines de Semana Completos: ").SetBold());
            parrafo.Add(new Text($"{pijama.FindesCompletos:0.00}"));
            tabla.AddCell(new Cell(1, 4).Add(parrafo).AddStyle(estiloResumen));
            celda.Add(tabla);
            //SECCION DIETAS Y PLUSES MES ACTUAL
            celda.Add(new Paragraph("Dietas y Pluses Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 }));
            tabla.AddStyle(estiloTabla);
            tabla.AddCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("P. Cena")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.DietaDesayuno.ToString("0.00"));
            tabla.AddCell(pijama.DietaComida.ToString("0.00"));
            tabla.AddCell(pijama.DietaCena.ToString("0.00"));
            tabla.AddCell(pijama.DietaPlusCena.ToString("0.00"));
            tabla.AddCell(new Cell().Add(new Paragraph("Menor D.")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Paquet.")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Otros")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.PlusMenorDescanso.ToString("0.00 €"));
            tabla.AddCell(pijama.PlusLimpieza.ToString("0.00 €"));
            tabla.AddCell(pijama.PlusPaqueteria.ToString("0.00 €"));
            tabla.AddCell(pijama.OtrosPluses.ToString("0.00 €"));
            tabla.SetBorderBottom(new SolidBorder(0));
            celda.Add(tabla);
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
            tabla.AddStyle(estiloTabla).SetMarginTop(-3);
            tabla.AddCell(new Cell().Add(new Paragraph("Total Findes")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("Total Pluses")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.ImporteTotalFindes.ToString("0.00 €"));
            tabla.AddCell($"{pijama.TotalDietas:0.00} ({pijama.ImporteTotalDietas:0.00 €})");
            tabla.AddCell(pijama.ImporteTotalPluses.ToString("0.00 €"));
            parrafo = new Paragraph();
            parrafo.Add(new Text("Importe Total: ").SetBold());
            parrafo.Add(new Text($"{pijama.ImporteTotal:0.00 €}"));
            tabla.AddCell(new Cell(1, 3).Add(parrafo).AddStyle(estiloResumen));
            tabla.SetBorderTop(new SolidBorder(0));
            celda.Add(tabla);
            //SECCION RESUMEN HASTA MES ACTUAL
            celda.Add(new Paragraph("Resumen Hasta Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
            tabla.AddStyle(estiloTabla);
            tabla.AddCell(new Cell().Add(new Paragraph("DNDs Pendientes")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("DCs Pendientes")).AddStyle(estiloEncabezado));
            tabla.AddCell(new Cell().Add(new Paragraph("H. Acumuladas")).AddStyle(estiloEncabezado));
            tabla.AddCell(pijama.DNDsPendientesHastaMes.ToString("00"));
            tabla.AddCell(pijama.DCsPendientesHastaMes.ToString("00"));
            tabla.AddCell((string)cnvSuperHoraMixta.Convert(pijama.AcumuladasHastaMes, null, null, null));
            parrafo = new Paragraph();
            parrafo.Add(new Text("DCs Generados: ").SetBold());
            parrafo.Add(new Text($"{pijama.DCsGeneradosHastaMes:0.00}"));
            tabla.AddCell(new Cell(1, 3).Add(parrafo).AddStyle(estiloResumen));
            celda.Add(tabla);
            // Devolvemos la celda.
            return celda;

        }


        private static Cell GetResumenPijamaNuevo(Pijama.HojaPijama pijama) {
            // Definimos la celda a devolver.
            Cell celda = new Cell();
            // Definimos el valor que nos indica si es fila alterna.
            bool esFilaAlterna = false;
            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo Tabla
            iText.Layout.Style estiloTabla = new iText.Layout.Style().SetTextAlignment(TextAlignment.LEFT)
                                                                     .SetMargins(0, 0, 2, 0)
                                                                     .SetPaddings(1f, 0, 1f, 0)
                                                                     .SetBorder(new SolidBorder(1))
                                                                     .SetFont(arial)
                                                                     .SetWidth(UnitValue.CreatePercentValue(100))
                                                                     .SetFontSize(9);
            // Estilo Seccion
            iText.Layout.Style estiloSeccion = new iText.Layout.Style().SetTextAlignment(TextAlignment.LEFT)
                                                                       .SetFontColor(new DeviceRgb(192, 0, 0))
                                                                       .SetMargin(0)
                                                                       .SetBold()
                                                                       .SetFont(arial)
                                                                       .SetFontSize(12);
            // Estilo Titulo
            iText.Layout.Style estiloTitulo = new iText.Layout.Style().SetTextAlignment(TextAlignment.LEFT)
                                                                      .SetMargin(0)
                                                                      .SetPaddings(0, 1.5f, 0, 1.5f)
                                                                      .SetBold()
                                                                      .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                                                      .SetBorderBottom(new SolidBorder(0.2f))
                                                                      .SetFont(arial)
                                                                      .SetFontSize(9);

            // Estilo Valor
            iText.Layout.Style estiloValor = new iText.Layout.Style().SetTextAlignment(TextAlignment.RIGHT)
                                                                      .SetMargin(0)
                                                                      .SetPaddings(0, 1.5f, 0, 1.5f)
                                                                      .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                                                                      .SetBorderBottom(new SolidBorder(0.2f))
                                                                      .SetFont(arial)
                                                                      .SetFontSize(9);

            // Estilo eventual
            iText.Layout.Style estiloEventual = new iText.Layout.Style().SetMarginLeft(15)
                                                                        .SetFontSize(8);
            // Estilo fila alterna
            iText.Layout.Style estiloFilaAlterna = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(204, 236, 255));


            // Estilo Encabezado
            iText.Layout.Style estiloEncabezado = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(153, 204, 255))
                                                                          .SetBold();


            // Estilo Resumen
            iText.Layout.Style estiloResumen = new iText.Layout.Style().SetBackgroundColor(new DeviceRgb(153, 204, 255))
                                                                       .SetBorder(new SolidBorder(1))
                                                                       .SetPaddings(1.5f, 0, 1.5f, 0)
                                                                       .SetTextAlignment(TextAlignment.CENTER)
                                                                       .SetFont(arial)
                                                                       .SetFontSize(9);
            // Estilo Eventuales
            iText.Layout.Style estiloEventuales = new iText.Layout.Style().SetMargins(0, 0, 0, 0)
                                                                          .SetFontColor(new DeviceRgb(0, 112, 192));
            // Definimos la tabla y párrafo a usar.
            Table tabla;
            Paragraph parrafo;
            Cell celdaTitulo;
            Cell celdaValor;
            // SECCION HORAS MES ACTUAL
            celda.Add(new Paragraph("Horas Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 55f, 45f }));
            tabla.AddStyle(estiloTabla);
            // Horas trabajadas
            if (pijama.Trabajadas.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Horas Trabajadas")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.Trabajadas, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Horas acumuladas
            if (pijama.Acumuladas.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Horas Acumuladas")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.Acumuladas, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Horas nocturnas
            if (pijama.Nocturnas.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Horas Nocturnas")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.Nocturnas, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Horas cobradas
            if (pijama.HorasCobradas.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Horas Cobradas")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.HorasCobradas, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Exceso Jornada
            if (pijama.ExcesoJornada.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Exceso de Jornada")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.ExcesoJornada, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Otras Horas
            if (pijama.OtrasHoras.Ticks != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Otras Horas")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.OtrasHoras, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            celda.Add(tabla);
            //SECCION DÍAS MES ACTUAL
            celda.Add(new Paragraph("Días Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 80f, 20f }));
            tabla.AddStyle(estiloTabla);
            esFilaAlterna = false;
            // Días Trabajados
            if (pijama.Trabajo != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días trabajados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Trabajo.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
                if (pijama.NoEsFijo) {
                    celdaTitulo = new Cell().Add(new Paragraph("(Debería trabajar)").SetMarginLeft(15)).AddStyle(estiloTitulo).AddStyle(estiloEventual);
                    celdaValor = new Cell().Add(new Paragraph(pijama.DiasComputoTrabajo.ToString("00.##"))).AddStyle(estiloValor).AddStyle(estiloEventual);
                    if (esFilaAlterna) {
                        celdaTitulo.AddStyle(estiloFilaAlterna);
                        celdaValor.AddStyle(estiloFilaAlterna);
                    }
                    tabla.AddCell(celdaTitulo);
                    tabla.AddCell(celdaValor);
                }
                esFilaAlterna = !esFilaAlterna;
            }
            // Días Descanso
            if (pijama.Descanso != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días descanso (J-D)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Descanso.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
                if (pijama.NoEsFijo) {
                    celdaTitulo = new Cell().Add(new Paragraph("(Debería descansar)").SetMarginLeft(15)).AddStyle(estiloTitulo).AddStyle(estiloEventual);
                    celdaValor = new Cell().Add(new Paragraph(pijama.DiasComputoDescanso.ToString("00.##"))).AddStyle(estiloValor).AddStyle(estiloEventual);
                    if (esFilaAlterna) {
                        celdaTitulo.AddStyle(estiloFilaAlterna);
                        celdaValor.AddStyle(estiloFilaAlterna);
                    }
                    tabla.AddCell(celdaTitulo);
                    tabla.AddCell(celdaValor);
                }
                esFilaAlterna = !esFilaAlterna;
            }
            // Días de vacaciones
            if (pijama.Vacaciones != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días vacaciones (O-V)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Vacaciones.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
                if (pijama.NoEsFijo) {
                    celdaTitulo = new Cell().Add(new Paragraph("(Debería tener de vacaciones)").SetMarginLeft(15)).AddStyle(estiloTitulo).AddStyle(estiloEventual);
                    celdaValor = new Cell().Add(new Paragraph(pijama.DiasComputoVacaciones.ToString("00.##"))).AddStyle(estiloValor).AddStyle(estiloEventual);
                    if (esFilaAlterna) {
                        celdaTitulo.AddStyle(estiloFilaAlterna);
                        celdaValor.AddStyle(estiloFilaAlterna);
                    }
                    tabla.AddCell(celdaTitulo);
                    tabla.AddCell(celdaValor);
                }
                esFilaAlterna = !esFilaAlterna;
            }
            // Días DND
            if (pijama.DescansosNoDisfrutados != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Descansos no disfrutados (DND)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.DescansosNoDisfrutados.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días trabajo en JD
            if (pijama.TrabajoEnDescanso != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días trabajados en J-D")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.TrabajoEnDescanso.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días FN
            if (pijama.DescansoEnFinde != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Descansos en Finde (FN)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.DescansoEnFinde.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días E
            if (pijama.Enfermo != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días de enfermedad (E)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Enfermo.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días DS
            if (pijama.DescansoSuelto != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Descansos sueltos (DS)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.DescansoSuelto.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días DC
            if (pijama.DescansoCompensatorio != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Descansos compensatorios (DC)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.DescansoCompensatorio.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días PER
            if (pijama.Permiso != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días de permiso (PER)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Permiso.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días F6
            if (pijama.LibreDisposicionF6 != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días de libre disposición (F6)")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.LibreDisposicionF6.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días comité
            if (pijama.Comite != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Días de comité")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.Comite.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días comite en JD
            if (pijama.ComiteEnDescanso != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("    En J-D")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.ComiteEnDescanso.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Días comité en DC
            if (pijama.ComiteEnDC != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("    En D-C")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(pijama.ComiteEnDC.ToString("00"))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Resumen días
            parrafo = new Paragraph();
            parrafo.Add(new Text("Total días: ").SetBold());
            parrafo.Add(new Text($"{pijama.DiasActivo:00} de {pijama.DiasMes:00}"));
            tabla.AddCell(new Cell(1, 2).Add(parrafo).AddStyle(estiloResumen));
            celda.Add(tabla);
            //SECCION FINES DE SEMANA MES ACTUAL
            celda.Add(new Paragraph("Fines de Semana Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 70f, 30f }));
            tabla.AddStyle(estiloTabla);
            esFilaAlterna = false;
            // Sábados Trabajados/Descansados
            if (pijama.SabadosTrabajados != 0) {
                string valor = $"{pijama.SabadosTrabajados.ToString("00")}/{pijama.SabadosDescansados.ToString("00")}";
                celdaTitulo = new Cell().Add(new Paragraph("Sábados Trabajados/Descansados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(valor)).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Total Plus Sábados 
            if (pijama.PlusSabados != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Total Plus Sábados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusSabados, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Domingos Trabajados/Descansados
            if (pijama.DomingosTrabajados != 0) {
                string valor = $"{pijama.DomingosTrabajados.ToString("00")}/{pijama.DomingosDescansados.ToString("00")}";
                celdaTitulo = new Cell().Add(new Paragraph("Domingos Trabajados/Descansados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(valor)).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Total Plus Domingos 
            if (pijama.PlusDomingos != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Total Plus Domingos")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusDomingos, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Festivos Trabajados/Descansados
            if (pijama.FestivosTrabajados != 0) {
                string valor = $"{pijama.FestivosTrabajados.ToString("00")}/{pijama.FestivosDescansados.ToString("00")}";
                celdaTitulo = new Cell().Add(new Paragraph("Festivos Trabajados/Descansados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(valor)).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Total Plus Festivos 
            if (pijama.PlusFestivos != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Total Plus Festivos")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusFestivos, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Findes Completos Disfrutados 
            if (pijama.FindesCompletos != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Findes Completos Disfrutados")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimal.Convert(pijama.FindesCompletos, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // TOTAL FINDES
            if (pijama.ImporteTotalFindes != 0) {
                parrafo = new Paragraph();
                parrafo.Add(new Text("Total Findes+Festivos = ").SetBold());
                parrafo.Add(new Text((string)cnvDecimalEuro.Convert(pijama.ImporteTotalFindes, null, null, null)));
                tabla.AddCell(new Cell(1, 2).Add(parrafo).AddStyle(estiloResumen));
            }
            celda.Add(tabla);
            //SECCION DIETAS Y PLUSES MES ACTUAL
            celda.Add(new Paragraph("Dietas y Pluses Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 55f, 45f }));
            tabla.AddStyle(estiloTabla);
            esFilaAlterna = false;
            // Dietas Desayuno
            if (pijama.DietaDesayuno != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Dietas Desayuno")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimal.Convert(pijama.DietaDesayuno, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Dietas Comida
            if (pijama.DietaComida != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Dietas Comida")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimal.Convert(pijama.DietaComida, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Dietas Cena
            if (pijama.DietaCena != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Dietas Cena")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimal.Convert(pijama.DietaCena, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Dietas Plus Cena
            if (pijama.DietaPlusCena != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Dietas Plus Cena")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimal.Convert(pijama.DietaPlusCena, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Plus Menor Descanso
            if (pijama.PlusMenorDescanso != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Plus Menor Descanso")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusMenorDescanso, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Plus Limpieza
            if (pijama.PlusLimpieza != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Plus Limpieza")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusLimpieza, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Plus Paquetería
            if (pijama.PlusPaqueteria != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Plus Paquetería")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.PlusPaqueteria, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Otros Pluses
            if (pijama.OtrosPluses != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("Otros Pluses")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.OtrosPluses, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // TOTAL DIETAS/IMPORTE
            if (pijama.TotalDietas != 0) {
                string valor = $"{(string)cnvDecimal.Convert(pijama.TotalDietas, null, null, null)}" +
                               $"/" +
                               $"{(string)cnvDecimalEuro.Convert(pijama.ImporteTotalDietas, null, null, null)}";
                celdaTitulo = new Cell().Add(new Paragraph("TOTAL DIETAS/IMPORTE")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph(valor)).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // Total Pluses
            if (pijama.ImporteTotalPluses != 0) {
                celdaTitulo = new Cell().Add(new Paragraph("TOTAL PLUSES")).AddStyle(estiloTitulo);
                celdaValor = new Cell().Add(new Paragraph((string)cnvDecimalEuro.Convert(pijama.ImporteTotalPluses, null, null, null))).AddStyle(estiloValor);
                if (esFilaAlterna) {
                    celdaTitulo.AddStyle(estiloFilaAlterna);
                    celdaValor.AddStyle(estiloFilaAlterna);
                }
                esFilaAlterna = !esFilaAlterna;
                tabla.AddCell(celdaTitulo);
                tabla.AddCell(celdaValor);
            }
            // TOTAL FINDES
            if (pijama.ImporteTotal != 0) {
                parrafo = new Paragraph();
                parrafo.Add(new Text("IMPORTE TOTAL A PERCIBIR = ").SetBold());
                parrafo.Add(new Text((string)cnvDecimalEuro.Convert(pijama.ImporteTotal, null, null, null)));
                tabla.AddCell(new Cell(1, 2).Add(parrafo).AddStyle(estiloResumen));
            }
            //
            celda.Add(tabla);
            //SECCION RESUMEN HASTA MES ACTUAL
            celda.Add(new Paragraph("Pendiente Hasta Mes Actual").AddStyle(estiloSeccion));
            tabla = new Table(UnitValue.CreatePercentArray(new float[] { 33.3333f, 33.3333f, 33.3333f }));
            tabla.AddStyle(estiloTabla);
            tabla.SetVerticalAlignment(VerticalAlignment.BOTTOM);
            tabla.AddCell(new Cell().Add(new Paragraph("DNDs")).AddStyle(estiloResumen).SetPaddings(0, 1.5f, 0, 1.5f));
            tabla.AddCell(new Cell().Add(new Paragraph("DCs")).AddStyle(estiloResumen).SetPaddings(0, 1.5f, 0, 1.5f));
            tabla.AddCell(new Cell().Add(new Paragraph("H.Acumuladas")).AddStyle(estiloResumen).SetPaddings(0, 1.5f, 0, 1.5f));
            celdaTitulo = new Cell().Add(new Paragraph(pijama.DNDsPendientesHastaMes.ToString("0.0000")))
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetPaddings(0, 1.5f, 0, 1.5f)
                                    .SetBorder(new SolidBorder(1));
            tabla.AddCell(celdaTitulo);
            celdaTitulo = new Cell().Add(new Paragraph(pijama.DCsPendientesHastaMes.ToString("0.0000")))
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetPaddings(0, 1.5f, 0, 1.5f)
                                    .SetBorder(new SolidBorder(1));
            tabla.AddCell(celdaTitulo);
            celdaTitulo = new Cell().Add(new Paragraph((string)cnvSuperHoraMixta.Convert(pijama.AcumuladasHastaMes, null, null, null)))
                                    .SetTextAlignment(TextAlignment.CENTER)
                                    .SetPaddings(0, 1.5f, 0, 1.5f)
                                    .SetBorder(new SolidBorder(1));
            tabla.AddCell(celdaTitulo);
            parrafo = new Paragraph();
            parrafo.Add(new Text("DCs Generados: ").SetBold());
            parrafo.Add(new Text($"{pijama.DCsGeneradosHastaMes:0.0000}"));
            tabla.AddCell(new Cell(1, 3).Add(parrafo).AddStyle(estiloResumen).SetPaddings(0, 1.5f, 0, 1.5f));
            celda.Add(tabla);
            // Devolvemos la celda.
            return celda;
        }

        private static Table GetHojaPijama(Pijama.HojaPijama pijama) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            iText.Layout.Style estiloTabla = new iText.Layout.Style();
            estiloTabla.SetWidth(UnitValue.CreatePercentValue(100))
                       .SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                       .SetFont(arial)
                       .SetMargin(0)
                       .SetPadding(0);
            iText.Layout.Style estiloCelda = new iText.Layout.Style();
            estiloCelda.SetBorder(iText.Layout.Borders.Border.NO_BORDER)
                       .SetMargin(0)
                       .SetPadding(0)
                       .SetFont(arial)
                       .SetFontSize(16);
            // Creamos la tabla
            Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 50, 25, 25 }));
            tabla.AddStyle(estiloTabla);

            Cell celda;
            // Añadimos al trabajador
            celda = new Cell();
            celda.Add(new Paragraph(pijama.TextoTrabajador).SetMargin(0));
            celda.AddStyle(estiloCelda);
            celda.SetPaddings(0, 0, 0, 2);
            celda.SetTextAlignment(TextAlignment.LEFT);
            celda.SetVerticalAlignment(VerticalAlignment.BOTTOM);
            tabla.AddCell(celda);
            // Añadimos la fecha
            celda = new Cell();
            celda.Add(new Paragraph(pijama.TextoMesActual).SetMargin(0));
            celda.AddStyle(estiloCelda);
            celda.SetPaddings(0, 10, 0, 0);
            celda.SetTextAlignment(TextAlignment.RIGHT);
            celda.SetVerticalAlignment(VerticalAlignment.BOTTOM);
            tabla.AddCell(celda);
            // Añadimos el logo del sindicato
            celda = new Cell();
            celda.AddStyle(estiloCelda);
            tabla.AddCell(celda);
            // Añadimos la tabla
            celda = new Cell(1, 2);
            celda.AddStyle(estiloCelda);
            celda.SetPaddings(0, 8, 0, 0);
            celda.Add(GetTablaPijama(pijama.ListaDias));
            tabla.AddCell(celda);
            // Añadimos el resumen
            //celda = GetResumenPijama(pijama);
            celda = GetResumenPijamaNuevo(pijama);
            celda.AddStyle(estiloCelda);
            tabla.AddCell(celda);
            // Devolvemos la tabla
            return tabla;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS PDF
        // ====================================================================================================

        public static async Task CrearPijamaEnPdf(Document doc, Pijama.HojaPijama pijama) {

            await Task.Run(() => {

                float anchoHoja = PageSize.A4.Rotate().GetWidth();
                float altoHoja = PageSize.A4.Rotate().GetHeight();
                iText.Layout.Element.Image imagen = InformesServicio.GetLogoSindicato();
                if (imagen != null) {
                    imagen.ScaleToFit(anchoHoja / 5, 30);
                    float ancho = imagen.GetImageScaledWidth();
                    float alto = imagen.GetImageScaledHeight();
                    imagen.SetFixedPosition(anchoHoja - 25 - ancho, altoHoja - 25 - alto);
                    doc.Add(imagen);
                }
                // Añadimos la tabla al documento
                doc.Add(GetHojaPijama(pijama));
            });
        }


        public static async Task CrearTodosPijamasEnPdf(Document doc, ListCollectionView listaCalendarios) {

            await Task.Run(() => {
                // Activamos la barra de progreso.
                App.Global.IniciarProgreso("Recopilando...");
                double num = 1;
                List<Pijama.HojaPijama> Lista = new List<Pijama.HojaPijama>();
                foreach (Object obj in listaCalendarios) {
                    double valor = num / listaCalendarios.Count * 100;
                    App.Global.ValorBarraProgreso = valor;
                    Calendario calendario = obj as Calendario;
                    if (calendario == null) continue;
                    Pijama.HojaPijama hojapijama = new Pijama.HojaPijama(calendario, new MensajesServicio());
                    Lista.Add(hojapijama);
                    num++;
                }
                // Definimos el logo del sindicato.
                float anchoHoja = PageSize.A4.Rotate().GetWidth();
                float altoHoja = PageSize.A4.Rotate().GetHeight();
                iText.Layout.Element.Image imagen = InformesServicio.GetLogoSindicato();
                if (imagen != null) {
                    imagen.ScaleToFit(anchoHoja / 5, 30);
                    float ancho = imagen.GetImageScaledWidth();
                    float alto = imagen.GetImageScaledHeight();
                    imagen.SetFixedPosition(anchoHoja - 25 - ancho, altoHoja - 25 - alto);
                }
                // Creamos una hoja por cada pijama.
                num = 1;
                App.Global.IniciarProgreso("Creando PDF...");
                foreach (Pijama.HojaPijama pijama in Lista) {
                    double valor = num / Lista.Count * 100;
                    App.Global.ValorBarraProgreso = valor;
                    if (num > 1) doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    num++;
                    doc.Add(GetHojaPijama(pijama));
                    if (imagen != null) doc.Add(imagen);
                }
            });
        }


        public static async Task CrearReclamacionEnPdf(Document doc, DateTime fecha, Conductor conductor, IEnumerable<Reclamacion> reclamaciones) {

            await Task.Run(() => {

                // Iniciamos la creación del PDF.
                App.Global.IniciarProgreso("Creando PDF...");
                float anchoHoja = PageSize.A4.GetWidth();
                float altoHoja = PageSize.A4.GetHeight();
                PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
                doc.SetFont(arial);
                doc.SetFontSize(12);
                PdfAcroForm form = PdfAcroForm.GetAcroForm(doc.GetPdfDocument(), true);

                // Añadimos el texto inicial
                Paragraph parrafo = new Paragraph();
                parrafo.SetFontSize(26).SetBold();
                parrafo.SetMarginTop(4);
                parrafo.SetFixedLeading(27);
                parrafo.Add("RECLAMACIÓN\nDE CONCEPTOS");
                doc.Add(parrafo);

                // Añadimos el centro de trabajo y el conductor
                parrafo = new Paragraph();
                parrafo.SetFontSize(16);
                parrafo.SetMarginTop(12);
                parrafo.Add(new Text($"Centro: ").SetBold());
                parrafo.Add($"{App.Global.CentroActual.ToString().ToUpper()}");
                parrafo.Add("\n");
                parrafo.Add(new Text($"Trabajador/a: ").SetBold());
                parrafo.Add($"{conductor.Apellidos}, {conductor.Nombre} ({conductor.Id:000})");
                doc.Add(parrafo);

                // ==================================================
                // TABLA
                // ==================================================
                // Estilos
                iText.Layout.Style estiloCabecera = new iText.Layout.Style();
                estiloCabecera.SetFontSize(11).SetBold();
                estiloCabecera.SetTextAlignment(TextAlignment.CENTER);
                estiloCabecera.SetBackgroundColor(new DeviceRgb(255, 80, 80));
                estiloCabecera.SetBorder(new SolidBorder(1));
                iText.Layout.Style estiloBordes = new iText.Layout.Style();
                estiloBordes.SetBorderLeft(new SolidBorder(1));
                estiloBordes.SetBorderRight(new SolidBorder(1));
                // Creación de tabla
                Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1, 1, 1 }));
                tabla.SetWidth(UnitValue.CreatePercentValue(100));
                tabla.SetFontSize(10);
                tabla.SetMargins(40, 0, 0, 0);
                tabla.SetPadding(0);
                // Añadimos los encabezados.
                Cell celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("CONCEPTOS A RECLAMAR"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("En PIJAMA"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("REAL"));
                tabla.AddHeaderCell(celda);
                celda = new Cell().AddStyle(estiloCabecera);
                celda.Add(new Paragraph("DIFERENCIA"));
                tabla.AddHeaderCell(celda);
                // Creamos un array para los campos
                var campos = new PdfServicio.TextFieldRenderer[17, 4];
                // Añadimos las filas.
                for (int f = 0; f < 17; f++) {
                    for (int c = 0; c < 4; c++) {
                        App.Global.ValorBarraProgreso = 5.8;
                        celda = new Cell();
                        if (f % 2 != 0) celda.SetBackgroundColor(new DeviceRgb(221, 221, 221));
                        celda.SetHeight(16);
                        celda.AddStyle(estiloBordes);
                        var alineacion = PdfFormField.ALIGN_CENTER;
                        if (c == 0) alineacion = PdfFormField.ALIGN_LEFT;
                        var campo = new PdfServicio.TextFieldRenderer(celda, $"Fila{f:00}Columna{c}", 2, 10, alineacion);
                        campos[f, c] = campo;
                        celda.SetNextRenderer(campo);
                        tabla.AddCell(celda);
                    }
                }
                //Añadimos las reclamaciones pasadas.
                int fila = 0;
                foreach (Reclamacion reclamacion in reclamaciones) {
                    campos[fila, 0].valor = reclamacion.Concepto;
                    campos[fila, 1].valor = reclamacion.EnPijama;
                    campos[fila, 2].valor = reclamacion.Real;
                    campos[fila, 3].valor = reclamacion.Diferencia;
                    fila++;
                }

                // Dibujamos los bordes de la tabla.
                tabla.SetBorder(new SolidBorder(1));
                // Añadimos la tabla al documento.
                doc.Add(tabla);

                // Añadimos el logo del sindicato.
                iText.Layout.Element.Image logoSindicato = InformesServicio.GetLogoSindicato();
                if (logoSindicato != null) {
                    logoSindicato.ScaleToFit(anchoHoja / 3, 75);
                    logoSindicato.SetFixedPosition(anchoHoja - 40 - logoSindicato.GetImageScaledWidth(), altoHoja - 40 - logoSindicato.GetImageScaledHeight());
                    doc.Add(logoSindicato);
                }

                // Añadimos la fecha
                parrafo = new Paragraph($"{fecha:MMMM yyyy}".ToUpper());
                parrafo.SetFontSize(14);
                parrafo.SetFixedPosition(50, 640, (anchoHoja - 80) / 2);
                doc.Add(parrafo);

                // Añadimos el número de reclamación
                iText.Kernel.Geom.Rectangle cuadroNumReclamacion = new iText.Kernel.Geom.Rectangle(40 + ((anchoHoja - 80) / 2), 642, ((anchoHoja - 80) / 2) - 10, 20);
                string numReclamacion = $"Nº Reclamación: {fecha:yyyyMM}{conductor.Id:000}/01";
                PdfTextFormField campoNumReclamacion = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroNumReclamacion, "NumeroReclamacion", numReclamacion);
                campoNumReclamacion.SetJustification(PdfFormField.ALIGN_RIGHT);
                campoNumReclamacion.SetVisibility(PdfFormField.VISIBLE);
                campoNumReclamacion.SetFontSize(14);
                form.AddField(campoNumReclamacion);

                // Añadimos el título de las notas.
                parrafo = new Paragraph($"NOTAS");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(40, 225, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos el campo para las notas.
                iText.Kernel.Geom.Rectangle cuadroNotas = new iText.Kernel.Geom.Rectangle(45, 55, ((anchoHoja - 80) / 3) * 2 - 20, 160);
                PdfTextFormField campoNotas = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroNotas, "Notas", "", arial, 12);
                campoNotas.SetJustification(PdfFormField.ALIGN_LEFT);
                campoNotas.SetVisibility(PdfFormField.VISIBLE);
                campoNotas.SetMultiline(true);
                form.AddField(campoNotas);

                // Añadimos el título de la fecha.
                parrafo = new Paragraph($"FECHA");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(((anchoHoja - 80) / 3) * 2 + 40, 225, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos el campo para la fecha.
                iText.Kernel.Geom.Rectangle cuadroFecha = new iText.Kernel.Geom.Rectangle(((anchoHoja - 80) / 3) * 2 + 45, 190, ((anchoHoja - 80) / 3) - 10, 25);
                string fechaFinal = $"{DateTime.Now:dd - MM - yyyy}";
                PdfTextFormField campoFecha = PdfFormField.CreateText(doc.GetPdfDocument(), cuadroFecha, "FechaFinal", fechaFinal, arial, 16);
                campoFecha.SetJustification(PdfFormField.ALIGN_CENTER);
                campoFecha.SetVisibility(PdfFormField.VISIBLE);
                form.AddField(campoFecha);

                // Añadimos el título de la firma.
                parrafo = new Paragraph($"El Trabajador/a");
                parrafo.SetFontSize(12).SetBold();
                parrafo.SetFixedPosition(((anchoHoja - 80) / 3) * 2 + 40, 150, (anchoHoja - 80) / 3);
                doc.Add(parrafo);

                // Añadimos la marca de agua.
                iText.Layout.Element.Image marcaAgua = InformesServicio.GetMarcaDeAgua();
                if (marcaAgua != null) {
                    marcaAgua.ScaleToFit((anchoHoja / 3) * 2, 800);
                    marcaAgua.SetOpacity(0.05f);
                    marcaAgua.SetFixedPosition((anchoHoja / 2) - (marcaAgua.GetImageScaledWidth() / 2), ((altoHoja) / 2) - (marcaAgua.GetImageScaledHeight() / 2));
                    doc.Add(marcaAgua);
                }

                // ==================================================
                // ACCIONES EN EL LIENZO
                // ==================================================

                PdfCanvas lienzo = new PdfCanvas(doc.GetPdfDocument().GetFirstPage());

                // Fecha y Número de reclamación
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(40, 640, anchoHoja - 80, 24, 5);
                lienzo.Stroke();

                // Notas
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(40, 50, ((anchoHoja - 80) / 3) * 2 - 10, 170, 5);
                lienzo.Stroke();

                // Fecha Final
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(((anchoHoja - 80) / 3) * 2 + 40, 185, (anchoHoja - 80) / 3, 35, 5);
                lienzo.Stroke();

                // Firma
                lienzo.SetStrokeColor(ColorConstants.BLACK);
                lienzo.SetLineWidth(1);
                lienzo.RoundRectangle(((anchoHoja - 80) / 3) * 2 + 40, 50, (anchoHoja - 80) / 3, 95, 5);
                lienzo.Stroke();
            });

        }




        #endregion
        // ====================================================================================================




    }
}
