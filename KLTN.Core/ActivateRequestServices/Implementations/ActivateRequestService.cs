using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.RequestActivateServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Utils.Constants;

namespace KLTN.Core.RequestActivateServices.Implementations
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
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => x.IsActivated == false).ToList();
                else
                    activateRequestingList = _activateRequest.Find<ActivateRequest>(x => x.StudentAddress.ToLower() == studentAddress.ToLower() && x.IsActivated == false).ToList();
                if (activateRequestingList != null && activateRequestingList.Count > 0)
                    foreach (var activateRequesting in activateRequestingList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activateRequesting.RequestId,
                            ProductName = activateRequesting.ProductName,
                            ProductId = activateRequesting.ProductId,
                            StudentAddress = activateRequesting.StudentAddress,
                            ProductHahIPFS = activateRequesting.ProductHahIPFS,
                            AmountToActivate = activateRequesting.AmountToActivate,
                            ProductTypeName = activateRequesting.ProductTypeName,
                            RequestedTime = activateRequesting.RequestedTime,
                            IsActivated = activateRequesting.IsActivated,
                            ProductImg = activateRequesting.ProductImg,
                            ProductDescription = activateRequesting.ProductDescription,
                            ActivatedTime = activateRequesting.ActivatedTime
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
                var activatedRequestList = new List<ActivateRequest>();
                if (studentAddress == null)
                    activatedRequestList = _activateRequest.Find<ActivateRequest>(x => x.IsActivated).ToList();
                else
                    activatedRequestList = _activateRequest.Find<ActivateRequest>(x => x.StudentAddress.ToLower() == studentAddress.ToLower() && x.IsActivated).ToList();
                if (activatedRequestList != null && activatedRequestList.Count > 0)
                    foreach (var activatedRequest in activatedRequestList)
                        result.Add(new ActivateRequestResponseDTO()
                        {
                            RequestId = activatedRequest.RequestId,
                            ProductName = activatedRequest.ProductName,
                            ProductId = activatedRequest.ProductId,
                            StudentAddress = activatedRequest.StudentAddress,
                            ProductHahIPFS = activatedRequest.ProductHahIPFS,
                            AmountToActivate = activatedRequest.AmountToActivate,
                            ProductTypeName = activatedRequest.ProductTypeName,
                            RequestedTime = activatedRequest.RequestedTime,
                            IsActivated = activatedRequest.IsActivated,
                            ProductImg = activatedRequest.ProductImg,
                            ProductDescription = activatedRequest.ProductDescription,
                            ActivatedTime = activatedRequest.ActivatedTime
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
        public ActivateRequestResponseDTO GetDetailOfActivateRequest(long requestId)
        {
            try
            {
                var activateRequest = _activateRequest.Find<ActivateRequest>(x => x.RequestId == requestId).FirstOrDefault();
                return (new ActivateRequestResponseDTO()
                {
                    RequestId = activateRequest.RequestId,
                    ProductName = activateRequest.ProductName,
                    ProductId = activateRequest.ProductId,
                    StudentAddress = activateRequest.StudentAddress,
                    ProductHahIPFS = activateRequest.ProductHahIPFS,
                    AmountToActivate = activateRequest.AmountToActivate,
                    RequestedTime= activateRequest.RequestedTime,
                    IsActivated = activateRequest.IsActivated,
                    ProductTypeName = activateRequest.ProductTypeName,
                    ProductImg = activateRequest.ProductImg,
                    ProductDescription = activateRequest.ProductDescription,
                    ActivatedTime = activateRequest.ActivatedTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CreateNewActivateRequest(RequestActivateDTO product)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == product.StudentAddress.ToLower()).FirstOrDefault();
                var productActivate = new ActivateRequest() { };
                var productRemaining = new long();
                var productToActivate = new ProductOfStudentDTO() { };
                foreach (var productStudent in student.ProductOfStudentList)
                {
                    if (productStudent.ProductId == product.ProductId)
                    {
                        productRemaining = productStudent.Amount - product.AmountToActivate;
                        if (productRemaining < 0)
                            throw new CustomException("Not enough product to request activate", 200);
                        productToActivate = productStudent;
                        productActivate = new ActivateRequest()
                        {
                            ProductName = productStudent.ProductName,
                            ProductId = productStudent.ProductId,
                            ProductHahIPFS = productStudent.ProductHahIPFS,
                            ProductTypeName = productStudent.ProductTypeName,
                            ProductImg = productStudent.ProductImg,
                            ProductDescription = productStudent.ProductDescription
                        };
                        break;
                    }
                }
                await _activateRequest.InsertOneAsync(new ActivateRequest()
                {
                    RequestId = product.RequestId,
                    ProductName = productActivate.ProductName,
                    ProductId = product.ProductId,
                    StudentAddress = product.StudentAddress,
                    ProductHahIPFS = productActivate.ProductHahIPFS,
                    AmountToActivate = product.AmountToActivate,
                    ProductTypeName = productActivate.ProductTypeName,
                    RequestedTime = product.RequestedTime,
                    ProductImg = productActivate.ProductImg,
                    ProductDescription = productActivate.ProductDescription,
                    ActivatedTime = 0,
                    IsActivated = false
                });

                var filter = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.StudentAddress.ToLower());
                if (productRemaining > 0)
                {
                    var update = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductId == product.ProductId).FirstOrDefault().Amount, productRemaining);
                    await _student.UpdateOneAsync(filter, update);
                }
                else
                {
                    var update = Builders<Student>.Update.Pull(x => x.ProductOfStudentList, productToActivate);
                    await _student.UpdateOneAsync(filter, update);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task CancelActivateRequest(List<long> requestIds)
        {
            try
            {
                foreach (var requestId in requestIds)
                {
                    var isExisted = false;
                    var productActivating = _activateRequest.Find<ActivateRequest>(x => x.RequestId == requestId).FirstOrDefault();
                    var activatingAmount = productActivating.AmountToActivate;
                    var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == productActivating.StudentAddress.ToLower()).FirstOrDefault();
                    foreach (var productStudent in student.ProductOfStudentList)
                    {
                        if (productStudent.ProductId == productActivating.ProductId)
                        {
                            isExisted = true;
                            var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == productActivating.StudentAddress.ToLower());
                            var updateStudentAmount = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductId == productActivating.ProductId).FirstOrDefault().Amount, productStudent.Amount + activatingAmount);
                            await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                            break;
                        }
                    }
                    if (!isExisted)
                    {
                        var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == productActivating.StudentAddress.ToLower());
                        var updateStudentAmount = Builders<Student>.Update.Push(x => x.ProductOfStudentList, new ProductOfStudentDTO()
                        {
                            ProductId = productActivating.ProductId,
                            Amount = activatingAmount,
                            ProductDescription = productActivating.ProductDescription,
                            ProductHahIPFS = productActivating.ProductHahIPFS,
                            ProductImg = productActivating.ProductImg,
                            ProductName = productActivating.ProductName,
                            ProductTypeName = productActivating.ProductTypeName
                        });
                        await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                    }
                } 

                var filter = Builders<ActivateRequest>
                  .Filter
                  .In(x => x.RequestId, requestIds);
                await _activateRequest.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CancelActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task ActivateRequest(ActivateRequestDTO request)
        {
            try
            {
                var filter = Builders<ActivateRequest>.Filter.Where(x => x.RequestId == request.RequestId);
                var updateStatus = Builders<ActivateRequest>.Update.Set(x => x.IsActivated, true);
                var updateActivateTime = Builders<ActivateRequest>.Update.Set(x => x.ActivatedTime, request.ActivatedTime);

                await _activateRequest.UpdateOneAsync(filter, updateStatus);
                await _activateRequest.UpdateOneAsync(filter, updateActivateTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
