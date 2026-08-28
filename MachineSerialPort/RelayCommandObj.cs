using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MachineSerialPort
{
    public class RelayCommandObj : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Constructor for commands with a parameter
        public RelayCommandObj(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor for commands without a parameter
        public RelayCommandObj(Action execute, Func<bool>? canExecute = null)
            : this(p => execute(), p => canExecute?.Invoke() ?? true)
        {
        }

        public bool CanExecute(object? parameter)
        {
            // The check should be for the predicate itself, then invoke it.
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
