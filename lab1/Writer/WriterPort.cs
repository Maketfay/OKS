using Common;
using Common.Coders;
using System.Text;

namespace Writer
{
    public class WriterPort : PortWrapper
    {
        private int portId;

        private CsmaCommunicator _communicator;
        public WriterPort(string portName,int  speed) : base(portName, speed)
        {
            portId = int.Parse(portName.Replace("COM", "").Trim());

            _communicator = new CsmaCommunicator();
        }

        public void WriteProccess(int destinationPort)
        {
            base.PortInfo();

            BitstuffCoder _bitstuffCoder = new BitstuffCoder();

            while (true)
            {
                var data = Console.ReadLine();

                var bytes = Encoding.ASCII.GetBytes(data);

                var valueBytes = bytes.Append((byte)0).ToArray();

                var stuffedValue = _bitstuffCoder.Encode(BaseCoder.Decode(valueBytes));

                var hamingValue = HammingCoder.Encode(stuffedValue);

                var package = DataPackageOperations.Configure(hamingValue, portId, destinationPort);

                var dataToSend = package.Serialize();

                _communicator.Send(_serialPort, dataToSend);
            }
        }
    }
}
