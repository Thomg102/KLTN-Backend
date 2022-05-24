using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("Register")]
    public class RegisterEventDTO : IEventDTO
    {
        [Parameter("address", "_student", 1, false)]
        public string StudentAddr { get; set; }
    }
}
