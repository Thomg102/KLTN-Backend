using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.TuitionServices.DTOs
{
    public class TuitionDTO
    {
        public string TuitionName { get; set; }
        public string TuitionAddress { get; set; }
        public string TuitionDescription { get; set; }
        public string TuitionHashIPFS { get; set; }
        public string TuitionStatus { get; set; }
        public int SchoolYear { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long TokenAmount { get; set; }
    }
}
