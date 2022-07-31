using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ItemReceived")]
    public class ReceivedItemMissionEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("address", "student", 2, false)]
        public string StudentAddress { get; set; }

        [Parameter("uint256", "amount", 3, false)]
        public long Amount { get; set; }
    }
}
