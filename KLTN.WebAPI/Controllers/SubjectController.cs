using KLTN.Common.Exceptions;
using KLTN.Core.SubjectServices.DTOs;
using KLTN.Core.SubjectServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ILogger<SubjectController> _logger;
        public SubjectController(ILogger<SubjectController> logger, ISubjectService subjectService)
        {
            _logger = logger;
            _subjectService = subjectService;
        }

        [HttpGet("{subjectAddress}")]
        public JsonResult GetDetailOfSubject(string subjectAddress, [FromQuery] string studentAddress)
        {
            var result = _subjectService.GetDetailOfSubject(subjectAddress, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllSubject([FromQuery] string studentAddress)
        {
            var result = _subjectService.GetAllSubject(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("lecturer/{lecturerAddress}")]
        public JsonResult GetAllSubjectOfLecturer(string lecturerAddress)
        {
            var result = _subjectService.GetAllSubjectOfLecturer(lecturerAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewSubject([FromBody] SubjectDTO subject)
        {
            _subjectService.CreateNewSubject(subject);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
