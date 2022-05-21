using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.StudentServices.DTOs
{
    public class StudentResponseDTO
    {
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string StudentAddress { get; set; }
        public string ClassroomName { get; set; }
        public string DepartmentName { get; set; }
    }
}
