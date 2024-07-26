using Microsoft.EntityFrameworkCore;
using SeatBooking.DAL.DALRepository.Contracts;
using SeatBooking.DAL.DataContext;
using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBooking.DAL.DALRepository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDatabaseContext _context;

        public BookingRepository(BookingDatabaseContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBookingAsync(Booking booking)
        {
            _context.BE_Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking.BookingId; // Return the ID of the newly created booking
        }


        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.BE_Bookings.FindAsync(bookingId);
            if (booking != null && booking.IsActive)
            {
                booking.IsActive = false;
                _context.BE_Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return true; // Return true if the booking was successfully canceled
            }
            return false; // Return false if the booking ID was not found or the booking is not active
        }
        


        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _context.BE_Bookings.ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.BE_Bookings.FindAsync(bookingId);
        }

        public async Task<Booking> GetActiveBookingByEmployeeAsync(int employeeId)
        {
            return await _context.BE_Bookings
                .Where(b => b.EmployeeId == employeeId && b.IsActive)
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Booking>> GetActiveBookingsBySeatAsync(int seatId)
        {
            return await _context.BE_Bookings
                .Where(b => b.SeatId == seatId && b.IsActive)
                .ToListAsync();
        }


        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
}

