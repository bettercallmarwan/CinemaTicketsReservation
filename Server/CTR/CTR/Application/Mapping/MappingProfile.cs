using AutoMapper;
using CTR.Application.DTOs.Reservation;
using CTR.Models.Classes;

namespace CTR.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie.Title));
        }
    }
}
