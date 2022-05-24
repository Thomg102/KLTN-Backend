using KLTN.Core.ActivateRequestServices.DTOs;
using KLTN.Core.ActiveRequestServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.RequestActiveServices.Interfaces
{
    public interface IActivateRequestService
    {
        List<ActivateRequestResponseDTO> GetListOfActivateRequesting(string studentAddress);
        List<ActivateRequestResponseDTO> GetListOfActivatedRequest(string studentAddress);
        ActivateRequestResponseDTO GetDetailOfActivateRequest(string requestId);
        Task CreateNewActivateRequest(ActivateRequestDTO product);
    }
}
