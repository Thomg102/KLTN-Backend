using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("AddStudentToTuition")]
    public class AddStudentToTuitionEventDTO : IEventDTO
    {
        [Parameter("uint256", "studentsAmount", 1, false)]
        public long StudentsAmount { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }
    }
}
