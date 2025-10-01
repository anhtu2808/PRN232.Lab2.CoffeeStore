using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IUserRepository Users { get; }
    Task<int> CompleteAsync();
}