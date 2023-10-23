using Common;
using Common.Coders;
using Common.Communicators;
using Common.Data;
using Common.Extensions;
using System.Collections;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace Node
{
    public class NodeManager: IDisposable
    {
        private PortWrapper _reader;
        private PortWrapper _writer;

        private byte _portId;

        private TokenRingCommunicator _communicator;

        private bool _isMonitor;
        public NodeManager(string writerPortName, string readerPortName, int speed, byte portId, bool isMonitor) 
        {
            _writer = new PortWrapper(writerPortName,speed);
            _reader = new PortWrapper(readerPortName,speed);

            _portId = portId;
            _isMonitor = isMonitor;
            _communicator = new TokenRingCommunicator();

        }

        public void Process() 
        {
            _communicator.Open(_reader.serialPort, _writer.serialPort, _portId, _isMonitor);

            Console.WriteLine("Node started with id " + (int)_portId);

            while (true) 
            {
                Console.WriteLine("To send message enter: send");
                var command = Console.ReadLine();

                if(command.Equals("send"))
                {
                    Console.WriteLine("Enter destination id: ");
                    var dest = Console.ReadLine();
                    var destNumber = int.Parse(dest.Trim());

                    Write(destNumber);
                }
            }
        }

        public void Write(int destinationId)
        {

            Console.WriteLine("Enter message: ");

            var data = Console.ReadLine();

            var bytes = Encoding.ASCII.GetBytes(data);

            var valueBytes = bytes.Append((byte)0).ToArray();

            var hamingValue = HammingCoder.Encode(valueBytes);

            var package = DataPackageOperations.Configure(hamingValue, _portId, destinationId);

            _communicator.Send(package);
         
        }

        public void Dispose()
        {
            _reader.Dispose();
            _writer.Dispose();
        }
    }
}
