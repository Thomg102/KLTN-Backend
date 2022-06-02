using KLTN.Common.Exceptions;
using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.RequestActivateServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivateRequestController
    {
        private readonly IActivateRequestService _activateRequestService;
        private readonly ILogger<ActivateRequestController> _logger;
        public ActivateRequestController(ILogger<ActivateRequestController> logger, IActivateRequestService activateRequestService)
        {
            _logger = logger;
            _activateRequestService = activateRequestService;
        }

        [HttpGet("activate-requesting")]
        public JsonResult GetListOfActivateRequesting([FromQuery] string studentAddress)
        {
            var result = _activateRequestService.GetListOfActivateRequesting(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("activate-requested")]
        public JsonResult GetListOfActivatedRequest([FromQuery] string studentAddress)
        {
            var result = _activateRequestService.GetListOfActivatedRequest(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("{requestId}")]
        public JsonResult GetDetailOfActivateRequest(string requestId)
        {
            var result = _activateRequestService.GetDetailOfActivateRequest(requestId);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewActivateRequest([FromBody] RequestActivateDTO activateRequest)
        {
            _activateRequestService.CreateNewActivateRequest(activateRequest);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
