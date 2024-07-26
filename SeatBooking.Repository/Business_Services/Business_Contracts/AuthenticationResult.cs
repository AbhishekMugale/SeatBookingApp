using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services.Business_Contracts
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public Employee Employee { get; set; }
        public string RoleName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
