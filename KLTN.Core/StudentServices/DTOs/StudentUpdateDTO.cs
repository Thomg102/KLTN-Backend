using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.StudentServices.DTOs
{
    public class StudentUpdateDTO
    {
        public string StudentName { get; set; }
        public string StudentImg { get; set; }
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
