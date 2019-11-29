#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Microsoft.Win32;
using Orion.Interfaces;

namespace Orion.Servicios {
    public class FileService : IFileService {

        public string FileDialog(string title, string defaultPath) {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = title;
            dialog.CheckPathExists = true;
            dialog.InitialDirectory = defaultPath;

            if (dialog.ShowDialog() == true) {
                return dialog.FileName;
            }

            return "";
        }
    }
}
