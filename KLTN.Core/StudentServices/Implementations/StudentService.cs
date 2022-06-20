using KLTN.Common.Exceptions;
using KLTN.Core.StudentServices.DTOs;
using KLTN.Core.StudentServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models;
using KLTN.DAL.Models.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.StudentServices.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ILogger<StudentService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Mission> _mission;
        private readonly IMongoCollection<Scholarship> _scholarship;
        private readonly IMongoCollection<Tuition> _tuition;
        private readonly IMongoCollection<Student> _student;
        private readonly IMongoCollection<Subject> _subject;

        public StudentService(ILogger<StudentService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _student = _context.GetCollection<Student>(typeof(Student).Name);
            _mission = _context.GetCollection<Mission>(typeof(Mission).Name);
            _scholarship = _context.GetCollection<Scholarship>(typeof(Scholarship).Name);
            _tuition = _context.GetCollection<Tuition>(typeof(Tuition).Name);
            _subject = _context.GetCollection<Subject>(typeof(Subject).Name);
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
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                Type = "missions",
                                ContractAddress = mission.MissionAddress,
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
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                Type = "tuitions",
                                ContractAddress = tuition.TuitionAddress,
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
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new BalanceHistoriesResponseDTO()
                            {
                                Type = "scholarships",
                                ContractAddress = scholarship.ScholarshipAddress,
                                HistoryName = scholarship.ScholarshipName,
                                Amount = scholarship.TokenAmount,
                                SubmitTime = scholarship.EndTime
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
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new StudentCertificateDTO()
                            {
                                Type = "subjects",
                                ContractAddress = subject.SubjectAddress,
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
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower() && joinedStudentList.IsCompleted)
                        {
                            result.Add(new StudentCertificateDTO()
                            {
                                Type = "missions",
                                ContractAddress = mission.MissionAddress,
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
        public async Task CreateNewStudent(StudentDTO student)
        {
            try
            {
                var isExisted = false;
                var lecturerList = _student.Find<Student>(_ => true).ToList();
                foreach (var lecturerInfo in lecturerList)
                    if (lecturerInfo.StudentAddress.ToLower() == student.StudentAddress.ToLower())
                        isExisted = true;
                if (!isExisted)
                    await _student.InsertOneAsync(new Student()
                    {
                        StudentName = student.StudentName,
                        StudentImg = student.StudentImg,
                        StudentId = student.StudentId,
                        StudentAddress = student.StudentAddress,
                        MajorName = student.MajorName,
                        ClassroomName = student.ClassroomName,
                        DepartmentName = student.DepartmentName,
                        SchoolYear = student.SchoolYear,
                        Sex = student.Sex,
                        DateOfBirth = student.DateOfBirth,
                        BirthPlace = student.BirthPlace,
                        Ethnic = student.Ethnic,
                        NationalId = student.NationalId,
                        DateOfNationalId = student.DateOfNationalId,
                        PlaceOfNationalId = student.PlaceOfNationalId,
                        PermanentAddress = student.PermanentAddress,
                        StudentHashIPFS = student.StudentHashIPFS,
                        DepartmentShortenName = student.DepartmentShortenName,
                        ProductOfStudentList = new List<ProductOfStudentDTO>() { }
                    });
                else
                    throw new CustomException("Address was permission", 300);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewStudent");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateStudentIntoDatabase(string studentAddress, StudentUpdateDTO studentInfo)
        {
            try
            {
                var currentHashIPFS = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().StudentHashIPFS;
                var filter = Builders<Student>.Filter.Eq(x => x.StudentAddress.ToLower(), studentAddress.ToLower());
                var update = Builders<Student>.Update.Set(x => x.StudentName, studentInfo.StudentName)
                                                     .Set(x => x.StudentImg, studentInfo.StudentImg)
                                                     .Set(x => x.Sex, studentInfo.Sex)
                                                     .Set(x => x.DateOfBirth, studentInfo.DateOfBirth)
                                                     .Set(x => x.BirthPlace, studentInfo.BirthPlace)
                                                     .Set(x => x.Ethnic, studentInfo.Ethnic)
                                                     .Set(x => x.NationalId, studentInfo.NationalId)
                                                     .Set(x => x.DateOfNationalId, studentInfo.DateOfNationalId)
                                                     .Set(x => x.PlaceOfNationalId, studentInfo.PlaceOfNationalId)
                                                     .Set(x => x.PermanentAddress, studentInfo.PermanentAddress)
                                                     .Set(x => x.StudentHashIPFS, studentInfo.StudentHashIPFS);
                if (studentInfo.StudentHashIPFS != currentHashIPFS)
                    await _student.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentIntoDatabase");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task RevokeStudentRole(List<string> studentAddrs)
        {
            try
            {
                foreach (var studentAddr in studentAddrs)
                {
                    var filter = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == studentAddr.ToLower());
                    await _student.DeleteOneAsync(filter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RevokeStudentRole");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
