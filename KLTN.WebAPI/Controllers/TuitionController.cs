using KLTN.Common.Exceptions;
using KLTN.Core.TuitionServices.DTOs;
using KLTN.Core.TuitionServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;
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
