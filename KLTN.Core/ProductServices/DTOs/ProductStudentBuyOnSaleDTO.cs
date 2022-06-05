using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductStudentBuyOnSaleDTO
    {
        public long ProductId { get; set; }
        public long BuyAmount { get; set; }
        public string SellerAddress { get; set; }
        public string BuyerAddress { get; set; }
    }
}
