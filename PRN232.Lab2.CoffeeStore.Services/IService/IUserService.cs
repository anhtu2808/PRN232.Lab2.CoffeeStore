using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.User;

namespace PRN232.Lab2.CoffeeStore.Services.IService;

public interface IUserService
{
    Task<PageResponse<UserResponse>> GetUsers(UserFilterRequest filter);
    Task<UserResponse?> GetUserById(Guid id);
    Task<UserResponse?> CreateUser(UserRequest request, Role role);
    Task<UserResponse?> UpdateUser(Guid id, UserRequest request);
    Task<bool> DeleteUser(Guid id);
    
}