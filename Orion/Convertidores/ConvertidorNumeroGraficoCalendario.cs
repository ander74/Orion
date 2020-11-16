﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Globalization;
using System.Windows.Data;

namespace Orion.Convertidores {



    [ValueConversion(typeof(Tuple<int, int>), typeof(string))]
    class ConvertidorNumeroGraficoCalendario : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value != null && value is Tuple<int, int>) {
                Tuple<int, int> comboGrafico = value as Tuple<int, int>;
                if (comboGrafico.Item1 == 0) return "";
                if (comboGrafico.Item1 > 0) return comboGrafico.Item1.ToString("0000");
                switch (comboGrafico.Item1) {
                    case -1: return "O-V";
                    case -2: return "J-D";
                    case -3: return "FN";
                    case -4: return "E";
                    case -5: return "DS";
                    case -6: return "DC";
                    case -7: return "F6";
                    case -8: return "DND";
                    case -9: return "PER";
                    case -10: return "E(JD)";
                    case -11: return "E(FN)";
                    case -12: return "OV(JD)";
                    case -13: return "OV(FN)";
                    case -14: return "F6(DC)";
                    case -15: return "FOR";
                    case -16: return "OVA";
                    case -17: return "OVA(JD)";
                    case -18: return "OVA(FN)";
                }
            }
            return "";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            int grafico = 0;
            int codigo = 0;
            if (value != null && value is string) {

                string[] texto = ((string)value).ToLower().Replace("-", "").Split('.');

                switch (texto[0]) {
                    // Gráficos
                    case "ov": grafico = -1; break;
                    case "jd": grafico = -2; break;
                    case "fn": grafico = -3; break;
                    case "e": case "ge": grafico = -4; break;
                    case "ds": grafico = -5; break;
                    case "dc": case "oh": case "dh": grafico = -6; break;
                    case "f6": case "f4": grafico = -7; break;
                    case "dnd": case "df": grafico = -8; break;
                    case "per": grafico = -9; break;
                    case "ejd": case "e(jd)": grafico = -10; break;
                    case "efn": case "e(fn)": grafico = -11; break;
                    case "ovjd": case "ov(jd)": grafico = -12; break;
                    case "ovfn": case "ov(fn)": grafico = -13; break;
                    case "f6dc": case "dcf6": grafico = -14; break;
                    case "for": case "cap": grafico = -15; break;
                    case "ova": grafico = -16; break;
                    case "ovajd": case "ova(jd)": grafico = -17; break;
                    case "ovafn": case "ova(fn)": grafico = -18; break;
                    // Códigos
                    case "co": codigo = 1; break;
                    case "ce": codigo = 2; break;
                    default: Int32.TryParse(texto[0], out grafico); break;
                }

                if (texto.GetUpperBound(0) > 0) {
                    switch (texto[1]) {
                        case "co": codigo = 1; break;
                        case "ce": codigo = 2; break;
                        case "jd": case "d": codigo = 3; break;
                        default: Int32.TryParse(texto[1], out codigo); break;
                    }
                }
            }
            return new Tuple<int, int>(grafico, codigo);
        }


    } // Final de clase.
}
