using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases.Tables
{
    public class SavedSession
    {
        public string SessionName { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int CurrentGroup { get; set; }
        public string SelectedOrder { get; set; } = string.Empty; // Storing the SAP Order number
        public List<LeatherArea> LeatherData { get; set; } = new();
    }
}
