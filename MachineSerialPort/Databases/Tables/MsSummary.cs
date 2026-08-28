using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases.Tables
{
    public class MsSummary
    {
        public int MsId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime DocDate { get; set; }
        public TimeSpan MsTime { get; set; }
        public string MatNo { get; set; } = string.Empty;
        public string MatDesc { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string ComputerName { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string SAPClient { get; set; } = string.Empty;
        public List<MsMatDoc> MatDocs { get; set; } = new();
    }
}
