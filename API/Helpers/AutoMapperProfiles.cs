using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //mapujemy AppUser na MemberDto
                //ForMember potrzebne jest aby pobrać Url do głównego zdjęcia
                //ForMember oznacza na którą property chcemy wpłynąć
                    //destination - którą property wpływamy
                    //options - oznaczamy z kąd chcemy mapować 
                        //src - wybieramy źródło ( pośród zdjęć szukamy zdjęcia głównego czyli IsMain i wybieramy z niego URL)

            //podsumowująć wybieramy na co chcemy mapować następnie szukamy tego co chcemy tam wsadzić 
            CreateMap<AppUser, MemberDto>()
                .ForMember(destination => destination.PhotoUrl, options => options.MapFrom(src =>
                                                     src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            //mapujemy Photo na PhotoDto
            CreateMap<Photo,PhotoDto>();
        }
    }
}