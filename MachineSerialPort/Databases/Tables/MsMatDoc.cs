using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases.Tables
{
    public class MsMatDoc
    {
        public int MsMdId { get; set; }
        public int MsId { get; set; }
        public string MatDocNo { get; set; } = string.Empty;
        public DateTime DocDate { get; set; }
        public string PersonnelNo { get; set; } = string.Empty;
        public string WageGroup { get; set; } = string.Empty;
        public string PUom { get; set; } = string.Empty;
        public string StoreLoc { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string NgReason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsPacking { get; set; } = true;
        public string ConfirmNo {  get; set; } = string.Empty;
        public string ConfirmCt { get; set; } = string.Empty;
        public List<MsData> Details { get; set; } = new();
        public virtual MsSummary? Summary { get; set; }
    }
}
