namespace Orion.Models
{
	using System.Data.OleDb;

	public class Pluses : NotifyBase
	{

		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================

		public Pluses() { }


		public Pluses(OleDbDataReader lector)
		{
			ParseFromReader(lector, this);
		}

		/// <summary>
		/// Crea un nuevo objeto Pluses con los importes del objeto pasado, pero con el año indicado.
		/// </summary>
		/// <param name="año"></param>
		/// <param name="pluses"></param>
		public Pluses(int año, Pluses pluses)
		{
			this.Año = año;
			if (pluses != null) {
				this.ImporteDietas = pluses.ImporteDietas;
				this.ImporteSabados = pluses.ImporteSabados;
				this.ImporteFestivos = pluses.ImporteFestivos;
				this.PlusNocturnidad = pluses.PlusNocturnidad;
				this.DietaMenorDescanso = pluses.DietaMenorDescanso;
				this.PlusLimpieza = pluses.PlusLimpieza;
				this.PlusPaqueteria = pluses.PlusPaqueteria;
				this.PlusNavidad = pluses.PlusNavidad;
			}
			Nuevo = true;
		}

		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region MÉTODOS ESTÁTICOS
		// ====================================================================================================

		public static void ParseFromReader(OleDbDataReader lector, Pluses pluses)
		{
			pluses.Id = lector.ToInt32("Id");
			pluses.Año = lector.ToInt16("Año");
			pluses.ImporteDietas = lector.ToDecimal("ImporteDietas");
			pluses.ImporteSabados = lector.ToDecimal("ImporteSabados");
			pluses.ImporteFestivos = lector.ToDecimal("ImporteFestivos");
			pluses.PlusNocturnidad = lector.ToDecimal("PlusNocturnidad");
			pluses.DietaMenorDescanso = lector.ToDecimal("DietaMenorDescanso");
			pluses.PlusLimpieza = lector.ToDecimal("PlusLimpieza");
			pluses.PlusPaqueteria = lector.ToDecimal("PlusPaqueteria");
			pluses.PlusNavidad = lector.ToDecimal("PlusNavidad");
		}


		public static void ParseToCommand(OleDbCommand Comando, Pluses pluses)
		{
			Comando.Parameters.AddWithValue("Año", pluses.Año);
			Comando.Parameters.AddWithValue("ImporteDietas", pluses.ImporteDietas.ToString("0.0000"));
			Comando.Parameters.AddWithValue("ImporteSabados", pluses.ImporteSabados.ToString("0.0000"));
			Comando.Parameters.AddWithValue("ImporteFestivos", pluses.ImporteFestivos.ToString("0.0000"));
			Comando.Parameters.AddWithValue("PlusNocturnidad", pluses.PlusNocturnidad.ToString("0.0000"));
			Comando.Parameters.AddWithValue("DietaMenorDescanso", pluses.DietaMenorDescanso.ToString("0.0000"));
			Comando.Parameters.AddWithValue("PlusLimpieza", pluses.PlusLimpieza.ToString("0.0000"));
			Comando.Parameters.AddWithValue("PlusPaqueteria", pluses.PlusPaqueteria.ToString("0.0000"));
			Comando.Parameters.AddWithValue("PlusNavidad", pluses.PlusNavidad.ToString("0.0000"));
			Comando.Parameters.AddWithValue("Id", pluses.Id);
		}


		#endregion
		// ====================================================================================================



		// ====================================================================================================
		#region PROPIEDADES
		// ====================================================================================================


		private long _id;
		public long Id
		{
			get { return _id; }
			set { SetValue(ref _id, value); }
		}



		private int _año;
		public int Año
		{
			get { return _año; }
			set { SetValue(ref _año, value); }
		}


		private decimal _importedietas;
		public decimal ImporteDietas
		{
			get { return _importedietas; }
			set { SetValue(ref _importedietas, value); }
		}



		private decimal _importesabados;
		public decimal ImporteSabados
		{
			get { return _importesabados; }
			set { SetValue(ref _importesabados, value); }
		}


		private decimal _importefestivos;
		public decimal ImporteFestivos
		{
			get { return _importefestivos; }
			set { SetValue(ref _importefestivos, value); }
		}


		private decimal _plusnocturnidad;
		public decimal PlusNocturnidad
		{
			get { return _plusnocturnidad; }
			set { SetValue(ref _plusnocturnidad, value); }
		}


		private decimal _dietamenordescanso;
		public decimal DietaMenorDescanso
		{
			get { return _dietamenordescanso; }
			set { SetValue(ref _dietamenordescanso, value); }
		}


		private decimal _pluslimpieza;
		public decimal PlusLimpieza
		{
			get { return _pluslimpieza; }
			set { SetValue(ref _pluslimpieza, value); }
		}


		private decimal _pluspaqueteria;
		public decimal PlusPaqueteria
		{
			get { return _pluspaqueteria; }
			set { SetValue(ref _pluspaqueteria, value); }
		}


		private decimal _plusnavidad;
		public decimal PlusNavidad
		{
			get { return _plusnavidad; }
			set { SetValue(ref _plusnavidad, value); }
		}







		#endregion
		// ====================================================================================================


	}
}
