using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.DepartmentServices.DTOs;
using KLTN.Core.DepartmentServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.DepartmentServices.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ILogger<DepartmentService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<Department> _department;
        public DepartmentService(ILogger<DepartmentService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;
            _department = _context.GetCollection<Department>(typeof(Department).Name);
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

        public async Task CreateNewDepartment(DepartmentDTO department)
        {
            try
            {
                await _department.InsertOneAsync(new Department()
                {
                    DepartmentName = department.DepartmentName,
                    DepartmentShortenName = department.DepartmentShortenName,
                    SubjectList = department.SubjectList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewDepartment");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CreateNewSubjectInDepartment(string departmentShortenName, SubjectType subjectType)
        {
            try
            {
                var filter = Builders<Department>.Filter.Eq("DepartmentShortenName", departmentShortenName);
                var update = Builders<Department>.Update.Push("SubjectList", subjectType);

                await _department.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewSubjectInDepartment");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
