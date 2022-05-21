using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.StudentServices.DTOs
{
    public class BalanceHistoriesResponseDTO
    {
        public string HistoryName { get; set; }
        public long Amount { get; set; }
        public long SubmitTime { get; set; }
    }
}
