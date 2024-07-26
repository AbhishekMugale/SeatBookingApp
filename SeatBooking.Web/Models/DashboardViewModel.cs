namespace SeatBooking.Web.Models
{
    public class DashboardViewModel
    {
        public List<SeatBooking.Web.Models.SeatViewModel> Seats { get; set; }
        public BookingViewModel ActiveBooking { get; set; }
        public List<SeatBooking.Web.Models.SeatViewModel> AllSeats { get; set; }
        public Dictionary<int, BookingViewModel> SeatBookings { get; set; }
        public Dictionary<int, string> BookingEmployeeNames { get; set; }

    }
}
