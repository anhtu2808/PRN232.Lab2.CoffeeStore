using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.API.Configuration;
using PRN232.Lab2.CoffeeStore.API.middleware;
using PRN232.Lab2.CoffeeStore.API.Utils;
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

builder.Services.AddHttpContextAccessor();

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<CoffeeStoreDbContext>(options =>
    options.UseSqlServer(connectionString));


// Custom validate request model
builder.Services.AddCustomApiBehavior();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwt();

//DI Mapper
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(PageMapping).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(ProductProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(UserProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(CategoryProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(OrderProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(OrderDetailProfile).Assembly); });
builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(PaymentProfile).Assembly); });

// DI Repository
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();


// DI Service
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// DI Utils
builder.Services.AddScoped<JwtUtils>();

//Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

// Cấu hình pipeline cho HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();