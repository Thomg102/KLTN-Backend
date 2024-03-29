﻿using KLTN.DAL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.SubjectServices.DTOs
{
    public class SubjectDetailResponseDTO
    {
        public string SubjectName { get; set; }
        public string SubjectShortenName { get; set; }
        public string SubjectDescription { get; set; }
        public string SubjectStatus { get; set; }
        public string DepartmentName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public int MaxStudentAmount { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string LecturerName { get; set; }
        public long TokenAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
        public bool IsJoined { get; set; }
    }
}
