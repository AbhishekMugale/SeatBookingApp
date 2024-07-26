using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository.Contracts
{
    public interface IEmployeeRepository
    {
        Task<int> CreateEmployeeAsync(Employee employee, string encryptedPassword);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<Employee> GetEmployeeByIdAsync(int employeeId);

       
    }
}
