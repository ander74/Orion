#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Orion.MVVM {

    /// <summary>
    /// Representa un comando asíncrono enlazable a la propiedad Command de XAML.
    /// </summary>
    public class CommandAsync : ICommandAsync {

        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public CommandAsync(Func<Task> execute, Func<bool> canExecute = null, IErrorHandler errorHandler = null) {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute() {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync() {
            if (CanExecute()) {
                try {
                    _isExecuting = true;
                    await _execute();
                } finally {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter) {
            return CanExecute();
        }

        void ICommand.Execute(object parameter) {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }

    /// <summary>
    /// Representa un comando asíncrono genérico enlazable a la propiedad Command de XAML.
    /// </summary>
    /// <typeparam name="T">Tipo del parámetro que se le pasa al comando.</typeparam>
    public class CommandAsync<T> : ICommandAsync<T> {

        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public CommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null) {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter) {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter) {
            if (CanExecute(parameter)) {
                try {
                    _isExecuting = true;
                    await _execute(parameter);
                } finally {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter) {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter) {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }

}
