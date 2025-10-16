using PRN232.Lab2.CoffeeStore.Models.Enums;
using PRN232.Lab2.CoffeeStore.Models.Request.User;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API;

public static class AppInitializer
{
    public static async Task SeedAdministratorAsync(IHost app)
    {
        // Lấy configuration từ app
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var adminUserConfig = configuration.GetSection("AdminUserSeed");

        // Tạo một service scope để lấy các dịch vụ
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var _userService = serviceProvider.GetRequiredService<IUserService>();
                var filter = new UserFilterRequest
                {
                    Username = adminUserConfig["UserName"]
                };
                var existedAdmin = _userService.GetUsers(filter);
                if (existedAdmin.Result.TotalCount > 0)
                {
                    logger.LogInformation("Administrator user already exists. Skipping seeding.");
                    return;
                }
                
                string username = adminUserConfig["UserName"];
                string password = adminUserConfig["Password"];
                string email = adminUserConfig["Email"];
                var userRequest = new UserRequest()
                {
                    Username = username,
                    Password = password,
                    Email = email
                };
                await _userService.CreateUser(userRequest, Role.Admin);
                logger.LogInformation("Administrator user seeded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the administrator user.");
            }
        }
    }
}