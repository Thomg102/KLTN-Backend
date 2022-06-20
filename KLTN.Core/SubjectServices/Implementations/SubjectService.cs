using KLTN.Common.Enums;
using KLTN.Common.Exceptions;
using KLTN.Core.SubjectServices.DTOs;
using KLTN.Core.SubjectServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.SubjectServices.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ILogger<SubjectService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Subject> _subject;
        private readonly IMongoCollection<Student> _student;

        public SubjectService(ILogger<SubjectService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _subject = _context.GetCollection<Subject>(typeof(Subject).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
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
                    JoinedStudentList = subject.JoinedStudentList,
                    ChainNetworkId = subject.ChainNetworkId,
                    LecturerAddress = subject.LecturerAddress,
                    SubjectAddress = subject.SubjectAddress,
                    SubjectHashIPFS = subject.SubjectHashIPFS,
                    SubjectId = subject.SubjectId,
                    SubjectImg = subject.SubjectImg,
                    IsJoined = false
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in subject.JoinedStudentList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
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
                            if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentSubjectResponseDTO()
                                {
                                    SubjectName = joinedSubject.SubjectName,
                                    SubjectShortenName = joinedSubject.SubjectShortenName,
                                    SubjectDescription = joinedSubject.SubjectDescription,
                                    SubjectStatus = joinedSubject.SubjectStatus,
                                    DepartmentName = joinedSubject.DepartmentName,
                                    StartTime = joinedSubject.StartTime,
                                    EndTime = joinedSubject.EndTime,
                                    EndTimeToResigter = joinedSubject.EndTimeToResigter,
                                    EndTimeToComFirm = joinedSubject.EndTimeToComFirm,
                                    MaxStudentAmount = joinedSubject.MaxStudentAmount,
                                    JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                                    LecturerName = joinedSubject.LecturerName,
                                    JoinedStudentList = joinedSubject.JoinedStudentList,
                                    ChainNetworkId = joinedSubject.ChainNetworkId,
                                    LecturerAddress = joinedSubject.LecturerAddress,
                                    SubjectAddress = joinedSubject.SubjectAddress,
                                    SubjectHashIPFS = joinedSubject.SubjectHashIPFS,
                                    SubjectId = joinedSubject.SubjectId,
                                    SubjectImg = joinedSubject.SubjectImg,
                                    IsJoined = true
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentSubjectResponseDTO()
                        {
                            SubjectName = joinedSubject.SubjectName,
                            SubjectShortenName = joinedSubject.SubjectShortenName,
                            SubjectDescription = joinedSubject.SubjectDescription,
                            SubjectStatus = joinedSubject.SubjectStatus,
                            DepartmentName = joinedSubject.DepartmentName,
                            StartTime = joinedSubject.StartTime,
                            EndTime = joinedSubject.EndTime,
                            EndTimeToResigter = joinedSubject.EndTimeToResigter,
                            EndTimeToComFirm = joinedSubject.EndTimeToComFirm,
                            MaxStudentAmount = joinedSubject.MaxStudentAmount,
                            JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                            LecturerName = joinedSubject.LecturerName,
                            JoinedStudentList = joinedSubject.JoinedStudentList,
                            ChainNetworkId = joinedSubject.ChainNetworkId,
                            LecturerAddress = joinedSubject.LecturerAddress,
                            SubjectAddress = joinedSubject.SubjectAddress,
                            SubjectHashIPFS = joinedSubject.SubjectHashIPFS,
                            SubjectId = joinedSubject.SubjectId,
                            SubjectImg = joinedSubject.SubjectImg,
                            IsJoined = false
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
                            SubjectShortenName = joinedSubject.SubjectShortenName,
                            SubjectDescription = joinedSubject.SubjectDescription,
                            SubjectStatus = joinedSubject.SubjectStatus,
                            DepartmentName = joinedSubject.DepartmentName,
                            StartTime = joinedSubject.StartTime,
                            EndTime = joinedSubject.EndTime,
                            EndTimeToResigter = joinedSubject.EndTimeToResigter,
                            EndTimeToComFirm = joinedSubject.EndTimeToComFirm,
                            MaxStudentAmount = joinedSubject.MaxStudentAmount,
                            JoinedStudentAmount = joinedSubject.JoinedStudentAmount,
                            LecturerName = joinedSubject.LecturerName,
                            JoinedStudentList = joinedSubject.JoinedStudentList,
                            ChainNetworkId = joinedSubject.ChainNetworkId,
                            LecturerAddress = joinedSubject.LecturerAddress,
                            SubjectAddress = joinedSubject.SubjectAddress,
                            SubjectHashIPFS = joinedSubject.SubjectHashIPFS,
                            SubjectId = joinedSubject.SubjectId,
                            SubjectImg = joinedSubject.SubjectImg,
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
                    LecturerName = subject.LecturerName,
                    JoinedStudentList = new List<JoinedStudentDTO>() { }
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
            var subjectList = await _subject.AsQueryable()
                .Where(x => x.StartTime <= now && x.EndTimeToComFirm > now && x.ChainNetworkId == chainNetworkId)
                .Select(x => x.SubjectAddress)
                .ToListAsync();
            return subjectList;
        }

        public async Task UpdateStudentRegister(string subjectAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Subject>.Filter.Where(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Subject>.Update.Push(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });

                await _subject.UpdateOneAsync(filter, update);

                var mission = _subject.Find<Subject>(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Subject>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount + 1);
                await _subject.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateStudentCancelRegister(string subjectAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Subject>.Filter.Where(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Subject>.Update.Pull(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });

                await _subject.UpdateOneAsync(filter, update);

                var mission = _subject.Find<Subject>(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Subject>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount - 1);
                await _subject.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentCancelRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerConfirmComplete(string subjectAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var Subject = _subject.Find<Subject>(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Subject>.Filter.Where(x =>
                    x.SubjectAddress.ToLower() == subjectAddress.ToLower()
                    && x.ChainNetworkId == chainNetworkId
                );
                foreach (var joinedStudentList in Subject.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                        {

                            int index = (Subject.JoinedStudentList).IndexOf(joinedStudentList);
                            var update = Builders<Subject>.Update.Set(x => x.JoinedStudentList[index].IsCompleted, true);

                            await _subject.UpdateOneAsync(filter, update);
                            break;
                        }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerUnConfirmComplete(string subjectAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var Subject = _subject.Find<Subject>(x => x.SubjectAddress.ToLower() == subjectAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Subject>.Filter.Where(x =>
                                x.SubjectAddress.ToLower() == subjectAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                foreach (var joinedStudentList in Subject.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                        {
                            var update = Builders<Subject>.Update.Set(x => x.JoinedStudentList.Where(y => y.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().IsCompleted, false);

                            await _subject.UpdateOneAsync(filter, update);
                            break;
                        }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerUnConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CloseSubject(string subjectAddress, int chainNetworkId)
        {
            try
            {
                var filter = Builders<Subject>.Filter.Where(x =>
                                x.SubjectAddress.ToLower() == subjectAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                var update = Builders<Subject>.Update.Set(x => x.SubjectStatus, Status.Closed.ToString());
                await _subject.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CloseSubject");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task LockSubject(List<string> subjectAddrs)
        {
            try
            {
                foreach (var subjectAddr in subjectAddrs)
                {
                    var filter = Builders<Subject>.Filter.Where(x => x.SubjectAddress.ToLower() == subjectAddr.ToLower());
                    await _subject.DeleteOneAsync(filter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LockSubject");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
