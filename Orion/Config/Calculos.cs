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
using System.Threading.Tasks;

namespace Orion.Config {

	public static class Calculos {

		// ====================================================================================================
		//  CLASE HORAS
		// ====================================================================================================
		public static class Horas {

			/* TRABAJADAS */
			public static TimeSpan Trabajadas(TimeSpan? inicio, TimeSpan? final, TimeSpan? iniciopartido, TimeSpan? finalpartido) {
				// Calculamos el tiempo total.
				TimeSpan total = (final ?? new TimeSpan(0)) - (inicio ?? new TimeSpan(0));
				// Regulamos el tiempo total.
				if (total.TotalSeconds < 0) total += new TimeSpan(1, 0, 0, 0);
				// calculamos el tiempo partido.
				TimeSpan partido = (finalpartido ?? new TimeSpan(0)) - (iniciopartido ?? new TimeSpan(0));
				// regulamos el tiempo partido
				if (partido.TotalSeconds < 0) partido += new TimeSpan(1, 0, 0, 0);
				// Si el tiempo partido es más de tres horas, le sumamos al total el tiempo que excede.
				TimeSpan excesoPartido = new TimeSpan(0);
				if (partido > App.Global.Convenio.MaxHorasParticionGraficoPartido) excesoPartido = partido - App.Global.Convenio.MaxHorasParticionGraficoPartido;
				// Devolvemos el tiempo trabajado.
				return total - (partido - excesoPartido);
			}


			/* ACUMULADAS */
			public static TimeSpan Acumuladas(TimeSpan? trabajadas) {
				TimeSpan horas = (trabajadas ?? new TimeSpan(0)) - App.Global.Convenio.JornadaMedia;
				return horas.TotalSeconds < 0 ? new TimeSpan(0) : horas;
			}

			/* NOCTURNAS */
			public static TimeSpan Nocturnas(TimeSpan? inicio, TimeSpan? final, int turno) {
				// Si inicio o final son nulos, devolvemos cero
				if (!inicio.HasValue || !final.HasValue) return new TimeSpan(0);
				// Establecemos el horario nocturno.
				TimeSpan inicionocturnas = App.Global.Convenio.InicioNocturnas;
				TimeSpan finalnocturnas = App.Global.Convenio.FinalNocturnas;
				// Inicializamos el resultado
				TimeSpan resultado = new TimeSpan(0);
				// Regulamos el servicio.
				if (final < inicio) final += new TimeSpan(1,0,0,0);
				// Si el turno no es 3...
				if (turno != 3) {
					if (inicio < finalnocturnas) resultado = finalnocturnas - inicio.Value;
					if (final > inicionocturnas) resultado = final.Value - inicionocturnas;
				}
				// Devolvemos el resultado.
				return resultado;
			}
		}


		// ====================================================================================================
		//  CLASE DIETAS
		// ====================================================================================================
		public static class Dietas {

			/* DESAYUNO */
			public static decimal Desayuno(TimeSpan? inicio, int turno) {
				if (turno != 1 || !inicio.HasValue) return 0m;
				TimeSpan horadesayuno = App.Global.Convenio.HoraDesayuno;
				if (inicio < horadesayuno) {
					TimeSpan tiempo = horadesayuno - inicio.Value;
					return tiempo.TotalMinutes > 60 ? 1m : tiempo.ToDecimal();
				} else {
					return 0m;
				}
			}


			/* COMIDA */
			public static decimal Comida(TimeSpan? inicio, TimeSpan? final, int turno, TimeSpan? iniciopartido, TimeSpan? finalpartido) {
				if (turno == 3) return 0m;
				if (turno == 4) {
					return ComidaCompleta(iniciopartido, finalpartido, turno) + ComidaParcial(iniciopartido, finalpartido, turno);
				} else {
					return ComidaCompleta(inicio, final, turno) + ComidaParcial(inicio, final, turno);
				}
			}

			private static TimeSpan TiempoPartidoComida(TimeSpan? iniciopartido, TimeSpan? finalpartido) {
				// Si iniciopartido o finalpartido son nulos, devolvemos cero.
				if (!iniciopartido.HasValue || !finalpartido.HasValue) return new TimeSpan(0);
				// Extraemos el horario de comida para servicios partidos.
				TimeSpan iniciocomidapartido = App.Global.Convenio.InicioComidaPartido;
				TimeSpan finalcomidapartido = App.Global.Convenio.FinalComidaPartido;
				// Regulamos el servicio.
				if (finalpartido < iniciopartido) finalpartido = finalpartido.Value.Add(new TimeSpan(1,0,0,0));
				// Inicializamos el resultado a devolver.
				TimeSpan resultado = new TimeSpan(0);
				// Recorremos minuto a minuto comprobando si pertenece al horario de comida.
				for (int m = (int)iniciopartido.Value.TotalMinutes; m < finalpartido.Value.TotalMinutes; m++) {
					if (m >= iniciocomidapartido.TotalMinutes && m < finalcomidapartido.TotalMinutes) resultado= resultado.Add(new TimeSpan(0,1,0));
				}
				// Devolvemos el resultado.
				return resultado;
			}

