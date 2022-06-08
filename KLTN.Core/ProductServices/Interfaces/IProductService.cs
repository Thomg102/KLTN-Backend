using KLTN.Core.ProductServices.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLTN.Core.ProductServices.Interfaces
{
    public interface IProductService
    {
        List<ProductDetailResponseDTO> GetListOfStudentProduct(string studentAddress);
        ProductDetailResponseDTO GetDetailOfStudentProduct(long productId, string studentAddress);
        ProductDetailResponseDTO GetDetailOfProductOnSale(long productId, string walletAddress);
        List<ProductOnSaleResponseDTO> GetListOfProductOnSale(string walletAddress);
        List<ProductTypeResponseDTO> GetListOfAllProductType();
        Task CreateNewProductOnSale(ProductOnSaleDTO product);
        Task ListProductOnSale(ProductStudentListOnSaleDTO product);
        Task DelistProductOnSale(ProductStudentDelistOnSaleDTO product);
        Task BuyProductOnSale(ProductStudentBuyOnSaleDTO product);
        Task UpdateBuyPriceProductOnSale(ProductUpdateBuyPriceOnSaleDTO product);
        Task UpdateAmountProductOnSale(ProductUpdateAmountOnSaleDTO product);
        List<BuyerOfProductOnSaleResponseDTO> GetListBuyerOfProductOnSale(long productNftId);
    }
}
