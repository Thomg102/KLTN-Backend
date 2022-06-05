using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ActivateNFTRequested")]
    public class RequestActivateNFTEvent : IEventDTO
    {
        [Parameter("uint256", "_activateId", 1, false)]
        public long RequestId { get; set; }

        [Parameter("uint256", "_itemId", 2, false)]
        public long ProductId { get; set; }

        [Parameter("uint256", "_amount", 3, false)]
        public long AmountToActive { get; set; }

        [Parameter("uint256", "_requestedTime", 4, false)]
        public long RequestedTime { get; set; }

        [Parameter("address", "_owner", 5, false)]
        public string StudentAddress { get; set; }
    }
}
