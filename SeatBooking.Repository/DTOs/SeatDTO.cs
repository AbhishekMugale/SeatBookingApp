using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.DTOs
{
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
