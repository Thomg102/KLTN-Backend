using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ScholarshipServices.DTOs
{
    public class ScholarshipDTO
    {
        public string ScholarshipAddress { get; set; }
        public string ScholarshipName { get; set; }
        public string ScholarshipHashIPFS { get; set; }
        public string DepartmentName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public string LecturerAddress { get; set; }
        public string LecturerName { get; set; }
        public long TokenAmount { get; set; }
    }
}
