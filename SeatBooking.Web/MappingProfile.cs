using AutoMapper;
using SeatBooking.DAL.Models;
using SeatBooking.Repository.DTOs;
using SeatBooking.Web.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EmployeeDTO, EmployeeViewModel>().ReverseMap();
        CreateMap<CreateEmployeeViewModel, EmployeeDTO>();
        CreateMap<BookingDTO, BookingViewModel>().ReverseMap();
        CreateMap<SeatDTO, SeatViewModel>().ReverseMap();

        CreateMap<Booking, BookingDTO>().ReverseMap();
        CreateMap<Employee, EmployeeDTO>().ReverseMap();
        CreateMap<Seat, SeatDTO>().ReverseMap();
    }
}
