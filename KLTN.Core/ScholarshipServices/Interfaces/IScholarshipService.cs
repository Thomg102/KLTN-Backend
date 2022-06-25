using KLTN.Core.ScholarshipServices.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLTN.Core.ScholarshipServices.Interfaces
{
    public interface IScholarshipService
    {
        ScholarshipDetailResponseDTO GetDetailOfScholarship(string scholarshipAddress, string studentAddress);
        List<StudentScholarshipResponseDTO> GetAllScholarship(string studentAddress);
        List<LecturerScholarshipResponseDTO> GetAllScholarshipOfLecturer(string lecturerAddress);
        Task CreateNewScholarship(ScholarshipDTO scholarship);
        Task<List<string>> GetScholarshipListInProgress(int chainNetworkId);
        Task UpdateStudentRegister(string scholarshipAddress, int chainNetworkId, string studentAddress);
        Task UpdateStudentCancelRegister(string scholarshipAddress, int chainNetworkId, string studentAddress);
        Task UpdateLecturerConfirmComplete(string scholarshipAddress, int chainNetworkId, List<string> studentAddressList);
        Task UpdateLecturerUnConfirmComplete(string scholarshipAddress, int chainNetworkId, List<string> studentAddressList);

        Task CloseScholarship(string scholarshipAddress, int chainNetworkId);
        Task LockScholarship(List<string> scholarshipAddrs);
        Task<List<string>> GetScholarshipListReadyToClose(int chainNetworkId);
    }
}
