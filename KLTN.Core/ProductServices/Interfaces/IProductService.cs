using KLTN.Core.ProductServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.ProductServices.Interfaces
{
    public interface IProductService
    {
        List<ProductDetailResponseDTO> GetListOfStudentProduct(string studentAddress);
        ProductDetailResponseDTO GetDetailOfStudentProduct(string productId, string studentAddress);
        ProductDetailResponseDTO GetDetailOfProductOnSale(string productId, string studentAddress);
        List<ProductDetailResponseDTO> GetListOfProductOnSale(string studentAddress);
        List<ProductDetailResponseDTO> GetListOfAllProductOnSale();
        List<ProductTypeResponseDTO> GetListOfAllProductType();
        Task CreateNewProductOnSale(ProductOnSaleDTO product);
    }
}
