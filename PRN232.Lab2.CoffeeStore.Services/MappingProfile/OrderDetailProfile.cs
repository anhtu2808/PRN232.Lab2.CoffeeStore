using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Request.Order;
using PRN232.Lab2.CoffeeStore.Models.Response.Order;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class OrderDetailProfile : Profile
{
    public OrderDetailProfile()
    {
        CreateMap<OrderItemRequest,OrderDetail>();
        CreateMap<OrderDetail,OrderDetailResponse>();
    }
}