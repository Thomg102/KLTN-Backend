using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Collections.Generic;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("confirmCompletedAddress")]
    public class ConfirmCompletedAddress : FunctionMessage
    {
        [Parameter("address[]", "_students", 1)]
        public List<string> StudentList { get; set; }
    }

    [Function("confirmCompletedAddress")]
    public class ConfirmCompletedAddressSubject : FunctionMessage
    {
        [Parameter("address[]", "_students", 1)]
        public List<string> StudentList { get; set; }

        [Parameter("tuple[]", "_score", 2)]
        public List<Score> ScoretList { get; set; }
    }

    public class Score
    {
        [Parameter("uint256[]", "score", 1)]
        public virtual List<long> ScoreArray { get; set; }
    }
}
