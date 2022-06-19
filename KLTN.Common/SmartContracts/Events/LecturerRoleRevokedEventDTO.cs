using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("LecturerRoleRevoked")]
    public class LecturerRoleRevokedEventDTO : IEventDTO
    {
        [Parameter("address[]", "lecturerAddrs", 1, false)]
        public List<string> LecturerAddrs { get; set; }
    }
}
