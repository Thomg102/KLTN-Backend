using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ActivateNFTRequestCanceled")]
    public class CancelRequestActivateNFTEvent : IEventDTO
    {
        [Parameter("uint256[]", "_activateIds", 1, false)]
        public List<long> RequestIds { get; set; }

        [Parameter("uint256", "_cancelRequestTime", 2, false)]
        public long AmountOnSale { get; set; }
    }
}
