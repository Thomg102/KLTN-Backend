﻿namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductStudentListOnSaleDTO
    {
        public long ProductNftId { get; set; }
        public long AmountOnSale { get; set; }
        public decimal PriceOfOneItem { get; set; }
        public string SaleAddress { get; set; }
    }
}
