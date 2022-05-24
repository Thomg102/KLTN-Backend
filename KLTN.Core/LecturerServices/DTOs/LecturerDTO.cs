using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.LecturerServices.DTOs
{
    public class LecturerDTO
    {
        public string LecturerName { get; set; }
        public string LecturerId { get; set; }
        public string LecturerAddress { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
    }
}
