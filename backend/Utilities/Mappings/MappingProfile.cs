using AutoMapper;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using Microsoft.AspNetCore.Identity.Data;

namespace learnyx.Utilities.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<User, UserDTO>();
    }
}