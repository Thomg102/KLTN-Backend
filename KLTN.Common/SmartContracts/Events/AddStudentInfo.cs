using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("AddStudentInfo")]
    public class AddStudentInfoEventDTO : IEventDTO
    {
        [Parameter("address", "studentAddr", 1, false)]
        public string StudentAddr { get; set; }

        [Parameter("string", "hashInfo", 1, false)]
        public string HashInfo { get; set; }
    }
}
