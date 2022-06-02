using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.ActivateRequestServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.RequestActivateServices.Interfaces
{
    public interface IActivateRequestService
    {
        List<ActivateRequestResponseDTO> GetListOfActivateRequesting(string studentAddress);
        List<ActivateRequestResponseDTO> GetListOfActivatedRequest(string studentAddress);
        ActivateRequestResponseDTO GetDetailOfActivateRequest(long requestId);
        Task CreateNewActivateRequest(RequestActivateDTO product);
        Task CancelActivateRequest(List<long> requestIds);
        Task ActivateRequest(ActivateRequestDTO request);
    }
}
