using Common.Coders;
using System.Collections;
using System.IO.Ports;

namespace Common.Communicators
{
    public class CsmaCommunicator
    {
        private const byte JAM_SIGNAL = 125;

        private const byte END_OF_MESSAGE = 126;

        private Random _random = new Random();
        public void Send(SerialPort serialPort, byte[] data)
        {
            foreach (byte item in data)
            {
                for (int numberOfAttempt = 0; ; numberOfAttempt++)
                {
                    waitFree();

                    serialPort.Write(new byte[] { item }, 0, 1);

                    if (isCollide())
                    {
                        serialPort.Write(new byte[] { JAM_SIGNAL }, 0, 1);

                        delaySending(numberOfAttempt);
                    }
                    else break;
                }

            }

            serialPort.Write(new byte[] { END_OF_MESSAGE }, 0, 1);
        }

        public byte[] Read(SerialPort serialPort)
        {
            var receiveBuffer = new List<byte>();

            while (true)
            {
                byte[] b = new byte[1];
                if (serialPort.BytesToRead == 0)
                {
                    continue;
                }
                serialPort.Read(b, 0, 1);

                Console.WriteLine("Data byte: " + BaseCoder.BitsToString(new BitArray(b)));

                if (b[0] == END_OF_MESSAGE) break;

                if (b[0] == JAM_SIGNAL)
                {
                    Console.WriteLine("Jam detected");
                    receiveBuffer.RemoveAt(receiveBuffer.Count - 1);
                    continue;
                }
                receiveBuffer.Add(b[0]);
            }

            return receiveBuffer.ToArray();
        }

        private void waitFree()
        {
            if (isChannelBusy())
            {
                Thread.Sleep(100);
            }
        }

        private bool isChannelBusy() => randomChance();
        private bool isCollide() => randomChance();
        private bool randomChance() => _random.Next(100) > 50;
        private void delaySending(int number) => Thread.Sleep(new Random().Next((int)Math.Pow(2, Math.Min(10, number))) * 10);
    }
}
