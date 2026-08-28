using MachineSerialPort.Databases.Tables;
using SapOdGrdTt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class RecoverySession
    {
        public int MsId { get; set; } = 0;
        public string OrderNo { get; set; } = string.Empty;
        public string MsDate { get; set; } = string.Empty;
        public string MsTime { get; set; } = string.Empty;
        public string MatNo { get; set; } = string.Empty;
        public string MatDesc { get; set; } = string.Empty;

        public string PersonnelNo { get; set; } = string.Empty;
        public string WageGroup { get; set; } = string.Empty;
        public string PUom { get; set; } = string.Empty;
        public string StoreLoc { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string NgReason { get; set; } = string.Empty;
        public bool PackingSelected { get; set; } = true;

        public int CurrentGroupNumber { get; set; } = 1;
        public decimal TotalAreaSum { get; set; } = 0;
        public decimal TotalPackingAreaSum { get; set; } = 0;

        public List<LeatherArea> MeasureData { get; set; } = new();
        public List<LeatherArea> PackingData { get; set; } = new();
        public List<LeatherArea> PackingBatchData { get; set; } = new();
        public List<RSIS_S_RANGE> MaterialDocRanges { get; set; } = new List<RSIS_S_RANGE>();

        public string CustomSessionName { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public TimeSpan ElapsedTime { get; set; } = TimeSpan.Zero;
    }
}
