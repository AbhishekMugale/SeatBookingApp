
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.DAL.DataContext;
using SeatBooking.DAL.Models;

namespace SeatBooking.DAL.DALRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BookingDatabaseContext _context;

        public EmployeeRepository(BookingDatabaseContext context)
        {
            _context = context;
        }

        public async Task<int> CreateEmployeeAsync(Employee employee, string encryptedPassword)
        {
            // Assign the encrypted password to the employee
            employee.Password = encryptedPassword;

            // Add the employee to the database context and save changes
            _context.BE_Employees.Add(employee);
            await _context.SaveChangesAsync();

            return employee.EmployeeId; // Return the ID of the newly created employee
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.BE_Employees.ToListAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _context.BE_Employees.FindAsync(employee.EmployeeId);

            if (existingEmployee == null)
            {
                return false; // Employee not found
            }

            // Update only the properties you want to change
            existingEmployee.Username = employee.Username;
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.IsActive = employee.IsActive;

            // Save changes
            await _context.SaveChangesAsync();

            return true; // Employee updated successfully
        }
        
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.BE_Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        
    }
}








