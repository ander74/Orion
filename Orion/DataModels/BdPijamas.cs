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
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using Orion.Models;
using System.Windows;
using Orion.Config;

namespace Orion.DataModels {

	public static class BdPijamas {


		/*================================================================================
 		* GET DÍAS PIJAMA
 		*================================================================================*/
		public static List<DiaPijama> GetDiasPijama(DateTime fecha, IEnumerable<DiaCalendario> listadias, bool reduccionJornada, OleDbConnection conexion = null) {

			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);

			// Creamos la lista que se devolverá.
			List<DiaPijama> lista = new List<DiaPijama>();

			using (conexion) {

				string comandoSQL = "SELECT * " +
									"FROM (SELECT * " +
									"      FROM Graficos" +
									"      WHERE IdGrupo = (SELECT Id " +
									"                       FROM GruposGraficos " +
									"                       WHERE Validez = (SELECT Max(Validez) " +
									"                                        FROM GruposGraficos " +
									"                                        WHERE Validez <= ?)))" +
									"WHERE Numero = ?";

				try {
					// Abrimos la conexión.
					conexion.Open();
					// Definimos el final anterior.
					TimeSpan? finalAnterior = null;

					foreach (DiaCalendario dia in listadias) {
						// Establecemos la fecha del día, el número de gráfico y creamos el día.
						if (dia.Dia > DateTime.DaysInMonth(fecha.Year, fecha.Month)) continue;
						DateTime fechadia = new DateTime(fecha.Year, fecha.Month, dia.Dia);
						DiaPijama diapijama = new DiaPijama { Dia = dia.Dia, Grafico = dia.Grafico, Codigo=dia.Codigo, Fecha = fechadia,
															  ExcesoJornada = dia.ExcesoJornada,
															  FacturadoPaqueteria = dia.FacturadoPaqueteria, Limpieza = dia.Limpieza};
						// Establecemos si el día es festivo o no.
						if (App.Global.CalendariosVM.EsFestivo(fechadia)) diapijama.EsFestivo = true;

						// Establecemos el gráfico a buscar, por si está seleccionado el comodín.
						int g = dia.Grafico;
						if (dia.GraficoVinculado != 0 && g == App.Global.PorCentro.Comodin) g = dia.GraficoVinculado;

						// Creamos el comando que extrae el gráfico correspondiente.
						OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
						comando.Parameters.AddWithValue("validez", fechadia.ToString("yyyy-MM-dd"));
						comando.Parameters.AddWithValue("grafico", g);

						// Ejecutamos el comando y extraemos el gráfico.
						OleDbDataReader lector = comando.ExecuteReader();
						Grafico grafico = null;
						if (lector.Read()) {
							grafico = new Grafico(lector);

							// Rellenamos el día pijama
							diapijama.Turno = grafico.Turno;
							diapijama.Inicio = grafico.Inicio;
							diapijama.Final = grafico.Final;
							diapijama.InicioPartido = grafico.InicioPartido;
							diapijama.FinalPartido = grafico.FinalPartido;
							if (reduccionJornada) {
								diapijama.Horas = grafico.Trabajadas;
							} else {
								diapijama.Horas = grafico.Trabajadas < App.Global.Convenio.JornadaMedia ? App.Global.Convenio.JornadaMedia : grafico.Trabajadas;
							}
							diapijama.Acumuladas = grafico.Acumuladas;
							diapijama.Nocturnas = grafico.Nocturnas;
							diapijama.Desayuno = grafico?.Desayuno ?? 0m;
							diapijama.Comida = grafico?.Comida ?? 0m;
							diapijama.Cena = grafico?.Cena ?? 0m;
							diapijama.PlusCena = grafico?.PlusCena ?? 0m;
							// Plus Paquetería.
							if (grafico.PlusPaqueteria) {
								diapijama.PlusPaqueteria = App.Global.Convenio.PlusPaqueteria;
							} else {
								diapijama.PlusPaqueteria = dia.FacturadoPaqueteria * 0.10m; //TODO: Añadir este valor a las opciones.
							}
							// Plus Limpieza
							if (grafico.PlusLimpieza) {
								diapijama.PlusLimpieza = App.Global.Convenio.PlusLimpieza;
							} else {
								if (dia.Limpieza == null)
									diapijama.PlusLimpieza = App.Global.Convenio.PlusLimpieza / 2m; //TODO: Evaluar si se hace así o es un valor fijo.
								if (dia.Limpieza == true)
									diapijama.PlusLimpieza = App.Global.Convenio.PlusLimpieza;
							}
							// Plus Nocturnidad
							if (grafico.Turno == 3) diapijama.PlusNocturnidad = App.Global.Convenio.PlusNocturnidad;
							// Plus Navidad
							if (grafico.Numero > 0 && diapijama.Fecha.Day == 25 && diapijama.Fecha.Month == 12) {
								diapijama.PlusNavidad = App.Global.Convenio.PlusNavidad;
							}
							if (grafico.Numero > 0 && diapijama.Fecha.Day == 1 && diapijama.Fecha.Month == 1) {
								diapijama.PlusNavidad = App.Global.Convenio.PlusNavidad;
							}
							// Calculamos la dieta por menor descanso si existe para menos de 12 horas.
							if (finalAnterior.HasValue && diapijama.Inicio.HasValue) {
								// Añadimos uno al inicio de hoy.
								TimeSpan inicio = diapijama.Inicio.Value.Add(new TimeSpan(1, 0, 0, 0));
								// Si el final anterior es mayor que el inicio, añadimos otro día al inicio.
								if (inicio < finalAnterior.Value) inicio = inicio.Add(new TimeSpan(1, 0, 0, 0));
								// Si las horas que separan el inicio menos el final anterior son menos de doce...
								decimal diferenciahoras = (decimal)(inicio - finalAnterior.Value).TotalHours;
								if (diferenciahoras < 12) {
									//TODO: Establecer la dieta pijama.
									diapijama.PlusMenorDescanso = (12 - diferenciahoras) * App.Global.Convenio.DietaMenorDescanso;
								}
							}

							// Recalculamos los datos del pijama si hay que hacerlo.
							// TODO: Confirmar antes que se recalculen.
							if (!grafico.NoCalcular) diapijama?.Recalcular();
							//TODO: Recalcular si descuadre existe.
							//if (!grafico.NoCalcular && !dia.BloquearDescuadre && dia.Descuadre != 0) diapijama?.Recalcular();

						}
						lector.Close();
						// Establecemos las horas si es un permiso.
						if (dia.Grafico == -9) diapijama.Horas = App.Global.Convenio.JornadaMedia;

						// Establecemos el final anterior, si existe.
						if (diapijama.Final.HasValue) {
							finalAnterior = diapijama.Final.Value;
						} else {
							if (dia.Grafico != -5) finalAnterior = null;
						}

						// Si es un día de comite, se añade una dieta de comida.
						if ((dia.Codigo == 1) || (dia.Codigo == 2)) {
							if (diapijama.Comida < 1) diapijama.Comida = 1m;
							//diapijama.Cena = 0m; //TODO: Pendiente de que se confirme este dato.
							//diapijama.Desayuno = 0m; //TODO: Pendiente de que se confirme este dato.
						}

						// Añadimos el día pijama a la lista.
						lista.Add(diapijama);
					}
				} catch (Exception ex) {
					Utils.VerError("BdPijamas.GetDiasPijama", ex);
				}
			}

