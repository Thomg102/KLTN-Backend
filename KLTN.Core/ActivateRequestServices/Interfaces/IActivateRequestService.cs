using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.ProductServices.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLTN.Core.RequestActivateServices.Interfaces
{
    public interface IActivateRequestService
    {
        List<ActivateRequestResponseDTO> GetListOfActivateRequesting(string studentAddress);
        List<ActivateRequestResponseDTO> GetListOfActivatedRequest(string studentAddress);
        ProductDetailResponseDTO GetDetailOfActivateRequest(long requestId);
        Task CreateNewActivateRequest(RequestActivateDTO product);
        Task CancelActivateRequest(List<long> requestIds);
        Task ActivateRequest(ActivateRequestDTO request);
        Task CreateActivateCode(ActivateCodeRequestDTO request);
    }
}
