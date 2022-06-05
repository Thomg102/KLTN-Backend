using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductStudentListOnSaleDTO
    {
        public long ProductId { get; set; }
        public long AmountOnSale { get; set; }
        public long PriceOfOneItem { get; set; }
        public string SaleAddress { get; set; }
    }
}
