using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Order;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<CreateOrderRequest, Order>();
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment))
            ;
    }
}