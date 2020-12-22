#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Orion.Views;

namespace Orion.Config {

    public static class Utils {



        /// <summary>
        /// Lee el texto almacenado en el portapapeles y lo introduce en una lista de arrays de cadenas.
        /// De esta manera, se crea una tabla de valores de texto en la que la lista tiene las filas y el
        /// Array tiene las columnas.
        /// </summary>
        /// <returns>Lista de Arrays de cadenas.</returns>
        public static List<string[]> parseClipboard() {
            if (!Clipboard.ContainsText()) return null;
            List<string[]> lista = new List<string[]>();

            string[] datos = Clipboard.GetText().Split("\n".ToCharArray());

            foreach (string texto in datos) {
                if (String.IsNullOrEmpty(texto)) continue;
                string[] fila = texto.Replace("\n", "").Replace("\r", "").Split("\t".ToCharArray());
                lista.Add(fila);
            }
            return lista;
        }


        /// <summary>
        /// Convierte una hora en texto con formato "hh:mm" en un TimeSpan dado.
        /// </summary>
        /// <param name="texto">Hora en formato hh:mm a convertir.</param>
        /// <param name="hora">TimeSpan en el que se almacenará la hora</param>
        /// <param name="esPlus">Si es true, permite poner horas entre 0 y 29. Si es false, solo permite horas entre 0 y 23.</param>
        /// <returns>TimeSpan pasado con la hora que hay en el texto si es una hora válida o el TimeSpan pasado tal cual.</returns>
        public static bool ParseHora(string texto, out TimeSpan hora, bool esPlus = false) {
            texto = texto.Replace(".", ":").Replace(",", ":");
            if (esPlus) {
                texto = texto.Replace("24:", "1.00:");
                texto = texto.Replace("25:", "1.01:");
                texto = texto.Replace("26:", "1.02:");
                texto = texto.Replace("27:", "1.03:");
                texto = texto.Replace("28:", "1.04:");
                texto = texto.Replace("29:", "1.05:");
            }
            return TimeSpan.TryParse(texto, out hora);
        }


        //====================================================================================================
        // VER ERROR
        //====================================================================================================
        public static void VerError(string localizacion, Exception ex) {

            VentanaError ventanaerror = new VentanaError();

            ventanaerror.TbLocalizacion.Text = localizacion + " -- " + ex.Source;
            ventanaerror.TbDescripcion.Text = ex.Message + "\n\n--- TRAZADO DEL ERROR ---\n\n" + ex.StackTrace;

            ventanaerror.ShowDialog();
        }


        //====================================================================================================
        // COMBINAR CARPETAS 
        //====================================================================================================
        public static string CombinarCarpetas(string carpeta1, string carpeta2) {
            // Definimos la carpeta inicial
            string archivo = carpeta1.Replace("\\", "/") + "/" + carpeta2;
            archivo = archivo.Replace("///", "/");
            archivo = archivo.Replace("//", "/");
            return archivo;
        }


        //====================================================================================================
        #region SEGURIDAD EN CADENAS DE TEXTO
        //====================================================================================================

        private static string entropia = "Orion";


        /// <summary>
        /// Devuelve el texto pasado codificado.
        /// </summary>
        public static string CodificaTexto(string texto) {

            byte[] Llave; //Arreglo donde guardaremos la llave para el cifrado 3DES.

            byte[] caracteres = UTF8Encoding.UTF8.GetBytes(texto); //Arreglo donde guardaremos la cadena descifrada.

            // Ciframos utilizando el Algoritmo MD5.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            Llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(entropia));
            md5.Clear();

            //Ciframos utilizando el Algoritmo 3DES.
            TripleDESCryptoServiceProvider codificador = new TripleDESCryptoServiceProvider();
            codificador.Key = Llave;
            codificador.Mode = CipherMode.ECB;
            codificador.Padding = PaddingMode.PKCS7;
            ICryptoTransform encriptador = codificador.CreateEncryptor(); // Iniciamos la conversión de la cadena
            byte[] resultado = encriptador.TransformFinalBlock(caracteres, 0, caracteres.Length); //Arreglo de bytes donde guardaremos la cadena cifrada.
            codificador.Clear();

            return Convert.ToBase64String(resultado, 0, resultado.Length); // Convertimos la cadena y la regresamos.
        }


        /// <summary>
        /// Devuelve el texto pasado descodificado
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        public static string DescodificaTexto(string texto) {

            byte[] llave;

            byte[] caracteres = Convert.FromBase64String(texto); // Arreglo donde guardaremos la cadena descovertida.

            // Ciframos utilizando el Algoritmo MD5.
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            llave = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(entropia));
            md5.Clear();

            //Ciframos utilizando el Algoritmo 3DES.
            TripleDESCryptoServiceProvider codificador = new TripleDESCryptoServiceProvider();
            codificador.Key = llave;
            codificador.Mode = CipherMode.ECB;
            codificador.Padding = PaddingMode.PKCS7;
            ICryptoTransform encriptador = codificador.CreateDecryptor();
            byte[] resultado = encriptador.TransformFinalBlock(caracteres, 0, caracteres.Length);
            codificador.Clear();

            string texto_descifrado = UTF8Encoding.UTF8.GetString(resultado); // Obtenemos la cadena
            return texto_descifrado; // Devolvemos la cadena
        }




        #endregion
        //====================================================================================================


        // ====================================================================================================
        #region INCIDENCIAS
        // ====================================================================================================

        public static (int Grafico, int Codigo) GetCodigoIncidencia(string texto) {
            (int Grafico, int Codigo) resultado = (0, 0);
            texto = texto.ToLower();
            if (texto.StartsWith("co-")) {
                resultado.Codigo = 1;
                texto = texto.Substring(3);
            } else if (texto.StartsWith("ce-")) {
                resultado.Codigo = 2;
                texto = texto.Substring(3);
            } else if (texto.StartsWith("jd-")) {
                resultado.Codigo = 3;
                texto = texto.Substring(3);
            }
            texto = texto.Replace("-", "");
            switch (texto) {
                case "ov": resultado.Grafico = -1; break;
                case "jd": resultado.Grafico = -2; break;
                case "fn": resultado.Grafico = -3; break;
                case "e": case "ge": resultado.Grafico = -4; break;
                case "ds": resultado.Grafico = -5; break;
                case "dc": case "oh": case "dh": resultado.Grafico = -6; break;
                case "f6": case "f4": resultado.Grafico = -7; break;
                case "dnd": case "df": resultado.Grafico = -8; break;
                case "per": resultado.Grafico = -9; break;
                case "ejd": case "e(jd)": resultado.Grafico = -10; break;
                case "efn": case "e(fn)": resultado.Grafico = -11; break;
                case "ovjd": case "ov(jd)": resultado.Grafico = -12; break;
                case "ovfn": case "ov(fn)": resultado.Grafico = -13; break;
                case "f6dc": case "dcf6": resultado.Grafico = -14; break;
                case "for": case "cap": resultado.Grafico = -15; break;
                case "ova": resultado.Grafico = -16; break;
                case "ovajd": case "ova(jd)": resultado.Grafico = -17; break;
                case "ovafn": case "ova(fn)": resultado.Grafico = -18; break;
                case "lac": resultado.Grafico = -19; break;
                default: Int32.TryParse(texto, out resultado.Grafico); break;
            }
            return resultado;
        }

        #endregion
        // ====================================================================================================



    } //Final de clase.
}
