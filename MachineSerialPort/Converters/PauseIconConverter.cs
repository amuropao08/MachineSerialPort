using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MachineSerialPort.Converters
{
    public class PauseIconConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) => (bool)v ? PackIconKind.Play : PackIconKind.Pause;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
