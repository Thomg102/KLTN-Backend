using KLTN.DAL.Models.DTOs;
using System.Collections.Generic;

namespace KLTN.Core.ScholarshipServices.DTOs
{
    public class LecturerScholarshipResponseDTO
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
        public decimal TokenAmount { get; set; }
        public int JoinedStudentAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
    }
}
