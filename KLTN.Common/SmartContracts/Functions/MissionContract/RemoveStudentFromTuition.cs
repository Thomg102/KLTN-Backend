using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("removeStudentFromTuition")]
    public class RemoveStudentFromTuition : FunctionMessage
    {
        [Parameter("address[]", "_students", 1)]
        public List<string> StudentList { get; set; }
    }
}
