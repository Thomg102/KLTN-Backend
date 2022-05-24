using KLTN.Core.DepartmentServices.DTOs;
using KLTN.DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KLTN.Core.DepartmentServices.Interfaces
{
    public interface IDepartmentService
    {
        List<SubjectType> GetListSubjectOfDepartment(string departmentShortenName);
        List<DepartmentResponeDTO> GetAllDepartment();
        Task CreateNewDepartment(DepartmentDTO department);
        Task CreateNewSubjectInDepartment(string departmentShortenName, SubjectType subjectType);
    }
}
