﻿using KLTN.Common.Exceptions;
using KLTN.Core.MissionServices.DTOs;
using KLTN.Core.MissionServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;
        private readonly ILogger<MissionController> _logger;
        public MissionController(ILogger<MissionController> logger, IMissionService missionService)
        {
            _logger = logger;
            _missionService = missionService;
        }

        [HttpGet("{missionAddress}")]
        public JsonResult GetDetailOfMission(string missionAddress, [FromQuery] string studentAddress)
        {
            var result = _missionService.GetDetailOfMission(missionAddress, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetAllMission([FromQuery] string studentAddress)
        {
            var result = _missionService.GetAllMission(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("lecturer/{lecturerAddress}")]
        public JsonResult GetAllMissionOfLecturer(string lecturerAddress)
        {
            var result = _missionService.GetAllMissionOfLecturer(lecturerAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("type")]
        public JsonResult GetListOfAllMissionType()
        {
            var result = _missionService.GetListOfAllMissionType();
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("")]
        public JsonResult CreateNewMission([FromBody] MissionDTO mission)
        {
            _missionService.CreateNewMission(mission);
            return new JsonResult(new SuccessResponseModel());
        }

        [HttpPost("UpdateStudentRegister/{missionAddress}/{chainNetworkId}")]
        public JsonResult UpdateStudentRegister(string missionAddress, int chainNetworkId, [FromBody] string studentAddress)
        {
            _missionService.UpdateStudentRegister(missionAddress, chainNetworkId, studentAddress);
            return new JsonResult(new SuccessResponseModel());
        }

        [HttpPost("UpdateStudentCancelRegister/{missionAddress}/{chainNetworkId}")]
        public JsonResult UpdateStudentCancelRegister(string missionAddress, int chainNetworkId, [FromBody] string studentAddress)
        {
            _missionService.UpdateStudentCancelRegister(missionAddress, chainNetworkId, studentAddress);
            return new JsonResult(new SuccessResponseModel());
        }

        [HttpPost("UpdateLecturerConfirmComplete/{missionAddress}/{chainNetworkId}")]
        public JsonResult UpdateLecturerConfirmComplete( string missionAddress, int chainNetworkId, [FromBody] List<string> studentList)
        {
            _missionService.UpdateLecturerConfirmComplete(missionAddress, chainNetworkId, studentList);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
