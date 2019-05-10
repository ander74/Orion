#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
namespace Orion.Config
{

	/// <summary>
	/// Clase que encapsula un Item T y el nombre de la propiedad que ha cambiado en T para devolverlo en el
	/// evento ItemPropertyChanged de la clase NotifyCollection
	/// </summary>
	/// <typeparam name="T">Tipo del item cambiado.</typeparam>
	public class ItemChangedEventArgs<T>
	{

		public T ChangedItem { get; }
		public string PropertyName { get; }

		public ItemChangedEventArgs(T item, string propertyName)
		{
			ChangedItem = item;
			PropertyName = propertyName;
		}
	}

}
