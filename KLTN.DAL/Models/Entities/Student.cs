using KLTN.Common.Enums;
using KLTN.DAL.Models.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KLTN.DAL.Models.Entities
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string StudentImg { get; set; }
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string StudentAddress { get; set; }
        public string MajorName { get; set; }
        public string ClassroomName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentShortenName { get; set; }
        public int SchoolYear { get; set; }
        public string Sex { get; set; }
        public long DateOfBirth { get; set; }
        public string BirthPlace { get; set; }
        public string Ethnic { get; set; }
        public string NationalId { get; set; }
        public long DateOfNationalId { get; set; }
        public string PlaceOfNationalId { get; set; }
        public string PermanentAddress { get; set; }
        public string StudentHashIPFS { get; set; }
        public List<ProductOfStudentDTO> ProductOfStudentList { get; set; }
    }
}
