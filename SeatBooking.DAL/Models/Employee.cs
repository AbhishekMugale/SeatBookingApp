using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeatBooking.DAL.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public int CreatedBy { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Role Role { get; set; } = null!;

}
