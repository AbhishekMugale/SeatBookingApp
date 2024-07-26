namespace SeatBooking.Web.Models
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public int EmployeeId { get; set; }
        public int SeatId { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingDuration { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
    }
}
