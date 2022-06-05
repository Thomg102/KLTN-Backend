using KLTN.DAL.Models.DTOs;
using System.Collections.Generic;

namespace KLTN.Core.MissionServices.DTOs
{
    public class MissionDetailResponseDTO
    {
        public string MissionId { get; set; }
        public string MissionAddress { get; set; }
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
