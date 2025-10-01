using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.IRepository;

public interface IUserRepository : IGenericRepository<User, Guid>
{
    Task<User?> GetByUsernameAndPasswordAsync(string username, string password);
}