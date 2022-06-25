using KLTN.Core.SubjectServices.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLTN.Core.SubjectServices.Interfaces
{
    public interface ISubjectService
    {
        SubjectDetailResponseDTO GetDetailOfSubject(string subjectAddress, string studentAddress);
        List<StudentSubjectResponseDTO> GetAllSubject(string studentAddress);
        List<LecturerSubjectResponseDTO> GetAllSubjectOfLecturer(string lecturerAddress);
        Task CreateNewSubject(SubjectDTO subject);
        Task<List<string>> GetSubjectListInProgress(int chainNetworkId);
        Task UpdateStudentRegister(string subjectAddress, int chainNetworkId, string studentAddress);
        Task UpdateStudentCancelRegister(string subjectAddress, int chainNetworkId, string studentAddress);
        Task UpdateLecturerConfirmComplete(string subjectAddress, int chainNetworkId, List<string> studentList);
        Task UpdateLecturerUnConfirmComplete(string subjectAddress, int chainNetworkId, List<string> studentAddress);
        Task CloseSubject(string subjectAddress, int chainNetworkId);
        Task LockSubject(List<string> subjectAddrs);
        Task<List<string>> GetSubjectListReadyToClose(int chainNetworkId);
    }
}
