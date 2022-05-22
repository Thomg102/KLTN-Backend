﻿using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.StudentServices.DTOs;
using KLTN.Core.StudentServices.Interfaces;
using KLTN.DAL.Models;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.StudentServices.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ILogger<StudentService> _logger;
        private readonly IMongoCollection<Mission> _mission;
        private readonly IMongoCollection<Scholarship> _scholarship;
        private readonly IMongoCollection<Tuition> _tuition;
        private readonly IMongoCollection<Student> _student;
        private readonly IMongoCollection<Subject> _subject;

        private readonly WebAPIAppSettings _settings;

        public StudentService(ILogger<StudentService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _student = database.GetCollection<Student>(_settings.StudentCollectionName);
            _mission = database.GetCollection<Mission>(_settings.MissionCollectionName);
            _scholarship = database.GetCollection<Scholarship>(_settings.ScholarshipCollectionName);
            _tuition = database.GetCollection<Tuition>(_settings.TuitionCollectionName);
            _subject = database.GetCollection<Subject>(_settings.SubjectCollectionName);
        }

        // Get detail info of specific Student
        public StudentDetailInfoResponseDTO GetDetailOfStudent(string studentAddress)
        {
            try
            {
                var studentInfo = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                return JsonConvert.DeserializeObject<StudentDetailInfoResponseDTO>(JsonConvert.SerializeObject(studentInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfStudent");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get balance histories of specific Student
        public List<BalanceHistoriesResponseDTO> GetBalanceHistoriesOfStudent(string studentAddress)
        {
            try
            {
                if (studentAddress == null)
                    throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
                var result = new List<BalanceHistoriesResponseDTO>();
                var missionList = _mission.Find<Mission>(_ => true).ToList();
                var tuitionList = _tuition.Find<Tuition>(_ => true).ToList();
                var scholarshipList = _scholarship.Find<Scholarship>(_ => true).ToList();
                foreach (var mission in missionList)
                {
                    foreach (var joinedStudentList in mission.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                HistoryName = mission.MissionName,
                                Amount = mission.TokenAmount,
                                SubmitTime = mission.EndTimeToComFirm
                            });
                            break;
                        }
                }
                foreach (var tuition in tuitionList)
                {
                    foreach (var joinedStudentList in tuition.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                HistoryName = tuition.TuitionName,
                                Amount = tuition.TokenAmount * (-1),
                                SubmitTime = tuition.EndTime
                            });
                            break;
                        }
                }
                foreach (var scholarship in scholarshipList)
                {
                    foreach (var joinedStudentList in scholarship.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                HistoryName = scholarship.ScholarshipName,
                                Amount = scholarship.TokenAmount,
                                SubmitTime = scholarship.EndTimeToComFirm
                            });
                            break;
                        }
                }

                return result.OrderByDescending(x => x.SubmitTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBalanceHistoriesOfStudent");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get list subject certificate of student
        public List<StudentCertificateDTO> GetListSubjectCertificate(string studentAddress)
        {
            try
            {
                if (studentAddress == null)
                    throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);

                var result = new List<StudentCertificateDTO>();
                var subjectList = _subject.Find<Subject>(_ => true).ToList();
                foreach (var subject in subjectList)
                {
                    foreach (var joinedStudentList in subject.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new StudentCertificateDTO()
                            {
                                CertificateName = subject.SubjectName,
                                StartTime = subject.StartTime,
                                EndTime = subject.EndTime
                            });
                            break;
                        }
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListSubjectCertificate");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get list mission certificate of student
        public List<StudentCertificateDTO> GetListMissionCertificate(string studentAddress)
        {
            try
            {
                if (studentAddress == null)
                    throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);

                var result = new List<StudentCertificateDTO>();
                var missionList = _mission.Find<Mission>(_ => true).ToList();
                foreach (var mission in missionList)
                {
                    foreach (var joinedStudentList in mission.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new StudentCertificateDTO()
                            {
                                CertificateName = mission.MissionName,
                                StartTime = mission.StartTime,
                                EndTime = mission.EndTime
                            });
                            break;
                        }
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListMissionCertificate");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get all Students
        public List<StudentResponseDTO> GetAllStudent()
        {
            try
            {
                var result = new List<StudentResponseDTO>();
                var studentInfo = _student.Find<Student>(_ => true).ToList();
                foreach (var info in studentInfo)
                {
                    var student = new StudentResponseDTO
                    {
                        StudentName = info.StudentName,
                        StudentId = info.StudentId,
                        StudentAddress = info.StudentAddress,
                        ClassroomName = info.ClassroomName,
                        DepartmentName = info.DepartmentName
                    };
                    result.Add(student);
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllStudent");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}