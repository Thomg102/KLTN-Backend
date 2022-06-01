using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("close")]
    public class Close : FunctionMessage
    {
        [Parameter("address", "pool", 1)]
        public string Pool { get; set; }
    }
}
