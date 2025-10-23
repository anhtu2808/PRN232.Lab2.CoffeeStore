using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Payment;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.IRepository;

public interface IPaymentRepository : IGenericRepository<Payment, int>
{
    Task<PageResponse<Payment>> GetPaymentsAsync(PaymentFilter filter);
}