using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.StudentServices.DTOs
{
    public class StudentDTO
    {
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string StudentAddress { get; set; }
        public string MajorName { get; set; }
        public string ClassroomName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
        public int SchoolYear { get; set; }
        public string Sex { get; set; }
        public long DateOfBirth { get; set; }
        public string BirthPlace { get; set; }
        public string Ethnic { get; set; }
        public string NationalId { get; set; }
        public int DateOfNationalId { get; set; }
        public string PlaceOfNationalId { get; set; }
        public string PermanentAddress { get; set; }
        public string StudentHashIPFS { get; set; }
    }
}
