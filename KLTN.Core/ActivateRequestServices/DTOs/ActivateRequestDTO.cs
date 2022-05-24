using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class ActivateRequestDTO
    {
        public string RequestId { get; set; }
        public string ProductName { get; set; }
        public string StudentAddress { get; set; }
        public string ProductId { get; set; }
        public string ProductHahIPFS { get; set; }
        public string AmountToActive { get; set; }
        public string ProductTypeName { get; set; }
    }
}
