namespace Common.Data
{
    public class AccessControlField
    {
        public byte PriorityBits { get; set; }
        public bool TokenBit { get; set; }
        public bool MonitorBit { get; set; }
        public byte ReservationBits { get; set; }
    }
}
