﻿using KLTN.DAL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.MissionServices.DTOs
{
    public class MissionDetailResponseDTO
    {
        public string MissionName { get; set; }
        public string MissionShortenName { get; set; }
        public string MissionDescription { get; set; }
        public string MissionStatus { get; set; }
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
