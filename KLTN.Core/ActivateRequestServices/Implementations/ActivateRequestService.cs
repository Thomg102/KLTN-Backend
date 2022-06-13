using KLTN.Common.Enums;
using KLTN.Common.Exceptions;
using KLTN.Common.Models.AppSettingModels;
using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.ProductServices.DTOs;
using KLTN.Core.RequestActivateServices.Interfaces;
using KLTN.DAL;
using KLTN.DAL.Models.DTOs;
using KLTN.DAL.Models.Entities;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMongoCollection<ProductType> _productType;
        private readonly IMongoCollection<CodeActivateProduct> _codeActivate;
        private readonly MailSettings mailSettings;

        public ActivateRequestService(ILogger<ActivateRequestService> logger, IMongoDbContext context, IOptions<MailSettings> settings)
        {
            _logger = logger;
            _context = context;
            _activateRequest = _context.GetCollection<ActivateRequest>(typeof(ActivateRequest).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
            _productType = _context.GetCollection<ProductType>(typeof(ProductType).Name);
            _codeActivate = _context.GetCollection<CodeActivateProduct>(typeof(CodeActivateProduct).Name);
            mailSettings = settings.Value;
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
                            ActivatedTime = activateRequesting.ActivatedTime,
                            ProductNftId = activateRequesting.ProductNftId,
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
        public ProductDetailResponseDTO GetDetailOfActivateRequest(long requestId)
        {
            try
            {
                var activateRequest = _activateRequest.Find<ActivateRequest>(x => x.RequestId == requestId).FirstOrDefault();
                return new ProductDetailResponseDTO()
                {
                    ProductName = activateRequest.ProductName,
                    ProductImg = activateRequest.ProductImg,
                    ProductId = activateRequest.ProductId,
                    ProductNftId = activateRequest.ProductNftId,
                    ProductDescription = activateRequest.ProductDescription,
                    ProductHahIPFS = activateRequest.ProductHahIPFS,
                    Amount = activateRequest.AmountToActivate,
                    ProductTypeName = activateRequest.ProductTypeName,
                    OwnerAddress = activateRequest.StudentAddress,
                    IsActivated = activateRequest.IsActivated,
                    ActivatedTime = activateRequest.ActivatedTime,
                    RequestedTime = activateRequest.RequestedTime,
                    RequestId = activateRequest.RequestId,
                    Status = activateRequest.IsActivated ? ProductStatus.Activated.ToString() : ProductStatus.Request.ToString()
                };
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
                    if (productStudent.ProductNftId == product.ProductNftId)
                    {
                        productRemaining = productStudent.Amount - product.AmountToActivate;
                        if (productRemaining < 0)
                            throw new CustomException("Not enough product to request activate", 200);
                        productToActivate = productStudent;
                        productActivate = new ActivateRequest()
                        {
                            ProductName = productStudent.ProductName,
                            ProductNftId = productStudent.ProductNftId,
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
                    ProductId = productActivate.ProductId,
                    ProductNftId = product.ProductNftId,
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
                    var update = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductNftId == product.ProductNftId).FirstOrDefault().Amount, productRemaining);
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

                var productTypeName = _activateRequest.Find<ActivateRequest>(x => x.RequestId == request.RequestId).FirstOrDefault().ProductTypeName;
                var isIdependentNFT = _productType.Find<ProductType>(x => x.ProductTypeAlias.ToLower() == productTypeName.ToLower()).FirstOrDefault().IsIdependentNFT;
                if (isIdependentNFT)
                {
                    var codeActivate = _codeActivate.Find<CodeActivateProduct>(x => x.ProductTypeName.ToLower() == productTypeName.ToLower() && x.IsUsed == false).FirstOrDefault().Code;
                    var studentAddress = _activateRequest.Find<ActivateRequest>(x => x.RequestId == request.RequestId).FirstOrDefault().StudentAddress;
                    var studentId = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault().StudentId;
                    var nameProduct = _activateRequest.Find<ActivateRequest>(x => x.RequestId == request.RequestId).FirstOrDefault().ProductName;

                    var mailContent = new MailContent(){
                        To = studentId + mailSettings.Domain.ToLower(),
                        Subject = "MA KICH HOAT VAT PHAM " + nameProduct.ToUpper(),
                        Body = "Ma kich hoat vat pham " + nameProduct + ": " + codeActivate + ". Vui long khong chia se ma kich hoat nay cho ai khac."
                    };
                    await SendMail(mailContent);

                    var filterUpdateCodeUsed = Builders<CodeActivateProduct>.Filter.Where(x => x.Code == codeActivate);
                    var updateUpdateCodeUsed = Builders<CodeActivateProduct>.Update.Set(x => x.IsUsed, true);
                    await _codeActivate.UpdateOneAsync(filterUpdateCodeUsed, updateUpdateCodeUsed);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ActivateRequest");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task SendMail(MailContent mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(mailContent.To));
            email.Subject = mailContent.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);

                _logger.LogInformation("Error send mail, stored at - " + emailsavefile);
                _logger.LogError(ex.Message);
            }

            smtp.Disconnect(true);

            _logger.LogInformation("send mail to " + mailContent.To);
        }
    }
}
