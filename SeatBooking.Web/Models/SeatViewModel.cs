using SeatBooking.DAL.Models;
using System;

namespace SeatBooking.Web.Models
{
    public class SeatViewModel
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public ICollection<Booking> Bookings { get; set; } // Booking associated with the seat


    }
}
