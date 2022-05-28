using KLTN.Common.Enums;
using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.TuitionServices.DTOs;
using KLTN.Core.TuitionServices.Interfaces;
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

namespace KLTN.Core.TuitionServices.Implementations
{
    public class TuitionService : ITuitionService
    {
        private readonly ILogger<TuitionService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Tuition> _tuition;
        private readonly IMongoCollection<Student> _student;

        public TuitionService(ILogger<TuitionService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _tuition = _context.GetCollection<Tuition>(typeof(Tuition).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
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

        public async Task CreateNewTuition(TuitionDTO tuition)
        {
            try
            {
                await _tuition.InsertOneAsync(new Tuition()
                {
                    ChainNetwork = tuition.ChainNetwork,
                    ImgURL = tuition.Img,
                    TuitionName = tuition.TuitionName,
                    TuitionAddress = tuition.TuitionAddress,
                    TuitionDescription = tuition.TuitionDescription,
                    TuitionHashIPFS = tuition.TuitionHashIPFS,
                    TuitionStatus = Status.Opening.ToString(),
                    SchoolYear = tuition.SchoolYear,
                    StartTime = tuition.StartTime,
                    EndTime = tuition.EndTime,
                    TokenAmount = tuition.TokenAmount,
                    CurrencyAmount = tuition.CurrencyAmount,
                    LecturerInCharge = tuition.LecturerInCharge,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewTuition");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task<List<string>> GetTuitionListInProgress(int chainNetworkId)
        {
            long now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            var titionList = await _tuition.AsQueryable()
                .Where(x => x.StartTime <= now && x.EndTime < now)
                .Select(x => x.TuitionAddress)
                .ToListAsync();
            return titionList;
        }

        public async Task AddStudentToTuition(string tuitionpAddress, int chainNetworkId, List<string> studentAddressList)
        {

        }

        public async Task RemoveStudentFromTuition(string tuitionAddress, int chainNetworkId, string studentAddress)
        {

        }

        public async Task UpdateStudentCompeletedPayment(string tuitionAddress, int chainNetworkId, string studentAddress)
        {

        }

        public async Task CloseTuition(string tuitionAddress, int chainNetworkId)
        {

        }
    }
}
