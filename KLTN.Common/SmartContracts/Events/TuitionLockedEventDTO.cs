using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("TuitionLocked")]
    public class TuitionLockedEventDTO : IEventDTO
    {
        [Parameter("address[]", "_listTuitions", 1, false)]
        public List<string> TuitionAddrs { get; set; }
    }
}
