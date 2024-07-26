using Microsoft.EntityFrameworkCore;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.DAL.DataContext;
using SeatBooking.DAL.Models;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly BookingDatabaseContext _context;

        public UserRepository(BookingDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetByUsername(string username)
        {
            return await _context.BE_Employees.FirstOrDefaultAsync(e => e.Username == username);
        }



        public async Task UpdatePassword(string username, string newPassword)
        {
            var employee = await GetByUsername(username);
            if (employee != null)
            {
                employee.Password = newPassword;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("User not found");
            }
        }
    }
}