using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public ProductRepository(CoffeeStoreDbContext context) : base(context)
    {
    }
}