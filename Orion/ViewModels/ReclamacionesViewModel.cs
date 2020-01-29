#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Orion.Models;

namespace Orion.ViewModels {
	public class ReclamacionesViewModel : NotifyBase {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region COMANDOS
		// ====================================================================================================

		#region BORRAR DÍA GRÁFICO
		// Comando
		private ICommand _cmdborrardiagrafico;
		public ICommand cmdBorrarDiaGrafico {
			get {
				if (_cmdborrardiagrafico == null) _cmdborrardiagrafico = new RelayCommand(p => BorrarDiaGrafico(), p => PuedeBorrarDiaGrafico());
				return _cmdborrardiagrafico;
			}
		}

		// Se puede ejecutar
		private bool PuedeBorrarDiaGrafico() {
			return DiaGraficoSeleccionado != null;
		}

		// Ejecución del comando
		private void BorrarDiaGrafico() {

			ListaGraficos.Remove(DiaGraficoSeleccionado);
			DiaGraficoSeleccionado = null;

		}
		#endregion



		#region COMANDO MostrarNotas

		// Comando
		private ICommand _cmdmostrarnotas;
		public ICommand cmdMostrarNotas {
			get {
				if (_cmdmostrarnotas == null) _cmdmostrarnotas = new RelayCommand(p => MostrarNotas());
				return _cmdmostrarnotas;
			}
		}


		// Ejecución del comando
		private void MostrarNotas() {
			VisibilidadNotas = !VisibilidadNotas;
		}
		#endregion




		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES GENERALES
		// ====================================================================================================

		private DateTime _fechareclamacion = DateTime.Today;
		public DateTime FechaReclamacion {
			get { return _fechareclamacion; }
			set {
				if (_fechareclamacion != value) {
					_fechareclamacion = value;
					PropiedadCambiada();
				}
			}
		}


		public int ConceptosReclamados {
			get {
				int i = ListaGraficos.Count;
				// Horas
				if (TrabajadasChecked) i++;
				if (AcumuladasChecked) i++;
				if (NocturnasChecked) i++;
				if (CobradasChecked) i++;
				// Importes
				if (DietasChecked) i++;
				if (SabadosChecked) i++;
				if (FestivosChecked) i++;
				if (MenorDescansoChecked) i++;
				if (LimpiezaChecked) i++;
				if (PaqueteriaChecked) i++;
				if (NocturnidadChecked) i++;
				if (ViajeChecked) i++;
				if (NavidadChecked) i++;
				// Días
				if (DescansosChecked) i++;
				if (VacacionesChecked) i++;
				if (DescansosNoDisfrutadosChecked) i++;
				if (EnfermoChecked) i++;
				if (DescansosSueltosChecked) i++;
				if (DescansosCompensatoriosChecked) i++;
				if (PermisoChecked) i++;
				if (LibreDisposicionChecked) i++;
				if (ComiteChecked) i++;
				// Resultado
				return i;
			}
		}


