#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

namespace Orion {

    /// <summary>
    /// Esta clase contiene la definición de las claves de los diferentes servicios y APIs que se usan. 
    /// Las claves reales están instanciadas en el constructor que ponemos en otro archivo llamado ApiKeysLocal.cs 
    /// que no se comparte con GitHub para no exponer las claves secretas.
    /// </summary>
    public static partial class ApiKeys {

        /* DROPBOX */
        public static readonly string DropboxAppKey = "";
        public static readonly string DropboxAppKeySecret = "";


        /* ONE DRIVE */
        public static readonly string OneDriveClientId = "";
        public static readonly string OneDriveClientSecret = "";
        public static readonly string OneDriveTenantId = "";
        public static readonly string OneDriveScope = "";

    }
}
