#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Collections.Specialized;
using Orion.Config;
using System.ComponentModel;

namespace Orion.Models {

	public class Calendario :NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================

		#endregion


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================
		public Calendario() {
			// Creamos la lista de días.
			//ListaDias = new ObservableCollection<DiaCalendario>();

		}

		public Calendario(OleDbDataReader lector) {
			FromReader(lector);
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");
			_idconductor = lector.ToInt32("IdConductor");
			_fecha = lector.ToDateTime("Fecha");
			_notas = lector.ToString("Notas");
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Conductor":
					IdConductor = 0; break;
				case "01": case "02": case "03": case "04": case "05": case "06": case "07": case "08": case "09": case "10":
				case "11": case "12": case "13": case "14": case "15": case "16": case "17": case "18": case "19": case "20":
				case "21": case "22": case "23": case "24": case "25": case "26": case "27": case "28": case "29": case "30":
				case "31":
					ListaDias[int.Parse(header) - 1].Grafico = 0; break;
			}
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, Calendario calendario) {
			calendario.Id = lector.ToInt32("Id");
			calendario.IdConductor = lector.ToInt32("IdConductor");
			calendario.Fecha = lector.ToDateTime("Fecha");
			calendario.Notas = lector.ToString("Notas");
		}


		public static void ParseToCommand(OleDbCommand Comando, Calendario calendario) {
			Comando.Parameters.AddWithValue("@Idconductor", calendario.IdConductor);
			Comando.Parameters.AddWithValue("@Fecha", calendario.Fecha.ToString("yyyy-MM-dd"));
			Comando.Parameters.AddWithValue("@Notas", calendario.Notas);
			Comando.Parameters.AddWithValue("@Id", calendario.Id);
		}


		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================
		private void ListaDias_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

			if (e.NewItems != null) {
				foreach (DiaCalendario dia in e.NewItems) {
					dia.IdCalendario = this.Id;
					dia.Nuevo = true;
					dia.ObjetoCambiado += ObjetoCambiadoEventHandler;
				}
			}

			if (e.OldItems != null) {
				foreach (DiaCalendario dia in e.OldItems) {
					dia.ObjetoCambiado -= ObjetoCambiadoEventHandler;
				}
			}

			PropiedadCambiada(nameof(ListaDias));
		}

		private void ObjetoCambiadoEventHandler(object sender, PropertyChangedEventArgs e) {
			Modificado = true;
		}

		#endregion


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


		private int _idconductor;
		public int IdConductor {
			get { return _idconductor; }
			set {
				if (_idconductor != value) {
					_idconductor = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private DateTime _fecha;
		public DateTime Fecha {
			get { return _fecha; }
			set {
				if (_fecha != value) {
					_fecha = value;
					//foreach (DiaCalendario dia in ListaDias) {
					//	dia.DiaFecha = new DateTime(_fecha.Year, _fecha.Month, dia.DiaFecha.Day);
					//}
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


		private bool _conductorindefinido;
		public bool ConductorIndefinido {
			get { return _conductorindefinido; }
			set {
				if (_conductorindefinido != value) {
					_conductorindefinido = value;
					PropiedadCambiada();
				}
			}
		}


		private string _informe = "";
		public string Informe {
			get { return _informe == "" ? null : _informe; }
			set {
				if (_informe != value) {
					_informe = value;
					PropiedadCambiada();
					PropiedadCambiada(nameof(HayInforme));
				}
			}
		}


		public bool HayInforme {
			get {
				return (Informe != null && Informe.Trim().Length > 0);
			}
		}


		private ObservableCollection<DiaCalendario> _listadias;
		public ObservableCollection<DiaCalendario> ListaDias {
			get {
				if (_listadias == null) ListaDias = new ObservableCollection<DiaCalendario>();
				return _listadias;
			}
			set {
				if (_listadias != value) {
					// Creamos una nueva lista.
					_listadias = new ObservableCollection<DiaCalendario>();
					// Activamos el evento CollectionChanged
					_listadias.CollectionChanged += ListaDias_CollectionChanged;
					// Llenamos la lista con 31 días vacíos.
					for (int m = 1; m <= 31; m++) {
						DiaCalendario d = new DiaCalendario { IdCalendario = this.Id, Dia = m, Nuevo = true };
						if (m <= DateTime.DaysInMonth(Fecha.Year, Fecha.Month)) {
							d.DiaFecha = new DateTime(Fecha.Year, Fecha.Month, m);
						} else {
							d.DiaFecha = new DateTime(Fecha.Year, Fecha.Month, 1);
						}
						d.Modificado = false;
						_listadias.Add(d);
					}
					// Sustituimos los días que existen por los días vacíos en la lista.
					foreach (DiaCalendario dia in value) {						
						_listadias[dia.Dia - 1] = dia;
						dia.Nuevo = false;
					}
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		#endregion

	}
}
