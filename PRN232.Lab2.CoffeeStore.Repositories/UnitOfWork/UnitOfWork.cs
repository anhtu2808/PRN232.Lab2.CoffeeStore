using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.Repository;


namespace PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CoffeeStoreDbContext _context;

    //  Repository properties
    public IProductRepository Products { get; private set; }
    public IUserRepository Users { get; private set; }

    public ICategoryRepository Categories { get; private set; }

    public IOrderRepository Orders { get; private set; }
    
    public IOrderDetailRepository OrderDetails { get; private set; }
    
    public IPaymentRepository Payments { get; private set; }
    
    public IRefreshTokenRepository RefreshTokens { get; private set; }

    public UnitOfWork(CoffeeStoreDbContext context)
    {
        _context = context;
        Categories = new CategoryRepository(_context);
        Products = new ProductRepository(_context);
        Users = new UserRepository(_context);
        Orders = new OrderRepository(_context);
        OrderDetails = new OrderDetailRepository(_context);
        Payments = new PaymentRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
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