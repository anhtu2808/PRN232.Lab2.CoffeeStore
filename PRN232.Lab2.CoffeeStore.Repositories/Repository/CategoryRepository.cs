using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
{
    public CategoryRepository(CoffeeStoreDbContext context) : base(context)
    {
    }
}