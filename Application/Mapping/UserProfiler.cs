using AutoMapper;
using Domain.DTOs.User;
using Domain.Entities;

namespace Application.Mapping;

public class UserProfiler : Profile
{
    public UserProfiler()
    {
        CreateMap<UserDto, User>()
            .ReverseMap();

        CreateMap<UserCreateDto, User>()
            .ForMember(opt => opt.Id, 
                opt 
                    => opt.MapFrom(src
                        => src.ChatId));
    }
}