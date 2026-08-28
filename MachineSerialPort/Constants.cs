using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort
{
    public class Constants
    {
        
    }

    public enum DeviceState
    {
        Disconnected,
        Connected,
        Ready,
        ConfirmStartMeasuring,
        StartMeasuring,
        ConfirmCancel,
        ConfirmStopMeasuring,
        StopMeasuring,
        Saved
    }
}
