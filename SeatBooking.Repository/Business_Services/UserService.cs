
using System.Threading.Tasks;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.DAL.Models;
using SeatBooking.Repository.Business_Services.Business_Contracts;

namespace SeatBooking.Repository.Business_Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
       
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            
        }
        public UserService(IUserRepository userRepository, string jwtSecret)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthenticationResult> Authenticate(string username, string password)
        {
            var authenticationResult = new AuthenticationResult();

            var retrievedEmployee = await _userRepository.GetByUsername(username);

            if (retrievedEmployee == null)
            {
                authenticationResult.IsAuthenticated = false;
                authenticationResult.ErrorMessage = "Incorrect Username or Password";
                return authenticationResult;
            }

            if (!PasswordHelper.ComparePasswords(password, retrievedEmployee.Password))
            {
                authenticationResult.IsAuthenticated = false;
                authenticationResult.ErrorMessage = "Incorrect Username or Password";
                return authenticationResult;
            }
            if (!retrievedEmployee.IsActive)
            {
                authenticationResult.IsAuthenticated = false;
                authenticationResult.ErrorMessage = "You are not an active employee in the organization";
                return authenticationResult;
            }
            authenticationResult.IsAuthenticated = true;
            authenticationResult.Employee = retrievedEmployee;
            authenticationResult.RoleName = await GetRoleName(retrievedEmployee.RoleId);

            return authenticationResult;
        }

        public async Task<string> GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "Admin",
                2 => "Employee",
                _ => "Unknown"
            };
        }
    }
}