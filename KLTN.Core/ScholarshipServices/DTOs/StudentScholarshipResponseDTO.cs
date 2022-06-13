﻿using KLTN.DAL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.ScholarshipServices.DTOs
{
    public class StudentScholarshipResponseDTO
    {
        public int ChainNetworkId { get; set; }
        public string ScholarshipImg { get; set; }
        public string ScholarshipId { get; set; }
        public string ScholarshipAddress { get; set; }
        public string ScholarshipName { get; set; }
        public string ScholarshipStatus { get; set; }
        public string ScholarshipHashIPFS { get; set; }
        public string ScholarShipDescription { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public string LecturerInCharge { get; set; }
        public string LecturerName { get; set; }
        public long TokenAmount { get; set; }
        public int JoinedStudentAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
        public bool IsJoined { get; set; }
    }
}
