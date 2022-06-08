﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductOnSaleResponseDTO
    {
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string ProductId { get; set; }
        public long ProductNftId { get; set; }
        public string ProductDescription { get; set; }
        public string ProductHahIPFS { get; set; }
        public long TotalAmountOnSale { get; set; }
        public string ProductTypeName { get; set; }
        public string Status { get; set; }
        public string MinPrice { get; set; }
    }
}