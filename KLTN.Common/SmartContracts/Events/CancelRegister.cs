using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("CancelRegister")]
    public class CancelRegisterEventDTO : IEventDTO
    {
        [Parameter("address", "_student", 1, false)]
        public string StudentAddr { get; set; }
    }
}
