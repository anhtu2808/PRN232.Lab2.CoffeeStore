using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Models.Response.Product;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore()); 
        CreateMap<Product, ProductResponse>()
            .ForMember(
                dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.Category!.CategoryId) 
            )
            .ForMember(
                dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category!.Name) 
            );
    }
}