			/// <summary>
			/// Devuelve la dieta de comida completa.
			/// </summary>
			/// <param name="inicio">Si el turno es 4, se trata del inicio partido. Si no, es el inicio del servicio</param>
			/// <param name="final">Si el turno es 4, se trata del final partido. Si no, es el final del servicio.</param>
			/// <param name="turno"></param>
			/// <returns></returns>
			private static decimal ComidaCompleta(TimeSpan? inicio, TimeSpan? final, int turno) {
				// Si inicio o final son nulos, devolvemos cero.
				if (!inicio.HasValue || !final.HasValue) return 0m;
				// Inicializamos el horario de comida continuo y el resultado.
				TimeSpan iniciocomidacontinuo = App.Global.Convenio.InicioComidaContinuo;
				TimeSpan finalcomidacontinuo = App.Global.Convenio.FinalComidaContinuo;
				decimal resultado = 0m;
				// Si el turno es 1 y el final es mayor que la hora límite...
				if (turno == 1 && final > finalcomidacontinuo) resultado = 1m;
				// Si el turno es 2 y el inicio es menor que la hora límite...
				if (turno == 2 && inicio.Value.TotalMinutes > 180 && inicio < iniciocomidacontinuo) resultado = 1m;
				// Si el turno es 1 o 2 y empieza antes de las 12:30 y termina despues de las 15:30, se añade una dieta.
				if (turno < 3 && final > finalcomidacontinuo && inicio < iniciocomidacontinuo) {
					resultado += 1m;
				}
				// Si el turno es 4 y el tiempo partido que se tiene a la hora de la comida es menor de una hora...
				if (turno == 4 && TiempoPartidoComida(inicio, final).TotalMinutes < 60) resultado = 1m;
				return resultado;
			}

			/// <summary>
			/// Devuelve la dieta de comida parcial.
			/// </summary>
			/// <param name="inicio">Si el turno es 4, se trata del inicio partido. Si no, es el inicio del servicio</param>
			/// <param name="final">Si el turno es 4, se trata del final partido. Si no, es el final del servicio.</param>
			/// <param name="turno"></param>
			/// <returns></returns>
			private static decimal ComidaParcial(TimeSpan? inicio, TimeSpan? final, int turno) {
				// Si inicio o final son nulos, devolvemos cero.
				if (!inicio.HasValue || !final.HasValue) return 0m;
				// Definimos el horario de comidas
				TimeSpan iniciocomidacontinuo = App.Global.Convenio.InicioComidaContinuo;
				TimeSpan finalcomidacontinuo = App.Global.Convenio.FinalComidaContinuo;
				// Definimos las variables a usar.
				TimeSpan tiempopartido;
				decimal resultado = 0m;
				// Si el turno es 2
				if (turno == 2) {
					if (inicio == iniciocomidacontinuo) {
						resultado = 1m;
					} else if (inicio.Value.TotalMinutes >= (iniciocomidacontinuo.TotalMinutes + 5) &&
							   inicio.Value.TotalMinutes < (iniciocomidacontinuo.TotalMinutes + 30)) {

						resultado = (iniciocomidacontinuo.Add(new TimeSpan(0,30,0)) - inicio).ToDecimal();
					}
				}
				if (turno == 4) {
					tiempopartido = TiempoPartidoComida(inicio, final);
					if (tiempopartido.TotalMinutes >= 60 && tiempopartido.TotalMinutes < 120) {
						resultado = (new TimeSpan(0,120,0) - tiempopartido).ToDecimal();
					} else if (tiempopartido.TotalMinutes > 0 && tiempopartido.TotalMinutes < 60) {
						resultado = (new TimeSpan(0,60,0) - tiempopartido).ToDecimal();
					}
				}
				return resultado;
			}


			/* CENA */
			public static decimal Cena(TimeSpan? inicio, TimeSpan? final, int turno) {
				if (turno == 3 || !inicio.HasValue || !final.HasValue) return 0m;
				// Declaramos las propiedades a usar
				TimeSpan iniciocena = App.Global.Convenio.InicioCena;
				TimeSpan finalcena = App.Global.Convenio.FinalCena;
				// Ajustamos el servicio.
				if (final < inicio) final += new TimeSpan(1,0,0,0);
				// Ajustamos el horario de cena.
				if (finalcena < iniciocena) finalcena += new TimeSpan(1,0,0,0);
				// Inicializamos el resultado.
				decimal resultado = 0m;
				// Si el final está dentro de los límites de la cena, cogemos la diferencia.
				if (final > iniciocena && final < finalcena) resultado = (final - iniciocena).ToDecimal();
				// Si el final se pasa de los límites de la cena, cogemos una dieta entera.
				if (final >= finalcena) resultado = 1;
				return resultado;
			}


			/* PLUS CENA */
			public static decimal PlusCena(TimeSpan? inicio, TimeSpan? final, int turno) {
				if (turno == 3 || !inicio.HasValue || !final.HasValue) return 0m;
				// Declaramos el InicioPlusCena del convenio y el porcentaje
				TimeSpan iniciopluscena = App.Global.Convenio.InicioPlusCena;
				int porcentajepluscena = App.Global.Convenio.PorcentajePlusCena;
				// Ajustamos el servicio.
				if (final < inicio) final += new TimeSpan(1,0,0,0);
				// Ajustamos el InicioPlusCena
				if (iniciopluscena.TotalMinutes < 360) iniciopluscena += new TimeSpan(1,0,0,0);
				// Devolvemos el resultado.
				return (final > iniciopluscena ? (final - iniciopluscena).ToDecimal() : 0 ) * (porcentajepluscena / 100m);
			}
		}



	}
}
