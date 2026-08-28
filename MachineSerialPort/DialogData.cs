using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MachineSerialPort
{
    public class DialogData
    {
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public MaterialDesignThemes.Wpf.PackIconKind Icon { get; set; }
        public Brush IconColor { get; set; } = Brushes.Transparent;
        public string YesMessage { get; set; } = string.Empty;
        public string NoMessage { get; set; } = string.Empty;
    }
}
