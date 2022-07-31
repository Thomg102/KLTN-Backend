using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace KLTN.DAL.Models
{
    public class Mission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public int ChainNetworkId { get; set; }
        public string MissionId { get; set; }
        public string MissionImg { get; set; }
        public string MissionAddress { get; set; }
        public string MissionName { get; set; }
        public string MissionDescription { get; set; }
        public string MissionStatus { get; set; }
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
        public int JoinedStudentAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
        public int RewardType { get; set; }
        public string RewardName { get; set; }
        public string NFTId { get; set; }
    }

    public class MissionType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string MissionName { get; set; }
        public string MissionHash { get; set; }
    }
}
