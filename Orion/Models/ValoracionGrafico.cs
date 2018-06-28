#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using System;
using System.Data.OleDb;

namespace Orion.Models {

	/// <summary>
	/// Contiene una valoración de un gráfico concreto.
	/// Esta instancia determina valores nulos para los objetos de hora.
	/// </summary>
	public class ValoracionGrafico: NotifyBase {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================
		#endregion


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public ValoracionGrafico() { }


		public ValoracionGrafico(OleDbDataReader lector) {
			FromReader(lector);
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Inicio": Inicio = null; break;
				case "Linea": Linea = 0; break;
				case "Descripcion": Descripcion = ""; break;
				case "Final": Final = null; break;
				case "Tiempo": Tiempo = TimeSpan.Zero; break;
			}
		}


		public void Calcular() {
			if (!Inicio.HasValue || !Final.HasValue) return;
			if (Inicio.Value.Ticks > Extensiones.HoraMáxima || Final.Value.Ticks > Extensiones.HoraMáxima) return;
			TimeSpan t = Final.Value - Inicio.Value;
			if (t.Ticks < 0 || t.Ticks > Extensiones.HoraMáxima) t = TimeSpan.Zero;
			Tiempo = t;
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");//(lector["Id"] is DBNull) ? 0 : (Int32)lector["Id"];
			_idgrafico = lector.ToInt32("IdGrafico");//(lector["IdGrafico"] is DBNull) ? 0 : (Int32)lector["IdGrafico"];
			_inicio = lector.ToTimeSpanNulable("Inicio");
			_linea = lector.ToDecimal("Linea");//(lector["Linea"] is DBNull) ? 0 : (decimal)lector["Linea"];
			_descripcion = lector.ToString("Descripcion");//(lector["Descripcion"] is DBNull) ? "" : (string)lector["Descripcion"];
			_final = lector.ToTimeSpanNulable("Final");
			_tiempo = lector.ToTimeSpan("Tiempo");
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS SOBRECARGADOS
		// ====================================================================================================
		public override bool Equals(object obj) {
			// Si el objeto pasado no es un objeto válido devolvemos falso.
			if (!(obj is ValoracionGrafico)) return false;
			// Pasamos el objeto a una valoracion.
			ValoracionGrafico v = obj as ValoracionGrafico;
			// Evaluamos la línea
			if (v.Linea != Linea) return false;
			// Evaluamos el inicio
			if (v.Inicio.HasValue && Inicio.HasValue) {
				if (v.Inicio.Value.Ticks != Inicio.Value.Ticks) return false;
			} else {
				if (v.Inicio.HasValue || Inicio.HasValue) return false;
			}
			// Evaluamos el final
			if (v.Final.HasValue && Final.HasValue) {
				if (v.Final.Value.Ticks != Final.Value.Ticks) return false;
			} else {
				if (v.Final.HasValue || Final.HasValue) return false;
			}
			// Evaluamos el tiempo
			if (v.Tiempo.Ticks != Tiempo.Ticks) return false;

			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}


		public override string ToString() {
			string textoInicio = "";
			string textoFinal = "";
			string textoTiempo = "";
			if (Inicio.HasValue) textoInicio = Inicio.Value.ToString(@"hh\:mm");
			if (Final.HasValue) textoFinal = Final.Value.ToString(@"hh\:mm");
			textoTiempo = Tiempo.ToString(@"hh\:mm");
			return textoInicio + " " + Linea.ToString() + " " + Descripcion + " " + textoFinal + " " + textoTiempo;
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================
		public static void ParseFromReader(OleDbDataReader lector, ValoracionGrafico valoracion) {
			valoracion.Id = lector.ToInt32("Id");
			valoracion.IdGrafico = lector.ToInt32("IdGrafico");
			valoracion.Inicio = lector.ToTimeSpanNulable("Inicio");
			valoracion.Linea = lector.ToDecimal("Linea");
			valoracion.Descripcion = lector.ToString("Descripcion");
			valoracion.Final = lector.ToTimeSpanNulable("Final");
			valoracion.Tiempo = lector.ToTimeSpan("Tiempo");
		}

		public static void ParseToCommand(OleDbCommand Comando, ValoracionGrafico valoracion) {
			Comando.Parameters.AddWithValue("idgrafico", valoracion.IdGrafico);
			Comando.Parameters.AddWithValue("inicio", valoracion.Inicio.HasValue ? valoracion.Inicio.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("linea", valoracion.Linea.ToString("0.0000"));
			Comando.Parameters.AddWithValue("descripcion", valoracion.Descripcion);
			Comando.Parameters.AddWithValue("final", valoracion.Final.HasValue ? valoracion.Final.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("tiempo", valoracion.Tiempo.Ticks);
			Comando.Parameters.AddWithValue("id", valoracion.Id);
		}
		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private long _id;
		public long Id{
			get { return _id; }
			set {
				if (_id != value) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private long _idgrafico;
		public long IdGrafico {
			get { return _idgrafico; }
			set {
				if (_idgrafico != value) {
					_idgrafico = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _inicio;
		public TimeSpan? Inicio {
			get { return _inicio; }
			set {
				if (_inicio != value) {
					_inicio = value;
					Modificado = true;
					Calcular();
					PropiedadCambiada();
				}
			}
		}


		private decimal _linea;
		public decimal Linea {
			get { return _linea; }
			set {
				if (_linea != value) {
					_linea = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _descripcion = "";
		public string Descripcion {
			get { return _descripcion; }
			set {
				if (_descripcion != value) {
					_descripcion = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _final;
		public TimeSpan? Final {
			get { return _final; }
			set {
				if (_final != value) {
					_final = value;
					Modificado = true;
					Calcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _tiempo;
		public TimeSpan Tiempo {
			get { return _tiempo; }
			set {
				if (_tiempo != value) {
					_tiempo = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion


	} //Final de clase.
}
