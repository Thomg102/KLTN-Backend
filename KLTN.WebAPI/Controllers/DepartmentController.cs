using KLTN.Common.Exceptions;
using KLTN.Core.DepartmentServices.DTOs;
using KLTN.Core.DepartmentServices.Interfaces;
using KLTN.DAL.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(ILogger<DepartmentController> logger, IDepartmentService departmentService)
        {
            _logger = logger;
            _departmentService = departmentService;
        }

        [HttpGet("{departmentShortenName}")]
        public JsonResult GetListSubjectOfDepartment(string departmentShortenName)
        {
            var result = _departmentService.GetListSubjectOfDepartment(departmentShortenName);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllDepartment()
        {
            var result = _departmentService.GetAllDepartment();
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewDepartment([FromBody] DepartmentDTO department)
        {
            _departmentService.CreateNewDepartment(department);
            return new JsonResult(new SuccessResponseModel());
        }

        [HttpPost("{departmentShortenName}")]
        public JsonResult CreateNewSubjectInDepartment(string departmentShortenName, [FromBody] SubjectType subjectType)
        {
            _departmentService.CreateNewSubjectInDepartment(departmentShortenName, subjectType);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
