using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Services.Service;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;

namespace CoffeeStore.Tests.Service;

public class CategoryServiceMockTests
{
    private readonly IMapper _mapper;

    public CategoryServiceMockTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryResponse>();
        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCalled_VerifiesRepositoryMethodIsCalledOnce()
    {
        // ===== ARRANGE =====
        
        // 1. Tạo dữ liệu test
        var testCategory = new Category 
        { 
            CategoryId = 1, 
            Name = "Coffee",
            Description = "All coffee products"
        };

        // 2. Tạo MOCK cho ICategoryRepository
        // Mock khác Stub: Mock sẽ được dùng để VERIFY BEHAVIOR
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        mockCategoryRepo
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(testCategory);

        // 3. Tạo Mock cho IUnitOfWork
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork
            .Setup(uow => uow.Categories)
            .Returns(mockCategoryRepo.Object);

        // 4. Tạo service với mock dependencies
        var categoryService = new CategoryService(mockUnitOfWork.Object, _mapper);

        // ===== ACT =====
        var result = await categoryService.GetByIdAsync(1);

        // ===== ASSERT =====
        
        // Assert 1: Kiểm tra OUTPUT (giống như Stub)
        Assert.NotNull(result);
        Assert.Equal("Coffee", result.Name);

        // Assert 2: VERIFY BEHAVIOR - Đây là điểm khác biệt của MOCK
        // Kiểm tra xem GetByIdAsync có được gọi ĐÚNG 1 LẦN với tham số id = 1 không
        mockCategoryRepo.Verify(
            repo => repo.GetByIdAsync(1),  // Method và parameter cần verify
            Times.Once()                    // Số lần được gọi (Once = 1 lần)
        );

        // ĐẶC ĐIỂM MOCK: Nếu method KHÔNG được gọi hoặc gọi SAI số lần => Test FAIL
        // Stub thì không quan tâm method có được gọi hay không
    }
}