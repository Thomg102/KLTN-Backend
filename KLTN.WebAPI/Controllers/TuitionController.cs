using KLTN.Core.TuitionServices.DTOs;
using KLTN.Core.TuitionServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;
namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TuitionController : ControllerBase
    {
        private readonly ITuitionService _tuitionService;
        private readonly ILogger<SubjectController> _logger;
        public TuitionController(ILogger<SubjectController> logger, ITuitionService tuitionService)
        {
            _logger = logger;
            _tuitionService = tuitionService;
        }

        [HttpGet("lecturer/{lecturerAddress}")]
        public JsonResult GetAllTuitionOfLecturer(string lecturerAddress)
        {
            var result = _tuitionService.GetAllTuitionOfLecturer(lecturerAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("{tuitionAddress}")]
        public JsonResult GetDetailOfTuition(string tuitionAddress, [FromQuery] string studentAddress)
        {
            var result = _tuitionService.GetDetailOfTuition(tuitionAddress, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllTuition([FromQuery] string studentAddress)
        {
            var result = _tuitionService.GetAllTuition(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewTuition([FromBody] TuitionDTO tuition)
        {
            _tuitionService.CreateNewTuition(tuition);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
