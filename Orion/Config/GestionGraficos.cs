#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Orion.Config {

	public static class GestionGraficos {

		/// <summary>
		/// Enumera los tipos de valoracion posibles.
		///		Vacio = No contiene ningún caracter de texto.
		///		Completo = Tiene todos los elementos. (Hora, Línea, Código, Descripción, Hora, Tiempo)
		///		Parcial = Igual que completo, sin la línea. (Hora, Código, Descripción, Hora, Tiempo)
		///		Información = Sólo contiene un texto informativo.
		///		InicioGrafico = Línea donde inicia un gráfico. (Grafico)
		///		FinalGrafico = Línea donde finaliza un gráfico. (Valoracion)
		///		Desconocido = Tiene un formato, pero no es conocido.
		/// </summary>
		public enum TipoValoracion : int { Vacio = 0, Completo = 1, Parcial = 2, ParcialLinea = 3, ParcialCodigo = 4, ParcialVacio = 5, Informacion = 6, InicioGrafico = 7, FinalGrafico = 8, Desconocido = 9 }


		/// <summary>
		/// Limipia un texto dejándolo preparado para parsearlo con el gestor de gráficos.
		/// </summary>
		/// <param name="texto">Texto a limpiar</param>
		/// <returns>Texto límpio de caracteres no deseados.</returns>
		public static string LimpiarTexto(string texto) {

			// Eliminamos los espacios exteriores
			texto = texto.Trim();

			// Eliminamos las palabras en euskera y la palabra 'Dieta' y quitamos acentos a gráfico y valoración.
			texto = texto.Replace("Grafikoa/", "");
			texto = texto.Replace("Balorazioa/", "");
			texto = texto.Replace("Dieta", "");
			texto = texto.Replace("Gráfico", "Grafico");
			texto = texto.Replace("Valoración", "Valoracion");

			// Sustituimos las comas por puntos TODO: Comprobar si al parsear a numero se usa realmente el punto y no la coma.
			texto = texto.Replace(",", ".");

			// Quitamos los espacios extra.
			Regex recortador = new Regex(@"\s+");
			texto = recortador.Replace(texto, " ");

			// Eliminamos los espacios que haya entre los : y los elementos.
			texto = texto.Replace(" :", ":");
			texto = texto.Replace(": ", ":");

			// Añadimos un espacio delante del caracter de abrir paréntesis.
			texto = texto.Replace("(", " (");

			return texto;

		}


		public static int getNumeroGrafico(string texto) {

			if (String.IsNullOrEmpty(texto)) return 0;
			string[] subtextos = texto.Split(' ');
			int resultado = 0;

			foreach (string t in subtextos) {
				if (t.StartsWith("Grafico")) {
					string[] tt = t.Split(':');
					if (tt.Length > 1) {
						Int32.TryParse(tt[1], out resultado);
						return resultado;
					}
				}
			}

			return resultado;
		}


		public static TimeSpan getValoracionGrafico(string texto) {

			if (String.IsNullOrEmpty(texto)) return new TimeSpan(5,0,0,0);
			string[] subtextos = texto.Split(' ');
			TimeSpan resultado = new TimeSpan(5, 0, 0, 0);

			foreach (string t in subtextos) {
				if (t.StartsWith("Valoracion")) {
					string[] tt = t.Split(':');
					if (tt.Length > 2) {
						string ttt = tt[1] + ":" + tt[2];
						TimeSpan.TryParse(ttt, out resultado);
						return resultado;
					}
				}
			}

			return resultado;
		}


		public static bool esCodigo(string texto) {
			switch (texto.ToLower()) {
				case "des": return true;
				case "descanso": return true;
				case "cab": return true;
				case "cabecera": return true;
				case "ord": return true;
				case "vac": return true;
				case "vacio": return true;
				case "vacío": return true;
				case "rel": return true;
				case "res": return true;
				case "reserva": return true;
				case "viajero": return true;
			}
			return false;
		}


		/// <summary>
		/// Devuelve el tipo de valoración que se ha detectado en el texto y modifica la valoración pasada en consecuencia al tipo devuelto.
		/// </summary>
		/// <param name="texto">Texto del que deducir la valoración. Debe estar limpio.</param>
		/// <param name="valoracion">Valoración que se modificará en función del tipo devuelto.</param>
		/// <returns>Tipo de valoración detectado.</returns>
		public static TipoValoracion ParseaTexto(string texto, ref ValoracionGrafico valoracion) {

			// Si la cadena está vacía, devolvemos vacío.
			if (String.IsNullOrEmpty(texto.Trim())) return TipoValoracion.Vacio;

			// Iniciamos las variables.
			string[] subtextos = texto.Split(' ');
			TipoValoracion resultado = TipoValoracion.Desconocido;

			// Evaluamos el primer elemento del texto.
			if (subtextos.Length > 0) {
				// Si el primer elemento es una hora...
				if (Utils.ParseHora(subtextos[0], out TimeSpan hora, true)) {
					// Guardamos la hora en la valoración.
					valoracion.Inicio = hora;
				// Si no es una hora...
				} else {
					// Si empieza por Gráfico, es un inicio de gráfico.
					if (subtextos[0].StartsWith("Grafico")) {
						// Insertamos el número de gráfico en el campo Línea de la valoración.
						valoracion.Linea = getNumeroGrafico(texto);
						return TipoValoracion.InicioGrafico;
						// Si empieza por Valoración es un final de gráfico.
					} else if (subtextos[0].StartsWith("Valoracion")) {
						// Insertamos el tiempo de valoración del gráfico en el campo Tiempo de la valoración.
						valoracion.Tiempo = getValoracionGrafico(texto);
						return TipoValoracion.FinalGrafico;
						// Si no empieza por Aldaketa, es una información
					} else if (subtextos[0].StartsWith("Aldaketa")) {
						return TipoValoracion.Vacio;
					} else { 
						valoracion.Descripcion = texto;
						return TipoValoracion.Informacion;
					}
				}
			}

			// Evaluamos el segundo elemento.
			if (subtextos.Length > 1) {
				// Si el segundo elemento es un número...
				decimal linea;
				if (Decimal.TryParse(subtextos[1], out linea)) {
					// Guardamos el número en la valoración.
					valoracion.Linea = linea;
					resultado = TipoValoracion.ParcialLinea;
				} else if(esCodigo(subtextos[1])) {
					//valoracion.Codigo = subtextos[1];
					resultado = TipoValoracion.ParcialVacio;
				} else {
					resultado = TipoValoracion.ParcialVacio;
				}
			}

			// Evaluamos el tercer elemento
			if (subtextos.Length > 2) {
				if(resultado == TipoValoracion.ParcialLinea) {
					if (esCodigo(subtextos[2])) {
						//valoracion.Codigo = subtextos[2];
						resultado = TipoValoracion.Completo;
					}
				}
			}

			


			// Evaluamos el tipo que se ha detectado.
			switch (resultado) {
				case TipoValoracion.Completo:
					valoracion.Descripcion = (texto.Substring(subtextos[0].Length + subtextos[1].Length + 1 + subtextos[2].Length + 1)).Trim();
					break;
				case TipoValoracion.ParcialLinea:
				case TipoValoracion.ParcialCodigo:
					valoracion.Descripcion = (texto.Substring(subtextos[0].Length + subtextos[1].Length + 1)).Trim();
					break;
				case TipoValoracion.ParcialVacio:
					valoracion.Descripcion = (texto.Substring(subtextos[0].Length)).Trim();
					break;
			}

			return resultado;

		}





	}
}
