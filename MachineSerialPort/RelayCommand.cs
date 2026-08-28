using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MachineSerialPort
{
    public class RelayCommand : ICommand
    {
        private readonly Action? _execute;
        private readonly Func<bool>? _canExecute;
        private readonly Func<Task>? _executeAsync;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _executeAsync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public async void Execute(object? parameter)
        {
            if (_execute != null)
            {
                _execute(); // Execute synchronous method
            }
            else if (_executeAsync != null)
            {
                try
                {
                    await _executeAsync();
                }
                catch (Exception ex)
                {
                    // IMPORTANT: Handle exceptions that occur in the async method, 
                    // as they won't bubble up otherwise (due to async void).
                    System.Windows.MessageBox.Show(
                        $"An error occurred: {ex.Message}",
                        "Async Command Error",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }
    }
}
