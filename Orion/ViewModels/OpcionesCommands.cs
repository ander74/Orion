#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Newtonsoft.Json;
using Orion.Config;
using Orion.DataModels;
using Orion.Models;
using Orion.Properties;
using Orion.Views;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace Orion.ViewModels {

	public partial class OpcionesViewModel {


		#region CAMBIAR CARPETA DATOS
		//Comando
		private ICommand _cmdcambiarcarpeta;
		public ICommand cmdCambiarCarpeta {
			get {
				if (_cmdcambiarcarpeta == null) _cmdcambiarcarpeta = new RelayCommand(p => CambiarCarpeta(p));
				return _cmdcambiarcarpeta;
			}
		}

		// Ejecución del comando
		private void CambiarCarpeta(object parametro) {
			string carpeta = parametro as string;
			if (carpeta == null) return;
			FolderBrowserDialog dialogo = new FolderBrowserDialog();
			dialogo.RootFolder = Environment.SpecialFolder.MyComputer;
			dialogo.ShowNewFolderButton = true;
			switch (carpeta) {
				case "Datos":
					dialogo.Description = "Ubicación Base de Datos";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaDatos;
					if (dialogo.ShowDialog() == DialogResult.OK) {
						App.Global.Configuracion.CarpetaDatos = dialogo.SelectedPath;
						App.Global.Configuracion.SincronizarEnDropbox = false;
					}
					return;
				case "Dropbox":
					dialogo.Description = "Ubicación Carpeta de Dropbox";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaDropbox;
					if (dialogo.ShowDialog() == DialogResult.OK) {
						App.Global.Configuracion.CarpetaDropbox = dialogo.SelectedPath;
						App.Global.Configuracion.SincronizarEnDropbox = false;
					}
					return;
				case "Ayuda":
					dialogo.Description = "Ubicación de la Ayuda";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaAyuda;
					if (dialogo.ShowDialog() == DialogResult.OK) App.Global.Configuracion.CarpetaAyuda = dialogo.SelectedPath;
					return;
				case "Informes":
					dialogo.Description = "Ubicación Hojas Pijama";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaInformes;
					if (dialogo.ShowDialog() == DialogResult.OK) App.Global.Configuracion.CarpetaInformes = dialogo.SelectedPath;
					return;
				case "CopiasSeguridad":
					dialogo.Description = "Ubicación Copias de Seguridad";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaCopiasSeguridad;
					if (dialogo.ShowDialog() == DialogResult.OK) App.Global.Configuracion.CarpetaCopiasSeguridad = dialogo.SelectedPath;
					return;
				case "OrigenActualizar":
					dialogo.Description = "Ubicación Archivos Actualización";
					dialogo.SelectedPath = App.Global.Configuracion.CarpetaOrigenActualizar;
					if (dialogo.ShowDialog() == DialogResult.OK) App.Global.Configuracion.CarpetaOrigenActualizar = dialogo.SelectedPath;
					return;
            }
        }
		#endregion


		#region GUARDAR
		//Comando
		private ICommand _cmdguardar;
		public ICommand cmdGuardar {
			get {
				if (_cmdguardar == null) _cmdguardar = new RelayCommand(p => Guardar(), p => PuedeGuardar());
				return _cmdguardar;
			}
		}

		// Puede Ejecutarse
		// (Esta función es pública por el evento Closing de MainView. Ver más información en dicho evento.)
		public bool PuedeGuardar() {
			bool resultado = false;
			if (HayCambios) resultado = true;
			return resultado;
		}

		// Ejecución del comando 
		// (Esta función es pública por el evento Closing de MainView. Ver más información en dicho evento.)
		public void Guardar() {

			// Guardamos todo.
			GuardarTodo();
		}
		#endregion


		#region DROPBOX A PC
		//Comando
		private ICommand _cmddropboxapc;
		public ICommand cmdDropboxAPc {
			get {
				if (_cmddropboxapc == null) _cmddropboxapc = new RelayCommand(p => DropboxAPc());
				return _cmddropboxapc;
			}
		}

		// Ejecución del comando
		private void DropboxAPc() {

			if (mensajes.VerMensaje("¡ATENCIÓN!\n\nVas a copiar los datos de Dropbox al ordenador.\n\n¿Desea continuar?",
									   "Copiar de Dropbox a PC", true) == false) return;

			try {
				// Hacemos copia de seguridad de los datos de Dropbox.
				if (!Respaldo.CopiaDatos()) {
					if (mensajes.VerMensaje("¡ATENCIÓN!\n\nNo se ha podido hacer una copia de seguridad de los datos.\n\n¿Desea continuar?",
									   "Copiar de Dropbox a PC", true) == false) return;
				}
				// Copiamos la carpeta Datos a Dropbox.
				Respaldo.DropboxToDatos();
			} catch (Exception ex) {
				mensajes.VerError("OpcionesCommands.DropboxAPc", ex);
			}

		}
		#endregion


		#region PC A DROPBOX
		//Comando
		private ICommand _cmdpcadropbox;
		public ICommand cmdPcADropbox {
			get {
				if (_cmdpcadropbox == null) _cmdpcadropbox = new RelayCommand(p => PcADropbox());
				return _cmdpcadropbox;
			}
		}

		// Ejecución del comando
		private void PcADropbox() {

			if (mensajes.VerMensaje("¡ATENCIÓN!\n\nVas a copiar los datos del ordenador a Dropbox.\n\n¿Desea continuar?",
									   "Copiar de PC a Dropbox", true) == false) return;

			try {
				// Hacemos copia de seguridad de los datos de Dropbox.
				if (!Respaldo.CopiaDropbox()) {
					if (mensajes.VerMensaje("¡ATENCIÓN!\n\nNo se ha podido hacer una copia de seguridad de Dropbox.\n\n¿Desea continuar?",
									   "Copiar de PC a Dropbox", true) == false) return;
				}
				// Copiamos la carpeta Datos a Dropbox.
				Respaldo.DatosToDropbox();
			} catch (Exception ex) {
				mensajes.VerError("OpcionesCommands.PcADropbox", ex);
			}

		}
		#endregion


		#region MOSTRAR GENERALES
		//Comando
		private ICommand _cmdmostrargenerales;
		public ICommand cmdMostrarGenerales {
			get {
				if (_cmdmostrargenerales == null) _cmdmostrargenerales = new RelayCommand(p => MostrarGenerales(), p => PuedeMostrarGenerales());
				return _cmdmostrargenerales;
			}
		}

		// Puede ejecutar el comando
		private bool PuedeMostrarGenerales() {
			return VisibilidadPanelGenerales == Visibility.Collapsed;
		}

		// Ejecución del comando 
		private void MostrarGenerales() {

			CargarDatos();
			VisibilidadPanelConvenio = Visibility.Collapsed;
			VisibilidadPanelPorCentro = Visibility.Collapsed;
			VisibilidadPanelGenerales = Visibility.Visible;
		}
		#endregion


		#region MOSTRAR CONVENIO
		//Comando
		private ICommand _cmdmostrarconvenio;
		public ICommand cmdMostrarConvenio {
			get {
				if (_cmdmostrarconvenio == null) _cmdmostrarconvenio = new RelayCommand(p => MostrarConvenio(), p => PuedeMostrarConvenio());
				return _cmdmostrarconvenio;
			}
		}

		// Puede ejecutar el comando
		private bool PuedeMostrarConvenio() {
			return VisibilidadPanelConvenio == Visibility.Collapsed;
		}

		// Ejecución del comando 
		private void MostrarConvenio() {

			VisibilidadPanelGenerales = Visibility.Collapsed;
			VisibilidadPanelPorCentro = Visibility.Collapsed;
			VisibilidadPanelConvenio = Visibility.Visible;

		}
		#endregion


		#region MOSTRAR POR CENTRO
		//Comando
		private ICommand _cmdmostrarporcentro;
		public ICommand cmdMostrarPorCentro {
			get {
				if (_cmdmostrarporcentro == null) _cmdmostrarporcentro = new RelayCommand(p => MostrarPorCentro(), p => PuedeMostrarPorCentro());
				return _cmdmostrarporcentro;
			}
		}

		// Puede ejecutar el comando
		private bool PuedeMostrarPorCentro() {
			return VisibilidadPanelPorCentro == Visibility.Collapsed;
		}

		// Ejecución del comando 
		private void MostrarPorCentro() {

			VisibilidadPanelGenerales = Visibility.Collapsed;
			VisibilidadPanelConvenio = Visibility.Collapsed;
			VisibilidadPanelPorCentro = Visibility.Visible;

		}
		#endregion


		#region AÑO FESTIVOS MENOS
		//Comando
		private ICommand _cmdañofestivosmenos;
		public ICommand cmdAñoFestivosMenos {
			get {
				if (_cmdañofestivosmenos == null) _cmdañofestivosmenos = new RelayCommand(p => AñoFestivosMenos());
				return _cmdañofestivosmenos;
			}
		}

		// Ejecución del comando 
		private void AñoFestivosMenos() {

			//GuardarDatos();
			//AñoFestivos--;
			//CargarDatos();
			App.Global.FestivosVM.AñoActual--;

		}
		#endregion


		#region AÑO FESTIVOS MAS
		//Comando
		private ICommand _cmdañofestivosmas;
		public ICommand cmdAñoFestivosMas {
			get {
				if (_cmdañofestivosmas == null) _cmdañofestivosmas = new RelayCommand(p => AñoFestivosMas());
				return _cmdañofestivosmas;
			}
		}

		// Ejecución del comando 
		private void AñoFestivosMas() {

			//GuardarDatos();
			//AñoFestivos++;
			//CargarDatos();
			App.Global.FestivosVM.AñoActual++;

		}
		#endregion


		//#region BORRAR CELDAS FESTIVOS
		//private ICommand _cmdborrarceldasfestivos;
		//public ICommand cmdBorrarCeldasFestivos {
		//	get {
		//		if (_cmdborrarceldasfestivos == null) _cmdborrarceldasfestivos = new RelayCommand(p => BorrarCeldasFestivos(p));
		//		return _cmdborrarceldasfestivos;
		//	}
		//}

		//private void BorrarCeldasFestivos(object parametro) {
		//	if (parametro == null) return;
		//	System.Windows.Controls.DataGrid tabla = (System.Windows.Controls.DataGrid)parametro;
		//	List<Festivo> lista = new List<Festivo>();
		//	foreach (DataGridCellInfo celda in tabla.SelectedCells) {
		//		lista.Add((Festivo)celda.Item);
		//	}
		//	foreach (Festivo f in lista) {
		//		//_listaborrados.Add(f);
		//		//_listafestivos.Remove(f);
		//		App.Global.FestivosVM.Borrar(f);
		//	}
		//	lista.Clear();

		//}
		//#endregion


		#region COPIA SEGURIDAD MANUAL
		//Comando
		private ICommand _cmdcopiaseguridadmanual;
		public ICommand cmdCopiaSeguridadManual {
			get {
				if (_cmdcopiaseguridadmanual == null) _cmdcopiaseguridadmanual = new RelayCommand(p => CopiaSeguridadManual());
				return _cmdcopiaseguridadmanual;
			}
		}

		// Ejecución del comando
		private void CopiaSeguridadManual() {

			if (App.Global.HayCambios) {
				if (mensajes.VerMensaje("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?\n\n" +
												"(Los cambios no guardados no se reflejarán en la copia de seguridad.)",
												"NO SE HAN GUARDADO LOS CAMBIOS", true) == true)
					App.Global.GuardarCambios();
			}

			Respaldo.CopiaDatos("Manual");
			App.Global.Configuracion.UltimaCopia = DateTime.Now;
			mensajes.VerMensaje("Copia de seguridad creada correctamente.", "Copia Seguridad");
		}
		#endregion


		#region CAMBIAR CONTRASEÑA
		//Comando
		private ICommand _cmdcambiarcontraseña;
		public ICommand cmdCambiarContraseña {
			get {
				if (_cmdcambiarcontraseña == null) _cmdcambiarcontraseña = new RelayCommand(p => CambiarContraseña());
				return _cmdcambiarcontraseña;
			}
		}

		// Ejecución del comando
		private void CambiarContraseña() {

			if (App.Global.HayCambios) {
				if (mensajes.VerMensaje("¡¡ ATENCIÓN !!\n\nHay cambios sin guardar.\n\n¿Desea guardar los cambios?\n\n",
												"NO SE HAN GUARDADO LOS CAMBIOS", true) == true)
					App.Global.GuardarCambios();
			}

			App.Global.TextoProgreso = "Cambiando Contraseñas ...";
			App.Global.VisibilidadProgreso = Visibility.Visible;

			try {
				VentanaCambioContraseña ventana = new VentanaCambioContraseña();
				if (ventana.ShowDialog() == true) {
					//Si la contraseña anterior no es la que estaba...
					if (Utils.CodificaTexto(ventana.PwAnterior.Password) != App.Global.Configuracion.ContraseñaDatos) {
						mensajes.VerMensaje("La contraseña anterior no es válida", "ERROR");
					} else {
						// Guardamos la contraseña nueva, por si acaso.
						App.Global.Configuracion.ContraseñaDatos = Utils.CodificaTexto(ventana.PwNueva.Password);
						App.Global.Configuracion.Guardar(App.Global.ArchivoOpcionesConfiguracion);
					}
				}
			} catch (Exception ex) {
				Utils.VerError("OpcionesCommands.CambiarContraseña", ex);
			} finally {
				App.Global.VisibilidadProgreso = Visibility.Collapsed;
			}
		}
		#endregion


		#region INCREMENTAR IMPORTES
		//Comando
		private ICommand _cmdincrementarimportes;
		public ICommand cmdIncrementarImportes {
			get {
				if (_cmdincrementarimportes == null) _cmdincrementarimportes = new RelayCommand(p => IncrementarImportes());
				return _cmdincrementarimportes;
			}
		}

		// Ejecución del comando
		private void IncrementarImportes() {

			if (PorcentajeIncremento == 0m) return;

			decimal multiplicador = 1 + (PorcentajeIncremento < 0 ? PorcentajeIncremento * -1 : PorcentajeIncremento) / 100;

			StringBuilder mensaje = new StringBuilder();
			mensaje.Append($"NUEVOS VALORES\n\n");
			if (PorcentajeIncremento < 0)
			{
				mensaje.Append($"Importe Dietas: {PlusesActuales.ImporteDietas / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Sábados: {PlusesActuales.ImporteSabados / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Festivos: {PlusesActuales.ImporteFestivos / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Nocturnidad: {PlusesActuales.PlusNocturnidad / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Menor Descanso: {PlusesActuales.DietaMenorDescanso / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Limpieza: {PlusesActuales.PlusLimpieza / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Paquetería: {PlusesActuales.PlusPaqueteria / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Navidad: {PlusesActuales.PlusNavidad / multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Viaje: {App.Global.PorCentro.PlusViaje / multiplicador:0.00}\n\n");
			} else
			{
				mensaje.Append($"Importe Dietas: {PlusesActuales.ImporteDietas * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Sábados: {PlusesActuales.ImporteSabados * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Festivos: {PlusesActuales.ImporteFestivos * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Nocturnidad: {PlusesActuales.PlusNocturnidad * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Menor Descanso: {PlusesActuales.DietaMenorDescanso * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Limpieza: {PlusesActuales.PlusLimpieza * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Paquetería: {PlusesActuales.PlusPaqueteria * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Navidad: {PlusesActuales.PlusNavidad * multiplicador:0.00}\n");
				mensaje.Append($"Importe Plus Viaje: {App.Global.PorCentro.PlusViaje * multiplicador:0.00}\n\n");
			}
			mensaje.Append($"¿Aceptar?");

			if (mensajes.VerMensaje(mensaje.ToString(), "Incremento de importes", true) == true) {
				if (PorcentajeIncremento < 0)
				{
					PlusesActuales.ImporteDietas /= multiplicador;
					PlusesActuales.ImporteSabados /= multiplicador;
					PlusesActuales.ImporteFestivos /= multiplicador;
					PlusesActuales.PlusNocturnidad /= multiplicador;
					PlusesActuales.DietaMenorDescanso /= multiplicador;
					PlusesActuales.PlusLimpieza /= multiplicador;
					PlusesActuales.PlusPaqueteria /= multiplicador;
					PlusesActuales.PlusNavidad /= multiplicador;
					App.Global.PorCentro.PlusViaje /= multiplicador;
				} else {
					PlusesActuales.ImporteDietas *= multiplicador;
					PlusesActuales.ImporteSabados *= multiplicador;
					PlusesActuales.ImporteFestivos *= multiplicador;
					PlusesActuales.PlusNocturnidad *= multiplicador;
					PlusesActuales.DietaMenorDescanso *= multiplicador;
					PlusesActuales.PlusLimpieza *= multiplicador;
					PlusesActuales.PlusPaqueteria *= multiplicador;
					PlusesActuales.PlusNavidad *= multiplicador;
					App.Global.PorCentro.PlusViaje *= multiplicador;
				}

			}

		}
		#endregion


		#region ACERCA DE

		// Comando
		private ICommand _cmdacercade;
		public ICommand cmdAcercaDe {
			get {
				if (_cmdacercade == null) _cmdacercade = new RelayCommand(p => AcercaDe());
				return _cmdacercade;
			}
		}


		// Ejecución del comando
		private void AcercaDe() {

			VentanaAcercaDe ventana = new VentanaAcercaDe() { DataContext = new VentanaAcercaDeVM() };
			ventana.ShowDialog();

		}
        #endregion


        #region RUTA LOGOS

        // Comando
        private ICommand _cmdrutalogos;
        public ICommand cmdRutaLogos {
            get {
                if (_cmdrutalogos == null) _cmdrutalogos = new RelayCommand(p => RutaLogos(p));
                return _cmdrutalogos;
            }
        }

        // Ejecución del comando
        private void RutaLogos(object parametro) {

            string texto = parametro as string;
            if (texto == null) return;

            string ruta = App.RutaInicial;
            OpenFileDialog dialogo = new OpenFileDialog();
            dialogo.Filter = "Archivos Imagen|*.jpg;*.png|Todos los archivos|*.*";
            switch (texto) {
                case "Logo":
                    if (!string.IsNullOrWhiteSpace(App.Global.Configuracion.RutaLogoSindicato)) ruta = Path.GetDirectoryName(App.Global.Configuracion.RutaLogoSindicato);
                    dialogo.Title = "Logo del Sindicato";
                    dialogo.FileName = Path.GetFileName(App.Global.Configuracion.RutaLogoSindicato);
                    break;
                case "MarcaAgua":
                    if (!string.IsNullOrWhiteSpace(App.Global.Configuracion.RutaMarcaAgua)) ruta = Path.GetDirectoryName(App.Global.Configuracion.RutaMarcaAgua);
                    dialogo.Title = "Marca de Agua";
                    dialogo.FileName = Path.GetFileName(App.Global.Configuracion.RutaMarcaAgua);
                    break;
            }
            dialogo.InitialDirectory = ruta;
            if (dialogo.ShowDialog() == DialogResult.OK) {
                switch (texto) {
                    case "Logo":
                        App.Global.Configuracion.RutaLogoSindicato = dialogo.FileName;
                        break;
                    case "MarcaAgua":
                        App.Global.Configuracion.RutaMarcaAgua = dialogo.FileName;
                        break;
                }
            }


        }
		#endregion


		#region COMANDO AÑO PLUSES MENOS

		// Comando
		private ICommand _cmdañoplusesmenos;
		public ICommand cmdAñoPlusesMenos
		{
			get
			{
				if (_cmdañoplusesmenos == null) _cmdañoplusesmenos = new RelayCommand(p => AñoPlusesMenos());
				return _cmdañoplusesmenos;
			}
		}


		// Ejecución del comando
		private void AñoPlusesMenos()
		{
			if (!ListaPluses.Any(p => p.Año == AñoPluses - 1))
			{
				if (mensajes.VerMensaje($"El año {AñoPluses - 1} no existe.\n\n" +
					$"Se va a crear un nuevo año con los valores del año actual.\n\n¿Desea continuar?", "Año no existente.", true) == false) return;
			}
			AñoPluses--;
		}
		#endregion


		#region COMANDO AÑO PLUSES MAS

		// Comando
		private ICommand _cmdañoplusesmas;
		public ICommand cmdAñoPlusesMas
		{
			get
			{
				if (_cmdañoplusesmas == null) _cmdañoplusesmas = new RelayCommand(p => AñoPlusesMas());
				return _cmdañoplusesmas;
			}
		}


		// Ejecución del comando
		private void AñoPlusesMas()
		{
			if (!ListaPluses.Any(p => p.Año == AñoPluses + 1))
			{
				if (mensajes.VerMensaje($"El año {AñoPluses + 1} no existe.\n\n" +
					$"Se va a crear un nuevo año con los valores del año actual.\n\n¿Desea continuar?", "Año no existente.", true) == false) return;
			}
			AñoPluses++;
		}
        #endregion


    }
}
