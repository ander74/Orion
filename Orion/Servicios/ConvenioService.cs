#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion


namespace Orion.Servicios {


    public class ConvenioService {


        // ====================================================================================================
        #region SINGLETON
        // ====================================================================================================

        private static ConvenioService instance;

        private ConvenioService() { }

        public static ConvenioService GetInstance() {
            if (instance == null) instance = new ConvenioService();
            return instance;
        }


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================

        //public bool ExisteArticulo(int numero) => Articulos.Any(a => a.Numero == numero);

        //public ArticuloConvenio GetArticulo(int numero) => Articulos.FirstOrDefault(a => a.Numero == numero);


        //public void LoadConvenio(string ruta) {
        //    if (File.Exists(ruta)) {
        //        string datos = File.ReadAllText(ruta, System.Text.Encoding.UTF8);
        //        JsonConvert.PopulateObject(datos, Articulos);
        //    }
        //}


        //public void SaveConvenio(string ruta) {
        //    string datos = JsonConvert.SerializeObject(Articulos, Formatting.Indented);
        //    File.WriteAllText(ruta, datos, System.Text.Encoding.UTF8);
        //}


        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================




        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CLASE ARTICULO CONVENIO
        // ====================================================================================================

        public class ArticuloConvenio {

        }

        #endregion
        // ====================================================================================================

    }
}
