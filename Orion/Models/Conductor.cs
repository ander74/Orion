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
using System.Collections.Specialized;
using Orion.Config;
using System.ComponentModel;
using Orion.DataModels;
using System.Data.Common;

namespace Orion.Models {

	public class Conductor : NotifyBase {


		// ====================================================================================================
		#region  CAMPOS PRIVADOS
		// ====================================================================================================
		#endregion


		// ====================================================================================================
		#region  CONSTRUCTORES
		// ====================================================================================================
		public Conductor() {
			//_regulaciones.CollectionChanged += ListaRegulaciones_CollectionChanged;
		}

		public Conductor(OleDbDataReader lector) {
			FromReader(lector);
			//_regulaciones.CollectionChanged += ListaRegulaciones_CollectionChanged;
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");
			_nombre = lector.ToString("Nombre");
			_apellidos = lector.ToString("Apellidos");
			_indefinido = lector.ToBool("Indefinido");
			_telefono = lector.ToString("Telefono");
			_email = lector.ToString("Email");
			_acumuladas = lector.ToTimeSpan("Acumuladas");
			_descansos = lector.ToInt16("Descansos");
			_descansosnodisfrutados = lector.ToInt16("DescansosNoDisfrutados");
			_plusdistancia = lector.ToDecimal("PlusDistancia");
			_reduccionjornada = lector.ToBool("ReduccionJornada");
			_notas = lector.ToString("Notas");
		}
		#endregion


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Número": Id = 0; break;
				case "Nombre": Nombre = ""; break;
				case "Apellidos": Apellidos = ""; break;
				case "Fijo": Indefinido = false; break;
				case "Teléfono": Telefono = ""; break;
				case "Email": Email = ""; break;
				case "Horas": Acumuladas = TimeSpan.Zero; break;
				case "DCs": Descansos = 0; break;
				case "DNDs": DescansosNoDisfrutados = 0; break;
				case "Plus Distancia": PlusDistancia = 0m; break;
				case "R.Jor.": ReduccionJornada = false; break;
			}
		}


		/// <summary>
		/// Los siguientes dos procedimientos pertenecen a la interfaz IModelDB que permite usar este objeto en
		/// los métodos genéricos de acceso a la base de datos.
		/// </summary>
		/// <param name="lector"></param>
		public void SetDataFromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");
			_nombre = lector.ToString("Nombre");
			_apellidos = lector.ToString("Apellidos");
			_indefinido = lector.ToBool("Indefinido");
			_telefono = lector.ToString("Telefono");
			_email = lector.ToString("Email");
			_acumuladas = lector.ToTimeSpan("Acumuladas");
			_descansos = lector.ToInt16("Descansos");
			_descansosnodisfrutados = lector.ToInt16("DescansosNoDisfrutados");
			_plusdistancia = lector.ToDecimal("PlusDistancia");
			_notas = lector.ToString("Notas");
		}

		public void GetDataToCommand(ref OleDbCommand comando) {
			comando.Parameters.AddWithValue("@Nombre", this.Nombre);
			comando.Parameters.AddWithValue("@Apellidos", this.Apellidos);
			comando.Parameters.AddWithValue("@Indefinido", this.Indefinido);
			comando.Parameters.AddWithValue("@Telefono", this.Telefono);
			comando.Parameters.AddWithValue("@Email", this.Email);
			comando.Parameters.AddWithValue("@Acumuladas", this.Acumuladas.Ticks);
			comando.Parameters.AddWithValue("@Descansos", this.Descansos);
			comando.Parameters.AddWithValue("@Descansosnodisfrutados", this.DescansosNoDisfrutados);
			comando.Parameters.AddWithValue("@Plusdistancia", this.PlusDistancia.ToString("0.0000"));
			comando.Parameters.AddWithValue("@Notas", this.Notas);
			comando.Parameters.AddWithValue("@Id", this.Id);
		}




		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, Conductor conductor) {
			conductor.Id = lector.ToInt32("Id");
			conductor.Nombre = lector.ToString("Nombre");
			conductor.Apellidos = lector.ToString("Apellidos");
			conductor.Indefinido = lector.ToBool("Indefinido");
			conductor.Telefono = lector.ToString("Telefono");
			conductor.Email = lector.ToString("Email");
			conductor.Acumuladas = lector.ToTimeSpan("Acumuladas");
			conductor.Descansos = lector.ToInt16("Descansos");
			conductor.DescansosNoDisfrutados = lector.ToInt16("DescansosNoDisfrutados");
			conductor.PlusDistancia = lector.ToDecimal("PlusDistancia");
			conductor.ReduccionJornada = lector.ToBool("ReduccionJornada");
			conductor.Notas = lector.ToString("Notas");
		}


		public static void ParseToCommand(OleDbCommand Comando, Conductor conductor) {
			Comando.Parameters.AddWithValue("@Nombre", conductor.Nombre);
			Comando.Parameters.AddWithValue("@Apellidos", conductor.Apellidos);
			Comando.Parameters.AddWithValue("@Indefinido", conductor.Indefinido);
			Comando.Parameters.AddWithValue("@Telefono", conductor.Telefono);
			Comando.Parameters.AddWithValue("@Email", conductor.Email);
			Comando.Parameters.AddWithValue("@Acumuladas", conductor.Acumuladas.Ticks);
			Comando.Parameters.AddWithValue("@Descansos", conductor.Descansos);
			Comando.Parameters.AddWithValue("@Descansosnodisfrutados", conductor.DescansosNoDisfrutados);
			Comando.Parameters.AddWithValue("@Plusdistancia", conductor.PlusDistancia.ToString("0.0000"));
			Comando.Parameters.AddWithValue("@ReduccionJornada", conductor.ReduccionJornada);
			Comando.Parameters.AddWithValue("@Notas", conductor.Notas);
			Comando.Parameters.AddWithValue("@Id", conductor.Id);
		}
		#endregion


		// ====================================================================================================
		#region EVENTOS
		// ====================================================================================================
		private void ListaRegulaciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

			if (e.NewItems != null) {
				foreach (RegulacionConductor regulacion in e.NewItems) {
					regulacion.IdConductor = this.Id;
					regulacion.Nuevo = true;
				}
				Modificado = true;
			}

			if (e.OldItems != null) {
				Modificado = true;
			}

			PropiedadCambiada(nameof(ListaRegulaciones));
		}

		private void _regulaciones_ItemPropertyChanged(object sender, ItemChangedEventArgs<RegulacionConductor> e)
		{
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


		// Propiedad no utilizada en esta clase, pero obligatoria por la interfaz IModelOleDB
		public int IdRelacionado { get; set; }


		private string _nombre = "";
		public string Nombre {
			get { return _nombre; }
			set {
				if (value != _nombre) {
					_nombre = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _apellidos = "";
		public string Apellidos {
			get { return _apellidos; }
			set {
				if (value != _apellidos) {
					_apellidos = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool _indefinido;
		public bool Indefinido {
			get { return _indefinido; }
			set {
				if (value != _indefinido) {
					_indefinido = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _telefono = "";
		public string Telefono {
			get { return _telefono; }
			set {
				if (value != _telefono) {
					_telefono = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _email = "";
		public string Email {
			get { return _email; }
			set {
				if (value != _email) {
					_email = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _acumuladas;
		public TimeSpan Acumuladas {
			get { return _acumuladas; }
			set {
				if (value != _acumuladas) {
					_acumuladas = value;
					Modificado = true;
					PropiedadCambiada();
					PropiedadCambiada(nameof(HorasNegativas));
				}
			}
		}


		private int _descansos;
		public int Descansos {
			get { return _descansos; }
			set {
				if (value != _descansos) {
					_descansos = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _descansosnodisfrutados;
		public int DescansosNoDisfrutados {
			get { return _descansosnodisfrutados; }
			set {
				if (_descansosnodisfrutados != value) {
					_descansosnodisfrutados = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}

		
		private decimal _plusdistancia;
		public decimal PlusDistancia {
			get { return _plusdistancia; }
			set {
				if (_plusdistancia != value) {
					_plusdistancia = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private bool _reduccionjornada;
		public bool ReduccionJornada {
			get { return _reduccionjornada; }
			set {
				if (_reduccionjornada != value) {
					_reduccionjornada = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private string _notas = "";
		public string Notas {
			get { return _notas; }
			set {
				if (value != _notas) {
					_notas = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		public bool HorasNegativas {
			get {
				return _acumuladas.Ticks < 0;
			}
		}


		private NotifyCollection<RegulacionConductor> _regulaciones = new NotifyCollection<RegulacionConductor>();
		public NotifyCollection<RegulacionConductor> ListaRegulaciones {
			get { return _regulaciones; }
			set {
				if (_regulaciones != value) {
					_regulaciones = value;
					Modificado = true;
					_regulaciones.CollectionChanged += ListaRegulaciones_CollectionChanged;
					_regulaciones.ItemPropertyChanged += _regulaciones_ItemPropertyChanged;
					PropiedadCambiada();
				}
			}
		}

		#endregion


	} //Final de clase
}
