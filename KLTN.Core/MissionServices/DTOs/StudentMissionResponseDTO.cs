using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.MissionServices.DTOs
{
    public class StudentMissionResponseDTO
    {
        public string MissionName { get; set; }
        public string MissionAddress { get; set;}
        public string MissionShortenName { get; set; }
        public int MaxStudentAmount { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string MissionStatus { get; set; }
        public bool IsJoined { get; set; }
        public long StartTime { get; set; }
    }
}
