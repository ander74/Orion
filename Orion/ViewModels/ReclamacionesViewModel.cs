#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Orion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.ViewModels
{
    public class ReclamacionesViewModel: NotifyBase{

		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================

		private string[] ConceptosHoras = new string[] {
			"Horas Acumuladas",
			"Horas Nocturnas",
		};


		private string[] ConceptosDias = new string[] {
			"Descansos",
			"Vacaciones",
			"Descansos no disfrutados",
			"Días Enfermo",
			"Descansos Sueltos",
			"Descansos Compensatorios",
			"Días de permiso",
			"Días de libre disposición",
			"Días de comité",
		};


		private string[] ConceptosImporte = new string[] {
			"Dietas",
			"Plus sábados",
			"Plus festivos",
			"Plus menor descanso",
			"Plus de limpieza",
			"Plus de media limpieza",
			"Plus de paquetería",
			"Plus de nocturnidad",
			"Plus de viaje",
			"Plus de Navidad"
		};



		#endregion
		// ====================================================================================================


	}
}
