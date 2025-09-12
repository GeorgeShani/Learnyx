using AutoMapper;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Requests;

namespace learnyx.Utilities.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, User>();
        CreateMap<User, UserDTO>();
    }
}