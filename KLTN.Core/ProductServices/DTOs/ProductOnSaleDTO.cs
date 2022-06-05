namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductOnSaleDTO
    {
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string ProductId { get; set; }
        public long ProductNftId { get; set; }
        public string ProductHahIPFS { get; set; }
        public long AmountOnSale { get; set; }
        public long PriceOfOneItem { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductDescription { get; set; }
        public string SaleAddress { get; set; }
    }
}
