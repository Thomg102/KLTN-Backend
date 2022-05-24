using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.LecturerServices.DTOs;
using KLTN.Core.LecturerServicess.DTOs;
using KLTN.Core.LecturerServicess.Interfaces;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.LecturerServicess.Implementations
{
    public class LecturerService : ILecturerService
    {
        private readonly ILogger<LecturerService> _logger;
        private readonly IMongoCollection<Lecturer> _lecturer;

        private readonly WebAPIAppSettings _settings;
        public LecturerService(ILogger<LecturerService> logger, IOptions<WebAPIAppSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _lecturer = database.GetCollection<Lecturer>(_settings.LecturerCollectionName);
        }

        // Get detail info of specific Lecturer
        public LecturerDetailInfoResponseDTO GetDetailOfLecturer(string lecturerAddress)
        {
            try
            {
                var lecturerInfo = _lecturer.Find<Lecturer>(x => x.LecturerAddress.ToLower() == lecturerAddress.ToLower()).FirstOrDefault();
                return JsonConvert.DeserializeObject<LecturerDetailInfoResponseDTO>(JsonConvert.SerializeObject(lecturerInfo));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfLecturer");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get all Lecturers
        public List<LecturerResponseDTO> GetAllLecturer()
        {
            try
            {
                var result = new List<LecturerResponseDTO>();
                var lecturerInfo = _lecturer.Find<Lecturer>(_ => true).ToList();
                foreach (var info in lecturerInfo)
                {
                    var lecturer = new LecturerResponseDTO
                    {
                        LecturerName = info.LecturerName,
                        LecturerId = info.LecturerId,
                        LecturerAddress = info.LecturerAddress,
                        DepartmentName = info.DepartmentName,
                        DepartmentShortenName = info.DepartmentShortenName
                    };
                    result.Add(lecturer);
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllLecturer");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CreateNewLectuter(LecturerDTO lecturer)
        {
            try
            {
                await _lecturer.InsertOneAsync(new Lecturer()
                {
                    LecturerName = lecturer.LecturerName,
                    LecturerId = lecturer.LecturerId,
                    LecturerAddress = lecturer.LecturerAddress,
                    DepartmentName = lecturer.DepartmentName,
                    DepartmentShortenName = lecturer.DepartmentShortenName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewLectuter");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
