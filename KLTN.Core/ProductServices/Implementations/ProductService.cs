using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ProductServices.DTOs;
using KLTN.Core.ProductServices.Interfaces;
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
                        ProductImg = product.ProductImg,
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
        public ProductDetailResponseDTO GetDetailOfStudentProduct(long productId, string studentAddress)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                foreach (var product in student.ProductOfStudentList)
                {
                    if (product.ProductId == productId)
                        return new ProductDetailResponseDTO()
                        {
                            ProductName = product.ProductName,
                            ProductImg = product.ProductImg,
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
        public ProductDetailResponseDTO GetDetailOfProductOnSale(long productId, string studentAddress, string adminAddress)
        {
            try
            {
                var product = new ProductOnSale();
                if (studentAddress == null)
                    product = _product.Find<ProductOnSale>(x => x.ProductId == productId && x.SaleAddress.ToLower() == studentAddress.ToLower()).FirstOrDefault();
                else
                    product = _product.Find<ProductOnSale>(x => x.ProductId == productId && x.SaleAddress.ToLower() == adminAddress.ToLower()).FirstOrDefault();
                if (product != null)
                    return new ProductDetailResponseDTO()
                    {
                        ProductName = product.ProductName,
                        ProductImg = product.ProductImg,
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
                            ProductImg = product.ProductImg,
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
                            ProductImg = product.ProductImg,
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
                var productOnSales = _product.Find<ProductOnSale>(_ => true).ToList();
                foreach(var productOnSale in productOnSales)
                    if (product.ProductId == productOnSale.ProductId)
                        throw new CustomException("Create existed Product", 100);
                await _product.InsertOneAsync(new ProductOnSale()
                {
                    ProductName = product.ProductName,
                    ProductId = product.ProductId,
                    ProductImg = product.ProductImg,
                    ProductHahIPFS = product.ProductHahIPFS,
                    AmountOnSale = product.AmountOnSale,
                    PriceOfOneItem = product.PriceOfOneItem.ToString(),
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

        public async Task ListProductOnSale(ProductStudentListOnSaleDTO product)
        {
            try
            {
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == product.SaleAddress.ToLower()).FirstOrDefault();
                var productOnsale = new ProductOnSale() { };
                var productRemaining = new long();
                var productToSale = new ProductOfStudentDTO() { };
                foreach (var productStudent in student.ProductOfStudentList)
                {
                    if (productStudent.ProductId == product.ProductId)
                    {
                        productRemaining = productStudent.Amount - product.AmountOnSale;
                        if (productRemaining < 0)
                            throw new CustomException("Not enough product to sale", 101);
                        productToSale = productStudent;
                        productOnsale = new ProductOnSale()
                        {
                            ProductName = productStudent.ProductName,
                            ProductId = productStudent.ProductId,
                            ProductHahIPFS = productStudent.ProductHahIPFS,
                            ProductTypeName = productStudent.ProductTypeName,
                            ProductDescription = productStudent.ProductDescription
                        };
                        break;
                    }
                }
                await _product.InsertOneAsync(new ProductOnSale()
                {
                    ProductName = productOnsale.ProductName,
                    ProductId = product.ProductId,
                    ProductHahIPFS = productOnsale.ProductHahIPFS,
                    AmountOnSale = product.AmountOnSale,
                    PriceOfOneItem = product.PriceOfOneItem.ToString(),
                    ProductTypeName = productOnsale.ProductTypeName,
                    ProductDescription = productOnsale.ProductDescription,
                    SaleAddress = product.SaleAddress
                });

                var filter = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.SaleAddress.ToLower());
                if (productRemaining > 0)
                {
                    var update = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductId == product.ProductId).FirstOrDefault().Amount, productRemaining);
                    await _student.UpdateOneAsync(filter, update);
                }
                else
                {
                    var update = Builders<Student>.Update.Pull(x => x.ProductOfStudentList, productToSale);
                    await _student.UpdateOneAsync(filter, update);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ListProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task DelistProductOnSale(ProductStudentDelistOnSaleDTO product)
        {
            try
            {
                var filter = Builders<ProductOnSale>.Filter.Where(x =>
                    x.SaleAddress.ToLower() == product.SaleAddress.ToLower()
                    && x.ProductId == product.ProductId
                );
                await _product.DeleteOneAsync(filter);

                var isExisted = false;
                var productOnSale = _product.Find<ProductOnSale>(x => x.ProductId == product.ProductId && x.SaleAddress.ToLower() == product.SaleAddress.ToLower()).FirstOrDefault();
                var sellingAmount = productOnSale.AmountOnSale;
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == product.SaleAddress.ToLower()).FirstOrDefault();
                foreach (var productStudent in student.ProductOfStudentList)
                {
                    if (productStudent.ProductId == product.ProductId)
                    {
                        isExisted = true;
                        var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.SaleAddress.ToLower());
                        var updateStudentAmount = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductId == product.ProductId).FirstOrDefault().Amount, productStudent.Amount + sellingAmount);
                        await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                        break;
                    }
                }
                if (!isExisted)
                {
                    var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.SaleAddress.ToLower());
                    var updateStudentAmount = Builders<Student>.Update.Push(x => x.ProductOfStudentList, new ProductOfStudentDTO()
                    {
                        ProductId = product.ProductId,
                        Amount = sellingAmount,
                        ProductDescription = productOnSale.ProductDescription,
                        ProductHahIPFS = productOnSale.ProductHahIPFS,
                        ProductImg = productOnSale.ProductImg,
                        ProductName = productOnSale.ProductName,
                        ProductTypeName = productOnSale.ProductTypeName
                    });
                    await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DelistProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task BuyProductOnSale(ProductStudentBuyOnSaleDTO product)
        {
            try
            {
                var filter = Builders<ProductOnSale>.Filter.Where(x =>
                    x.SaleAddress.ToLower() == product.SellerAddress.ToLower()
                    && x.ProductId == product.ProductId
                );
                var productOnSale = _product.Find<ProductOnSale>(x => x.ProductId == product.ProductId && x.SaleAddress.ToLower() == product.SellerAddress.ToLower()).FirstOrDefault();
                var sellingAmount = productOnSale.AmountOnSale;

                if (sellingAmount > product.BuyAmount)
                {
                    var update = Builders<ProductOnSale>.Update.Set(x => x.AmountOnSale, sellingAmount - product.BuyAmount);
                    await _product.UpdateOneAsync(filter, update);
                }
                else if (sellingAmount == product.BuyAmount)
                    await _product.DeleteOneAsync(filter);


                var isExisted = false;
                var student = _student.Find<Student>(x => x.StudentAddress.ToLower() == product.BuyerAddress.ToLower()).FirstOrDefault();
                foreach (var productStudent in student.ProductOfStudentList)
                {
                    if (productStudent.ProductId == product.ProductId)
                    {
                        isExisted = true;
                        var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.BuyerAddress.ToLower());
                        var updateStudentAmount = Builders<Student>.Update.Set(x => x.ProductOfStudentList.Where(y => y.ProductId == product.ProductId).FirstOrDefault().Amount, productStudent.Amount + product.BuyAmount);
                        await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                        break;
                    }
                }
                if (!isExisted)
                {
                    var filterStudentAmount = Builders<Student>.Filter.Where(x => x.StudentAddress.ToLower() == product.BuyerAddress.ToLower());
                    var updateStudentAmount = Builders<Student>.Update.Push(x => x.ProductOfStudentList, new ProductOfStudentDTO()
                    {
                        ProductId = product.ProductId,
                        Amount = product.BuyAmount,
                        ProductDescription = productOnSale.ProductDescription,
                        ProductHahIPFS = productOnSale.ProductHahIPFS,
                        ProductImg = productOnSale.ProductImg,
                        ProductName = productOnSale.ProductName,
                        ProductTypeName = productOnSale.ProductTypeName
                    });
                    await _student.UpdateOneAsync(filterStudentAmount, updateStudentAmount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in BuyProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateBuyPriceProductOnSale(ProductUpdateBuyPriceOnSaleDTO product)
        {
            try
            {
                var filter = Builders<ProductOnSale>.Filter.Where(x =>
                    x.SaleAddress.ToLower() == product.SaleAddress.ToLower()
                    && x.ProductId == product.ProductId
                );
                var update = Builders<ProductOnSale>.Update.Set(x => x.PriceOfOneItem, product.PriceOfOneItem.ToString());
                await _product.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateBuyPriceProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }

        public async Task UpdateAmountProductOnSale(ProductUpdateAmountOnSaleDTO product)
        {
            try
            {
                var filter = Builders<ProductOnSale>.Filter.Where(x =>
                    x.SaleAddress.ToLower() == product.SaleAddress.ToLower()
                    && x.ProductId == product.ProductId
                );
                var update = Builders<ProductOnSale>.Update.Set(x => x.AmountOnSale, product.AmountOnSale);
                await _product.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAmountProductOnSale");
                throw new CustomException(ErrorMessage.UNKNOWN, ErrorCode.UNKNOWN);
            }
        }
    }
}
