using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("PriceUpdated")]
    public class UpdateBuyPriceProductOnSaleEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("uint256", "_oneItemPrice", 2, false)]
        public BigInteger PriceOfOneItem { get; set; }

        [Parameter("address", "ownerOfItem", 3, false)]
        public string SaleAddress { get; set; }
    }
}
