using AutoMapper;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Mapper;

public class UserProfile
    : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, IdentityUser>();
        CreateMap<IdentityUser, AppUser>();
    }
}