using KLTN.Core.LecturerServices.DTOs;
using KLTN.Core.LecturerServicess.DTOs;
using KLTN.DAL.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KLTN.Core.LecturerServicess.Interfaces
{
    public interface ILecturerService
    {
        Lecturer GetDetailOfLecturer(string lecturerAddress);
        List<LecturerResponseDTO> GetAllLecturer();
        Task CreateNewLectuter(LecturerDTO lecturer);
        Task RevokeLecturerRole(List<string> lecturerAddrs);
        LecturerEventAmountResponeDTO GetLecturerEventAmount(string lecturerAddr);
    }
}
