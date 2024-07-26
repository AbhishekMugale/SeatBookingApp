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
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateBookingAsync(BookingDTO bookingDto)
        {
            var booking = _mapper.Map<Booking>(bookingDto);
            var existingBooking = await _bookingRepository.GetActiveBookingByEmployeeAsync(booking.EmployeeId);
            if (existingBooking != null)
            {
                throw new Exception("Employee already has an active booking.");
            }

            return await _bookingRepository.CreateBookingAsync(booking);
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            return await _bookingRepository.CancelBookingAsync(bookingId);
        }

        public async Task<BookingDTO> GetActiveBookingByEmployeeAsync(int employeeId)
        {
            var booking = await _bookingRepository.GetActiveBookingByEmployeeAsync(employeeId);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<List<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<IEnumerable<BookingDTO>> GetActiveBookingsBySeatAsync(int seatId)
        {
            var bookings = await _bookingRepository.GetActiveBookingsBySeatAsync(seatId);
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task UpdateBookingStatusAsync()
        {
            var activeBookings = await _bookingRepository.GetAllBookingsAsync();

            foreach (var booking in activeBookings)
            {
                if (booking.IsActive && booking.BookingDate.AddDays(booking.BookingDuration) < DateTime.Now)
                {
                    booking.IsActive = false;
                    await _bookingRepository.UpdateBookingAsync(booking);
                }
            }
        }
    }
}





//using SeatBooking.DAL.Models;
//using SeatBooking.DAL.DALRepository.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SeatBooking.Repository.Business_Services.Business_Contracts;
//using Microsoft.EntityFrameworkCore;

//namespace SeatBooking.Repository.Business_Services
//{ 
//    public class BookingService : IBookingService
//    {
//        private readonly IBookingRepository _bookingRepository;

//        public BookingService(IBookingRepository bookingRepository)
//        {
//            _bookingRepository = bookingRepository;
//        }

//        public async Task<int> CreateBookingAsync(Booking booking)
//        {
//            var existingBooking = await _bookingRepository.GetActiveBookingByEmployeeAsync(booking.EmployeeId);
//            if (existingBooking != null)
//            {
//                throw new Exception("Employee already has an active booking.");
//            }

//            return await _bookingRepository.CreateBookingAsync(booking);
//        }

//        public async Task<bool> CancelBookingAsync(int bookingId)
//        {
//            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
//            if (booking != null && booking.IsActive)
//            {
//                booking.IsActive = false;
//                return await _bookingRepository.UpdateBookingAsync(booking);
//            }
//            return false; // Return false if the booking is not found or not active
//        }

//        public async Task<Booking> GetActiveBookingByEmployeeAsync(int employeeId)
//        {
//            return await _bookingRepository.GetActiveBookingByEmployeeAsync(employeeId);
//        }

//        public async Task<List<Booking>> GetAllBookingsAsync()
//        {
//            return await _bookingRepository.GetAllBookingsAsync();
//        }

//        public async Task<Booking> GetBookingByIdAsync(int bookingId)
//        {
//            return await _bookingRepository.GetBookingByIdAsync(bookingId);
//        }
//        public async Task<IEnumerable<Booking>> GetActiveBookingsBySeatAsync(int seatId)
//        {
//            return await _bookingRepository.GetActiveBookingsBySeatAsync(seatId);
//        }



//        public async Task UpdateBookingStatusAsync()
//        {
//            var activeBookings = await _bookingRepository.GetAllBookingsAsync();

//            foreach (var booking in activeBookings)
//            {
//                if (booking.IsActive && booking.BookingDate.AddDays(booking.BookingDuration) < DateTime.Now)
//                {
//                    booking.IsActive = false;
//                    await _bookingRepository.UpdateBookingAsync(booking); // Implement this method in the repository
//                }
//            }
//        }




//    }
//}

