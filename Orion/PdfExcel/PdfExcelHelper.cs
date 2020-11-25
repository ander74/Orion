#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.IO;
using System.Linq;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using iBorders = iText.Layout.Borders;
using Win = System.Windows.Media;

namespace Orion.PdfExcel {

    public class PdfExcelHelper {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private static PdfExcelHelper instance;

        // Estilos para el Pdf.
        private Style styTable = new Style();
        private Style styTitle = new Style();
        private Style styLogo = new Style();
        private Style styHeader = new Style();
        private Style styHeaderFirst = new Style();
        private Style styHeaderLast = new Style();
        private Style styCell = new Style();
        private Style styCellFirst = new Style();
        private Style styCellLast = new Style();
        private Style styCellAlternate = new Style();
        private Style styTotal = new Style();
        private Style styTotalFirst = new Style();
        private Style styTotalLast = new Style();


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR + SINGLETON
        // ====================================================================================================

        private PdfExcelHelper() { }

        public static PdfExcelHelper GetInstance() {
            if (instance == null) instance = new PdfExcelHelper();
            return instance;
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        /// <summary>
        /// Genera los estilos necesarios para construir la tabla a partir de las propiedades del objeto <see cref="PdfTableData"/>.
        /// </summary>
        private void generateStyles(PdfTableData table) {
            // FUENTE
            var tableFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            // TABLA
            styTable.SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetMargins(0, 0, 0, 0)
                .SetPaddings(0, 0, 0, 0)
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetFont(tableFont)
                .SetFontSize(table.CellsFontSize);
            // TÍTULO
            styTitle.SetBold()
                .SetBorder(iBorders.Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(table.TitleFontSize);
            // LOGO
            styLogo.SetBorder(iBorders.Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT);
            // CELDA ENCABEZADO
            styHeader.SetBackgroundColor(colorToPdf(table.HeadersBackground))
                .SetBold()
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFontSize(table.HeadersFontSize)
                .SetFontColor(colorToPdf(table.HeadersForeground))
                .SetBorderTop(borderToPdf(table.TableBorder))
                .SetBorderBottom(borderToPdf(PdfBorder.SOLID_MEDIO));
            // CELDA ENCABEZADO PRIMERA
            styHeaderFirst.SetBorderLeft(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA ENCABEZADO ÚLTIMA
            styHeaderLast.SetBorderRight(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA
            styCell.SetBackgroundColor(colorToPdf(table.RowsBackground))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA PRIMERA
            styCellFirst.SetBorderLeft(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA ÚLTIMA
            styCellLast.SetBorderRight(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA ALTERNA
            styCellAlternate.SetBackgroundColor(colorToPdf(table.AlternateRowsBackground))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA TOTALES
            styTotal.SetBackgroundColor(colorToPdf(table.HeadersBackground))
                .SetBold()
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFontSize(table.HeadersFontSize)
                .SetFontColor(colorToPdf(table.HeadersForeground))
                .SetBorderTop(borderToPdf(PdfBorder.SOLID_MEDIO))
                .SetBorderBottom(borderToPdf(table.TableBorder));
            // CELDA TOTALES PRIMERA
            styTotalFirst.SetBorderLeft(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            // CELDA TOTALES ÚLTIMA
            styTotalLast.SetBorderRight(borderToPdf(table.TableBorder))
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
        }


        /// <summary>
        /// Extrae el logo del sindicato de las opciones y lo devuelve como un objeto Image.
        /// </summary>
        private Image getImageFromFile(string path) {
            Image imagen = null;
            if (File.Exists(path)) {
                switch (Path.GetExtension(path).ToLower()) {
                    case ".jpg":
                        imagen = new Image(ImageDataFactory.CreateJpeg(new Uri(path)));
                        break;
                    case ".png":
                        imagen = new Image(ImageDataFactory.CreatePng(new Uri(path)));
                        break;
                }
            }
            return imagen;
        }


        /// <summary>
        /// Convierte un <see cref="PdfBorder"/> en un <see cref="Border"/> válido para crear la tabla.
        /// </summary>
        private iBorders.Border borderToPdf(PdfBorder borde) {
            switch (borde) {
                case PdfBorder.SOLID_FINO: return new iBorders.SolidBorder(0.4f);
                case PdfBorder.SOLID_MEDIO: return new iBorders.SolidBorder(0.8f);
                case PdfBorder.SOLID_GRUESO: return new iBorders.SolidBorder(1.5f);
                case PdfBorder.RAYAS_FINO: return new iBorders.DashedBorder(0.4f);
                case PdfBorder.RAYAS_MEDIO: return new iBorders.DashedBorder(0.8f);
                case PdfBorder.RAYAS_GRUESO: return new iBorders.DashedBorder(1.5f);
                case PdfBorder.PUNTOS_FINO: return new iBorders.RoundDotsBorder(0.4f);
                case PdfBorder.PUNTOS_MEDIO: return new iBorders.RoundDotsBorder(0.8f);
                case PdfBorder.PUNTOS_GRUESO: return new iBorders.RoundDotsBorder(1.5f);
                case PdfBorder.DOBLE_FINO: return new iBorders.DoubleBorder(0.8f);
                case PdfBorder.DOBLE_MEDIO: return new iBorders.DoubleBorder(1.5f);
                case PdfBorder.DOBLE_GRUESO: return new iBorders.DoubleBorder(2.8f);
            }
            return iBorders.Border.NO_BORDER;
        }


        /// <summary>
        /// Convierte un <see cref="System.Windows.Media.Color"/> en un <see cref="iText.Kernel.Colors.Color"/>.
        /// </summary>
        private Color colorToPdf(Win.Color color) {
            return new DeviceRgb(color.R, color.G, color.B);
        }


        /// <summary>
        /// Convierte un <see cref="System.Windows.TextAlignment"/> en un <see cref="ExcelStyle.ExcelHorizontalAlignment"/>.
        /// </summary>
        /// <param name="textAlign"></param>
        /// <returns></returns>
        private ExcelHorizontalAlignment hAlignToExcel(System.Windows.TextAlignment textAlign) {
            switch (textAlign) {
                case System.Windows.TextAlignment.Left: return ExcelHorizontalAlignment.Left;
                case System.Windows.TextAlignment.Right: return ExcelHorizontalAlignment.Right;
                case System.Windows.TextAlignment.Center: return ExcelHorizontalAlignment.Center;
                case System.Windows.TextAlignment.Justify: return ExcelHorizontalAlignment.Justify;
            }
            return ExcelHorizontalAlignment.Center;
        }


        /// <summary>
        /// Convierte un <see cref="System.Windows.VerticalAlignment"/> en un <see cref="ExcelStyle.ExcelVerticalAlignment"/>.
        /// </summary>
        private ExcelVerticalAlignment vAlignToExcel(System.Windows.VerticalAlignment vAlign) {
            switch (vAlign) {
                case System.Windows.VerticalAlignment.Top: return ExcelVerticalAlignment.Top;
                case System.Windows.VerticalAlignment.Center: return ExcelVerticalAlignment.Center;
                case System.Windows.VerticalAlignment.Bottom: return ExcelVerticalAlignment.Bottom;
            }
            return ExcelVerticalAlignment.Center;
        }


        /// <summary>
        /// Convierte un <see cref="PdfBorder"/> en un <see cref="ExcelStyle.ExcelBorderStyle"/>.
        /// </summary>
        private ExcelBorderStyle borderToExcel(PdfBorder borde) {
            switch (borde) {
                case PdfBorder.SOLID_FINO: return ExcelBorderStyle.Thin;
                case PdfBorder.SOLID_MEDIO: return ExcelBorderStyle.Medium;
                case PdfBorder.SOLID_GRUESO: return ExcelBorderStyle.Thick;
                case PdfBorder.RAYAS_FINO: return ExcelBorderStyle.Dashed;
                case PdfBorder.RAYAS_MEDIO: return ExcelBorderStyle.MediumDashed;
                case PdfBorder.RAYAS_GRUESO: return ExcelBorderStyle.MediumDashed;
                case PdfBorder.PUNTOS_FINO: return ExcelBorderStyle.Dotted;
                case PdfBorder.PUNTOS_MEDIO: return ExcelBorderStyle.Dotted;
                case PdfBorder.PUNTOS_GRUESO: return ExcelBorderStyle.Dotted;
                case PdfBorder.DOBLE_FINO: return ExcelBorderStyle.Double;
                case PdfBorder.DOBLE_MEDIO: return ExcelBorderStyle.Double;
                case PdfBorder.DOBLE_GRUESO: return ExcelBorderStyle.Double;
            }
            return ExcelBorderStyle.None;
        }


        /// <summary>
        /// Aplica las propiedades que contiene el objeto <see cref="PdfCellInfo"/> a una celda de excel.
        /// </summary>
        private void applyExcelStyleToCell(ref ExcelRange celda, PdfCellInfo info) {
            if (!string.IsNullOrEmpty(info.ExcelNumberFormat)) celda.Style.Numberformat.Format = info.ExcelNumberFormat;
            if (info.TextAlign is System.Windows.TextAlignment textAlign) celda.Style.HorizontalAlignment = hAlignToExcel(textAlign);
            if (info.TextFontColor is Win.Color color) celda.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(color.R, color.G, color.B));
            if (info.TextFontSize is double fontSize) celda.Style.Font.Size = Convert.ToSingle(fontSize);
            if (info.VAlign is System.Windows.VerticalAlignment vAlign) celda.Style.VerticalAlignment = vAlignToExcel(vAlign);
            if (info.IsBold == true) celda.Style.Font.Bold = true;
            if (info.IsItalic == true) celda.Style.Font.Italic = true;
            if (info.Borders.left != PdfBorder.DEFAULT) celda.Style.Border.Left.Style = borderToExcel(info.Borders.left);
            if (info.Borders.right != PdfBorder.DEFAULT) celda.Style.Border.Right.Style = borderToExcel(info.Borders.right);
            if (info.Borders.top != PdfBorder.DEFAULT) celda.Style.Border.Top.Style = borderToExcel(info.Borders.top);
            if (info.Borders.bottom != PdfBorder.DEFAULT) celda.Style.Border.Bottom.Style = borderToExcel(info.Borders.bottom);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        /// <summary>
        /// Devuelve el <see cref="Style"/> que se debe aplicar a una celda usando las propiedades 
        /// del objeto <see cref="PdfCellInfo"/>.
        /// </summary>
        public Style GetPdfStyle(PdfCellInfo celda) {
            var estilo = new Style();
            if (celda.TextFontColor is Win.Color color) estilo.SetFontColor(new DeviceRgb(color.R, color.G, color.B));
            if (celda.TextFontSize is double fontSize) estilo.SetFontSize(Convert.ToSingle(fontSize));
            if (celda.IsBold == true) estilo.SetBold();
            if (celda.IsItalic == true) estilo.SetItalic();
            switch (celda.TextAlign) {
                case System.Windows.TextAlignment.Left:
                    estilo.SetTextAlignment(TextAlignment.LEFT);
                    break;
                case System.Windows.TextAlignment.Right:
                    estilo.SetTextAlignment(TextAlignment.RIGHT);
                    break;
                case System.Windows.TextAlignment.Center:
                    estilo.SetTextAlignment(TextAlignment.CENTER);
                    break;
                case System.Windows.TextAlignment.Justify:
                    estilo.SetTextAlignment(TextAlignment.JUSTIFIED);
                    break;
            }
            switch (celda.VAlign) {
                case System.Windows.VerticalAlignment.Top:
                    estilo.SetVerticalAlignment(VerticalAlignment.TOP);
                    break;
                case System.Windows.VerticalAlignment.Center:
                    estilo.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    break;
                case System.Windows.VerticalAlignment.Bottom:
                    estilo.SetVerticalAlignment(VerticalAlignment.BOTTOM);
                    break;
                case System.Windows.VerticalAlignment.Stretch:
                    estilo.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                    break;

            }
            if (celda.Borders.left != PdfBorder.DEFAULT) estilo.SetBorderLeft(borderToPdf(celda.Borders.left));
            if (celda.Borders.right != PdfBorder.DEFAULT) estilo.SetBorderRight(borderToPdf(celda.Borders.right));
            if (celda.Borders.top != PdfBorder.DEFAULT) estilo.SetBorderTop(borderToPdf(celda.Borders.top));
            if (celda.Borders.bottom != PdfBorder.DEFAULT) estilo.SetBorderBottom(borderToPdf(celda.Borders.bottom));
            return estilo;
        }


        /// <summary>
        /// Devuelve una tabla con un título a la izquierda y un logotipo a la derecha.
        /// </summary>
        public Table GetPdfTitleTable(string texto, string logoPath) {
            // Creamos la tabla y le aplicamos el estilo.
            Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }));
            tabla.AddStyle(styTable);
            // Creamos el párrafo de la izquierda y lo añadimos a la tabla
            Paragraph parrafo = new Paragraph(texto).SetFixedLeading(16);
            tabla.AddCell(new Cell().Add(parrafo).AddStyle(styTitle));
            // Añadimos el logo del sindicato a la derecha.
            Image imagen = getImageFromFile(logoPath);
            if (imagen != null) {
                imagen.SetHeight(35);
                imagen.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                tabla.AddCell(new Cell().Add(imagen).AddStyle(styLogo));
            }
            // Devolvemos la tabla.
            return tabla;
        }


        /// <summary>
        /// Devuelve una <see cref="iText.Layout.Element.Table"/> tras generarla usando las propiedades 
        /// del objeto <see cref="PdfTableData"/>.
        /// </summary>
        public Table GetPdfTable(PdfTableData table) {
            // Generamos los estilos
            generateStyles(table);
            // Generamos la tabla con el título y el logo.
            var tablaTitulo = GetPdfTitleTable(table.Title, table.LogoPath);
            // Creamos la tabla a devolver usando los anchos de columna.
            var tabla = new Table(UnitValue.CreatePercentArray(table.ColumnWidths.ToArray()));
            tabla.AddStyle(styTable);
            // Insertamos lá tabla de título
            tabla.AddHeaderCell(new Cell(1, table.ColumnWidths.Count()).Add(tablaTitulo).AddStyle(styTitle));
            // Insertamos los encabezados, si existen.
            int columna = 0;
            if (table.Headers != null && table.Headers.Any()) {
                foreach (var header in table.Headers) {
                    if (!header.IsMerged) {
                        columna += header.MergedCells.cols;
                        var celda = new Cell(header.MergedCells.rows, header.MergedCells.cols);
                        celda.Add(new Paragraph(header.Value?.ToString() ?? string.Empty))
                            .AddStyle(styHeader)
                            .AddStyle(GetPdfStyle(header));
                        if (columna == 1) celda.AddStyle(styHeaderFirst);
                        if (columna == table.Headers.Count()) celda.AddStyle(styHeaderLast);
                        tabla.AddHeaderCell(celda);
                    }
                }
            }
            // Inertamos las celdas, si existen.
            var isFondoAlterno = false;
            int filaAlterna = 1;
            int fila = 1;
            if (table.Data != null && table.Data.Any()) {
                foreach (var row in table.Data) {
                    columna = 0;
                    foreach (var cell in row) {
                        if (!cell.IsMerged) {
                            columna += cell.MergedCells.cols;
                            var celda = new Cell(cell.MergedCells.rows, cell.MergedCells.cols);
                            celda.Add(new Paragraph(cell.Value?.ToString() ?? string.Empty))
                                .AddStyle(styCell)
                                .AddStyle(GetPdfStyle(cell));
                            if (columna == 1) celda.AddStyle(styCellFirst);
                            if (columna == row.Count()) celda.AddStyle(styCellLast);
                            if (isFondoAlterno) celda.AddStyle(styCellAlternate);
                            tabla.AddCell(celda);
                        }
                    }
                    if (filaAlterna == table.AlternateRowsCount) {
                        isFondoAlterno = !isFondoAlterno;
                        filaAlterna = 0;
                    }
                    filaAlterna++;
                    fila++;
                }
            }
            // Insertamos los totales, si existen
            if (table.Totals != null && table.Totals.Any()) {
                columna = 0;
                foreach (var total in table.Totals) {
                    if (!total.IsMerged) {
                        columna += total.MergedCells.cols;
                        var celda = new Cell(total.MergedCells.rows, total.MergedCells.cols);
                        celda.Add(new Paragraph(total.Value?.ToString() ?? string.Empty))
                            .AddStyle(styTotal)
                            .AddStyle(GetPdfStyle(total));
                        if (columna == 1) celda.AddStyle(styTotalFirst);
                        if (columna == table.Totals.Count()) celda.AddStyle(styTotalLast);
                        tabla.AddCell(celda);
                    }
                }
            }

            // Ponemos el borde inferior de la tabla.
            tabla.SetBorderBottom(borderToPdf(table.TableBorder));

            // Devolvemos la tabla
            return tabla;
        }


        /// <summary>
        /// Genera un libro de excel con una hoja en la que se inserta la tabla, usando las propiedades
        /// del objeto <see cref="PdfTableData"/> y lo guarda en la ruta especificada.
        /// </summary>
        public void SaveAsExcel(PdfTableData table, string path, string worksheetName = "Hoja 1") {
            using (var excel = new ExcelPackage()) {
                var hoja = excel.Workbook.Worksheets.Add(worksheetName);

                // Ajustamos el ancho de las columnas. El 100% de la tabla en medida Excel es 165.
                var anchoExcel = 165f;
                var anchoTotal = table.ColumnWidths.Sum();
                if (anchoTotal == 0) return; //TODO: Plantear generar un error aquí, o al menos un aviso.
                int col = 1;
                foreach (var colWidth in table.ColumnWidths) {
                    var ancho = colWidth * anchoExcel / anchoTotal;
                    hoja.Column(col).Width = ancho;
                    col++;
                }
                // Ajustamos los valores por defecto de la hoja.
                hoja.CustomHeight = true;
                hoja.DefaultRowHeight = 20;
                hoja.Row(1).Height = 60;
                var numeroFilas = table.Headers == null ? 0 : 1;
                numeroFilas += table.Data == null ? 0 : table.Data.Count();
                numeroFilas += table.Totals == null ? 0 : 1;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                hoja.Cells[2, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                if (table.Headers != null && table.Headers.Any()) {
                    hoja.Cells[2, 1, 2, table.ColumnWidths.Count()].Style.Border.BorderAround(borderToExcel(table.TableBorder));
                    hoja.Cells[3, 1, table.Data.Count() + 2, table.ColumnWidths.Count()].Style.Border.BorderAround(borderToExcel(table.TableBorder));
                } else {
                    hoja.Cells[2, 1, table.Data.Count() + 1, table.ColumnWidths.Count()].Style.Border.BorderAround(borderToExcel(table.TableBorder));
                }
                if (table.Totals != null && table.Totals.Any()) {
                    hoja.Cells[numeroFilas + 1, 1, numeroFilas + 1, table.ColumnWidths.Count()].Style.Border.BorderAround(borderToExcel(table.TableBorder));
                }
                // Hacemos el merge de la primera fila.
                hoja.Cells[1, 1, 1, Math.Abs(table.ColumnWidths.Count() / 2)].Merge = true;
                // Título
                hoja.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                hoja.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                hoja.Cells["A1"].Style.Font.Size = 20f;
                hoja.Cells["A1"].Style.WrapText = true;
                hoja.Cells["A1"].Value = table.Title;
                // Imagen
                if (!string.IsNullOrEmpty(table.LogoPath)) {
                    var excelImage = hoja.Drawings.AddPicture("Logo", new FileInfo(table.LogoPath));
                    excelImage.ChangeCellAnchor(eEditAs.OneCell);
                    hoja.Cells[1, table.ColumnWidths.Count()].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    col = 0;
                    var ancho = 20d;
                    while (ancho > 0) {
                        ancho -= hoja.Column(table.ColumnWidths.Count() - col).Width;
                        col++;
                    }
                    excelImage.SetPosition(0, 0, table.ColumnWidths.Count() - col, 0);
                    excelImage.SetSize(95);
                }
                // Encabezados si los hay.
                int fila = 2;
                if (table.Headers != null && table.Headers.Any()) {
                    col = 0;
                    foreach (var header in table.Headers) {
                        if (!header.IsMerged) {
                            col++;
                            var celda = hoja.Cells[fila, col];
                            if (header.MergedCells.cols > 1) {
                                hoja.Cells[fila, col, fila, col + header.MergedCells.cols - 1].Merge = true;
                            }
                            col += header.MergedCells.cols - 1;
                            celda.Value = decimal.TryParse(header.Value, out decimal d) ? (object)d : header.Value;
                            celda.Style.Font.Bold = true;
                            celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            celda.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(table.HeadersBackground.R, table.HeadersBackground.G, table.HeadersBackground.B));
                            applyExcelStyleToCell(ref celda, header);
                        }
                    }
                    hoja.Row(fila).Height = 20;
                    fila++;
                }
                // Celdas Si las hay
                var isFondoAlterno = false;
                int filaAlterna = 1;
                if (table.Data != null && table.Data.Any()) {
                    foreach (var row in table.Data) {
                        col = 0;
                        foreach (var cell in row) {
                            if (!cell.IsMerged) {
                                col++;
                                var celda = hoja.Cells[fila, col];
                                if (cell.MergedCells.cols > 1 || cell.MergedCells.rows > 1) {
                                    hoja.Cells[fila, col, fila + cell.MergedCells.rows - 1, col + cell.MergedCells.cols - 1].Merge = true;
                                }
                                col += cell.MergedCells.cols - 1;
                                applyExcelStyleToCell(ref celda, cell);
                                if (isFondoAlterno) {
                                    celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    celda.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(table.AlternateRowsBackground.R, table.AlternateRowsBackground.G, table.AlternateRowsBackground.B));
                                }
                                celda.Value = decimal.TryParse(cell.Value, out decimal d) ? (object)d : cell.Value;
                            }
                        }
                        hoja.Row(fila).Height = 20;
                        if (filaAlterna == table.AlternateRowsCount) {
                            isFondoAlterno = !isFondoAlterno;
                            filaAlterna = 0;
                        }
                        filaAlterna++;
                        fila++;
                    }
                }
                // Totales si los hay.
                if (table.Totals != null && table.Totals.Any()) {
                    col = 0;
                    foreach (var total in table.Totals) {
                        if (!total.IsMerged) {
                            col++;
                            var celda = hoja.Cells[fila, col];
                            if (total.MergedCells.cols > 1) {
                                hoja.Cells[fila, col, fila, col + total.MergedCells.cols - 1].Merge = true;
                            }
                            col += total.MergedCells.cols - 1;
                            celda.Value = decimal.TryParse(total.Value, out decimal d) ? (object)d : total.Value;
                            celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            celda.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(table.HeadersBackground.R, table.HeadersBackground.G, table.HeadersBackground.B));
                            applyExcelStyleToCell(ref celda, total);
                        }
                    }
                    hoja.Row(fila).Height = 20;
                    fila++;
                }
                // Configuración de la página.
                hoja.PrinterSettings.PrintArea = hoja.Cells[1, 1, table.Data.Count() + 2, table.ColumnWidths.Count()];
                hoja.PrinterSettings.PaperSize = ePaperSize.A4;
                hoja.PrinterSettings.Orientation = eOrientation.Landscape;
                hoja.PrinterSettings.LeftMargin = 1 / 2.54m;
                hoja.PrinterSettings.TopMargin = 1 / 2.54m;
                hoja.PrinterSettings.RightMargin = 1 / 2.54m;
                hoja.PrinterSettings.BottomMargin = 1 / 2.54m;
                hoja.PrinterSettings.HorizontalCentered = true;
                hoja.PrinterSettings.RepeatRows = hoja.Cells["1:2"];
                hoja.PrinterSettings.FitToPage = true;
                hoja.PrinterSettings.FitToWidth = 1;
                hoja.PrinterSettings.FitToHeight = 0;


                excel.SaveAs(new FileInfo(path));
            }
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================


        #endregion
        // ====================================================================================================




    }
}
