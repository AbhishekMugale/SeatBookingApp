namespace SeatBooking.Web.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; } = 1;
    }

}
