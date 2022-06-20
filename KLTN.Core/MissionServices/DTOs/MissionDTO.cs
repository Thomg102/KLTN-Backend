namespace KLTN.Core.MissionServices.DTOs
{
    public class MissionDTO
    {
        public int ChainNetworkId { get; set; }
        public string MissionId { get; set; }
        public string MissionImg { get; set; }
        public string MissionAddress { get; set; }
        public string MissionName { get; set; }
        public string MissionShortenName { get; set; }
        public string MissionDescription { get; set; }
        public string MissionHashIPFS { get; set; }
        public string DepartmentName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public long EndTimeToResigter { get; set; }
        public long EndTimeToComFirm { get; set; }
        public int MaxStudentAmount { get; set; }
        public string LecturerAddress { get; set; }
        public string LecturerName { get; set; }
        public decimal TokenAmount { get; set; }
    }
}