		private string _notas;
		public string Notas {
			get { return _notas; }
			set {
				if (_notas != value) {
					_notas = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _visibilidadnotas;
		public bool VisibilidadNotas {
			get { return _visibilidadnotas; }
			set {
				if (_visibilidadnotas != value) {
					_visibilidadnotas = value;
					PropiedadCambiada();
				}
			}
		}


		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES HORAS
		// ====================================================================================================


		private bool _trabajadaschecked;
		public bool TrabajadasChecked {
			get { return _trabajadaschecked; }
			set {
				if (_trabajadaschecked != value) {
					_trabajadaschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _acumuladaschecked;
		public bool AcumuladasChecked {
			get { return _acumuladaschecked; }
			set {
				if (_acumuladaschecked != value) {
					_acumuladaschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _nocturnaschecked;
		public bool NocturnasChecked {
			get { return _nocturnaschecked; }
			set {
				if (_nocturnaschecked != value) {
					_nocturnaschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _cobradaschecked;
		public bool CobradasChecked {
			get { return _cobradaschecked; }
			set {
				if (_cobradaschecked != value) {
					_cobradaschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _trabajadas;
		public TimeSpan Trabajadas {
			get { return _trabajadas; }
			set {
				if (_trabajadas != value) {
					_trabajadas = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _acumuladas;
		public TimeSpan Acumuladas {
			get { return _acumuladas; }
			set {
				if (_acumuladas != value) {
					_acumuladas = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _nocturnas;
		public TimeSpan Nocturnas {
			get { return _nocturnas; }
			set {
				if (_nocturnas != value) {
					_nocturnas = value;
					PropiedadCambiada();
				}
			}
		}


		private TimeSpan _cobradas;
		public TimeSpan Cobradas {
			get { return _cobradas; }
			set {
				if (_cobradas != value) {
					_cobradas = value;
					PropiedadCambiada();
				}
			}
		}






		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES DÍAS
		// ====================================================================================================


		private bool _descansoschecked;
		public bool DescansosChecked {
			get { return _descansoschecked; }
			set {
				if (_descansoschecked != value) {
					_descansoschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _vacacioneschecked;
		public bool VacacionesChecked {
			get { return _vacacioneschecked; }
			set {
				if (_vacacioneschecked != value) {
					_vacacioneschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _descansosnodisfrutadoschecked;
		public bool DescansosNoDisfrutadosChecked {
			get { return _descansosnodisfrutadoschecked; }
			set {
				if (_descansosnodisfrutadoschecked != value) {
					_descansosnodisfrutadoschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _enfermochecked;
		public bool EnfermoChecked {
			get { return _enfermochecked; }
			set {
				if (_enfermochecked != value) {
					_enfermochecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _descansossueltoschecked;
		public bool DescansosSueltosChecked {
			get { return _descansossueltoschecked; }
			set {
				if (_descansossueltoschecked != value) {
					_descansossueltoschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _descansoscompensatorioschecked;
		public bool DescansosCompensatoriosChecked {
			get { return _descansoscompensatorioschecked; }
			set {
				if (_descansoscompensatorioschecked != value) {
					_descansoscompensatorioschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _permisochecked;
		public bool PermisoChecked {
			get { return _permisochecked; }
			set {
				if (_permisochecked != value) {
					_permisochecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _libredisposicionchecked;
		public bool LibreDisposicionChecked {
			get { return _libredisposicionchecked; }
			set {
				if (_libredisposicionchecked != value) {
					_libredisposicionchecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _comitechecked;
		public bool ComiteChecked {
			get { return _comitechecked; }
			set {
				if (_comitechecked != value) {
					_comitechecked = value;
					PropiedadCambiada();
				}
			}
		}


		private int _descansos;
		public int Descansos {
			get { return _descansos; }
			set {
				if (_descansos != value) {
					_descansos = value;
					PropiedadCambiada();
				}
			}
		}


		private int _vacaciones;
		public int Vacaciones {
			get { return _vacaciones; }
			set {
				if (_vacaciones != value) {
					_vacaciones = value;
					PropiedadCambiada();
				}
			}
		}


		private int _descansosnodisfrutados;
		public int DescansosNoDisfrutados {
			get { return _descansosnodisfrutados; }
			set {
				if (_descansosnodisfrutados != value) {
					_descansosnodisfrutados = value;
					PropiedadCambiada();
				}
			}
		}


		private int _enfermo;
		public int Enfermo {
			get { return _enfermo; }
			set {
				if (_enfermo != value) {
					_enfermo = value;
					PropiedadCambiada();
				}
			}
		}


		private int _descansossueltos;
		public int DescansosSueltos {
			get { return _descansossueltos; }
			set {
				if (_descansossueltos != value) {
					_descansossueltos = value;
					PropiedadCambiada();
				}
			}
		}


		private int _descansoscompensatorios;
		public int DescansosCompensatorios {
			get { return _descansoscompensatorios; }
			set {
				if (_descansoscompensatorios != value) {
					_descansoscompensatorios = value;
					PropiedadCambiada();
				}
			}
		}


		private int _permiso;
		public int Permiso {
			get { return _permiso; }
			set {
				if (_permiso != value) {
					_permiso = value;
					PropiedadCambiada();
				}
			}
		}


		private int _libredisposicion;
		public int LibreDisposicion {
			get { return _libredisposicion; }
			set {
				if (_libredisposicion != value) {
					_libredisposicion = value;
					PropiedadCambiada();
				}
			}
		}


		private int _comite;
		public int Comite {
			get { return _comite; }
			set {
				if (_comite != value) {
					_comite = value;
					PropiedadCambiada();
				}
			}
		}






		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES IMPORTES
		// ====================================================================================================


		private bool _dietaschecked;
		public bool DietasChecked {
			get { return _dietaschecked; }
			set {
				if (_dietaschecked != value) {
					_dietaschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _sabadoschecked;
		public bool SabadosChecked {
			get { return _sabadoschecked; }
			set {
				if (_sabadoschecked != value) {
					_sabadoschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _festivoschecked;
		public bool FestivosChecked {
			get { return _festivoschecked; }
			set {
				if (_festivoschecked != value) {
					_festivoschecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _menordescansochecked;
		public bool MenorDescansoChecked {
			get { return _menordescansochecked; }
			set {
				if (_menordescansochecked != value) {
					_menordescansochecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _limpiezachecked;
		public bool LimpiezaChecked {
			get { return _limpiezachecked; }
			set {
				if (_limpiezachecked != value) {
					_limpiezachecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _paqueteriachecked;
		public bool PaqueteriaChecked {
			get { return _paqueteriachecked; }
			set {
				if (_paqueteriachecked != value) {
					_paqueteriachecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _nocturnidadchecked;
		public bool NocturnidadChecked {
			get { return _nocturnidadchecked; }
			set {
				if (_nocturnidadchecked != value) {
					_nocturnidadchecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _viajechecked;
		public bool ViajeChecked {
			get { return _viajechecked; }
			set {
				if (_viajechecked != value) {
					_viajechecked = value;
					PropiedadCambiada();
				}
			}
		}


		private bool _navidadchecked;
		public bool NavidadChecked {
			get { return _navidadchecked; }
			set {
				if (_navidadchecked != value) {
					_navidadchecked = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _dietas;
		public decimal Dietas {
			get { return _dietas; }
			set {
				if (_dietas != value) {
					_dietas = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _sabados;
		public decimal Sabados {
			get { return _sabados; }
			set {
				if (_sabados != value) {
					_sabados = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _festivos;
		public decimal Festivos {
			get { return _festivos; }
			set {
				if (_festivos != value) {
					_festivos = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _menordescanso;
		public decimal MenorDescanso {
			get { return _menordescanso; }
			set {
				if (_menordescanso != value) {
					_menordescanso = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _limpieza;
		public decimal Limpieza {
			get { return _limpieza; }
			set {
				if (_limpieza != value) {
					_limpieza = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _paqueteria;
		public decimal Paqueteria {
			get { return _paqueteria; }
			set {
				if (_paqueteria != value) {
					_paqueteria = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _nocturnidad;
		public decimal Nocturnidad {
			get { return _nocturnidad; }
			set {
				if (_nocturnidad != value) {
					_nocturnidad = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _viaje;
		public decimal Viaje {
			get { return _viaje; }
			set {
				if (_viaje != value) {
					_viaje = value;
					PropiedadCambiada();
				}
			}
		}


		private decimal _navidad;
		public decimal Navidad {
			get { return _navidad; }
			set {
				if (_navidad != value) {
					_navidad = value;
					PropiedadCambiada();
				}
			}
		}







		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES GRÁFICOS
		// ====================================================================================================

		private List<DiaGrafico> _listagraficos = new List<DiaGrafico>();
		public List<DiaGrafico> ListaGraficos {
			get { return _listagraficos; }
			set {
				if (_listagraficos != value) {
					_listagraficos = value;
					PropiedadCambiada();
				}
			}
		}


		private DiaGrafico _diagraficoseleccionado;
		public DiaGrafico DiaGraficoSeleccionado {
			get { return _diagraficoseleccionado; }
			set {
				if (_diagraficoseleccionado != value) {
					_diagraficoseleccionado = value;
					PropiedadCambiada();
				}
			}
		}


		/// <summary>
		/// Clase usada para la lista de gráficos a reclamar.
		/// </summary>
		public class DiaGrafico : NotifyBase {


			private int _dia;
			public int Dia {
				get { return _dia; }
				set {
					if (_dia != value) {
						_dia = value;
						PropiedadCambiada();
					}
				}
			}


			private int _grafico;
			public int Grafico {
				get { return _grafico; }
				set {
					if (_grafico != value) {
						_grafico = value;
						PropiedadCambiada();
					}
				}
			}


		}

		#endregion
		// ====================================================================================================








	}
}
