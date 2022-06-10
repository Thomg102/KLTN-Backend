namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductDetailResponseDTO
    {
        public long RequestId { get; set; }
        public string ProductName { get; set; }
        public string OwnerAddress { get; set; }
        public string ProductId { get; set; }
        public long ProductNftId { get; set; }
        public string ProductHahIPFS { get; set; }
        public long Amount { get; set; }
        public string ProductTypeName { get; set; }
        public long RequestedTime { get; set; }
        public long ActivatedTime { get; set; }
        public bool IsActivated { get; set; }
        public string ProductImg { get; set; }
        public string PriceOfOneItem { get; set; }
        public string ProductDescription { get; set; }
        public string Status { get; set; }
    }
}
