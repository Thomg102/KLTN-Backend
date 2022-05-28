using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.TuitionServices.DTOs
{
    public class TuitionDTO
    {
        public int ChainNetwork { get; set; }
        public string Img { get; set; }
        public string TuitionId { get; set; }
        public string TuitionName { get; set; }
        public string TuitionAddress { get; set; }
        public string TuitionDescription { get; set; }
        public string TuitionHashIPFS { get; set; }
        public string TuitionStatus { get; set; }
        public int SchoolYear { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long TokenAmount { get; set; }
        public long CurrencyAmount { get; set; }
        public string LecturerInCharge { get; set; }
        public string LecturerName { get; set; }
    }
}
