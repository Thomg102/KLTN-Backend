using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ProductServices.DTOs;
using KLTN.Core.ProductServices.Interfaces;
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

namespace KLTN.Core.ProductServices.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<ProductOnSale> _product;
        private readonly IMongoCollection<ProductType> _productType;
        private readonly IMongoCollection<Student> _student;

        public ProductService(ILogger<ProductService> logger, IMongoDbContext context)
        {
            _logger = logger;
            _context = context;

            _product = _context.GetCollection<ProductOnSale>(typeof(ProductOnSale).Name);
            _student = _context.GetCollection<Student>(typeof(Student).Name);
            _productType = _context.GetCollection<ProductType>(typeof(ProductType).Name);
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
        public ProductDetailResponseDTO GetDetailOfProductOnSale(string productId, string studentAddress, string adminAddress)
        {
            try
            {
                var product = new ProductOnSale();
                if (studentAddress == null)
                    product = _product.Find<ProductOnSale>(x => x.ProductId.ToLower() == productId.ToLower() && x.SaleAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                else
                    product = _product.Find<ProductOnSale>(x => x.ProductId.ToLower() == productId.ToLower() && x.SaleAddress.ToLower() == adminAddress.ToLower()).FirstOrDefault();
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
        public List<ProductDetailResponseDTO> GetListOfProductOnSale(string studentAddress, string adminAddress)
        {
            try
            {
                var result = new List<ProductDetailResponseDTO>();
                var productList = new List<ProductOnSale>();
                if (studentAddress == null)
                    productList = _product.Find<ProductOnSale>(x => x.SaleAddress.ToLower() == adminAddress.ToLower()).ToList();
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

        public List<ProductTypeResponseDTO> GetListOfAllProductType()
        {
            try
            {
                var result = new List<ProductTypeResponseDTO>();
                var productTypeList = _productType.Find<ProductType>(_ => true).ToList();
                if (productTypeList != null && productTypeList.Count > 0)
                    foreach (var productType in productTypeList)
                        result.Add(new ProductTypeResponseDTO()
                        {
                            ProductTypeName = productType.ProductTypeName,
                            IsIdependentNFT = productType.IsIdependentNFT
                        });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetListOfAllProductType");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
        public async Task CreateNewProductOnSale(ProductOnSaleDTO product)
        {
            try
            {
                await _product.InsertOneAsync(new ProductOnSale()
                {
                    ProductName = product.ProductName,
                    ProductId = product.ProductId,
                    ProductHahIPFS = product.ProductHahIPFS,
                    AmountOnSale = product.AmountOnSale,
                    PriceOfOneItem = product.PriceOfOneItem,
                    ProductTypeName = product.ProductTypeName,
                    ProductDescription = product.ProductDescription,
                    SaleAddress = product.SaleAddress
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateNewProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
