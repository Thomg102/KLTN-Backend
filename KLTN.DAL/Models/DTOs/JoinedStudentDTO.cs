using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.DTOs
{
    public class JoinedStudentDTO
    {
        public string StudentId { get; set; }
        public string StudentAddress { get; set; }
        public string StudentName { get; set; }
        public bool IsCompleted { get; set; }
    }
}
