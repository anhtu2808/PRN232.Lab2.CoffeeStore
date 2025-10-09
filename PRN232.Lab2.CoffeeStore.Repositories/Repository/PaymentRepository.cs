using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
{
    public PaymentRepository(CoffeeStoreDbContext context) : base(context)
    {
    }
}
