using System.ComponentModel.DataAnnotations;

namespace SeatBooking.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username must contain only letters and numbers")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
