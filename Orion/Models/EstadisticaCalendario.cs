#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models
{
    public class EstadisticaCalendario {




        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public int Dia { get; set; }
        
        public int Turno1 { get; set; }

        public int Turno2 { get; set; }

        public int Turno3 { get; set; }

        public int Turno4 { get; set; }

        public int TotalDias { get; set; }

        public int TotalJornadas { get; set; }

        public int JornadasMenoresMedia { get; set; }

        public int JornadasMayoresMedia { get; set; }

        public TimeSpan Trabajadas { get; set; }

        public TimeSpan TiempoPartido { get; set; }

        public TimeSpan Acumuladas { get; set; }

        public TimeSpan Nocturnas { get; set; }

        public TimeSpan MediaTrabajadas { get; set; }

        public TimeSpan HorasNegativas { get; set; }

        public decimal Desayuno { get; set; }

        public decimal Comida { get; set; }

        public decimal Cena { get; set; }

        public decimal PlusCena { get; set; }

        public decimal ImporteDesayuno { get; set; }

        public decimal ImporteComida { get; set; }

        public decimal ImporteCena { get; set; }

        public decimal ImportePlusCena { get; set; }

        public decimal PlusNocturnidad { get; set; }

        public decimal PlusMenorDescanso { get; set; }

        public decimal PlusLimipeza { get; set; }

        public decimal PlusPaqueteria { get; set; }

        public decimal PlusNavidad { get; set; }





        #endregion
        // ====================================================================================================


    }
}
