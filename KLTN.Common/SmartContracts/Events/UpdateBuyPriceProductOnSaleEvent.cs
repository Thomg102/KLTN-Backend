using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("PriceUpdated")]
    public class UpdateBuyPriceProductOnSaleEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("uint256", "_oneItemPrice", 2, false)]
        public long PriceOfOneItem { get; set; }

        [Parameter("address", "ownerOfItem", 3, false)]
        public string SaleAddress { get; set; }
    }
}
