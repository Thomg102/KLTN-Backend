using KLTN.Core.TuitionServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.TuitionServices.Interfaces
{
    public interface ITuitionService
    {
        TuitionDetailResponseDTO GetDetailOfTuition(string tuitionAddress, string studentAddress);
        List<StudentTuitionResponseDTO> GetAllTuition(string studentAddress);
        Task CreateNewTuition(TuitionDTO tuition);
        Task<List<string>> GetTuitionListInProgress(int chainNetworkId);
        Task AddStudentToTuition(string tuitionpAddress, int chainNetworkId, List<string> studentAddressList);
        Task RemoveStudentFromTuition(string tuitionAddress, int chainNetworkId, List<string> studentAddress);
        Task UpdateStudentCompeletedPayment(string tuitionAddress, int chainNetworkId, string studentAddress);
        Task CloseTuition(string tuitionAddress, int chainNetworkId);
        Task LockTuition(List<string> tuitionAddrs);
    }
}
