using Common.Coders;
using Common.Data;
using Common.Extensions;
using System.IO.Ports;
using System.Security.AccessControl;

namespace Common.Communicators
{
    public class TokenRingCommunicator
    {
        private DateTime _lastTokenRecieved;

        private byte Priority;

        private Queue<DataPackage> messageQueue = new Queue<DataPackage>();

        private byte _portId;       

        private SerialPort _recieverSerialPort;

        private SerialPort _senderSerialPort;
        public void Open(SerialPort reader, SerialPort writer, byte PortId, bool isMonitor)
        {
            Priority = PortId;

            _portId = PortId;

            _recieverSerialPort = reader;
            _senderSerialPort = writer;

            _recieverSerialPort.DataReceived += new SerialDataReceivedEventHandler(Read);

            if (isMonitor) 
            {
                Task.Run(() => WatchTokenRestore());
                _recieverSerialPort.DataReceived += (s, e) => UpdateTokenRecievedDate();
            }            
        }

        public void UpdateTokenRecievedDate()
        {
            _lastTokenRecieved = DateTime.Now;
        }
        public async Task WatchTokenRestore()
        {

            if (DateTime.Now > _lastTokenRecieved.AddSeconds(2))
            {
                TokenRingPackage package = new TokenRingPackage()
                {
                    AccessControl = new AccessControlField()
                    {
                        MonitorBit = true,
                        TokenBit = true,
                        PriorityBits = 0,
                        ReservationBits = 0
                    }
                };
                _senderSerialPort.Write(package);
            }
            await Task.Delay(5000);
        }
        public void Read(object sender, SerialDataReceivedEventArgs e)
        {
            var package = _recieverSerialPort.Read();

            if (package.AccessControl.TokenBit == true)
            {
                if (_portId == 3)
                {
                    Console.WriteLine("Token received");
                    Thread.Sleep(2000);
                }
                if (messageQueue.Count > 0)
                {
                    Console.WriteLine("There is message in queue and token received");
                    if (package.AccessControl.PriorityBits <= Priority)
                    {
                        package.AccessControl.PriorityBits = Priority;
                        package.AccessControl.TokenBit = false;
                        package.AccessControl.ReservationBits = 0;

                        package.FrameStatus = new FrameStatusField()
                        {
                            AddressRecognized = false,
                            FrameCopied = false
                        };
                        package.Data = new DataPackage();
                        package.Data = messageQueue.Dequeue();

                        Priority = (byte)Math.Max(Priority - 1, 1);

                        Console.WriteLine("Message next");
                    }
                    else
                    {
                        if (package.AccessControl.ReservationBits < Priority) package.AccessControl.ReservationBits = Priority;

                        Priority = (byte)Math.Min(Priority + 1, 7);

                        Console.WriteLine("Token skiped, priority added");
                    }

                }

                _senderSerialPort.Write(package);
                return;
            }

            if (package.Data.SourceAddress == _portId)
            {
                Console.WriteLine("Message returned");
                package.AccessControl.PriorityBits = package.AccessControl.ReservationBits;
                package.AccessControl.ReservationBits = 0;
                package.AccessControl.TokenBit = true;

                _senderSerialPort.Write(package);
                return;
            }

            if (package.Data.DestinationAddress == _portId)
            {
                Console.WriteLine("My message");
                package.FrameStatus.AddressRecognized = true;
                package.FrameStatus.FrameCopied = true;

                var hammingMessage = HammingCoder.Decode(package.Data.Data, package.Data.Fcs);
                Console.WriteLine(hammingMessage);

                _senderSerialPort.Write(package);
                return;
            }

            if (package.AccessControl.ReservationBits < Priority)
            {
                if (messageQueue.Count > 0)
                {
                    package.AccessControl.ReservationBits = Priority;
                    Priority = (byte)Math.Min(Priority + 1, 7);
                }
            }

            _senderSerialPort.Write(package);

            Console.WriteLine("Message hes been sent on");
        }
        public void Send(DataPackage package)
        {
            messageQueue.Enqueue(package);

            Console.WriteLine("Package in queue");
        }
        
    }

}
