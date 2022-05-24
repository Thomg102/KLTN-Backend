using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.LecturerServicess.DTOs
{
    public class LecturerDetailInfoResponseDTO
    {
        public string LecturerName { get; set; }
        public string LecturerId { get; set; }
        public string LecturerAddress { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
    }
}
