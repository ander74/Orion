#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion.Interfaces {

    /// <summary>
    /// Define operaciones con archivos, tales como abrir diálogos para seleccionar archivos o carpetas.
    /// </summary>
    public interface IFileService {

        string FileDialog(string title, string defaultPath);
    }
}
