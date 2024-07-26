using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository.Contracts
{
    public interface ISeatRepository
    {
        
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync();
        Task<IEnumerable<Seat>> GetAllSeatsAsync();
        Task<Seat> GetSeatByIdAsync(int seatId);
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(DateTime startDate, DateTime endDate);
    }
}
