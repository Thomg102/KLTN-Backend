using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Events
{
    [Event("AdminItemListed")]
    public class CreateAndListProductEvent : IEventDTO
    {
        [Parameter("string", "hash", 1, false)]
        public string HashInfo { get; set; }

        [Parameter("uint256", "itemId", 2, true)]
        public long ProductId { get; set; }

        [Parameter("uint256", "amount", 3, false)]
        public long AmountOnSale { get; set; }

        [Parameter("uint256", "oneItemPrice", 4, false)]
        public long PriceOfOneItem { get; set; }

        [Parameter("address", "ownerOfItem", 5, false)]
        public string SaleAddress { get; set; }
    }
}
