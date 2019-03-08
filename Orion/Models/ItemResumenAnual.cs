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

namespace Orion.Models {

	public class ItemResumenAnual {


		// ====================================================================================================
		#region CAMPOS PRIVADOS
		// ====================================================================================================

		private TimeSpan horaAcumulada = TimeSpan.Zero;
		private int diasAcumulados = 0;
		private decimal numeroAcumulado = 0m;

		private string[] Datos = new string[13];

		private string[] Acumulado = new string[13];

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================

		public ItemResumenAnual() { }


		public ItemResumenAnual(string descripcion) {
			this.Descripcion = descripcion;
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region MÉTODOS PÚBLICOS
		// ====================================================================================================

		public void SetDato(int mes, TimeSpan hora) {
			Datos[mes] = hora.Ticks == 0 ? "" : $"{hora.ToTexto(true)} ({hora.ToDecimal():0.00})";
			horaAcumulada += hora;
			Acumulado[mes] = horaAcumulada.Ticks == 0 ? "" : $"{horaAcumulada.ToTexto(true)} ({horaAcumulada.ToDecimal():0.00})";
		}

		public void SetDato(int mes, int dias) {
			Datos[mes] = dias == 0 ? "" : dias.ToString("00");
			diasAcumulados += dias;
			Acumulado[mes] = diasAcumulados == 0 ? "" : diasAcumulados.ToString("00");
		}

		public void SetDato(int mes, decimal numero) {
			Datos[mes] = Math.Round(numero, 2) == 0 ? "" : Math.Round(numero, 2).ToString("0.00");
			numeroAcumulado += numero;
			Acumulado[mes] = Math.Round(numeroAcumulado, 2) == 0 ? "" : Math.Round(numeroAcumulado, 2).ToString("0.00");
		}

		public void SetDatoEventual(int mes, int dias, decimal diasEventual)
		{
			Datos[mes] = (dias == 0 ? "" : dias.ToString("00")) + (Math.Round(diasEventual, 2) == 0 ? "" : " (" + Math.Round(diasEventual, 2).ToString("0.00") + ")");
			diasAcumulados += dias;
			numeroAcumulado += diasEventual;
			Acumulado[mes] = (diasAcumulados == 0 ? "" : diasAcumulados.ToString("00")) + (Math.Round(numeroAcumulado, 2) == 0 ? "" : " (" + Math.Round(numeroAcumulado, 2).ToString("0.00") + ")");
		}

		#endregion
		// ====================================================================================================


		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================

		public string Descripcion { get; set; }

		public string Enero {
			get { return Datos[1] + "\n" + Acumulado[1]; }
		}

		public string Febrero {
			get { return Datos[2] + "\n" + Acumulado[2]; }
		}

		public string Marzo {
			get { return Datos[3] + "\n" + Acumulado[3]; }
		}

		public string Abril {
			get { return Datos[4] + "\n" + Acumulado[4]; }
		}

		public string Mayo {
			get { return Datos[5] + "\n" + Acumulado[5]; }
		}

		public string Junio {
			get { return Datos[6] + "\n" + Acumulado[6]; }
		}

		public string Julio {
			get { return Datos[7] + "\n" + Acumulado[7]; }
		}

		public string Agosto {
			get { return Datos[8] + "\n" + Acumulado[8]; }
		}

		public string Septiembre {
			get { return Datos[9] + "\n" + Acumulado[9]; }
		}

		public string Octubre {
			get { return Datos[10] + "\n" + Acumulado[10]; }
		}

		public string Noviembre {
			get { return Datos[11] + "\n" + Acumulado[11]; }
		}

		public string Diciembre {
			get { return Datos[12] + "\n" + Acumulado[12]; }
		}

		#endregion
		// ====================================================================================================


	}
}
