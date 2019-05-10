#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Config;
using Orion.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Orion.Models {

	public class Festivo: NotifyBase {


		// ====================================================================================================
		#region CONSTRUCTOR
		// ====================================================================================================
		public Festivo() {
		}

		public Festivo(OleDbDataReader lector) {
			FromReader(lector);
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS PRIVADOS
		// ====================================================================================================
		private void FromReader(OleDbDataReader lector) {
			_id = lector.ToInt32("Id");
			_año = lector.ToInt16("Año");
			_fecha = lector.ToDateTime("Fecha");
		}

		#endregion

		
		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================
		public void BorrarValorPorHeader(string header) {
			switch (header) {
				case "Año":
					Año = 0; break;
				case "Festivos":
					Fecha = new DateTime(2001, 1, 2); break;
			}
		}

		#endregion


		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, Festivo festivo) {
			festivo.Id = lector.ToInt32("Id");
			festivo.Año = lector.ToInt16("Año");
			festivo.Fecha = lector.ToDateTime("Fecha");
		}


		public static void ParseToCommand(OleDbCommand Comando, Festivo festivo) {
			Comando.Parameters.AddWithValue("año", festivo.Año);
			Comando.Parameters.AddWithValue("fecha", festivo.Fecha.ToString("yyyy-MM-dd"));
			Comando.Parameters.AddWithValue("id", festivo.Id);
		}


		#endregion


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		private long _id;
		public long Id {
			get { return _id; }
			set {
				if (_id != value) {
					_id = value;
					Modificado = true;
					PropiedadCambiada();
				}
			}
		}


		private int _año;
		public int Año {
			get { return _año; }
			set {
				if (_año != value) {
					_año = value;
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
					Modificado = true;
					if (_fecha.Year != _año) _fecha = new DateTime(_año, _fecha.Month, _fecha.Day);
					PropiedadCambiada();
				}
			}
		}


        #endregion


        #region COMANDO BORRAR

        // Comando
        private ICommand cmdborrarfestivo;
        public ICommand cmdBorrarFestivo {
            get {
                if (cmdborrarfestivo == null) cmdborrarfestivo = new RelayCommand(p => BorrarFestivo());
                return cmdborrarfestivo;
            }
        }

        private void BorrarFestivo() {
            this.Borrado = true;
            PropiedadCambiada(nameof(Borrado));
        }
        #endregion


    }
}
