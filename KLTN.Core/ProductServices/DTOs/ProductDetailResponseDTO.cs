﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ProductServices.DTOs
{
    public class ProductDetailResponseDTO
    {
        public string ProductName { get; set; }
        public string ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string ProductHahIPFS { get; set; }
        public string Amount { get; set; }
        public string ProductTypeName { get; set; }
    }
}
