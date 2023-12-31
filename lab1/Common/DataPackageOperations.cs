﻿using Common.Coders;
using Common.Data;
using Common.Extensions;
using System.Collections;

namespace Common
{
    public static class DataPackageOperations
    {
        public static DataPackage Configure(byte[] data, int sourceAdress, int destinationAdress)
        {
            return new DataPackage
            {
                Flag = GetStartFlag(),
                DestinationAddress = destinationAdress,
                SourceAddress = sourceAdress,
                Length = data.Length,
                Data = data,
                Fcs = CalculateFcs(data)
            };
        }

        public static byte GetStartFlag()
        {
            return 'z' + 14;
        }

        public static bool GetFcs(byte fcs)
        {
            var bitString = BaseCoder.BitsToString(new BitArray(fcs));

            return fcs == 1;

        }

        public static byte CalculateFcs(byte[] message)
        {
            var bitString = BaseCoder.BitsToString(new BitArray(message));

            string fcs = string.Empty;

            var paritet = CalculateParitet(bitString);

            fcs += paritet ? '1' : '0';

            var result = BaseCoder.StringToBits(fcs).ToByteArray();

            return result[0];
        }

        public static bool CalculateParitet(string bitSting)
        {
            return bitSting.Where(s => s == '1').Count() % 2 == 1;
        }
    }
}
