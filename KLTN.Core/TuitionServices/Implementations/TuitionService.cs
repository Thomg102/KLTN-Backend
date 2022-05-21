using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.TuitionServices.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.TuitionServices.Implementations
{
    public class TuitionService
    {
        private readonly ILogger<TuitionService> _logger;
        private readonly IMongoCollection<Tuition> _tuition;
        private readonly IMongoCollection<Student> _student;

        private readonly WebAPIAppSettings _settings;

        public TuitionService(ILogger<TuitionService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _tuition = database.GetCollection<Tuition>(_settings.TuitionCollectionName);
            _student = database.GetCollection<Student>(_settings.StudentCollectionName);
        }

        // Get detail of specific tuition Student/Admin
        public TuitionDetailResponseDTO GetDetailOfTuition(string tuitionAddress, string studentAddress)
        {
            try
            {
                var tuition = _tuition.Find<Tuition>(x => x.TuitionAddress.ToLower() == tuitionAddress.ToLower()).FirstOrDefault();
                var result = new TuitionDetailResponseDTO()
                {
                    TuitionName = tuition.TuitionName,
                    TuitionDescription = tuition.TuitionDescription,
                    TuitionStatus = tuition.TuitionStatus,
                    StartTime = tuition.StartTime,
                    EndTime = tuition.EndTime,
                    JoinedStudentAmount = tuition.JoinedStudentAmount,
                    TokenAmount = tuition.TokenAmount,
                    JoinedStudentList = tuition.JoinedStudentList
                };
                if (studentAddress != null)
                    foreach (var joinedStudentList in tuition.JoinedStudentList)
                        if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                        {
                            result.IsJoined = true;
                            break;
                        }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfTuition");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        //Get all tuition of Student/Admin
        public List<StudentTuitionResponseDTO> GetAllTuition(string studentAddress)
        {
            try
            {
                var result = new List<StudentTuitionResponseDTO>();
                var studentSchoolYear = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().SchoolYear;
                var joinedTuitions = _tuition.Find<Tuition>(_ => true).ToList();
                if (studentAddress != null)
                    joinedTuitions = _tuition.Find<Tuition>(x => x.SchoolYear == studentSchoolYear).ToList();
                foreach (var joinedTuition in joinedTuitions)
                {
                    var isExistedJoinedStudent = false;
                    if (studentAddress != null)
                        foreach (var joinedStudentList in joinedTuition.JoinedStudentList)
                        {
                            if (joinedStudentList.StudentName.ToLower() == studentAddress.ToLower())
                            {
                                result.Add(new StudentTuitionResponseDTO()
                                {
                                    TuitionName = joinedTuition.TuitionName,
                                    TuitionAddress = joinedTuition.TuitionAddress,
                                    JoinedStudentAmount = joinedTuition.JoinedStudentAmount,
                                    TuitionStatus = joinedTuition.TuitionStatus,
                                    IsJoined = true,
                                    StartTime = joinedTuition.StartTime
                                });
                                isExistedJoinedStudent = true;
                                break;
                            }
                        }
                    if (!isExistedJoinedStudent)
                        result.Add(new StudentTuitionResponseDTO()
                        {
                            TuitionName = joinedTuition.TuitionName,
                            TuitionAddress = joinedTuition.TuitionAddress,
                            JoinedStudentAmount = joinedTuition.JoinedStudentAmount,
                            TuitionStatus = joinedTuition.TuitionStatus,
                            IsJoined = false,
                            StartTime = joinedTuition.StartTime
                        });
                }
                return result.OrderByDescending(x => x.StartTime).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllTuition");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
