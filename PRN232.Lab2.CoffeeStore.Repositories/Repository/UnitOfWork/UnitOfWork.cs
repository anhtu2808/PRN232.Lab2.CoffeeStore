// using System.Collections;
// using PRN232.Lab2.CoffeeStore.Repositories.Entity;
//
// namespace PRN232.Lab2.CoffeeStore.Repositories.Repository;
//
// public class UnitOfWork : IUnitOfWork
// {
//     private readonly CoffeeStoreDbContext _context;
//     private readonly Hashtable _repos = new();
//
//     public UnitOfWork(CoffeeStoreDbContext context)
//     {
//         _context = context;
//     }
//
//     public IGenericRepository<T, TId> GetRepository<T, TId>()
//         where T : class
//         where TId : notnull
//     {
//         var typeName = typeof(T).Name;
//         if (_repos.ContainsKey(typeName))
//             return (IGenericRepository<T, TId>)_repos[typeName]!;
//
//         var repoInstance = new GenericRepository<T, TId>(_context);
//         _repos.Add(typeName, repoInstance);
//         return repoInstance;
//     }
//
//     public async Task<int> SaveChangesAsync() =>
//         await _context.SaveChangesAsync();
// }