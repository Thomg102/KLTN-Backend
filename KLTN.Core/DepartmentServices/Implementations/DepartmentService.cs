using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.DepartmentServices.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.DepartmentServices.Implementations
{
    public class DepartmentService
    {
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMongoCollection<Department> _department;

        private readonly WebAPIAppSettings _settings;
        public DepartmentService(ILogger<DepartmentService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _department = database.GetCollection<Department>(_settings.DepartmentCollectionName);
        }

        // Get list subjects of specific Department
        public List<SubjectType> GetListSubjectOfDepartment(string departmentShortenName)
        {
            try
            {
                var result = new List<SubjectType>();
                var departmentInfo = _department.Find<Department>(x => x.DepartmentShortenName.ToLower() == departmentShortenName.ToLower()).FirstOrDefault();
                foreach (var subject in departmentInfo.SubjectList)
                    result.Add(subject);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListSubjectOfDepartment");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get all department
        public List<DepartmentResponeDTO> GetAllDepartment()
        {
            try
            {
                var result = new List<DepartmentResponeDTO>();
                var departmentList = _department.Find<Department>(_ => true).ToList();
                foreach (var department in departmentList)
                    result.Add(new DepartmentResponeDTO()
                    {
                        DepartmentName = department.DepartmentName,
                        DepartmentShortenName = department.DepartmentShortenName
                    });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllDepartment");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
