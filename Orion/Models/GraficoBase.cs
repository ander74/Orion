#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models {


	public class GraficoBase: NotifyBase {


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================
		public GraficoBase() { }


		public GraficoBase(OleDbDataReader lector) {
			ParseFromReader(lector, this);
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void Recalcular() {
			if (IsServicioCompleto()) {
				CalcularServicio();
			}
		}


		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private bool IsServicioCompleto() {
			if (Turno < 1 || Turno > 4) return false;
			if (!Inicio.HasValue || !Final.HasValue) return false;
			if (Inicio.Value.Ticks < 0 || Final.Value.Ticks < 0) return false;
			return true;
		}


		private void CalcularServicio() {
			if (NoCalcular) return;
			TimeSpan trabajadas = Calculos.Horas.Trabajadas(_inicio.Value, _final.Value, _iniciopartido, _finalpartido);
			TimeSpan acumuladas = Calculos.Horas.Acumuladas(trabajadas);
			TimeSpan nocturnas = Calculos.Horas.Nocturnas(_inicio.Value, _final.Value, _turno);
			decimal desayuno = Calculos.Dietas.Desayuno(_inicio.Value, _turno);
			decimal comida = Calculos.Dietas.Comida(_inicio.Value, _final.Value, _turno, _iniciopartido, _finalpartido);
			decimal cena = Calculos.Dietas.Cena(_inicio.Value, _final.Value, _turno);
			decimal pluscena = Calculos.Dietas.PlusCena(_inicio.Value, _final.Value, _turno);
			bool notificar = false;
			if (_trabajadas != trabajadas) { _trabajadas = trabajadas; notificar = true; }
			if (_acumuladas != acumuladas) { _acumuladas = acumuladas; notificar = true; }
			if (_nocturnas != nocturnas) { _nocturnas = nocturnas; notificar = true; }
			if (_desayuno != desayuno) { _desayuno = desayuno; notificar = true; }
			if (_comida != comida) { _comida = comida; notificar = true; }
			if (_cena != cena) { _cena = cena; notificar = true; }
			if (_pluscena != pluscena) { _pluscena = pluscena; notificar = true; }
			if (notificar) { Modificado = true; PropiedadCambiada(""); }
		}


		#endregion


		// ====================================================================================================
		#region MÉTODOS SOBRECARGADOS
		// ====================================================================================================
		public override bool Equals(object obj) {
			if (!(obj is GraficoBase)) return false;
			GraficoBase g = obj as GraficoBase;
			if (g.Numero != Numero || g.Turno != Turno) return false;
			if (g.Inicio != Inicio || g.Final != Final) return false;
			if (g.InicioPartido != InicioPartido || g.FinalPartido != FinalPartido) return false;
			if (g.Valoracion != Valoracion) return false;
			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================
		public static void ParseFromReader(OleDbDataReader lector, GraficoBase grafico) {
			grafico.Id = DBUtils.FromReaderEnteroLargo(lector, "Id");
			grafico.IdGrupo = DBUtils.FromReaderEnteroLargo(lector, "IdGrupo");
			grafico.NoCalcular = DBUtils.FromReaderSiNo(lector, "NoCalcular");
			grafico.Numero = DBUtils.FromReaderEntero(lector, "Numero");
			grafico.Turno = DBUtils.FromReaderEntero(lector, "Turno");
			grafico.Inicio = Utils.ReaderToHora(lector, "Inicio");
			grafico.Final = Utils.ReaderToHora(lector, "Final");
			grafico.InicioPartido = Utils.ReaderToHora(lector, "InicioPartido");
			grafico.FinalPartido = Utils.ReaderToHora(lector, "FinalPartido");
			grafico.Valoracion = Utils.ReaderToHoraSinNulo(lector, "Valoracion");
			grafico.Trabajadas = Utils.ReaderToHoraSinNulo(lector, "Trabajadas");
			grafico.Acumuladas = Utils.ReaderToHoraSinNulo(lector, "Acumuladas");
			grafico.Nocturnas = Utils.ReaderToHoraSinNulo(lector, "Nocturnas");
			grafico.Desayuno = DBUtils.FromReaderDecimal(lector, "Desayuno");
			grafico.Comida = DBUtils.FromReaderDecimal(lector, "Comida");
			grafico.Cena = DBUtils.FromReaderDecimal(lector, "Cena");
			grafico.PlusCena = DBUtils.FromReaderDecimal(lector, "PlusCena");
			grafico.PlusLimpieza = DBUtils.FromReaderSiNo(lector, "PlusLimpieza");
			grafico.PlusPaqueteria = DBUtils.FromReaderSiNo(lector, "PlusPaqueteria");
		}


		public static void ParseToCommand(OleDbCommand Comando, GraficoBase grafico) {
			Comando.Parameters.AddWithValue("idgrupo", grafico.IdGrupo);
			Comando.Parameters.AddWithValue("nocalcular", grafico.NoCalcular);
			Comando.Parameters.AddWithValue("numero", grafico.Numero);
			Comando.Parameters.AddWithValue("turno", grafico.Turno);
			Comando.Parameters.AddWithValue("inicio", grafico.Inicio.HasValue ? grafico.Inicio.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("final", grafico.Final.HasValue ? grafico.Final.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("iniciopartido", grafico.InicioPartido.HasValue ? grafico.InicioPartido.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("finalpartido", grafico.FinalPartido.HasValue ? grafico.FinalPartido.Value.Ticks : (object)DBNull.Value);
			Comando.Parameters.AddWithValue("valoracion", grafico.Valoracion.Ticks);
			Comando.Parameters.AddWithValue("trabajadas", grafico.Trabajadas.Ticks);
			Comando.Parameters.AddWithValue("acumuladas", grafico.Acumuladas.Ticks);
			Comando.Parameters.AddWithValue("nocturnas", grafico.Nocturnas.Ticks);
			Comando.Parameters.AddWithValue("desayuno", grafico.Desayuno.ToString("0.0000"));
			Comando.Parameters.AddWithValue("comida", grafico.Comida.ToString("0.0000"));
			Comando.Parameters.AddWithValue("cena", grafico.Cena.ToString("0.0000"));
			Comando.Parameters.AddWithValue("pluscena", grafico.PlusCena.ToString("0.0000"));
			Comando.Parameters.AddWithValue("pluslimpieza", grafico.PlusLimpieza);
			Comando.Parameters.AddWithValue("pluspaqueteria", grafico.PlusPaqueteria);
			Comando.Parameters.AddWithValue("id", grafico.Id);
		}
		#endregion


		// ====================================================================================================
		#region  PROPIEDADES
		// ====================================================================================================
		private long _id;
		public long Id {
			get { return _id; }
			set {
				if (value != _id) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private long _idgrupo;
		public long IdGrupo {
			get { return _idgrupo; }
			set {
				if (value != _idgrupo) {
					_idgrupo = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool _nocalcular;
		public bool NoCalcular {
			get { return _nocalcular; }
			set {
				if (_nocalcular != value) {
					_nocalcular = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _numero;
		public int Numero {
			get { return _numero; }
			set {
				if (value != _numero) {
					_numero = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _turno = 1;
		public int Turno {
			get { return _turno; }
			set {
				if (value != _turno) {
					_turno = (value < 1 || value > 4) ? 1 : value;
					Modificado = true;
					Recalcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _inicio;
		public TimeSpan? Inicio {
			get { return _inicio; }
			set {
				if (!value.Equals(_inicio)) {
					_inicio = value;
					Modificado = true;
					Recalcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _final;
		public TimeSpan? Final {
			get { return _final; }
			set {
				if (!value.Equals(_final)) {
					_final = value;
					Modificado = true;
					Recalcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _iniciopartido;
		public TimeSpan? InicioPartido {
			get { return _iniciopartido; }
			set {
				if (!value.Equals(_iniciopartido)) {
					_iniciopartido = value;
					Modificado = true;
					Recalcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan? _finalpartido;
		public TimeSpan? FinalPartido {
			get { return _finalpartido; }
			set {
				if (!value.Equals(_finalpartido)) {
					_finalpartido = value;
					Modificado = true;
					Recalcular();
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _valoracion;
		public TimeSpan Valoracion {
			get { return _valoracion; }
			set {
				if (!value.Equals(_valoracion)) {
					_valoracion = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _trabajadas;
		public TimeSpan Trabajadas {
			get { return _trabajadas; }
			set {
				if (!value.Equals(_trabajadas)) {
					_trabajadas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _acumuladas;
		public TimeSpan Acumuladas {
			get { return _acumuladas; }
			set {
				if (!value.Equals(_acumuladas)) {
					_acumuladas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _nocturnas;
		public TimeSpan Nocturnas {
			get { return _nocturnas; }
			set {
				if (!value.Equals(_nocturnas)) {
					_nocturnas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private decimal _desayuno;
		public decimal Desayuno {
			get { return _desayuno; }
			set {
				if (value != _desayuno) {
					_desayuno = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private decimal _comida;
		public decimal Comida {
			get { return _comida; }
			set {
				if (value != _comida) {
					_comida = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private decimal _cena;
		public decimal Cena {
			get { return _cena; }
			set {
				if (value != _cena) {
					_cena = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private decimal _pluscena;
		public decimal PlusCena {
			get { return _pluscena; }
			set {
				if (value != _pluscena) {
					_pluscena = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool _pluslimpieza;
		public bool PlusLimpieza {
			get { return _pluslimpieza; }
			set {
				if (_pluslimpieza != value) {
					_pluslimpieza = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool _pluspaqueteria;
		public bool PlusPaqueteria {
			get { return _pluspaqueteria; }
			set {
				if (_pluspaqueteria != value) {
					_pluspaqueteria = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion




	}
}
