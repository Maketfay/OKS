using System.IO.Ports;

namespace Common
{
    public class PortWrapper : IDisposable
    {
        public SerialPort serialPort;

        public PortWrapper(string portName, int speed)
        {
            serialPort = new SerialPort(portName, speed, Parity.None, 8, StopBits.One);
            serialPort.Open();

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
        }

        public void PortInfo()
        {
            Console.WriteLine($"Process started on port {serialPort.PortName} with speed: {serialPort.BaudRate}");
        }
        public void Dispose()
        {
            serialPort.Dispose();
        }
    }
}
