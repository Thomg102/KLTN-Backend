using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("NFTActivated")]
    public class ActivateRequestNFTEvent : IEventDTO
    {
        [Parameter("uint256", "_activateId", 1, false)]
        public long RequestId { get; set; }

        [Parameter("uint256", "_itemId", 2, false)]
        public long ProductId { get; set; }

        [Parameter("uint256", "_amount", 3, false)]
        public long AmountToActive { get; set; }

        [Parameter("uint256", "_activatedTime", 4, false)]
        public long ActivatedTime { get; set; }

        [Parameter("address", "_owner", 5, false)]
        public string StudentAddress { get; set; }

        [Parameter("bool", "_isCourseNFT", 6, false)]
        public bool IsIdependentNFT { get; set; }
    }
}
