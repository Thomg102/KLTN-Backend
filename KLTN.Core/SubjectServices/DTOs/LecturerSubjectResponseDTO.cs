using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.SubjectServices.DTOs
{
    public class LecturerSubjectResponseDTO
    {
        public string SubjectName { get; set; }
        public string SubjectAddress { get; set; }
        public string SubjectShortenName { get; set; }
        public int MaxStudentAmount { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string SubjectStatus { get; set; }
        public long StartTime { get; set; }
    }
}
