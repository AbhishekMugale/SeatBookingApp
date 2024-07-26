using Microsoft.EntityFrameworkCore;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.DAL.DataContext;
using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository
{
    public class SeatRepository : ISeatRepository
    {
        private readonly BookingDatabaseContext _context;

        public SeatRepository(BookingDatabaseContext context)
        {
            _context = context;
        }

      
        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync()
        {
            return await _context.BD_Seats
                .Include(s => s.Bookings)
                .Where(s => s.IsActive)
                .ToListAsync();
        }

        public async Task<Seat> GetSeatByIdAsync(int seatId)
        {
            return await _context.BD_Seats.FindAsync(seatId);
        }
        public async Task<IEnumerable<Seat>> GetAllSeatsAsync()
        {
            return await _context.BD_Seats.ToListAsync();
        }
        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.BD_Seats
                .Include(s => s.Bookings)
                .Where(s => s.IsActive && !s.Bookings.Any(b => b.IsActive &&
                                                               (startDate < b.BookingDate.AddDays(b.BookingDuration) && endDate > b.BookingDate)))
                .ToListAsync();
        }
    }
}
