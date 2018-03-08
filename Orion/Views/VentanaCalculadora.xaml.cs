#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Convertidores;
using Orion.Models;
using Orion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orion.Views {
	/// <summary>
	/// Lógica de interacción para VentanaCalculadora.xaml
	/// </summary>
	public partial class VentanaCalculadora :Window {


		public VentanaCalculadora() {
			InitializeComponent();

		}

		private void Ventana_Closing(object sender, CancelEventArgs e) {
			// Activamos el botón y guardamos los cambios.
			App.Global.Configuracion.BotonCalculadoraActivo = true;
			App.Global.Configuracion.Guardar(App.Global.ArchivoOpcionesConfiguracion);
		}
	}
}
