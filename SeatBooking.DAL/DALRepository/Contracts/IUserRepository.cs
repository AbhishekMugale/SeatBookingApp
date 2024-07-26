using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SeatBooking.DAL.DALRepository.Contracts
{
    public interface IUserRepository
    {
        Task<Employee> GetByUsername(string username);
       
        Task UpdatePassword(string username, string newPassword);

    }
}


