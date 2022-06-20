using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("UnConfirm")]
    public class UnConfirmEventDTO : IEventDTO
    {
        [Parameter("uint256", "studentsAmount", 1, false)]
        public int StudentAddr { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }
    }
}
