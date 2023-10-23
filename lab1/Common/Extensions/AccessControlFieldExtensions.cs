using Common.Data;
using System.Security.AccessControl;

namespace Common.Extensions
{
    public static class AccessControlFieldExtensions
    {
        public static byte ToByte(this AccessControlField accessControl)
        {
            byte acByte = 0;

            acByte = (byte)(acByte | accessControl.ReservationBits);
            if (accessControl.TokenBit) acByte = (byte)(acByte | 0b00010000);
            if (accessControl.MonitorBit) acByte = (byte)(acByte | 0b00001000);
            acByte = (byte)(acByte | (accessControl.PriorityBits << 5));
            return acByte;
        }

        public static AccessControlField ToAccessControl(this byte acByte)
        {
            var accessControl = new AccessControlField();

            accessControl.PriorityBits = (byte)((acByte & 0b11100000) >> 5);
            accessControl.TokenBit = (acByte & 0b00010000) > 0;
            accessControl.MonitorBit = (acByte & 0b00001000) > 0;
            accessControl.ReservationBits = (byte)(acByte & 0b00000111);

            return accessControl;
        }
    }
}
