namespace Orion.Config
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.ComponentModel;

	/// <summary>
	/// Esta clase añade la notificación de cambios en las propiedades de los elementos dentro de la colección.
	/// 
	/// Estudiar si al estar la colección enlazada a un Grid (o elemento similar) reconoce los cambios en los
	/// elementos dentro de la colección. Si no, habrá que llamar a 'OnCollectionChange' para que se notifiquen
	/// los cambios dentro de los elementos.
	/// 
	/// Otro comportamiento podría ser, que cuando un elemento dispare el evento PropertyChanged, se dispare
	/// el evento CollectionChanged de la propia colección, mediante la llamada al OnCollectionChanged.
	/// 
	/// </summary>
	public class NotifyCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
	{

		/// <summary>
		/// Evento que se lanzará cuando cambia una propiedad dentro de un elemento de la colección.
		/// </summary>
		public event EventHandler<ItemChangedEventArgs<T>> ItemPropertyChanged;


		// ====================================================================================================
		#region CONSTRUCTORES
		// ====================================================================================================

		public NotifyCollection() : base() { }


		public NotifyCollection(List<T> list) : base(list)
		{
			ObserveAll();
		}


		public NotifyCollection(IEnumerable<T> enumerable) : base(enumerable)
		{
			ObserveAll();
		}


		#endregion
		// ====================================================================================================


		/// <summary>
		/// Cuando se añade, elimina o cambia un elemento de la colección, se registra o no 
		/// el evento PropertyChanged del elemento
		/// </summary>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (T item in e.OldItems) item.PropertyChanged -= ChildPropertyChanged;
			}

			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
			{
				foreach (T item in e.NewItems) item.PropertyChanged += ChildPropertyChanged;
			}

			base.OnCollectionChanged(e);
		}


		/// <summary>
		/// Cuando se vacía la colección, se suprime el registro del evento PropertyChanged de todos los elementos.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (T item in Items) item.PropertyChanged -= ChildPropertyChanged;
			base.ClearItems();
		}

		/// <summary>
		/// Se registra el evento PropertyChanged a todos los elementos de la colección.
		/// </summary>
		private void ObserveAll()
		{
			foreach (T item in Items) item.PropertyChanged += ChildPropertyChanged;
		}


		/// <summary>
		/// Método que se ejecuta cuando se dispara el evento PropertyChanged de un elemento.
		/// Este método, lanza el evento ItemPropertyChanged, enviando como argumentos el elemento de la
		/// colección en el que ha cambiado alguna propiedad y el nombre de la propiedad.
		/// </summary>
		private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			T typedSender = (T)sender;
			var args = new ItemChangedEventArgs<T>(typedSender, e.PropertyName);
			ItemPropertyChanged?.Invoke(this, args);
		}
	}
}
