using System;
using System.ComponentModel.DataAnnotations;

namespace SeatBooking.Web.Models
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username must be between 1 and 50 characters", MinimumLength = 1)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be between 1 and 100 characters", MinimumLength = 1)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int RoleID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, ErrorMessage = "First name must be between 1 and 20 characters", MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(20, ErrorMessage = "Last name must be between 1 and 20 characters", MinimumLength = 1)]
        public string LastName { get; set; }

        // Additional properties


        public bool IsActive { get; set; } = true;
        

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; } = 1;
    }
}
