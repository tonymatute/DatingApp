using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace API.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AppUsers, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                    src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDTO, AppUsers>();
        }
    }
}
