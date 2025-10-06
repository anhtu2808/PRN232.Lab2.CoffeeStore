using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class PageMapping : Profile
{
    public PageMapping()
    {
        CreateMap(typeof(PageResponse<>), typeof(PageResponse<>));
    }
}