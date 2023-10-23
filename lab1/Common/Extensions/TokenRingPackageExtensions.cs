using Common.Data;
using System.Security.AccessControl;

namespace Common.Extensions
{
    public static class TokenRingPackageExtensions
    {
        public static byte[] ToByteArray(this TokenRingPackage package)
        {
            var bytes = new List<byte>();

            bytes.Add(package.AccessControl.ToByte());

            if (package.AccessControl.TokenBit == false)
            {
                bytes.Add(package.FrameStatus.ToByte());
                bytes.AddRange(package.Data.Serialize());
            }

            return bytes.ToArray();
        }

        public static TokenRingPackage ToTokenRingPackage(this byte[] bytes)
        {
            var package = new TokenRingPackage();

            package.AccessControl = bytes[0].ToAccessControl();
            if (package.AccessControl.TokenBit == false)
            {
                package.FrameStatus = bytes[1].ToFrameStatus();
                package.Data.Deserialize(bytes.Skip(2).ToArray());
            }

            return package;
        }
    }
}
