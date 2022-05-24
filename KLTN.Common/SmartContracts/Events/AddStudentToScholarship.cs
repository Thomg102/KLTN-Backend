using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("AddStudentToScholarship")]
    public class AddStudentToScholarshipEventDTO : IEventDTO
    {
        [Parameter("uint256", "studentsAmount", 1, false)]
        public long StudentsAmount { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }
    }
}