			// Devolvemos la lista.
			return lista;
		}


		/*================================================================================
 		* GET DÍAS PIJAMA 2
 		*================================================================================*/
		public static List<Pijama.DiaPijama> GetDiasPijama2(IEnumerable<DiaCalendarioBase> listadias, OleDbConnection conexion = null) {

			// Si no se pasa una conexión, se establece la conexion del centro actual.
			if (conexion == null) conexion = new OleDbConnection(App.Global.CadenaConexion);
			// Creamos la lista que se devolverá.
			List<Pijama.DiaPijama> lista = new List<Pijama.DiaPijama>();

			string comandoSQL = "SELECT * " +
								"FROM (SELECT * " +
								"      FROM Graficos" +
								"      WHERE IdGrupo = (SELECT Id " +
								"                       FROM GruposGraficos " +
								"                       WHERE Validez = (SELECT Max(Validez) " +
								"                                        FROM GruposGraficos " +
								"                                        WHERE Validez <= ?)))" +
								"WHERE Numero = ?";


			using (conexion) {
				// Habrá que quitar el control de excepciones aquí y ponerselo en la llamada al método, ya que aquí no hay
				// gestión de ventanas (o no debería haberlo).
				try {
					conexion.Open();
					foreach (DiaCalendarioBase dia in listadias) {
						// Creamos el día pijama a añadir a la lista.
						Pijama.DiaPijama diaPijama = new Pijama.DiaPijama(dia);
						// Establecemos el gráfico a buscar, por si está seleccionado el comodín.
						int GraficoBusqueda = dia.Grafico;
						if (dia.GraficoVinculado != 0 && dia.Grafico == App.Global.PorCentro.Comodin) GraficoBusqueda = dia.GraficoVinculado;
						// Creamos el comando SQL y añadimos los parámetros
						OleDbCommand comando = new OleDbCommand(comandoSQL, conexion);
						comando.Parameters.AddWithValue("@Validez", dia.DiaFecha.ToString("yyyy-MM-dd"));
						comando.Parameters.AddWithValue("@Numero", GraficoBusqueda);
						// Ejecutamos el comando y extraemos el gráfico.
						OleDbDataReader lector = comando.ExecuteReader();
						if (lector.Read()) diaPijama.GraficoTrabajado = new Grafico(lector);
						// Añadimos el día pijama a la lista.
						lista.Add(diaPijama);
						lector.Close();
					}
				} catch (Exception ex) {
					Utils.VerError("BdPijamas.GetDiasPijama2", ex);
				}
			}
			return lista;
		}


	} //Final de clase
}
