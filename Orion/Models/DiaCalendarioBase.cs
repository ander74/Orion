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

	public class DiaCalendarioBase: NotifyBase {


		// ====================================================================================================
		#region  CONSTRUCTORES
		// ====================================================================================================
		public DiaCalendarioBase() { }


		public DiaCalendarioBase(OleDbDataReader lector) {
			ParseFromReader(lector, this);
		}


		public DiaCalendarioBase(DiaCalendarioBase dia) {
			_idcalendario = dia.IdCalendario;
			_dia = dia.Dia;
			_diafecha = dia.DiaFecha;
			_grafico = dia.Grafico;
			_codigo = dia.Codigo;
			_excesojornada = dia.ExcesoJornada;
			_facturadopaqueteria = dia.FacturadoPaqueteria;
			_limpieza = dia.Limpieza;
			_graficovinculado = dia.GraficoVinculado;
			_notas = dia.Notas;
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, DiaCalendarioBase diacalendario) {
			diacalendario.Id = lector.ToInt32("Id");
			diacalendario.IdCalendario = lector.ToInt32("IdCalendario");
			diacalendario.Dia = lector.ToInt16("Dia");
			diacalendario.DiaFecha = lector.ToDateTime("DiaFecha");
			diacalendario.Grafico = lector.ToInt16("Grafico");
			diacalendario.Codigo = lector.ToInt16("Codigo");
			diacalendario.ExcesoJornada = lector.ToTimeSpan("ExcesoJornada");
			diacalendario.FacturadoPaqueteria = lector.ToDecimal("FacturadoPaqueteria");
			diacalendario.Limpieza = lector["Limpieza"] is DBNull ? null : (bool?)lector["Limpieza"];
			diacalendario.GraficoVinculado = lector.ToInt16("GraficoVinculado");
			diacalendario.Notas = lector.ToString("Notas");
		}


		public static void ParseToCommand(OleDbCommand Comando, DiaCalendarioBase diacalendario) {
			Comando.Parameters.AddWithValue("@IdCalendario", diacalendario.IdCalendario);
			Comando.Parameters.AddWithValue("@Dia", diacalendario.Dia);
			Comando.Parameters.AddWithValue("@DiaFecha", diacalendario.DiaFecha.ToString("yyyy-MM-dd"));
			Comando.Parameters.AddWithValue("@Grafico", diacalendario.Grafico);
			Comando.Parameters.AddWithValue("@Codigo", diacalendario.Codigo);
			Comando.Parameters.AddWithValue("@ExcesoJornada", diacalendario.ExcesoJornada.Ticks);
			Comando.Parameters.AddWithValue("@FacturadoPaqueteria", diacalendario.FacturadoPaqueteria.ToString("0.0000"));
			Comando.Parameters.AddWithValue("@Limpieza", diacalendario.Limpieza == null ? (object)DBNull.Value : diacalendario.Limpieza);
			Comando.Parameters.AddWithValue("@GraficoVinculado", diacalendario.GraficoVinculado);
			Comando.Parameters.AddWithValue("@Notas", diacalendario.Notas.TrimEnd(new char[] { ' ', '\n', '\r', '\t' }));
			Comando.Parameters.AddWithValue("@Id", diacalendario.Id);
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private int _id;
		public int Id {
			get { return _id; }
			set {
				_id = value;
				Modificado = true;
				PropiedadCambiada();
			}
		}


		private int _idcalendario;
		public int IdCalendario {
			get { return _idcalendario; }
			set {
				if (_idcalendario != value) {
					_idcalendario = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _dia;
		public int Dia {
			get { return _dia; }
			set {
				if (_dia != value) {
					_dia = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private DateTime _diafecha;
		public DateTime DiaFecha {
			get { return _diafecha; }
			set {
				if (_diafecha != value) {
					_diafecha = value;
					PropiedadCambiada();
				}
			}
		}


		private int _grafico = 0;
		public int Grafico {
			get { return _grafico; }
			set {
				if (_grafico != value) {
					if (value == -4 && _grafico == -2) {
						_grafico = -10;
					} else if (value == -4 && _grafico == -3) {
						_grafico = -11;
					} else { 
						_grafico = value;
					}
					Modificado = true;
					PropiedadCambiada();
					PropiedadCambiada(nameof(ComboGrafico));
				}
			}
		}


		private int _codigo;
		public int Codigo {
			get { return _codigo; }
			set {
				if (_codigo != value) {
					_codigo = value;
					Modificado = true;
					PropiedadCambiada();
					PropiedadCambiada(nameof(ComboGrafico));
				}
			}
		}


		public Tuple<int, int> ComboGrafico {
			get { return new Tuple<int, int>(Grafico, Codigo); }
			set {
				if (value.Item1 == 0 && value.Item2 == 0) {
					Grafico = 0;
					Codigo = 0;
				} else {
					if (Grafico != value.Item1 && value.Item1 != 0) {
						Grafico = value.Item1;
					}
					if (Codigo != value.Item2) {
						Codigo = value.Item2;
					}
				}
			}
		}


		private TimeSpan _excesojornada;
		public TimeSpan ExcesoJornada {
			get { return _excesojornada; }
			set {
				if (_excesojornada != value) {
					_excesojornada = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private decimal _facturadopaqueteria;
		public decimal FacturadoPaqueteria {
			get { return _facturadopaqueteria; }
			set {
				if (_facturadopaqueteria != value) {
					_facturadopaqueteria = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool? _limpieza = false;
		public bool? Limpieza {
			get { return _limpieza; }
			set {
				if (_limpieza != value) {
					_limpieza = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _graficovinculado;
		public int GraficoVinculado {
			get { return _graficovinculado; }
			set {
				if (_graficovinculado != value) {
					_graficovinculado = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _notas = "";
		public string Notas {
			get { return _notas; }
			set {
				if (_notas != value) {
					_notas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion
		// ====================================================================================================


	}

}
