using PRN232.Lab2.CoffeeStore.Repositories.Repository.GenericRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository.UnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<T, TId> GetRepository<T, TId>()
        where T : class
        where TId : notnull;

    Task<int> SaveChangesAsync();
}