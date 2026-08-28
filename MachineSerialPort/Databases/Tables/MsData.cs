using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Databases.Tables
{
    public class MsData
    {
        public int MsDId { get; set; }
        public int MsMdId { get; set; }
        public int Rank { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Area { get; set; }
        public int PackingGroup { get; set; }
        public int PackingRank { get; set; }
        public virtual MsMatDoc? MatDoc { get; set; }
    }
}
