using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class ReturnResult
    {
        private ReturnValue returnValue = new ReturnValue();

        public class ReturnValue {
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public ReturnValue GetValue()
        {
            return returnValue;
        }

        public void SetValue(bool status, string msg)
        {
            returnValue.IsSuccess = status;
            returnValue.Message = msg;
        }
    }
}
