﻿using Nethereum.ABI.FunctionEncoding.Attributes;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("RemoveStudentFromTuition")]
    public class RemoveStudentFromTuitionEventDTO : IEventDTO
    {
        [Parameter("address", "student", 1, false)]
        public string StudentsAddr { get; set; }

        [Parameter("uint256", "timestamp", 2, false)]
        public long Timestamp { get; set; }
    }
}
