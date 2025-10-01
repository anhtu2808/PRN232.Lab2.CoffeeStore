using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.User;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<UserRequest, User>();
    }
}