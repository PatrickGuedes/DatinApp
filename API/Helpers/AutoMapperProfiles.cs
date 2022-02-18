using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                                            .ForMember(dest => dest.PhotoUrl, opt => opt
                                            .MapFrom(src => src.Photos
                                            .FirstOrDefault(photo => photo.IsMain).Url))

                                            .ForMember(dest => dest.Age, opt => opt
                                            .MapFrom(src => src.DateOfBirth.CalculateAge()))

                                            ;
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.UserSenderPhotoUrl, opt => opt.MapFrom(
                           src => src.UserSender.Photos
                                                        .FirstOrDefault(photo => photo.IsMain).Url))

                .ForMember(dest => dest.UserRecipientPhotoUrl, opt => opt.MapFrom(
                           src => src.UserRecipient.Photos
                                                        .FirstOrDefault(photo => photo.IsMain).Url));
        }

    }
}