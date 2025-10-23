using AutoMapper;
using CoffeeStore.Tests.Repository;
using Microsoft.Extensions.Logging.Abstractions;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Services.Service;

namespace CoffeeStore.Tests.Service;

/// <summary>
/// Test using Fake Repository (no Moq framework)
/// </summary>
public class CategoryServiceFakeTests
{
    private readonly IMapper _mapper;

    public CategoryServiceFakeTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryResponse>();
        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetByIdAsync_WithFake_ReturnsExistingCategory()
    {
        // ARRANGE
        // Create Fake Repository - NO Moq, manual implementation
        var fakeRepo = new FakeCategoryRepository();
        var fakeUow = new FakeUnitOfWork(fakeRepo);
        var service = new CategoryService(fakeUow, _mapper);

        // ACT
        // Get category with id = 1 (exists in fake data)
        var result = await service.GetByIdAsync(1);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Coffee", result.Name);
        Assert.Equal("Hot coffee drinks", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WithFake_ReturnsNull_WhenNotFound()
    {
        // ARRANGE
        var fakeRepo = new FakeCategoryRepository();
        var fakeUow = new FakeUnitOfWork(fakeRepo);
        var service = new CategoryService(fakeUow, _mapper);

        // ACT
        // Get category with id = 999 (does not exist in fake data)
        var exception = await Assert.ThrowsAsync<AppException>(
            async () => await service.GetByIdAsync(999)
        );

        // ASSERT
        Assert.Equal("Category not found.", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithFake_ReturnsDifferentCategories()
    {
        // ARRANGE
        var fakeRepo = new FakeCategoryRepository();
        var fakeUow = new FakeUnitOfWork(fakeRepo);
        var service = new CategoryService(fakeUow, _mapper);

        // ACT
        var coffee = await service.GetByIdAsync(1);
        var tea = await service.GetByIdAsync(2);
        var juice = await service.GetByIdAsync(3);

        // ASSERT
        Assert.Equal("Coffee", coffee.Name);
        Assert.Equal("Tea", tea.Name);
        Assert.Equal("Juice", juice.Name);
    }
    
}