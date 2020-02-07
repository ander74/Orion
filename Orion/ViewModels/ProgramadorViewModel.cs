#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.ViewModels {
	using System;
	using System.Data;
	using System.Windows.Input;
	using Models;
	using Servicios;

	public class ProgramadorViewModel : NotifyBase {

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================

		private IMensajes Mensajes;

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================

		public ProgramadorViewModel(IMensajes servicioMensajes) {
			Mensajes = servicioMensajes;
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region COMANDOS
		// ====================================================================================================



		#region COMANDO SQL NON QUERY

		// Comando
		private ICommand sqlNonQuery;
		public ICommand cmdSqlNonQuery {
			get {
				if (sqlNonQuery == null) sqlNonQuery = new RelayCommand(p => SqlNonQuery());
				return sqlNonQuery;
			}
		}

		// Ejecución del comando
		private void SqlNonQuery() {

			string lineSeparator = ((char)0x2028).ToString();
			string paragraphSeparator = ((char)0x2029).ToString();
			string[] comandos;
			string texto = CampoTexto.Replace("\n", string.Empty)
									 .Replace("\r", string.Empty)
									 .Replace("\r\n", string.Empty)
									 .Replace(lineSeparator, string.Empty)
									 .Replace(paragraphSeparator, string.Empty);
			comandos = texto.Split(';');

			CampoResultado = string.Empty;
			try {
				foreach (string cmd in comandos) {
					if (string.IsNullOrWhiteSpace(cmd)) continue;
					CampoResultado += $"Ejecutando ({DateTime.Now}):\n";
					CampoResultado += $"    {cmd}\n";
					var resultado = App.Global.Repository.ExecureNonQuery(new SQLiteExpression(cmd));
					CampoResultado += $"    Filas afectadas = {resultado}\n";

				}
			} catch (Exception ex) {
				Mensajes.VerError("Comando SQL NON QUERY", ex);
				CampoResultado += $"Consulta Errónea.\n\n";
				return;
			}
			CampoResultado += $"Consulta Terminada.\n\n";
		}

		#endregion



		#region COMANDO SQL ESCALAR

		// Comando
		private ICommand sqlEscalar;
		public ICommand cmdSqlEscalar {
			get {
				if (sqlEscalar == null) sqlEscalar = new RelayCommand(p => SqlEscalar());
				return sqlEscalar;
			}
		}

		// Ejecución del comando
		private void SqlEscalar() {
			string lineSeparator = ((char)0x2028).ToString();
			string paragraphSeparator = ((char)0x2029).ToString();
			string[] comandos;
			string texto = CampoTexto.Replace("\n", string.Empty)
									 .Replace("\r", string.Empty)
									 .Replace("\r\n", string.Empty)
									 .Replace(lineSeparator, string.Empty)
									 .Replace(paragraphSeparator, string.Empty);
			comandos = texto.Split(';');

			CampoResultado = string.Empty;
			try {
				foreach (string cmd in comandos) {
					if (string.IsNullOrWhiteSpace(cmd)) continue;
					CampoResultado += $"Ejecutando ({DateTime.Now}):\n";
					CampoResultado += $"    {cmd}\n";
					var resultado = App.Global.Repository.GetScalar(new SQLiteExpression(cmd));
					CampoResultado += $"    Resultado = {resultado}\n";
				}
			} catch (Exception ex) {
				Mensajes.VerError("Comando SQL ESCALAR", ex);
				CampoResultado += $"Consulta Errónea.\n\n";
				return;
			}
			CampoResultado += $"Consulta Terminada.\n\n";
		}
		#endregion



		#region COMANDO SQL READER

		// Comando
		private ICommand sqlReader;
		public ICommand cmdSqlReader {
			get {
				if (sqlReader == null) sqlReader = new RelayCommand(p => SqlReader());
				return sqlReader;
			}
		}

		// Ejecución del comando
		private void SqlReader() {

			string lineSeparator = ((char)0x2028).ToString();
			string paragraphSeparator = ((char)0x2029).ToString();
			string[] comandos;
			string texto = CampoTexto.Replace("\n", string.Empty)
									 .Replace("\r", string.Empty)
									 .Replace("\r\n", string.Empty)
									 .Replace(lineSeparator, string.Empty)
									 .Replace(paragraphSeparator, string.Empty);
			comandos = texto.Split(';');

			CampoResultado = string.Empty;
			try {
				var cmd = comandos[0];
				if (!string.IsNullOrWhiteSpace(cmd)) {
					CampoResultado += $"Ejecutando ({DateTime.Now}):\n";
					CampoResultado += $"    {cmd}\n";
					Tabla = App.Global.Repository.ExecuteReader(new SQLiteExpression(cmd));
					VistaTabla = Tabla.AsDataView();
					CampoResultado += $"    Total Filas = {Tabla.Rows.Count}\n";
				}
			} catch (Exception ex) {
				Mensajes.VerError("Comando SQL READER", ex);
				CampoResultado += $"Consulta Errónea.\n\n";
				return;
			}
			CampoResultado += $"Consulta Terminada.\n\n";
		}

		#endregion




		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private string campoTexto = string.Empty;
		public string CampoTexto {
			get { return campoTexto; }
			set { SetValue(ref campoTexto, value); }
		}



		private string campoResultado;
		public string CampoResultado {
			get { return campoResultado; }
			set { SetValue(ref campoResultado, value); }
		}



		private DataTable tabla;
		public DataTable Tabla {
			get => tabla;
			set {
				SetValue(ref tabla, value);
			}
		}


		private DataView vistaTabla;
		public DataView VistaTabla {
			get => vistaTabla;
			set => SetValue(ref vistaTabla, value);
		}



		#endregion
		// ====================================================================================================


	}
}
