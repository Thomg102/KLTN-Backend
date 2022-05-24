using KLTN.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.DepartmentServices.DTOs
{
    public class DepartmentDTO
    {
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
        public List<SubjectType> SubjectList { get; set; }
    }
}
