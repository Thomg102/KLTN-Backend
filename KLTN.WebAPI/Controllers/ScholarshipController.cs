using KLTN.Core.ScholarshipServices.DTOs;
using KLTN.Core.ScholarshipServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScholarshipController : ControllerBase
    {
        private readonly IScholarshipService _scholarshipService;
        private readonly ILogger<ScholarshipController> _logger;
        public ScholarshipController(ILogger<ScholarshipController> logger, IScholarshipService scholarshipService)
        {
            _logger = logger;
            _scholarshipService = scholarshipService;
        }

        [HttpGet("{scholarshipAddress}")]
        public JsonResult GetDetailOfScholarship(string scholarshipAddress, [FromQuery] string studentAddress)
        {
            var result = _scholarshipService.GetDetailOfScholarship(scholarshipAddress, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllScholarship([FromQuery] string studentAddress)
        {
            var result = _scholarshipService.GetAllScholarship(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("lecturer/{lecturerAddress}")]
        public JsonResult GetAllScholarshipOfLecturer(string lecturerAddress)
        {
            var result = _scholarshipService.GetAllScholarshipOfLecturer(lecturerAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewScholarship([FromBody] ScholarshipDTO scholarship)
        {
            _scholarshipService.CreateNewScholarship(scholarship);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
