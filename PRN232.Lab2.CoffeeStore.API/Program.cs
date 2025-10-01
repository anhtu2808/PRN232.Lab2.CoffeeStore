using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.API.middleware;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.Repository;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;
using PRN232.Lab2.CoffeeStore.Services.MappingProfile;
using PRN232.Lab2.CoffeeStore.Services.Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<CoffeeStoreDbContext>(options =>
    options.UseSqlServer(connectionString));


// Custom validate request model
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => char.ToLowerInvariant(kvp.Key[0]) + kvp.Key.Substring(1),
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new ApiResponse<object>
            {
                StatusCode = 400,
                Message = "Validation failed",
                Data = errors
            };

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DI Mapper
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(ProductProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(UserProfile).Assembly); });

// DI Repository
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// DI Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Cấu hình pipeline cho HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();

public interface IUserService
{
}