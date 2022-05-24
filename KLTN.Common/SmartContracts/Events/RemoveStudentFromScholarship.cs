using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("RemoveStudentFromScholarship")]
    public class RemoveStudentFromScholarshipEventDTO : IEventDTO
    {
        [Parameter("address", "student", 1, false)]
        public long StudentsAmount { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }
    }
}
