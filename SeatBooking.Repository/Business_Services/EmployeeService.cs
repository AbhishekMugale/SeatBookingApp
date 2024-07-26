using AutoMapper;
using SeatBooking.DAL.Models;
using SeatBooking.DAL.DALRepository.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using SeatBooking.Repository.DTOs;

namespace SeatBooking.Repository.Business_Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateEmployeeAsync(EmployeeDTO employeeDto, string plainTextPassword)
        {
            string encryptedPassword = PasswordHelper.HashPassword(plainTextPassword);
            var employee = _mapper.Map<Employee>(employeeDto);
            return await _employeeRepository.CreateEmployeeAsync(employee, encryptedPassword);
        }

        public async Task<int> GetEmployeeIdByUsernameAsync(string username)
        {
            var employee = await _userRepository.GetByUsername(username);
            if (employee != null)
            {
                return employee.EmployeeId;
            }
            else
            {
                throw new Exception("Employee not found");
            }
        }

        public async Task<string> GetEmployeeFirstNamebyUserNameAsync(string username) 
        {
            var employee = await _userRepository.GetByUsername(username);
            if (employee != null)
            {
                return employee.FirstName;
            }
            else
            {
                throw new Exception("Employee not found");
            }

        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return _mapper.Map<List<EmployeeDTO>>(employees);
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeDTO employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
            return _mapper.Map<EmployeeDTO>(employee);
        }
    }
}


//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using SeatBooking.DAL.DALRepository;
//using SeatBooking.DAL.DALRepository.Contracts;
//using SeatBooking.DAL.Models;
//using SeatBooking.Repository.Business_Services.Business_Contracts;

//namespace SeatBooking.Repository.Business_Services
//{
//    public class EmployeeService : IEmployeeService
//    {

//        private readonly IEmployeeRepository _employeeRepository;
//        private readonly IUserRepository _userRepository;

//        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository)
//        {
//            _employeeRepository = employeeRepository;
//            _userRepository = userRepository;
//        }

//        public async Task<int> CreateEmployeeAsync(Employee employee, string plainTextPassword)
//        {
//            // Encrypt the password before storing it
//            string encryptedPassword = PasswordHelper.HashPassword(plainTextPassword);
//            return await _employeeRepository.CreateEmployeeAsync(employee, encryptedPassword);
//        }
//        public async Task<int> GetEmployeeIdByUsernameAsync(string username)
//        {
//            var employee = await _userRepository.GetByUsername(username);

//            if (employee != null)
//            {
//                return employee.EmployeeId;
//            }
//            else
//            {
//                throw new Exception("Employee not found");
//            }
//        }
//        public async Task<List<Employee>> GetAllEmployeesAsync()
//        {
//            return await _employeeRepository.GetAllEmployeesAsync();
//        }


//        public async Task<bool> UpdateEmployeeAsync(Employee employee)
//        {

//            return await _employeeRepository.UpdateEmployeeAsync(employee);
//        }
//        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
//        {
//            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
//        }

//    }
//}