using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ActivateRequestServices.DTOs
{
    public class ActivateCodeRequestDTO
    {
        public string ProductTypeName { get; set; }
        public List<string> ListCode { get; set; }
    }
}
