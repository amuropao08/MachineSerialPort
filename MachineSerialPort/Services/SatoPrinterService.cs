using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MachineSerialPort.Services
{
    public class SatoPrinterService
    {
        private readonly string _printerIp;
        private readonly int _port;

        public SatoPrinterService(string printerIp = "192.168.1.200", int port = 9100)
        {
            _printerIp = printerIp;
            _port = port;
        }

        public async Task PrintSampleQrAsync(string qrData, string labelHeader = "SAMPLE QR CODE")
        {
            // STX = \x02, ETX = \x03, ESC = \x1B
            char STX = (char)0x02;
            char ETX = (char)0x03;
            char ESC = (char)0x1B;

            // Construct SBPL command block
            StringBuilder sb = new StringBuilder();
            sb.Append(STX);
            sb.Append(ESC + "A"); // Start Label

            // Header text
            sb.Append(ESC + "V050" + ESC + "H100" + ESC + "L0202" + ESC + "P02" + labelHeader);

            // QR Code Command: <ESC>2D30, <ESC>DN (Length, Data)
            // 2D30 = Model 2 QR Code
            string qrPayload = ESC + "2D30,H,05,0,0" + ESC + $"DN{qrData.Length:D4}{qrData}";
            sb.Append(ESC + "V100" + ESC + "H120" + qrPayload);

            // Sub-text under QR
            sb.Append(ESC + "V250" + ESC + "H100" + ESC + "L0101" + ESC + "P02" + qrData);

            // Print Quantity
            sb.Append(ESC + "Q1"); // Quantity = 1
            sb.Append(ESC + "Z");  // End Label
            sb.Append(ETX);

            byte[] sendBuffer = Encoding.ASCII.GetBytes(sb.ToString());

            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_printerIp, _port);
                using (var stream = client.GetStream())
                {
                    await stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
                    await stream.FlushAsync();
                }
            }
        }
    }
}