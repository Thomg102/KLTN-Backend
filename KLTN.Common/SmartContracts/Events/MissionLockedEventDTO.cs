using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("MissionLocked")]
    public class MissionLockedEventDTO : IEventDTO
    {
        [Parameter("address[]", "_listMissions", 1, false)]
        public List<string> MissionAddrs { get; set; }
    }
}
