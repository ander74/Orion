#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Orion.Config;
using Orion.Convertidores;
using Orion.DataModels;
using Orion.Models;
using Orion.Servicios;

namespace Orion.PrintModel {

    public static class GraficosPrintModel {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================
        private static ConvertidorNumeroGrafico cnvNumGrafico = new ConvertidorNumeroGrafico();
        private static ConvertidorHora cnvHora = new ConvertidorHora();
        private static ConvertidorSuperHoraMixta cnvSuperHoraMixta = new ConvertidorSuperHoraMixta();
        private static ConvertidorDecimal cnvDecimal = new ConvertidorDecimal();
        private static ConvertidorDiasF6 cnvDiasF6 = new ConvertidorDiasF6();
        private static ConvertidorColorDia cnvColorDia = new ConvertidorColorDia();

        private static string rutaPlantillaGraficos = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/Graficos.xlsx");
        private static string rutaPlantillaGraficosIndividuales = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/GraficoIndividual.xlsx");
        private static string rutaPlantillaEstadisticasGraficos = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/EstadisticasGraficos.xlsx");
        private static string rutaPlantillaEstadisticasGraficosPorCentros = Utils.CombinarCarpetas(App.RutaInicial, "/Plantillas/EstadisticasGraficosPorCentros.xlsx");
        #endregion


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        private static Table GetTablaGraficos(ListCollectionView listaGraficos, DateTime fecha) {
            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            Style estiloTabla = new Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(8);
            // Estilo titulos
            Style estiloTitulos = new Style();
            estiloTitulos.SetBold()
                         .SetBorder(Border.NO_BORDER)
                         .SetFontSize(14);
            // Estilo de las celdas de encabezado.
            Style estiloEncabezados = new Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(112, 173, 71))
                             .SetBold()
                             .SetFontSize(8);
            // Estilo de las celdas de izq.
            Style estiloIzq = new Style();
            estiloIzq.SetBorderLeft(new SolidBorder(1));
            // Estilo de las celdas de med.
            Style estiloMed = new Style();
            estiloMed.SetBorderTop(new SolidBorder(1))
                     .SetBorderBottom(new SolidBorder(1));
            // Estilo de las celdas de der.
            Style estiloDer = new Style();
            estiloDer.SetBorderRight(new SolidBorder(1));
            // Creamos la tabla
            float[] ancho = new float[] { 3, 3, 3, 3, 3, 3, 3, 3, 2, 3, 3, 3, 3, 3, 3, 3, 3 };
            Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);

