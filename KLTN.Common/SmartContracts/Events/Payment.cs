using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("Payment")]
    public class PaymentEventDTO : IEventDTO
    {
        [Parameter("address", "student", 1, false)]
        public string StudentsAddr { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }

        [Parameter("uint8", "_method", 3, false)]
        public int PaymentMethod { get; set; }
    }
}
