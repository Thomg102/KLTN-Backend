using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("Close")]
    public class CloseEventDTO : IEventDTO
    {
        [Parameter("uint256", "timestamp", 1, false)]
        public long Timestamp { get; set; }
    }
}
