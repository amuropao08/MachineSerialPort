using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases.Tables
{
    public class Rfc_pass
    {
        public string? Pwd {  get; set; }
        public DateTime Createdate { get; set; }
        public DateTime Expiredate { get; set; }
    }
}
