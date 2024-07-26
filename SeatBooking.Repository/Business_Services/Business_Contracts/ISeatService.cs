using SeatBooking.DAL.Models;
using SeatBooking.Repository.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBooking.Repository.Business_Services.Business_Contracts
{
    public interface ISeatService
    {

        Task<IEnumerable<SeatDTO>> GetAvailableSeatsAsync();
        Task<IEnumerable<SeatDTO>> GetAvailableSeatsAsync(DateTime startDate, DateTime endDate);
        Task<SeatDTO> GetSeatByIdAsync(int seatId);
        Task<IEnumerable<SeatDTO>> GetAllSeatsAsync();


    }
}
