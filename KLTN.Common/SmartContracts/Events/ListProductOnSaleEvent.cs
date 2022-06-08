﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("ItemListed")]
    public class ListProductOnSaleEvent : IEventDTO
    {
        [Parameter("uint256", "itemId", 1, true)]
        public long ProductId { get; set; }

        [Parameter("uint256", "amount", 2, false)]
        public long AmountOnSale { get; set; }

        [Parameter("uint256", "oneItemPrice", 3, false)]
        public BigInteger PriceOfOneItem { get; set; }

        [Parameter("address", "ownerOfItem", 4, false)]
        public string SaleAddress { get; set; }
    }
}