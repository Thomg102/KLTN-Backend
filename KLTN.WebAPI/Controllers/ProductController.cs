using KLTN.Core.ProductServices.DTOs;
using KLTN.Core.ProductServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;

namespace KLTN.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet("{studentAddress}")]
        public JsonResult GetListOfStudentProduct(string studentAddress)
        {
            var result = _productService.GetListOfStudentProduct(studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("{studentAddress}/{productNftId}")]
        public JsonResult GetDetailOfStudentProduct(long productNftId, string studentAddress)
        {
            var result = _productService.GetDetailOfStudentProduct(productNftId, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("product-on-sale/{productNftId}")]
        public JsonResult GetDetailOfProductOnSale(long productNftId, [FromQuery] string walletAddress)
        {
            var result = _productService.GetDetailOfProductOnSale(productNftId, walletAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("product-on-sale")]
        public JsonResult GetListOfProductOnSale([FromQuery] string walletAddress)
        {
            var result = _productService.GetListOfProductOnSale(walletAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("product-on-sale/buyer/{productNftId}")]
        public JsonResult GetListBuyerOfProductOnSale(long productNftId)
        {
            var result = _productService.GetListBuyerOfProductOnSale(productNftId);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("type")]
        public JsonResult GetListOfAllProductType()
        {
            var result = _productService.GetListOfAllProductType();
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpPost("product-on-sale")]
        public JsonResult CreateNewProductOnSale([FromBody] ProductOnSaleDTO product)
        {
            _productService.CreateNewProductOnSale(product);
            return new JsonResult(new SuccessResponseModel());
        }
    }
}
