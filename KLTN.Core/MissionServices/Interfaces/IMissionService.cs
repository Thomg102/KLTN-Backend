using KLTN.Core.MissionServices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.MissionServices.Interfaces
{
    public interface IMissionService
    {
        MissionDetailResponseDTO GetDetailOfMission(string missionAddress, string studentAddress);
        List<StudentMissionResponseDTO> GetAllMission(string studentAddress);
        List<LecturerMissionResponseDTO> GetAllMissionOfLecturer(string lecturerAddress);
        List<MissionTypeResponseDTO> GetListOfAllMissionType();
        Task CreateNewMission(MissionDTO mission);
        Task<List<string>> GetMissionListInProgress(int chainNetworkId);
        Task UpdateStudentRegister(string missionAddress, int chainNetworkId, string studentAddress);
        Task UpdateStudentCancelRegister(string missionAddress, int chainNetworkId, string studentAddress);
        Task UpdateLecturerConfirmComplete(List<string> studentList);
        Task UpdateLecturerUnConfirmComplete(string studentAddress);
        Task CloseMission();
    }
}
