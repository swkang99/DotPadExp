using System.IO.Ports;

namespace DotPadExp.DotPad.Protocol
{
    public class ComSerial
    {
        public bool IsOpen = false;
        private readonly SerialPort _serialPort = new();
        private readonly string _portName = "COM3";
        private readonly int _baudRate = 115200;
        private readonly int _dataBits = 8;
        private readonly StopBits _stopBits = StopBits.One;

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