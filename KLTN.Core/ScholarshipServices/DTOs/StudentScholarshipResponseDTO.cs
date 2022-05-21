using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ScholarshipServices.DTOs
{
    public class StudentScholarshipResponseDTO
    {
        public string ScholarshipName { get; set; }
        public string ScholarshipAddress { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string ScholarshipStatus { get; set; }
        public bool IsJoined { get; set; }
        public long StartTime { get; set; }
    }
}
