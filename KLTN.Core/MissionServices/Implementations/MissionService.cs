using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.MissionServices.DTOs;
using KLTN.Core.MissionServices.Interfaces;
using KLTN.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;
using KLTN.DAL.Models.DTOs;
using KLTN.Common.Enums;
using KLTN.DAL;
using KLTN.DAL.Models.Entities;

namespace KLTN.Core.MissionServices.Implementations
{
    public class MissionService : IMissionService
    {
        private readonly ILogger<MissionService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Mission> _mission;
        private readonly IMongoCollection<Student> _student;
        private readonly IMongoCollection<MissionType> _missionType;

        public MissionService(ILogger<MissionService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;
            _mission = _context.GetCollection<Mission>(typeof(Mission).Name);
            _missionType = _context.GetCollection<MissionType>(typeof(MissionType).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
        }

        // Get detail of specific mission Student/Lecturer/Admin
        public MissionDetailResponseDTO GetDetailOfMission(string missionAddress, string studentAddress)
        {
            try
            {
                var mission = _mission.Find<Mission>(x => x.MissionAddress.ToLower() == missionAddress.ToLower()).FirstOrDefault();
                var result = new MissionDetailResponseDTO()
                {
                    MissionName = mission.MissionName,
                    MissionShortenName = mission.MissionShortenName,
                    MissionDescription = mission.MissionDescription,
                    MissionStatus = mission.MissionStatus,
                    DepartmentName = mission.DepartmentName,
                    StartTime = mission.StartTime,
                    EndTime = mission.EndTime,
                    EndTimeToResigter = mission.EndTimeToResigter,
                    EndTimeToComFirm = mission.EndTimeToComFirm,
                    MaxStudentAmount = mission.MaxStudentAmount,
                    JoinedStudentAmount = mission.JoinedStudentAmount,
                    LecturerName = mission.LecturerName,
                    TokenAmount = mission.TokenAmount,
                    JoinedStudentList = mission.JoinedStudentList
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in mission.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                        {
                            result.IsJoined = true;
                            break;
                        }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfMission");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all mission of Student/Admin
        public List<StudentMissionResponseDTO> GetAllMission(string studentAddress)
        {
            try
            {
                var result = new List<StudentMissionResponseDTO>();
                var joinedMissions = _mission.Find<Mission>(_ => true).ToList();
                foreach (var joinedMission in joinedMissions)
                {
                    var isExistedJoinedStudent = false;
                    if (studentAddress != null)
                        foreach (var joinedStudentList in joinedMission.JoinedStudentList)
                        {
                            if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentMissionResponseDTO()
                                {
                                    MissionId = joinedMission.MissionId,
                                    MissionName = joinedMission.MissionName,
                                    MissionAddress = joinedMission.MissionAddress,
                                    MissionShortenName = joinedMission.MissionShortenName,
                                    MaxStudentAmount = joinedMission.MaxStudentAmount,
                                    JoinedStudentAmount = joinedMission.JoinedStudentAmount,
                                    MissionStatus = joinedMission.MissionStatus,
                                    IsJoined = true,
                                    StartTime = joinedMission.StartTime
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentMissionResponseDTO()
                        {
                            MissionId = joinedMission.MissionId,
                            MissionName = joinedMission.MissionName,
                            MissionAddress = joinedMission.MissionAddress,
                            MissionShortenName = joinedMission.MissionShortenName,
                            MaxStudentAmount = joinedMission.MaxStudentAmount,
                            JoinedStudentAmount = joinedMission.JoinedStudentAmount,
                            MissionStatus = joinedMission.MissionStatus,
                            IsJoined = false,
                            StartTime = joinedMission.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllMission");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task<List<string>> GetMissionListInProgress(int chainNetworkId)
        {
            long now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var missionList = await _mission.AsQueryable()
                .Where(x => x.StartTime <= now && x.EndTimeToComFirm > now && x.ChainNetworkId == chainNetworkId)
                .Select(x => x.MissionAddress)
                .ToListAsync();
            return missionList;
        }

        //Get all mission of Lecturer
        public List<LecturerMissionResponseDTO> GetAllMissionOfLecturer(string lecturerAddress)
        {
            try
            {
                var result = new List<LecturerMissionResponseDTO>();
                var joinedMissions = _mission.Find<Mission>(_ => true).ToList();
                foreach (var joinedMission in joinedMissions)
                {
                    if (joinedMission.LecturerAddress.ToLower() == lecturerAddress.ToLower())
                        result.Add(new LecturerMissionResponseDTO()
                        {
                            MissionName = joinedMission.MissionName,
                            MissionAddress = joinedMission.MissionAddress,
                            MissionShortenName = joinedMission.MissionShortenName,
                            MaxStudentAmount = joinedMission.MaxStudentAmount,
                            JoinedStudentAmount = joinedMission.JoinedStudentAmount,
                            MissionStatus = joinedMission.MissionStatus,
                            StartTime = joinedMission.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllMissionOfLecturer");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public List<MissionTypeResponseDTO> GetListOfAllMissionType()
        {
            try
            {
                var result = new List<MissionTypeResponseDTO>();
                var missionTypeList = _missionType.Find<MissionType>(_ => true).ToList();
                if (missionTypeList != null && missionTypeList.Count > 0)
                    foreach (var missionType in missionTypeList)
                        result.Add(new MissionTypeResponseDTO()
                        {
                            MissionName = missionType.MissionName,
                            MissionHash = missionType.MissionHash
                        });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfAllProductType");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CreateNewMission(MissionDTO mission)
        {
            try
            {
                await _mission.InsertOneAsync(new Mission()
                {
                    ChainNetworkId = mission.ChainNetworkId,
                    MissionId = mission.MissionId,
                    MissionImg = mission.MissionImg,
                    MissionName = mission.MissionName,
                    MissionAddress = mission.MissionAddress,
                    MissionShortenName = mission.MissionShortenName,
                    MissionDescription = mission.MissionDescription,
                    MissionStatus = Status.Opening.ToString(),
                    MissionHashIPFS = mission.MissionHashIPFS,
                    DepartmentName = mission.DepartmentName,
                    StartTime = mission.StartTime,
                    EndTime = mission.EndTime,
                    EndTimeToResigter = mission.EndTimeToResigter,
                    EndTimeToComFirm = mission.EndTimeToComFirm,
                    MaxStudentAmount = mission.MaxStudentAmount,
                    LecturerAddress = mission.LecturerAddress,
                    LecturerName = mission.LecturerName,
                    TokenAmount = mission.TokenAmount,
                    JoinedStudentList = new List<JoinedStudentDTO>() { }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewMission");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateStudentRegister(string missionAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Mission>.Filter.Where(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Mission>.Update.Push(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });

                await _mission.UpdateOneAsync(filter, update);

                var mission = _mission.Find<Mission>(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Mission>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount + 1);
                await _mission.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateStudentCancelRegister(string missionAddress, int chainNetworkId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                var filter = Builders<Mission>.Filter.Where(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId);
                var update = Builders<Mission>.Update.Pull(x => x.JoinedStudentList, new JoinedStudentDTO()
                {
                    StudentAddress = studentAddress.ToLower(),
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    IsCompleted = false,
                });
                await _mission.UpdateOneAsync(filter, update);

                var mission = _mission.Find<Mission>(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var updateJoinedStudentAmount = Builders<Mission>.Update.Set(x => x.JoinedStudentAmount, mission.JoinedStudentAmount - 1);
                await _mission.UpdateOneAsync(filter, updateJoinedStudentAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStudentCancelRegister");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerConfirmComplete(string missionAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var mission = _mission.Find<Mission>(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Mission>.Filter.Where(x =>
                    x.MissionAddress.ToLower() == missionAddress.ToLower()
                    && x.ChainNetworkId == chainNetworkId
                );
                foreach (var joinedStudentList in mission.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentAddress.ToLower() == studentAddress.ToLower())
                            {

                                int index = (mission.JoinedStudentList).IndexOf(joinedStudentList);
                                var update = Builders<Mission>.Update.Set(x => x.JoinedStudentList[index].IsCompleted, true);

                                await _mission.UpdateOneAsync(filter, update);
                                break;
                            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateLecturerUnConfirmComplete(string missionAddress, int chainNetworkId, List<string> studentAddressList)
        {
            try
            {
                var mission = _mission.Find<Mission>(x => x.MissionAddress.ToLower() == missionAddress.ToLower() && x.ChainNetworkId == chainNetworkId).FirstOrDefault();
                var filter = Builders<Mission>.Filter.Where(x =>
                                x.MissionAddress.ToLower() == missionAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                foreach (var joinedStudentList in mission.JoinedStudentList)
                    foreach (var studentAddress in studentAddressList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                        {
                            var update = Builders<Mission>.Update.Set(x => x.JoinedStudentList.Where(y => y.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().IsCompleted, false);

                            await _mission.UpdateOneAsync(filter, update);
                            break;
                        }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateLecturerUnConfirmComplete");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CloseMission(string missionAddress, int chainNetworkId)
        {
            try
            {
                var filter = Builders<Mission>.Filter.Where(x =>
                                x.MissionAddress.ToLower() == missionAddress.ToLower()
                                && x.ChainNetworkId == chainNetworkId
                            );
                var update = Builders<Mission>.Update.Set(x => x.MissionStatus, Status.Closed.ToString());
                await _mission.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CloseMission");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task LockMission(List<string> missionAddrs)
        {
            try
            {
                var filter = Builders<Mission>.Filter.In(x => x.MissionAddress.ToLower(), missionAddrs.ConvertAll(d => d.ToLower()));
                await _mission.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LockMission");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
