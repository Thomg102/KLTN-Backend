using KLTN.Common.Exceptions;
using KLTN.Common.Models;
using KLTN.Core.ProductServices.DTOs;
using KLTN.Core.ProductServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Utils.Constants;

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

        [HttpGet("{studentAddress}/{productId}")]
        public JsonResult GetDetailOfStudentProduct(string productId, string studentAddress)
        {
            var result = _productService.GetDetailOfStudentProduct(productId, studentAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("product-on-sale/{productId}")]
        public JsonResult GetDetailOfProductOnSale(string productId, [FromQuery] string studentAddress)
        {
            var result = _productService.GetDetailOfProductOnSale(productId, studentAddress, WebAPIAppSettings.Value.AdminAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("product-on-sale")]
        public JsonResult GetListOfProductOnSale([FromQuery] string studentAddress)
        {
            var result = _productService.GetListOfProductOnSale(studentAddress, WebAPIAppSettings.Value.AdminAddress);
            return new JsonResult(new SuccessResponseModel(result));
        }

        [HttpGet("")]
        public JsonResult GetListOfAllProductOnSale()
        {
            var result = _productService.GetListOfAllProductOnSale();
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
