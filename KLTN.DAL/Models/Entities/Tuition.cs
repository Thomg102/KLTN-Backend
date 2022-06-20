using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace KLTN.DAL.Models.Entities
{
    public class Tuition
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public int ChainNetworkId { get; set; }
        public string ImgURL { get; set; }
        public string TuitionId { get; set; }
        public string TuitionName { get; set; }
        public string TuitionAddress { get; set; }
        public string TuitionStatus { get; set; }
        public string TuitionDescription { get; set; }
        public string TuitionHashIPFS { get; set; }
        public int SchoolYear { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public decimal TokenAmount { get; set; }
        public decimal CurrencyAmount { get; set; }
        public string LecturerInCharge { get; set; }
        public string LecturerName { get; set; }
        public int JoinedStudentAmount { get; set; }
        public List<JoinedStudentDTO> JoinedStudentList { get; set; }
    }
}
