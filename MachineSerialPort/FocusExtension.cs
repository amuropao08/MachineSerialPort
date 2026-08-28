using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MachineSerialPort.Extensions
{
    public static class FocusExtension
    {
        public static readonly DependencyProperty FocusedCommandProperty =
            DependencyProperty.RegisterAttached(
                "FocusedCommand",
                typeof(ICommand),
                typeof(FocusExtension),
                new PropertyMetadata(null, OnFocusedCommandChanged));

        public static ICommand GetFocusedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(FocusedCommandProperty);
        }

        public static void SetFocusedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(FocusedCommandProperty, value);
        }

        private static void OnFocusedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                if (e.NewValue is ICommand command)
                {
                    // The command is what triggers the focus
                    control.GotFocus += (s, args) =>
                    {
                        if (command.CanExecute(null))
                        {
                            command.Execute(null);
                        }
                    };
                }
            }
        }
    }
}
