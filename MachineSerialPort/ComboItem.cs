using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class ComboItem
    {
        public string Value { get; set; }     // The specific value (e.g., ID, Code)
        public string Text { get; set; }   // The text shown to the user (e.g., Name, Description)

        // Override ToString() to ensure the Text is displayed if data binding properties fail.
        public override string ToString()
        {
            return Text;
        }
    }
}
