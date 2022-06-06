using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("StudentRoleRevoked")]
    public class StudentRoleRevokedEventDTO : IEventDTO
    {
        [Parameter("address[]", "studentAddrs", 1, false)]
        public List<string> StudentAddrs { get; set; }
    }
}
