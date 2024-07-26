using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository.Contracts
{
    public interface IBookingRepository
    {
        Task<int> CreateBookingAsync(Booking booking);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int bookingId);

        Task<Booking> GetActiveBookingByEmployeeAsync(int employeeId);

        Task<IEnumerable<Booking>> GetActiveBookingsBySeatAsync(int seatId);
        Task<bool> UpdateBookingAsync(Booking booking);
    }
}
