using KLTN.Common.Enums;
using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class ActivateRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string RequestId { get; set; }
        public string ProductName { get; set; }
        public string StudentAddress { get; set; }
        public string ProductId { get; set; }
        public string ProductHahIPFS { get; set; }
        public string AmountToActive { get; set; }
        public string ProductTypeName { get; set; }
        public bool IsActived { get; set; }
    }
}
