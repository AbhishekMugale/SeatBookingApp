using System;
using System.Collections.Generic;

namespace SeatBooking.DAL.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
