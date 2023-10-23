using Common.Data;
using System.IO.Ports;

namespace Common.Extensions
{
    public static class SerialPortExtensions
    {
        public static void Write(this SerialPort serialPort, TokenRingPackage package) 
        {
            var bytes = package.ToByteArray();
            serialPort.Write(bytes, 0, bytes.Length);
        }

        public static TokenRingPackage Read(this SerialPort serialPort)
        {
            var buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            var package = buffer.ToTokenRingPackage();
            return package;
        }
    }
}
