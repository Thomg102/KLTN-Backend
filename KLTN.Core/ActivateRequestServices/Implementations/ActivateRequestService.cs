using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.ActiveRequestServices.DTOs;
using KLTN.Core.RequestActiveServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.RequestActiveServices.Implementations
{
    public class ActivateRequestService : IActivateRequestService
    {
        private readonly ILogger<ActivateRequestService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<ActivateRequest> _activateRequest;
        private readonly IMongoCollection<Student> _student;

        public ActivateRequestService(ILogger<ActivateRequestService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;
            _activateRequest = _context.GetCollection<ActivateRequest>(typeof(ActivateRequest).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
        }

        // Get list of Activate requesting Student/Admin
        public List<ActivateRequestResponseDTO> GetListOfActivateRequesting(string studentAddress)
        {
            try
            {
                var result = new List<ActivateRequestResponseDTO>();
                var activateRequestingList = new List<ActivateRequest>();
                if (studentAddress == null)
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => x.IsActived == false).ToList();
                else
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => x.StudentAddress.ToLower() == studentAddress.ToLower() && x.IsActived == false).ToList();
                if (activateRequestingList != null && activateRequestingList.Count > 0)
                    foreach (var activeRequesting in activateRequestingList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activeRequesting.RequestId,
                            ProductName = activeRequesting.ProductName,
                            ProductId = activeRequesting.ProductId,
                            StudentAddress = activeRequesting.StudentAddress,
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
                    activedRequestList = _activateRequest.Find<ActivateRequest>(x => x.StudentAddress.ToLower() == studentAddress.ToLower() && x.IsActived).ToList();
                if (activedRequestList != null && activedRequestList.Count > 0)
                    foreach (var activatedRequest in activedRequestList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activatedRequest.RequestId,
                            ProductName = activatedRequest.ProductName,
                            ProductId = activatedRequest.ProductId,
                            StudentAddress = activatedRequest.StudentAddress,
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
                    RequestId = activateRequest.RequestId,
                    ProductName = activateRequest.ProductName,
                    ProductId = activateRequest.ProductId,
                    StudentAddress = activateRequest.StudentAddress,
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

        public async Task CreateNewActivateRequest(ActivateRequestDTO product)
        {
            try
            {
                await _activateRequest.InsertOneAsync(new ActivateRequest()
                {
                    RequestId = product.RequestId,
                    ProductName = product.ProductName,
                    ProductId = product.ProductId,
                    StudentAddress = product.StudentAddress,
                    ProductHahIPFS = product.ProductHahIPFS,
                    AmountToActive = product.AmountToActive,
                    ProductTypeName = product.ProductTypeName,
                    IsActived = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
