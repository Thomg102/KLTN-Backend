using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ScholarshipLocked")]

    public class ScholarshipLockedEventDTO : IEventDTO
    {
        [Parameter("address[]", "_listScholarships", 1, false)]
        public List<string> ScholarshipAddrs { get; set; }
    }
}
