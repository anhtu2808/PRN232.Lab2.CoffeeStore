using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Response.Payment;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Services.MappingProfile;

public class PaymentProfile : Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentResponse>();
    }
}