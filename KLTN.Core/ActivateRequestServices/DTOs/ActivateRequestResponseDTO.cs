namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class ActivateRequestResponseDTO
    {
        public long RequestId { get; set; }
        public string ProductName { get; set; }
        public long RequestedTime { get; set; }
        public long ActivatedTime { get; set; }
        public string StudentAddress { get; set; }
        public long ProductNftId { get; set; }
        public string ProductId { get; set; }
        public string ProductHahIPFS { get; set; }
        public long AmountToActivate { get; set; }
        public string ProductTypeName { get; set; }
        public bool IsActivated { get; set; }
        public string ProductImg { get; set; }
        public string ProductDescription { get; set; }
    }
}
