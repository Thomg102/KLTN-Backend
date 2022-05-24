using KLTN.Common.Exceptions;
using KLTN.Core.StudentServices.DTOs;
using KLTN.Core.StudentServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;
        public StudentController(ILogger<StudentController> logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }

        [HttpGet("{studentAddress}")]
        public JsonResult GetDetailOfStudent(string studentAddress)
        {
            var result = _studentService.GetDetailOfStudent(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("histories/{studentAddress}")]
        public JsonResult GetBalanceHistoriesOfStudent(string studentAddress)
        {
            var result = _studentService.GetBalanceHistoriesOfStudent(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("subject/{studentAddress}")]
        public JsonResult GetListSubjectCertificate(string studentAddress)
        {
            var result = _studentService.GetListSubjectCertificate(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("mission/{studentAddress}")]
        public JsonResult GetListMissionCertificate(string studentAddress)
        {
            var result = _studentService.GetListMissionCertificate(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllStudent()
        {
            var result = _studentService.GetAllStudent();
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewStudent([FromBody] StudentDTO student)
        {
            _studentService.CreateNewStudent(student);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
