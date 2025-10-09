using PRN232.Lab2.CoffeeStore.Repositories.Entity;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class OrderDetailRepository : GenericRepository<OrderDetail, int>, IOrderDetailRepository
{
    public OrderDetailRepository(CoffeeStoreDbContext context) : base(context)
    {
    }
}