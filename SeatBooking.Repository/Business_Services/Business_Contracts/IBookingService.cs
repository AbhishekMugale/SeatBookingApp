using SeatBooking.DAL.Models;
using SeatBooking.Repository.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services.Business_Contracts
{
    public interface IBookingService
    {
        Task<int> CreateBookingAsync(BookingDTO bookingDto);
        Task<bool> CancelBookingAsync(int bookingId);
        Task<BookingDTO> GetActiveBookingByEmployeeAsync(int employeeId);
        Task<List<BookingDTO>> GetAllBookingsAsync();
        Task<BookingDTO> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDTO>> GetActiveBookingsBySeatAsync(int seatId);
        Task UpdateBookingStatusAsync();




    }
}
