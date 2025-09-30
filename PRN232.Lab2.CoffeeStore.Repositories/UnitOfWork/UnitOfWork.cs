
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.Repository;


namespace PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CoffeeStoreDbContext _context; 

    //  Repository properties
    public IProductRepository Products { get; private set; }

    public UnitOfWork(CoffeeStoreDbContext context)
    {
        _context = context;

        Products = new ProductRepository(_context);
    }


    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}