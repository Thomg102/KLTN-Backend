namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductStudentBuyOnSaleDTO
    {
        public long ProductNftId { get; set; }
        public long BuyAmount { get; set; }
        public string SellerAddress { get; set; }
        public string BuyerAddress { get; set; }
    }
}
