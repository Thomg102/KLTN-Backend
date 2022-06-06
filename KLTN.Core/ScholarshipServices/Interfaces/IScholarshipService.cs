using KLTN.Core.ScholarshipServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
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
        Task AddStudentToScholarship(string scholarshipAddress, int chainNetworkId, List<string> studentAddressList);
        Task RemoveStudentFromScholarship(string scholarshipAddress, int chainNetworkId, List<string> studentAddress);
        Task CloseScholarship(string scholarshipAddress, int chainNetworkId);
        Task LockScholarship(List<string> scholarshipAddrs);
    }
}
