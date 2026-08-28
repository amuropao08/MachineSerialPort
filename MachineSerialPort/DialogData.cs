using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class DialogData
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public MaterialDesignThemes.Wpf.PackIconKind Icon { get; set; }
        public System.Windows.Media.Brush IconColor { get; set; }
        public string YesMessage { get; set; }
        public string NoMessage { get; set; }
    }
}
