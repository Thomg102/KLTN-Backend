using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class BuyerOfProductOnSaleResponseDTO
    {
        public long AmountOnSale { get; set; }
        public string PriceOfOneItem { get; set; }
        public string OwnerAddress { get; set; }
        public string Status { get; set; }
    }
}
