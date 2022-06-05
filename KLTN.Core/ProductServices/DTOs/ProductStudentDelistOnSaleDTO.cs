using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductStudentDelistOnSaleDTO
    {
        public long ProductId { get; set; }
        public long AmountOnSale { get; set; }
        public string SaleAddress { get; set; }
    }
}
