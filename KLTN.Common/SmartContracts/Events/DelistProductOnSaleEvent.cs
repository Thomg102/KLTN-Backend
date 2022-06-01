using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ItemDelisted")]
    public class DelistProductOnSaleEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("uint256", "amount", 2, false)]
        public long AmountOnSale { get; set; }

        [Parameter("address", "ownerOfItem", 3, false)]
        public string SaleAddress { get; set; }
    }
}
