using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

namespace CoffeeStore.Tests.Repository;

/// <summary>
/// Fake UnitOfWork for testing
/// </summary>
public class FakeUnitOfWork : IUnitOfWork
{
    public IProductRepository Products { get; }
    public IUserRepository Users { get; }
    public ICategoryRepository Categories { get; }
    public IOrderRepository Orders { get; }
    public IOrderDetailRepository OrderDetails { get; }
    public IPaymentRepository Payments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public async Task<int> CompleteAsync()
    {
        throw new NotImplementedException();
    }

    public FakeUnitOfWork(ICategoryRepository categoryRepository)
    {
        Categories = categoryRepository;
    }

    public Task SaveChangesAsync()
    {
        // Fake: do nothing
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Fake: do nothing
    }
}

