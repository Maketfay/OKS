using Common.Data;

namespace Common.Extensions
{
    public static class FrameStatusExtensions
    {
        public static byte ToByte(this FrameStatusField frameStatus)
        {
            byte acByte = 0;

            if (frameStatus.AddressRecognized) acByte = (byte)(acByte | 0b10001000);
            if (frameStatus.FrameCopied) acByte = (byte)(acByte | 0b01000100);

            return acByte;
        }

        public static FrameStatusField ToFrameStatus(this byte fsByte)
        {
            var frameControl = new FrameStatusField();

            frameControl.AddressRecognized = (fsByte & 0b10001000) > 0;
            frameControl.FrameCopied = (fsByte & 0b01001000) > 0;

            return frameControl;
        }
    }
}
