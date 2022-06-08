using KLTN.Core.StudentServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.StudentServices.Interfaces
{
    public interface IStudentService
    {
        StudentDetailInfoResponseDTO GetDetailOfStudent(string studentAddress);
        List<BalanceHistoriesResponseDTO> GetBalanceHistoriesOfStudent(string studentAddress);
        List<StudentCertificateDTO> GetListSubjectCertificate(string studentAddress);
        List<StudentCertificateDTO> GetListMissionCertificate(string studentAddress);
        List<StudentResponseDTO> GetAllStudent();
        Task CreateNewStudent(StudentDTO student);
        Task UpdateStudentIntoDatabase(string studentAddress, StudentUpdateDTO studentInfo);
        Task RevokeStudentRole(List<string> studentAddrs);
    }
}
