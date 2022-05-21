using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ProductServices.DTOs;
using KLTN.DAL.Models.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPI.Utils.Constants;

namespace KLTN.Core.ProductServices.Implementations
{
    public class ProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMongoCollection<ProductOnSale> _product;
        private readonly IMongoCollection<Student> _student;
        private string _adminAddress;

        private readonly WebAPIAppSettings _settings;

        public ProductService(ILogger<ProductService> logger, IOptions<WebAPIAppSettings> settings)
        {
            var client = new MongoClient(_settings.ConnectionString);
            var database = client.GetDatabase(_settings.DatabaseName);

            _logger = logger;
            _settings = settings.Value;

            _product = database.GetCollection<ProductOnSale>(_settings.ProductCollectionName);
            _student = database.GetCollection<Student>(_settings.StudentCollectionName);
            _adminAddress = _settings.AdminAddress;
        }

        // Get list of Student product 
        public List<ProductDetailResponseDTO> GetListOfStudentProduct(string studentAddress)
        {
            try
            {
                var result = new List<ProductDetailResponseDTO>();
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                foreach (var product in student.ProductOfStudentList)
                    result.Add(new ProductDetailResponseDTO()
                    {
                        ProductName = product.ProductName,
                        ProductId = product.ProductId,
                        ProductDescription = product.ProductDescription,
                        ProductHahIPFS = product.ProductHahIPFS,
                        Amount = product.Amount,
                        ProductTypeName = product.ProductTypeName,
                    });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfStudentProduct");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get detail of specific Student product 
        public ProductDetailResponseDTO GetDetailOfStudentProduct(string productId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                foreach (var product in student.ProductOfStudentList)
                {
                    if (product.ProductId.ToLower() == productId.ToLower())
                        return new ProductDetailResponseDTO()
                        {
                            ProductName = product.ProductName,
                            ProductId = product.ProductId,
                            ProductDescription = product.ProductDescription,
                            ProductHahIPFS = product.ProductHahIPFS,
                            Amount = product.Amount,
                            ProductTypeName = product.ProductTypeName,
                        };

                }
                return new ProductDetailResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfStudentProduct");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get detail of specific product on sale Student/Admin
        public ProductDetailResponseDTO GetDetailOfProductOnSale(string productId, string studentAddress)
        {
            try
            {
                var product = new ProductOnSale();
                if (studentAddress == null)
                    product = _product.Find<ProductOnSale>(x => x.ProductId.ToLower() == productId.ToLower() && x.SaleAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                else
                    product = _product.Find<ProductOnSale>(x => x.ProductId.ToLower() == productId.ToLower() && x.SaleAddress.ToLower() == _adminAddress.ToLower()).FirstOrDefault();
                if (product != null)
                    return new ProductDetailResponseDTO()
                    {
                        ProductName = product.ProductName,
                        ProductId = product.ProductId,
                        ProductDescription = product.ProductDescription,
                        ProductHahIPFS = product.ProductHahIPFS,
                        Amount = product.AmountOnSale,
                        ProductTypeName = product.ProductTypeName,
                    };
                return new ProductDetailResponseDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDetailOfStudentProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get list of product On sale Student/Admin
        public List<ProductDetailResponseDTO> GetListOfProductOnSale(string studentAddress)
        {
            try
            {
                var result = new List<ProductDetailResponseDTO>();
                var productList = new List<ProductOnSale>();
                if (studentAddress == null)
                    productList = _product.Find<ProductOnSale>(x => x.SaleAddress.ToLower() == _adminAddress.ToLower()).ToList();
                else
                    productList = _product.Find<ProductOnSale>(x => x.SaleAddress.ToLower() == studentAddress.ToLower()).ToList();
                if (productList != null && productList.Count > 0)
                    foreach (var product in productList)
                        result.Add(new ProductDetailResponseDTO()
                        {
                            ProductName = product.ProductName,
                            ProductId = product.ProductId,
                            ProductDescription = product.ProductDescription,
                            ProductHahIPFS = product.ProductHahIPFS,
                            Amount = product.AmountOnSale,
                            ProductTypeName = product.ProductTypeName,
                        });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        // Get lits of all product On sale
        public List<ProductDetailResponseDTO> GetListOfAllProductOnSale()
        {
            try
            {
                var result = new List<ProductDetailResponseDTO>();
                var productList = _product.Find<ProductOnSale>(_ => true).ToList();
                if (productList != null && productList.Count > 0)
                    foreach (var product in productList)
                    result.Add(new ProductDetailResponseDTO()
                    {
                        ProductName = product.ProductName,
                        ProductId = product.ProductId,
                        ProductDescription = product.ProductDescription,
                        ProductHahIPFS = product.ProductHahIPFS,
                        Amount = product.AmountOnSale,
                        ProductTypeName = product.ProductTypeName,
                    });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfAllProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
