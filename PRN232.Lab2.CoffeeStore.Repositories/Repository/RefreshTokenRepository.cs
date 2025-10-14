using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class RefreshTokenRepository : GenericRepository<RefreshToken, int>, IRefreshTokenRepository
{
    public RefreshTokenRepository(CoffeeStoreDbContext context) : base(context)
    {
    }
}