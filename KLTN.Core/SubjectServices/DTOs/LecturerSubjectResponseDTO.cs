using KLTN.DAL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.SubjectServices.DTOs
{
    public class LecturerSubjectResponseDTO
    {
        public int ChainNetworkId { get; set; }
        public string SubjectId { get; set; }
        public string SubjectAddress { get; set; }
        public string SubjectName { get; set; }
        public string SubjectShortenName { get; set; }
        public string SubjectImg { get; set; }
        public string SubjectDescription { get; set; }
        public string SubjectStatus { get; set; }
        public string SubjectHashIPFS { get; set; }
        public string DepartmentName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public int MaxStudentAmount { get; set; }
        public string LecturerAddress { get; set; }
        public string LecturerName { get; set; }
        public int JoinedStudentAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
    }
}
