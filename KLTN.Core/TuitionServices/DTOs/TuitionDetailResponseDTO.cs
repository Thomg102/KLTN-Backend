using KLTN.DAL.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.Core.TuitionServices.DTOs
{
    public class TuitionDetailResponseDTO
    {
        public string TuitionName { get; set; }
        public string TuitionDescription { get; set; }
        public string TuitionStatus { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public int JoinedStudentAmount { get; set; }
        public long TokenAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
        public bool IsJoined { get; set; }
    }
}
