using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Request.Category;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>();
        CreateMap<CreateCategoryRequest,Category>();
        CreateMap<UpdateCategoryRequest, Category>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}