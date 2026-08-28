using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MachineSerialPort.Converters
{
    public class PauseColorConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c) => (bool)v ? Brushes.Crimson : Brushes.ForestGreen;
        public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
    }
}
