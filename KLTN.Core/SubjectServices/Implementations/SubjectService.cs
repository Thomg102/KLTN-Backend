using KLTN.Common.Enums;
using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.SubjectServices.DTOs;
using KLTN.Core.SubjectServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.SubjectServices.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ILogger<SubjectService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Subject> _subject;

        public SubjectService(ILogger<SubjectService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _subject = _context.GetCollection<Subject>(typeof(Subject).Name);
        }

        // Get detail of specific subject Student/Lecturer/Admin
        public SubjectDetailResponseDTO GetDetailOfSubject(string subjectAddress, string studentAddress)
        {
            try
            {
                var subject = _subject.Find<Subject>(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower()).FirstOrDefault();
                var result = new SubjectDetailResponseDTO()
                {
                    SubjectName = subject.SubjectName,
                    SubjectShortenName = subject.SubjectShortenName,
                    SubjectDescription = subject.SubjectDescription,
                    SubjectStatus = subject.SubjectStatus,
                    DepartmentName = subject.DepartmentName,
                    StartTime = subject.StartTime,
                    EndTime = subject.EndTime,
                    EndTimeToResigter = subject.EndTimeToResigter,
                    EndTimeToComFirm = subject.EndTimeToComFirm,
                    MaxStudentAmount = subject.MaxStudentAmount,
                    JoinedStudentAmount = subject.JoinedStudentAmount,
                    LecturerName = subject.LecturerName,
                    JoinedStudentList = subject.JoinedStudentList
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in subject.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                        {
                            result.IsJoined = true;
                            break;
                        }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfSubject");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all subject of Student/Admin
        public List<StudentSubjectResponseDTO> GetAllSubject(string studentAddress)
        {
            try
            {
                var result = new List<StudentSubjectResponseDTO>();
                var joinedSubjects = _subject.Find<Subject>(_ => true).ToList();
                foreach (var joinedSubject in joinedSubjects)
                {
                    var isExistedJoinedStudent = false;
                    if (studentAddress != null)
                        foreach (var joinedStudentList in joinedSubject.JoinedStudentList)
                        {
                            if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentSubjectResponseDTO()
                                {
                                    SubjectName = joinedSubject.SubjectName,
                                    SubjectAddress = joinedSubject.SubjectAddress,
                                    SubjectShortenName = joinedSubject.SubjectShortenName,
                                    MaxStudentAmount = joinedSubject.MaxStudentAmount,
                                    JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                                    SubjectStatus = joinedSubject.SubjectStatus,
                                    IsJoined = true,
                                    StartTime = joinedSubject.StartTime
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentSubjectResponseDTO()
                        {
                            SubjectName = joinedSubject.SubjectName,
                            SubjectAddress = joinedSubject.SubjectAddress,
                            SubjectShortenName = joinedSubject.SubjectShortenName,
                            MaxStudentAmount = joinedSubject.MaxStudentAmount,
                            JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                            SubjectStatus = joinedSubject.SubjectStatus,
                            IsJoined = false,
                            StartTime = joinedSubject.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSubject");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all subject of Lecturer
        public List<LecturerSubjectResponseDTO> GetAllSubjectOfLecturer(string lecturerAddress)
        {
            try
            {
                var result = new List<LecturerSubjectResponseDTO>();
                var joinedSubjects = _subject.Find<Subject>(_ => true).ToList();
                foreach (var joinedSubject in joinedSubjects)
                {
                    if (joinedSubject.LecturerAddress.ToLower() == lecturerAddress.ToLower())
                        result.Add(new LecturerSubjectResponseDTO()
                        {
                            SubjectName = joinedSubject.SubjectName,
                            SubjectAddress = joinedSubject.SubjectAddress,
                            SubjectShortenName = joinedSubject.SubjectShortenName,
                            MaxStudentAmount = joinedSubject.MaxStudentAmount,
                            JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                            SubjectStatus = joinedSubject.SubjectStatus,
                            StartTime = joinedSubject.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllSubjectOfLecturer");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CreateNewSubject(SubjectDTO subject)
        {
            try
            {
                await _subject.InsertOneAsync(new Subject()
                {
                    ChainNetworkId = subject.ChainNetworkId,
                    SubjectId = subject.SubjectId,
                    SubjectImg = subject.SubjectImg,
                    SubjectName = subject.SubjectName,
                    SubjectAddress = subject.SubjectAddress,
                    SubjectShortenName = subject.SubjectShortenName,
                    SubjectDescription = subject.SubjectDescription,
                    SubjectStatus = Status.Opening.ToString(),
                    SubjectHashIPFS = subject.SubjectHashIPFS,
                    DepartmentName = subject.DepartmentName,
                    StartTime = subject.StartTime,
                    EndTime = subject.EndTime,
                    EndTimeToResigter = subject.EndTimeToResigter,
                    EndTimeToComFirm = subject.EndTimeToComFirm,
                    MaxStudentAmount = subject.MaxStudentAmount,
                    LecturerAddress = subject.LecturerAddress,
                    LecturerName = subject.LecturerName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewSubject");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task<List<string>> GetSubjectListInProgress(int chainNetworkId)
        {
            long now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var missionList = await _subject.AsQueryable()
                .Where(x => x.StartTime <= now && x.EndTimeToComFirm > now && x.ChainNetworkId == chainNetworkId)
                .Select(x => x.SubjectAddress)
                .ToListAsync();
            return missionList;
        }

        public async Task UpdateStudentRegister(string subjectAddress, int chainNetworkId, string studentAddress)
        {

        }

        public async Task UpdateStudentCancelRegister(string subjectAddress, int chainNetworkId, string studentAddress)
        {

        }

        public async Task UpdateLecturerConfirmComplete(string subjectAddress, int chainNetworkId, List<string> studentAddressList)
        {

        }

        public async Task UpdateLecturerUnConfirmComplete(string subjectAddress, int chainNetworkId, string studentAddress)
        {

        }

        public async Task CloseSubject(string subjectAddress, int chainNetworkId)
        {

        }
    }
}
