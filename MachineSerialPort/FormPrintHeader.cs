using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class FormPrintHeader
    {
        public string OrderNo { get; set; } = string.Empty;
        public string MatDesc { get; set; } = string.Empty;
        public string MatNo { get; set; } = string.Empty;
        public DateTime PrintDate { get; set; }
    }
}
