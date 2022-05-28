using KLTN.Common.Enums;
using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class Lecturer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string LecturerImg { get; set; }
        public string LecturerName { get; set; }
        public string LecturerId { get; set; }
        public string LecturerAddress { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string LecturerHashIPFS { get; set; }
    }
}
