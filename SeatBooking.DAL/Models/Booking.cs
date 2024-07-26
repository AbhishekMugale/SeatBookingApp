using System;
using System.Collections.Generic;

namespace SeatBooking.DAL.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int EmployeeId { get; set; }

    public int SeatId { get; set; }

    public DateTime BookingDate { get; set; }

    public int BookingDuration { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
