﻿using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.MissionServices.DTOs;
using KLTN.Core.MissionServices.Interfaces;
using KLTN.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;
using KLTN.DAL.Models.DTOs;

namespace KLTN.Core.MissionServices.Implementations
{
    public class MissionService : IMissionService
    {
        private readonly ILogger<MissionService> _logger;
        private readonly IMongoCollection<Mission> _mission;

        private readonly WebAPIAppSettings _settings;

        public MissionService(ILogger<MissionService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _mission = database.GetCollection<Mission>(_settings.MissionCollectionName);
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
                            if(joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentMissionResponseDTO()
                                {
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
    }
}
