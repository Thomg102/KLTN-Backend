using KLTN.Common.Enums;
using KLTN.Common.Exceptions;
using KLTN.Core.ScholarshipServices.DTOs;
using KLTN.Core.ScholarshipServices.Interfaces;
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

namespace KLTN.Core.ScholarshipServices.Implementations
{
    public class ScholarshipService : IScholarshipService
    {
        private readonly ILogger<ScholarshipService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Scholarship> _scholarship;
        private readonly IMongoCollection<Student> _student;

        public ScholarshipService(ILogger<ScholarshipService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _scholarship = _context.GetCollection<Scholarship>(typeof(Scholarship).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
        }

        // Get detail of specific scholarship Student/Lecturer/Admin
        public ScholarshipDetailResponseDTO GetDetailOfScholarship(string scholarshipAddress, string studentAddress)
        {
            try
            {
                var scholarship = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower()).FirstOrDefault();
                var result = new ScholarshipDetailResponseDTO()
                {
                    LecturerInCharge = scholarship.LecturerInCharge,
                    ScholarshipAddress = scholarship.ScholarshipAddress,
                    ScholarShipDescription = scholarship.ScholarShipDescription,
                    ScholarshipHashIPFS = scholarship.ScholarshipHashIPFS,
                    ScholarshipId = scholarship.ScholarshipId,
                    ScholarshipName = scholarship.ScholarshipName,
                    ScholarshipStatus = scholarship.ScholarshipStatus,
                    StartTime = scholarship.StartTime,
                    EndTimeToComFirm = scholarship.EndTimeToComFirm,
                    EndTimeToResigter = scholarship.EndTimeToResigter,
                    EndTime = scholarship.EndTime,
                    JoinedStudentAmount = scholarship.JoinedStudentAmount,
                    LecturerName = scholarship.LecturerName,
                    TokenAmount = scholarship.TokenAmount,
                    JoinedStudentList = scholarship.JoinedStudentList,
                    ChainNetworkId = scholarship.ChainNetworkId,
                    ScholarshipImg = scholarship.ScholarshipImg,
                    IsJoined = false
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in scholarship.JoinedStudentList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
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
                //var studentDepartment = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().DepartmentName;
                var joinedScholarships = _scholarship.Find<Scholarship>(_ => true).ToList();
                /* if (studentAddress != null)
                     joinedScholarships = _scholarship.Find<Scholarship>(x => x.DepartmentName.ToLower() == studentDepartment.ToLower()).ToList();*/
                foreach (var joinedScholarship in joinedScholarships)
                {
                    var isExistedJoinedStudent = false;
                    if (studentAddress != null)
                        foreach (var joinedStudentList in joinedScholarship.JoinedStudentList)
                        {
                            if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentScholarshipResponseDTO()
                                {
                                    ScholarshipId = joinedScholarship.ScholarshipId,
                                    ScholarshipName = joinedScholarship.ScholarshipName,
                                    ScholarshipAddress = joinedScholarship.ScholarshipAddress,
                                    JoinedStudentAmount = joinedScholarship.JoinedStudentAmount,
                                    ScholarshipStatus = joinedScholarship.ScholarshipStatus,
                                    IsJoined = true,
                                    StartTime = joinedScholarship.StartTime,
                                    ChainNetworkId = joinedScholarship.ChainNetworkId,
                                    EndTime = joinedScholarship.EndTime,
                                    EndTimeToComFirm = joinedScholarship.EndTimeToComFirm,
                                    EndTimeToResigter = joinedScholarship.EndTimeToResigter,
                                    JoinedStudentList = joinedScholarship.JoinedStudentList,
                                    LecturerInCharge = joinedScholarship.LecturerInCharge,
                                    LecturerName = joinedScholarship.LecturerName,
                                    ScholarShipDescription = joinedScholarship.ScholarShipDescription,
                                    ScholarshipHashIPFS = joinedScholarship.ScholarshipHashIPFS,
                                    ScholarshipImg = joinedScholarship.ScholarshipImg,
                                    TokenAmount = joinedScholarship.TokenAmount
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentScholarshipResponseDTO()
                        {
                            ScholarshipId = joinedScholarship.ScholarshipId,
                            ScholarshipName = joinedScholarship.ScholarshipName,
                            ScholarshipAddress = joinedScholarship.ScholarshipAddress,
                            JoinedStudentAmount = joinedScholarship.JoinedStudentAmount,
                            ScholarshipStatus = joinedScholarship.ScholarshipStatus,
                            IsJoined = false,
                            StartTime = joinedScholarship.StartTime,
                            ChainNetworkId = joinedScholarship.ChainNetworkId,
                            EndTime = joinedScholarship.EndTime,
                            EndTimeToComFirm = joinedScholarship.EndTimeToComFirm,
                            EndTimeToResigter = joinedScholarship.EndTimeToResigter,
                            JoinedStudentList = joinedScholarship.JoinedStudentList,
                            LecturerInCharge = joinedScholarship.LecturerInCharge,
                            LecturerName = joinedScholarship.LecturerName,
                            ScholarShipDescription = joinedScholarship.ScholarShipDescription,
                            ScholarshipHashIPFS = joinedScholarship.ScholarshipHashIPFS,
                            ScholarshipImg = joinedScholarship.ScholarshipImg,
                            TokenAmount = joinedScholarship.TokenAmount
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
                var joinedScholarships = _scholarship.Find<Scholarship>(_ => true).ToList();
                foreach (var joinedScholarship in joinedScholarships)
                {
                    if (joinedScholarship.LecturerInCharge.ToLower() == lecturerAddress.ToLower())
                        result.Add(new LecturerScholarshipResponseDTO()
                        {
                            ScholarshipId = joinedScholarship.ScholarshipId,
                            ScholarshipName = joinedScholarship.ScholarshipName,
                            ScholarshipAddress = joinedScholarship.ScholarshipAddress,
                            JoinedStudentAmount = joinedScholarship.JoinedStudentAmount,
                            ScholarshipStatus = joinedScholarship.ScholarshipStatus,
                            StartTime = joinedScholarship.StartTime,
                            ChainNetworkId = joinedScholarship.ChainNetworkId,
                            EndTime = joinedScholarship.EndTime,
                            EndTimeToComFirm = joinedScholarship.EndTimeToComFirm,
                            EndTimeToResigter = joinedScholarship.EndTimeToResigter,
                            JoinedStudentList = joinedScholarship.JoinedStudentList,
                            LecturerInCharge = joinedScholarship.LecturerInCharge,
                            LecturerName = joinedScholarship.LecturerName,
                            ScholarShipDescription = joinedScholarship.ScholarShipDescription,
                            ScholarshipHashIPFS = joinedScholarship.ScholarshipHashIPFS,
                            ScholarshipImg = joinedScholarship.ScholarshipImg,
                            TokenAmount = joinedScholarship.TokenAmount
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

        public async Task CreateNewScholarship(ScholarshipDTO scholarship)
        {
            try
            {
                await _scholarship.InsertOneAsync(new Scholarship()
                {
                    ChainNetworkId = scholarship.ChainNetworkId,
                    ScholarshipId = scholarship.ScholarshipId,
                    ScholarshipImg = scholarship.ScholarshipImg,
                    ScholarshipName = scholarship.ScholarshipName,
                    ScholarshipAddress = scholarship.ScholarshipAddress,
                    ScholarshipStatus = Status.Opening.ToString(),
                    ScholarshipHashIPFS = scholarship.ScholarshipHashIPFS,
                    ScholarShipDescription = scholarship.ScholarShipDescription,
                    StartTime = scholarship.StartTime,
                    EndTimeToResigter = scholarship.EndTimeToResigter,
                    EndTimeToComFirm = scholarship.EndTimeToComFirm,
                    EndTime = scholarship.EndTime,
                    LecturerInCharge = scholarship.LecturerInCharge,
                    LecturerName = scholarship.LecturerName,
                    TokenAmount = scholarship.TokenAmount,
                    JoinedStudentList = new List<JoinedStudentDTO>() { }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewScholarship");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task<List<string>> GetScholarshipListInProgress(int chainNetworkId)
        {
            long now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var scholarshipList = await _scholarship.AsQueryable()
                .Where(x => x.StartTime <= now && x.EndTime > now && x.ChainNetworkId == chainNetworkId)
                .Select(x => x.ScholarshipAddress)
                .ToListAsync();
            return scholarshipList;
        }

        public async Task UpdateStudentRegister(string scholarshipAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Scholarship>.Filter.Where(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Scholarship>.Update.Push(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });

                await _scholarship.UpdateOneAsync(filter, update);

                var mission = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Scholarship>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount + 1);
                await _scholarship.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateStudentCancelRegister(string scholarshipAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Scholarship>.Filter.Where(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Scholarship>.Update.Pull(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });
                await _scholarship.UpdateOneAsync(filter, update);

                var mission = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Scholarship>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount - 1);
                await _scholarship.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentCancelRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerConfirmComplete(string scholarshipAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var mission = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Scholarship>.Filter.Where(x =>
                    x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower()
                    && x.ChainNetworkId == chainNetworkId
                );
                foreach (var joinedStudentList in mission.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                        {

                            int index = (mission.JoinedStudentList).IndexOf(joinedStudentList);
                            var update = Builders<Scholarship>.Update.Set(x => x.JoinedStudentList[index].IsCompleted, true);

                            await _scholarship.UpdateOneAsync(filter, update);
                            break;
                        }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerUnConfirmComplete(string scholarshipAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var mission = _scholarship.Find<Scholarship>(x => x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Scholarship>.Filter.Where(x =>
                                x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                foreach (var joinedStudentList in mission.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                        {
                            var update = Builders<Scholarship>.Update.Set(x => x.JoinedStudentList.Where(y => y.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().IsCompleted, false);

                            await _scholarship.UpdateOneAsync(filter, update);
                            break;
                        }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerUnConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CloseScholarship(string scholarshipAddress, int chainNetworkId)
        {
            try
            {
                var filter = Builders<Scholarship>.Filter.Where(x =>
                                x.ScholarshipAddress.ToLower() == scholarshipAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                var update = Builders<Scholarship>.Update.Set(x => x.ScholarshipStatus, Status.Closed.ToString());
                await _scholarship.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CloseScholarship");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task LockScholarship(List<string> scholarshipAddrs)
        {
            try
            {
                foreach (var scholarshipAddr in scholarshipAddrs)
                {
                    var filter = Builders<Scholarship>.Filter.Where(x => x.ScholarshipAddress.ToLower() == scholarshipAddr.ToLower());
                    await _scholarship.DeleteOneAsync(filter);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LockScholarship");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
