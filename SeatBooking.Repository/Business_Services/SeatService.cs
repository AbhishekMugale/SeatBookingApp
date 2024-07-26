using AutoMapper;
using SeatBooking.DAL.Models;
using SeatBooking.DAL.DALRepository.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using Microsoft.AspNetCore.Http;
using SeatBooking.Repository.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SeatBooking.Repository.Business_Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeatService(ISeatRepository seatRepository, IEmployeeService employeeService, IBookingService bookingService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _seatRepository = seatRepository;
            _employeeService = employeeService;
            _bookingService = bookingService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<SeatDTO>> GetAvailableSeatsAsync()
        {
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

            if (existingBooking != null)
            {
                var startDate = DateTime.Today;
                var endDate = existingBooking.BookingDate.AddDays(existingBooking.BookingDuration);
                var seats = await _seatRepository.GetAvailableSeatsAsync(startDate, endDate);
                return _mapper.Map<IEnumerable<SeatDTO>>(seats);
            }
            else
            {
                var seats = await _seatRepository.GetAvailableSeatsAsync();
                return _mapper.Map<IEnumerable<SeatDTO>>(seats);
            }
        }

        public async Task<IEnumerable<SeatDTO>> GetAvailableSeatsAsync(DateTime startDate, DateTime endDate)
        {
            var seats = await _seatRepository.GetAvailableSeatsAsync(startDate, endDate);
            return _mapper.Map<IEnumerable<SeatDTO>>(seats);
        }

        public async Task<SeatDTO> GetSeatByIdAsync(int seatId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            return _mapper.Map<SeatDTO>(seat);
        }
        public async Task<IEnumerable<SeatDTO>> GetAllSeatsAsync()
        {
            var seats = await _seatRepository.GetAllSeatsAsync();
            return _mapper.Map<IEnumerable<SeatDTO>>(seats);
        }
    }
}

















//using SeatBooking.DAL.Models;
//using SeatBooking.DAL.DALRepository.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using SeatBooking.Repository.Business_Services.Business_Contracts;

//namespace SeatBooking.Repository.Business_Services
//{
//    public class SeatService : ISeatService
//    {
//        private readonly ISeatRepository _seatRepository;
//        private readonly IEmployeeService _employeeService;
//        private readonly IBookingService _bookingService;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public SeatService(ISeatRepository seatRepository, IEmployeeService employeeService, IBookingService bookingService, IHttpContextAccessor httpContextAccessor)
//        {
//            _seatRepository = seatRepository;
//            _employeeService = employeeService;
//            _bookingService = bookingService;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync()
//        {
//            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
//            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

//            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

//            if (existingBooking != null)
//            {
//                var startDate = DateTime.Today;
//                var endDate = existingBooking.BookingDate.AddDays(existingBooking.BookingDuration);
//                return await _seatRepository.GetAvailableSeatsAsync(startDate, endDate);
//            }
//            else
//            {
//                return await _seatRepository.GetAvailableSeatsAsync();
//            }
//        }

//        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(DateTime startDate, DateTime endDate)
//        {
//            return await _seatRepository.GetAvailableSeatsAsync(startDate, endDate);
//        }

//        public async Task<Seat> GetSeatByIdAsync(int seatId)
//        {
//            return await _seatRepository.GetSeatByIdAsync(seatId);
//        }
//    }
//}


