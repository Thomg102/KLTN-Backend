﻿using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ScholarshipServices.DTOs;
using KLTN.Core.ScholarshipServices.Interfaces;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.ScholarshipServices.Implementations
{
    public class ScholarshipService : IScholarshipService
    {
        private readonly ILogger<ScholarshipService> _logger;
        private readonly IMongoCollection<Scholarship> _scholarship;
        private readonly IMongoCollection<Student> _student;

        private readonly WebAPIAppSettings _settings;

        public ScholarshipService(ILogger<ScholarshipService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _scholarship = database.GetCollection<Scholarship>(_settings.ScholarshipCollectionName);
            _student = database.GetCollection<Student>(_settings.StudentCollectionName);
        }

        // Get detail of specific scholarship Student/Lecturer/Admin
        public ScholarshipDetailResponseDTO GetDetailOfScholarship(string scholarshipAddress, string studentAddress)
        {
            try
            {
                var scholarship = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower()).FirstOrDefault();
                var result = new ScholarshipDetailResponseDTO()
                {
                    ScholarshipName = scholarship.ScholarshipName,
                    ScholarshipStatus = scholarship.ScholarshipStatus,
                    DepartmentName = scholarship.DepartmentName,
                    StartTime = scholarship.StartTime,
                    EndTime = scholarship.EndTime,
                    EndTimeToResigter = scholarship.EndTimeToResigter,
                    EndTimeToComFirm = scholarship.EndTimeToComFirm,
                    JoinedStudentAmount = scholarship.JoinedStudentAmount,
                    LecturerName = scholarship.LecturerName,
                    TokenAmount = scholarship.TokenAmount,
                    JoinedStudentList = scholarship.JoinedStudentList
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in scholarship.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                        {
                            result.IsJoined = true;
                            break;
                        }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfScholarship");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all scholarship of Student/Admin
        public List<StudentScholarshipResponseDTO> GetAllScholarship(string studentAddress)
        {
            try
            {
                var result = new List<StudentScholarshipResponseDTO>();
                var studentDepartment = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().DepartmentName;
                var joinedScholarships = _scholarship.Find<Scholarship>(_ => true).ToList();
                if (studentAddress != null)
                    joinedScholarships = _scholarship.Find<Scholarship>(x => x.DepartmentName.ToLower() == studentDepartment.ToLower()).ToList();
                foreach (var joinedScholarship in joinedScholarships)
                {
                    var isExistedJoinedStudent = false;
                    if (studentAddress != null)
                        foreach (var joinedStudentList in joinedScholarship.JoinedStudentList)
                        {
                            if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentScholarshipResponseDTO()
                                {
                                    ScholarshipName = joinedScholarship.ScholarshipName,
                                    ScholarshipAddress = joinedScholarship.ScholarshipAddress,
                                    JoinedStudentAmount = joinedScholarship.JoinedStudentAmount,
                                    ScholarshipStatus = joinedScholarship.ScholarshipStatus,
                                    IsJoined = true,
                                    StartTime = joinedScholarship.StartTime
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentScholarshipResponseDTO()
                        {
                            ScholarshipName = joinedScholarship.ScholarshipName,
                            ScholarshipAddress = joinedScholarship.ScholarshipAddress,
                            JoinedStudentAmount = joinedScholarship.JoinedStudentAmount,
                            ScholarshipStatus = joinedScholarship.ScholarshipStatus,
                            IsJoined = false,
                            StartTime = joinedScholarship.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllScholarship");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all scholarship of Lecturer
        public List<LecturerScholarshipResponseDTO> GetAllScholarshipOfLecturer(string lecturerAddress)
        {
            try
            {
                var result = new List<LecturerScholarshipResponseDTO>();
                var joinedScholarship = _scholarship.Find<Scholarship>(_ => true).ToList();
                foreach (var joinedSubject in joinedScholarship)
                {
                    if (joinedSubject.LecturerAddress.ToLower() == lecturerAddress.ToLower())
                        result.Add(new LecturerScholarshipResponseDTO()
                        {
                            ScholarshipName = joinedSubject.ScholarshipName,
                            ScholarshipAddress = joinedSubject.ScholarshipAddress,
                            JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                            ScholarshipStatus = joinedSubject.ScholarshipStatus,
                            StartTime = joinedSubject.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllScholarshipOfLecturer");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
