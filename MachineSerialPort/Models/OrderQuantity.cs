using MachineSerialPort.Databases.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Models
{
    public class OrderQuantity
    {
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Total { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Delivered { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal Remaining { get; set; }
    }
}