            string textoEncabezado = $"GRÁFICOS\n{fecha:dd - MMMM - yyyy} ({App.Global.CentroActual})".ToUpper();
            Table tablaEncabezado = InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado);

            tabla.AddHeaderCell(new Cell(1, 17).Add(tablaEncabezado).AddStyle(estiloTitulos));

            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Número")).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Turno")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Inicio")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Final")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Ini. Partido")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Fin. Partido")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Valoracion")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Dif.")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Plus Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Paquetería")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed));
            // Añadimos las celdas con los calendarios.
            int indice = 1;
            foreach (Object obj in listaGraficos) {
                Grafico g = obj as Grafico;
                if (g == null) continue;
                // Estilo Fondo Alternativo
                Style estiloFondo = new Style().SetBackgroundColor(ColorConstants.WHITE);
                if (indice % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(226, 239, 218));
                indice++;
                // Estilo Valoracion Diferente
                Style estiloValDif = new Style();
                if (g.DiferenciaValoracion.Ticks != 0) estiloValDif.SetFontColor(new DeviceRgb(255, 20, 147));
                // Escribimos el grafico.
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvNumGrafico.Convert(g.Numero, null, null, null)}"))
                                        .AddStyle(estiloIzq).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{g.Turno.ToString("0")}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Inicio, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Final, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.InicioPartido, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.FinalPartido, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Valoracion, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo).AddStyle(estiloValDif));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Trabajadas, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo).AddStyle(estiloValDif));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.DiferenciaValoracion, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo).AddStyle(estiloValDif));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Acumuladas, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(g.Nocturnas, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvDecimal.Convert(g.Desayuno, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvDecimal.Convert(g.Comida, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvDecimal.Convert(g.Cena, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvDecimal.Convert(g.PlusCena, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(g.PlusLimpieza ? "X" : "")}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(g.PlusPaqueteria ? "X" : "")}"))
                                        .AddStyle(estiloDer).AddStyle(estiloFondo));
            }
            // Ponemos los bordes de la tabla.
            tabla.SetBorderBottom(new SolidBorder(1));
            // Devolvemos la tabla.
            return tabla;
        }


        private static Table GetGraficoIndividual(Grafico grafico) {

            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            Style estiloTabla = new Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                       .SetMargins(0, 0, 0, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(8);
            // Estilo de las celdas de encabezado.
            Style estiloEncabezados = new Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(112, 173, 71))
                             .SetBold()
                             .SetFontSize(8);
            // Creamos la tabla
            float[] ancho = new float[] { 10, 10, 60, 10, 10 };
            Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Inicio")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Línea")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Descripción")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Final")).AddStyle(estiloEncabezados));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Tiempo")).AddStyle(estiloEncabezados));
            // Añadimos los datos del gráfico
            int indice = 1;
            foreach (var valoracion in grafico.ListaValoraciones) {
                // Estilo Fondo Alternativo
                Style estiloFondo = new Style().SetBackgroundColor(ColorConstants.WHITE);
                if (indice % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(226, 239, 218));
                indice++;
                // Escribimos el grafico.
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(valoracion.Inicio, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(valoracion.Linea == 0 ? "" : valoracion.Linea.ToString())}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{valoracion.Descripcion}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(valoracion.Final, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{(string)cnvHora.Convert(valoracion.Tiempo, null, VerValores.NoCeros, null)}"))
                                        .AddStyle(estiloFondo));

            }
            // Ponemos los bordes de la tabla.
            tabla.GetHeader().SetBorder(new SolidBorder(1));
            tabla.SetBorder(new SolidBorder(1));
            // Devolvemos la tabla.
            return tabla;

        }


        private static Table GetTablaEstadisticaGraficos(List<EstadisticasGraficos> lista, DateTime fecha) {
            // Nombres de turnos.
            string[] turnos = { "Desconocido", "Mañana", "Tarde", "Noche", "Partido" };
            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            Style estiloTabla = new Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                       .SetMargins(10, 0, 10, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(8);
            // Estilo titulos
            Style estiloTitulos = new Style();
            estiloTitulos.SetBold()
                         .SetBorder(Border.NO_BORDER)
                         .SetFontSize(14);
            // Estilo de las celdas de encabezado.
            Style estiloEncabezados = new Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(112, 173, 71))
                             .SetBold()
                             .SetFontSize(8);
            // Estilo de las celdas de izq.
            Style estiloIzq = new Style();
            estiloIzq.SetBorderLeft(new SolidBorder(1));
            // Estilo de las celdas de med.
            Style estiloMed = new Style();
            estiloMed.SetBorderTop(new SolidBorder(1))
                     .SetBorderBottom(new SolidBorder(1));
            // Estilo de las celdas de der.
            Style estiloDer = new Style();
            estiloDer.SetBorderRight(new SolidBorder(1));
            // Estilo Fondo SubCabecera
            Style estiloFondoSub = new Style().SetBackgroundColor(new DeviceRgb(196, 215, 155));
            // Creamos la tabla
            float[] ancho = new float[] { 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell().Add(new Paragraph(fecha.ToString("dd-MMM-yyyy").Replace(".", "").ToUpper())).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nº Gráficos")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Valoración")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Plus Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Paquetería")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed));
            // Definimos el total de gráficos.
            var totalGraficos = lista.Sum(e => e.NumeroGraficos);
            // Añadimos los cuatro turnos
            for (int turno = 1; turno < 5; turno++) {
                var estadistica = lista.FirstOrDefault(e => e.Turno == turno);
                if (estadistica == null) estadistica = new EstadisticasGraficos { Turno = turno };
                // Estilo Fondo Alternativo
                Style estiloFondo = new Style().SetBackgroundColor(ColorConstants.WHITE);
                if (turno % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(226, 239, 218));
                // Añadimos el turno en texto.
                tabla.AddCell(new Cell().Add(new Paragraph($"{turnos[turno]}")).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloFondoSub));
                // Calculamos el porcentaje del turno.
                var porcentaje = totalGraficos == 0 ? 0.00m : Math.Round(estadistica.NumeroGraficos * 100m / totalGraficos, 2);
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.NumeroGraficos}  {porcentaje}%")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Valoracion.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Trabajadas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Acumuladas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Nocturnas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Desayuno.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Comida.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Cena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.PlusCena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.TotalDietas.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Limpieza.ToString("00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Paqueteria.ToString("00")}")).AddStyle(estiloDer).AddStyle(estiloFondo));
            }
            // Añadimos los totales.
            tabla.AddCell(new Cell().Add(new Paragraph($"TOTAL")).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.NumeroGraficos)}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Valoracion.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Trabajadas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Acumuladas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Nocturnas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Desayuno).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Comida).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Cena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.PlusCena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.TotalDietas).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Limpieza).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Paqueteria).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));

            // Devolvemos la tabla.
            return tabla;
        }


        private static Table GetTablaEstadisticaGraficosPorCentro(List<EstadisticasGraficos> lista) {
            // Nombres de turnos.
            string[] turnos = { "Desconocido", "Mañana", "Tarde", "Noche", "Partido" };
            // Fuente a utilizar en la tabla.
            PdfFont arial = PdfFontFactory.CreateFont("c:/windows/fonts/calibri.ttf", true);
            // Estilo de la tabla.
            Style estiloTabla = new Style();
            estiloTabla.SetTextAlignment(TextAlignment.CENTER)
                       .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                       .SetMargins(10, 0, 10, 0)
                       .SetPaddings(0, 0, 0, 0)
                       .SetWidth(UnitValue.CreatePercentValue(100))
                       .SetFont(arial)
                       .SetFontSize(8);
            // Estilo titulos
            Style estiloTitulos = new Style();
            estiloTitulos.SetBold()
                         .SetBorder(Border.NO_BORDER)
                         .SetFontSize(14);
            // Estilo de las celdas de encabezado.
            Style estiloEncabezados = new Style();
            estiloEncabezados.SetBackgroundColor(new DeviceRgb(170, 62, 26))
                             .SetBold()
                             .SetFontColor(ColorConstants.WHITE)
                             .SetFontSize(8);

            // Estilo de las celdas de izq.
            Style estiloIzq = new Style();
            estiloIzq.SetBorderLeft(new SolidBorder(1));
            // Estilo de las celdas de med.
            Style estiloMed = new Style();
            estiloMed.SetBorderTop(new SolidBorder(1))
                     .SetBorderBottom(new SolidBorder(1));
            // Estilo de las celdas de der.
            Style estiloDer = new Style();
            estiloDer.SetBorderRight(new SolidBorder(1));
            // Estilo Fondo SubCabecera
            Style estiloFondoSub = new Style();
            estiloFondoSub.SetBackgroundColor(new DeviceRgb(236, 152, 123));
            estiloFondoSub.SetFontColor(ColorConstants.BLACK);
            // Estilo Encabezado Vertical
            Style estiloEncabezadoVertical = new Style();
            estiloEncabezadoVertical.SetBorder(new SolidBorder(1));
            estiloEncabezadoVertical.SetFontSize(12);
            estiloEncabezadoVertical.SetBold();
            //estiloEncabezadoVertical.SetRotationAngle(Math.PI / 2);
            estiloEncabezadoVertical.SetPadding(5);
            estiloEncabezadoVertical.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            //estiloEncabezadoVertical.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            // Creamos la tabla
            float[] ancho = new float[] { 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
            Table tabla = new Table(UnitValue.CreatePercentArray(ancho));
            // Asignamos el estilo a la tabla.
            tabla.AddStyle(estiloTabla);
            // Añadimos las celdas de encabezado.
            tabla.AddHeaderCell(new Cell(1, 2).Add(new Paragraph("")).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nº Gráficos")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Valoración")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Trabajadas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Acumuladas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Nocturnas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Desayuno")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Comida")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Plus Cena")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Total Dietas")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Limpieza")).AddStyle(estiloEncabezados).AddStyle(estiloMed));
            tabla.AddHeaderCell(new Cell().Add(new Paragraph("Paquetería")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed));

            // LLenamos los centros
            AddEstadisticaCentroToTable(lista.Where(e => e.Centro == Centros.Bilbao).ToList(), tabla);
            AddEstadisticaCentroToTable(lista.Where(e => e.Centro == Centros.Donosti).ToList(), tabla);
            AddEstadisticaCentroToTable(lista.Where(e => e.Centro == Centros.Arrasate).ToList(), tabla);
            AddEstadisticaCentroToTable(lista.Where(e => e.Centro == Centros.Vitoria).ToList(), tabla);

            // TOTALES
            // Definimos el total de gráficos
            var totalGraficosBIO = lista.Sum(e => e.NumeroGraficos);
            // Ponemos la fecha y el centro en la primera fila.
            tabla.AddCell(new Cell(5, 1).Add(new Paragraph($"TOTALES")).AddStyle(estiloEncabezadoVertical).AddStyle(estiloFondoSub));
            // Añadimos los cuatro turnos
            for (int turno = 1; turno < 5; turno++) {
                var estadistica = new EstadisticasGraficos { Turno = turno };
                // Llenar la estadistica del turno con los datos.
                estadistica.NumeroGraficos = lista.Where(e => e.Turno == turno).Sum(e => e.NumeroGraficos);
                estadistica.Valoracion = new TimeSpan(lista.Where(e => e.Turno == turno).Sum(e => e.Valoracion.Ticks));
                estadistica.Trabajadas = new TimeSpan(lista.Where(e => e.Turno == turno).Sum(e => e.Trabajadas.Ticks));
                estadistica.Acumuladas = new TimeSpan(lista.Where(e => e.Turno == turno).Sum(e => e.Acumuladas.Ticks));
                estadistica.Nocturnas = new TimeSpan(lista.Where(e => e.Turno == turno).Sum(e => e.Nocturnas.Ticks));
                estadistica.Desayuno = lista.Where(e => e.Turno == turno).Sum(e => e.Desayuno);
                estadistica.Comida = lista.Where(e => e.Turno == turno).Sum(e => e.Desayuno);
                estadistica.Cena = lista.Where(e => e.Turno == turno).Sum(e => e.Desayuno);
                estadistica.PlusCena = lista.Where(e => e.Turno == turno).Sum(e => e.Desayuno);
                estadistica.Limpieza = lista.Where(e => e.Turno == turno).Sum(e => e.Limpieza);
                estadistica.Paqueteria = lista.Where(e => e.Turno == turno).Sum(e => e.Paqueteria);
                // Estilo Fondo Alternativo
                Style estiloFondo = new Style().SetBackgroundColor(ColorConstants.WHITE);
                if (turno % 2 == 0) estiloFondo.SetBackgroundColor(new DeviceRgb(249, 220, 210));
                // Añadimos el turno en texto.
                tabla.AddCell(new Cell().Add(new Paragraph($"{turnos[turno]}")).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloFondoSub));
                // Calculamos el porcentaje del turno.
                var porcentaje = totalGraficosBIO == 0 ? 0.00m : Math.Round(estadistica.NumeroGraficos * 100m / totalGraficosBIO, 2);
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.NumeroGraficos}  {porcentaje}%")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Valoracion.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Trabajadas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Acumuladas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Nocturnas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Desayuno.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Comida.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Cena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.PlusCena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.TotalDietas.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Limpieza.ToString("00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Paqueteria.ToString("00")}")).AddStyle(estiloDer).AddStyle(estiloFondo));
            }
            // Añadimos los totales.
            tabla.AddCell(new Cell().Add(new Paragraph($"TOTAL")).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.NumeroGraficos)}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Valoracion.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Trabajadas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Acumuladas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Nocturnas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Desayuno).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Comida).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Cena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.PlusCena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.TotalDietas).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Limpieza).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Paqueteria).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));

            // Devolvemos la tabla
            return tabla;
        }


        private static void AddEstadisticaCentroToTable(List<EstadisticasGraficos> lista, Table tabla) {
            // Nombres de turnos.
            string[] turnos = { "Desconocido", "Mañana", "Tarde", "Noche", "Partido" };
            // Definimos el centro
            var centro = lista.FirstOrDefault()?.Centro ?? Centros.Desconocido;
            // Definimos los fondos.
            var fondoEncabezado = new DeviceRgb(170, 62, 26);
            var fondoSubCabecera = new DeviceRgb(236, 152, 123);
            var fondoAlterno = new DeviceRgb(249, 220, 210);
            // Definimos los fondos en función del centro
            switch (centro) {
                case Centros.Bilbao:
                    fondoEncabezado = new DeviceRgb(123, 137, 29);
                    fondoSubCabecera = new DeviceRgb(210, 224, 112);
                    fondoAlterno = new DeviceRgb(240, 244, 206);
                    break;
                case Centros.Donosti:
                    fondoEncabezado = new DeviceRgb(185, 111, 0);
                    fondoSubCabecera = new DeviceRgb(255, 191, 96);
                    fondoAlterno = new DeviceRgb(255, 234, 202);
                    break;
                case Centros.Arrasate:
                    fondoEncabezado = new DeviceRgb(49, 104, 134);
                    fondoSubCabecera = new DeviceRgb(138, 187, 213);
                    fondoAlterno = new DeviceRgb(216, 233, 241);
                    break;
                case Centros.Vitoria:
                    fondoEncabezado = new DeviceRgb(192, 145, 1);
                    fondoSubCabecera = new DeviceRgb(254, 217, 105);
                    fondoAlterno = new DeviceRgb(255, 242, 204);
                    break;
            }
            // Estilo de las celdas de encabezado.
            Style estiloEncabezados = new Style();
            estiloEncabezados.SetBackgroundColor(fondoEncabezado)
                             .SetBold()
                             .SetFontSize(8);
            // Estilo de las celdas de izq.
            Style estiloIzq = new Style();
            estiloIzq.SetBorderLeft(new SolidBorder(1));
            // Estilo de las celdas de med.
            Style estiloMed = new Style();
            estiloMed.SetBorderTop(new SolidBorder(1))
                     .SetBorderBottom(new SolidBorder(1));
            // Estilo de las celdas de der.
            Style estiloDer = new Style();
            estiloDer.SetBorderRight(new SolidBorder(1));
            // Estilo Fondo SubCabecera
            Style estiloFondoSub = new Style().SetBackgroundColor(fondoSubCabecera);
            // Estilo Encabezado Vertical
            Style estiloEncabezadoVertical = new Style();
            estiloEncabezadoVertical.SetBorder(new SolidBorder(1));
            estiloEncabezadoVertical.SetFontSize(12);
            estiloEncabezadoVertical.SetBold();
            estiloEncabezadoVertical.SetVerticalAlignment(VerticalAlignment.MIDDLE);
            //estiloEncabezadoVertical.SetRotationAngle(Math.PI / 2);
            estiloEncabezadoVertical.SetPadding(5);
            // Definimos el total de gráficos
            var totalGraficosBIO = lista.Sum(e => e.NumeroGraficos);
            // Ponemos la fecha y el centro en la primera fila.
            tabla.AddCell(new Cell(5, 1).Add(new Paragraph($"{centro}\n{lista.FirstOrDefault()?.Validez.ToString("dd-MM-yyyy")}".ToUpper())).AddStyle(estiloEncabezadoVertical).AddStyle(estiloFondoSub));
            // Añadimos los cuatro turnos
            for (int turno = 1; turno < 5; turno++) {
                var estadistica = lista.FirstOrDefault(e => e.Turno == turno);
                if (estadistica == null) estadistica = new EstadisticasGraficos { Turno = turno };
                // Estilo Fondo Alternativo
                Style estiloFondo = new Style().SetBackgroundColor(ColorConstants.WHITE);
                if (turno % 2 == 0) estiloFondo.SetBackgroundColor(fondoAlterno);
                // Añadimos el turno en texto.
                tabla.AddCell(new Cell().Add(new Paragraph($"{turnos[turno]}")).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloFondoSub));
                // Calculamos el porcentaje del turno.
                var porcentaje = totalGraficosBIO == 0 ? 0.00m : Math.Round(estadistica.NumeroGraficos * 100m / totalGraficosBIO, 2);
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.NumeroGraficos}  {porcentaje}%")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Valoracion.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Trabajadas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Acumuladas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Nocturnas.ToTexto(true)}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Desayuno.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Comida.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Cena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.PlusCena.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.TotalDietas.ToString("0.00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Limpieza.ToString("00")}")).AddStyle(estiloFondo));
                tabla.AddCell(new Cell().Add(new Paragraph($"{estadistica.Paqueteria.ToString("00")}")).AddStyle(estiloDer).AddStyle(estiloFondo));
            }
            // Añadimos los totales.
            tabla.AddCell(new Cell().Add(new Paragraph($"TOTAL")).AddStyle(estiloEncabezados).AddStyle(estiloIzq).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.NumeroGraficos)}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Valoracion.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Trabajadas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Acumuladas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{new TimeSpan(lista.Sum(e => e.Nocturnas.Ticks)).ToTexto(true)}")).AddStyle(estiloMed).AddStyle(estiloEncabezados).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Desayuno).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Comida).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Cena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.PlusCena).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.TotalDietas).ToString("0.00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Limpieza).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloMed).AddStyle(estiloFondoSub));
            tabla.AddCell(new Cell().Add(new Paragraph($"{lista.Sum(e => e.Paqueteria).ToString("00")}")).AddStyle(estiloEncabezados).AddStyle(estiloDer).AddStyle(estiloMed).AddStyle(estiloFondoSub));
        }

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        public static async Task CrearGraficosEnPdf(Document doc, ListCollectionView lista, DateTime fecha) {

            await Task.Run(() => {
                doc.Add(GetTablaGraficos(lista, fecha));
            });
        }


        public static async Task CrearGraficosIndividualesEnPdf(Document doc, ListCollectionView lista, DateTime fecha) {

            await Task.Run(() => {

                int indice = 1;
                double num = 1;
                foreach (object obj in lista) {
                    double valor = num / lista.Count * 100;
                    App.Global.ValorBarraProgreso = valor;
                    num++;
                    Grafico g = obj as Grafico;
                    if (g == null) continue;
                    if (indice > 1) doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                    // Creamos la tabla de título
                    //Table tablaTitulo = PdfTools.GetTablaTitulo($"{fecha:dd - MMMM - yyyy}".ToUpper(), App.Global.CentroActual.ToString().ToUpper());
                    string textoTitulo = $"{fecha:dd - MMMM - yyyy}\n{App.Global.CentroActual}".ToUpper();
                    Style estiloTitulo = new Style();
                    estiloTitulo.SetFontSize(12).SetBold();
                    estiloTitulo.SetMargins(0, 0, 6, 0);
                    estiloTitulo.SetPadding(0);
                    estiloTitulo.SetWidth(UnitValue.CreatePercentValue(100));
                    estiloTitulo.SetBorder(Border.NO_BORDER);
                    Table tablaTitulo = InformesServicio.GetTablaEncabezadoSindicato(textoTitulo, estiloTitulo);
                    // Creamos la tabla de informacion
                    string textoGrafico = "Gráfico: " + (string)cnvNumGrafico.Convert(g.Numero, null, null, null) + "   Turno: " + g.Turno.ToString("0");
                    string textoValoracion = "Valoración: " + (string)cnvHora.Convert(g.Valoracion, null, VerValores.NoCeros, null);
                    Table tablaInformacion = PdfTools.GetTablaTitulo(textoGrafico, textoValoracion);
                    tablaInformacion.SetWidth(UnitValue.CreatePercentValue(100));
                    tablaInformacion.SetFontSize(9).SetBold();
                    // Creamos la tabla de valoraciones
                    Table tablaValoraciones = GetGraficoIndividual(g);
                    // Creamos la tabla de valores
                    string textoValores = "";
                    if (g.Desayuno > 0) textoValores += String.Format("Desayuno: {0:0.00}  ", g.Desayuno);
                    if (g.Comida > 0) textoValores += String.Format("Comida: {0:0.00}  ", g.Comida);
                    if (g.Cena > 0) textoValores += String.Format("Cena: {0:0.00}  ", g.Cena);
                    if (g.PlusCena > 0) textoValores += String.Format("Plus Cena: {0:0.00}  ", g.PlusCena);
                    if (g.PlusLimpieza) textoValores += "Limp.  ";
                    if (g.PlusPaqueteria) textoValores += "Paqu.  ";
                    string textoHoras = $"Trab: {(string)cnvHora.Convert(g.Trabajadas, null, null, null)}  " +
                                        $"Acum: {(string)cnvHora.Convert(g.Acumuladas, null, null, null)}  " +
                                        $"Noct: {(string)cnvHora.Convert(g.Nocturnas, null, null, null)}";
                    Table tablaValores = PdfTools.GetTablaTitulo(textoValores, textoHoras);
                    tablaValores.SetWidth(UnitValue.CreatePercentValue(100));
                    tablaValores.SetFontSize(9).SetBold();

                    // Añadimos las tablas al documento
                    doc.Add(tablaTitulo);
                    doc.Add(tablaInformacion);
                    doc.Add(tablaValoraciones);
                    doc.Add(tablaValores);
                    indice++;
                }





            });
        }


        public static async Task CrearEstadisticasGraficosEnPdf(Document doc, DateTime fecha) {
            await Task.Run(() => {
                List<EstadisticasGraficos> lista = App.Global.Repository.GetEstadisticasGrupoGraficos(fecha, App.Global.Convenio.JornadaMedia).ToList();
                string textoEncabezado = $"ESTADÍSTICAS GRÁFICOS\n{App.Global.CentroActual}".ToUpper();
                doc.Add(InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado));
                doc.Add(GetTablaEstadisticaGraficos(lista, fecha));
            });
        }


        public static async Task CrearEstadisticasGruposGraficosEnPdf(Document doc, DateTime fecha) {
            await Task.Run(() => {
                // Definimos y llenamos la lista de estadisticas.
                List<EstadisticasGraficos> lista = BdGraficos.GetEstadisticasGraficosDesdeFecha(fecha);
                // Creamos el encabezado.
                string textoEncabezado = $"ESTADÍSTICAS GRÁFICOS\n{App.Global.CentroActual}".ToUpper();
                doc.Add(InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado));
                // Por cada fecha, añadimos una tabla.
                int indice = 0;
                double num = 1;
                foreach (var estadistica in lista.GroupBy(e => e.Validez).Select(g => g.First())) {
                    double valor = num / lista.Count * 100;
                    App.Global.ValorBarraProgreso = valor;
                    num++;
                    // Si hay 3 tablas añadidas, añadimos salto de página y encabezado.
                    if (indice > 0 && indice % 3 == 0) {
                        doc.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                        doc.Add(InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado));
                    }
                    doc.Add(GetTablaEstadisticaGraficos(lista.Where(e => e.Validez == estadistica.Validez).ToList(), estadistica.Validez));
                    indice++;
                }
            });
        }


        public static async Task CrearEstadisticasGraficosPorCentros(Document doc, List<EstadisticasGraficos> lista) {
            await Task.Run(() => {
                // Creamos el encabezado.
                string textoEncabezado = $"ESTADÍSTICAS GRÁFICOS\nTODOS LOS CENTROS";
                doc.Add(InformesServicio.GetTablaEncabezadoSindicato(textoEncabezado));
                // Añadimos la tabla al documento.
                doc.Add(GetTablaEstadisticaGraficosPorCentro(lista));
            });
        }

        #endregion
        // ====================================================================================================


    }

}
