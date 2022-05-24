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
    }
}
