using Microsoft.Identity.Client;
using SeatBooking.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services.Business_Contracts
{
    public interface IUserService
    {
        Task<AuthenticationResult> Authenticate(string username, string password);
    }
}
