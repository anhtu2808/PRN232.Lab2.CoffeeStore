using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    IOrderDetailRepository OrderDetails { get; }
    IPaymentRepository Payments { get; }
    Task<int> CompleteAsync();
}