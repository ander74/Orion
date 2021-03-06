﻿#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.DataModels {


	public static class Respaldo {


		/// <summary>
		/// Copia toda la carpeta de Datos a la carpeta de Dropbox.
		/// </summary>
		/// <returns>True si la copia fue satisfactoria.</returns>
		public static bool DatosToDropbox() {
			// Si la carpeta de Datos no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDatos)) return false;
			// Si la carpeta de Dropbox no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDropbox)) return false;
			// Copiamos todos los archivos de la carpeta de Datos a la de Dropbox.
			foreach (string archivo in Directory.GetFiles(App.Global.Configuracion.CarpetaDatos)) {
				File.Copy(archivo, Path.Combine(App.Global.Configuracion.CarpetaDropbox, Path.GetFileName(archivo)), true);
			}
			return true;
		}


		/// <summary>
		/// Copia toda la carpeta de Dropbox a la carpeta de Datos.
		/// </summary>
		/// <returns>True si la copia fue satisfactoria.</returns>
		public static bool DropboxToDatos() {
			// Si la carpeta de Dropbox no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDropbox)) return false;
			// Si la carpeta de Datos no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDatos)) return false;
			// Copiamos todos los archivos de la carpeta de Dropbox a la de Datos.
			foreach (string archivo in Directory.GetFiles(App.Global.Configuracion.CarpetaDropbox)) {
				File.Copy(archivo, Path.Combine(App.Global.Configuracion.CarpetaDatos, Path.GetFileName(archivo)), true);
			}
			return true;
		}


		/// <summary>
		/// Copia los archivos de la carpeta de Datos a una carpeta de Copia de Seguridad
		/// </summary>
		/// <param name="modificador">Texto que se añade al código de copia de seguridad.</param>
		/// <returns>True si la copia ha sido satisfactoria.</returns>
		public static bool CopiaDatos(string modificador = "Datos") {
			// Si la carpeta de Datos no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDatos)) return false;
			// Creamos el codigo de la copia de seguridad
			string codigo = String.Format("{0:yyyyMMddhhmmss}-{1}", DateTime.Now, modificador);
			// Definimos el nombre de la carpeta para la copia de seguridad.
			string carpeta = Path.Combine(App.Global.Configuracion.CarpetaCopiasSeguridad, codigo);
			// Si la carpeta definida no existe, la creamos.
			if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
			// Copiamos todos los archivos de la carpeta de Datos a la de Copia de Seguridad.
			foreach (string archivo in Directory.GetFiles(App.Global.Configuracion.CarpetaDatos)) {
				File.Copy(archivo, Path.Combine(carpeta, Path.GetFileName(archivo)));
			}
			return true;
		}


		/// <summary>
		/// Copia los archivos de la carpeta de Dropbox a una carpeta de Copia de Seguridad
		/// </summary>
		/// <param name="modificador">Texto que se añade al código de copia de seguridad.</param>
		/// <returns>True si la copia ha sido satisfactoria.</returns>
		public static bool CopiaDropbox(string modificador = "Dropbox") {
			// Si la carpeta de Datos no existe, salimos.
			if (!Directory.Exists(App.Global.Configuracion.CarpetaDropbox)) return false;
			// Creamos el codigo de la copia de seguridad
			string codigo = String.Format("{0:yyyyMMddhhmmss}-{1}", DateTime.Now, modificador);
			// Definimos el nombre de la carpeta para la copia de seguridad.
			string carpeta = Path.Combine(App.Global.Configuracion.CarpetaCopiasSeguridad, codigo);
			// Si la carpeta definida no existe, la creamos.
			if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
			// Copiamos todos los archivos de la carpeta de Datos a la de Copia de Seguridad.
			foreach (string archivo in Directory.GetFiles(App.Global.Configuracion.CarpetaDropbox)) {
				File.Copy(archivo, Path.Combine(carpeta, Path.GetFileName(archivo)));
			}
			return true;
		}


		/// <summary>
		/// Evalúa las fechas de modificación de los archivos de Dropbox y locales y actualiza los más antiguos.
		/// </summary>
		public static void SincronizarDropbox() {
			// Utilizar File.GetLastWriteTime para extraer la información de los archivos y compararlos.

			// ARCHIVO DE LÍNEAS
			string local = Path.Combine(App.Global.Configuracion.CarpetaDatos, "Lineas.accdb");
			string dropbox = Path.Combine(App.Global.Configuracion.CarpetaDropbox, "Lineas.accdb");
			if (File.GetLastWriteTime(local) > File.GetLastWriteTime(dropbox)) {
				File.Copy(local, dropbox, true);
			} else if (File.GetLastWriteTime(local) < File.GetLastWriteTime(dropbox)) {
				File.Copy(dropbox, local, true);
			}
			// ARRASATE
			local = Path.Combine(App.Global.Configuracion.CarpetaDatos, "Arrasate.accdb");
			dropbox = Path.Combine(App.Global.Configuracion.CarpetaDropbox, "Arrasate.accdb");
			if (File.GetLastWriteTime(local) > File.GetLastWriteTime(dropbox)) {
				File.Copy(local, dropbox, true);
			} else if (File.GetLastWriteTime(local) < File.GetLastWriteTime(dropbox)) {
				File.Copy(dropbox, local, true);
			}
			// BILBAO
			local = Path.Combine(App.Global.Configuracion.CarpetaDatos, "Bilbao.accdb");
			dropbox = Path.Combine(App.Global.Configuracion.CarpetaDropbox, "Bilbao.accdb");
			if (File.GetLastWriteTime(local) > File.GetLastWriteTime(dropbox)) {
				File.Copy(local, dropbox, true);
			} else if (File.GetLastWriteTime(local) < File.GetLastWriteTime(dropbox)) {
				File.Copy(dropbox, local, true);
			}
			// DONOSTI
			local = Path.Combine(App.Global.Configuracion.CarpetaDatos, "Donosti.accdb");
			dropbox = Path.Combine(App.Global.Configuracion.CarpetaDropbox, "Donosti.accdb");
			if (File.GetLastWriteTime(local) > File.GetLastWriteTime(dropbox)) {
				File.Copy(local, dropbox, true);
			} else if (File.GetLastWriteTime(local) < File.GetLastWriteTime(dropbox)) {
				File.Copy(dropbox, local, true);
			}
			// VITORIA
			local = Path.Combine(App.Global.Configuracion.CarpetaDatos, "Vitoria.accdb");
			dropbox = Path.Combine(App.Global.Configuracion.CarpetaDropbox, "Vitoria.accdb");
			if (File.GetLastWriteTime(local) > File.GetLastWriteTime(dropbox)) {
				File.Copy(local, dropbox, true);
			} else if (File.GetLastWriteTime(local) < File.GetLastWriteTime(dropbox)) {
				File.Copy(dropbox, local, true);
			}


		}


	}

}
