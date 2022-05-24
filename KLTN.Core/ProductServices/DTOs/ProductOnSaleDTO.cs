using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductOnSaleDTO
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string ProductHahIPFS { get; set; }
        public string AmountOnSale { get; set; }
        public long PriceOfOneItem { get; set; }
        public string ProductTypeName { get; set; }
        public string ProductDescription { get; set; }
    }
}
