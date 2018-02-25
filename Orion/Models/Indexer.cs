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

	public class Indexer<T> {

		private IList<T> _origenDatos;

		public Indexer(IList<T> origen) {
			_origenDatos = origen;
		}

		public T this[int index] {
			get { return _origenDatos[index]; }
			set { _origenDatos[index] = value; }
		}
	}


	public static class IndexerHelper {

		public static Indexer<T> GetIndexer<T>(this IList<T> coleccionIndexada) {
			return new Indexer<T>(coleccionIndexada);
		}
	}
}
