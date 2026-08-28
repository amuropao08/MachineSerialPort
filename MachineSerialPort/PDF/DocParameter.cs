using SapOdGrdTt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.PDF
{
    public class DocParameter
    {
        public ObservableCollection<LeatherArea> MeasureData { get; set; } = new();
        public MSEG[] TotalGradeQty { get; set; } = Array.Empty<MSEG>();
        public string DocDate { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string NGReason { get; set; } = string.Empty;
        public string MatDesc { get; set; } = string.Empty;
        public string MatNo { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string TotalQty { get; set; } = string.Empty;
        public string PQty { get; set; } = string.Empty;
        public string Qty { get; set; } = string.Empty;
        public int PackingGroup;
    }
}
