using KLTN.Common.Enums;
using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class Department
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
        public List<SubjectType> SubjectList { get; set; }
    }

    public class SubjectType
    {
        public string SubjectName { get; set; }
        public string SubjectHash { get; set; }
    }
}
