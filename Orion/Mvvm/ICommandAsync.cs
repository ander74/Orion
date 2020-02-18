#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System.Threading.Tasks;
using System.Windows.Input;

namespace Orion.MVVM {

    /// <summary>
    /// Define un comando asíncrono.
    /// </summary>
    public interface ICommandAsync : ICommand {
        Task ExecuteAsync();
        bool CanExecute();
    }


    /// <summary>
    /// Define un comando asíncrono genérico.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro que se le pasará al comando.</typeparam>
    public interface ICommandAsync<T> : ICommand {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }

}
