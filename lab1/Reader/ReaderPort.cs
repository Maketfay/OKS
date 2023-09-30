using Common;
using Common.Coders;
using Common.Extensions;
using System.Collections;
using System.IO.Ports;

namespace Reader
{
    internal class ReaderPort : PortWrapper
    {
        BitstuffCoder _bitstuffCoder;

        CsmaCommunicator _communicator;
        public ReaderPort(string portName, int speed) : base(portName, speed)
        {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(Output);
            _bitstuffCoder = new BitstuffCoder();
            _communicator = new CsmaCommunicator();
        }

        public void ReadProccess() 
        {
            base.PortInfo();

            while (true) ;
        }

        private void Output(object sender, SerialDataReceivedEventArgs e)
        {
            var buffer = _communicator.Read(_serialPort);

            int startIndex = -1;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == DataPackageOperations.GetStartFlag())
                {
                    startIndex = i;
                    break;
                }
            }

            if (startIndex >= 0)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer, startIndex, buffer.Length - startIndex);

                DataPackage receivedPackage = new DataPackage();
                if (receivedPackage.Deserialize(segment.ToArray()))
                {
                    var bitString = BaseCoder.BitsToString(new BitArray(receivedPackage.Data));

                    Console.WriteLine("Received data:      " + bitString);

                    bitString = HammingCoder.DistortBits(bitString);

                    Console.WriteLine("Data after distort: " + bitString);

                    receivedPackage.Data = BaseCoder.StringToBits(bitString).ToByteArray();

                    var hammingMessage = HammingCoder.Decode(receivedPackage.Data, receivedPackage.Fcs);
                    var message = _bitstuffCoder.Decode(BaseCoder.Encode(hammingMessage));
                    Console.WriteLine(message);
                    var messageBytes = BaseCoder.Encode(message);

                    Console.WriteLine("Data after hamming decode:  " + BaseCoder.BitsToString(new BitArray(messageBytes)));

                    Console.WriteLine($"{receivedPackage.Length} bytse received");

                }
            }
        }
    }
}
