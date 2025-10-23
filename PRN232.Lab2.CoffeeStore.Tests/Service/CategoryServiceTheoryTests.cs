using PRN232.Lab2.CoffeeStore.Models.Exception;

namespace CoffeeStore.Tests.Service;

using Xunit;
using AutoMapper;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Services.Service;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

/// <summary>
/// Demo XUnit Theory with InlineData
/// Theory allows running the same test with different input data
/// </summary>
public class CategoryServiceTheoryTests
{
    private readonly IMapper _mapper;

    public CategoryServiceTheoryTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryResponse>();
        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
    }

    // ============================================
    // [Theory] - Run test with multiple data sets
    // Each [InlineData] creates a new test case
    // ============================================
    [Theory]
    [InlineData(1, "Coffee")]      // Test case 1: id=1, name="Coffee"
    [InlineData(2, "Tea")]         // Test case 2: id=2, name="Tea"
    [InlineData(3, "Juice")]       // Test case 3: id=3, name="Juice"
    public async Task GetByIdAsync_WithDifferentIds_ReturnsCorrectCategory(
        int categoryId, 
        string expectedName)
    {
        // ARRANGE
        // Create stub data based on input parameters
        var stubCategory = new Category 
        { 
            CategoryId = categoryId, 
            Name = expectedName 
        };

        var stubRepo = new Mock<ICategoryRepository>();
        stubRepo.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(stubCategory);

        var stubUow = new Mock<IUnitOfWork>();
        stubUow.Setup(u => u.Categories).Returns(stubRepo.Object);

        var service = new CategoryService(stubUow.Object, _mapper);

        // ACT
        var result = await service.GetByIdAsync(categoryId);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.CategoryId);
        Assert.Equal(expectedName, result.Name);
    }

    // ============================================
    // Another Theory Example - Test validation
    // ============================================
    [Theory]
    [InlineData(0)]      // Invalid: zero
    [InlineData(-1)]     // Invalid: negative
    [InlineData(-100)]   // Invalid: negative
    public async Task GetByIdAsync_WithInvalidId_ThrowsException(int invalidId)
    {
        // ARRANGE
        var stubRepo = new Mock<ICategoryRepository>();
        stubRepo.Setup(r => r.GetByIdAsync(invalidId))
                .ReturnsAsync((Category?)null);  // Return null for invalid IDs

        var stubUow = new Mock<IUnitOfWork>();
        stubUow.Setup(u => u.Categories).Returns(stubRepo.Object);

        var service = new CategoryService(stubUow.Object, _mapper);

        // ACT & ASSERT
        // Verify exception is thrown for invalid IDs
        await Assert.ThrowsAsync<AppException>(
            async () => await service.GetByIdAsync(invalidId)
        );
    }

    // ============================================
    // Theory with multiple parameters
    // ============================================
    [Theory]
    [InlineData(1, "Coffee", "Hot coffee drinks", true)]
    [InlineData(2, "Tea", "Various tea types", true)]
    [InlineData(3, "Juice", null, false)]  // null description
    public async Task GetByIdAsync_WithCompleteData_MapsAllProperties(
        int id, 
        string name, 
        string description, 
        bool hasDescription)
    {
        // ARRANGE
        var stubCategory = new Category 
        { 
            CategoryId = id,
            Name = name,
            Description = description
        };

        var stubRepo = new Mock<ICategoryRepository>();
        stubRepo.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(stubCategory);

        var stubUow = new Mock<IUnitOfWork>();
        stubUow.Setup(u => u.Categories).Returns(stubRepo.Object);

        var service = new CategoryService(stubUow.Object, _mapper);

        // ACT
        var result = await service.GetByIdAsync(id);

        // ASSERT
        Assert.Equal(id, result.CategoryId);
        Assert.Equal(name, result.Name);
        
        if (hasDescription)
        {
            Assert.NotNull(result.Description);
            Assert.Equal(description, result.Description);
        }
        else
        {
            Assert.Null(result.Description);
        }
    }
}
