using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("NewTuition")]
    public class NewTuitionEventDTO : IEventDTO
    {
        [Parameter("address", "_contractAddress", 1, false)]
        public string ContractAddress { get; set; }

        [Parameter("string", "_urlMetadata", 2, false)]
        public string UrlMetadata { get; set; }
    }
}
