using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace OrionUpdate {
    class Actualizar {

        private static string rutaOrigen = string.Empty;
        private static string rutaDestino = string.Empty;

        static void Main(string[] args) {

            string x;

            try {
                // Iniciamos la aplicación mostrando un mensaje de bienvenida.
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("  ORION 1.0");
                Console.WriteLine("  =========");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("    Actualización de la aplicación.");
                Console.WriteLine();
                Console.WriteLine("    Vas a actualizar Orion a la última versión estable.");
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("    Pulsa una tecla para continuar.");
                Console.Write("    ");
                x = Console.ReadLine();

                // Si no hay argumentos, salimos.
                if (args.Length < 1) {
                    GenerarError();
                    return;
                }

                // Cogemos la ruta de los archivos actualizados de los argumentos.
                rutaOrigen = args[0];
                rutaDestino = Directory.GetCurrentDirectory();

                // Si la ruta de Origen no existe, salimos.
                if (!Directory.Exists(rutaOrigen)) {
                    GenerarError();
                    return;
                }

                // Si las carpetas x86 y x64 no existen, las creamos...
                string x86 = CombinarYCrearCarpeta(rutaDestino, "x86");
                string x64 = CombinarYCrearCarpeta(rutaDestino, "x64");

                // MICROSOFT.DATA.SQLITE
                // Si las carpetas runtime y demás no existen, las creamos...
                //string runtime = CombinarYCrearCarpeta(rutaDestino, "runtimes");
                //string winArm = CombinarYCrearCarpeta(runtime, "win-arm");
                //string winx64 = CombinarYCrearCarpeta(runtime, "win-x64");
                //string winx86 = CombinarYCrearCarpeta(runtime, "win-x86");
                //string nativeArm = CombinarYCrearCarpeta(winArm, "native");
                //string nativex64 = CombinarYCrearCarpeta(winx64, "native");
                //string nativex86 = CombinarYCrearCarpeta(winx86, "native");

                // ELIMINAMOS LOS ARCHIVOS ANTERIORES.
                DirectoryInfo info = new DirectoryInfo(rutaDestino);
                foreach (var archivo in info.GetFiles()) {
                    if (archivo.Name != "OrionUpdate.exe") archivo.Delete();
                }
                info = new DirectoryInfo(x86);
                foreach (var archivo in info.GetFiles()) {
                    archivo.Delete();
                }
                info = new DirectoryInfo(x64);
                foreach (var archivo in info.GetFiles()) {
                    archivo.Delete();
                }


                // BIBLIOTECAS
                if (!CopiarArchivoRaiz("Xceed.Wpf.Toolkit.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("System.ValueTuple.dll")) {
                    GenerarError();
                    return;
                }

                if (!CopiarArchivoRaiz("Newtonsoft.Json.dll")) {
                    GenerarError();
                    return;
                }

                //if (!CopiarArchivoRaiz("System.Buffers.dll")) {
                //    GenerarError();
                //    return;
                //}

                //if (!CopiarArchivoRaiz("System.Memory.dll")) {
                //    GenerarError();
                //    return;
                //}

                //if (!CopiarArchivoRaiz("System.Numerics.Vectors.dll")) {
                //    GenerarError();
                //    return;
                //}

                //if (!CopiarArchivoRaiz("System.Runtime.CompilerServices.Unsafe.dll")) {
                //    GenerarError();
                //    return;
                //}


                // Las siguientes son para iText7
                if (!CopiarArchivoRaiz("BouncyCastle.Crypto.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("Common.Logging.Core.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("Common.Logging.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.barcodes.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.forms.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.io.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.kernel.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.layout.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.pdfa.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.sign.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.styledxmlparser.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("itext.svg.dll")) {
                    GenerarError();
                    return;
                }
                // Las siguientes son para Live Charts
                if (!CopiarArchivoRaiz("LiveCharts.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("LiveCharts.Wpf.dll")) {
                    GenerarError();
                    return;
                }
                // Las siguientes son para EPPlus (excel)
                if (!CopiarArchivoRaiz("EPPlus.dll")) {
                    GenerarError();
                    return;
                }

                // SYSTEM.DATA.SQLITE
                // Las siguientes son para Entity Framework
                if (!CopiarArchivoRaiz("EntityFramework.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("EntityFramework.SqlServer.dll")) {
                    GenerarError();
                    return;
                }


                // SYSTEM.DATA.SQLITE
                // Las siguientes son para SQLite
                if (!CopiarArchivoRaiz("System.Data.SQLite.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("System.Data.SQLite.EF6.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("System.Data.SQLite.Linq.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("x64\\SQLite.Interop.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("x86\\SQLite.Interop.dll")) {
                    GenerarError();
                    return;
                }


                // MICROSOFT.DATA.SQLITE
                // Las siguientes son para SQLite
                //if (!CopiarArchivoRaiz("e_sqlite3.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Microsoft.Data.Sqlite.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("SQLitePCLRaw.batteries_v2.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("SQLitePCLRaw.core.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("SQLitePCLRaw.nativelibrary.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("SQLitePCLRaw.provider.dynamic_cdecl.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("runtimes\\win-arm\\native\\e_sqlite3.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("runtimes\\win-x64\\native\\e_sqlite3.dll")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("runtimes\\win-x86\\native\\e_sqlite3.dll")) {
                //    GenerarError();
                //    return;
                //}




                // CONFIGURACION
                if (!CopiarArchivoRaiz("Orion.exe.config")) {
                    GenerarError();
                    return;
                }

                //// PLANTILLAS
                //if (!CopiarArchivoRaiz("Plantillas\\Calendarios.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\EstadisticasGraficos.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\EstadisticasGraficosPorCentros.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\FallosCalendarios.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\GraficoIndividual.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\Graficos.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\Pijama.xlsx")) {
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\Reclamacion.xlsx")) {//TODO: Eliminar este archivo cuando esté correcta la reclamación PDF
                //    GenerarError();
                //    return;
                //}
                //if (!CopiarArchivoRaiz("Plantillas\\Reclamacion.pdf")) {
                //    GenerarError();
                //    return;
                //}

                // PROGRAMA
                if (!CopiarArchivoRaiz("Orion.exe")) {
                    GenerarError();
                    return;
                }




                // Si todo ha ido bien, se informa y se finaliza.
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("      Actualizado con éxito");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("    Pulsa una tecla para finalizar.");
                Console.Write("      ");
                x = Console.ReadLine();

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                x = Console.ReadLine();
            }

            Process.Start(@"Orion.exe");
        }




        static void GenerarError() {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("      Se ha producido un error al actualizar.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("      Pongase en contacto con el administrador de la aplicación.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("    Pulsa una tecla para finalizar.");
            Console.Write("    ");
            string x = Console.ReadLine();
        }




        static bool CopiarArchivoRaiz(string archivo) {

            if (string.IsNullOrWhiteSpace(rutaOrigen) || string.IsNullOrWhiteSpace(rutaDestino)) return false;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"      Actualizando {archivo}.... ");
            string archivoOrigen = Path.Combine(rutaOrigen, archivo);
            string archivoDestino = Path.Combine(rutaDestino, archivo);
            try {
                File.Copy(archivoOrigen, archivoDestino, true);
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR => {ex.Message}");
                return false;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Thread.Sleep(500);
            Console.WriteLine("OK");
            return true;
        }


        static string CombinarYCrearCarpeta(string carpeta1, string carpeta2) {
            string carpeta = Path.Combine(carpeta1, carpeta2);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            return carpeta;
        }


    }
}
