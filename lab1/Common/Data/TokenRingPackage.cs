namespace Common.Data
{
    public class TokenRingPackage
    {
        public AccessControlField AccessControl { get; set; } = new();
        public DataPackage Data { get; set; } = new();
        public FrameStatusField FrameStatus { get; set; } = new();
    }
}
