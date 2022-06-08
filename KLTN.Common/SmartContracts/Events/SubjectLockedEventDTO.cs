using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("SubjectLocked")]
    public class SubjectLockedEventDTO : IEventDTO
    {
        [Parameter("address[]", "_listSubjects", 1, false)]
        public List<string> SubjectAddrs { get; set; }
    }
}
