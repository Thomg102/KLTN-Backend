namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductUpdateBuyPriceOnSaleDTO
    {
        public long ProductNftId { get; set; }
        public long PriceOfOneItem { get; set; }
        public string SaleAddress { get; set; }
    }
}
