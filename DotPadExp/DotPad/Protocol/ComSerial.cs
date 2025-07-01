using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;

namespace DotPadExp.DotPad.Protocol
{
    public partial class ComSerial
    {
        public bool IsOpen = false;
        private readonly SerialPort _serialPort = new();
        private readonly string? _portName;
        private readonly int _baudRate = 115200;
        private readonly int _dataBits = 8;
        private readonly StopBits _stopBits = StopBits.One;

        [GeneratedRegex(@"\d+")]
        private static partial Regex MyRegex();

        public ComSerial()
        {
            List<string> usbComPorts = [];

            if (OperatingSystem.IsWindows())
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'");
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    string? name = obj["Name"]?.ToString();
                    if (name != null && name.Contains("usb", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // 예: "USB Serial Device (COM3)" → COM 포트 번호만 추출
                        int start = name.LastIndexOf("(COM");
                        if (start >= 0)
                        {
                            int end = name.IndexOf(')', start);
                            if (end > start)
                            {
                                string comPort = name.Substring(start + 1, end - start - 1); // "COM3"
                                usbComPorts.Add(comPort);
                            }
                        }
                    }
                }
            }

            usbComPorts.Sort((a, b) =>
            {
                int numA = int.Parse(MyRegex().Match(a).Value);
                int numB = int.Parse(MyRegex().Match(b).Value);
                return numA.CompareTo(numB);
            });

            _portName ??= usbComPorts[0];
        }

        public void SerialOpen()
        {
            try
            {
                _serialPort.PortName = _portName;
                _serialPort.BaudRate = _baudRate;
                _serialPort.DataBits = _dataBits;
                _serialPort.StopBits = _stopBits;
                _serialPort.Parity = Parity.None;
                _serialPort.Open();
                IsOpen = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening port: {ex.Message}");
            }
        }

        public void SerialSend(byte[] data)
        {
            try
            {
                if (IsOpen)
                {
                    _serialPort.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
            }
        }
 
        public void SerialClose()
        {
            if (IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}