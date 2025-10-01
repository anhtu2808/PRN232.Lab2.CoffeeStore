using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class UserRepository : GenericRepository<User, Guid>, IUserRepository
{
    private readonly DbSet<User> _dbSet;

    public UserRepository(CoffeeStoreDbContext context) : base(context)
    {
        _dbSet = context.Set<User>();
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);
    }
}