﻿using KLTN.Common.Exceptions;
using KLTN.Core.LecturerServices.DTOs;
using KLTN.Core.LecturerServicess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;
        private readonly ILogger<LecturerController> _logger;
        public LecturerController(ILogger<LecturerController> logger, ILecturerService lecturerService)
        {
            _logger = logger;
            _lecturerService = lecturerService;
        }

        [HttpGet("{lecturerAddress}")]
        public JsonResult GetDetailOfLecturer(string lecturerAddress)
        {
            var result = _lecturerService.GetDetailOfLecturer(lecturerAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllLecturer()
        {
            var result = _lecturerService.GetAllLecturer();
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewLecturer([FromBody] LecturerDTO lecturer)
        {
            _lecturerService.CreateNewLectuter(lecturer);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
