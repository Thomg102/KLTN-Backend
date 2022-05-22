using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ActiveRequestServices.DTOs
{
    public class ActivateRequestResponseDTO
    {
        public string RequestId { get; set; }
        public string ProductName { get; set; }
        public string StudentName { get; set; }
        public string ProductId { get; set; }
        public string ProductHahIPFS { get; set; }
        public string AmountToActive { get; set; }
        public string ProductTypeName { get; set; }
        public bool IsActived { get; set; }
    }
}
