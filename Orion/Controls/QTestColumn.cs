#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Orion.Models;

namespace Orion.Controls {

    public class QTestColumn : DataGridTextColumn {

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem) {
            var celda = base.GenerateElement(cell, dataItem);
            var header = new TextBlock();
            //var bb = this.Binding as Binding;
            //var b = new Binding { Path = bb.Path, Source = cell.DataContext, Converter = new Convertidores.ConvertidorNumeroGraficoCalendario() };
            //texto.SetBinding(TextBlock.TextProperty, b);

            // Si el dato de la fila es un Calendario y la celda es un TextBlock...
            if (dataItem is Calendario cal && celda is TextBlock txt) {
                // Si la columna tiene un día asociado...
                if (cell.Column.DisplayIndex <= cal.ListaDias.Count) {
                    // Extraemos el Dia del calendario
                    var dia = cal.ListaDias[cell.Column.DisplayIndex - 1];
                    // Definimos el header
                    if (dia.DiaFecha.DayOfWeek == DayOfWeek.Sunday || App.Global.CalendariosVM.EsFestivo(dia.DiaFecha)) {
                        header.Foreground = Brushes.Red;
                    } else if (dia.DiaFecha.DayOfWeek == DayOfWeek.Saturday) {
                        header.Foreground = Brushes.Blue;
                    } else {
                        header.Foreground = Brushes.Black;
                    }
                    header.Text = $"{dia.DiaFecha.Day:00}";
                    // FontWeigh
                    txt.FontWeight = dia.Grafico < 0 || dia.Codigo == 3 ? FontWeights.Bold : FontWeights.Normal;
                    // TextDecorations
                    var decoraciones = new TextDecorationCollection();
                    if (dia.Codigo == 2) decoraciones.Add(TextDecorations.Strikethrough); //Tachado
                    if (dia.HayExtras) decoraciones.Add(TextDecorations.Underline); // Subrayado
                    txt.TextDecorations = decoraciones;
                    // ForeGround
                    var color = Brushes.DimGray;
                    switch (dia.Grafico) {
                        case -1:
                        case -12:
                        case -13:
                        case -15:
                        case -16:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorOV);
                            break;
                        case -2:
                        case -3:
                        case -17:
                        case -18:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorJD);
                            break;
                        case -4:
                        case -10:
                        case -11:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorE);
                            break;
                        case -5:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorDS);
                            break;
                        case -6:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorDC);
                            break;
                        case -7:
                        case -14:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorF6);
                            break;
                        case -8:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorF6);
                            break;
                        case -9:
                            color = new SolidColorBrush(App.Global.Configuracion.ColorJD);
                            break;
                    }
                    switch (dia.DiaFecha.DayOfWeek) {
                        case DayOfWeek.Saturday:
                            color = new SolidColorBrush(Colors.Blue);
                            break;
                        case DayOfWeek.Sunday:
                            color = new SolidColorBrush(Colors.Red);
                            break;
                    }
                    if (App.Global.CalendariosVM.EsFestivo(dia.DiaFecha)) color = new SolidColorBrush(Colors.Red);
                    txt.Foreground = color;
                }
                cell.Column.Header = header;
            }
            return celda;
        }


    }
}
