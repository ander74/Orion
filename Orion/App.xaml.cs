#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.IO;
using System.Reflection;
using System.Windows;
using Orion.ViewModels;
using Orion.Views;

namespace Orion {

    // ENUMERACIÓN DE CENTROS
    public enum Centros : int { Desconocido = 0, Bilbao = 1, Donosti = 2, Arrasate = 3, Vitoria = 4, Lineas = 5 }


    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application {

        /// <summary>
        /// ViewModel global para toda la aplicación.
        /// </summary>
        public static GlobalVM Global = new GlobalVM();

        // Ruta del programa.
        public static string RutaInicial = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // Calculadora
        public static VentanaCalculadora Calculadora;

        // Actualizar al salir
        public static bool ActualizarAlSalir = false;

        public static bool ActualizacionDisponible = false;

    }
}
