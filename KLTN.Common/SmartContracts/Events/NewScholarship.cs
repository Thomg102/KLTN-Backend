using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("NewScholarship")]
    public class NewScholarshipEventDTO : IEventDTO
    {
        [Parameter("string", "_urlMetadata", 1, true)]
        public string UrlMetadata { get; set; }
    }
}
