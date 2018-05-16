using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

				// PROGRAMA
				if (!CopiarArchivoRaiz("Orion.exe")) {
					GenerarError();
					return;
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

				//TODO: Eliminar.
				if (!CopiarArchivoRaiz("itextsharp.dll")) {
					GenerarError();
					return;
				}
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
                // Las siguientes son para Live Charts
                if (!CopiarArchivoRaiz("LiveCharts.dll")) {
                    GenerarError();
                    return;
                }
                if (!CopiarArchivoRaiz("LiveCharts.Wpf.dll")) {
                    GenerarError();
                    return;
                }


                // CONFIGURACION
                if (!CopiarArchivoRaiz("Orion.exe.config")) {
					GenerarError();
					return;
				}

				// PLANTILLAS
				if (!CopiarArchivoRaiz("Plantillas\\Calendarios.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\EstadisticasGraficos.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\EstadisticasGraficosPorCentros.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\FallosCalendarios.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\GraficoIndividual.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\Graficos.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\Pijama.xlsx")) {
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\Reclamacion.xlsx")) {//TODO: Eliminar este archivo cuando esté correcta la reclamación PDF
					GenerarError();
					return;
				}
				if (!CopiarArchivoRaiz("Plantillas\\Reclamacion.pdf")) {
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



	}
}
