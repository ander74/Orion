namespace Orion.ViewModels {
	using System;
	using System.Data.OleDb;
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



		#region COMANDO 

		// Comando
		private ICommand comandoSqlNonQuery;
		public ICommand cmdComandoSqlNonQuery {
			get {
				if (comandoSqlNonQuery == null) comandoSqlNonQuery = new RelayCommand(p => ComandoSqlNonQuery());
				return comandoSqlNonQuery;
			}
		}

		// Ejecución del comando
		private void ComandoSqlNonQuery() {

			string lineSeparator = ((char)0x2028).ToString();
			string paragraphSeparator = ((char)0x2029).ToString();
			string[] comandos;
			string texto = CampoTexto.Replace("\n", string.Empty)
									 .Replace("\r", string.Empty)
									 .Replace("\r\n", string.Empty)
									 .Replace(lineSeparator, string.Empty)
									 .Replace(paragraphSeparator, string.Empty);
			//if (texto.Contains(";")) {
				comandos = texto.Split(';');
			//} else {
			//	comandos = new string[] { texto };
			//}

			CampoResultado = string.Empty;

			using (OleDbConnection conexion = new OleDbConnection(App.Global.CadenaConexion)) {
				using (OleDbCommand comando = conexion.CreateCommand()) {
					conexion.Open();
					try {
						foreach (string cmd in comandos) {
							if (string.IsNullOrWhiteSpace(cmd)) continue;
							//comando.CommandText = cmd.Replace("\n",string.Empty)
							//						 .Replace("\r", string.Empty)
							//						 .Replace("\r\n", string.Empty)
							//						 .Replace(lineSeparator, string.Empty)
							//						 .Replace(paragraphSeparator, string.Empty);
							comando.CommandText = cmd;
							int resultado = comando.ExecuteNonQuery();
							CampoResultado += $"Ejecutando ({DateTime.Now}):\n";
							CampoResultado += $"    {cmd}\n";
							CampoResultado += $"    {resultado} registros afectados.\n\n";
						}
					} catch (Exception ex){
						Mensajes.VerError("Comando SQL Non Query", ex);
					}
				}
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



		#endregion
		// ====================================================================================================


	}
}
