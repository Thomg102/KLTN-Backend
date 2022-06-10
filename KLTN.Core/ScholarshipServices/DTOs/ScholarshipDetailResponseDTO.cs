using KLTN.DAL.Models.DTOs;
using System.Collections.Generic;

namespace KLTN.Core.ScholarshipServices.DTOs
{
    public class ScholarshipDetailResponseDTO
    {
        public string ScholarshipAddress { get; set; }
        public string ScholarshipId { get; set; }
        public string ScholarshipHashIPFS { get; set; }
        public string ScholarShipDescription { get; set; }
        public string LecturerInCharge { get; set; }
        public string ScholarshipName { get; set; }
        public string ScholarshipStatus { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public int JoinedStudentAmount { get; set; }
        public string LecturerName { get; set; }
        public long TokenAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
        public bool IsJoined { get; set; }
    }
}
