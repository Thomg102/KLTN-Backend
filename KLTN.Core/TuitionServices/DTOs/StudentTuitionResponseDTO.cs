using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.TuitionServices.DTOs
{
    public class StudentTuitionResponseDTO
    {
        public string TuitionId { get; set; }
        public string TuitionName { get; set; }
        public string TuitionAddress { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string TuitionStatus { get; set; }
        public bool IsJoined { get; set; }
        public long StartTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}
