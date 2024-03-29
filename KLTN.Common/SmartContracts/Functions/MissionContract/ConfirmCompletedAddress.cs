﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Common.SmartContracts.Functions.MissionContract
{
    [Function("confirmCompletedAddress")]
    public class ConfirmCompletedAddress : FunctionMessage
    {
        [Parameter("address[]", "_students", 1)]
        public List<string> StudentList { get; set; }
    }
}
