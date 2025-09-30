using PRN232.Lab2.CoffeeStore.Repositories.Repository.GenericRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    // IProductRepository Products { get; }
    // IOrderRepository Orders { get; }

    Task<int> CompleteAsync();
}