using KLTN.Core.SubjectServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.SubjectServices.Interfaces
{
    public interface ISubjectService
    {
        SubjectDetailResponseDTO GetDetailOfSubject(string subjectAddress, string studentAddress);
        List<StudentSubjectResponseDTO> GetAllSubject(string studentAddress);
        List<LecturerSubjectResponseDTO> GetAllSubjectOfLecturer(string lecturerAddress);
        Task CreateNewSubject(SubjectDTO subject);
    }
}
