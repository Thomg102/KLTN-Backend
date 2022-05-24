using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("AddLecturerInfo")]
    public class AddLecturerInfoEventDTO : IEventDTO
    {
        [Parameter("address", "lecturerAddr", 1, false)]
        public string LecturerAddr { get; set; }

        [Parameter("string", "hashInfo", 2, false)]
        public string HashInfo { get; set; }
    }
}
