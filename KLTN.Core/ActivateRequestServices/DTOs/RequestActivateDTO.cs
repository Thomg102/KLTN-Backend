namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class RequestActivateDTO
    {
        public long RequestId { get; set; }
        public long RequestedTime { get; set; }
        public string StudentAddress { get; set; }
        public long ProductNftId { get; set; }
        public long AmountToActivate { get; set; }
    }
}
