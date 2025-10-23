using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Payment;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;

public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
{
    private readonly DbSet<Payment> _dbSet;

    public PaymentRepository(CoffeeStoreDbContext context) : base(context)
    {
        _dbSet = context.Set<Payment>();
    }

   public async Task<PageResponse<Payment>> GetPaymentsAsync(PaymentFilter filter)
    {
        IQueryable<Payment> query = _dbSet.Include(p => p.Order);

        // Lọc theo OrderId
        if (filter.OrderId.HasValue)
        {
            query = query.Where(p => p.OrderId == filter.OrderId.Value);
        }

        // Lọc theo Status
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(p => p.Status.ToLower().Contains(filter.Status.Trim().ToLower()));
        }

        // Lọc theo phương thức thanh toán
        if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
        {
            query = query.Where(p => p.PaymentMethod.ToLower().Contains(filter.PaymentMethod.Trim().ToLower()));
        }

        // Lọc theo khoảng ngày thanh toán
        if (filter.FromDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(p => p.PaymentDate <= filter.ToDate.Value);
        }

        // Tìm kiếm theo keyword (nếu có)
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var searchTerm = filter.Keyword.Trim().ToLower();
            query = query.Where(p =>
                p.PaymentMethod.ToLower().Contains(searchTerm) ||
                p.Status.ToLower().Contains(searchTerm)
            );
        }

        // Sắp xếp
        var isDescending = filter.SortDirection == "desc";
        query = ApplyOrderByPropertyName(query, filter.Sort, isDescending);

        // Tổng số lượng
        var totalCount = await query.CountAsync();

        // Phân trang
        var page = filter.Page > 0 ? filter.Page : 1;
        var pageSize = filter.PageSize > 0 ? filter.PageSize : 10;

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        // Field selection (nếu có include property)
        // if (filter.IncludeProperties.Any())
        // {
        //     var selectString = "new (" + string.Join(",", filter.IncludeProperties) + ")";
        //     var projected = await query.Select(selectString).ToDynamicListAsync();
        //     return new PageResponse<Payment>(projected, totalCount, page, pageSize);
        // }

        // Trả về entity đầy đủ
        var items = await query.ToListAsync();
        return new PageResponse<Payment>(items, totalCount, page, pageSize);
    }
}
