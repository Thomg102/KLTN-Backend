using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("isReadyToClose", "bool")]
    public class IsReadyToClose : FunctionMessage
    {
    }
}
