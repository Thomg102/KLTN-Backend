using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ActiveRequestServices.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.RequestActiveServices.Implementations
{
    public class ActivateRequestService
    {
        private readonly ILogger<ActivateRequestService> _logger;
        private readonly IMongoCollection<ActivateRequest> _activateRequest;
        private readonly IMongoCollection<Student> _student;

        private readonly WebAPIAppSettings _settings;

        public ActivateRequestService(ILogger<ActivateRequestService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _activateRequest = database.GetCollection<ActivateRequest>(_settings.ActiveRequestCollectionName);
            _student = database.GetCollection<Student>(_settings.StudentCollectionName);
        }

        // Get list of Activate requesting Student/Admin
        public List<ActivateRequestResponseDTO> GetListOfActivateRequesting(string studentAddress)
        {
            try
            {
                var result = new List<ActivateRequestResponseDTO>();
                var activateRequestingList = new List<ActivateRequest>();
                if (studentAddress == null)
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => !x.IsActived).ToList();
                else
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => x.StudentName.ToLower() == studentAddress.ToLower() && !x.IsActived).ToList();
                if (activateRequestingList != null && activateRequestingList.Count > 0)
                    foreach (var activeRequesting in activateRequestingList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activeRequesting.RequestId,
                            ProductName = activeRequesting.ProductName,
                            ProductId = activeRequesting.ProductId,
                            StudentName = activeRequesting.StudentName,
                            ProductHahIPFS = activeRequesting.ProductHahIPFS,
                            AmountToActive = activeRequesting.AmountToActive,
                            ProductTypeName = activeRequesting.ProductTypeName,
                            IsActived = activeRequesting.IsActived,
                        });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfActivateRequesting");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get list of activated request Student/Admin
        public List<ActivateRequestResponseDTO> GetListOfActivatedRequest(string studentAddress)
        {
            try
            {
                var result = new List<ActivateRequestResponseDTO>();
                var activedRequestList = new List<ActivateRequest>();
                if (studentAddress == null)
                    activedRequestList = _activateRequest.Find<ActivateRequest>(x => x.IsActived).ToList();
                else
                    activedRequestList = _activateRequest.Find<ActivateRequest>(x => x.StudentName.ToLower() == studentAddress.ToLower() && x.IsActived).ToList();
                if (activedRequestList != null && activedRequestList.Count > 0)
                    foreach (var activatedRequest in activedRequestList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activatedRequest.RequestId,
                            ProductName = activatedRequest.ProductName,
                            ProductId = activatedRequest.ProductId,
                            StudentName = activatedRequest.StudentName,
                            ProductHahIPFS = activatedRequest.ProductHahIPFS,
                            AmountToActive = activatedRequest.AmountToActive,
                            ProductTypeName = activatedRequest.ProductTypeName,
                            IsActived = activatedRequest.IsActived
                        });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfActivatedRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get Detail of activate request Student/Admin
        public ActivateRequestResponseDTO GetDetailOfActivateRequest(string requestId)
        {
            try
            {
                var activateRequest = _activateRequest.Find<ActivateRequest>(x => x.RequestId.ToLower() == requestId.ToLower()).FirstOrDefault();
                return (new ActivateRequestResponseDTO()
                {
                    ProductName = activateRequest.ProductName,
                    ProductId = activateRequest.ProductId,
                    StudentName = activateRequest.StudentName,
                    ProductHahIPFS = activateRequest.ProductHahIPFS,
                    AmountToActive = activateRequest.AmountToActive,
                    ProductTypeName = activateRequest.ProductTypeName,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
