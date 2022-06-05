using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class ActivateRequestDTO
    {
        public long RequestId { get; set; }
        public long ActivatedTime { get; set; }
        public string IsIdependentNFT { get; set; }
    }
}
