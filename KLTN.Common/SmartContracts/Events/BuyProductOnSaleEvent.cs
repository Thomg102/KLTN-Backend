using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ItemBought")]
    public class BuyProductOnSaleEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("address", "buyer", 2, false)]
        public string Buyer { get; set; }

        [Parameter("address", "seller", 3, false)]
        public string Seller { get; set; }

        [Parameter("uint256", "amount", 4, false)]
        public long Amount { get; set; }

        [Parameter("uint256", "price", 5, false)]
        public BigInteger TotalPrice { get; set; }
    }
}
