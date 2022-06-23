using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("isReadyToClose", "bool")]
    public class IsReadyToClose : FunctionMessage
    {
    }

    [Function("status", "uint8")]
    public class ContractStatus : FunctionMessage
    {
    }
}
