using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using SeatBooking.Repository.DTOs;
using SeatBooking.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeatBooking.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ISeatService _seatService;
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public EmployeeController(
            IEmployeeService employeeService,
            ISeatService seatService,
            IBookingService bookingService,
            IUserService userService,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _seatService = seatService;
            _bookingService = bookingService;
            _userService = userService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard(int employeeId)
        {
            ViewBag.EmployeeId = employeeId;
            return await Dashboard(HttpContext, "AdminDashboard");
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EmployeeDashboardAsync()
        {
            return await Dashboard(HttpContext, "EmployeeDashboard");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateEmployee()
        {
            var model = new CreateEmployeeViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employeeDto = _mapper.Map<EmployeeDTO>(model);

                int createdEmployeeId = await _employeeService.CreateEmployeeAsync(employeeDto, model.Password);

                if (createdEmployeeId > 0)
                {
                    return RedirectToAction("AdminDashboard", "Employee");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to create employee.");
                    return View(model);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var employeeViewModels = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(employeeViewModels);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ShowAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();

            var bookingViewModels = from booking in bookings
                                    join employee in employees on booking.EmployeeId equals employee.EmployeeId
                                    select new BookingViewModel
                                    {
                                        BookingId = booking.BookingId,
                                        EmployeeId = booking.EmployeeId,
                                        SeatId = booking.SeatId,
                                        BookingDate = booking.BookingDate,
                                        BookingDuration = booking.BookingDuration,
                                        IsActive = booking.IsActive,
                                        Username = employee.Username // Set the username here
                                    };

            return View(bookingViewModels.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> BookSeat(int seatId, int bookingDuration)
        {
            var seat = await _seatService.GetSeatByIdAsync(seatId);
            if (seat == null || !seat.IsActive)
            {
                ModelState.AddModelError(string.Empty, "You already have an active booking.");
            }

            var username = HttpContext.User.Identity.Name;
            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

            if (existingBooking != null)
            {
                TempData["ErrorMessage"] = "You already have an active booking.";
                return RedirectToAction(User.IsInRole("Admin") ? "AdminDashboard" : "EmployeeDashboard");
            }

            var model = new ConfirmBookingViewModel
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                BookingDuration = bookingDuration
            };

            return View("ConfirmBooking", model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int seatId, int bookingDuration)
        {
            var seat = await _seatService.GetSeatByIdAsync(seatId);
            if (seat == null || !seat.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid or inactive seat.");
                return RedirectToAction("EmployeeDashboard");
            }

            var username = HttpContext.User.Identity.Name;
            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

            if (existingBooking != null)
            {
                TempData["ErrorMessage"] = "You already have an active booking.";
                return RedirectToAction("EmployeeDashboard");
            }

            var bookingStartTime = DateTime.Today.AddDays(1).Date.AddHours(9);
            var bookingDto = new BookingDTO
            {
                EmployeeId = employeeId,
                SeatId = seatId,
                BookingDate = bookingStartTime,
                BookingDuration = bookingDuration,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = employeeId
            };

            try
            {
                var createdBookingId = await _bookingService.CreateBookingAsync(bookingDto);

                if (createdBookingId > 0)
                {
                    await _bookingService.UpdateBookingStatusAsync();
                    return RedirectToAction(User.IsInRole("Admin") ? "AdminDashboard" : "EmployeeDashboard");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create booking.";
                    return RedirectToAction("EmployeeDashboard");
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the booking.";
                return RedirectToAction("EmployeeDashboard");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var isAdmin = User.IsInRole("Admin");
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                var username = HttpContext.User.Identity.Name;
                var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

                if (isAdmin || booking.EmployeeId == employeeId)
                {
                    var result = await _bookingService.CancelBookingAsync(bookingId);
                    if (result)
                    {
                        return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Failed to cancel booking.");
                        return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unauthorized to cancel this booking.");
                    return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while canceling the booking.");
                return RedirectToAction("EmployeeDashboard");
            }
        }
        private async Task<IActionResult> Dashboard(HttpContext httpContext, string viewName)
        {
            var username = HttpContext.User.Identity.Name;

           // ViewBag.WelcomeMessage = $"Welcome {username}";
            var employeename = await _employeeService.GetEmployeeFirstNamebyUserNameAsync(username);
            ViewBag.WelcomeMessage = $"Welcome {employeename}";
            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);
            

            IEnumerable<SeatDTO> allSeats = await _seatService.GetAllSeatsAsync();
            IEnumerable<SeatDTO> availableSeats;
            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

            var seatBookings = new Dictionary<int, BookingDTO>();
            var bookingEmployeeNames = new Dictionary<int, string>();

            if (existingBooking != null)
            {
                var startDate = DateTime.Today.AddDays(1).Date.AddHours(9);
                var endDate = existingBooking.BookingDate.AddDays(existingBooking.BookingDuration).Date.AddHours(17);

                availableSeats = await _seatService.GetAvailableSeatsAsync(startDate, endDate);

                var bookedSeatIds = (await _bookingService.GetActiveBookingsBySeatAsync(existingBooking.SeatId))
                                            .Select(b => b.SeatId);

                availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
            }
            else
            {
                availableSeats = await _seatService.GetAvailableSeatsAsync();

                var bookedSeatIds = new List<int>();
                foreach (var seat in availableSeats)
                {
                    var activeBookingsForSeat = await _bookingService.GetActiveBookingsBySeatAsync(seat.SeatId);
                    if (activeBookingsForSeat.Any())
                    {
                        bookedSeatIds.Add(seat.SeatId);
                    }
                }

                availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
            }

            foreach (var seat in allSeats)
            {
                var activeBookingsForSeat = await _bookingService.GetActiveBookingsBySeatAsync(seat.SeatId);
                if (activeBookingsForSeat.Any())
                {
                    var booking = activeBookingsForSeat.First();
                    seatBookings[seat.SeatId] = booking;

                    var employee = await _employeeService.GetEmployeeByIdAsync(booking.EmployeeId);
                    bookingEmployeeNames[seat.SeatId] = employee?.FirstName;
                }
            }

            var allSeatsViewModels = _mapper.Map<List<SeatViewModel>>(allSeats);
            var availableSeatsViewModels = _mapper.Map<List<SeatViewModel>>(availableSeats);
            var dashboardModel = new DashboardViewModel
            {
                AllSeats = allSeatsViewModels,
                Seats = availableSeatsViewModels,
                ActiveBooking = existingBooking != null ? _mapper.Map<BookingViewModel>(existingBooking) : null,
                SeatBookings = seatBookings.ToDictionary(kvp => kvp.Key, kvp => _mapper.Map<BookingViewModel>(kvp.Value)),
                BookingEmployeeNames = bookingEmployeeNames
            };

            return View(viewName, dashboardModel);
        }


        //private async Task<IActionResult> Dashboard(HttpContext httpContext, string viewName)
        //{
        //    var username = HttpContext.User.Identity.Name;
        //    ViewBag.WelcomeMessage = $"Welcome {username}";

        //    var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

        //    IEnumerable<SeatDTO> allSeats = await _seatService.GetAllSeatsAsync();

        //    IEnumerable<SeatDTO> availableSeats;
        //    var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

        //    if (existingBooking != null)
        //    {
        //        var startDate = DateTime.Today.AddDays(1).Date.AddHours(9);
        //        var endDate = existingBooking.BookingDate.AddDays(existingBooking.BookingDuration).Date.AddHours(17);

        //        availableSeats = await _seatService.GetAvailableSeatsAsync(startDate, endDate);

        //        var bookedSeatIds = (await _bookingService.GetActiveBookingsBySeatAsync(existingBooking.SeatId))
        //                                .Select(b => b.SeatId);

        //        availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
        //    }
        //    else
        //    {
        //        availableSeats = await _seatService.GetAvailableSeatsAsync();

        //        var bookedSeatIds = new List<int>();
        //        foreach (var seat in availableSeats)
        //        {
        //            var activeBookingsForSeat = await _bookingService.GetActiveBookingsBySeatAsync(seat.SeatId);
        //            if (activeBookingsForSeat.Any())
        //            {
        //                bookedSeatIds.Add(seat.SeatId);
        //            }
        //        }

        //        availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
        //    }

        //    var allSeatsViewModels = _mapper.Map<List<SeatViewModel>>(allSeats);


        //    var webSeats = _mapper.Map<List<SeatViewModel>>(availableSeats);
        //    var dashboardModel = new DashboardViewModel
        //    {
        //        AllSeats = allSeatsViewModels,
        //        Seats = webSeats,
        //        ActiveBooking = existingBooking != null ? _mapper.Map<BookingViewModel>(existingBooking) : null
        //    };

        //    return View(viewName, dashboardModel);
        //}



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);
            return View(employeeViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employeeDto = _mapper.Map<EmployeeDTO>(model);

                var result = await _employeeService.UpdateEmployeeAsync(employeeDto);
                if (result)
                {
                    return RedirectToAction("ViewAllEmployees");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
                }
            }

            return View(model);
        }
    }
}









//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SeatBooking.DAL.Models;
//using SeatBooking.Repository.Business_Services.Business_Contracts;
//using SeatBooking.Repository.DTOs;
//using SeatBooking.Web.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SeatBooking.Web.Controllers
//{
//    public class EmployeeController : Controller
//    {
//        private readonly IEmployeeService _employeeService;
//        private readonly ISeatService _seatService;
//        private readonly IBookingService _bookingService;
//        private readonly IUserService _userService;

//        public EmployeeController(IEmployeeService employeeService, ISeatService seatService, IBookingService bookingService, IUserService userService)
//        {
//            _employeeService = employeeService;
//            _seatService = seatService;
//            _bookingService = bookingService;
//            _userService = userService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> AdminDashboard(int employeeId)
//        {
//            ViewBag.EmployeeId = employeeId;
//            return await Dashboard(HttpContext, "AdminDashboard");
//        }
//        [Authorize(Roles = "Employee")]
//        public async Task<IActionResult> EmployeeDashboardAsync()
//        {
//            return await Dashboard(HttpContext, "EmployeeDashboard");
//        }

//        [Authorize(Roles = "Admin")]
//        [HttpGet]
//        public IActionResult CreateEmployee()
//        {
//            var model = new CreateEmployeeViewModel();
//            return View(model);
//        }
//        [HttpPost]
//        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var employeeDto = new EmployeeDTO
//                {
//                    Username = model.Username,
//                    Password = model.Password,
//                    RoleId = model.RoleID,
//                    FirstName = model.FirstName,
//                    LastName = model.LastName,
//                    CreatedBy = 1,
//                    IsActive = true,
//                    // Add other properties as needed
//                };

//                int createdEmployeeId = await _employeeService.CreateEmployeeAsync(employeeDto, model.Password);

//                if (createdEmployeeId > 0)
//                {
//                    return RedirectToAction("AdminDashboard", "Employee");
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Failed to create employee.");
//                    return View(model);
//                }
//            }

//            return View(model);
//        }

//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> ViewAllEmployees()
//        {
//            var employees = await _employeeService.GetAllEmployeesAsync();
//            var employeeViewModels = employees.Select(e => new EmployeeViewModel
//            {
//                EmployeeId = e.EmployeeId,
//                Username = e.Username,
//                FirstName = e.FirstName,
//                LastName = e.LastName,
//                IsActive = e.IsActive,
//                CreatedDate = e.CreatedDate,
//                CreatedBy = e.CreatedBy
//            }).ToList();
//            return View(employeeViewModels);
//        }

//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> ShowAllBookings()
//        {
//            var bookings = await _bookingService.GetAllBookingsAsync();
//            var employees = await _employeeService.GetAllEmployeesAsync();

//            var bookingViewModels = from booking in bookings
//                                    join employee in employees on booking.EmployeeId equals employee.EmployeeId
//                                    select new BookingViewModel
//                                    {
//                                        BookingId = booking.BookingId,
//                                        EmployeeId = booking.EmployeeId,
//                                        SeatId = booking.SeatId,
//                                        BookingDate = booking.BookingDate,
//                                        BookingDuration = booking.BookingDuration,
//                                        IsActive = booking.IsActive,
//                                        Username = employee.Username // Set the username here
//                                    };

//            return View(bookingViewModels.ToList());
//        }




//        [HttpPost]
//        public async Task<IActionResult> BookSeat(int seatId, int bookingDuration)
//        {
//            var seat = await _seatService.GetSeatByIdAsync(seatId);
//            if (seat == null || !seat.IsActive)
//            {
//                //ModelState.AddModelError(string.Empty, "Invalid or inactive seat.");
//                ModelState.AddModelError(string.Empty, "You already have an active booking.");

//            }

//            var username = HttpContext.User.Identity.Name;
//            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

//            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

//            if (existingBooking != null)
//            {
//                // ModelState.AddModelError(string.Empty, "You already have an active booking.");
//                TempData["ErrorMessage"] = "You already have an active booking.";
//                if (User.IsInRole("Admin"))
//                {
//                    return RedirectToAction("AdminDashboard");
//                }
//                else
//                {
//                    return RedirectToAction("EmployeeDashboard");
//                }


//            }

//            var model = new ConfirmBookingViewModel
//            {
//                SeatId = seat.SeatId,
//                SeatNumber = seat.SeatNumber,
//                BookingDuration = bookingDuration
//            };

//            return View("ConfirmBooking", model);
//        }

//        [HttpPost]
//        public async Task<IActionResult> ConfirmBooking(int seatId, int bookingDuration)
//        {
//            var seat = await _seatService.GetSeatByIdAsync(seatId);
//            if (seat == null || !seat.IsActive)
//            {
//                ModelState.AddModelError(string.Empty, "Invalid or inactive seat.");
//                return RedirectToAction("EmployeeDashboard");
//            }

//            var username = HttpContext.User.Identity.Name;
//            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

//            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

//            if (existingBooking != null)
//            {
//                //ModelState.AddModelError(string.Empty, "You already have an active booking.");
//                TempData["ErrorMessage"] = "You already have an active booking.";
//                return RedirectToAction("EmployeeDashboard");
//            }
//            var bookingStartTime = DateTime.Today.AddDays(1).Date.AddHours(9);
//            var bookingdto = new BookingDTO
//            {
//                EmployeeId = employeeId,
//                SeatId = seatId,
//                BookingDate = bookingStartTime,
//                BookingDuration = bookingDuration,
//                IsActive = true,
//                CreatedDate = DateTime.Now,
//                CreatedBy = employeeId
//            };

//            try
//            {
//                var createdBookingId = await _bookingService.CreateBookingAsync(bookingdto);

//               if (createdBookingId > 0)
//               {
//                    // Update booking status immediately after creation
//                    await _bookingService.UpdateBookingStatusAsync();

//                    var isAdmin = User.IsInRole("Admin");

//                    // Redirect based on the role
//                    return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
//                }
//                else
//                {
//                    TempData["ErrorMessage"] = "Failed to create booking.";
//                    return RedirectToAction("EmployeeDashboard");
//                }
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "An error occurred while creating the booking.";
//                return RedirectToAction("EmployeeDashboard");
//            }
//        }


//        [HttpPost]
//        public async Task<IActionResult> CancelBooking(int bookingId)
//        {
//            try
//            {
//                var isAdmin = User.IsInRole("Admin");
//                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
//                var username = HttpContext.User.Identity.Name;
//                var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

//                if (isAdmin || booking.EmployeeId == employeeId)
//                {
//                    var result = await _bookingService.CancelBookingAsync(bookingId);
//                    if (result)
//                    {
//                        return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
//                    }
//                    else
//                    {
//                        ModelState.AddModelError(string.Empty, "Failed to cancel booking.");
//                        return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
//                    }
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Unauthorized to cancel this booking.");
//                    return RedirectToAction(isAdmin ? "AdminDashboard" : "EmployeeDashboard");
//                }
//            }
//            catch (Exception ex)
//            {
//                ModelState.AddModelError(string.Empty, "An error occurred while canceling the booking.");
//                return RedirectToAction("EmployeeDashboard");
//            }
//        }
//        private async Task<IActionResult> Dashboard(HttpContext httpContext,string viewName)
//        {


//            var username = HttpContext.User.Identity.Name;
//            ViewBag.WelcomeMessage = $"Welcome {username}";

//            var employeeId = await _employeeService.GetEmployeeIdByUsernameAsync(username);

//            IEnumerable<SeatDTO> availableSeats;

//            var existingBooking = await _bookingService.GetActiveBookingByEmployeeAsync(employeeId);

//            if (existingBooking != null)
//            {
//                var startDate = DateTime.Today.AddDays(1).Date.AddHours(9); // Start from next day's morning 9 AM
//                var endDate = existingBooking.BookingDate.AddDays(existingBooking.BookingDuration).Date.AddHours(17); // Assume office ends at 5 PM

//                availableSeats = await _seatService.GetAvailableSeatsAsync(startDate, endDate);

//                //// Extract seat IDs from existing bookings
//                var bookedSeatIds = (await _bookingService.GetActiveBookingsBySeatAsync(existingBooking.SeatId))
//                                        .Select(b => b.SeatId);

//                // Filter out the seats already booked by other users
//                availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
//            }
//            else
//            {
//                availableSeats = await _seatService.GetAvailableSeatsAsync();

//                // Get all active bookings for each seat and filter out booked seats
//                var bookedSeatIds = new List<int>();
//                foreach (var seat in availableSeats)
//                {
//                    var activeBookingsForSeat = await _bookingService.GetActiveBookingsBySeatAsync(seat.SeatId);
//                    if (activeBookingsForSeat.Any())
//                    {
//                        bookedSeatIds.Add(seat.SeatId);
//                    }
//                }

//                // Exclude seats that are already booked by other users
//                availableSeats = availableSeats.Where(seat => !bookedSeatIds.Contains(seat.SeatId));
//            }

//            var webSeats = availableSeats.Select(seat => new SeatBooking.Web.Models.Seat
//            {
//                SeatId = seat.SeatId,
//                SeatNumber = seat.SeatNumber,
//                IsActive = seat.IsActive,
//                CreatedDate = seat.CreatedDate,
//                CreatedBy = seat.CreatedBy
//            }).ToList();
//            var dashboardModel = new DashboardViewModel
//            {
//                Seats = webSeats,
//                ActiveBooking = existingBooking != null ? new BookingViewModel
//                {
//                    BookingId = existingBooking.BookingId,
//                    SeatId = existingBooking.SeatId,
//                    BookingDate = existingBooking.BookingDate,
//                    BookingDuration = existingBooking.BookingDuration,
//                    IsActive = existingBooking.IsActive
//                } : null
//            };

//            return View(viewName, dashboardModel);
//            //return View(viewName, webSeats);
//        }



//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateEmployee(int id)
//        {
//            var employee = await _employeeService.GetEmployeeByIdAsync(id);
//            if (employee == null)
//            {
//                return NotFound();
//            }

//            var employeeViewModel = new EmployeeViewModel
//            {
//                EmployeeId = employee.EmployeeId,
//                FirstName = employee.FirstName,
//                LastName = employee.LastName,
//                Username = employee.Username,
//                IsActive = employee.IsActive,
//                CreatedBy = employee.CreatedBy,
//                //CreatedDate = employee.CreatedDate
//            };

//            return View(employeeViewModel);
//        }

//        [HttpPost]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var employeedto = new EmployeeDTO
//                {
//                    EmployeeId = model.EmployeeId,
//                    FirstName = model.FirstName,
//                    LastName = model.LastName,
//                    IsActive = model.IsActive,
//                    // Password property is not included
//                };

//                var result = await _employeeService.UpdateEmployeeAsync(employeedto);
//                if (result)
//                {
//                    return RedirectToAction("ViewAllEmployees");
//                }
//                else
//                {
//                    ModelState.AddModelError(string.Empty, "Failed to update employee.");
//                }
//            }

//            return View(model);
//        }

//    }
//}