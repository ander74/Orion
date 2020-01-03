#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Text.RegularExpressions;

namespace Orion.Config {

    public class IntRangoCollection : RangoCollection<int> {

        // ====================================================================================================
        #region CONSTRUCTORES
        // ====================================================================================================

        public IntRangoCollection() { }


        public IntRangoCollection(string rangoExpression) {
            rangoExpression = rangoExpression.Replace(" ", "");
            if (string.IsNullOrEmpty(rangoExpression)) return;
            var rangos = rangoExpression.Split(',');
            var patronNumeroUnico = new Regex(@"^\d+$");
            var patronRango = new Regex(@"^\d+-{1}\d+$");
            foreach (var rango in rangos) {
                // Chequear si es un solo numero.
                if (patronNumeroUnico.IsMatch(rango)) {
                    int valor = int.Parse(rango);
                    this.Add(new Rango<int>(valor));
                    continue;
                }
                if (patronRango.IsMatch(rango)) {
                    var valores = rango.Split('-');
                    int min = int.Parse(valores[0]);
                    int max = int.Parse(valores[1]);
                    if (min > max) continue;
                    this.Add(new Rango<int>(min, max));
                    continue;
                }
            }
        }

        #endregion
        // ====================================================================================================



    }

}
