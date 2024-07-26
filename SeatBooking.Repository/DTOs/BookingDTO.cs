using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int EmployeeId { get; set; }
        public int SeatId { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingDuration { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
}
