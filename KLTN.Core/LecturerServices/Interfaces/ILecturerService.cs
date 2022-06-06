using KLTN.Core.LecturerServices.DTOs;
using KLTN.Core.LecturerServicess.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.LecturerServicess.Interfaces
{
    public interface ILecturerService
    {
        LecturerDetailInfoResponseDTO GetDetailOfLecturer(string lecturerAddress);
        List<LecturerResponseDTO> GetAllLecturer();
        Task CreateNewLectuter(LecturerDTO lecturer);
        Task RevokeLecturerRole(List<string> lecturerAddrs);
    }
}
