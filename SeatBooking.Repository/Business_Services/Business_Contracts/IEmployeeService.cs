using SeatBooking.DAL.Models;
using SeatBooking.Repository.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services.Business_Contracts
{
    public interface IEmployeeService
    {
        Task<int> CreateEmployeeAsync(EmployeeDTO employeeDto, string plainTextPassword);
        Task<int> GetEmployeeIdByUsernameAsync(string username);
        Task<List<EmployeeDTO>> GetAllEmployeesAsync();
        Task<bool> UpdateEmployeeAsync(EmployeeDTO employeeDto);
        Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId);
        Task<string> GetEmployeeFirstNamebyUserNameAsync(string username);

    }

}
