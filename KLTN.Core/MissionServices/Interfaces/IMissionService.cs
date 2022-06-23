using KLTN.Core.MissionServices.DTOs;
using System.Collections.Generic;
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
        Task UpdateLecturerConfirmComplete(string missionAddress, int chainNetworkId, List<string> studentList);
        Task UpdateLecturerUnConfirmComplete(string missionAddress, int chainNetworkId, List<string> studentAddress);
        Task CloseMission(string missionAddress, int chainNetworkId);
        Task LockMission(List<string> missionAddrs);
        Task<List<string>> GetMissionListReadyToClose(int chainNetworkId);
    }
}
