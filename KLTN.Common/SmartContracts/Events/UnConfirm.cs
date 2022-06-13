using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("UnConfirm")]
    public class UnConfirmEventDTO : IEventDTO
    {
        [Parameter("address", "studentsAmount", 1, false)]
        public string StudentAddr { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public string Timestamp { get; set; }
    }
}
