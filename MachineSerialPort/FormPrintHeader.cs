using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class FormPrintHeader
    {
        public string OrderNo { get; set; }
        public string MatDesc { get; set; }
        public string MatNo { get; set; }
        public DateTime PrintDate { get; set; }
    }
}
